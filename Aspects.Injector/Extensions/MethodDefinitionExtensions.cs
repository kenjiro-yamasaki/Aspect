using Mono.Cecil;
using System.Reflection;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// <see cref="MethodDefinition"/> の拡張メソッド。
    /// </summary>
    internal static class MethodDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="methodDefinition">注入対象のメソッド定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this MethodDefinition methodDefinition, Assembly assembly)
        {
            var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MethodLevelAspect)}";
            var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";

            foreach (var attribute in methodDefinition.CustomAttributes)
            {
                var baseAttributeType = attribute.AttributeType.Resolve().BaseType.Resolve();

                while (baseAttributeType != null && baseAttributeType.BaseType != null)
                {
                    if (baseAttributeType.FullName == baseFullName && baseAttributeType.Scope.Name == baseScopeName)
                    {
                        var aspect = attribute.Create<MethodLevelAspect>(assembly);
                        aspect.Inject(methodDefinition);
                        break;
                    }

                    baseAttributeType = baseAttributeType.BaseType.Resolve();
                }
            }
        }

        #endregion
    }
}
