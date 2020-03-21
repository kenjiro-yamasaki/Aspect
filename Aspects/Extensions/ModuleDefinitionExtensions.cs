using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ModuleDefinition"/> の拡張メソッド。
    /// </summary>
    public static class ModuleDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="module">モジュール。</param>
        /// <param name="multicastAttributes">マルチキャスト属性コレクション。</param>
        internal static void InjectAdvice(this ModuleDefinition module, IEnumerable<MulticastAttribute> multicastAttributes)
        {
            // モジュールのマルチキャスト属性を生成します。
            var currentMulticastAttributes = new List<MulticastAttribute>();
            foreach (var customAttribute in module.CustomAttributes.ToList())
            {
                if (customAttribute.IsMulticastAttribute())
                {
                    var multicastAttribute = customAttribute.Create<MulticastAttribute>();
                    multicastAttribute.CustomAttribute = customAttribute;
                    currentMulticastAttributes.Add(multicastAttribute);

                    module.CustomAttributes.Remove(customAttribute);
                }
            }
            multicastAttributes = currentMulticastAttributes.Concat(multicastAttributes.OrderByDescending(ma => ma.AttributePriority));

            // 型にアドバイスを注入します。
            foreach (var type in module.Types.ToList())
            {
                type.InjectAdvice(multicastAttributes);
            }
        }

        #endregion
    }
}
