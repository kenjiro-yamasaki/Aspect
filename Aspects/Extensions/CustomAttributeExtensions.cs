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
        /// アスペクトを生成します。
        /// </summary>
        /// <typeparam name="TAspect">アスペクトの型。</typeparam>
        /// <param name="attribute">カスタム属性。</param>
        /// <returns>アスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute attribute)
            where TAspect : class
        {
            /// カスタム属性のコンストラクター引数を取得します。
            var arguments = new List<object>();
            foreach (var argument in attribute.ConstructorArguments)
            {
                var argumentType = argument.Type.ToSystemType();

                if (argumentType.IsEnum)
                {
                    arguments.Add(Enum.ToObject(argumentType, argument.Value));
                }
                else if (argumentType.FullName == "System.Type")
                {
                    var value = argument.Value as TypeReference;
                    arguments.Add(Type.GetType(value.FullName));
                }
                else
                {
                    arguments.Add(argument.Value);
                }
            }

            /// 属性のインスタンスを生成します。
            var aspectType = attribute.AttributeType.ToSystemType();
            var instance = Activator.CreateInstance(aspectType, arguments.ToArray()) as TAspect;

            /// 属性のプロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                if (property.Argument.Type.FullName == "System.Type")
                {
                    var value = property.Argument.Value as TypeReference;
                    aspectType.GetProperty(property.Name).SetValue(instance, Type.GetType(value.FullName), null);
                }
                else 
                {
                    aspectType.GetProperty(property.Name).SetValue(instance, property.Argument.Value, null);
                }
            }

            return instance;
        }

        #endregion
    }
}
