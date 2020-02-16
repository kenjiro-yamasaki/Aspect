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
            var program = new Program();

            var task = program.例外();
            Logger.Trace("1");
            task.Wait();
            Logger.Trace("8");

            //var program = new Program();

            //int result1;
            //int result2 = 2;
            //int result3;
            //int result4;
            //int result5;
            //int result6;
            //int result7;
            //int result8;
            //int result9;

            ////出力引数をインクリメント(out result1, result2, out result3, out result4, out result5, out result6, out result7, out result8, out result9);
            //出力引数をインクリメント(out result1, result2, out result3, out result4, out result5, out result6, out result7, out result8);

            Console.Read();
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
        private async Task 例外()
        {
            Logger.Trace("0");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });

            //Logger.Trace("3");

            //await Task.Run(() =>
            //{
            //    Thread.Sleep(10);
            //    Logger.Trace("4");
            //});

            //Logger.Trace("5");

            //await Task.Run(() =>
            //{
            //    Thread.Sleep(10);
            //    Logger.Trace("6");
            //});

            //Logger.Trace("7");
            throw new InvalidOperationException();
        }

    }
}
