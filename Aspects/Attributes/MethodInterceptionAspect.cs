﻿using Mono.Cecil;
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
                var methodInjector     = new AsyncMethodInjector(method, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsInjector(method, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceAsyncMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeAsyncImplMethod(methodInjector.OriginalMethod);
                aspectArgsInjector.OverrideTaskResultProperty();

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspectAttribute);
                method.CustomAttributes.Remove(asyncStateMachineAttribute);
            }
            else
            {
                var methodInjector     = new MethodInjector(method, aspectAttribute);
                var aspectArgsInjector = new MethodInterceptionArgsInjector(method, aspectAttribute);

                aspectArgsInjector.CreateAspectArgsImpl();
                aspectArgsInjector.CreateConstructor();
                ReplaceMethod(methodInjector, aspectArgsInjector);
                aspectArgsInjector.OverrideInvokeImplMethod(methodInjector.OriginalMethod);

                /// アスペクト属性を削除します。
                method.CustomAttributes.Remove(aspectAttribute);
            }
        }

        /// <summary>
        /// 対象メソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">対象メソッドへの注入。</param>
        /// <param name="aspectArgsInjector">アスペクト引数への注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
        /// 対象メソッドのコードを、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceMethod(MethodInjector methodInjector, MethodInterceptionArgsInjector aspectArgsInjector)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            methodInjector.CopyMethod();

            /// 対象メソッドのコードを書き換えます。
            {
                /// var aspect     = new Aspect(...) { ... };
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodInterceptionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvoke(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                methodInjector.NewAspectAttribute();
                methodInjector.NewArguments();
                methodInjector.NewAspectArgs(aspectArgsInjector.DerivedAspectArgsType);
                methodInjector.SetMethod();
                methodInjector.InvokeEventHandler(nameof(OnInvoke));
                methodInjector.SetAspectArguments(pointerOnly: true);
                methodInjector.Return();
            }
        }

        /// <summary>
        /// 非同期メソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">対象メソッドへの注入。</param>
        /// <param name="aspectArgsInjector">アスペクト引数への注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
        /// 対象メソッドのコードを、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceAsyncMethod(AsyncMethodInjector methodInjector, MethodInterceptionArgsInjector aspectArgsInjector)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            methodInjector.CopyMethod();

            /// 対象メソッドのコードを書き換えます。
            {
                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodInterceptionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                methodInjector.NewAspectAttribute();
                methodInjector.NewArguments();
                methodInjector.NewAspectArgs(aspectArgsInjector.DerivedAspectArgsType);
                methodInjector.SetMethod();

                var taskType = methodInjector.TargetMethod.ReturnType;
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
                    methodInjector.StartAsyncStateMachine(stateMachineType, aspectType, aspectArgsType);
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
