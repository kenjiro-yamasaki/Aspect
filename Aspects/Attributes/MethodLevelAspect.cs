using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドレベルアスペクト。
    /// </summary>
    [Serializable]
    public abstract class MethodLevelAspect : Attribute
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public MethodLevelAspect()
            : base()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        public void Inject(MethodDefinition method, CustomAttribute attribute)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            OnInject(method, attribute);
        }

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        protected abstract void OnInject(MethodDefinition method, CustomAttribute attribute);

        #endregion
    }
}
