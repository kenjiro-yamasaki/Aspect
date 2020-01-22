using SoftCube.Asserts;
using SoftCube.Logging;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドタイプ。
    /// </summary>
    public enum MethodType
    {
        /// <summary>
        /// 通常のメソッド。
        /// </summary>
        NormalMethod = 0,

        /// <summary>
        /// イテレーターメソッド。
        /// </summary>
        IteratorMethod = 1,

        /// <summary>
        /// 非同期メソッド。
        /// </summary>
        AsyncMethod = 2,
    }

    public class OnMethodBoundaryAspectLogger : OnMethodBoundaryAspect
    {
        #region プロパティ

        /// <summary>
        /// メソッドタイプ。
        /// </summary>
        public MethodType MethodType { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="methodType">メソッドタイプ。</param>
        public OnMethodBoundaryAspectLogger(MethodType methodType)
        {
            MethodType = methodType;
        }

        #endregion

        #region メソッド

        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Trace("OnEntry");

            /// 引数をログ出力します。
            foreach (var argument in args.Arguments)
            {
                Logger.Trace(ArgumentFormatter.Format(argument));
            }
        }

        public override void OnYield(MethodExecutionArgs args)
        {
            Logger.Trace("OnYield");

            /// 戻り値をログ出力します。
            if (MethodType == MethodType.IteratorMethod || MethodType == MethodType.AsyncMethod)
            {
                Logger.Trace(ArgumentFormatter.Format(args.YieldValue));
            }
        }

        public override void OnResume(MethodExecutionArgs args)
        {
            Logger.Trace("OnResume");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Trace("OnSuccess");

            /// 戻り値をログ出力します。
            if (MethodType == MethodType.NormalMethod)
            {
                Logger.Trace(ArgumentFormatter.Format(args.ReturnValue));
            }
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Logger.Trace("OnException");

            Logger.Trace(args.Exception.Message);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Trace("OnExit");
        }

        #endregion
    }
}
