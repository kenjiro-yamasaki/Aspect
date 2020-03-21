using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="AssemblyDefinition"/> の拡張メソッド。
    /// </summary>
    public static class AssemblyDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="assembly">アセンブリ。</param>
        public static void InjectAdvice(this AssemblyDefinition assembly)
        {
            using var profile = Profiling.Profiler.Start($"{nameof(AssemblyDefinitionExtensions)}.{nameof(InjectAdvice)}");

            // アセンブリのマルチキャスト属性を生成します。
            var multicastAttributes = new List<MulticastAttribute>();
            foreach (var customAttribute in assembly.CustomAttributes)
            {
                if (customAttribute.IsMulticastAttribute())
                {
                    var multicastAttribute = customAttribute.Create<MulticastAttribute>();
                    multicastAttribute.CustomAttribute = customAttribute;
                    multicastAttributes.Add(multicastAttribute);
                }
            }

            // モジュールにアドバイスを注入します。
            foreach (var module in assembly.Modules)
            {
                module.InjectAdvice(multicastAttributes.OrderBy(ma => ma.AttributePriority));
            }
        }

        #endregion
    }
}
