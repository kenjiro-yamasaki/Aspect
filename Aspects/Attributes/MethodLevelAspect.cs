using Mono.Cecil;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドレベルアスペクト。
    /// </summary>
    public abstract class MethodLevelAspect : MulticastAttribute
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
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public abstract void InjectAdvice(MethodDefinition targetMethod, CustomAttribute aspectAttribute);

        #endregion
    }
}
