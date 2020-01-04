﻿using SoftCube.Log;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// ロガーアスペクト。
    /// </summary>
    public class LoggerAspect : OnMethodBoundaryAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public LoggerAspect()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メッソドの開始イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Trace("OnEntry");
        }

        /// <summary>
        /// メッソドの正常終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Trace("OnSuccess");

            if (args.ReturnValue == null)
            {
                Logger.Trace("null");
            }
            else
            {
                Logger.Trace(args.ReturnValue.ToString());
            }
        }

        /// <summary>
        /// メッソドの例外終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public override void OnException(MethodExecutionArgs args)
        {
            Logger.Trace("OnException");
        }

        /// <summary>
        /// メッソドの終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Trace("OnExit");
        }

        #endregion
    }
}