using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="MethodDefinition"/> の拡張メソッド。
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        #region プロパティ

        /// <summary>
        /// 戻り値が存在するかを判断します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>戻り値が存在するか。</returns>
        public static bool HasReturnValue(this MethodDefinition method)
        {
            return method.ReturnType.FullName != "System.Void";
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="multicastAttributes">マルチキャスト属性コレクション。</param>
        public static void InjectAdvice(this MethodDefinition method, IEnumerable<MulticastAttribute> multicastAttributes)
        {
            using var profile = Profiling.Profiler.Start($"{nameof(MethodDefinitionExtensions)}.{nameof(InjectAdvice)}");

            // メソッドのマルチキャスト属性を生成します。
            var currentMulticastAttributes = new List<MulticastAttribute>();
            foreach (var customAttribute in method.CustomAttributes.ToList())
            {
                if (customAttribute.IsMulticastAttribute())
                {
                    var multicastAttribute = customAttribute.Create<MulticastAttribute>();
                    multicastAttribute.CustomAttribute = customAttribute;
                    currentMulticastAttributes.Add(multicastAttribute);

                    method.CustomAttributes.Remove(customAttribute);
                }
            }
            multicastAttributes = multicastAttributes.Concat(currentMulticastAttributes.OrderBy(ma => ma.AttributePriority));

            // メソッドレベルアスペクトを適用します。
            foreach (var group in multicastAttributes.GroupBy(ma => ma.GetType()))
            {
                // 
                var methodLevelAsspects = new List<MethodLevelAspect>();
                foreach (var methodLevelAspect in group.OfType<MethodLevelAspect>().Reverse())
                {
                    if (methodLevelAspect.AttributeExclude)
                    {
                        break;
                    }

                    methodLevelAsspects.Add(methodLevelAspect);

                    if (methodLevelAspect.AttributeReplace)
                    {
                        break;
                    }
                }
                methodLevelAsspects.Reverse();

                //
                foreach (var methodLevelAspect in methodLevelAsspects)
                {
                    methodLevelAspect.TargetMethod = method;
                    methodLevelAspect.InjectAdvice();
                }
            }
        }

        /// <summary>
        /// イテレーターステートマシン属性を取得します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>イテレーターステートマシン属性。</returns>
        public static CustomAttribute GetIteratorStateMachineAttribute(this MethodDefinition method)
        {
            return method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");
        }

        /// <summary>
        /// 非同期ステートマシン属性を取得します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>非同期ステートマシン属性。</returns>
        public static CustomAttribute GetAsyncStateMachineAttribute(this MethodDefinition method)
        {
            return method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
        }

        /// <summary>
        /// IL コードを最適化します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void Optimize(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();

            int offset = 0;
            foreach (var instruction in processor.Body.Instructions)
            {
                instruction.Offset = offset++;
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Br || i.OpCode == OpCodes.Br_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Br_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Br;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brtrue || i.OpCode == OpCodes.Brtrue_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Brtrue_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Brtrue;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brfalse || i.OpCode == OpCodes.Brfalse_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Brfalse_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Brfalse;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Leave || i.OpCode == OpCodes.Leave_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Leave_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Leave;
                }
            }

            var exceptionHandlers = method.Body.ExceptionHandlers.OrderBy(eh => eh.HandlerStart.Offset).ToList();
            method.Body.ExceptionHandlers.Clear();
            foreach (var exceptionHandler in exceptionHandlers)
            {
                method.Body.ExceptionHandlers.Add(exceptionHandler);
            }
        }

        /// <summary>
        /// IL コードをログ出力します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void Log(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();

            Logger.Info($"{method.FullName}");

            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Info($"{instruction}");
            }

            foreach (var handler in processor.Body.ExceptionHandlers)
            {
                Logger.Info($"TryStart     : {handler.TryStart}");
                Logger.Info($"TryEnd       : {handler.TryEnd}");
                Logger.Info($"HandlerStart : {handler.HandlerStart}");
                Logger.Info($"HandlerEnd   : {handler.HandlerEnd}");
            }
        }

        /// <summary>
        /// ローカル変数を追加します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="variableType">ローカル変数の型。</param>
        /// <returns>ローカル変数のインデックス。</returns>
        public static int AddVariable(this MethodDefinition method, TypeReference variableType)
        {
            var variables = method.Body.Variables;
            var module    = method.Module;

            var variable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(variableType)));

            return variable;
        }

        /// <summary>
        /// ローカル変数を追加します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="variableType">ローカル変数の型。</param>
        /// <returns>ローカル変数のインデックス。</returns>
        public static int AddVariable(this MethodDefinition method, Type variableType)
        {
            var variables = method.Body.Variables;
            var module    = method.Module;

            var variable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(variableType)));
        
            return variable;
        }

        /// <summary>
        /// メソッドを複製 (ディープクローン) します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>複製されたメソッド。</returns>
        public static MethodDefinition Clone(this MethodDefinition method)
        {
            var methodAttribute = method.Attributes & ~(MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            var clonedMethod = new MethodDefinition("*" + method.Name, methodAttribute, method.ReturnType);

            // ローカル変数コレクションを複製 (ディープクローン) します。
            var variableToVariable = new Dictionary<VariableDefinition, VariableDefinition>();      // 元々の命令→複製された命令。
            foreach (var variable in method.Body.Variables)
            {
                var clonedVariable = new VariableDefinition(variable.VariableType);
                clonedMethod.Body.Variables.Add(clonedVariable);

                variableToVariable.Add(variable, clonedVariable);
            }

            // 命令コレクションを複製 (ディープクローン) します。
            var processor = clonedMethod.Body.GetILProcessor();
            var instructionToInstruction = new Dictionary<Instruction, Instruction>();              // 元々の命令→複製された命令。
            var offsetToInstruction      = new Dictionary<int, Instruction>();
            foreach (var instruction in method.Body.Instructions)
            {
                var clonedInstruction = processor.Create(OpCodes.Nop);
                clonedInstruction.Offset  = instruction.Offset;
                clonedInstruction.OpCode  = instruction.OpCode;
                clonedInstruction.Operand = instruction.Operand;
                processor.Append(clonedInstruction);

                offsetToInstruction.Add(instruction.Offset, clonedInstruction);
                instructionToInstruction.Add(instruction, clonedInstruction);
            }

            //
            foreach (var instruction in clonedMethod.Body.Instructions)
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
            }

            // 例外ハンドラーコレクションを複製します。
            foreach (var handler in method.Body.ExceptionHandlers)
            {
                var clonedHandler = new ExceptionHandler(handler.HandlerType);
                clonedHandler.CatchType    = handler.CatchType;
                clonedHandler.TryStart     = handler.TryStart     == null ? null : instructionToInstruction[handler.TryStart];
                clonedHandler.TryEnd       = handler.TryEnd       == null ? null : instructionToInstruction[handler.TryEnd];
                clonedHandler.FilterStart  = handler.FilterStart  == null ? null : instructionToInstruction[handler.FilterStart];
                clonedHandler.HandlerStart = handler.HandlerStart == null ? null : instructionToInstruction[handler.HandlerStart];
                clonedHandler.HandlerEnd   = handler.HandlerEnd   == null ? null : instructionToInstruction[handler.HandlerEnd];

                clonedMethod.Body.ExceptionHandlers.Add(clonedHandler);
            }

            // デバッグ情報を複製します。
            foreach (var sequencePoint in method.DebugInformation.SequencePoints)
            {
                var clonedSequencePoint = new SequencePoint(offsetToInstruction[sequencePoint.Offset], sequencePoint.Document);
                clonedSequencePoint.StartLine   = sequencePoint.StartLine;
                clonedSequencePoint.EndLine     = sequencePoint.EndLine;
                clonedSequencePoint.StartColumn = sequencePoint.StartColumn;
                clonedSequencePoint.EndColumn   = sequencePoint.EndColumn;

                clonedMethod.DebugInformation.SequencePoints.Add(clonedSequencePoint);
            }

            return clonedMethod;
        }

        #endregion
    }
}
