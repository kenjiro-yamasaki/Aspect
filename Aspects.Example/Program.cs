using SoftCube.Asserts;
using SoftCube.Logging;
using System;
using System.Collections.Generic;
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

            var task = program.引数をインクリメント(1, 2, 3, 4, 5, 6, 7, 8, 9);
            task.Wait();

            var (result1, result2, result3, result4, result5, result6, result7, result8, result9) = task.Result;
            Logger.Trace(result1.ToString());
            Logger.Trace(result2.ToString());
            Logger.Trace(result3.ToString());
            Logger.Trace(result4.ToString());
            Logger.Trace(result5.ToString());
            Logger.Trace(result6.ToString());
            Logger.Trace(result7.ToString());
            Logger.Trace(result8.ToString());
            Logger.Trace(result9.ToString());

            Console.Read();
        }

        private class IncrementAspect : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                {
                    var argument = (int)args.Arguments[argumentIndex];
                    args.Arguments[argumentIndex] = argument + 1;
                }
            }
        }

        [IncrementAspect]
        private async Task<(int, int, int, int, int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
            });

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }
}
