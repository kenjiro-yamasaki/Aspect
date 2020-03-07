using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Logging;
using System;
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

        /// <summary>
        /// Arguments の型を取得します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>Arguments の型。</returns>
        public static Type ArgumentsType(this MethodDefinition method)
        {
            var parameters = method.Parameters;
            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();
            return parameters.Count switch
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
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        public static void InjectAdvice(this MethodDefinition method)
        {
            var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MethodLevelAspect)}";
            var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";

            foreach (var attribute in method.CustomAttributes.ToList())
            {
                var baseAttributeType = attribute.AttributeType.Resolve().BaseType.Resolve();

                while (baseAttributeType != null && baseAttributeType.BaseType != null)
                {
                    if (baseAttributeType.FullName == baseFullName && baseAttributeType.Scope.Name == baseScopeName)
                    {
                        var aspect = attribute.Create<MethodLevelAspect>();
                        aspect.Inject(method, attribute);
                        break;
                    }

                    baseAttributeType = baseAttributeType.BaseType.Resolve();
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

            Logger.Trace($"{method.FullName}");

            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Trace($"{instruction}");
            }

            foreach (var handler in processor.Body.ExceptionHandlers)
            {
                Logger.Trace($"TryStart     : {handler.TryStart}");
                Logger.Trace($"TryEnd       : {handler.TryEnd}");
                Logger.Trace($"HandlerStart : {handler.HandlerStart}");
                Logger.Trace($"HandlerEnd   : {handler.HandlerEnd}");
            }
        }



        
        public static int AddVariable(this MethodDefinition method, TypeReference type)
        {
            var variables = method.Body.Variables;
            var module    = method.Module;

            var variable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(type)));
        
            return variable;
        }

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
