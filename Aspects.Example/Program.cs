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

            var task = program.引数をインクリメント("4");
            Logger.Trace("1");
            task.Wait();

            Logger.Trace(task.Result);

            Console.Read();
        }

        [Serializable]
        public class IncrementAspect : MethodInterceptionAspect
        {
            //public override void OnInvoke(MethodInterceptionArgs args)
            //{
            //    args.Proceed();
            //    Logger.Trace("3");
            //}

            public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                await args.ProceedAsync();
                Logger.Trace("3");
            }
        }

        [IncrementAspect]
        private async Task<string> 引数をインクリメント(string arg1)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(100);
            });

            Logger.Trace("2");
            return arg1;
        }
    }
}
