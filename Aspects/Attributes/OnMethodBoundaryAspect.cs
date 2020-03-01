using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

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
                var iteratorStateMachineInjector = new IteratorStateMachineInjector(method, aspect, typeof(MethodExecutionArgs));

                iteratorStateMachineInjector.NewAspectAttribute();
                ReplaceMoveNextMethod(iteratorStateMachineInjector);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var asyncStateMachineInjector = new AsyncStateMachineInjector(method, aspect, typeof(MethodExecutionArgs));

                asyncStateMachineInjector.NewAspectAttribute();
                ReplaceMoveNextMethod(asyncStateMachineInjector);
            }
            else
            {
                var methodInjector = new MethodInjector(method, aspect);

                ReplaceMethod(methodInjector);
            }
        }

        #region 通常のメソッド

        /// <summary>
        /// 対象メソッドを書き換えます。
        /// </summary>
        /// <param name="methodInjector">対象メソッドへの注入。</param>
        private void ReplaceMethod(MethodInjector methodInjector)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            methodInjector.ReplaceMethod();

            /// 対象メソッドのコードを書き換えます。
            {
                var method    = methodInjector.TargetMethod;
                var module    = methodInjector.Module;
                var processor = methodInjector.Processor;

                /// 例外ハンドラーを追加します。
                var handlers = method.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                /// var aspect     = new Aspect(...) {...};
                /// var arguments  = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnEntry(aspectArgs);
                {
                    methodInjector.CreateAspectVariable();
                    methodInjector.CreateArgumentsVariable();
                    methodInjector.CreateAspectArgsVariable(module.ImportReference(typeof(MethodExecutionArgs)));
                    methodInjector.SetMethod();
                    methodInjector.InvokeEventHandler(nameof(OnEntry));
                }

                /// try
                /// {
                ///     aspectArgs.ReturnValue = OriginalMethod(...);
                ///     aspect.OnSuccess(aspectArgs);
                Instruction leave;
                {
                    @catch.TryStart = @finally.TryStart = processor.EmitNop();
                    methodInjector.InvokeOriginalMethod();
                    methodInjector.InvokeEventHandler(nameof(OnSuccess));
                    methodInjector.SetAspectArguments(pointerOnly: true);
                    leave = processor.EmitLeave(OpCodes.Leave);
                }

                /// }
                /// catch (Exception ex)
                /// {
                ///     aspectArgs.Exception = ex;
                ///     aspect.OnException(aspectArgs);
                ///     throw;
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    methodInjector.SetException();
                    methodInjector.InvokeEventHandler(nameof(OnException));
                    processor.Emit(OpCodes.Rethrow);
                }

                /// }
                /// finally
                /// {
                ///     aspect.OnExit(aspectArgs);
                {
                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    methodInjector.InvokeEventHandler(nameof(OnExit));
                    processor.Emit(OpCodes.Endfinally);
                }

                /// }
                /// return (TResult)aspectArgs.ReturnValue;
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    methodInjector.Return();
                }

                /// IL を最適化します。
                method.Optimize();
            }
        }

        #endregion

        #region イテレーターメソッド

        /// <summary>
        /// <see cref="IEnumerator.MoveNext"/> を書き換えます。
        /// </summary>
        /// <param name="injector">イテレーターステートマシンへの注入。</param>
        private void ReplaceMoveNextMethod(IteratorStateMachineInjector injector)
        {
            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// _arguments  = new Arguments(...);
                /// _aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// _aspect.OnEntry(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                injector.NewAspectArgs(processor);
                injector.InvokeEventHandler(processor, nameof(OnEntry));
                injector.SetArgumentFields(processor);
            });

            var onResume = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnResume(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                injector.InvokeEventHandler(processor, nameof(OnResume));
                injector.SetArgumentFields(processor);
            });

            var onYield = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.YieldValue = <> 2__current;
                /// _aspect.OnYield(aspectArgs);
                /// <>2__current = (TResult)aspectArgs.YieldValue;
                SetYieldValue(processor, injector);
                injector.InvokeEventHandler(processor, nameof(OnYield));
                SetCurrentField(processor, injector);
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnSuccess(aspectArgs);
                injector.InvokeEventHandler(processor, nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.Exception = exception;
                /// _aspect.OnException(aspectArgs);
                injector.SetException(processor);
                injector.InvokeEventHandler(processor, nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnExit(aspectArgs);
                injector.InvokeEventHandler(processor, nameof(OnExit));
            });

            injector.InjectMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        /// <summary>
        /// AspectArgs.YieldValue フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        private void SetYieldValue(ILProcessor processor, IteratorStateMachineInjector injector)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, injector.AspectArgsField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, injector.CurrentField);
            if (injector.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Box, injector.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Call, injector.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));
        }

        /// <summary>
        /// Current フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        private void SetCurrentField(ILProcessor processor, IteratorStateMachineInjector injector)
        {
            processor.Emit(OpCodes.Ldarg_0);

            processor.Emit(OpCodes.Dup);
            processor.Emit(OpCodes.Ldfld, injector.AspectArgsField);
            processor.Emit(OpCodes.Call, injector.Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetGetMethod()));
            if (injector.CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, injector.CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Stfld, injector.CurrentField);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// <see cref="IAsyncStateMachine.MoveNext"> を書き換えます。
        /// </summary>
        /// <param name="injector">非同期ステートマシンへの注入。</param>
        private void ReplaceMoveNextMethod(AsyncStateMachineInjector injector)
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
                injector.NewAspectArgs(processor, insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnEntry));
                injector.SetArgumentFields(processor, insert);
            });

            var onResume = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnResume(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                injector.InvokeEventHandler(processor, insert, nameof(OnResume));
                injector.SetArgumentFields(processor, insert);
            });

            var onYield = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnYield(aspectArgs);
                injector.InvokeEventHandler(processor, insert, nameof(OnYield));
            });

            var onSuccess = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.ReturnValue = result;
                /// aspect.OnSuccess(aspectArgs);
                /// result = (TResult)aspectArgs.ReturnValue;
                int resultVariable = 1;
                injector.SetReturnValue(processor, insert, resultVariable);
                injector.InvokeEventHandler(processor, insert, nameof(OnSuccess));
                injector.SetReturnVariable(processor, insert, resultVariable);
            });

            var onException = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.Exception = exception;
                /// aspect.OnException(aspectArgs);
                injector.SetException(processor, insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnException));
            });

            var onExit = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnExit(aspectArgs);
                injector.InvokeEventHandler(processor, insert, nameof(OnExit));
            });

            injector.InjectMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
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
