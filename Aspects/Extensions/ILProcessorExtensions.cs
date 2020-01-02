using Mono.Cecil.Cil;
using System;
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
        /// 
        /// </summary>
        /// <param name="processor"><see cref="ILProcessor"/>。</param>
        /// <returns></returns>
        internal static Instruction FirstInstruction(this ILProcessor processor)
        {
            return processor.Body.Instructions.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"><see cref="ILProcessor"/>。</param>
        /// <returns></returns>
        internal static Instruction ReturnInstruction(this ILProcessor processor)
        {
            var method = processor.Body.Method;

            switch (method.ReturnType.MetadataType)
            {
                case Mono.Cecil.MetadataType.Void:
                    return processor.Body.Instructions.Last();

                case Mono.Cecil.MetadataType.Boolean:
                case Mono.Cecil.MetadataType.Char:
                case Mono.Cecil.MetadataType.SByte:
                case Mono.Cecil.MetadataType.Byte:
                case Mono.Cecil.MetadataType.Int16:
                case Mono.Cecil.MetadataType.UInt16:
                case Mono.Cecil.MetadataType.Int32:
                case Mono.Cecil.MetadataType.UInt32:
                case Mono.Cecil.MetadataType.Double:
                case Mono.Cecil.MetadataType.Single:
                case Mono.Cecil.MetadataType.String:
                    return processor.Body.Instructions.Last().Previous.Previous.Previous.Previous;

                case Mono.Cecil.MetadataType.Int64:
                case Mono.Cecil.MetadataType.UInt64:
                    return processor.Body.Instructions.Last().Previous.Previous.Previous.Previous.Previous;

                case Mono.Cecil.MetadataType.Pointer:
                case Mono.Cecil.MetadataType.ByReference:
                case Mono.Cecil.MetadataType.ValueType:
                case Mono.Cecil.MetadataType.Class:
                case Mono.Cecil.MetadataType.Var:
                case Mono.Cecil.MetadataType.Array:
                case Mono.Cecil.MetadataType.GenericInstance:
                case Mono.Cecil.MetadataType.TypedByReference:
                case Mono.Cecil.MetadataType.IntPtr:
                case Mono.Cecil.MetadataType.UIntPtr:
                case Mono.Cecil.MetadataType.FunctionPointer:
                case Mono.Cecil.MetadataType.Object:
                case Mono.Cecil.MetadataType.MVar:
                case Mono.Cecil.MetadataType.RequiredModifier:
                case Mono.Cecil.MetadataType.OptionalModifier:
                case Mono.Cecil.MetadataType.Sentinel:
                case Mono.Cecil.MetadataType.Pinned:
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// <see cref="Instruction"/> をコピーします。
        /// </summary>
        /// <param name="processor"><see cref="ILProcessor"/>。</param>
        /// <param name="source">コピー元の <see cref="Instruction">。</param>
        /// <returns>コピーした <see cref="Instruction">。</returns>
        internal static Instruction Copy(this ILProcessor processor, Instruction source)
        {
            if (source.OpCode == OpCodes.Ldloc_0 || source.OpCode == OpCodes.Ldloc_1 || source.OpCode == OpCodes.Ldloc_2 || source.OpCode == OpCodes.Ldloc_3)
            {
                return processor.Create(source.OpCode);
            }
            else if (source.OpCode == OpCodes.Ldstr)
            {
                return processor.Create(source.OpCode, source.Operand as string);
            }
            else if (source.OpCode == OpCodes.Ldc_I4_0 || source.OpCode == OpCodes.Ldc_I4_1 || source.OpCode == OpCodes.Ldc_I4_2 || source.OpCode == OpCodes.Ldc_I4_3 || source.OpCode == OpCodes.Ldc_I4_4 || source.OpCode == OpCodes.Ldc_I4_5 || source.OpCode == OpCodes.Ldc_I4_6 || source.OpCode == OpCodes.Ldc_I4_7 || source.OpCode == OpCodes.Ldc_I4_8 || source.OpCode == OpCodes.Ldc_I4_M1)
            {
                return processor.Create(source.OpCode);
            }
            else if (source.OpCode == OpCodes.Ldc_I4_S)
            {
                return processor.Create(source.OpCode, (sbyte)source.Operand);
            }
            else if (source.OpCode == OpCodes.Conv_I8)
            {
                return processor.Create(source.OpCode);
            }
            else if (source.OpCode == OpCodes.Ldc_R4)
            {
                return processor.Create(source.OpCode, (float)source.Operand);
            }
            else if (source.OpCode == OpCodes.Ldc_R8)
            {
                return processor.Create(source.OpCode, (double)source.Operand);
            }
            else
            {
                throw new NotSupportedException($"{source} のコピーには対応していません。");
            }
        }

        #endregion
    }
}
