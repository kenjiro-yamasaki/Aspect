using System;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプト引数。
    /// </summary>
    public class MethodInterceptionArgs : AdviceArgs
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
        public MethodInterceptionArgs(object instance, Arguments arguments)
            : base(instance)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを呼び出します。
        /// </summary>
        /// <param name="arguments">引数コレクション。</param>
        /// <returns>戻り値。</returns>
        public object Invoke(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 現在の引数を渡し、その戻り値を <see cref="ReturnValue"/> に格納することにより、インターセプトされたメソッドの呼び出します
        /// </summary>
        public void Proceed()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
