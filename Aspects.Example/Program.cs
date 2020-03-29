using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var instance = new MyClass();

            var result = instance.Method1().ToList();

            Console.ReadKey();
        }

        public class TraceAttribute : OnMethodBoundaryAspect
        {
            public string Category { get; set; }

            public override void OnEntry(MethodExecutionArgs args)
            {
                Logger.Trace("Entering " + args.Method.DeclaringType.FullName + "." + args.Method.Name + " " + Category);
            }
        }

        public class MyClass
        {
            [Trace(Category = "B")]
            public IEnumerable<string> Method1()
            {
                yield return "AA";
                yield return "BB";
            }
        }
    }
}
