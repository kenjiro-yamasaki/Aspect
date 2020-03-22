using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// コードの注入。
    /// </summary>
    public class CodeInjector
    {
        #region プロパティ

        /// <summary>
        /// メソッド本体。
        /// </summary>
        private MethodBody MethodBody { get; }

        /// <summary>
        /// 命令コレクション。
        /// </summary>
        private IEnumerable<Instruction> Instructions => MethodBody.Instructions;

        /// <summary>
        /// ローカル変数コレクション。
        /// </summary>
        private IEnumerable<VariableDefinition> Variables => MethodBody.Variables;

        /// <summary>
        /// 例外ハンドラーコレクション。
        /// </summary>
        private IEnumerable<ExceptionHandler> ExceptionHandlers => MethodBody.ExceptionHandlers;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="methodBody">メソッド本体。</param>
        public CodeInjector(MethodBody methodBody)
        {
            MethodBody = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定メソッドの末尾にコードを注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public void InjectTo(MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();

            // ローカル変数を追加します。
            int variableOffset = method.Body.Variables.Count;
            foreach (var variable in Variables)
            {
                method.Body.Variables.Add(variable);
            }

            // 命令コレクションと例外ハンドラーコレクションを複製 (ディープクローン) します。
            var instructions      = new List<Instruction>();                                        // 複製された命令コレクション。
            var exceptionHandlers = new List<ExceptionHandler>();                                   // 複製された例外ハンドラーコレクション。
            {
                // 命令コレクションを複製します。
                var instructionToInstruction = new Dictionary<Instruction, Instruction>();          // 元々の命令→複製された命令。
                instructionToInstruction.Add(null, null);

                foreach (var originalInstruction in Instructions)
                {
                    var instruction = processor.Create(OpCodes.Nop);
                    instruction.OpCode  = originalInstruction.OpCode;
                    instruction.Operand = originalInstruction.Operand;
                    instructions.Add(instruction);

                    instructionToInstruction.Add(originalInstruction, instruction);
                }

                foreach (var instruction in instructions)
                {
                    switch (instruction.Operand)
                    {
                        case Instruction instructionOperand:
                            instruction.Operand = instructionToInstruction[instructionOperand];
                            break;

                        case Instruction[] instructionsOperand:
                            instruction.Operand = instructionsOperand.Select(io => instructionToInstruction[io]).ToArray();
                            break;
                    }
                }

                // 例外ハンドラーコレクションを複製します。
                foreach (var originalExceptionHandler in ExceptionHandlers)
                {
                    var exceptionHandler = new ExceptionHandler(originalExceptionHandler.HandlerType);
                    exceptionHandler.TryStart     = instructionToInstruction[originalExceptionHandler.TryStart];
                    exceptionHandler.TryEnd       = instructionToInstruction[originalExceptionHandler.TryEnd];
                    exceptionHandler.FilterStart  = instructionToInstruction[originalExceptionHandler.FilterStart];
                    exceptionHandler.HandlerStart = instructionToInstruction[originalExceptionHandler.HandlerStart];
                    exceptionHandler.HandlerEnd   = instructionToInstruction[originalExceptionHandler.HandlerEnd];
                    exceptionHandlers.Add(exceptionHandler);
                }
            }

            // 命令コレクションを追加します。
            // ・ローカル変数のインデックスを補正します。
            // ・Leave 命令と Ret 命令を最終命令 (branchTarget) への Branch 命令に変更します。
            var branchTarget = processor.Create(OpCodes.Nop);
            foreach (var instruction in instructions)
            {
                if (instruction.OpCode == OpCodes.Ldloc_0)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = 0 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Ldloc_1)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = 1 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Ldloc_2)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = 2 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Ldloc_3)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = 3 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Ldloc_S || instruction.OpCode == OpCodes.Ldloc)
                {
                    instruction.OpCode  = OpCodes.Ldloc;
                    instruction.Operand = (int)instruction.Operand + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Ldloca_S || instruction.OpCode == OpCodes.Ldloca)
                {
                    instruction.OpCode  = OpCodes.Ldloca;
                    instruction.Operand = (int)instruction.Operand + variableOffset;
                }
                else  if (instruction.OpCode == OpCodes.Stloc_0)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = 0 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Stloc_1)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = 1 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Stloc_2)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = 2 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Stloc_3)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = 3 + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Stloc_S || instruction.OpCode == OpCodes.Stloc)
                {
                    instruction.OpCode  = OpCodes.Stloc;
                    instruction.Operand = (int)instruction.Operand + variableOffset;
                }
                else if (instruction.OpCode == OpCodes.Leave_S || instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Ret)
                {
                    instruction.OpCode  = OpCodes.Br;
                    instruction.Operand = branchTarget;
                }

                processor.Append(instruction);
            }
            processor.Append(branchTarget);

            // 例外ハンドラーを追加します。
            foreach (var exceptionHandler in exceptionHandlers)
            {
                method.Body.ExceptionHandlers.Add(exceptionHandler);
            }
        }

        #endregion
    }
}
