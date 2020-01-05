using Mono.Cecil;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="AssemblyDefinition"/> の拡張メソッド。
    /// </summary>
    public static class AssemblyDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="assemblyDefinition">注入対象のアセンブリ定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        public static void Inject(this AssemblyDefinition assemblyDefinition, Assembly assembly)
        {
            foreach (var module in assemblyDefinition.Modules)
            {
                module.Inject(assembly);
            }
        }

        #endregion
    }
}
