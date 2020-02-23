using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SoftCube.AsyncTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            var task = program.AsyncFunc();
            task.Wait();

            Console.Read();
        }

        [DebuggerStepThrough]
        private Task AsyncFunc()
        {
            var aspect       = new Aspect();
            var aspectArgs   = new AspectArgs();
            var stateMachine = new AsyncStateMachine(aspect, aspectArgs);
            stateMachine.Builder.Start(ref stateMachine);
            return stateMachine.Builder.Task;
        }
    }
}
