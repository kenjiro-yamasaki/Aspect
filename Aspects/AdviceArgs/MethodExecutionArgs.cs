using System;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド実行引数。
    /// </summary>
    public class MethodExecutionArgs : AdviceArgs
    {
        #region プロパティ

        /// <summary>
        /// メソッド情報。
        /// </summary>
        public MethodBase Method { get; set; }

        /// <summary>
        /// 引数コレクション。
        /// </summary>
        public Arguments Arguments { get; }

        /// <summary>
        /// 戻り値。
        /// </summary>
        public object ReturnValue { get; set; }

        /// <summary>
        /// 例外。
        /// </summary>
        public Exception Exception { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、null)。</param>
        /// <param name="arguments">引数コレクション。</param>
        public MethodExecutionArgs(object instance, Arguments arguments)
            : base(instance)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        #endregion
    }
}
