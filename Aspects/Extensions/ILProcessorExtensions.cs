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
        /// Branch 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
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
        /// Leave 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
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

        #region InsertBefore

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.Create(opcode);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, int operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, long operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, byte operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, sbyte operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, float operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, double operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, string operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, TypeReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, MethodReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, FieldReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, ParameterDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, VariableDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction[] operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode, CallSite operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の前に Branch 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された Branch 命令。</returns>
        /// <remarks>
        /// Branch 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
        /// 転送先の命令よりも先に Branch 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Branch 命令を挿入し、挿入した Branch 命令を戻します。
        /// 転送先の命令が生成された後、Branch 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertBranchBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateBranch(opcode);
            processor.InsertBefore(insert, instruction);
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
        /// Leave 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
        /// 転送先の命令よりも先に Leave 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Leave 命令を挿入し、挿入した Leave 命令を戻します。
        /// 転送先の命令が生成された後、Leave 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertLeaveBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateLeave(opcode);
            processor.InsertBefore(insert, instruction);
            return instruction;
        }

        #endregion

        #region InsertAfter

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.Create(opcode);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, int operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, long operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, byte operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, sbyte operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, float operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, double operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, string operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, TypeReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, MethodReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, FieldReference operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, ParameterDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, VariableDefinition operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, Instruction[] operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <param name="operand">オペランド。</param>
        /// <returns>挿入された命令。</returns>
        public static Instruction InsertAfter(this ILProcessor processor, Instruction insert, OpCode opcode, CallSite operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に Branch 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された Branch 命令。</returns>
        /// <remarks>
        /// Branch 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
        /// 転送先の命令よりも先に Branch 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Branch 命令を挿入し、挿入した Branch 命令を戻します。
        /// 転送先の命令が生成された後、Branch 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertBranchAfter(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateBranch(opcode);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        /// <summary>
        /// 指定命令の後に Leave 命令を挿入します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令。</param>
        /// <param name="opcode">オペコード。</param>
        /// <returns>挿入された Leave 命令。</returns>
        /// <remarks>
        /// Leave 命令を生成するとき、オペランド (転送先の命令) が指定できないことがあります。
        /// 転送先の命令よりも先に Leave 命令を生成することが一般的であるため、このような状況は良く起こります。
        /// このメソッドは、オペランド (転送先の命令) が <c>null</c> の Leave 命令を挿入し、挿入した Leave 命令を戻します。
        /// 転送先の命令が生成された後、Leave 命令のオペランドを設定してください。
        /// </remarks>
        public static Instruction InsertLeaveAfter(this ILProcessor processor, Instruction insert, OpCode opcode)
        {
            var instruction = processor.CreateLeave(opcode);
            processor.InsertAfter(insert, instruction);
            return instruction;
        }

        #endregion

        /// <summary>
        /// 命令コレクションの末尾に属性を追加する IL コードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="attribute">属性。</param>
        /// <returns>属性の変数インデックス。</returns>
        internal static int Emit(this ILProcessor processor, CustomAttribute attribute)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// ローカル変数を追加します。
            var variables      = method.Body.Variables;
            var attributeIndex = variables.Count();
            var attributeType  = attribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(attributeType)));

            /// 属性を生成して、ローカル変数にストアします。
            var argumentTypes = attribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = attribute.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case int @int:
                        processor.Emit(OpCodes.Ldc_I4, @int);
                        break;

                    case string @string:
                        processor.Emit(OpCodes.Ldstr, @string);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            processor.Emit(OpCodes.Newobj, module.ImportReference(attributeType.GetConstructor(argumentTypes.ToArray())));
            processor.Emit(OpCodes.Stloc, attributeIndex);

            /// プロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.Argument.Value;

                processor.Emit(OpCodes.Ldloc, attributeIndex);

                switch (propertyValue)
                {
                    case int @int:
                        processor.Emit(OpCodes.Ldc_I4, @int);
                        break;

                    case string @string:
                        processor.Emit(OpCodes.Ldstr, @string);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                processor.Emit(OpCodes.Callvirt, module.ImportReference(attributeType.GetProperty(propertyName).GetSetMethod()));
            }

            return attributeIndex;
        }

        /// <summary>
        /// 命令の前に属性を追加する IL コードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="instruction">命令。</param>
        /// <param name="attribute">属性。</param>
        /// <returns>属性の変数インデックス。</returns>
        internal static int InsertBefore(this ILProcessor processor, Instruction instruction, CustomAttribute attribute)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// ローカル変数を追加します。
            var variables      = method.Body.Variables;
            var attributeIndex = variables.Count();
            var attributeType  = attribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(attributeType)));

            /// 属性を生成して、ローカル変数にストアします。
            var argumentTypes  = attribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = attribute.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case int @int:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldc_I4, @int));
                        break;

                    case string @string:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldstr, @string));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            processor.InsertBefore(instruction, processor.Create(OpCodes.Newobj, module.ImportReference(attributeType.GetConstructor(argumentTypes.ToArray()))));
            processor.InsertBefore(instruction, processor.Create(OpCodes.Stloc, attributeIndex));

            /// プロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                var propertyName  = property.Name;
                var propertyValue = property.Argument.Value;

                processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, attributeIndex));

                switch (propertyValue)
                {
                    case int @int:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldc_I4, @int));
                        break;

                    case string @string:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldstr, @string));
                        break;

                    default:
                        throw new NotSupportedException();
                }

                processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, module.ImportReference(attributeType.GetProperty(propertyName).GetSetMethod())));
            }

            return attributeIndex;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal static Instruction EmitAndReturn(this ILProcessor processor, OpCode opcode)
        {
            if (opcode == OpCodes.Br || opcode == OpCodes.Br_S || opcode == OpCodes.Beq || opcode == OpCodes.Beq_S || opcode == OpCodes.Brtrue || opcode == OpCodes.Brtrue_S || opcode == OpCodes.Brfalse || opcode == OpCodes.Brfalse_S || opcode == OpCodes.Leave || opcode == OpCodes.Leave_S)
            {
                var instruction = processor.Create(OpCodes.Nop);
                processor.Append(instruction);
                instruction.OpCode = opcode;
                return instruction;
            }
            else
            {
                var instruction = processor.Create(opcode);
                processor.Append(instruction);
                return instruction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal static Instruction EmitAndReturn(this ILProcessor processor, OpCode opcode, int operand)
        {
            var instruction = processor.Create(opcode, operand);
            processor.Append(instruction);
            return instruction;
        }








        //internal static void InsertBefore(this ILProcessor processor, Instruction insert, OpCode opcode)
        //{
        //    if (opcode == OpCodes.Br || opcode == OpCodes.Br_S || opcode == OpCodes.Beq || opcode == OpCodes.Beq_S || opcode == OpCodes.Brtrue || opcode == OpCodes.Brtrue_S || opcode == OpCodes.Brfalse || opcode == OpCodes.Brfalse_S || opcode == OpCodes.Leave || opcode == OpCodes.Leave_S)
        //    {
        //        var instruction = processor.Create(OpCodes.Nop);
        //        processor.InsertBefore(insert, instruction);
        //        instruction.OpCode = opcode;
        //    }
        //    else
        //    {
        //        var instruction = processor.Create(opcode);
        //        processor.InsertBefore(insert, instruction);
        //    }
        //}

        //internal static Instruction InsertBeforeAndReturn(this ILProcessor processor, Instruction insert, OpCode opcode)
        //{
        //    if (opcode == OpCodes.Br || opcode == OpCodes.Br_S || opcode == OpCodes.Beq || opcode == OpCodes.Beq_S || opcode == OpCodes.Brtrue || opcode == OpCodes.Brtrue_S || opcode == OpCodes.Brfalse || opcode == OpCodes.Brfalse_S || opcode == OpCodes.Leave || opcode == OpCodes.Leave_S)
        //    {
        //        var instruction = processor.Create(OpCodes.Nop);
        //        processor.InsertBefore(insert, instruction);
        //        instruction.OpCode = opcode;
        //        return instruction;
        //    }
        //    else
        //    {
        //        var instruction = processor.Create(opcode);
        //        processor.InsertBefore(insert, instruction);
        //        return instruction;
        //    }
        //}

        #endregion
    }
}
