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

            var result = program.引数が1つ("a");
            Logger.Trace(result);

            Console.Read();
        }


        public class ChangeArg1Aspect : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                {
                    var argument = args.Arguments[argumentIndex] as string;
                    args.Arguments[argumentIndex] = argument.ToUpper();
                }
            }
        }

        [ChangeArg1Aspect]
        public string 引数が1つ(string arg1)
        {
            return arg1;
        }

        //[Fact]
        //public void 引数が1つ_正しくアスペクトが適用される()
        //{
        //    var result = 引数が1つ("a");
        //    Assert.Equal("A", result);
        //}
    }
}
