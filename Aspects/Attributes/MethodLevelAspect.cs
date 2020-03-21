using Mono.Cecil;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドレベルアスペクト。
    /// </summary>
    public abstract class MethodLevelAspect : MulticastAttribute
    {
        #region プロパティ

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; set; }

        #endregion

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
        public abstract void InjectAdvice();

        #endregion
    }
}
