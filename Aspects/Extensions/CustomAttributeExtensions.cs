using Mono.Cecil;
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
        /// 属性を生成します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型。</typeparam>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns>属性。</returns>
        internal static TAttribute Create<TAttribute>(this CustomAttribute customAttribute)
            where TAttribute : class
        {
            // 属性のコンストラクター引数を取得します。
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

            // 属性のインスタンスを生成します。
            var attributeType = customAttribute.AttributeType.ToSystemType();
            var instance = Activator.CreateInstance(attributeType, arguments.ToArray()) as TAttribute;

            // 属性のプロパティを設定します。
            foreach (var property in customAttribute.Properties)
            {
                if (property.Argument.Type.FullName == "System.Type")
                {
                    var value = property.Argument.Value as TypeReference;
                    attributeType.GetProperty(property.Name).SetValue(instance, Type.GetType(value.FullName), null);
                }
                else 
                {
                    attributeType.GetProperty(property.Name).SetValue(instance, property.Argument.Value, null);
                }
            }

            return instance;
        }

        /// <summary>
        /// <see cref="MulticastAttribute"/> かを判断します。
        /// </summary>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns><see cref="MulticastAttribute"/> か。</returns>
        internal static bool IsMulticastAttribute(this CustomAttribute customAttribute)
        {
            var baseCustomAttributeType = customAttribute.AttributeType.Resolve().BaseType.Resolve();
            while (baseCustomAttributeType != null && baseCustomAttributeType.BaseType != null)
            {
                var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MulticastAttribute)}";
                var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";
                if (baseCustomAttributeType.FullName == baseFullName && baseCustomAttributeType.Scope.Name == baseScopeName)
                {
                    return true;
                }

                baseCustomAttributeType = baseCustomAttributeType.BaseType.Resolve();
            }

            return false;
        }

        /// <summary>
        /// <see cref="MethodLevelAspect"/> かを判断します。
        /// </summary>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns><see cref="MethodLevelAspect"/> か。</returns>
        internal static bool IsMethodLevelAspect(this CustomAttribute customAttribute)
        {
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
