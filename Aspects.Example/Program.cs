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
            instatnce.Method1();

            Console.ReadKey();
        }

        public sealed class Trace : OnMethodBoundaryAspect
        {
            public string Category { get; set; }

            public override void OnEntry(MethodExecutionArgs args)
            {
                Logger.Trace($"{args.Method.Name} {Category}");
            }
        }

        [Trace(AttributeTargetMemberAttributes = MulticastAttributes.Public, Category = "A")]
        public class MyClass
        {
            // This method will have 1 Trace aspect with Category set to A.
            public void Method1()
            {
            }

            // This method will have 2 Trace aspects with Category set to A, B
            protected void Method2()
            {
            }

            // This method will have 3 Trace aspects with Category set to A, B, C.
            [Trace(Category = "C")]
            private void Method3()
            {
            }
        }

        //public class TraceAttribute : MethodInterceptionAspect
        //{
        //    public string Category { get; set; }

        //    public override void OnInvoke(MethodInterceptionArgs args)
        //    {
        //        Logger.Trace("Entering " + args.Method.DeclaringType.FullName + "." + args.Method.Name + " " + Category);
        //        args.Proceed();
        //    }

        //    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
        //    {
        //        Logger.Trace("Entering " + args.Method.DeclaringType.FullName + "." + args.Method.Name + " " + Category);
        //        await args.ProceedAsync();
        //    }
        //}

        //[Trace(Category = "A")]
        //public class MyClass
        //{
        //    [Trace(Category = "B")]
        //    public async Task<string> Method1()
        //    {
        //        await Task.Run(() => Thread.Sleep(1000));

        //        Logger.Trace("XX");
        //        return "X";
        //    }

        //    [Trace(Category = "B")]
        //    public string Method2()
        //    {

        //        Logger.Trace("XX");
        //        return "X";
        //    }
        //}
    }
}
