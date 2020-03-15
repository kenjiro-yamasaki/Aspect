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
        public static void InjectAdvice(this AssemblyDefinition assembly)
        {
            using var profile = Profiling.Profiler.Start($"{nameof(AssemblyDefinitionExtensions)}.{nameof(InjectAdvice)}");

            foreach (var module in assembly.Modules)
            {
                module.Inject();
            }
        }

        #endregion
    }
}
