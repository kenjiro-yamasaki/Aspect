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

            var result = program.引数をインクリメント("a");

            Logger.Trace(result);

            Console.Read();
        }

        private class IncrementAspect : MethodInterceptionAspect
        {

            public override void OnInvoke(MethodInterceptionArgs args)
            {
                args.Proceed();
            }
        }

        [IncrementAspect]
        private string 引数をインクリメント(string arg1)
        {
            return arg1;
        }
    }
}
