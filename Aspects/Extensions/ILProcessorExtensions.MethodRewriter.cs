using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ILProcessor"/> の拡張メソッド。
    /// </summary>
    public static partial class ILProcessorExtensions
    {
        #region メソッド

        /// <summary>
        /// 指定メソッドの末尾にコードを注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void Append(this ILProcessor processor, MethodDefinition method)
        {
            // ローカル変数コレクションを追加します。
            int variableOffset = processor.Body.Variables.Count();
            foreach (var variable in method.Body.Variables)
            {
                processor.Body.Variables.Add(variable);
            }

            // 命令コレクションを追加します。
            var leaveTarget = processor.Create(OpCodes.Nop);
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldloc_0)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = processor.Body.Variables[0 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Ldloc_1)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = processor.Body.Variables[1 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Ldloc_2)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = processor.Body.Variables[2 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Ldloc_3)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = processor.Body.Variables[3 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Ldloc_S || instruction.OpCode == OpCodes.Ldloc)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                }
                else if (instruction.OpCode == OpCodes.Ldloca_S || instruction.OpCode == OpCodes.Ldloca)
                {
                    instruction.OpCode  = OpCodes.Ldloca;
                }
                else  if (instruction.OpCode == OpCodes.Stloc_0)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = processor.Body.Variables[0 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Stloc_1)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = processor.Body.Variables[1 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Stloc_2)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = processor.Body.Variables[2 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Stloc_3)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = processor.Body.Variables[3 + variableOffset];
                }
                else if (instruction.OpCode == OpCodes.Stloc_S || instruction.OpCode == OpCodes.Stloc)
                {
                    instruction.OpCode = OpCodes.Stloc;
                }
                else if (instruction.OpCode == OpCodes.Ret)
                {
                    if (instruction == method.Body.Instructions.Last())
                    {
                        instruction.OpCode  = OpCodes.Nop;
                        instruction.Operand = null;
                    }
                    else
                    {
                        instruction.OpCode  = OpCodes.Leave;
                        instruction.Operand = leaveTarget;
                    }
                }

                processor.Append(instruction);
            }
            processor.Append(leaveTarget);

            //
            foreach (var sequencePoint in method.DebugInformation.SequencePoints)
            {
                processor.Body.Method.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            // 例外ハンドラーコレクションを追加します。
            foreach (var exceptionHandler in method.Body.ExceptionHandlers)
            {
                processor.Body.ExceptionHandlers.Add(exceptionHandler);
            }
        }

        /// <summary>
        /// メソッドを書き換えます。
        /// </summary>
        /// <param name="onEntry">OnEntory アドバイスの注入処理。</param>
        /// <param name="onInvoke">OnInvoke アドバイスの注入処理。</param>
        /// <param name="onException">OnException アドバイスの注入処理。</param>
        /// <param name="onExit">OnFinally アドバイスの注入処理。</param>
        /// <remarks>
        /// メソッドを以下のように書き換えます。
        /// <code>
        /// ...OnEntry アドバイス...
        /// try
        /// {
        ///     ...OnInvoke アドバイス...
        /// }
        /// catch (Exception ex)
        /// {
        ///     ...OnException アドバイス...
        /// }
        /// finally
        /// {
        ///     ...OnFinally アドバイス...
        /// }
        /// ...OnReturn アドバイス...
        /// </code>
        /// </remarks>
        public static void RewriteMethod(this ILProcessor processor, Action onEntry, Action onInvoke, Action onException, Action onExit, Action onReturn)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            method.DebugInformation.SequencePoints.Clear();
            method.Body.Instructions.Clear();
            method.Body.Variables.Clear();
            method.Body.ExceptionHandlers.Clear();

            /// 例外ハンドラーを追加します。
            var handlers = method.Body.ExceptionHandlers;
            var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
            var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
            handlers.Add(@catch);
            handlers.Add(@finally);

            /// ...OnEntry アドバイス...
            onEntry();

            /// try
            /// {
            ///     ...OnInvoke アドバイス...
            @catch.TryStart = @finally.TryStart = processor.EmitNop();
            onInvoke();
            var leave = processor.EmitLeave(OpCodes.Leave);

            /// }
            /// catch (Exception ex)
            /// {
            ///     ...OnException アドバイス...
            /// }
            @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
            onException();

            /// finally
            /// {
            ///     ...OnFinally アドバイス...
            /// }
            @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
            onExit();
            processor.Emit(OpCodes.Endfinally);

            /// ...OnReturn アドバイス...
            leave.Operand = @finally.HandlerEnd = processor.EmitNop();
            onReturn();

            /// IL コードを最適化します。
            method.Optimize();
        }

        #endregion
    }
}
