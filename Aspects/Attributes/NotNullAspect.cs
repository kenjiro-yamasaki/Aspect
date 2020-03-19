using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非 <c>null</c> パラメーターアスペクト。
    /// </summary>
    /// <remarks>
    /// この属性を付けたパラメーターが、<c>null</c> を許容しないことを明示します。
    /// <c>null</c> を渡された場合、<see cref="ArgumentNullException"/> を投げます。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
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
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="parameter">ターゲットパラメーター。</param>
        protected override void InjectAdvice(ParameterDefinition parameter)
        {
            if (!parameter.ParameterType.IsValueType)
            {
                var methodDefinition = parameter.Method as MethodDefinition;
                var module           = methodDefinition.DeclaringType.Module;
                var processor        = methodDefinition.Body.GetILProcessor();
                var first            = processor.Body.Instructions[0];

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
