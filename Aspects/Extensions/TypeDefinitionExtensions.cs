using Mono.Cecil;
using System.Reflection;

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
        /// <param name="type">注入対象の <see cref="TypeDefinition"/>。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this TypeDefinition type, Assembly assembly)
        {
            foreach (var method in type.Methods)
            {
                method.Inject(assembly);
            }
        }

        #endregion
    }
}
