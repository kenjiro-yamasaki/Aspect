using SoftCube.Asserts;
using SoftCube.Logging;
using System;

namespace SoftCube.Aspects
{
    [Serializable]
    public class OnMethodBoundaryAspectLogger : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Trace("OnEntry");

            Logger.Trace(ArgumentFormatter.Format(args.Instance));

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
            Logger.Trace(ArgumentFormatter.Format(args.YieldValue));
        }

        public override void OnResume(MethodExecutionArgs args)
        {
            Logger.Trace("OnResume");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Trace("OnSuccess");

            ///// 戻り値をログ出力します。
            //Logger.Trace(ArgumentFormatter.Format(args.ReturnValue));
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
    }
}
