namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド実行引数。
    /// </summary>
    public class MethodExecutionArgs : MethodArgs
    {
        #region プロパティ

        /// <summary>
        /// イテレーターメソッドによって <c>yield return</c> された値。
        /// </summary>
        public object YieldValue { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、<c>null</c>)。</param>
        /// <param name="arguments">引数配列。</param>
        public MethodExecutionArgs(object instance, Arguments arguments)
            : base(instance, arguments)
        {
        }

        #endregion
    }
}
