﻿using SoftCube.Logging;
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

            int result1;
            int result2 = 2;
            int result3;
            int result4;
            int result5;
            int result6;
            int result7;
            int result8;
            int result9;

            program.出力引数をインクリメント(out result1, result2, out result3, out result4, out result5, out result6, out result7, out result8, out result9);
            //program.出力引数をインクリメント(out result1, result2, out result3, out result4, out result5, out result6, out result7, out result8);

            Console.Read();
        }

        private class IncrementAspect : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                args.Arguments.SetArgument(0, (int)args.Arguments.GetArgument(0) + 1);
                args.Arguments.SetArgument(1, (int)args.Arguments.GetArgument(1) + 1);
                args.Arguments.SetArgument(2, (int)args.Arguments.GetArgument(2) + 1);
                args.Arguments.SetArgument(3, (int)args.Arguments.GetArgument(3) + 1);
                args.Arguments.SetArgument(4, (int)args.Arguments.GetArgument(4) + 1);
                args.Arguments.SetArgument(5, (int)args.Arguments.GetArgument(5) + 1);
                args.Arguments.SetArgument(6, (int)args.Arguments.GetArgument(6) + 1);
                args.Arguments.SetArgument(7, (int)args.Arguments.GetArgument(7) + 1);
                args.Arguments.SetArgument(8, (int)args.Arguments.GetArgument(8) + 1);
            }
        }

        [IncrementAspect]
        private void 出力引数をインクリメント(out int arg1, int arg2, out int arg3, out int arg4, out int arg5, out int arg6, out int arg7, out int arg8, out int arg9)
        {
            arg1 = 1;
            arg2 = 2;
            arg3 = 3;
            arg4 = 4;
            arg5 = 5;
            arg6 = 6;
            arg7 = 7;
            arg8 = 8;
            arg9 = 9;
        }

        //[IncrementAspect]
        //private void 出力引数をインクリメント(out int arg1, int arg2, out int arg3, out int arg4, out int arg5, out int arg6, out int arg7, out int arg8)
        //{
        //    arg1 = 1;
        //    arg2 = 2;
        //    arg3 = 3;
        //    arg4 = 4;
        //    arg5 = 5;
        //    arg6 = 6;
        //    arg7 = 7;
        //    arg8 = 8;
        //}
    }
}
