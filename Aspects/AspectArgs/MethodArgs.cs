﻿using System;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド引数。
    /// </summary>
    public class MethodArgs : AspectArgs
    {
        #region プロパティ

        /// <summary>
        /// メソッド情報。
        /// </summary>
        public MethodBase Method { get; set; }

        /// <summary>
        /// 引数配列。
        /// </summary>
        public Arguments Arguments { get; set; }

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
        /// <param name="instance">メソッドが実行されたインスタンス (静的メッソドが実行された場合、<c>null</c>)。</param>
        /// <param name="arguments">引数配列。</param>
        public MethodArgs(object instance, Arguments arguments)
            : base(instance)
        {
            Arguments = arguments;
        }

        #endregion
    }
}
