using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
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
                var methodInjector     = new AsyncMethodRewriter(targetMethod, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsRewriter(targetMethod, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceAsyncMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeAsyncImplMethod(methodInjector.OriginalTargetMethod);
                aspectArgsInjector.OverrideTaskResultProperty();

                /// アスペクト属性を削除します。
                targetMethod.CustomAttributes.Remove(aspectAttribute);
                targetMethod.CustomAttributes.Remove(asyncStateMachineAttribute);
            }
            else
            {
                var methodInjector     = new MethodRewriter(targetMethod, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsRewriter(targetMethod, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceMethod(methodInjector, aspectArgsInjector.DerivedAspectArgsType);
                aspectArgsInjector.OverrideInvokeImplMethod(methodInjector.OriginalTargetMethod);

                /// アスペクト属性を削除します。
                targetMethod.CustomAttributes.Remove(aspectAttribute);
            }
        }

        /// <summary>
        /// 対象メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">ターゲットメソッドの書き換え。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        private void ReplaceMethod(MethodRewriter rewriter, TypeDefinition aspectArgsType)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            rewriter.CreateOriginalTargetMethod();

            /// 対象メソッドのコードを書き換えます。
            {
                /// var aspect     = new Aspect(...) { ... };
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodInterceptionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvoke(aspectArgs);
                /// arg0 = (TArg0)arguments[0];
                /// arg1 = (TArg1)arguments[1];
                /// ...
                /// return (TResult)aspectArgs.ReturnValue;
                var targetMethod            = rewriter.TargetMethod;
                var processor               = targetMethod.Body.GetILProcessor();

                var aspectAttributeVariable = targetMethod.AddVariable(rewriter.AspectAttributeType);
                var argumentsVariable       = targetMethod.AddVariable(rewriter.TargetMethod.ArgumentsType());
                var aspectArgsVariable      = targetMethod.AddVariable(typeof(MethodInterceptionArgs));

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
                processor.New(aspectArgsType);
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
        private void ReplaceAsyncMethod(AsyncMethodRewriter rewriter, MethodInterceptionArgsRewriter aspectArgsInjector)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            rewriter.CreateOriginalTargetMethod();

            /// 対象メソッドのコードを書き換えます。
            var method = rewriter.TargetMethod;
            {
                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodInterceptionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();

                var processor = rewriter.Processor;

                processor.NewAspectAttribute(rewriter.AspectAttribute);
                int aspectAttributeVariable = method.AddVariable(rewriter.AspectAttributeType);
                processor.Store(aspectAttributeVariable);

                processor.NewArguments();
                int argumentsVariable = method.AddVariable(rewriter.TargetMethod.ArgumentsType());
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
                processor.New(aspectArgsInjector.DerivedAspectArgsType);
                int aspectArgsVariable = method.AddVariable(typeof(MethodExecutionArgs));
                processor.Store(aspectArgsVariable);

                processor.Load(aspectArgsVariable);
                processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

                var taskType = rewriter.TargetMethod.ReturnType;
                if (taskType is GenericInstanceType genericInstanceType)
                {
                    /// var stateMachine = new MethodInterceptionAsyncStateMachine<TResult>(aspect, aspectArgs);
                    /// var builder      = stateMachine.Builder;
                    /// builder.Start(ref stateMachine);
                    /// return stateMachine.Builder.Task;
                    var returnType       = genericInstanceType.GenericArguments[0].ToSystemType();
                    var stateMachineType = typeof(MethodInterceptionAsyncStateMachine<>).MakeGenericType(returnType);
                    var aspectType       = typeof(MethodInterceptionAspect);
                    var aspectArgsType   = typeof(MethodInterceptionArgs);
                    rewriter.StartAsyncStateMachine(stateMachineType, aspectType, aspectArgsType, aspectAttributeVariable, aspectArgsVariable);
                }
                else
                {
                    /// var stateMachine = new MethodInterceptionAsyncStateMachine(aspect, aspectArgs);
                    /// var builder      = stateMachine.Builder;
                    /// builder.Start(ref stateMachine);
                    /// return stateMachine.Builder.Task;
                    var stateMachineType = typeof(MethodInterceptionAsyncStateMachine);
                    var aspectType       = typeof(MethodInterceptionAspect);
                    var aspectArgsType   = typeof(MethodInterceptionArgs);
                    rewriter.StartAsyncStateMachine(stateMachineType, aspectType, aspectArgsType, aspectAttributeVariable, aspectArgsVariable);
                }
            }
        }

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual void OnInvoke(MethodInterceptionArgs args)
        {
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
