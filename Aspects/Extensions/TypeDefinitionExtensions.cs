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
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="type">型。</param>
        internal static void InjectAdvice(this TypeDefinition type)
        {
            foreach (var method in type.Methods.ToArray())
            {
                method.InjectAdvice();
            }

            foreach (var nestedType in type.NestedTypes)
            {
                nestedType.InjectAdvice();
            }
        }

        #endregion
    }
}
