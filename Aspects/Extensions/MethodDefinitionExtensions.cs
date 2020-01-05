using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Log;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="MethodDefinition"/> の拡張メソッド。
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="methodDefinition">メソッド定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        public static void Inject(this MethodDefinition methodDefinition, Assembly assembly)
        {
            var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MethodLevelAspect)}";
            var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";

            foreach (var attribute in methodDefinition.CustomAttributes)
            {
                var baseAttributeType = attribute.AttributeType.Resolve().BaseType.Resolve();

                while (baseAttributeType != null && baseAttributeType.BaseType != null)
                {
                    if (baseAttributeType.FullName == baseFullName && baseAttributeType.Scope.Name == baseScopeName)
                    {
                        var aspect = attribute.Create<MethodLevelAspect>(assembly);
                        aspect.Inject(methodDefinition);
                        break;
                    }

                    baseAttributeType = baseAttributeType.BaseType.Resolve();
                }
            }
        }

        /// <summary>
        /// 戻り値が存在するかを判断します。
        /// </summary>
        /// <param name="methodDefinition">メソッド定義。</param>
        /// <returns>戻り値が存在するか。</returns>
        public static bool HasReturnValue(this MethodDefinition methodDefinition)
        {
            return methodDefinition.ReturnType.FullName != "System.Void";
        }

        /// <summary>
        /// IL コードを最適化します。
        /// </summary>
        /// <param name="methodDefinition">メソッド定義。</param>
        public static void OptimizeIL(this MethodDefinition methodDefinition)
        {
            var processor = methodDefinition.Body.GetILProcessor();

            int offset = 0;
            foreach (var instruction in processor.Body.Instructions)
            {
                instruction.Offset = offset++;
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Br_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (distance < sbyte.MinValue || sbyte.MaxValue < distance)
                {
                    instruction.OpCode = OpCodes.Br;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brtrue_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (distance < sbyte.MinValue || sbyte.MaxValue < distance)
                {
                    instruction.OpCode = OpCodes.Brtrue;
                }
            }

            foreach (var instruction in processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Brfalse_S))
            {
                var distance = instruction.DistanceTo((Instruction)instruction.Operand);
                if (distance < sbyte.MinValue || sbyte.MaxValue < distance)
                {
                    instruction.OpCode = OpCodes.Brfalse;
                }
            }
        }

        /// <summary>
        /// メソッドの内部状態をログ出力します (デバッグ用、削除可)。
        /// </summary>
        /// <param name="methodDefinition">メソッド定義。</param>
        public static void Log(this MethodDefinition methodDefinition)
        {
            var processor = methodDefinition.Body.GetILProcessor();

            Logger.Trace($"{methodDefinition.FullName}");

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

        #endregion
    }
}
