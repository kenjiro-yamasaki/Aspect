using Mono.Cecil;
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
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="type">型。</param>
        /// <param name="multicastAttributes">マルチキャスト属性コレクション。</param>
        internal static void InjectAdvice(this TypeDefinition type, IEnumerable<MulticastAttribute> multicastAttributes)
        {
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
            multicastAttributes = currentMulticastAttributes.Concat(multicastAttributes.OrderByDescending(ma => ma.AttributePriority));

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

        #endregion
    }
}
