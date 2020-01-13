using Mono.Cecil;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="TypeDefinition"/> の拡張メソッド。
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="type">型定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this TypeDefinition type)
        {
            foreach (var method in type.Methods.ToArray())
            {
                method.Inject();
            }

            foreach (var nestedType in type.NestedTypes)
            {
                nestedType.Inject();
            }
        }

        #endregion
    }
}
