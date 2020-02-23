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
            var arg2 = 2.0;
            var arg3 = "3";
            var arg4 = 4;
            var arg5 = "5";

            //program.戻り値なし(arg0, arg1, ref arg2, ref arg3, out arg4, out arg5);


            Console.Read();
        }

        private class EventLogger : MethodInterceptionAspect
        {
            public override void OnInvoke(MethodInterceptionArgs args)
            {
                args.Proceed();
            }
        }

        [EventLogger]
        private int 戻り値なし(ref decimal  arg0, ref byte arg1, ref ushort arg2, ref uint arg3, ref long arg4, ref ulong arg5)
        {
            return (int)arg0;
        }


        private void Test(ref ulong arg0, ref ushort arg1, ref bool arg2, ref double arg3)
        {
            EventLogger eventLogger = new EventLogger();
            var arguments = new Arguments<ulong, ushort, bool, double>(arg0, arg1, arg2, arg3);

            arg0 = arguments.Arg0;
            arg1 = arguments.Arg1;
            arg2 = arguments.Arg2;
            arg3 = arguments.Arg3;
        }
    }
}
