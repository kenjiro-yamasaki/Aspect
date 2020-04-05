using Mono.Cecil;
using System;
using System.Linq;
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
        sealed public override void InjectAdvice()
        {
            using var profile = Profiling.Profiler.Start($"{nameof(MethodInterceptionAspect)}.{nameof(InjectAdvice)}");

            //
            var originalTargetMethod = CreateOriginalTargetMethod();
            TargetMethod.DebugInformation.SequencePoints.Clear();
            TargetMethod.Body = new Mono.Cecil.Cil.MethodBody(TargetMethod);

            //
            var asyncStateMachineAttribute = TargetMethod.GetAsyncStateMachineAttribute();
            var isInvokeAsyncOverridden = CustomAttribute.AttributeType.Resolve().Methods.Any(m => m.Name == nameof(OnInvokeAsync));
            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                var rewriter = new MethodInterceptionArgsRewriter(TargetMethod, CustomAttribute);

                rewriter.CreateAspectArgsImpl();
                rewriter.CreateConstructor();
                ReplaceAsyncMethod(rewriter.AspectArgsImplType);
                rewriter.OverrideInvokeAsyncImplMethod(originalTargetMethod);
                rewriter.OverrideTaskResultProperty();
            }
            else
            {
                var rewriter = new MethodInterceptionArgsRewriter(TargetMethod, CustomAttribute);

                rewriter.CreateAspectArgsImpl();
                rewriter.CreateConstructor();
                ReplaceMethod(rewriter.AspectArgsImplType);
                rewriter.OverrideInvokeImplMethod(originalTargetMethod);
            }
        }

        /// <summary>
        /// オリジナルターゲットメソッド (ターゲットメソッドの元々のコード) を生成します。
        /// </summary>
        /// <seealso cref="OriginalTargetMethod"/>
        public MethodDefinition CreateOriginalTargetMethod()
        {
            var methodAttribute = TargetMethod.Attributes & ~(Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName);

            var methodName = $"*{TargetMethod.Name}";
            for (int number = 2; true; number++)
            {
                if (!TargetMethod.DeclaringType.Methods.Any(m => m.Name == methodName))
                {
                    break;
                }
                methodName = $"*{TargetMethod.Name}<{number}>";
            }

            var originalTargetMethod = new MethodDefinition(methodName, methodAttribute, TargetMethod.ReturnType);
            originalTargetMethod.Body = TargetMethod.Body;
            foreach (var parameter in TargetMethod.Parameters)
            {
                originalTargetMethod.Parameters.Add(parameter);
            }
            foreach (var sequencePoint in TargetMethod.DebugInformation.SequencePoints)
            {
                originalTargetMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }
            TargetMethod.DeclaringType.Methods.Add(originalTargetMethod);

            return originalTargetMethod;
        }

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">ターゲットメソッドの書き換え。</param>
        /// <param name="aspectArgsImplType">アスペクト引数の型。</param>
        private void ReplaceMethod(TypeDefinition aspectArgsImplType)
        {
            var processor               = TargetMethod.Body.GetILProcessor();
            var aspectAttributeVariable = TargetMethod.AddVariable(CustomAttributeType);
            var argumentsVariable       = TargetMethod.AddVariable(typeof(Arguments));
            var aspectArgsVariable      = TargetMethod.AddVariable(typeof(MethodInterceptionArgs));

            // var aspectAttribute = new AspectAttribute(...) {...};
            // var arguments       = new Arguments(...);
            // var aspectArgs      = new MethodInterceptionArgs(this, arguments);
            // aspectArgs.Method = MethodBase.GetCurrentMethod();
            // aspectAttribute.OnInvoke(aspectArgs);
            // arg0 = (TArg0)arguments[0];
            // arg1 = (TArg1)arguments[1];
            // ...
            // return (TResult)aspectArgs.ReturnValue;
            processor.NewAspectAttribute(CustomAttribute);
            processor.Store(aspectAttributeVariable);

            processor.NewArguments();
            processor.Store(argumentsVariable);

            if (TargetMethod.IsStatic)
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
            processor.LoadMethodBase(TargetMethod);
            processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

            //
            processor.Load(aspectAttributeVariable);
            processor.Load(aspectArgsVariable);
            processor.CallVirtual(CustomAttributeType, nameof(OnInvoke));
            processor.UpdateArguments(argumentsVariable, pointerOnly: true);

            if (TargetMethod.HasReturnValue())
            {
                processor.Load(aspectArgsVariable);
                processor.GetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                processor.Unbox(TargetMethod.ReturnType);
            }
            processor.Return();
        }

        /// <summary>
        /// 非同期メソッドを書き換えます。
        /// </summary>
        /// <param name="aspectArgsImplType">具体アスペクト引数の型。</param>
        private void ReplaceAsyncMethod(TypeDefinition aspectArgsImplType)
        {
            var targetMethod            = TargetMethod;
            var module                  = targetMethod.Module;
            var processor               = targetMethod.Body.GetILProcessor();
            var aspectAttributeType     = CustomAttributeType;
            var aspectAttributeVariable = targetMethod.AddVariable(aspectAttributeType);
            var argumentsVariable       = targetMethod.AddVariable(typeof(Arguments));
            var aspectArgsVariable      = targetMethod.AddVariable(typeof(MethodExecutionArgs));

            // var aspectAttribute = new AspectAttribute(...) {...};
            // var arguments       = new Arguments(...);
            // var aspectArgs      = new MethodInterceptionArgs(this, arguments);
            // aspectArgs.Method = MethodBase.GetCurrentMethod();
            processor.NewAspectAttribute(CustomAttribute);
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
            processor.LoadMethodBase(TargetMethod);
            processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

            // var stateMachine = new MethodInterceptionAsyncStateMachine(aspectAttribute, aspectArgs);
            // var builder      = stateMachine.Builder;
            // builder.Start(ref stateMachine);
            // return stateMachine.Builder.Task;
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
