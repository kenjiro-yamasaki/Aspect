using Mono.Cecil;
using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="CustomAttribute"/> の拡張メッソド。
    /// </summary>
    public static class CustomAttributeExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクトを生成します。
        /// </summary>
        /// <typeparam name="TAspect">アスペクトの型。</typeparam>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns>アスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute customAttribute)
            where TAspect : class
        {
            using var profile = Profiling.Profiler.Start($"{nameof(CustomAttributeExtensions)}.{nameof(Create)}");

            /// カスタム属性のコンストラクター引数を取得します。
            var arguments = new List<object>();
            foreach (var argument in customAttribute.ConstructorArguments)
            {
                var argumentType = argument.Type.ToSystemType();

                if (argumentType.IsEnum)
                {
                    arguments.Add(Enum.ToObject(argumentType, argument.Value));
                }
                else if (argumentType.FullName == "System.Type")
                {
                    var value = argument.Value as TypeReference;
                    arguments.Add(Type.GetType(value.FullName));
                }
                else
                {
                    arguments.Add(argument.Value);
                }
            }

            /// 属性のインスタンスを生成します。
            var aspectType = customAttribute.AttributeType.ToSystemType();
            var instance = Activator.CreateInstance(aspectType, arguments.ToArray()) as TAspect;

            /// 属性のプロパティを設定します。
            foreach (var property in customAttribute.Properties)
            {
                if (property.Argument.Type.FullName == "System.Type")
                {
                    var value = property.Argument.Value as TypeReference;
                    aspectType.GetProperty(property.Name).SetValue(instance, Type.GetType(value.FullName), null);
                }
                else 
                {
                    aspectType.GetProperty(property.Name).SetValue(instance, property.Argument.Value, null);
                }
            }

            return instance;
        }

        /// <summary>
        /// <see cref="MethodLevelAspect"/> かを判断します。
        /// </summary>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns><see cref="MethodLevelAspect"/> か。</returns>
        internal static bool IsMethodLevelAspect(this CustomAttribute customAttribute)
        {
            using var p = Profiling.Profiler.Start($"IsMethodLevelAspect");

            var baseCustomAttributeType = customAttribute.AttributeType.Resolve().BaseType.Resolve();
            while (baseCustomAttributeType != null && baseCustomAttributeType.BaseType != null)
            {
                var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MethodLevelAspect)}";
                var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";
                if (baseCustomAttributeType.FullName == baseFullName && baseCustomAttributeType.Scope.Name == baseScopeName)
                {
                    return true;
                }

                baseCustomAttributeType = baseCustomAttributeType.BaseType.Resolve();
            }

            return false;
        }

        #endregion
    }
}
