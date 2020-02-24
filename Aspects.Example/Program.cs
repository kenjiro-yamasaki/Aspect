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
            program.TypeArg();

            //program.Func(typeof(int));
        }


        //private void Func(Type type) { }

        private class TypeArgLogger : OnMethodBoundaryAspect
        {
            public Type Arg { get; }
            public TypeArgLogger(Type arg) => Arg = arg;
            public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
        }

        [TypeArgLogger(typeof(int))]
        private void TypeArg() { }
    }
}
