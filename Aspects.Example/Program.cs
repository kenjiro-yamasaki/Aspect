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

            var arg0 = 0;
            var arg1 = "1";
            var arg2 = 2;

            var (result0, result1, result2) = program.引数を変更(arg0, arg1, in arg2);


            Console.Read();
        }


        private class ChangeArguments : MethodInterceptionAspect
        {
            public override void OnInvoke(MethodInterceptionArgs args)
            {
                for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                {
                    switch (args.Arguments[argumentIndex])
                    {
                        case int argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case string argument:
                            args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                            break;

                        case null:
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }

                args.Proceed();
            }
        }

        [ChangeArguments]
        private (int, string, int) 引数を変更(int arg0, string arg1, in int arg2)
        {
            return (arg0, arg1, arg2);
        }
    }
}
