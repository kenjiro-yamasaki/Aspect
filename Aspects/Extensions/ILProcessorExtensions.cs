using Mono.Cecil.Cil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ILProcessor"/> の拡張メソッド。
    /// </summary>
    public static class ILProcessorExtensions
    {
        #region メソッド

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
            else if (source.OpCode == OpCodes.Ldc_I4_0 || source.OpCode == OpCodes.Ldc_I4_1 || source.OpCode == OpCodes.Ldc_I4_2)
            {
                return processor.Create(source.OpCode);
            }
            else if (source.OpCode == OpCodes.Ldc_I4_S)
            {
                return processor.Create(OpCodes.Ldc_I4_S, (sbyte)source.Operand);
            }
            else
            {
                throw new NotSupportedException($"{source} のコピーには対応していません。");
            }
        }

        #endregion
    }
}
