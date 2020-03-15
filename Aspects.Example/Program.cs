using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            instance.正常(0).Wait();

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
        private async Task 正常(int arg0)
        {
            Logger.Trace(arg0.ToString());

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });

            Logger.Trace("3");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("4");
            });

            Logger.Trace("5");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("6");
            });

            Logger.Trace("7");
        }
    }
}
