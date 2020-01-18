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
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="module">モジュール定義。</param>
        internal static void Inject(this ModuleDefinition module)
        {
            foreach (var type in module.Types.ToList())
            {
                type.Inject();
            }
        }

        #endregion
    }
}
