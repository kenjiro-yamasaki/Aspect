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

            char arg0 = 'a';
            program.引数を変更(ref arg0);

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
                        case sbyte argument:
                            args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                            break;

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
        private void 引数を変更(ref char arg0)
        {
            arg0 = 'b';
        }
    }
}
