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
        internal static Instruction EntryInstruction(this ILProcessor processor)
        {
            return processor.Body.Instructions.First();
        }

        internal static Instruction ExitInstruction(this ILProcessor processor)
        {
            if (processor.Body.Method.HasReturnValue())
            {
                return processor.Body.Instructions.Last().Previous.Previous;
            }
            else
            {
                return processor.Body.Instructions.Last();
            }
        }

        /// <summary>
        /// 戻り値をスタックにロードする命令を取得します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <returns>戻り値をスタックにロードする命令。</returns>
        internal static Instruction ReturnLoadInstruction(this ILProcessor processor)
        {
            return processor.Body.Instructions.Last().Previous;
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

                case Instruction value:
                    return processor.Create(source.OpCode, value);

                case FieldReference value:
                    return processor.Create(source.OpCode, value);

                case MethodReference value:
                    return processor.Create(source.OpCode, value);

                case TypeDefinition value:
                    return processor.Create(source.OpCode, value);

                case VariableDefinition value:
                    return processor.Create(source.OpCode, value);

                default:
                    throw new NotSupportedException($"{source} のコピーには対応していません。");
            }
        }

        #endregion
    }
}
