using Mono.Cecil;
using Mono.Cecil.Cil;
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

        #region アスペクト (カスタムコード) の注入

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        protected sealed override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {
            var methodInjector     = new MethodInjector(method, aspect);
            var aspectArgsInjector = new MethodInterceptionArgsInjector(method, aspect);

            var asyncStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
            var isInvokeAsyncOverridden    = methodInjector.AspectType.Methods.Any(m => m.Name == nameof(OnInvokeAsync));
            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                aspectArgsInjector.CreateDerivedAspectArgs();
                ReplaceAsyncMethod(methodInjector);
                aspectArgsInjector.OverrideInvokeAsyncMethod(methodInjector.OriginalMethod);
                aspectArgsInjector.OverrideProceedAsyncMethod(methodInjector.OriginalMethod);
            }
            else
            {
                aspectArgsInjector.CreateDerivedAspectArgs();
                ReplaceMethod(methodInjector);
                aspectArgsInjector.OverrideInvokeMethod(methodInjector.OriginalMethod);
                aspectArgsInjector.OverrideProceedMethod(methodInjector.OriginalMethod);
            }
        }

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceMethod(MethodInjector injector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            injector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                var method = injector.TargetMethod;
                method.Body = new Mono.Cecil.Cil.MethodBody(method);
                var declaringType         = method.DeclaringType;
                var derivedAspectArgsType = declaringType.NestedTypes.Single(nt => nt.Name == method.FullName + "+" + nameof(MethodInterceptionArgs));
                var processor             = method.Body.GetILProcessor();

                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvoke(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                injector.CreateAspectVariable(processor);
                injector.CreateArgumentsVariable(processor);
                injector.CreateAspectArgsVariable(processor, derivedAspectArgsType);
                injector.SetMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnInvoke));
                injector.UpdateArguments(processor);
                injector.Return(processor);
            }
        }

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceAsyncMethod(MethodInjector injector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            injector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                var method = injector.TargetMethod;
                method.Body = new Mono.Cecil.Cil.MethodBody(method);
                var declaringType         = method.DeclaringType;
                var derivedAspectArgsType = declaringType.NestedTypes.Single(nt => nt.Name == method.FullName + "+" + nameof(MethodInterceptionArgs));
                var processor             = method.Body.GetILProcessor();

                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvokeAsync(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                injector.CreateAspectVariable(processor);
                injector.CreateArgumentsVariable(processor);
                injector.CreateAspectArgsVariable(processor, derivedAspectArgsType);
                injector.SetMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnInvokeAsync));
                processor.Emit(OpCodes.Pop);
                injector.Return(processor);
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
            return null;
        }

        #endregion

        #endregion
    }
}
