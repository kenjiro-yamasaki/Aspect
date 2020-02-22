using SoftCube.Logging;
using System;
using System.Text;
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

            var task = program.例外_();
            task.Wait();

            var result = task.Result;
            Logger.Trace(result);

            Console.Read();
        }

        public async Task<string> 例外_()
        {
            var aspectArgs = new AspectArgs(this, new Arguments());
            var aspect = new Aspect();

            var task = aspect.OnInvokeAsync(aspectArgs);
            await task;
            return aspectArgs.ReturnValue as string;
        }

        public async Task<string> 例外()
        {
            Logger.Trace("0");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });
            Logger.Trace("3");

            return "4";
        }

        private class AspectArgs : MethodInterceptionArgs
        {
            public AspectArgs(object instance, Arguments arguments)
                : base(instance, arguments)
            {
            }

            public override async Task ProceedAsync()
            {
                var program = Instance as Program;
                ReturnValue = await program.例外();
            }
        }

        private class Aspect
        {
            public async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                await args.ProceedAsync();
            }
        }
    }

}
