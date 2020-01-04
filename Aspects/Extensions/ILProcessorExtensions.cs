﻿using Mono.Cecil;
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
