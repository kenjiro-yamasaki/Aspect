using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="ILProcessor"/> の拡張メソッド。
    /// </summary>
    public static class ILProcessorExtensions
    {
        #region メソッド

        /// <summary>
        /// 命令コレクションの末尾に属性を追加する IL コードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="attribute">属性。</param>
        /// <returns>属性の変数インデックス。</returns>
        internal static int Emit(this ILProcessor processor, CustomAttribute attribute)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// ローカル変数を追加します。
            var variables      = method.Body.Variables;
            var attributeIndex = variables.Count();
            var attributeType  = attribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(attributeType)));

            /// 属性を生成して、ローカル変数にストアします。
            var argumentTypes = attribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
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

            processor.Emit(OpCodes.Newobj, module.ImportReference(attributeType.GetConstructor(argumentTypes.ToArray())));
            processor.Emit(OpCodes.Stloc, attributeIndex);

            /// プロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.Argument.Value;

                processor.Emit(OpCodes.Ldloc, attributeIndex);

                switch (propertyValue)
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

                processor.Emit(OpCodes.Callvirt, module.ImportReference(attributeType.GetProperty(propertyName).GetSetMethod()));
            }

            return attributeIndex;
        }

        /// <summary>
        /// 命令の前に属性を追加する IL コードを追加します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="instruction">命令。</param>
        /// <param name="attribute">属性。</param>
        /// <returns>属性の変数インデックス。</returns>
        internal static int InsertBefore(this ILProcessor processor, Instruction instruction, CustomAttribute attribute)
        {
            var method       = processor.Body.Method;
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var instructions = method.Body.Instructions;

            /// ローカル変数を追加します。
            var variables      = method.Body.Variables;
            var attributeIndex = variables.Count();
            var attributeType  = attribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(module.ImportReference(attributeType)));

            /// 属性を生成して、ローカル変数にストアします。
            var argumentTypes  = attribute.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = attribute.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case int @int:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldc_I4, @int));
                        break;

                    case string @string:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldstr, @string));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            processor.InsertBefore(instruction, processor.Create(OpCodes.Newobj, module.ImportReference(attributeType.GetConstructor(argumentTypes.ToArray()))));
            processor.InsertBefore(instruction, processor.Create(OpCodes.Stloc, attributeIndex));

            /// プロパティを設定します。
            foreach (var property in attribute.Properties)
            {
                var propertyName  = property.Name;
                var propertyValue = property.Argument.Value;

                processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, attributeIndex));

                switch (propertyValue)
                {
                    case int @int:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldc_I4, @int));
                        break;

                    case string @string:
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldstr, @string));
                        break;

                    default:
                        throw new NotSupportedException();
                }

                processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, module.ImportReference(attributeType.GetProperty(propertyName).GetSetMethod())));
            }

            return attributeIndex;
        }

        #endregion
    }
}
