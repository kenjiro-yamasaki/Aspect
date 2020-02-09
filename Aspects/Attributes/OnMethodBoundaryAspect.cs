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

                /// aspect ローカル変数のインスタンスを生成します。
                /// aspectArgs ローカル変数のインスタンスを生成します。
                /// aspectArgs.Exception にメソッド情報を設定します。
                /// aspect.OnEntry を呼びだします。
                injector.CreateAspectVariable(processor);
                injector.CreateArgumentsVariable(processor);
                injector.CreateAspectArgsVariable<MethodExecutionArgs>(processor);
                injector.SetMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnEntry));

                /// try {
                ///     元々のメソッドを呼びだします。
                ///     aspect.OnSuccess を呼びだします。
                @catch.TryStart = @finally.TryStart = processor.EmitNop();
                injector.InvokeOriginalMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnSuccess));
                var leave = processor.EmitLeave(OpCodes.Leave);

                /// } catch (Exception exception) {
                ///     aspectArgs.Exception に例外を設定します。
                ///     aspect.OnException を呼びだします。
                @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                injector.SetException(processor);
                injector.InvokeEventHandler(processor, nameof(OnException));
                processor.Emit(OpCodes.Rethrow);

                /// } finally {
                ///     aspect.OnExit を呼びだします。
                @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                injector.InvokeEventHandler(processor, nameof(OnExit));
                processor.Emit(OpCodes.Endfinally);

                /// }
                /// リターンコードを追加します。
                leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                injector.Return(processor);

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
            var module         = injector.Module;
            var moveNextMethod = injector.MoveNextMethod;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            var originalMoveNextMethod = new MethodDefinition(moveNextMethod.Name + "<Original>", moveNextMethod.Attributes, moveNextMethod.ReturnType);
            foreach (var parameter in moveNextMethod.Parameters)
            {
                originalMoveNextMethod.Parameters.Add(parameter);
            }

            originalMoveNextMethod.Body = moveNextMethod.Body;

            foreach (var sequencePoint in moveNextMethod.DebugInformation.SequencePoints)
            {
                originalMoveNextMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            injector.StateMachineType.Methods.Add(originalMoveNextMethod);

            /// 元々のメソッドを書き換えます。
            moveNextMethod.Body = new Mono.Cecil.Cil.MethodBody(moveNextMethod);

            /// ローカル変数を追加します。
            var variables = moveNextMethod.Body.Variables;

            int resultVariable    = variables.Count;
            int exceptionVariable = variables.Count + 1;
            int exitFlagVariable  = variables.Count + 2;

            variables.Add(new VariableDefinition(module.TypeSystem.Boolean));
            variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));
            variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

            ///
            var processor = moveNextMethod.Body.GetILProcessor();
            {
                var branch = new Instruction[5];

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                branch[2] = processor.EmitBranch(OpCodes.Brtrue_S);

                /// <see cref="OnEntry"/> を呼びだします。
                injector.CreateAspectArgsInstance(processor);
                injector.InvokeEventHandler(processor, nameof(OnEntry));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                branch[3] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, injector.ResumeFlagField);
                branch[4] = processor.EmitBranch(OpCodes.Br_S);

                /// <see cref="OnResume"/> を呼びだします。
                branch[2].Operand = branch[3].Operand = processor.EmitNop();
                injector.InvokeEventHandler(processor, nameof(OnResume));

                branch[0].Operand = branch[1].Operand = branch[4].Operand = processor.EmitNop();
            }

            /// try {
            Instruction tryStart;
            Instruction leave;
            {
                var branch = new Instruction[8];

                tryStart =  processor.EmitNop();
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                processor.Emit(OpCodes.Ldc_I4_2);
                branch[0] = processor.EmitBranch(OpCodes.Beq_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, originalMoveNextMethod);
                processor.Emit(OpCodes.Stloc, resultVariable);

                branch[0].Operand = processor.EmitNop();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.IsDisposingField);
                branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldloc, resultVariable);
                branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);
                branch[3] = processor.EmitBranch(OpCodes.Br_S);

                branch[1].Operand = branch[2].Operand = processor.EmitNop();
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);

                branch[3].Operand = processor.EmitNop();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                branch[4] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                branch[5] = processor.EmitBranch(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldloc, exitFlagVariable);
                branch[6] = processor.EmitBranch(OpCodes.Brfalse_S);

                injector.InvokeEventHandler(processor, nameof(OnSuccess));
                branch[7] = processor.EmitBranch(OpCodes.Br_S);

                /// <see cref="OnYield"/> を呼びだします。
                branch[6].Operand = processor.EmitNop();
                injector.SetYieldValue(processor);
                injector.InvokeEventHandler(processor, nameof(OnYield));

                branch[4].Operand = branch[5].Operand = branch[7].Operand = leave = processor.EmitLeave(OpCodes.Leave);
            }

            /// } catch (Exception exception) {
            Instruction catchStart;
            {
                /// <see cref="OnException"/> を呼びだします。
                catchStart = processor.EmitNop();
                processor.Emit(OpCodes.Stloc, exceptionVariable);
                injector.SetException(processor, exceptionVariable);
                injector.InvokeEventHandler(processor, nameof(OnException));
                processor.Emit(OpCodes.Rethrow);
            }

            /// } finally {
            Instruction catchEnd;
            Instruction finallyStart;
            {
                var branch = new Instruction[3];

                catchEnd = finallyStart = processor.EmitNop();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ExitFlagField);
                branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, injector.ResumeFlagField);
                branch[1] = processor.EmitBranch(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldloc, exitFlagVariable);
                branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                /// ExitFlag を true にします。
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, injector.ExitFlagField);

                /// <see cref="OnExit"/> を呼びだします。
                injector.InvokeEventHandler(processor, nameof(OnExit));

                branch[0].Operand = branch[1].Operand = branch[2].Operand = processor.EmitNop();
                processor.Emit(OpCodes.Endfinally);
            }

            ///
            Instruction finallyEnd;
            {
                int resultIndex = variables.Count;
                variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                leave.Operand = finallyEnd = processor.EmitNop();
                processor.Emit(OpCodes.Ldloc, exitFlagVariable);
                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Ceq);
                processor.Emit(OpCodes.Stloc, resultIndex);
                processor.Emit(OpCodes.Ldloc, resultIndex);
                processor.Emit(OpCodes.Ret);
            }

            /// 例外ハンドラーを追加します。
            {
                var handlers = moveNextMethod.Body.ExceptionHandlers;

                /// Catch ハンドラーを追加します。
                var catchHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = tryStart,
                    TryEnd       = catchStart,
                    HandlerStart = catchStart,
                    HandlerEnd   = catchEnd,
                };
                handlers.Add(catchHandler);

                /// Finally ハンドラーを追加します。
                var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = tryStart,
                    TryEnd       = finallyStart,
                    HandlerStart = finallyStart,
                    HandlerEnd   = finallyEnd,
                };
                handlers.Add(finallyHandler);
            }

            /// IL を最適化します。
            moveNextMethod.OptimizeIL();
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> を書き換えます。
        /// </summary>
        /// <param name="injector">イテレーターステートマシンへの注入。</param>
        private void ReplaceDisposeMethod(IteratorStateMachineInjector injector)
        {
            var disposeMethod = injector.DisposeMethod;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            var originalDisposeMethod = new MethodDefinition(disposeMethod.Name + "<Original>", disposeMethod.Attributes, disposeMethod.ReturnType);
            foreach (var parameter in disposeMethod.Parameters)
            {
                originalDisposeMethod.Parameters.Add(parameter);
            }

            originalDisposeMethod.Body = disposeMethod.Body;
            foreach (var sequencePoint in disposeMethod.DebugInformation.SequencePoints)
            {
                originalDisposeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            injector.StateMachineType.Methods.Add(originalDisposeMethod);

            /// 元々のメソッドを書き換えます。
            disposeMethod.Body = new Mono.Cecil.Cil.MethodBody(disposeMethod);

            var processor = disposeMethod.Body.GetILProcessor();
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

            /// try {
            Instruction tryStart;
            Instruction leave;
            {
                tryStart = processor.EmitNop();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, originalDisposeMethod);
                leave = processor.EmitLeave(OpCodes.Leave_S);
            }

            /// } finally {
            Instruction finallyStart;
            {
                finallyStart = processor.EmitNop();
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

            Instruction finallyEnd;
            {
                leave.Operand = finallyEnd = processor.EmitNop();
                processor.Emit(OpCodes.Ret);
            }

            /// Finally ハンドラーを追加します。
            var handlers = disposeMethod.Body.ExceptionHandlers;
            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart     = tryStart,
                TryEnd       = finallyStart,
                HandlerStart = finallyStart,
                HandlerEnd   = finallyEnd,
            };
            handlers.Add(finallyHandler);
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
            int resultVariable    = 1;
            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

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

                var leaveTarget = processor.Emit(insert, OpCodes.Ldarg_0);                    // try 内の Leave 命令の転送先 (OnYield と OnSuccess の呼び出し処理に転送します)。
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
                innerCatchStart = processor.Emit(insert, OpCodes.Stloc, exceptionVariable);
                injector.SetException(processor, insert, exceptionVariable);
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
