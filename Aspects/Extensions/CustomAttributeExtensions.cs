using Mono.Cecil;
using System;
using System.Collections.Generic;
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
        /// 属性を生成します。
        /// </summary>
        /// <typeparam name="TAspect">属性が表現する型。</typeparam>
        /// <param name="attribute">属性。</param>
        /// <returns>属性が表現するアスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute attribute)
            where TAspect : class
        {
            /// 属性のコンストラクター引数を取得します。
            var arguments = new List<object>();
            foreach (var argument in attribute.ConstructorArguments)
            {
                var argumentType = argument.Type.ToSystemType();

                if (argumentType.IsEnum)
                {
                    arguments.Add(Enum.ToObject(argumentType, argument.Value));
                }
                else
                {
                    arguments.Add(argument.Value);
                }
            }

            /// 属性のインスタンスを生成します。
            var attributeType = attribute.AttributeType.ToSystemType();
            var instance = Activator.CreateInstance(attributeType, arguments.ToArray()) as TAspect;

            /// 属性のプロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                attributeType.GetProperty(property.Name).SetValue(instance, property.Argument.Value, null);
            }

            return instance;
        }

        #endregion
    }
}
