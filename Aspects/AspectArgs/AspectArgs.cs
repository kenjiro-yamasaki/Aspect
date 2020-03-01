namespace SoftCube.Aspects
{
    /// <summary>
    /// アスペクト引数。
    /// </summary>
    public class AspectArgs
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
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、<c>null</c>)。</param>
        public AspectArgs(object instance)
        {
            Instance = instance;
        }

        #endregion
    }
}
