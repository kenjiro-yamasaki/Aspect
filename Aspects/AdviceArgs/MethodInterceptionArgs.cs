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

        /// <summary>
        /// 非同期タスクの結果。
        /// </summary>
        /// <remarks>
        /// <see cref="InvokeAsync"/>、<see cref="ProceedAsync"/> の戻り値は <see cref="System.Threading.Tasks.Task"/> 型ですが、
        /// ターゲットメソッドの戻り値が <see cref="Task{TResult}"/> の場合、<see cref="Task{TResult}"/> 型のインスタンスを戻します。
        /// <see cref="Task{TResult}.Result"/> の値を取得したい場合、このプロパティを使用します。
        /// </remarks>
        public virtual object TaskResult => throw new NotImplementedException();

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、<c>null</c>)。</param>
        /// <param name="arguments">引数配列。</param>
        public MethodInterceptionArgs(object instance, Arguments arguments)
            : base(instance, arguments)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを呼びだします。
        /// </summary>
        /// <param name="arguments">引数配列。</param>
        /// <returns>戻り値。</returns>
        public object Invoke(Arguments arguments)
        {
            return InvokeImpl(arguments);
        }

        /// <summary>
        /// 現在の引数を渡し、その戻り値を <see cref="ReturnValue"/> に格納することにより、インターセプトされたメソッドの呼びだします。
        /// </summary>
        public void Proceed()
        {
            ReturnValue = InvokeImpl(Arguments);
        }

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを非同期で呼びだします。
        /// </summary>
        /// <returns>タスク。</returns>
        public async Task InvokeAsync(Arguments arguments)
        {
            Task = InvokeAsyncImpl(arguments);
            await Task;
        }

        /// <summary>
        /// 現在の引数を渡し、その戻り値を <see cref="ReturnValue"/> に格納することにより、インターセプトされたメソッドを非同期で呼びだします。
        /// </summary>
        /// <returns>タスク。</returns>
        public async Task ProceedAsync()
        {
            Task = InvokeAsyncImpl(Arguments);
            await Task;
            ReturnValue = TaskResult;
        }

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを呼びだします。
        /// </summary>
        /// <param name="arguments">引数配列。</param>
        /// <returns>戻り値。</returns>
        protected virtual object InvokeImpl(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="Arguments"/> に影響を与えることなく、指定された引数を使用して、インターセプトされたメソッドを非同期で呼びだします。
        /// </summary>
        /// <param name="arguments">引数配列。</param>
        /// <returns>タスク。</returns>
        protected virtual Task InvokeAsyncImpl(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
