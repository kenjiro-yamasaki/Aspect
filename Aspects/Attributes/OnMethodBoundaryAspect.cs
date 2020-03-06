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
                var rewriter = new IteratorStateMachineRewriter(method, aspect, typeof(MethodExecutionArgs));

                rewriter.NewAspectAttribute();
                RewriteMoveNextMethod(rewriter);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var rewriter = new AsyncStateMachineRewriter(method, aspect, typeof(MethodExecutionArgs));

                rewriter.NewAspectAttribute();
                RewriteMoveNextMethod(rewriter);
            }
            else
            {
                var rewriter = new MethodRewriter(method, aspect);

                RewriteTargetMethod(rewriter);
            }
        }

        #region 通常のメソッド

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">ターゲットメソッドの書き換え。</param>
        private void RewriteTargetMethod(MethodRewriter rewriter)
        {
            /// オリジナルターゲットメソッド (ターゲットメソッドの元々のコード) を生成します。
            rewriter.CreateOriginalTargetMethod();

            /// ターゲットメソッドを書き換えます。
            var onEntry = new Action<ILProcessor>(_ =>
            {
                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnEntry(aspectArgs);
                /// arg0 = (TArg0)arguments[0];
                /// arg1 = (TArg1)arguments[1];
                /// ...
                rewriter.NewAspectAttributeVariable();
                rewriter.NewArgumentsVariable();
                rewriter.NewAspectArgsVariable(rewriter.Module.ImportReference(typeof(MethodExecutionArgs)));
                rewriter.UpdateMethodProperty();
                rewriter.InvokeAspectHandler(nameof(OnEntry));
                rewriter.UpdateArguments(pointerOnly: false);
            });

            var onInvoke = new Action<ILProcessor>(_ =>
            {
                /// var returnValue = OriginalMethod(arg0, arg1, ...);
                /// aspectArgs.ReturnValue = returnValue;
                /// arguments[0] = arg0;
                /// arguments[1] = arg1;
                /// ...
                /// aspect.OnSuccess(aspectArgs);
                rewriter.InvokeOriginalTargetMethod();
                rewriter.UpdateReturnValueProperty();
                rewriter.UpdateArgumentsProperty(pointerOnly: true);
                rewriter.InvokeAspectHandler(nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// aspectArgs.Exception = ex;
                /// aspect.OnException(aspectArgs);
                rewriter.UpdateExceptionProperty();
                rewriter.InvokeAspectHandler(nameof(OnException));
                processor.Emit(OpCodes.Rethrow);
            });

            var onExit = new Action<ILProcessor>(_ =>
            {
                /// arg0 = (TArg0)arguments[0];
                /// arg1 = (TArg1)arguments[1];
                /// ...
                /// aspect.OnExit(aspectArgs);
                rewriter.InvokeAspectHandler(nameof(OnExit));
                rewriter.UpdateArguments(pointerOnly: true);
            });

            var onReturn = new Action<ILProcessor>(_ =>
            {
                /// return (TResult)aspectArgs.ReturnValue;
                rewriter.ReturnProperty();
            });

            rewriter.RewriteMethod(onEntry, onInvoke, onException, onExit, onReturn);
        }

        #endregion

        #region イテレーターメソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        private void RewriteMoveNextMethod(IteratorStateMachineRewriter rewriter)
        {
            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// _arguments  = new Arguments(...);
                /// _aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// _aspect.OnEntry(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                rewriter.NewAspectArgs(processor);
                rewriter.InvokeAspectHandler(processor, nameof(OnEntry));
                rewriter.SetArgumentFields(processor);
            });

            var onResume = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnResume(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                rewriter.InvokeAspectHandler(processor, nameof(OnResume));
                rewriter.SetArgumentFields(processor);
            });

            var onYield = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.YieldValue = <> 2__current;
                /// _aspect.OnYield(aspectArgs);
                /// <>2__current = (TResult)aspectArgs.YieldValue;
                SetYieldValue(processor, rewriter);
                rewriter.InvokeAspectHandler(processor, nameof(OnYield));
                SetCurrentField(processor, rewriter);
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnSuccess(aspectArgs);
                rewriter.InvokeAspectHandler(processor, nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.Exception = exception;
                /// _aspect.OnException(aspectArgs);
                rewriter.SetException(processor);
                rewriter.InvokeAspectHandler(processor, nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnExit(aspectArgs);
                rewriter.InvokeAspectHandler(processor, nameof(OnExit));
            });

            rewriter.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        /// <summary>
        /// AspectArgs.YieldValue フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        private void SetYieldValue(ILProcessor processor, IteratorStateMachineRewriter rewriter)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, rewriter.AspectArgsField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, rewriter.CurrentField);
            if (rewriter.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Box, rewriter.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Call, rewriter.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));
        }

        /// <summary>
        /// Current フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        private void SetCurrentField(ILProcessor processor, IteratorStateMachineRewriter rewriter)
        {
            processor.Emit(OpCodes.Ldarg_0);

            processor.Emit(OpCodes.Dup);
            processor.Emit(OpCodes.Ldfld, rewriter.AspectArgsField);
            processor.Emit(OpCodes.Call, rewriter.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetGetMethod()));
            if (rewriter.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, rewriter.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Stfld, rewriter.CurrentField);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">非同期ステートマシンの書き換え。</param>
        private void RewriteMoveNextMethod(AsyncStateMachineRewriter rewriter)
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
                rewriter.NewAspectArgs(processor, insert);
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnEntry));
                rewriter.SetArgumentFields(processor, insert);
            });

            var onResume = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnResume(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnResume));
                rewriter.SetArgumentFields(processor, insert);
            });

            var onYield = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnYield(aspectArgs);
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnYield));
            });

            var onSuccess = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.ReturnValue = result;
                /// aspect.OnSuccess(aspectArgs);
                /// result = (TResult)aspectArgs.ReturnValue;
                int resultVariable = 1;
                rewriter.SetReturnValue(processor, insert, resultVariable);
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnSuccess));
                rewriter.SetReturnVariable(processor, insert, resultVariable);
            });

            var onException = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.Exception = exception;
                /// aspect.OnException(aspectArgs);
                rewriter.SetException(processor, insert);
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnException));
            });

            var onExit = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnExit(aspectArgs);
                rewriter.InvokeAspectHandler(processor, insert, nameof(OnExit));
            });

            rewriter.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
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
