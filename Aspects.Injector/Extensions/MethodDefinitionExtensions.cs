using Mono.Cecil;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// MethodDefinition の拡張メソッド。
    /// </summary>
    internal static class MethodDefinitionExtensions
    {
        #region 静的メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド。</param>
        internal static void Inject(this MethodDefinition method)
        {
            var baseFullName  = $"{nameof(SoftCube)}.{nameof(Aspects)}.{nameof(MethodLevelAspect)}";
            var baseScopeName = $"{nameof(SoftCube)}.{nameof(Aspects)}.dll";

            foreach (var attribute in method.CustomAttributes)
            {
                var baseAttributeType = attribute.AttributeType.Resolve().BaseType.Resolve();

                while (baseAttributeType != null && baseAttributeType.BaseType != null)
                {
                    if (baseAttributeType.FullName == baseFullName && baseAttributeType.Scope.Name == baseScopeName)
                    {
                        var aspect = attribute.Create<MethodLevelAspect>();
                        aspect.Inject(method);
                        break;
                    }

                    baseAttributeType = baseAttributeType.BaseType.Resolve();
                }
            }
        }

        #endregion
    }
}
