using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド境界アスペクト。
    /// </summary>
    public abstract class OnMethodBoundaryAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected OnMethodBoundaryAspect()
        {
        }

        #endregion

        #region メソッド

        #region アドバイスの注入

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="method">対象メソッド。</param>
        /// <param name="aspect">アスペクト属性。</param>
        sealed protected override void InjectAdvice(MethodDefinition method, CustomAttribute aspect)
        {
            var iteratorStateMachineAttribute = method.GetIteratorStateMachineAttribute();
            var asyncStateMachineAttribute    = method.GetAsyncStateMachineAttribute();

            if (iteratorStateMachineAttribute != null)
            {
                var stateMachineInjector = new IteratorStateMachineInjector(method, aspect, typeof(MethodExecutionArgs));

                stateMachineInjector.NewAspectAttribute();
                RewriteMoveNextMethod(stateMachineInjector);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var stateMachineInjector = new AsyncStateMachineInjector(method, aspect, typeof(MethodExecutionArgs));

                stateMachineInjector.NewAspectAttribute();
                RewriteMoveNextMethod(stateMachineInjector);
            }
            else
            {
                var methodInjector = new MethodInjector(method, aspect);

                RewriteTargetMethod(methodInjector);
            }
        }

        #region 通常のメソッド

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">ターゲットメソッドへの注入。</param>
        private void RewriteTargetMethod(MethodInjector methodInjector)
        {
            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnEntry(aspectArgs);
                methodInjector.NewAspectAttribute();
                methodInjector.NewArguments();
                methodInjector.NewAspectArgs(methodInjector.Module.ImportReference(typeof(MethodExecutionArgs)));
                methodInjector.SetMethod();
                methodInjector.InvokeEventHandler(nameof(OnEntry));
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                /// aspect.OnSuccess(aspectArgs);
                methodInjector.InvokeEventHandler(nameof(OnSuccess));
                methodInjector.SetAspectArguments(pointerOnly: true);
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// aspectArgs.Exception = ex;
                /// aspect.OnException(aspectArgs);
                methodInjector.SetException();
                methodInjector.InvokeEventHandler(nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// aspect.OnExit(aspectArgs);
                methodInjector.InvokeEventHandler(nameof(OnExit));
            });

            methodInjector.RewriteTargetMethod(onEntry, onSuccess, onException, onExit);
        }

        #endregion

        #region イテレーターメソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="stateMachineInjector">イテレーターステートマシンへの注入。</param>
        private void RewriteMoveNextMethod(IteratorStateMachineInjector stateMachineInjector)
        {
            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// _arguments  = new Arguments(...);
                /// _aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// _aspect.OnEntry(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                stateMachineInjector.NewAspectArgs(processor);
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnEntry));
                stateMachineInjector.SetArgumentFields(processor);
            });

            var onResume = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnResume(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnResume));
                stateMachineInjector.SetArgumentFields(processor);
            });

            var onYield = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.YieldValue = <> 2__current;
                /// _aspect.OnYield(aspectArgs);
                /// <>2__current = (TResult)aspectArgs.YieldValue;
                SetYieldValue(processor, stateMachineInjector);
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnYield));
                SetCurrentField(processor, stateMachineInjector);
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnSuccess(aspectArgs);
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.Exception = exception;
                /// _aspect.OnException(aspectArgs);
                stateMachineInjector.SetException(processor);
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnExit(aspectArgs);
                stateMachineInjector.InvokeEventHandler(processor, nameof(OnExit));
            });

            stateMachineInjector.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        /// <summary>
        /// AspectArgs.YieldValue フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        private void SetYieldValue(ILProcessor processor, IteratorStateMachineInjector stateMachineInjector)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, stateMachineInjector.AspectArgsField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, stateMachineInjector.CurrentField);
            if (stateMachineInjector.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Box, stateMachineInjector.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Call, stateMachineInjector.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));
        }

        /// <summary>
        /// Current フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        private void SetCurrentField(ILProcessor processor, IteratorStateMachineInjector stateMachineInjector)
        {
            processor.Emit(OpCodes.Ldarg_0);

            processor.Emit(OpCodes.Dup);
            processor.Emit(OpCodes.Ldfld, stateMachineInjector.AspectArgsField);
            processor.Emit(OpCodes.Call, stateMachineInjector.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetGetMethod()));
            if (stateMachineInjector.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, stateMachineInjector.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Stfld, stateMachineInjector.CurrentField);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="stateMachineInjector">非同期ステートマシンへの注入。</param>
        private void RewriteMoveNextMethod(AsyncStateMachineInjector stateMachineInjector)
        {
            var onEntry = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// var instance = <> 4__this;
                /// arguments = new Arguments(...);
                /// aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// aspect.OnEntry(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                stateMachineInjector.NewAspectArgs(processor, insert);
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnEntry));
                stateMachineInjector.SetArgumentFields(processor, insert);
            });

            var onResume = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnResume(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnResume));
                stateMachineInjector.SetArgumentFields(processor, insert);
            });

            var onYield = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnYield(aspectArgs);
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnYield));
            });

            var onSuccess = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.ReturnValue = result;
                /// aspect.OnSuccess(aspectArgs);
                /// result = (TResult)aspectArgs.ReturnValue;
                int resultVariable = 1;
                stateMachineInjector.SetReturnValue(processor, insert, resultVariable);
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnSuccess));
                stateMachineInjector.SetReturnVariable(processor, insert, resultVariable);
            });

            var onException = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.Exception = exception;
                /// aspect.OnException(aspectArgs);
                stateMachineInjector.SetException(processor, insert);
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnException));
            });

            var onExit = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnExit(aspectArgs);
                stateMachineInjector.InvokeEventHandler(processor, insert, nameof(OnExit));
            });

            stateMachineInjector.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        #endregion

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メッソドが開始されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnEntry(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// <c>yield return</c> または <c>await</c> ステートメントの結果として、ステートマシンが結果を出力するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドでは、アドバイスは <c>yield return</c> ステートメントで呼びだされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果としてステートマシンが待機を開始した直後にアドバイスが呼びだされます。
        /// <c>await</c> ステートメントのオペランドが同期的に完了した操作である場合、ステートマシンは結果を出力せず、<see cref="OnYield(MethodExecutionArgs)"/> アドバイスは呼び出されません。
        /// </remarks>
        public virtual void OnYield(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// <c>yield return</c> または <c>await</c> ステートメントの後にステートマシンが実行を再開するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドの場合、このアドバイスは MoveNext() メソッドの前に呼びだされます。
        /// ただし、MoveNext() の最初の呼び出しは <see cref="OnEntry(MethodExecutionArgs)"/> にマップされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果として待機した後、ステートマシンが実行を再開した直後にアドバイスが呼びだされます。
        /// </remarks>
        public virtual void OnResume(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが正常終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが例外終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnException(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnExit(MethodExecutionArgs args)
        {
        }

        #endregion

        #endregion
    }
}
