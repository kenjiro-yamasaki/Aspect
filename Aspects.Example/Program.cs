using SoftCube.Logging;
using System;
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

            var task = program.戻り値なし();
            task.Wait();

            Console.Read();
        }

        private class EventLogger : MethodInterceptionAspect
        {
            public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                if (false)
                {
                    await Task.Run(() =>
                    {
                        //Thread.Sleep(10);
                    });
                }
            }
        }

        [EventLogger]
        private async Task 戻り値なし()
        {
            Logger.Trace("0");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });

            Logger.Trace("3");
        }
    }
}
