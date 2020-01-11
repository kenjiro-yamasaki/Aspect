using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="CustomAttribute"/> の拡張メッソド。
    /// </summary>
    public static class CustomAttributeExtensions
    {
        #region メソッド

        /// <summary>
        /// <see cref="CustomAttribute"/> が表現するアスペクトを生成します。
        /// </summary>
        /// <typeparam name="TAspect">カスタム属性が表現するアスペクトの型。</typeparam>
        /// <param name="customAttribute">カスタム属性。</param>
        /// <returns>カスタム属性が表現するアスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute customAttribute)
            where TAspect : class
        {
            //
            var aspectTypeDefinition = customAttribute.AttributeType.Resolve();
            var assembly             = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.FullName == aspectTypeDefinition.Module.Assembly.FullName);
            var aspectType           = assembly.GetType(aspectTypeDefinition.FullName);

            /// アスペクトのコンストラクター引数を取得します。
            object[] arguments = null;
            if (customAttribute.HasConstructorArguments)
            {
                arguments = customAttribute.ConstructorArguments.Select(a => a.Value).ToArray();
            }

            /// アスペクトのインスタンスを生成します。
            var aspect = Activator.CreateInstance(aspectType, arguments) as TAspect;

            /// アスペクトのプロパティを設定します。
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
