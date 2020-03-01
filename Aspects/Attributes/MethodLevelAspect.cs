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
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト属性。</param>
        public void Inject(MethodDefinition method, CustomAttribute aspect)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            InjectAdvice(method, aspect);
        }

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト属性。</param>
        protected abstract void InjectAdvice(MethodDefinition method, CustomAttribute aspect);

        #endregion
    }
}
