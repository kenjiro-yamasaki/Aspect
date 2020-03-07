using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ILProcessor"/> の拡張メソッド。
    /// </summary>
    public static class ILProcessorExtensions
    {
        #region メソッド

        #region Create

        /// <summary>
        /// Branch 命令を生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>Branch 命令 (ただし、オペランドは <c>null</c>)。</returns>
        /// <remarks>
        /// Branch 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Branch 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Branch 命令を生成します。
        /// 転送先の命令が生成された後、Branch 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction CreateBranch(this ILProcessor processor, OpCode opcode)
        {
            if (opcode == OpCodes.Br || opcode == OpCodes.Br_S || opcode == OpCodes.Beq || opcode == OpCodes.Beq_S || opcode == OpCodes.Brtrue || opcode == OpCodes.Brtrue_S || opcode == OpCodes.Brfalse || opcode == OpCodes.Brfalse_S)
            {
                var instruction = processor.Create(OpCodes.Nop);
                instruction.OpCode = opcode;
                return instruction;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Leave 命令を生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>Branch 命令 (ただし、オペランドは <c>null</c>)。</returns>
        /// <remarks>
        /// Leave 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Leave 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Leave 命令を生成します。
        /// 転送先の命令が生成された後、Leave 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction CreateLeave(this ILProcessor processor, OpCode opcode)
        {
            if (opcode == OpCodes.Leave || opcode == OpCodes.Leave_S)
            {
                var instruction = processor.Create(OpCodes.Nop);
                instruction.OpCode = opcode;
                return instruction;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.Create(opcode);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, int operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, long operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, byte operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, sbyte operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, float operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, double operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, string operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, TypeReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, MethodReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, FieldReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, ParameterDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, VariableDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction[] operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        public static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, CallSite operand)
        {
            var instruction = processor.Create(opcode, operand);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
        }

        /// <summary>
        /// 指定命令の前に Branch 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された Branch 命令。</returns>
        /// <remarks>
        /// Branch 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Branch 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Branch 命令を挿入し、挿入した Branch 命令を戻します。
        /// 転送先の命令が生成された後、Branch 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertBranchBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateBranch(opcode);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に Leave 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された Leave 命令。</returns>
        /// <remarks>
        /// Leave 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Leave 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Leave 命令を挿入し、挿入した Leave 命令を戻します。
        /// 転送先の命令が生成された後、Leave 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertLeaveBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateLeave(opcode);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に Nop 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <returns>追加された Nop 命令。</returns>
        public static Instruction InsertNopBefore(this ILProcessor processor, Instruction insert)
        {
            var instruction = processor.Create(OpCodes.Nop);
            if (insert != null)
            {
                processor.InsertBefore(insert, instruction);
            }
            else
            {
                processor.Append(instruction);
            }
            return instruction;
        }

        /// <summary>
        /// 命令の前にアスペクト属性を追加するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">命令。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        /// <returns>アスペクト属性の変数インデックス。</returns>
        internal static void InsertNewAspectAttributeBefore(this ILProcessor processor, Instruction insert, CustomAttribute aspectAttribute)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// 属性を生成して、ローカル変数にストアします。
            var attributeType  = aspectAttribute.AttributeType.ToSystemType();
            var argumentTypes  = aspectAttribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = aspectAttribute.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case bool value:
                        if (value)
                        {
                            processor.InsertBefore(insert, OpCodes.Ldc_I4_1);
                        }
                        else
                        {
                            processor.InsertBefore(insert, OpCodes.Ldc_I4_0);
                        }
                        break;

                    case sbyte value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4_S, value);
                        break;

                    case short value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case int value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case long value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I8, value);
                        break;

                    case byte value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4_S, (sbyte)value);
                        break;

                    case ushort value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, (short)value);
                        break;

                    case uint value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, (int)value);
                        break;

                    case ulong value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I8, (long)value);
                        break;

                    case float value:
                        processor.InsertBefore(insert, OpCodes.Ldc_R4, value);
                        break;

                    case double value:
                        processor.InsertBefore(insert, OpCodes.Ldc_R8, value);
                        break;

                    case char value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case string value:
                        processor.InsertBefore(insert, OpCodes.Ldstr, value);
                        break;

                    case TypeReference value:
                        processor.InsertBefore(insert, OpCodes.Ldtoken, module.ImportReference(value));
                        processor.InsertBefore(insert, OpCodes.Call, module.ImportReference(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            processor.InsertBefore(insert, OpCodes.Newobj, module.ImportReference(attributeType.GetConstructor(argumentTypes.ToArray())));

            /// プロパティを設定します。
            foreach (var property in aspectAttribute.Properties)
            {
                var propertyName  = property.Name;
                var propertyValue = property.Argument.Value;

                processor.InsertBefore(insert, OpCodes.Dup);

                switch (propertyValue)
                {
                    case bool value:
                        if (value)
                        {
                            processor.InsertBefore(insert, OpCodes.Ldc_I4_1);
                        }
                        else
                        {
                            processor.InsertBefore(insert, OpCodes.Ldc_I4_0);
                        }
                        break;

                    case sbyte value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4_S, value);
                        break;

                    case short value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case int value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case long value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I8, value);
                        break;

                    case byte value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4_S, (sbyte)value);
                        break;

                    case ushort value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, (short)value);
                        break;

                    case uint value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, (int)value);
                        break;

                    case ulong value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I8, (long)value);
                        break;

                    case float value:
                        processor.InsertBefore(insert, OpCodes.Ldc_R4, value);
                        break;

                    case double value:
                        processor.InsertBefore(insert, OpCodes.Ldc_R8, value);
                        break;

                    case char value:
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, value);
                        break;

                    case string value:
                        processor.InsertBefore(insert, OpCodes.Ldstr, value);
                        break;

                    case TypeReference value:
                        processor.InsertBefore(insert, OpCodes.Ldtoken, module.ImportReference(value));
                        processor.InsertBefore(insert, OpCodes.Call, module.ImportReference(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))));
                        break;

                    default:
                        throw new NotSupportedException();
                }

                processor.InsertBefore(insert, OpCodes.Callvirt, module.ImportReference(attributeType.GetProperty(propertyName).GetSetMethod()));
            }
        }

        #endregion

        #region Emit

        /// <summary>
        /// 末尾に Branch 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>追加された Branch 命令。</returns>
        /// <remarks>
        /// Branch 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Branch 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Branch 命令を挿入し、挿入した Branch 命令を戻します。
        /// 転送先の命令が生成された後、Branch 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction EmitBranch(this ILProcessor processor, OpCode opcode)
        {
            var instruction = processor.CreateBranch(opcode);
            processor.Append(instruction);
            return instruction;
        }

        /// <summary>
        /// 末尾に Leave 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>追加された Leave 命令。</returns>
        /// <remarks>
        /// Leave 命令を生成するとき、オペランド (転送先の命令) を指定できないことがあります。
        /// 転送先の命令よりも先に Leave 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Leave 命令を挿入し、挿入した Leave 命令を戻します。
        /// 転送先の命令が生成された後、Leave 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction EmitLeave(this ILProcessor processor, OpCode opcode)
        {
            var instruction = processor.CreateLeave(opcode);
            processor.Append(instruction);
            return instruction;
        }

        /// <summary>
        /// 末尾に Nop 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <returns>追加された Nop 命令。</returns>
        public static Instruction EmitNop(this ILProcessor processor)
        {
            var instruction = processor.Create(OpCodes.Nop);
            processor.Append(instruction);
            return instruction;
        }

        /// <summary>
        /// 末尾に Ldind 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="typeReference">型参照。</param>
        internal static void EmitLdind(this ILProcessor processor, TypeReference typeReference)
        {
            if (!typeReference.IsValueType)
            {
                processor.Emit(OpCodes.Ldind_Ref);
                return;
            }

            switch (typeReference.Name)
            {
                case nameof(SByte):
                    processor.Emit(OpCodes.Ldind_I1);
                    return;

                case nameof(Char):
                case nameof(Int16):
                    processor.Emit(OpCodes.Ldind_I2);
                    return;

                case nameof(Int32):
                    processor.Emit(OpCodes.Ldind_I4);
                    return;

                case nameof(Int64):
                    processor.Emit(OpCodes.Ldind_I8);
                    return;

                case nameof(Byte):
                case nameof(Boolean):
                    processor.Emit(OpCodes.Ldind_U1);
                    return;

                case nameof(UInt16):
                    processor.Emit(OpCodes.Ldind_U2);
                    return;

                case nameof(UInt32):
                    processor.Emit(OpCodes.Ldind_U4);
                    return;

                case nameof(UInt64):
                    processor.Emit(OpCodes.Ldind_I8);
                    return;

                case nameof(Single):
                    processor.Emit(OpCodes.Ldind_R4);
                    return;

                case nameof(Double):
                    processor.Emit(OpCodes.Ldind_R8);
                    return;

                default:
                    processor.Emit(OpCodes.Ldobj, typeReference);
                    return;
            }
        }

        /// <summary>
        /// 末尾に Stind 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="typeReference">型参照。</param>
        internal static void EmitStind(this ILProcessor processor, TypeReference typeReference)
        {
            if (!typeReference.IsValueType)
            {
                processor.Emit(OpCodes.Stind_Ref);
                return;
            }

            switch (typeReference.Name)
            {
                case nameof(Boolean):
                case nameof(Byte):
                case nameof(SByte):
                    processor.Emit(OpCodes.Stind_I1);
                    return;

                case nameof(Char):
                case nameof(Int16):
                case nameof(UInt16):
                    processor.Emit(OpCodes.Stind_I2);
                    return;

                case nameof(Int32):
                case nameof(UInt32):
                    processor.Emit(OpCodes.Stind_I4);
                    return;

                case nameof(Int64):
                case nameof(UInt64):
                    processor.Emit(OpCodes.Stind_I8);
                    return;

                case nameof(Single):
                    processor.Emit(OpCodes.Stind_R4);
                    return;

                case nameof(Double):
                    processor.Emit(OpCodes.Stind_R8);
                    return;

                default:
                    processor.Emit(OpCodes.Stobj, typeReference);
                    return;
            }
        }

        /// <summary>
        /// 命令の前にアスペクト属性を追加するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">命令。</param>
        /// <param name="aspect">アスペクト属性。</param>
        /// <returns>アスペクト属性の変数インデックス。</returns>
        internal static void NewAspectAttribute(this ILProcessor processor, CustomAttribute aspect)
        {
            InsertNewAspectAttributeBefore(processor, null, aspect);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void NewArguments(this ILProcessor processor)
        {
            var method     = processor.Body.Method;
            var module     = method.Module;
            var parameters = method.Parameters;

            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();
            var argumentsType = parameters.Count switch
            {
                0 => typeof(Arguments),
                1 => typeof(Arguments<>).MakeGenericType(parameterTypes),
                2 => typeof(Arguments<,>).MakeGenericType(parameterTypes),
                3 => typeof(Arguments<,,>).MakeGenericType(parameterTypes),
                4 => typeof(Arguments<,,,>).MakeGenericType(parameterTypes),
                5 => typeof(Arguments<,,,,>).MakeGenericType(parameterTypes),
                6 => typeof(Arguments<,,,,,>).MakeGenericType(parameterTypes),
                7 => typeof(Arguments<,,,,,,>).MakeGenericType(parameterTypes),
                8 => typeof(Arguments<,,,,,,,>).MakeGenericType(parameterTypes),
                _ => typeof(ArgumentsArray),
            };

            /// Arguments を生成して、ローカル変数にストアします。
            if (parameters.Count <= 8)
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (method.IsStatic)
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();
                        processor.EmitLdind(elementType);
                    }
                }
                processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(parameters.Select(p => p.ParameterType.ToSystemType(removePointer: true)).ToArray())));
            }
            else
            {
                processor.Emit(OpCodes.Ldc_I4, parameters.Count);
                processor.Emit(OpCodes.Newarr, module.ImportReference(typeof(object)));
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.Emit(OpCodes.Dup);
                    processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                    if (method.IsStatic)
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else 
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.EmitLdind(elementType);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                    }
                    else
                    {
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                    }
                    processor.Emit(OpCodes.Stelem_Ref);
                }

                processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processor"></param>
        /// <param name="types"></param>
        internal static void New<T>(this ILProcessor processor, params Type[] types)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            processor.Emit(OpCodes.Newobj, module.ImportReference(typeof(T).GetConstructor(types)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processor"></param>
        /// <param name="types"></param>
        internal static void New(this ILProcessor processor, TypeDefinition type)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            processor.Emit(OpCodes.Newobj, module.ImportReference(type.Methods.Single(m => m.Name == ".ctor")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        internal static void SetProperty(this ILProcessor processor, Type type, string propertyName)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            processor.Emit(OpCodes.Call, module.ImportReference(type.GetProperty(propertyName).GetSetMethod()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        internal static void GetProperty(this ILProcessor processor, Type type, string propertyName)
        {
            var method = processor.Body.Method;
            var module = method.Module;

            processor.Emit(OpCodes.Call, module.ImportReference(type.GetProperty(propertyName).GetGetMethod()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="variable"></param>
        internal static void Store(this ILProcessor processor, int variable)
        {
            processor.Emit(OpCodes.Stloc, variable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        internal static void LoadThis(this ILProcessor processor)
        {
            var method = processor.Body.Method;

            if (!method.IsStatic)
            {
                processor.Emit(OpCodes.Ldarg_0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        internal static void LoadNull(this ILProcessor processor) 
        {
            processor.Emit(OpCodes.Ldnull);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        internal static void LoadArguments(this ILProcessor processor)
        {
            var method = processor.Body.Method;
            var parameters = method.Parameters;

            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                if (method.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="local"></param>
        internal static void Load(this ILProcessor processor, int local)
        {
            processor.Emit(OpCodes.Ldloc, local);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="method"></param>
        internal static void Call(this ILProcessor processor, MethodDefinition method)
        {
            processor.Emit(OpCodes.Call, method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        internal static void CallVirtual(this ILProcessor processor, TypeReference type, string methodName)
        {
            var module = processor.Body.Method.Module;
            var typeDefinition = type.Resolve();

            while (true)
            {
                var handler = typeDefinition.Methods.SingleOrDefault(m => m.Name == methodName);
                if (handler != null)
                {
                    processor.Emit(OpCodes.Callvirt, module.ImportReference(handler));
                    break;
                }
                else
                {
                    Assert.NotNull(typeDefinition.Resolve().BaseType);
                    typeDefinition = typeDefinition.Resolve().BaseType.Resolve();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void CallStatic(this ILProcessor processor, Type type, string methodName, params Type[] argumentTypes)
        {
            var module = processor.Body.Method.Module;
            processor.Emit(OpCodes.Call, module.ImportReference(type.GetMethod(methodName, argumentTypes)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="type"></param>
        internal static void Box(this ILProcessor processor, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="type"></param>
        internal static void Unbox(this ILProcessor processor, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        internal static void Rethrow(this ILProcessor processor)
        {
            processor.Emit(OpCodes.Rethrow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        internal static void Return(this ILProcessor processor)
        {
             processor.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 引数を更新します。
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="argumentsVariable"></param>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        internal static void UpdateArguments(this ILProcessor processor, int argumentsVariable, bool pointerOnly)
        {
            var method     = processor.Body.Method;
            var module     = method.Module;
            var parameters = method.Parameters;

            if (parameters.Count <= 8)
            {
                var fieldNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldfld, module.ImportReference(method.ArgumentsType().GetField(fieldNames[parameterIndex])));
                        processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldfld, module.ImportReference(method.ArgumentsType().GetField(fieldNames[parameterIndex])));
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }

                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox, elementType);
                            processor.Emit(OpCodes.Ldobj, elementType);
                        }
                        processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox_Any, parameterType);
                        }
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// AspectArgs.Arguments を更新します。
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="argumentsVariable"></param>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        internal static void UpdateArgumentsProperty(this ILProcessor processor, int argumentsVariable, bool pointerOnly)
        {
            var method     = processor.Body.Method;
            var module     = method.Module;
            var parameters = method.Parameters;

            if (parameters.Count <= 8)
            {
                var fieldNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.EmitLdind(elementType);
                        processor.Emit(OpCodes.Stfld, module.ImportReference(method.ArgumentsType().GetField(fieldNames[parameterIndex])));
                    }
                    else if (!pointerOnly)
                    {
                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Stfld, module.ImportReference(method.ArgumentsType().GetField(fieldNames[parameterIndex])));
                    }
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.EmitLdind(elementType);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (method.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
