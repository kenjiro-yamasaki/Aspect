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
                var iteratorStateMachineInjector = new IteratorStateMachineInjector(method, aspect);

                iteratorStateMachineInjector.CreateAspectInstance();
                ReplaceMoveNextMethod(iteratorStateMachineInjector);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var asyncStateMachineInjector = new AsyncStateMachineInjector(method, aspect);

                asyncStateMachineInjector.CreateAspectInstance();
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
                injector.CreateAspectArgsInstance(processor);
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
                injector.SetYieldValue(processor);
                injector.InvokeEventHandler(processor, nameof(OnYield));
                injector.SetCurrentField(processor);
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

            injector.ReplaceMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// <see cref="IAsyncStateMachine.MoveNext"> を書き換えます。
        /// </summary>
        /// <param name="injector">非同期ステートマシンへの注入。</param>
        private void ReplaceMoveNextMethod(AsyncStateMachineInjector injector)
        {
            var module         = injector.Module;
            var moveNextMethod = injector.MoveNextMethod;
            var processor      = moveNextMethod.Body.GetILProcessor();
            var instructions   = moveNextMethod.Body.Instructions;

            /// ローカル変数を追加します。
            var variables = moveNextMethod.Body.Variables;
            int resultVariable = 1;

            /// 例外ハンドラーを追加します。
            /// 内側の例外ハンドラーが先になるように並び変えます。
            /// この順番を間違えるとランタイムエラーが発生します。
            var handlers = moveNextMethod.Body.ExceptionHandlers;

            var outerCatch      = handlers[0];
            var innerCatch      = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
            var innerFinally    = new ExceptionHandler(ExceptionHandlerType.Finally);
            innerCatch.TryStart = innerFinally.TryStart = outerCatch.TryStart;

            handlers.Clear();
            handlers.Add(innerCatch);
            handlers.Add(innerFinally);
            handlers.Add(outerCatch);

            /// try
            /// {
            ///     if (!resumeFlag)
            ///     {
            ///         var instance = <> 4__this;
            ///         arguments = new Arguments(...);
            ///         aspectArgs = new MethodExecutionArgs(instance, arguments);
            ///         aspect.OnEntry(aspectArgs);
            ///         resumeFlag = true;
            ///     }
            ///     else
            ///     {
            ///         aspect.OnResume(aspectArgs);
            ///     }
            ///     arg0 = arguments.Arg0;
            ///     arg1 = arguments.Arg1;
            ///     try
            ///     {
            {
                var branch = new Instruction[2];
                var insert = innerCatch.TryStart;

                outerCatch.TryStart = processor.InsertNopBefore(insert);

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, injector.ResumeFlagField);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Brtrue_S);

                injector.CreateAspectArgsInstance(processor, insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnEntry));
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldc_I4_1);
                processor.InsertBefore(insert, OpCodes.Stfld, injector.ResumeFlagField);
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br_S);

                branch[0].Operand = processor.InsertNopBefore(insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnResume));

                branch[1].Operand = processor.InsertNopBefore(insert);
                injector.SetArgumentFields(processor, insert);
            }

            ///         IL_XXXX: // leaveTarget
            ///         if (<> 1__state != -1)
            ///         {
            ///             aspect.OnYield(aspectArgs);
            ///         }
            ///         else
            ///         {
            ///             aspectArgs.ReturnValue = result;
            ///             aspect.OnSuccess(aspectArgs);
            ///             result = (TResult)aspectArgs.ReturnValue;
            ///         }
            Instruction leave;
            {
                var instruction = outerCatch.HandlerStart.Previous;
                if (instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S)
                {
                    leave = instruction;
                }
                else if (instruction.OpCode == OpCodes.Throw)
                {
                    leave = processor.InsertLeaveBefore(instruction.Next, OpCodes.Leave);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            {
                var insert = leave;
                var branch = new Instruction[2];

                var leaveTarget = processor.InsertNopBefore(insert);                                      // try 内の Leave 命令の転送先 (OnYield と OnSuccess の呼び出し処理に転送します)。
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, injector.StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Beq);

                injector.InvokeEventHandler(processor, insert, nameof(OnYield));
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br_S);
                branch[1].Operand = leave;

                branch[0].Operand = processor.InsertNopBefore(insert);
                injector.SetReturnValue(processor, insert, resultVariable);
                injector.InvokeEventHandler(processor, insert, nameof(OnSuccess));
                injector.SetReturnVariable(processor, insert, resultVariable);

                /// try 内の Leave 命令の転送先を書き換えます。
                /// この書き換えにより OnYield と OnSuccess の呼びだし処理に転送します。
                for (var instruction = innerCatch.TryStart; instruction != leaveTarget; instruction = instruction.Next)
                {
                    if (instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S)
                    {
                        instruction.OpCode  = OpCodes.Br;
                        instruction.Operand = leaveTarget;
                    }
                }
            }

            ///     }
            ///     catch (Exception exception)
            ///     {
            ///         aspectArgs.Exception = exception;
            ///         aspect.OnException(aspectArgs);
            ///         throw;
            ///     }
            {
                var insert = outerCatch.HandlerStart;

                innerCatch.TryEnd = innerCatch.HandlerStart = processor.InsertNopBefore(insert);
                injector.SetException(processor, insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnException));
                processor.InsertBefore(insert, OpCodes.Rethrow);
            }

            ///     finally
            ///     {
            ///         if (<>1__state == -1)
            ///         {
            ///             aspect.OnExit(aspectArgs);
            ///         }
            ///     }
            {
                var insert = outerCatch.HandlerStart;
                var branch = new Instruction[2];

                innerCatch.HandlerEnd = innerFinally.TryEnd = innerFinally.HandlerStart = processor.InsertNopBefore(insert);
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, injector.StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Beq);
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br);

                branch[0].Operand = processor.InsertNopBefore(insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnExit));

                branch[1].Operand = processor.InsertNopBefore(insert);
                processor.InsertBefore(insert, OpCodes.Endfinally);
                innerFinally.HandlerEnd = insert;
            }

            /// }
            /// catch (Exception exception)
            /// {
            ///     ...
            /// }
            /// if (<> 1__state == -1)
            /// {
            ///     ...
            /// }
            {
                var insert = outerCatch.HandlerEnd;

                leave.Operand = outerCatch.HandlerEnd = processor.InsertNopBefore(insert);
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, injector.StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                processor.InsertBefore(insert, OpCodes.Beq, insert);

                processor.InsertBefore(insert, OpCodes.Br, instructions.Last());
            }

            /// IL を最適化します。
            moveNextMethod.Optimize();
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
