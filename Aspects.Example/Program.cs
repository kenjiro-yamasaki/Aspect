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
            var instance = new Program();
            
            foreach (var result in instance.IntArg())
            {
                Logger.Trace(result.ToString());
            }
            
            Console.ReadKey();
        }


        private class IntArgLogger : OnMethodBoundaryAspect
        {
            public int Arg { get; }
            public IntArgLogger(int arg) => Arg = arg;



            public override void OnEntry(MethodExecutionArgs args)
            {
                Logger.Trace(Arg.ToString());
            }
        }

        [IntArgLogger(-1)]
        private IEnumerable<int> IntArg() { yield return 1; }
    }
}
