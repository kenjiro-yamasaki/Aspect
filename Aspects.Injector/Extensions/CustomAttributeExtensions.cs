using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// <see cref="CustomAttribute"/> の拡張メッソド。
    /// </summary>
    internal static class CustomAttributeExtensions
    {
        #region メソッド

        /// <summary>
        /// <see cref="CustomAttribute"/> が表現するアスペクトを生成します。
        /// </summary>
        /// <typeparam name="TAspect"><see cref="CustomAttribute"/> が表現するアスペクトの型。</typeparam>
        /// <param name="customAttribute">CustomAttribute。</param>
        /// <param name="assembly">アセンブリ。</param>
        /// <returns><see cref="CustomAttribute"/> が表現するアスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute customAttribute, Assembly assembly)
            where TAspect : class
        {
            //var type           = customAttribute.AttributeType.Resolve();
            //var aspectTypeName = type.FullName + ", " + type.Module.Assembly.Name.Name;
            //var aspectType     = Type.GetType(aspectTypeName);

            var type = customAttribute.AttributeType.Resolve();
            var aspectType = assembly.GetType(type.FullName);

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
