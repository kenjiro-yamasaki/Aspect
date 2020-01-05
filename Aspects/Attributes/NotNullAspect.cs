using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非nullパラメーターアスペクト。
    /// </summary>
    /// <remarks>
    /// この属性を付けたパラメーターが、nullを許容しないことを明示します。
    /// nullを渡された場合、System.ArgumentNullExceptionを投げる。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    [Serializable]
    public class NotNullAspect : ParameterLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public NotNullAspect()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アスペクト(カスタムコード)を注入します。
        /// </summary>
        /// <param name="parameter">注入対象のパラメーター定義</param>
        protected override void OnInject(ParameterDefinition parameter)
        {
            if (!parameter.ParameterType.IsValueType)
            {
                var method    = (parameter.Method as MethodDefinition);
                var module    = method.DeclaringType.Module.Assembly.MainModule;
                var processor = method.Body.GetILProcessor();
                var first     = processor.Body.Instructions[0];

                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg, parameter));
                processor.InsertBefore(first, processor.Create(OpCodes.Brtrue_S, first));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldstr, parameter.Name));
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(ArgumentNullException).GetConstructor(new Type[] { typeof(string) }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Throw));
            }
        }

        #endregion
    }
}
