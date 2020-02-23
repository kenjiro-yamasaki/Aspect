using System;
using System.Threading.Tasks;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプト引数。
    /// </summary>
    public abstract class MethodInterceptionArgs : MethodArgs
    {
        #region プロパティ

        /// <summary>
        /// 非同期タスク。
        /// </summary>
        public Task Task { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、<c>null</c>)。</param>
        /// <param name="arguments">引数コレクション。</param>
        public MethodInterceptionArgs(object instance, Arguments arguments)
            : base(instance, arguments)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを呼び出します。
        /// </summary>
        /// <param name="arguments">引数コレクション。</param>
        /// <returns>戻り値。</returns>
        public virtual object Invoke(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 現在の引数を渡し、その戻り値を <see cref="ReturnValue"/> に格納することにより、インターセプトされたメソッドの呼び出します。
        /// </summary>
        public virtual void Proceed()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task InvokeAsync(Arguments arguments)
        {
            return InvokeAsyncImpl(arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ProceedAsync()
        {
            Task = InvokeAsyncImpl(Arguments);
            await Task;
            ReturnValue = GetTaskResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected virtual Task InvokeAsyncImpl(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual object GetTaskResult()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
