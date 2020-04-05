using SoftCube.Logging;
using System;
using System.Reflection;
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

            var instatnce = new MyClass();
            //var result = instatnce.Method1().Result;
            var result = instatnce.Method2();
            Logger.Trace(result);

            Console.ReadKey();
        }

        public class TraceAttribute : MethodInterceptionAspect
        {
            public string Category { get; set; }

            public override void OnInvoke(MethodInterceptionArgs args)
            {
                Logger.Trace("Entering " + args.Method.DeclaringType.FullName + "." + args.Method.Name + " " + Category);
                args.Proceed();
            }

            public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                Logger.Trace("Entering " + args.Method.DeclaringType.FullName + "." + args.Method.Name + " " + Category);
                await args.ProceedAsync();
            }
        }

        public class MyClass
        {
            [Trace(Category = "A")]
            [Trace(Category = "B")]
            public async Task<string> Method1()
            {
                await Task.Run(() => Thread.Sleep(1000));

                Logger.Trace("XX");
                return "X";
            }

            [Trace(Category = "A")]
            [Trace(Category = "B")]
            public string Method2()
            {
                Logger.Trace("XX");
                return "X";
            }
        }
    }
}
