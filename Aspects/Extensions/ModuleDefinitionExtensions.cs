using Mono.Cecil;
using System.Reflection;

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
        /// <param name="moduleDefinition">注入対象のモジュール定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this ModuleDefinition moduleDefinition, Assembly assembly)
        {
            foreach (var type in moduleDefinition.Types)
            {
                type.Inject(assembly);
            }
        }

        #endregion
    }
}
