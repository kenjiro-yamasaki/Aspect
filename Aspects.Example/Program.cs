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

            int result = 1;
            program.@out(out result);
            Logger.Trace(result.ToString());

            Console.Read();
        }

        //private class IncrementAspect : OnMethodBoundaryAspect
        //{
        //    public override void OnEntry(MethodExecutionArgs args)
        //    {
        //        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
        //        {
        //            var argument = (int)args.Arguments[argumentIndex];
        //            args.Arguments[argumentIndex] = argument + 1;
        //        }
        //    }
        //}

        //[IncrementAspect]
        //private async Task<int> 引数をインクリメント(int arg1)
        //{
        //    await Task.Run(() =>
        //    {
        //        Thread.Sleep(10);
        //    });

        //    return arg1;
        //}





        private class IncrementAspect : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                var value = (int)args.Arguments.GetArgument(0);
                args.Arguments.SetArgument(0, value + 1);
            }
        }

        [IncrementAspect]
        private void @out(out int arg1)
        {
            arg1 = 1;
        }
    }
}
