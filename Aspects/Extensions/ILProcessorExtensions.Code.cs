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
        public static void Emit(this ILProcessor processor, MethodBody methodBody)
        {
            // ローカル変数コレクションを追加します。
            int variableOffset     = processor.Body.Variables.Count();
            var variableToVariable = new Dictionary<VariableDefinition, VariableDefinition>();      // 元々の命令→複製された命令。
            foreach (var originalVariable in methodBody.Variables)
            {
                var variable = new VariableDefinition(originalVariable.VariableType);
                processor.Body.Variables.Add(variable);

                variableToVariable.Add(originalVariable, variable);
            }

            // 命令コレクションを複製 (ディープクローン) します。
            var instructions             = new List<Instruction>();                                 // 複製された命令コレクション。
            var instructionToInstruction = new Dictionary<Instruction, Instruction>();              // 元々の命令→複製された命令。
            foreach (var originalInstruction in methodBody.Instructions)
            {
                var instruction = processor.Create(OpCodes.Nop);
                instruction.OpCode  = originalInstruction.OpCode;
                instruction.Operand = originalInstruction.Operand;
                instructions.Add(instruction);

                instructionToInstruction.Add(originalInstruction, instruction);
            }

            // 命令コレクションを追加します。
            var leaveTarget = processor.Create(OpCodes.Nop);
            foreach (var instruction in instructions)
            {
                switch (instruction.Operand)
                {
                    case Instruction originalInstruction:
                        instruction.Operand = instructionToInstruction[originalInstruction];
                        break;

                    case Instruction[] originalInstructions:
                        instruction.Operand = originalInstructions.Select(oi => instructionToInstruction[oi]).ToArray();
                        break;

                    case VariableDefinition originalVariable:
                        instruction.Operand = variableToVariable[originalVariable];
                        break;
                }

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
                    if (instruction == instructions.Last())
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

            // 例外ハンドラーコレクションを追加します。
            foreach (var originalExceptionHandler in methodBody.ExceptionHandlers)
            {
                var exceptionHandler = new ExceptionHandler(originalExceptionHandler.HandlerType);
                exceptionHandler.CatchType    = originalExceptionHandler.CatchType;
                exceptionHandler.TryStart     = originalExceptionHandler.TryStart     == null ? null : instructionToInstruction[originalExceptionHandler.TryStart];
                exceptionHandler.TryEnd       = originalExceptionHandler.TryEnd       == null ? null : instructionToInstruction[originalExceptionHandler.TryEnd];
                exceptionHandler.FilterStart  = originalExceptionHandler.FilterStart  == null ? null : instructionToInstruction[originalExceptionHandler.FilterStart];
                exceptionHandler.HandlerStart = originalExceptionHandler.HandlerStart == null ? null : instructionToInstruction[originalExceptionHandler.HandlerStart];
                exceptionHandler.HandlerEnd   = originalExceptionHandler.HandlerEnd   == null ? null : instructionToInstruction[originalExceptionHandler.HandlerEnd];

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
        public static void Emit(this ILProcessor processor, Action<ILProcessor> onEntry, Action<ILProcessor> onInvoke, Action<ILProcessor> onException, Action<ILProcessor> onExit, Action<ILProcessor> onReturn)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            /// 例外ハンドラーを追加します。
            var handlers = method.Body.ExceptionHandlers;
            var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
            var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
            handlers.Add(@catch);
            handlers.Add(@finally);

            /// ...OnEntry アドバイス...
            onEntry(processor);

            /// try
            /// {
            ///     ...OnInvoke アドバイス...
            @catch.TryStart = @finally.TryStart = processor.EmitNop();
            onInvoke(processor);
            var leave = processor.EmitLeave(OpCodes.Leave);

            /// }
            /// catch (Exception ex)
            /// {
            ///     ...OnException アドバイス...
            /// }
            @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
            onException(processor);

            /// finally
            /// {
            ///     ...OnFinally アドバイス...
            /// }
            @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
            onExit(processor);
            processor.Emit(OpCodes.Endfinally);

            /// ...OnReturn アドバイス...
            leave.Operand = @finally.HandlerEnd = processor.EmitNop();
            onReturn(processor);

            /// IL コードを最適化します。
            method.Optimize();
        }

        #endregion
    }
}
