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

            var arg0 = 0;
            var arg1 = "1";
            var reuslt = program.引数を変更(arg0, out arg1);

            Console.Read();
        }

        private class ChangeArguments : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                {
                    switch (args.Arguments[argumentIndex])
                    {
                        case bool argument:
                            args.Arguments[argumentIndex] = !argument;
                            break;

                        case sbyte argument:
                            args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                            break;

                        case short argument:
                            args.Arguments[argumentIndex] = (short)(argument + 1);
                            break;

                        case int argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case long argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case byte argument:
                            args.Arguments[argumentIndex] = (byte)(argument + 1);
                            break;

                        case ushort argument:
                            args.Arguments[argumentIndex] = (ushort)(argument + 1);
                            break;

                        case uint argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case ulong argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case float argument:
                            args.Arguments[argumentIndex] = argument + 1.0f;
                            break;

                        case double argument:
                            args.Arguments[argumentIndex] = argument + 1.0;
                            break;

                        case decimal argument:
                            args.Arguments[argumentIndex] = argument + 1.0m;
                            break;

                        case char argument:
                            args.Arguments[argumentIndex] = (int.Parse(argument.ToString()) + 1).ToString()[0];
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
            }
        }

        [ChangeArguments]
        private int 引数を変更(int arg0, out string arg1)
        {
            arg0 = 'b';
            arg1 = "c";
            return arg0;
        }
    }
}
