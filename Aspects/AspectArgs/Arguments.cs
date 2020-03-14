using System.Collections;
using System.Collections.Generic;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 引数配列。
    /// </summary>
    public class Arguments : IEnumerable, IEnumerable<object>, IReadOnlyList<object>
    {
        #region プロパティ

        /// <summary>
        /// 引数配列。
        /// </summary>
        public readonly object[] arguments;

        /// <summary>
        /// 引数の数。
        /// </summary>
        public int Count => arguments.Length;

        /// <summary>
        /// インデクサー。
        /// </summary>
        /// <param name="index">インデックス。</param>
        /// <returns>要素。</returns>
        public object this[int index]
        {
            get => GetArgument(index);
            set => SetArgument(index, value);
        }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arguments">引数配列。</param>
        public Arguments(object[] arguments)
        {
            this.arguments = arguments;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 反復子を取得します。
        /// </summary>
        /// <returns>反復子。</returns>
        public IEnumerator<object> GetEnumerator()
        {
            for (int index = 0; index < Count; index++)
            {
                yield return GetArgument(index);
            }
        }

        /// <summary>
        /// 反復子を取得します。
        /// </summary>
        /// <returns>反復子。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public object GetArgument(int index)
        {
            return arguments[index];
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public void SetArgument(int index, object value)
        {
            arguments[index] = value;
        }

        #endregion
    }
}
