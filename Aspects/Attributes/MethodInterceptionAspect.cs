using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプトアスペクト。
    /// </summary>
    public abstract class MethodInterceptionAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected MethodInterceptionAspect()
        {
        }

        #endregion

        #region メソッド

        #region アドバイスの注入

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        sealed public override void InjectAdvice(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
        {
            var asyncStateMachineAttribute = targetMethod.GetAsyncStateMachineAttribute();
            var isInvokeAsyncOverridden    = aspectAttribute.AttributeType.Resolve().Methods.Any(m => m.Name == nameof(OnInvokeAsync));

            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                var targetMethodRewriter = new MethodRewriter(targetMethod, aspectAttribute);
                var aspectArgsRewriter   = new MethodInterceptionArgsRewriter(targetMethod, aspectAttribute);

                targetMethodRewriter.CreateOriginalTargetMethod();

                aspectArgsRewriter.CreateAspectArgsImpl();
                aspectArgsRewriter.CreateConstructor();
                ReplaceAsyncMethod(targetMethodRewriter, aspectArgsRewriter.AspectArgsImplType);
                aspectArgsRewriter.OverrideInvokeAsyncImplMethod(targetMethodRewriter.OriginalTargetMethod);
                aspectArgsRewriter.OverrideTaskResultProperty();

                /// アスペクト属性を削除します。
                targetMethod.CustomAttributes.Remove(aspectAttribute);
                targetMethod.CustomAttributes.Remove(asyncStateMachineAttribute);
            }
            else
            {
                var targetMethodRewriter = new MethodRewriter(targetMethod, aspectAttribute);
                var aspectArgsRewriter   = new MethodInterceptionArgsRewriter(targetMethod, aspectAttribute);

                targetMethodRewriter.CreateOriginalTargetMethod();

                aspectArgsRewriter.CreateAspectArgsImpl();
                aspectArgsRewriter.CreateConstructor();
                ReplaceMethod(targetMethodRewriter, aspectArgsRewriter.AspectArgsImplType);
                aspectArgsRewriter.OverrideInvokeImplMethod(targetMethodRewriter.OriginalTargetMethod);

                /// アスペクト属性を削除します。
                targetMethod.CustomAttributes.Remove(aspectAttribute);
            }
        }

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">ターゲットメソッドの書き換え。</param>
        /// <param name="aspectArgsImplType">アスペクト引数の型。</param>
        private void ReplaceMethod(MethodRewriter rewriter, TypeDefinition aspectArgsImplType)
        {
            var targetMethod            = rewriter.TargetMethod;
            var processor               = targetMethod.Body.GetILProcessor();
            var aspectAttributeType     = rewriter.AspectAttributeType;
            var aspectAttributeVariable = targetMethod.AddVariable(aspectAttributeType);
            var argumentsVariable       = targetMethod.AddVariable(targetMethod.ArgumentsType());
            var aspectArgsVariable      = targetMethod.AddVariable(typeof(MethodInterceptionArgs));

            /// var aspectAttribute = new AspectAttribute(...) {...};
            /// var arguments       = new Arguments(...);
            /// var aspectArgs      = new MethodInterceptionArgs(this, arguments);
            /// aspectArgs.Method = MethodBase.GetCurrentMethod();
            /// aspectAttribute.OnInvoke(aspectArgs);
            /// arg0 = (TArg0)arguments[0];
            /// arg1 = (TArg1)arguments[1];
            /// ...
            /// return (TResult)aspectArgs.ReturnValue;
            processor.NewAspectAttribute(rewriter.AspectAttribute);
            processor.Store(aspectAttributeVariable);

            processor.NewArguments();
            processor.Store(argumentsVariable);

            if (rewriter.TargetMethod.IsStatic)
            {
                processor.LoadNull();
            }
            else
            {
                processor.LoadThis();
            }
            processor.Load(argumentsVariable);
            processor.New(aspectArgsImplType);
            processor.Store(aspectArgsVariable);

            processor.Load(aspectArgsVariable);
            processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
            processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

            processor.Load(aspectAttributeVariable);
            processor.Load(aspectArgsVariable);
            processor.CallVirtual(typeof(MethodInterceptionAspect), nameof(OnInvoke));
            processor.UpdateArguments(argumentsVariable, pointerOnly: true);

            if (rewriter.TargetMethod.HasReturnValue())
            {
                processor.Load(aspectArgsVariable);
                processor.GetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                processor.Unbox(rewriter.TargetMethod.ReturnType);
            }
            processor.Return();
        }

        /// <summary>
        /// 非同期メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">対象メソッドへの注入。</param>
        /// <param name="aspectArgsInjector">アスペクト引数への注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
        /// 対象メソッドのコードを、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodRewriter)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceAsyncMethod(MethodRewriter rewriter, TypeDefinition aspectArgsImplType)
        {
            var targetMethod            = rewriter.TargetMethod;
            var module                  = targetMethod.Module;
            var processor               = targetMethod.Body.GetILProcessor();
            var aspectAttributeType     = rewriter.AspectAttributeType;
            var aspectAttributeVariable = targetMethod.AddVariable(aspectAttributeType);
            var argumentsVariable       = targetMethod.AddVariable(targetMethod.ArgumentsType());
            var aspectArgsVariable      = targetMethod.AddVariable(typeof(MethodExecutionArgs));

            /// var aspectAttribute = new AspectAttribute(...) {...};
            /// var arguments       = new Arguments(...);
            /// var aspectArgs      = new MethodInterceptionArgs(this, arguments);
            /// aspectArgs.Method = MethodBase.GetCurrentMethod();
            processor.NewAspectAttribute(rewriter.AspectAttribute);
            processor.Store(aspectAttributeVariable);

            processor.NewArguments();
            processor.Store(argumentsVariable);

            if (targetMethod.IsStatic)
            {
                processor.LoadNull();
            }
            else
            {
                processor.LoadThis();
            }
            processor.Load(argumentsVariable);
            processor.New(aspectArgsImplType);
            processor.Store(aspectArgsVariable);

            processor.Load(aspectArgsVariable);
            processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
            processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

            /// var stateMachine = new MethodInterceptionAsyncStateMachine(aspectAttribute, aspectArgs);
            /// var builder      = stateMachine.Builder;
            /// builder.Start(ref stateMachine);
            /// return stateMachine.Builder.Task;
            Type stateMachineType;
            Type builderType;
            if (targetMethod.ReturnType is GenericInstanceType taskType)
            {
                var returnType   = taskType.GenericArguments[0].ToSystemType();
                stateMachineType = typeof(MethodInterceptionAsyncStateMachine<>).MakeGenericType(returnType);
                builderType      = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);
            }
            else
            {
                stateMachineType = typeof(MethodInterceptionAsyncStateMachine);
                builderType      = typeof(AsyncTaskMethodBuilder);
            }
            var stateMachineVariable = targetMethod.AddVariable(stateMachineType);
            var builderVariable      = targetMethod.AddVariable(builderType);

            processor.Load(aspectAttributeVariable);
            processor.Load(aspectArgsVariable);
            processor.New(stateMachineType, GetType(), typeof(MethodInterceptionArgs));
            processor.Store(stateMachineVariable);

            processor.Load(stateMachineVariable);
            processor.Load(module.ImportReference(stateMachineType.GetField("Builder")));
            processor.Store(builderVariable);

            processor.LoadAddress(builderVariable);
            processor.LoadAddress(stateMachineVariable);
            processor.Call(module.ImportReference(builderType.GetMethod("Start").MakeGenericMethod(stateMachineType)));

            processor.Load(stateMachineVariable);
            processor.LoadAddress(module.ImportReference(stateMachineType.GetField("Builder")));
            processor.Call(module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
            processor.Return();
        }

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual void OnInvoke(MethodInterceptionArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
