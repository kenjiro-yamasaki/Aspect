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
            var arg2 = 2;
            var arg3 = "3";
            var arg4 = 4;
            var arg5 = "5";

            program.戻り値なし(arg0, arg1, ref arg2, ref arg3, out arg4, out arg5);


            Console.Read();
        }

        private class EventLogger : MethodInterceptionAspect
        {
            public override void OnInvoke(MethodInterceptionArgs args)
            {
                args.Proceed();
            }

            //public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            //{
            //    if (false)
            //    {
            //        await Task.Run(() =>
            //        {
            //            //Thread.Sleep(10);
            //        });
            //    }
            //}
        }

        [EventLogger]
        private int 戻り値なし(int arg0, string arg1, ref int arg2, ref string arg3, out int arg4, out string arg5)
        {
            arg4 = 4;
            arg5 = "5";
            return arg0;
        }


        private int Test(int arg0, string arg1, ref int arg2, ref string arg3)
        {
            EventLogger eventLogger = new EventLogger();
            var arguments = new Arguments<int, string, int, string>(arg0, arg1, arg2, arg3);

            arg0 = arguments.Arg0;
            arg1 = arguments.Arg1;
            arg2 = arguments.Arg2;
            arg3 = arguments.Arg3;

            return arg0;

            //System.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs system.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs = new System.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs(this, arguments);
            //system.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs.Method = MethodBase.GetCurrentMethod();
            //eventLogger.OnInvoke(system.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs);
            //arg0 = arguments.Arg0;
            //arg1 = arguments.Arg1;
            //arg2 = arguments.Arg2;
            //*(int*)(&arg3) = (int)arguments.Arg3;
            //arg4 = arguments.Arg4;
            //*(int*)(&arg5) = (int)arguments.Arg5;
            //return (int)system.Int32 SoftCube.Aspects.Program::戻り値なし(System.Int32, System.String, System.Int32 &, System.String &, System.Int32 &, System.String &) + MethodInterceptionArgs.ReturnValue;
        }
    }
}
