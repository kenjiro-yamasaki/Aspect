using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 引数コレクション。
    /// </summary>
    public class Arguments : IEnumerable<object>, IEnumerable
    {
        #region フィールド

        /// <summary>
        /// 引数リスト。
        /// </summary>
        private List<object> arguments;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arguments">引数配列</param>
        public Arguments(params object[] arguments)
        {
            this.arguments = arguments.ToList();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 列挙子を取得します。
        /// </summary>
        /// <returns>列挙子</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return arguments.GetEnumerator();
        }

        /// <summary>
        /// 列挙子を取得します。
        /// </summary>
        /// <returns>列挙子</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return arguments.GetEnumerator();
        }

        #endregion
    }
}
