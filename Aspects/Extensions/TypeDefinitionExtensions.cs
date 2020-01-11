using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="TypeDefinition"/> の拡張メソッド。
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="type">型定義。</param>
        /// <param name="assembly">アセンブリ。</param>
        internal static void Inject(this TypeDefinition type)
        {
            foreach (var method in type.Methods.ToArray())
            {
                method.Inject();
            }

            foreach (var nestedType in type.NestedTypes)
            {
                nestedType.Inject();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyType"></param>
        /// <param name="propertyName"></param>
        /// <param name="backingStoreFieldName"></param>
        /// <returns></returns>
        internal static PropertyDefinition CreateProperty(this TypeDefinition type, TypeReference propertyTypeReference, string propertyName, string backingStoreFieldName)
        {
            var module                = type.Module;
            var backingStoreField     = new FieldDefinition(backingStoreFieldName, FieldAttributes.Private, propertyTypeReference);
            type.Fields.Add(backingStoreField);

            var property = new PropertyDefinition(propertyName, PropertyAttributes.None, backingStoreField.FieldType);
            property.HasThis = !backingStoreField.IsStatic;

            var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            if (backingStoreField.IsStatic)
            {
                attributes |= MethodAttributes.Static;
            }

            //
            {
                var getter = new MethodDefinition("get_" + property.Name, attributes, backingStoreField.FieldType);
                getter.SemanticsAttributes = MethodSemanticsAttributes.Getter;

                var processor = getter.Body.GetILProcessor();
                if (backingStoreField.IsStatic)
                {
                    processor.Emit(OpCodes.Ldsfld, backingStoreField);
                    processor.Emit(OpCodes.Ret);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, backingStoreField);
                    processor.Emit(OpCodes.Ret);
                }

                type.Methods.Add(getter);
                property.GetMethod = getter;
            }

            //
            {
                var setter = new MethodDefinition("set_" + property.Name, attributes, module.TypeSystem.Void);
                setter.SemanticsAttributes = MethodSemanticsAttributes.Setter;

                ParameterDefinition valueArg;
                setter.Parameters.Add(valueArg = new ParameterDefinition(backingStoreField.FieldType));

                var processor = setter.Body.GetILProcessor();
                if (backingStoreField.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Stsfld, backingStoreField);
                    processor.Emit(OpCodes.Ret);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldarg, valueArg);
                    processor.Emit(OpCodes.Stfld, backingStoreField);
                    processor.Emit(OpCodes.Ret);
                }

                type.Methods.Add(setter);
                property.SetMethod = setter;
            }

            type.Properties.Add(property);

            return property;
        }

        #endregion
    }
}
