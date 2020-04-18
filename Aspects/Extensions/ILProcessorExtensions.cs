using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ILProcessor"/> の拡張メソッド。
    /// </summary>
    public static partial class ILProcessorExtensions
    {
        #region フィールド

        /// <summary>
        /// (型名, プロパティ名, 引数型配列) → コンストラクター。
        /// </summary>
        private static readonly Dictionary<string, MethodReference> MethodCache = new Dictionary<string, MethodReference>();

        #endregion

        #region メソッド

        #region 低レベル命令

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

        #endregion

        #endregion

        #region 高レベル命令

        #region GetMethodKey

        /// <summary>
        /// メソッドキーを取得します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="methodName">メソッド名。</param>
        /// <param name="argumentTypes">引数型配列。</param>
        /// <returns>メソッドキー。</returns>
        private static string GetMethodKey(Type type, string methodName, Type[] argumentTypes)
        {
            var sb = new StringBuilder();
            sb.Append(type.FullName);
            sb.Append(methodName);
            foreach (var argumentType in argumentTypes)
            {
                sb.Append(argumentType.FullName);
            }
            return sb.ToString();
        }

        /// <summary>
        /// メソッドキーを取得します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="methodName">メソッド名。</param>
        /// <returns>メソッドキー。</returns>
        private static string GetMethodKey(Type type, string methodName)
        {
            var sb = new StringBuilder();
            sb.Append(type.FullName);
            sb.Append(methodName);
            return sb.ToString();
        }

        /// <summary>
        /// メソッドキーを取得します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="methodName">メソッド名。</param>
        /// <returns>メソッドキー。</returns>
        private static string GetMethodKey(TypeReference type, string methodName)
        {
            var sb = new StringBuilder();
            sb.Append(type.FullName);
            sb.Append(methodName);
            return sb.ToString();
        }

        #endregion

        #region New

        /// <summary>
        /// 末尾に指定型のインスタンスを生成するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">生成するインスタンスの型。</param>
        /// <param name="argumentTypes">引数型配列。</param>
        public static void New(this ILProcessor processor, Type type, params Type[] argumentTypes)
        {
            var key = GetMethodKey(type, ".ctor", argumentTypes);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetConstructor(argumentTypes)));
            }
            processor.Emit(OpCodes.Newobj, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に指定型のインスタンスを生成するコードを追加します。
        /// </summary>
        /// <typeparam name="T">型。</typeparam>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="argumentTypes">引数型配列。</param>
        public static void New<T>(this ILProcessor processor, Instruction insert, params Type[] argumentTypes)
        {
            var key = GetMethodKey(typeof(T), ".ctor", argumentTypes);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(typeof(T).GetConstructor(argumentTypes)));
            }

            processor.InsertBefore(insert, OpCodes.Newobj, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に指定型のインスタンスを生成するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">型。</param>
        public static void New(this ILProcessor processor, TypeReference type)
        {
            var module = processor.Body.Method.Module;
            processor.Emit(OpCodes.Newobj, module.ImportReference(type.Resolve().Methods.Single(m => m.Name == ".ctor")));
        }

        /// <summary>
        /// 末尾にアスペクト属性を生成するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public static void NewAspectAttribute(this ILProcessor processor, CustomAttribute aspectAttribute)
        {
            NewAspectAttribute(processor, null, aspectAttribute);
        }

        /// <summary>
        /// 指定命令の前にアスペクト属性を生成するコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public static void NewAspectAttribute(this ILProcessor processor, Instruction insert, CustomAttribute aspectAttribute)
        {
            var method       = processor.Body.Method;
            var module       = method.Module;
            var instructions = method.Body.Instructions;

            // 属性を生成して、ローカル変数にストアします。
            var attributeType       = aspectAttribute.AttributeType.Resolve();
            var attributeProperties = attributeType.GetProperties();
            var argumentTypes       = aspectAttribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues      = aspectAttribute.ConstructorArguments.Select(a => a.Value);
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

            processor.InsertBefore(insert, OpCodes.Newobj, module.ImportReference(attributeType.Methods.SingleOrDefault(m => m.Name == ".ctor")));

            // プロパティを設定します。
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

                processor.InsertBefore(insert, OpCodes.Callvirt, module.ImportReference(attributeProperties.Single(p => p.Name == propertyName).SetMethod));
            }
        }

        /// <summary>
        /// 末尾に Arguments を生成するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public static void NewArguments(this ILProcessor processor)
        {
            var method         = processor.Body.Method;
            var module         = method.Module;
            var parameters     = method.Parameters;
            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();

            var key = GetMethodKey(typeof(Arguments), ".ctor", new Type[] { typeof(object[]) });
            if (!MethodCache.ContainsKey(key))
            {
                MethodCache.Add(key, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) })));
            }

            // Arguments を生成して、ローカル変数にストアします。
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

            processor.Emit(OpCodes.Newobj, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に Arguments を生成するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <remarks>
        /// ステートマシンの Arguments を生成する場合、このメソッドを使用します。
        /// </remarks>
        public static void NewArguments(this ILProcessor processor, MethodDefinition targetMethod)
        {
            NewArguments(processor, null, targetMethod);
        }

        /// <summary>
        /// 指定命令の前に Arguments を生成するコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <remarks>
        /// ステートマシンの Arguments を生成する場合、このメソッドを使用します。
        /// </remarks>
        public static void NewArguments(this ILProcessor processor, Instruction insert, MethodDefinition targetMethod)
        {
            var method           = processor.Body.Method;
            var module           = method.Module;
            var stateMachineType = method.DeclaringType;
            var parameters       = targetMethod.Parameters;
            var parameterTypes   = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();

            var key = GetMethodKey(typeof(Arguments), ".ctor", new Type[] { typeof(object[]) });
            if (!MethodCache.ContainsKey(key))
            {
                MethodCache.Add(key, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) })));
            }

            processor.InsertBefore(insert, OpCodes.Ldc_I4, parameters.Count);
            processor.InsertBefore(insert, OpCodes.Newarr, module.ImportReference(typeof(object)));
            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                processor.InsertBefore(insert, OpCodes.Dup);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, parameterIndex);
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, stateMachineType.Fields.Single(f => f.Name == parameter.Name));
                if (parameter.ParameterType.IsValueType)
                {
                    processor.InsertBefore(insert, OpCodes.Box, parameter.ParameterType);
                }
                processor.InsertBefore(insert, OpCodes.Stelem_Ref);
            }

            processor.InsertBefore(insert, OpCodes.Newobj, MethodCache[key]);
        }

        #endregion

        #region Load

        /// <summary>
        /// 末尾にローカル変数をロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="variable">ローカル変数のインデックス。</param>
        public static void Load(this ILProcessor processor, int variable)
        {
            processor.Emit(OpCodes.Ldloc, variable);
        }

        /// <summary>
        /// 末尾にローカル変数のアドレスをロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="variable">ローカル変数のインデックス。</param>
        public static void LoadAddress(this ILProcessor processor, int variable)
        {
            processor.Emit(OpCodes.Ldloca, variable);
        }

        /// <summary>
        /// 指定命令の前にローカル変数をロードするコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="variable">ローカル変数のインデックス。</param>
        public static void Load(this ILProcessor processor, Instruction insert, int variable)
        {
            processor.InsertBefore(insert, OpCodes.Ldloc, variable);
        }

        /// <summary>
        /// 末尾にフィールドをロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="field">フィールド。</param>
        public static void Load(this ILProcessor processor, FieldReference field)
        {
            processor.Emit(OpCodes.Ldfld, field);
        }

        /// <summary>
        /// 末尾にフィールドのアドレスをロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="field">フィールド。</param>
        public static void LoadAddress(this ILProcessor processor, FieldReference field)
        {
            processor.Emit(OpCodes.Ldflda, field);
        }

        /// <summary>
        /// 指定命令の前にフィールドをロードするコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="field">フィールド。</param>
        public static void Load(this ILProcessor processor, Instruction insert, FieldReference field)
        {
            processor.InsertBefore(insert, OpCodes.Ldfld, field);
        }

        /// <summary>
        /// 末尾に this (第 0 引数) をロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <remarks>
        /// 静的メソッドの場合、コードを追加しません。
        /// </remarks>
        public static void LoadThis(this ILProcessor processor)
        {
            var method = processor.Body.Method;

            if (!method.IsStatic)
            {
                processor.Emit(OpCodes.Ldarg_0);
            }
        }

        /// <summary>
        /// 指定命令の前に this (第 0 引数) をロードするコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <remarks>
        /// 静的メソッドの場合、コードを追加しません。
        /// </remarks>
        public static void LoadThis(this ILProcessor processor, Instruction insert)
        {
            var method = processor.Body.Method;

            if (!method.IsStatic)
            {
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
            }
        }

        /// <summary>
        /// 末尾に null をロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public static void LoadNull(this ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldnull);
        }

        /// <summary>
        /// 指定命令の前に null をロードするコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        public static void LoadNull(this ILProcessor processor, Instruction insert)
        {
            processor.InsertBefore(insert, OpCodes.Ldnull);
        }

        /// <summary>
        /// 末尾に引数をロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public static void LoadArguments(this ILProcessor processor)
        {
            var method     = processor.Body.Method;
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
        /// 末尾に指定メソッドのメソッド情報 <see cref="System.Reflection.MethodBase"/> をロードするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="method">メソッド。</param>
        public static void LoadMethodBase(this ILProcessor processor, MethodDefinition method)
        {
            var module     = method.Module;
            var parameters = method.Parameters;

            //
            processor.Emit(OpCodes.Ldstr, method.DeclaringType.ToSystemType().FullName);
            processor.CallStatic(typeof(Type), nameof(Type.GetType), new [] { typeof(string) });
            if (!method.IsConstructor)
            {
                processor.Emit(OpCodes.Ldstr, method.Name);
            }

            processor.Emit(OpCodes.Ldc_I4, parameters.Count);
            processor.Emit(OpCodes.Newarr, module.ImportReference(typeof(Type)));
            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                var parameterType = parameter.ParameterType;

                processor.Emit(OpCodes.Dup);
                processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                if (parameterType.IsByReference)
                {
                    var elementType = parameterType.GetElementType();
                    processor.Emit(OpCodes.Ldstr, elementType.FullName);
                    processor.CallStatic(typeof(Type), nameof(Type.GetType), new [] { typeof(string) });
                    processor.CallVirtual(typeof(Type).GetMethod(nameof(Type.MakeByRefType)));
                    processor.Emit(OpCodes.Stelem_Ref);
                }
                else
                {
                    processor.Emit(OpCodes.Ldstr, parameterType.FullName);
                    processor.CallStatic(typeof(Type), nameof(Type.GetType), new [] { typeof(string) });
                    processor.Emit(OpCodes.Stelem_Ref);
                }
            }

            if (method.IsConstructor)
            {
                processor.CallVirtual(typeof(Type), nameof(Type.GetConstructor), new[] { typeof(Type[]) });
            }
            else
            {
                processor.CallVirtual(typeof(Type), nameof(Type.GetMethod), new [] { typeof(string), typeof(Type[]) });
            }
        }

        #endregion

        #region Store

        /// <summary>
        /// 末尾にローカル変数をストアするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="variable">ローカル変数のインデックス。</param>
        public static void Store(this ILProcessor processor, int variable)
        {
            processor.Emit(OpCodes.Stloc, variable);
        }

        /// <summary>
        /// 指定命令の前にローカル変数をストアするコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="variable">ローカル変数のインデックス。</param>
        public static void Store(this ILProcessor processor, Instruction insert, int variable)
        {
            processor.InsertBefore(insert, OpCodes.Stloc, variable);
        }

        /// <summary>
        /// 末尾にフィールドをストアするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="field">フィールド。</param>
        public static void Store(this ILProcessor processor, FieldReference field)
        {
            processor.Emit(OpCodes.Stfld, field);
        }

        /// <summary>
        /// 指定命令の前にフィールドをストアするコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="field">フィールド。</param>
        public static void Store(this ILProcessor processor, Instruction insert, FieldReference field)
        {
            processor.InsertBefore(insert, OpCodes.Stfld, field);
        }

        #endregion

        #region Call

        /// <summary>
        /// 末尾にメソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="method">メソッド。</param>
        public static void Call(this ILProcessor processor, MethodReference method)
        {
            processor.Emit(OpCodes.Call, method);
        }

        /// <summary>
        /// 末尾にメソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="method">メソッド。</param>
        public static void Call(this ILProcessor processor, System.Reflection.MethodInfo method)
        {
            var module = processor.Body.Method.Module;

            processor.Emit(OpCodes.Call, module.ImportReference(method));
        }

        /// <summary>
        /// 末尾に仮想メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="method">メソッド。</param>
        public static void CallVirtual(this ILProcessor processor, MethodReference method)
        {
            processor.Emit(OpCodes.Callvirt, method);
        }

        /// <summary>
        /// 末尾に仮想メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="method">メソッド。</param>
        public static void CallVirtual(this ILProcessor processor, System.Reflection.MethodInfo method)
        {
            var module = processor.Body.Method.Module;

            processor.Emit(OpCodes.Callvirt, module.ImportReference(method));
        }

        /// <summary>
        /// 末尾に仮想メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="methodName">メソッド名。</param>
        public static void CallVirtual(this ILProcessor processor, Type type, string methodName)
        {
            var key = GetMethodKey(type, methodName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetMethod(methodName)));
            }

            processor.Emit(OpCodes.Callvirt, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に仮想メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="methodName">メソッド名。</param>
        public static void CallVirtual(this ILProcessor processor, Type type, string methodName, Type[] argumentTypes)
        {
            var key = GetMethodKey(type, methodName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetMethod(methodName, argumentTypes)));
            }

            processor.Emit(OpCodes.Callvirt, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に仮想メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="methodName">メソッド名。</param>
        public static void CallVirtual(this ILProcessor processor, TypeDefinition type, string methodName)
        {
            var key = GetMethodKey(type, methodName);
            if (!MethodCache.ContainsKey(key))
            {
                var currentType = type;
                while (true)
                {
                    var method = currentType.Methods.SingleOrDefault(m => m.Name == methodName);
                    if (method != null)
                    {
                        var module = processor.Body.Method.Module;
                        MethodCache.Add(key, module.ImportReference(method));
                        break;
                    }
                    else
                    {
                        currentType = currentType.BaseType.Resolve();
                    }
                }
            }

            processor.Emit(OpCodes.Callvirt, MethodCache[key]);
        }

        /// <summary>
        /// 指定命令の前に仮想メソッドを呼びだすコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="methodName">メソッド名。</param>
        public static void CallVirtual(this ILProcessor processor, Instruction insert, TypeDefinition type, string methodName)
        {
            var key = GetMethodKey(type, methodName);
            if (!MethodCache.ContainsKey(key))
            {
                var currentType = type;
                while (true)
                {
                    var method = currentType.Methods.SingleOrDefault(m => m.Name == methodName);
                    if (method != null)
                    {
                        var module = processor.Body.Method.Module;
                        MethodCache.Add(key, module.ImportReference(method));
                        break;
                    }
                    else
                    {
                        currentType = currentType.BaseType.Resolve();
                    }
                }
            }

            processor.InsertBefore(insert, OpCodes.Callvirt, MethodCache[key]);
        }

        /// <summary>
        /// 末尾に静的メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="methodName">メソッド名。</param>
        /// <param name="argumentTypes">引数型配列。</param>
        public static void CallStatic(this ILProcessor processor, Type type, string methodName, params Type[] argumentTypes)
        {
            var module = processor.Body.Method.Module;
            processor.Emit(OpCodes.Call, module.ImportReference(type.GetMethod(methodName, argumentTypes)));
        }

        /// <summary>
        /// 末尾にコンストラクターを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">メソッドの宣言型。</param>
        /// <param name="argumentTypes">引数型配列。</param>
        public static void CallConstructor(this ILProcessor processor, Type type, Type[] argumentTypes)
        {
            var key = GetMethodKey(type, ".ctor", argumentTypes);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetConstructor(argumentTypes)));
            }

            processor.Emit(OpCodes.Call, MethodCache[key]);
        }

        #endregion

        #region Property

        /// <summary>
        /// 末尾にプロパティの set メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">プロパティの宣言型。</param>
        /// <param name="propertyName">プロパティ名。</param>
        public static void SetProperty(this ILProcessor processor, Type type, string propertyName)
        {
            var key = GetMethodKey(type, "set_" + propertyName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetProperty(propertyName).GetSetMethod()));
            }

            processor.Emit(OpCodes.Call, MethodCache[key]);
        }

        /// <summary>
        /// 指定命令の前にプロパティの set メソッドを呼びだすコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="type">プロパティの宣言型。</param>
        /// <param name="propertyName">プロパティ名。</param>
        public static void SetProperty(this ILProcessor processor, Instruction insert, Type type, string propertyName)
        {
            var key = GetMethodKey(type, "set_" + propertyName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetProperty(propertyName).GetSetMethod()));
            }

            processor.InsertBefore(insert, OpCodes.Call, MethodCache[key]);
        }

        /// <summary>
        /// 末尾にプロパティの get メソッドを呼びだすコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">プロパティの宣言型。</param>
        /// <param name="propertyName">プロパティ名。</param>
        public static void GetProperty(this ILProcessor processor, Type type, string propertyName)
        {
            var key = GetMethodKey(type, "get_" + propertyName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetProperty(propertyName).GetGetMethod()));
            }

            processor.Emit(OpCodes.Call, MethodCache[key]);
        }

        /// <summary>
        /// 指定命令の前にプロパティの get メソッドを呼びだすコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="type">プロパティの宣言型。</param>
        /// <param name="propertyName">プロパティ名。</param>
        public static void GetProperty(this ILProcessor processor, Instruction insert, Type type, string propertyName)
        {
            var key = GetMethodKey(type, "get_" + propertyName);
            if (!MethodCache.ContainsKey(key))
            {
                var module = processor.Body.Method.Module;
                MethodCache.Add(key, module.ImportReference(type.GetProperty(propertyName).GetGetMethod()));
            }

            processor.InsertBefore(insert, OpCodes.Call, MethodCache[key]);
        }

        #endregion

        #region Box

        /// <summary>
        /// 末尾に Box 化のコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">型。</param>
        public static void Box(this ILProcessor processor, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// 指定命令の前に Box 化のコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="type">型。</param>
        public static void Box(this ILProcessor processor, Instruction insert, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.InsertBefore(insert, OpCodes.Box, type);
            }
        }

        /// <summary>
        /// 末尾に Box 化解除のコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="type">型。</param>
        public static void Unbox(this ILProcessor processor, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, type);
            }
        }

        /// <summary>
        /// 指定命令の前に Box 化解除のコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="type">型。</param>
        public static void Unbox(this ILProcessor processor, Instruction insert, TypeReference type)
        {
            if (type.IsValueType)
            {
                processor.InsertBefore(insert, OpCodes.Unbox_Any, type);
            }
        }

        #endregion

        #region Rethrow

        /// <summary>
        /// 末尾に rethrow 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public static void Rethrow(this ILProcessor processor)
        {
            processor.Emit(OpCodes.Rethrow);
        }

        #endregion

        #region Return

        /// <summary>
        /// 末尾に return 命令を追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public static void Return(this ILProcessor processor)
        {
             processor.Emit(OpCodes.Ret);
        }

        #endregion

        #region Update

        /// <summary>
        /// 末尾に引数を更新するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="argumentsVariable">Arguments ローカル変数。</param>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public static void UpdateArguments(this ILProcessor processor, int argumentsVariable, bool pointerOnly)
        {
            var method     = processor.Body.Method;
            var module     = method.Module;
            var parameters = method.Parameters;

            var key = GetMethodKey(typeof(Arguments), "get_Item");
            if (!MethodCache.ContainsKey(key))
            {
                MethodCache.Add(key, module.ImportReference(module.ImportReference(typeof(Arguments)).Resolve().Properties.Single(p => p.Name == "Item").GetMethod));
            }

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
                    processor.Emit(OpCodes.Call, MethodCache[key]);
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
                    processor.Emit(OpCodes.Call, MethodCache[key]);
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

        /// <summary>
        /// 末尾に引数を更新するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="argumentsField">Arguments フィールド。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <remarks>
        /// ターゲットメソッドの引数は、ステートマシンの引数フィールド (引数と同名のフィールド) に転送されます。
        /// ステートマシンの引数フィールドを更新する場合、このメソッドを使用します。
        /// </remarks>
        public static void UpdateArguments(this ILProcessor processor, FieldDefinition argumentsField, MethodDefinition targetMethod)
        {
            UpdateArguments(processor, null, argumentsField, targetMethod);
        }

        /// <summary>
        /// 指定命令の前に引数を更新するコードを挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="argumentsField">Arguments フィールド。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <remarks>
        /// ターゲットメソッドの引数は、ステートマシンの引数フィールド (引数と同名のフィールド) に転送されます。
        /// ステートマシンの引数フィールドを更新する場合、このメソッドを使用します。
        /// </remarks>
        public static void UpdateArguments(this ILProcessor processor, Instruction insert, FieldDefinition argumentsField, MethodDefinition targetMethod)
        {
            var method           = processor.Body.Method;
            var module           = method.Module;
            var stateMachineType = method.DeclaringType;
            var parameters       = targetMethod.Parameters;
            var parameterTypes   = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

            var key = GetMethodKey(typeof(Arguments), "get_Item");
            if (!MethodCache.ContainsKey(key))
            {
                MethodCache.Add(key, module.ImportReference(module.ImportReference(typeof(Arguments)).Resolve().Properties.Single(p => p.Name == "Item").GetMethod));
            }

            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                var parameterType = parameter.ParameterType;

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Dup);
                processor.InsertBefore(insert, OpCodes.Ldfld, argumentsField);
                processor.InsertBefore(insert, OpCodes.Ldc_I4, parameterIndex);

                processor.InsertBefore(insert, OpCodes.Call, MethodCache[key]);
                if (parameterType.IsValueType)
                {
                    processor.InsertBefore(insert, OpCodes.Unbox_Any, parameterType);
                }
                processor.InsertBefore(insert, OpCodes.Stfld, stateMachineType.Fields.Single(f => f.Name == parameter.Name));
            }
        }

        /// <summary>
        /// 末尾に AspectArgs.Arguments を更新するコードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="argumentsVariable">Arguments ローカル変数。</param>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public static void UpdateArgumentsProperty(this ILProcessor processor, int argumentsVariable, bool pointerOnly)
        {
            var method     = processor.Body.Method;
            var module     = method.Module;
            var parameters = method.Parameters;

            var key = GetMethodKey(typeof(Arguments), nameof(Arguments.SetArgument));
            if (!MethodCache.ContainsKey(key))
            {
                MethodCache.Add(key, module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
            }

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
                    processor.Emit(OpCodes.Callvirt, MethodCache[key]);
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
                    processor.Emit(OpCodes.Callvirt, MethodCache[key]);
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
