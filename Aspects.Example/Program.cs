using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var instance = new MyClass();

            instance.Method1();

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

        [Trace(Category = "A")]
        public class MyClass
        {
            ////[Trace(Category = "A")]
            //public MyClass()
            //{
            //}

            //[Trace(Category = "A")]
            public void Method1()
            {
            }

            public void Method2()
            {
            }

            public void Method3()
            {
            }
        }
    }
}
