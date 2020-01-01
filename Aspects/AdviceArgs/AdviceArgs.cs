namespace SoftCube.Aspects
{
    /// <summary>
    /// アドバイス引数。
    /// </summary>
    public class AdviceArgs
    {
        #region プロパティ

        /// <summary>
        /// メソッドが実行されたインスタンス。
        /// </summary>
        public object Instance { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス(静的メッソドが実行された場合、null)。</param>
        public AdviceArgs(object instance)
        {
            Instance = instance;
        }

        #endregion
    }
}
