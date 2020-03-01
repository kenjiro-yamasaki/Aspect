using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// プログラム。
    /// </summary>
    class Program
    {
        /// <summary>
        /// メイン関数。
        /// </summary>
        /// <param name="args">アプリケーション引数。</param>
        static void Main(string[] args)
        {
            var instance = new Program();

            instance.正常().ToList();

            Console.ReadKey();
        }

        private class EventLogger : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                Logger.Trace("OnEntry");
            }

            public override void OnSuccess(MethodExecutionArgs args)
            {
                Logger.Trace("OnSuccess");
            }

            public override void OnException(MethodExecutionArgs args)
            {
                Logger.Trace("OnException");
            }

            public override void OnExit(MethodExecutionArgs args)
            {
                Logger.Trace("OnExit");
            }

            public override void OnResume(MethodExecutionArgs args)
            {
                Logger.Trace("OnResume");
            }

            public override void OnYield(MethodExecutionArgs args)
            {
                Logger.Trace("OnYield");
            }
        }

        [EventLogger]
        private IEnumerable<int> 正常()
        {
            Logger.Trace("A");
            yield return 0;
            Logger.Trace("B");
            yield return 1;
            Logger.Trace("C");
        }
    }
}
