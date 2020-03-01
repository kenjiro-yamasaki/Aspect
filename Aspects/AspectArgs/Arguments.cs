﻿using System;
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
        /// 引数の数。
        /// </summary>
        public int Count { get; }

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
        public Arguments()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="count">引数の数。</param>
        internal Arguments(int count)
        {
            Count = count;
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
        public virtual object GetArgument(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public virtual void SetArgument(int index, object value)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    public class Arguments<TArg0> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        public Arguments(TArg0 arg0)
            : base(1)
        {
            Arg0 = arg0;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1)
            : base(2)
        {
            Arg0 = arg0;
            Arg1 = arg1;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2)
            : base(3)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    /// <typeparam name="TArg3">第 3 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2, TArg3> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        /// <summary>
        /// 第 3 引数。
        /// </summary>
        public TArg3 Arg3;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        /// <param name="arg3">第 3 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            : base(4)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                case 3:
                    return Arg3;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                case 3:
                    Arg3 = (TArg3)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    /// <typeparam name="TArg3">第 3 引数の型。</typeparam>
    /// <typeparam name="TArg4">第 4 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2, TArg3, TArg4> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        /// <summary>
        /// 第 3 引数。
        /// </summary>
        public TArg3 Arg3;

        /// <summary>
        /// 第 4 引数。
        /// </summary>
        public TArg4 Arg4;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        /// <param name="arg3">第 3 引数。</param>
        /// <param name="arg4">第 4 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
            : base(5)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                case 3:
                    return Arg3;

                case 4:
                    return Arg4;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                case 3:
                    Arg3 = (TArg3)value;
                    break;

                case 4:
                    Arg4 = (TArg4)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    /// <typeparam name="TArg3">第 3 引数の型。</typeparam>
    /// <typeparam name="TArg4">第 4 引数の型。</typeparam>
    /// <typeparam name="TArg5">第 5 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        /// <summary>
        /// 第 3 引数。
        /// </summary>
        public TArg3 Arg3;

        /// <summary>
        /// 第 4 引数。
        /// </summary>
        public TArg4 Arg4;

        /// <summary>
        /// 第 5 引数。
        /// </summary>
        public TArg5 Arg5;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        /// <param name="arg3">第 3 引数。</param>
        /// <param name="arg4">第 4 引数。</param>
        /// <param name="arg5">第 5 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
            : base(6)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                case 3:
                    return Arg3;

                case 4:
                    return Arg4;

                case 5:
                    return Arg5;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                case 3:
                    Arg3 = (TArg3)value;
                    break;

                case 4:
                    Arg4 = (TArg4)value;
                    break;

                case 5:
                    Arg5 = (TArg5)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    /// <typeparam name="TArg3">第 3 引数の型。</typeparam>
    /// <typeparam name="TArg4">第 4 引数の型。</typeparam>
    /// <typeparam name="TArg5">第 5 引数の型。</typeparam>
    /// <typeparam name="TArg6">第 6 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        /// <summary>
        /// 第 3 引数。
        /// </summary>
        public TArg3 Arg3;

        /// <summary>
        /// 第 4 引数。
        /// </summary>
        public TArg4 Arg4;

        /// <summary>
        /// 第 5 引数。
        /// </summary>
        public TArg5 Arg5;

        /// <summary>
        /// 第 6 引数。
        /// </summary>
        public TArg6 Arg6;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        /// <param name="arg3">第 3 引数。</param>
        /// <param name="arg4">第 4 引数。</param>
        /// <param name="arg5">第 5 引数。</param>
        /// <param name="arg6">第 6 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
            : base(7)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                case 3:
                    return Arg3;

                case 4:
                    return Arg4;

                case 5:
                    return Arg5;

                case 6:
                    return Arg6;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                case 3:
                    Arg3 = (TArg3)value;
                    break;

                case 4:
                    Arg4 = (TArg4)value;
                    break;

                case 5:
                    Arg5 = (TArg5)value;
                    break;

                case 6:
                    Arg6 = (TArg6)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    /// <typeparam name="TArg0">第 0 引数の型。</typeparam>
    /// <typeparam name="TArg1">第 1 引数の型。</typeparam>
    /// <typeparam name="TArg2">第 2 引数の型。</typeparam>
    /// <typeparam name="TArg3">第 3 引数の型。</typeparam>
    /// <typeparam name="TArg4">第 4 引数の型。</typeparam>
    /// <typeparam name="TArg5">第 5 引数の型。</typeparam>
    /// <typeparam name="TArg6">第 6 引数の型。</typeparam>
    /// <typeparam name="TArg7">第 7 引数の型。</typeparam>
    public class Arguments<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 第 0 引数。
        /// </summary>
        public TArg0 Arg0;

        /// <summary>
        /// 第 1 引数。
        /// </summary>
        public TArg1 Arg1;

        /// <summary>
        /// 第 2 引数。
        /// </summary>
        public TArg2 Arg2;

        /// <summary>
        /// 第 3 引数。
        /// </summary>
        public TArg3 Arg3;

        /// <summary>
        /// 第 4 引数。
        /// </summary>
        public TArg4 Arg4;

        /// <summary>
        /// 第 5 引数。
        /// </summary>
        public TArg5 Arg5;

        /// <summary>
        /// 第 6 引数。
        /// </summary>
        public TArg6 Arg6;

        /// <summary>
        /// 第 7 引数。
        /// </summary>
        public TArg7 Arg7;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arg0">第 0 引数。</param>
        /// <param name="arg1">第 1 引数。</param>
        /// <param name="arg2">第 2 引数。</param>
        /// <param name="arg3">第 3 引数。</param>
        /// <param name="arg4">第 4 引数。</param>
        /// <param name="arg5">第 5 引数。</param>
        /// <param name="arg6">第 6 引数。</param>
        /// <param name="arg7">第 7 引数。</param>
        public Arguments(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
            : base(8)
        {
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            switch (index)
            {
                case 0:
                    return Arg0;

                case 1:
                    return Arg1;

                case 2:
                    return Arg2;

                case 3:
                    return Arg3;

                case 4:
                    return Arg4;

                case 5:
                    return Arg5;

                case 6:
                    return Arg6;

                case 7:
                    return Arg7;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Arg0 = (TArg0)value;
                    break;

                case 1:
                    Arg1 = (TArg1)value;
                    break;

                case 2:
                    Arg2 = (TArg2)value;
                    break;

                case 3:
                    Arg3 = (TArg3)value;
                    break;

                case 4:
                    Arg4 = (TArg4)value;
                    break;

                case 5:
                    Arg5 = (TArg5)value;
                    break;

                case 6:
                    Arg6 = (TArg6)value;
                    break;

                case 7:
                    Arg7 = (TArg7)value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        #endregion
    }

    /// <summary>
    /// 引数配列。
    /// </summary>
    public sealed class ArgumentsArray : Arguments
    {
        #region プロパティ

        /// <summary>
        /// 引数配列。
        /// </summary>
        public object[] Arguments { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="arguments">引数配列。</param>
        public ArgumentsArray(object[] arguments)
            : base(arguments.Length)
        {
            Arguments = arguments;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 引数を取得します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <returns>引数。</returns>
        public override object GetArgument(int index)
        {
            return Arguments[index];
        }

        /// <summary>
        /// 引数を設定します。
        /// </summary>
        /// <param name="index">引数のインデックス。</param>
        /// <param name="value">引数の値。</param>
        public override void SetArgument(int index, object value)
        {
            Arguments[index] = value;
        }

        #endregion
    }
}