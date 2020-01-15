using SoftCube.Asserts;
using SoftCube.Logging;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプションタイプ。
    /// </summary>
    public enum MethodInterceptionType
    {
        /// <summary>
        /// 仲介しません。
        /// </summary>
        None = 0b0000,

        /// <summary>
        /// Proceed によって仲介します。
        /// </summary>
        Proceed = 0b0001,

        /// <summary>
        /// Invoke によって仲介します。
        /// </summary>
        Invoke = 0b0010,
    }

    /// <summary>
    /// メソッドインターセプションロガー。
    /// </summary>
    public class MethodInterceptionAspectLogger : MethodInterceptionAspect
    {
        #region プロパティ

        /// <summary>
        /// メソッド仲介タイプ。
        /// </summary>
        public MethodInterceptionType Type { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="type">メソッド仲介タイプ。</param>
        public MethodInterceptionAspectLogger(MethodInterceptionType type)
        {
            Type = type;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">メソッド仲介引数。</param>
        public override void OnInvoke(MethodInterceptionArgs args)
        {

            /// メソッドを仲介します。
            switch (Type)
            {
                case MethodInterceptionType.Proceed:
                    Logger.Trace("OnInvoke");

                    /// 引数をログ出力します。
                    foreach (var argument in args.Arguments)
                    {
                        Logger.Trace(ArgumentFormatter.Format(argument));
                    }

                    Logger.Trace("Proceed");
                    args.Proceed();

                    /// 戻り値をログ出力します。
                    Logger.Trace(ArgumentFormatter.Format(args.ReturnValue));
                    break;

                case MethodInterceptionType.Invoke:
                    Logger.Trace("OnInvoke");

                    /// 引数をログ出力します。
                    foreach (var argument in args.Arguments)
                    {
                        Logger.Trace(ArgumentFormatter.Format(argument));
                    }

                    Logger.Trace("Invoke");
                    args.ReturnValue = args.Invoke(args.Arguments);

                    /// 戻り値をログ出力します。
                    Logger.Trace(ArgumentFormatter.Format(args.ReturnValue));
                    break;

                case MethodInterceptionType.None:
                default:
                    break;
            }
        }

        #endregion
    }
}
