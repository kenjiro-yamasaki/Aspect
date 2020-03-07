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
        /// <param name="method">対象メソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        protected sealed override void InjectAdvice(MethodDefinition method, CustomAttribute aspectAttribute)
        {
            var asyncStateMachineAttribute = method.GetAsyncStateMachineAttribute();
            var isInvokeAsyncOverridden    = aspectAttribute.AttributeType.Resolve().Methods.Any(m => m.Name == nameof(OnInvokeAsync));
            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                var methodInjector     = new AsyncMethodRewriter(method, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsRewriter(method, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceAsyncMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeAsyncImplMethod(methodInjector.OriginalTargetMethod);
                aspectArgsInjector.OverrideTaskResultProperty();

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspectAttribute);
                method.CustomAttributes.Remove(asyncStateMachineAttribute);
            }
            else
            {
                var methodInjector     = new MethodRewriter(method, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsRewriter(method, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeImplMethod(methodInjector.OriginalTargetMethod);

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspectAttribute);
            }
        }

        /// <summary>
        /// 対象メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">対象メソッドへの注入。</param>
        /// <param name="aspectArgsInjector">アスペクト引数への注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
        /// 対象メソッドのコードを、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodRewriter)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceMethod(MethodRewriter rewriter, MethodInterceptionArgsRewriter aspectArgsInjector)
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
                /// return (TResult)aspectArgs.ReturnValue;
                var processor = rewriter.Processor;

                processor.NewAspectAttribute(rewriter.AspectAttribute);
                int aspectAttributeVariable = processor.StoreLocal(rewriter.AspectAttribueType);

                processor.NewArguments();
                int argumentsVariable = processor.StoreLocal(rewriter.TargetMethod.ArgumentsType());

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
                int aspectArgsVariable = processor.StoreLocal(typeof(MethodInterceptionArgs));

                processor.Load(aspectArgsVariable);
                processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
                //rewriter.LoadTargetMethod();
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));


                //rewriter.NewAspectAttributeVariable();
                //rewriter.NewArgumentsVariable();
                //rewriter.NewAspectArgsVariable(aspectArgsInjector.DerivedAspectArgsType);
                //rewriter.StoreMethodProperty(rewriter.LoadTargetMethod, aspectArgsVariable);

                processor.Load(aspectAttributeVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(rewriter.AspectAttribueType, nameof(OnInvoke));
                //rewriter.InvokeAspectHandler(nameof(OnInvoke));

                rewriter.UpdateArguments(pointerOnly: true, argumentsVariable);
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
            {
                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodInterceptionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();

                var processor = rewriter.Processor;

                processor.NewAspectAttribute(rewriter.AspectAttribute);
                int aspectAttributeVariable = processor.StoreLocal(rewriter.AspectAttribueType);

                processor.NewArguments();
                int argumentsVariable = processor.StoreLocal(rewriter.TargetMethod.ArgumentsType());

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
                int aspectArgsVariable = processor.StoreLocal(typeof(MethodExecutionArgs));

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
