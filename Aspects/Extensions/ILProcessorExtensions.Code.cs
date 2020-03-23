using Mono.Cecil.Cil;
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
                //else if (instruction.OpCode == OpCodes.Leave_S || instruction.OpCode == OpCodes.Leave)
                //{
                //    instruction.OpCode = OpCodes.Leave;
                //    instruction.Operand = leaveTarget;
                //}
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

        #endregion
    }
}
