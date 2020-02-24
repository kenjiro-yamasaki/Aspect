using Mono.Cecil;
using System;
using System.Linq;
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

        #region アスペクト (カスタムコード) の注入

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        protected sealed override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {

            var asyncStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
            var isInvokeAsyncOverridden    = aspect.AttributeType.Resolve().Methods.Any(m => m.Name == nameof(OnInvokeAsync));
            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                var methodInjector     = new AsyncMethodInjector(method, aspect);
                var aspectArgsInjector = new MethodInterceptionArgsInjector(method, aspect);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceAsyncMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeAsyncImplMethod(methodInjector.OriginalMethod);
                aspectArgsInjector.OverrideTaskResultProperty();

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspect);
                method.CustomAttributes.Remove(asyncStateMachineAttribute);
            }
            else
            {
                var methodInjector     = new MethodInjector(method, aspect);
                var aspectArgsInjector = new MethodInterceptionArgsInjector(method, aspect);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeImplMethod(methodInjector.OriginalMethod);

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspect);
            }
        }

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceMethod(MethodInjector methodInjector, MethodInterceptionArgsInjector aspectArgsInjector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            methodInjector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvoke(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                methodInjector.CreateAspectVariable();
                methodInjector.CreateArgumentsVariable();
                methodInjector.CreateAspectArgsVariable(aspectArgsInjector.DerivedAspectArgsType);
                methodInjector.SetMethod();
                methodInjector.InvokeEventHandler(nameof(OnInvoke));
                methodInjector.SetAspectArguments(pointerOnly: true);
                methodInjector.Return();
            }
        }

        /// <summary>
        /// 注入対象の非同期メソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceAsyncMethod(AsyncMethodInjector methodInjector, MethodInterceptionArgsInjector aspectArgsInjector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            methodInjector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                methodInjector.CreateAspectVariable();
                methodInjector.CreateArgumentsVariable();
                methodInjector.CreateAspectArgsVariable(aspectArgsInjector.DerivedAspectArgsType);
                methodInjector.SetMethod();

                /// var stateMachine = new MethodInterceptionAsyncStateMachine<TResult>(aspect, aspectArgs);
                /// AsyncTaskMethodBuilder<string> builder = stateMachine.Builder;
                /// builder.Start(ref stateMachine);
                /// return stateMachine.Builder.Task;
                var taskType = methodInjector.TargetMethod.ReturnType;
                if (taskType is GenericInstanceType genericInstanceType)
                {
                    var returnType       = genericInstanceType.GenericArguments[0].ToSystemType();
                    var stateMachineType = typeof(MethodInterceptionAsyncStateMachine<>).MakeGenericType(returnType);
                    var aspectType       = typeof(MethodInterceptionAspect);
                    var aspectArgsType   = typeof(MethodInterceptionArgs);
                    methodInjector.StartAsyncStateMachine(stateMachineType, aspectType, aspectArgsType);
                }
                else
                {
                    var stateMachineType = typeof(MethodInterceptionAsyncStateMachine);
                    var aspectType       = typeof(MethodInterceptionAspect);
                    var aspectArgsType   = typeof(MethodInterceptionArgs);
                    methodInjector.StartAsyncStateMachine(stateMachineType, aspectType, aspectArgsType);
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
