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
            var reuslt = program.引数を変更(arg0, arg1).Result;

            Console.Read();
        }

        private class ChangeArguments : MethodInterceptionAspect
        {
            public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                await args.ProceedAsync();
            }
        }

        [ChangeArguments]
        private async Task<int> 引数を変更(int arg0, string arg1)
        {
            await Task.Run(() => {

            });

            arg0 = 'b';
            return arg0;
        }
    }
}
