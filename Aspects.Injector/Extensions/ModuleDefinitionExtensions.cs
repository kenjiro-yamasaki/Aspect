using Mono.Cecil;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// ModuleDefinitionの拡張メソッド。
    /// </summary>
    internal static class ModuleDefinitionExtensions
    {
        #region 静的メソッド

        /// <summary>
        /// アスペクト(カスタムコード)を注入します。
        /// </summary>
        /// <param name="module">注入対象のモジュール</param>
        internal static void Inject(this ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                type.Inject();
            }
        }

        #endregion
    }
}
