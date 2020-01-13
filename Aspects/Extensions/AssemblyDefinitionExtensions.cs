using Mono.Cecil;

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
        /// <param name="assembly">アセンブリ定義。</param>
        public static void Inject(this AssemblyDefinition assembly)
        {
            foreach (var module in assembly.Modules)
            {
                module.Inject();
            }
        }

        #endregion
    }
}
