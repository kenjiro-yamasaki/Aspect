using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
        /// <typeparam name="TAspect">属性が表現するアスペクトの型。</typeparam>
        /// <param name="attribute">属性。</param>
        /// <returns>属性が表現するアスペクト。</returns>
        internal static TAspect Create<TAspect>(this CustomAttribute attribute)
            where TAspect : class
        {
            /// 属性のコンストラクター引数を取得します。
            var arguments = new List<object>();
            if (attribute.HasConstructorArguments)
            {
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
            }

            /// 属性のインスタンスを生成します。
            var attributeType = attribute.AttributeType.ToSystemType();
            var instance = Activator.CreateInstance(attributeType, arguments.ToArray()) as TAspect;

            /// 属性のプロパティを設定します。
            if (attribute.HasProperties)
            {
                foreach (var attributeProperty in attribute.Properties)
                {
                    attributeType.GetProperty(attributeProperty.Name).SetValue(instance, attributeProperty.Argument.Value, null);
                }
            }

            return instance;
        }

        /// <summary>
        /// カスタム属性をメソッドのローカル変数としてロードします。
        /// </summary>
        /// <param name="attribute">カスタム属性。</param>
        /// <param name="method">メソッド。</param>
        /// <returns>カスタム属性の変数インデックス。</returns>
        internal static int LoadTo(this CustomAttribute attribute, MethodDefinition method)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();

            /// ローカル変数を追加します。
            var variables   = method.Body.Variables;
            var aspectIndex = variables.Count();                                                    /// アスペクトの変数インデックス。
            var aspectType  = attribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(aspectType)));

            ///
            var argumentTypes  = attribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = attribute.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case int @int:
                        processor.Emit(OpCodes.Ldc_I4, @int);
                        break;

                    case string @string:
                        processor.Emit(OpCodes.Ldstr, @string);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            processor.Emit(OpCodes.Newobj, module.ImportReference(aspectType.GetConstructor(argumentTypes.ToArray())));
            processor.Emit(OpCodes.Stloc, aspectIndex);








            return aspectIndex;
        }

        #endregion
    }
}
