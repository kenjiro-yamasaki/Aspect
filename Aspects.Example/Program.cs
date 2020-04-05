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

            var arg2 = "1";
            var result = MyClass.Method1(0, out arg2);
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
        }

        public class MyClass
        {
            [Trace(Category = "A")]
            [Trace(Category = "B")]
            public static string Method1(int arg0, out string arg1)
            {
                arg1 = default;

                Logger.Trace("XX");
                return "X";
            }
        }
    }
}
