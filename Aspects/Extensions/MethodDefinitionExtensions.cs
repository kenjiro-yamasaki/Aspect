using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="MethodDefinition"/> の拡張メソッド。
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        #region プロパティ

        /// <summary>
        /// 戻り値が存在するかを判断します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>戻り値が存在するか。</returns>
        public static bool HasReturnValue(this MethodDefinition method)
        {
            return method.ReturnType.FullName != "System.Void";
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="multicastAttributes">マルチキャスト属性コレクション。</param>
        public static void InjectAdvice(this MethodDefinition method, IEnumerable<MulticastAttribute> multicastAttributes)
        {
            using var profile = Profiling.Profiler.Start($"{nameof(MethodDefinitionExtensions)}.{nameof(InjectAdvice)}");

            // メソッドのマルチキャスト属性を生成します。
            var currentMulticastAttributes = new List<MulticastAttribute>();
            foreach (var customAttribute in method.CustomAttributes)
            {
                if (customAttribute.IsMulticastAttribute())
                {
                    var multicastAttribute = customAttribute.Create<MulticastAttribute>();
                    multicastAttribute.CustomAttribute = customAttribute;
                    currentMulticastAttributes.Add(multicastAttribute);
                }
            }
            multicastAttributes = multicastAttributes.Concat(currentMulticastAttributes.OrderBy(ma => ma.AttributePriority));

            // メソッドレベルアスペクトを適用します。
            foreach (var multicastAttribute in multicastAttributes)
            {
                if (multicastAttribute is MethodLevelAspect methodLevelAspect)
                {
                    methodLevelAspect.TargetMethod = method;
                    methodLevelAspect.InjectAdvice();
                }
            }
        }

        /// <summary>
        /// イテレーターステートマシン属性を取得します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>イテレーターステートマシン属性。</returns>
        public static CustomAttribute GetIteratorStateMachineAttribute(this MethodDefinition method)
        {
            return method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");
        }

        /// <summary>
        /// 非同期ステートマシン属性を取得します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>非同期ステートマシン属性。</returns>
        public static CustomAttribute GetAsyncStateMachineAttribute(this MethodDefinition method)
        {
            return method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
        }

        /// <summary>
        /// IL コードを最適化します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void Optimize(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();

            int offset = 0;
            foreach (var instruction in processor.Body.Instructions)
            {
                instruction.Offset = offset++;
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Br || i.OpCode == OpCodes.Br_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Br_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Br;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brtrue || i.OpCode == OpCodes.Brtrue_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Brtrue_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Brtrue;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brfalse || i.OpCode == OpCodes.Brfalse_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Brfalse_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Brfalse;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Leave || i.OpCode == OpCodes.Leave_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (sbyte.MinValue <= distance && distance <= sbyte.MaxValue)
                {
                    instruction.OpCode = OpCodes.Leave_S;
                }
                else
                {
                    instruction.OpCode = OpCodes.Leave;
                }
            }
        }

        /// <summary>
        /// IL コードをログ出力します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void Log(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();

            Logger.Info($"{method.FullName}");

            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Info($"{instruction}");
            }

            foreach (var handler in processor.Body.ExceptionHandlers)
            {
                Logger.Info($"TryStart     : {handler.TryStart}");
                Logger.Info($"TryEnd       : {handler.TryEnd}");
                Logger.Info($"HandlerStart : {handler.HandlerStart}");
                Logger.Info($"HandlerEnd   : {handler.HandlerEnd}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int AddVariable(this MethodDefinition method, TypeReference type)
        {
            var variables = method.Body.Variables;
            var module    = method.Module;

            var variable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(type)));

            return variable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int AddVariable(this MethodDefinition method, Type type)
        {
            var variables = method.Body.Variables;
            var module    = method.Module;

            var variable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(type)));
        
            return variable;
        }

        #endregion
    }
}
