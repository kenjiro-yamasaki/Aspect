using Mono.Cecil;
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
        internal static void InjectAdvice(this ModuleDefinition module)
        {
            foreach (var type in module.Types.ToList())
            {
                type.InjectAdvice();
            }
        }

        #endregion
    }
}
