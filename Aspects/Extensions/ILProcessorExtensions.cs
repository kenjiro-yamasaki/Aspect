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
                    return processor.ReturnInstructions().First();

                case MetadataType.Pointer:
                case MetadataType.ByReference:
                case MetadataType.Class:
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
            var result = new List<Instruction>();

            for (var instruction = processor.Body.Instructions.Last(); instruction.OpCode != OpCodes.Nop; instruction = instruction.Previous)
            {
                result.Add(instruction);
            }
            result.Reverse();

            return result;
        }

        internal static IReadOnlyList<Instruction> ReturnLoadInstructions(this ILProcessor processor)
        {
            var result = new List<Instruction>();

            foreach (var instruction in processor.ReturnInstructions())
            {
                if (instruction.OpCode == OpCodes.Stloc_0)
                {
                    break;
                }

                result.Add(instruction);
            }

            return result;
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

            if (source.OpCode == OpCodes.Ldstr)
            {
                return processor.Create(source.OpCode, source.Operand as string);
            }
            if (source.OpCode == OpCodes.Ldc_I4)
            {
                return processor.Create(source.OpCode, (int)source.Operand);
            }
            if (source.OpCode == OpCodes.Ldc_I4_S)
            {
                return processor.Create(source.OpCode, (sbyte)source.Operand);
            }
            if (source.OpCode == OpCodes.Ldc_R4)
            {
                return processor.Create(source.OpCode, (float)source.Operand);
            }
            if (source.OpCode == OpCodes.Ldc_R8)
            {
                return processor.Create(source.OpCode, (double)source.Operand);
            }
            if (source.OpCode == OpCodes.Ldsfld)
            {
                return processor.Create(source.OpCode, (FieldReference)source.Operand);
            }
            if (source.OpCode == OpCodes.Newobj)
            {
                return processor.Create(source.OpCode, (MethodReference)source.Operand);
            }

            throw new NotSupportedException($"{source} のコピーには対応していません。");
        }

        #endregion
    }
}
