using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドレベルアスペクト。
    /// </summary>
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
        public void Inject(MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            OnInject(method);
        }

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        protected abstract void OnInject(MethodDefinition method);

        #endregion
    }
}
