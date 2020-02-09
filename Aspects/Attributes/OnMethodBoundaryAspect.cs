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
    [Serializable]
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

        #region アスペクトの注入

        /// <summary>
        /// アスペクトをメソッドに注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        protected override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {
            /// 書き換え前の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();

            var iteratorStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");
            var asyncStateMachineAttribute    = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");

            if (iteratorStateMachineAttribute != null)
            {
                var injector = new IteratorStateMachineInjector(method, aspect);

                injector.CreateAspectInstance();
                ReplaceMoveNextMethod(injector);
                ReplaceDisposeMethod(injector);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var injector = new AsyncStateMachineInjector(method, aspect);

                injector.CreateAspectInstance();
                ReplaceMoveNextMethod(injector);
            }
            else
            {
                var injector = new MethodInjector(method, aspect);

                ReplaceMethod(injector);
            }

            /// 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        #region 通常のメソッド

        /// <summary>
        /// メソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        private void ReplaceMethod(MethodInjector injector)
        {
            var method = injector.TargetMethod;
            var module = method.Module;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            injector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                method.Body = new Mono.Cecil.Cil.MethodBody(method);
                var processor = method.Body.GetILProcessor();

                /// 例外ハンドラーを追加します。
                var handlers = method.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnEntry(aspectArgs);
                {
                    injector.CreateAspectVariable(processor);
                    injector.CreateArgumentsVariable(processor);
                    injector.CreateAspectArgsVariable<MethodExecutionArgs>(processor);
                    injector.SetMethod(processor);
                    injector.InvokeEventHandler(processor, nameof(OnEntry));
                }

                /// try
                /// {
                ///     aspectArgs.ReturnValue = OriginalMethod(...);
                ///     aspect.OnSuccess(aspectArgs);
                Instruction leave;
                {
                    @catch.TryStart = @finally.TryStart = processor.EmitNop();
                    injector.InvokeOriginalMethod(processor);
                    injector.InvokeEventHandler(processor, nameof(OnSuccess));
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
                    injector.SetException(processor);
                    injector.InvokeEventHandler(processor, nameof(OnException));
                    processor.Emit(OpCodes.Rethrow);
                }

                /// }
                /// finally
                /// {
                ///     aspect.OnExit(aspectArgs);
                {
                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    injector.InvokeEventHandler(processor, nameof(OnExit));
                    processor.Emit(OpCodes.Endfinally);
                }

                /// }
                /// return (TResult)aspectArgs.ReturnValue;
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    injector.Return(processor);
                }

                /// IL を最適化します。
                method.OptimizeIL();
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
            var module = injector.Module;

            /// 新たなメソッドを生成し、MoveNext メソッドの内容を移動します。
            injector.ReplaceMoveNext();

            /// 元々の MoveNext メソッドを書き換えます。
            {
                var moveNextMethod = injector.MoveNextMethod;
                moveNextMethod.Body = new MethodBody(moveNextMethod);

                /// ローカル変数を追加します。
                var variables = moveNextMethod.Body.Variables;

                int exitVariable = variables.Count;
                variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                /// 例外ハンドラーを追加します。
                var handlers = moveNextMethod.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                var processor = moveNextMethod.Body.GetILProcessor();

                /// if (!exitFlag && isDisposing == 0)
                /// {
                ///     if (!resumeFlag)
                ///     {
                ///         args = new Arguments(...);
                ///         aspectArgs = new MethodExecutionArgs(instance, args);
                ///         aspect.OnEntry(aspectArgs);
                ///         resumeFlag = true;
                ///     }
                ///     else
                ///     {
                ///         aspect.OnResume(aspectArgs);
                ///     }
                /// }
                {
                    var branch = new Instruction[4];

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                    branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                    branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                    branch[2] = processor.EmitBranch(OpCodes.Brtrue_S);

                    injector.CreateAspectArgsInstance(processor);
                    injector.InvokeEventHandler(processor, nameof(OnEntry));

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, injector.ResumeFlagField);
                    branch[3] = processor.EmitBranch(OpCodes.Br_S);

                    branch[2].Operand = processor.EmitNop();
                    injector.InvokeEventHandler(processor, nameof(OnResume));

                    branch[0].Operand = branch[1].Operand = branch[3].Operand = processor.EmitNop();
                }

                /// bool exit;
                /// try
                /// {
                ///     exit = true;
                ///     bool result;
                ///     if (isDisposing != 2)
                ///     {
                ///         result = MoveNext<Original>();
                ///     }
                ///     exit = isDisposing != 0 || !result;
                ///     if (!exitFlag && resumeFlag)
                ///     {
                ///         if (exit)
                ///         {
                ///             aspect.OnSuccess(aspectArgs);
                ///         }
                ///         else
                ///         {
                ///             aspectArgs.YieldValue = <> 2__current;
                ///             aspect.OnYield(aspectArgs);
                ///         }
                ///     }
                Instruction leave;
                {
                    var branch = new Instruction[8];

                    @catch.TryStart = @finally.TryStart = processor.EmitNop();

                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stloc, exitVariable);

                    int resultVariable = variables.Count;
                    variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                    processor.Emit(OpCodes.Ldc_I4_2);
                    branch[0] = processor.EmitBranch(OpCodes.Beq_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, injector.OriginalMoveNextMethod);
                    processor.Emit(OpCodes.Stloc, resultVariable);

                    branch[0].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                    branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldloc, resultVariable);
                    branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Stloc, exitVariable);
                    branch[3] = processor.EmitBranch(OpCodes.Br_S);

                    branch[1].Operand = branch[2].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stloc, exitVariable);

                    branch[3].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                    branch[4] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                    branch[5] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    branch[6] = processor.EmitBranch(OpCodes.Brfalse_S);

                    injector.InvokeEventHandler(processor, nameof(OnSuccess));
                    branch[7] = processor.EmitBranch(OpCodes.Br_S);

                    branch[6].Operand = processor.EmitNop();
                    injector.SetYieldValue(processor);
                    injector.InvokeEventHandler(processor, nameof(OnYield));

                    branch[4].Operand = branch[5].Operand = branch[7].Operand = leave = processor.EmitLeave(OpCodes.Leave);
                }

                /// }
                /// catch (Exception exception)
                /// {
                ///     aspectArgs.Exception = exception;
                ///     aspect.OnException(aspectArgs);
                ///     throw;
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    injector.SetException(processor);
                    injector.InvokeEventHandler(processor, nameof(OnException));
                    processor.Emit(OpCodes.Rethrow);
                }

                /// }
                /// finally
                /// {
                ///     if (!exitFlag && resumeFlag && exit)
                ///     {
                ///         exitFlag = true;
                ///         aspect.OnExit(aspectArgs);
                ///     }
                {
                    var branch = new Instruction[3];

                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                    branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                    branch[1] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, injector.ExitFlagField);

                    injector.InvokeEventHandler(processor, nameof(OnExit));

                    branch[0].Operand = branch[1].Operand = branch[2].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Endfinally);
                }

                /// }
                /// return !exit;
                {
                    int resultVariable = variables.Count;
                    variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Ceq);
                    processor.Emit(OpCodes.Stloc, resultVariable);
                    processor.Emit(OpCodes.Ldloc, resultVariable);
                    processor.Emit(OpCodes.Ret);
                }

                /// IL を最適化します。
                moveNextMethod.OptimizeIL();
            }
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> を書き換えます。
        /// </summary>
        /// <param name="injector">イテレーターステートマシンへの注入。</param>
        private void ReplaceDisposeMethod(IteratorStateMachineInjector injector)
        {
            /// 新たなメソッドを生成し、Dispose メソッドの内容を移動します。
            injector.ReplaceDispose();

            /// Dispose のメソッドを書き換えます。
            {
                var disposeMethod = injector.DisposeMethod;
                disposeMethod.Body = new MethodBody(disposeMethod);

                /// 例外ハンドラーを追加します。
                var handlers = disposeMethod.Body.ExceptionHandlers;
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@finally);

                var processor = disposeMethod.Body.GetILProcessor();

                /// if (isDisposing == 0)
                /// {
                ///     isDisposing = 1;
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                    var branch = processor.EmitBranch(OpCodes.Brfalse_S);
                    processor.Emit(OpCodes.Ret);

                    branch.Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, injector.IsDisposingField);
                }

                ///     try
                ///     {
                ///         System.IDisposable.Dispose<Original>();
                Instruction leave;
                {
                    @finally.TryStart = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, injector.OriginalDisposeMethod);
                    leave = processor.EmitLeave(OpCodes.Leave_S);
                }

                ///     }
                ///     finally
                ///     {
                ///         isDisposing = 2;
                ///         MoveNext();
                ///         isDisposing = 0;
                {
                    @finally.HandlerStart = @finally.TryEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_2);
                    processor.Emit(OpCodes.Stfld, injector.IsDisposingField);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Callvirt, injector.MoveNextMethod);

                    processor.Emit(OpCodes.Pop);
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Stfld, injector.IsDisposingField);
                    processor.Emit(OpCodes.Endfinally);
                }

                ///     }
                /// }
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ret);
                }
            }
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

            /// ローカル変数を追加します。
            var variables = moveNextMethod.Body.Variables;
            int resultVariable = 1;

            /// IL を書き換えます。
            var processor    = moveNextMethod.Body.GetILProcessor();
            var instructions = moveNextMethod.Body.Instructions;
            var handlers     = moveNextMethod.Body.ExceptionHandlers;

            var outerCatchHandler = handlers[0];
            var outerCatchStart   = outerCatchHandler.HandlerStart;
            var outerCatchEnd     = outerCatchHandler.HandlerEnd;
            var innerTryStart     = outerCatchHandler.TryStart;
            {
                var branch = new Instruction[2];
                var insert = innerTryStart;

                outerCatchHandler.TryStart = processor.Emit(insert, OpCodes.Ldarg_0);
                processor.Emit(insert, OpCodes.Ldfld, injector.ResumeFlagField);
                branch[0] = processor.EmitBranch(insert, OpCodes.Brtrue_S);

                /// AspectArgs フィールドのインスタンスを生成します。
                injector.CreateAspectArgsInstance(processor, insert);

                /// <see cref="OnEntry"/> を呼びだします。
                injector.InvokeEventHandler(processor, insert, nameof(OnEntry));

                processor.Emit(insert, OpCodes.Ldarg_0);
                processor.Emit(insert, OpCodes.Ldc_I4_1);
                processor.Emit(insert, OpCodes.Stfld, injector.ResumeFlagField);
                branch[1] = processor.EmitBranch(insert, OpCodes.Br_S);

                /// <see cref="OnResume"/> を呼びだします。
                branch[0].Operand = processor.Emit(insert, OpCodes.Nop);
                injector.InvokeEventHandler(processor, insert, nameof(OnResume));

                branch[1].Operand = processor.Emit(insert, OpCodes.Nop);
            }

            ///
            var leave = outerCatchHandler.HandlerStart.Previous;
            {
                var branch = new Instruction[2];
                var insert = leave;

                var leaveTarget = processor.Emit(insert, OpCodes.Ldarg_0);                          // try 内の Leave 命令の転送先 (OnYield と OnSuccess の呼び出し処理に転送します)。
                processor.Emit(insert, OpCodes.Ldfld, injector.StateField);
                processor.Emit(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.EmitBranch(insert, OpCodes.Beq);

                /// <see cref="OnYield"/> を呼びだします。
                injector.InvokeEventHandler(processor, insert, nameof(OnYield));
                branch[1] = processor.Emit(insert, OpCodes.Br_S, insert);

                /// <see cref="OnSuccess"/> を呼びだします。
                branch[0].Operand = processor.Emit(insert, OpCodes.Nop);
                injector.SetReturnValue(processor, insert, resultVariable);
                injector.InvokeEventHandler(processor, insert, nameof(OnSuccess));

                /// try 内の Leave 命令の転送先を書き換えます。
                /// この書き換えにより OnYield と OnSuccess の呼び出し処理に転送します。
                for (var instruction = innerTryStart; instruction != leaveTarget; instruction = instruction.Next)
                {
                    if (instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S)
                    {
                        instruction.OpCode  = OpCodes.Br;
                        instruction.Operand = leaveTarget;
                    }
                }
            }

            /// } catch (Exception exception) {
            Instruction innerCatchStart;
            {
                var insert = outerCatchStart;

                /// <see cref="OnException"/> を呼び出します。
                innerCatchStart = processor.Emit(insert, OpCodes.Nop);
                injector.SetException(processor, insert);
                injector.InvokeEventHandler(processor, insert, nameof(OnException));
                processor.Emit(insert, OpCodes.Rethrow);
            }

            /// } finally {
            Instruction innerCatchEnd;
            Instruction innerFinallyStart;
            Instruction innerFinallyEnd;
            {
                var branch = new Instruction[2];
                var insert = outerCatchStart;

                innerCatchEnd = innerFinallyStart = processor.Emit(insert, OpCodes.Ldarg_0);
                processor.Emit(insert, OpCodes.Ldfld, injector.StateField);
                processor.Emit(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.EmitBranch(insert, OpCodes.Beq);
                branch[1] = processor.EmitBranch(insert, OpCodes.Br);

                /// <see cref="OnExit"/> を呼び出します。
                branch[0].Operand = processor.Emit(insert, OpCodes.Nop);
                injector.InvokeEventHandler(processor, insert, nameof(OnExit));

                branch[1].Operand = processor.Emit(insert, OpCodes.Endfinally);
                innerFinallyEnd = insert;
            }

            {
                var insert = outerCatchEnd;

                leave.Operand = handlers[0].HandlerEnd = processor.Emit(insert, OpCodes.Ldarg_0);
                processor.Emit(insert, OpCodes.Ldfld, injector.StateField);
                processor.Emit(insert, OpCodes.Ldc_I4, -1);
                processor.Emit(insert, OpCodes.Beq, insert);

                processor.Emit(insert, OpCodes.Br, instructions.Last());
            }

            /// 例外ハンドラーを追加します。
            {
                /// Catch ハンドラーを追加します。
                var innerCatchHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = innerTryStart,
                    TryEnd       = innerCatchStart,
                    HandlerStart = innerCatchStart,
                    HandlerEnd   = innerCatchEnd,
                };

                /// Finally ハンドラーを追加します。
                var innerFinallryHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = innerTryStart,
                    TryEnd       = innerFinallyStart,
                    HandlerStart = innerFinallyStart,
                    HandlerEnd   = innerFinallyEnd,
                };

                /// 内側の例外ハンドラーが先になるように並び変えます。
                /// この順番を間違えるとランタイムエラーが発生します。
                handlers.Clear();
                handlers.Add(innerCatchHandler);
                handlers.Add(innerFinallryHandler);
                handlers.Add(outerCatchHandler);
            }

            /// IL を最適化します。
            moveNextMethod.OptimizeIL();
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
