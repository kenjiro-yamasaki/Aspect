using Mono.Cecil;
using System.Reflection;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// <see cref="ModuleDefinition"/> の拡張メソッド。
    /// </summary>
    internal static class ModuleDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="moduleDefinition">注入対象の <see cref="ModuleDefinition"/>。</param>
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
