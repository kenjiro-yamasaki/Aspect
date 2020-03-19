using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// パラメーターレベルアスペクト。
    /// </summary>
    public abstract class ParameterLevelAspect : Attribute
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public ParameterLevelAspect()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="parameter">ターゲットパラメーター。</param>
        protected abstract void InjectAdvice(ParameterDefinition parameter);

        #endregion
    }
}
