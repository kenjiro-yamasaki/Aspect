using Mono.Cecil;
using Mono.Cecil.Pdb;
using System.Linq;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// AssemblyDefinitionの拡張メソッド。
    /// </summary>
    internal static class AssemblyDefinitionExtensions
    {
        #region 静的メソッド

        /// <summary>
        /// アスペクト(カスタムコード)を注入します。
        /// </summary>
        /// <param name="assembly">注入対象のアセンブリ</param>
        internal static void Inject(this AssemblyDefinition assembly)
        {
            foreach (var module in assembly.Modules)
            {
                module.Inject();
            }
        }

        #endregion
    }
}
