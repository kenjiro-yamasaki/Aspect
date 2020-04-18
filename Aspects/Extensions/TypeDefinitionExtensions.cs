using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="TypeDefinition"/> の拡張メソッド。
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// 型にアドバイスを注入します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="multicastAttributes">マルチキャスト属性コレクション。</param>
        internal static void InjectAdvice(this TypeDefinition type, IEnumerable<MulticastAttribute> multicastAttributes)
        {
            // 「CompilerGenerated」属性を持つ型を無視します。
            if (type.CustomAttributes.Any(ca => ca.AttributeType.Name == "CompilerGeneratedAttribute"))
            {
                return;
            }

            // モジュールのマルチキャスト属性を生成します。
            var currentMulticastAttributes = new List<MulticastAttribute>();
            foreach (var customAttribute in type.CustomAttributes.ToList())
            {
                if (customAttribute.IsMulticastAttribute())
                {
                    var multicastAttribute = customAttribute.Create<MulticastAttribute>();
                    multicastAttribute.CustomAttribute = customAttribute;
                    currentMulticastAttributes.Add(multicastAttribute);

                    type.CustomAttributes.Remove(customAttribute);
                }
            }
            multicastAttributes = multicastAttributes.Concat(currentMulticastAttributes.OrderBy(ma => ma.AttributePriority));

            // メソッドにアドバイスを注入します。
            foreach (var method in type.Methods.ToArray())
            {
                method.InjectAdvice(multicastAttributes);
            }

            // ネストされた型にアドバイスを注入します。
            foreach (var nestedType in type.NestedTypes)
            {
                nestedType.InjectAdvice(multicastAttributes);
            }
        }

        /// <summary>
        /// 型にカスタム属性を追加します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="customAttributeType">カスタム属性の型。</param>
        public static void AddCustomAttribute(this TypeDefinition type, Type customAttributeType)
        {
            var module = type.Module;

            var constructor = module.ImportReference(customAttributeType.GetConstructor(Array.Empty<Type>()));
            var customAttribute = new CustomAttribute(constructor);
            type.CustomAttributes.Add(customAttribute);
        }

        /// <summary>
        /// 型のプロパティを取得します (基底クラスのプロパティを含む)。
        /// </summary>
        /// <param name="type">型。</param>
        /// <returns>型のプロパティ。</returns>
        public static IReadOnlyList<PropertyDefinition> GetProperties(this TypeDefinition type)
        {
            var properties = new List<PropertyDefinition>();

            while (true)
            {
                properties.AddRange(type.Properties);

                if (type.BaseType == null)
                {
                    break;
                }
                type = type.BaseType.Resolve();
            }

            return properties;
        }

        #endregion
    }
}
