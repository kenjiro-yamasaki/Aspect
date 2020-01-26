namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド実行引数。
    /// </summary>
    public class MethodExecutionArgs : MethodArgs
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、null)。</param>
        /// <param name="arguments">引数コレクション。</param>
        public MethodExecutionArgs(object instance, Arguments arguments)
            : base(instance, arguments)
        {
        }

        #endregion
    }
}
