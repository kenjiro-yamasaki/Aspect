using Mono.Cecil;
using System;
using System.Linq;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// CustomAttributeの拡張メッソド。
    /// </summary>
    internal static class CustomAttributeExtensions
    {
        #region 静的メソッド

        /// <summary>
        /// CustomAttributeが表現するアスペクトを生成します。
        /// </summary>
        /// <typeparam name="TAttribute">CustomAttributeが表現するアスペクトの型。</typeparam>
        /// <param name="customAttribute">CustomAttribute</param>
        /// <returns>CustomAttributeが表現するアスペクト</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute customAttribute)
            where TAspect : class
        {
            var type           = customAttribute.AttributeType.Resolve();
            var aspectTypeName = type.FullName + ", " + type.Module.Assembly.Name.Name;
            var aspectType     = Type.GetType(aspectTypeName);

            // アスペクトのコンストラクター引数を取得します。
            object[] arguments = null;
            if (customAttribute.HasConstructorArguments)
            {
                arguments = customAttribute.ConstructorArguments.Select(a => a.Value).ToArray();
            }

            // アスペクトのインスタンスを生成します。
            var aspect = Activator.CreateInstance(aspectType, arguments) as TAspect;

            // アスペクトのプロパティを設定します。
            if (customAttribute.HasProperties)
            {
                foreach (var attributeProperty in customAttribute.Properties)
                {
                    aspectType.GetProperty(attributeProperty.Name).SetValue(aspect, attributeProperty.Argument.Value, null);
                }
            }

            return aspect;
        }

        #endregion
    }
}
