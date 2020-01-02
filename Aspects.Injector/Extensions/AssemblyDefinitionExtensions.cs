using Mono.Cecil;
using System.Reflection;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// <see cref="AssemblyDefinition"/> の拡張メソッド。
    /// </summary>
    internal static class AssemblyDefinitionExtensions
    {
        #region 静的メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="assemblyDefinition">注入対象の <see cref="AssemblyDefinition"/>。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this AssemblyDefinition assemblyDefinition, Assembly assembly)
        {
            foreach (var module in assemblyDefinition.Modules)
            {
                module.Inject(assembly);
            }
        }

        #endregion
    }
}
