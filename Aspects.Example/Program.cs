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

            var result = program.引数をインクリメント(1);

            Logger.Trace(result.ToString());

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
        private async Task<int> 引数をインクリメント(int arg1)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
            });

            return arg1;
        }
    }
}
