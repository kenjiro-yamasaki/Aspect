using Mono.Cecil;
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
        /// <param name="aspect">アスペクト属性。</param>
        /// <returns>アスペクト属性の変数インデックス。</returns>
        internal static int InsertCreateAspectCodeBefore(this ILProcessor processor, Instruction insert, CustomAttribute aspect)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// ローカル変数を追加します。
            var variables      = method.Body.Variables;
            var attributeIndex = variables.Count();
            var attributeType  = aspect.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(attributeType)));

            /// 属性を生成して、ローカル変数にストアします。
            var argumentTypes  = aspect.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = aspect.ConstructorArguments.Select(a => a.Value);
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
            processor.InsertBefore(insert, OpCodes.Stloc, attributeIndex);

            /// プロパティを設定します。
            foreach (var property in aspect.Properties)
            {
                var propertyName  = property.Name;
                var propertyValue = property.Argument.Value;

                processor.InsertBefore(insert, OpCodes.Ldloc, attributeIndex);

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

            return attributeIndex;
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
        /// 命令の前にアスペクト属性を追加するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">命令。</param>
        /// <param name="aspect">アスペクト属性。</param>
        /// <returns>アスペクト属性の変数インデックス。</returns>
        internal static int EmitCreateAspectCode(this ILProcessor processor, CustomAttribute aspect)
        {
            return InsertCreateAspectCodeBefore(processor, null, aspect);
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

        #endregion

        #endregion
    }
}
