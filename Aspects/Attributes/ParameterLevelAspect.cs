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
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="parameterDefinition">注入対象のパラメーター定義。</param>
        public void Inject(ParameterDefinition parameterDefinition)
        {
            if (parameterDefinition == null)
            {
                throw new ArgumentNullException(nameof(parameterDefinition));
            }

            OnInject(parameterDefinition);
        }

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="parameterDefinition">注入対象のパラメーター定義。</param>
        protected abstract void OnInject(ParameterDefinition parameterDefinition);

        #endregion
    }
}
