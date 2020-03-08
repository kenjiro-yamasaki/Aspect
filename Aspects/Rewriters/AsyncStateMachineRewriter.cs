using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非同期ステートマシンの書き換え。
    /// </summary>
    public class AsyncStateMachineRewriter : StateMachineRewriter
    {
        #region プロパティ

        /// <summary>
        /// ステートマシン属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public AsyncStateMachineRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute, Type aspectArgsType)
            : base(targetMethod, aspectAttribute, aspectArgsType)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="onEntry">OnEntory アドバイスの注入処理。</param>
        /// <param name="onResume">OnResume アドバイスの注入処理。</param>
        /// <param name="onYield">OnYield アドバイスの注入処理。</param>
        /// <param name="onSuccess">OnSuccess アドバイスの注入処理。</param>
        /// <param name="onException">OnException アドバイスの注入処理。</param>
        /// <param name="onExit">OnExit アドバイスの注入処理。</param>
        public void RewriteMoveNextMethod(Action<ILProcessor, Instruction> onEntry, Action<ILProcessor, Instruction> onResume, Action<ILProcessor, Instruction> onYield, Action<ILProcessor, Instruction> onSuccess, Action<ILProcessor, Instruction> onException, Action<ILProcessor, Instruction> onExit)
        {
            var processor    = MoveNextMethod.Body.GetILProcessor();
            var instructions = MoveNextMethod.Body.Instructions;

            /// 例外ハンドラーを追加します。
            /// 内側の例外ハンドラーが先になるように並び変えます。
            /// この順番を間違えるとランタイムエラーが発生します。
            var handlers = MoveNextMethod.Body.ExceptionHandlers;

            var outerCatch      = handlers[0];
            var innerCatch      = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = Module.ImportReference(typeof(Exception)) };
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
            ///         onEntry();
            ///         resumeFlag = true;
            ///     }
            ///     else
            ///     {
            ///         onResume();
            ///     }
            ///     try
            ///     {
            {
                var branch = new Instruction[2];
                var insert = innerCatch.TryStart;

                outerCatch.TryStart = processor.InsertNopBefore(insert);

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, ResumeFlagField);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Brtrue_S);
                onEntry(processor, insert);

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldc_I4_1);
                processor.InsertBefore(insert, OpCodes.Stfld, ResumeFlagField);
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br_S);

                branch[0].Operand = processor.InsertNopBefore(insert);
                onResume(processor, insert);

                branch[1].Operand = processor.InsertNopBefore(insert);
            }

            ///         IL_XXXX: // leaveTarget
            ///         if (<> 1__state != -1)
            ///         {
            ///             onYield();
            ///         }
            ///         else
            ///         {
            ///             onSuccess();
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

                var leaveTarget = processor.InsertNopBefore(insert);                                // try 内の Leave 命令の転送先 (OnYield と OnSuccess の呼び出し処理に転送します)。
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Beq);
                onYield(processor, insert);
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br_S);
                branch[1].Operand = leave;

                branch[0].Operand = processor.InsertNopBefore(insert);
                onSuccess(processor, insert);

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
            ///         onException();
            ///         throw;
            ///     }
            {
                var insert = outerCatch.HandlerStart;

                innerCatch.TryEnd = innerCatch.HandlerStart = processor.InsertNopBefore(insert);
                onException(processor, insert);
                processor.InsertBefore(insert, OpCodes.Rethrow);
            }

            ///     finally
            ///     {
            ///         if (<>1__state == -1)
            ///         {
            ///             onExit();
            ///         }
            ///     }
            {
                var insert = outerCatch.HandlerStart;
                var branch = new Instruction[2];

                innerCatch.HandlerEnd = innerFinally.TryEnd = innerFinally.HandlerStart = processor.InsertNopBefore(insert);
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.InsertBranchBefore(insert, OpCodes.Beq);
                branch[1] = processor.InsertBranchBefore(insert, OpCodes.Br);

                branch[0].Operand = processor.InsertNopBefore(insert);
                onExit(processor, insert);

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
                processor.InsertBefore(insert, OpCodes.Ldfld, StateField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, -1);
                processor.InsertBefore(insert, OpCodes.Beq, insert);

                processor.InsertBefore(insert, OpCodes.Br, instructions.Last());
            }

            /// IL コードを最適化します。
            MoveNextMethod.Optimize();
        }

        #endregion
    }
}
