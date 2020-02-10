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

            var task = program.戻り値をインクリメント(1);
            task.Wait();

            Logger.Trace(task.Result.ToString());
            Console.Read();
        }

        private class IncrementAspect : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                var returnValue = (int)args.ReturnValue;
                args.ReturnValue = returnValue + 1;
            }
        }

        [IncrementAspect]
        private async Task<int> 戻り値をインクリメント(int arg1)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
            });

            return arg1;
        }

                //[Fact]
                //public void 戻り値をインクリメント_正しくアスペクトが適用される()
                //{
                //    var task = 戻り値をインクリメント(1);
                //    task.Wait();

                //    Assert.Equal(2, task.Result);
                //}





        //private class ChangeArgAspect : OnMethodBoundaryAspect
        //{
        //    public override void OnEntry(MethodExecutionArgs args)
        //    {
        //        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
        //        {
        //            var argument = args.Arguments[argumentIndex] as string;
        //            args.Arguments[argumentIndex] = argument.ToUpper();
        //        }
        //    }
        //}

        //[ChangeArgAspect]
        //private async Task<string> 引数を大文字に変更(string arg1)
        //{
        //    await Task.Run(() =>
        //    {
        //        Thread.Sleep(10);
        //    });

        //    return arg1;
        //}
    }
}
