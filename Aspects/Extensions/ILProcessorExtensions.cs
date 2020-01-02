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
    public static class ILProcessorExtensions
    {
        #region メソッド

        /// <summary>
        /// 最初の命令を取得します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <returns>最初の命令。</returns>
        internal static Instruction FirstInstruction(this ILProcessor processor)
        {
            return processor.Body.Instructions.First();
        }

        /// <summary>
        /// リターン命令 (戻り値がある場合、戻り値をスタックにロードする命令) を取得します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <returns>リターン命令 (戻り値がある場合、戻り値をスタックにロードする命令)。</returns>
        internal static Instruction ReturnInstruction(this ILProcessor processor)
        {
            var method = processor.Body.Method;

            switch (method.ReturnType.MetadataType)
            {
                case MetadataType.Void:
                    return processor.Body.Instructions.Last();

                case MetadataType.Boolean:
                case MetadataType.Char:
                case MetadataType.SByte:
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Double:
                case MetadataType.Single:
                case MetadataType.String:
                case MetadataType.ValueType:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                case MetadataType.Class:
                    return processor.ReturnInstructions().First();

                case MetadataType.Pointer:
                case MetadataType.ByReference:
                case MetadataType.Var:
                case MetadataType.Array:
                case MetadataType.GenericInstance:
                case MetadataType.TypedByReference:
                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                case MetadataType.FunctionPointer:
                case MetadataType.Object:
                case MetadataType.MVar:
                case MetadataType.RequiredModifier:
                case MetadataType.OptionalModifier:
                case MetadataType.Sentinel:
                case MetadataType.Pinned:
                default:
                    throw new NotSupportedException();
            }
        }

        internal static IReadOnlyList<Instruction> ReturnInstructions(this ILProcessor processor)
        {
            var returns = new List<Instruction>();

            var instruction = processor.Body.Instructions.Last();
            for (; instruction.OpCode != OpCodes.Stloc_0; instruction = instruction.Previous)
            {
                returns.Add(instruction);
            }

            var returnLoads = new List<Instruction>();
            while (returnLoads.Count == 0)
            {
                instruction = instruction.Previous;
                for (; instruction.OpCode != OpCodes.Nop; instruction = instruction.Previous)
                {
                    returns.Add(instruction);
                    returnLoads.Add(instruction);
                }
            }

            returns.Reverse();
            returnLoads.Reverse();

            return returns;
        }

        internal static IReadOnlyList<Instruction> ReturnLoadInstructions(this ILProcessor processor)
        {
            var returns = new List<Instruction>();

            var instruction = processor.Body.Instructions.Last();
            for (; instruction.OpCode != OpCodes.Stloc_0; instruction = instruction.Previous)
            {
                returns.Add(instruction);
            }

            var returnLoads = new List<Instruction>();
            while (returnLoads.Count == 0)
            {
                instruction = instruction.Previous;
                for (; instruction.OpCode != OpCodes.Nop; instruction = instruction.Previous)
                {
                    returns.Add(instruction);
                    returnLoads.Add(instruction);
                }
            }

            returns.Reverse();
            returnLoads.Reverse();

            return returnLoads;
        }




        /// <summary>
        /// 命令をコピーします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="source">コピー元の命令。</param>
        /// <returns>コピーした命令。</returns>
        internal static Instruction Copy(this ILProcessor processor, Instruction source)
        {
            if (source.Operand == null)
            {
                return processor.Create(source.OpCode);
            }

            switch (source.Operand)
            {
                case string value:
                    return processor.Create(source.OpCode, value);

                case int value:
                    return processor.Create(source.OpCode, value);

                case sbyte value:
                    return processor.Create(source.OpCode, value);

                case float value:
                    return processor.Create(source.OpCode, value);

                case double value:
                    return processor.Create(source.OpCode, value);

                case FieldReference value:
                    return processor.Create(source.OpCode, value);

                case MethodReference value:
                    return processor.Create(source.OpCode, value);

                default:
                    throw new NotSupportedException($"{source} のコピーには対応していません。");
            }
        }

        #endregion
    }
}
