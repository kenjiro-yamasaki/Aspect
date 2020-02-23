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

            var task = program.値型を戻す();
            task.Wait();

            var result = task.Result;
            Logger.Trace(result.ToString());

            Console.Read();
        }

        [Flags]
        private enum EventLoggerFlags
        {
            None = 0,
            ProceedAsync = 1 << 0,
            InvokeAsync = 1 << 1,
            Rethrow = 1 << 2,
        }

        private class EventLogger : MethodInterceptionAspect
        {
            private EventLoggerFlags Flags { get; }

            public EventLogger(EventLoggerFlags flags)
            {
                Flags = flags;
            }

            public override async Task OnInvokeAsync(MethodInterceptionArgs args)
            {
                Logger.Trace("OnEntry");
                try
                {
                    if ((Flags & EventLoggerFlags.ProceedAsync) == EventLoggerFlags.ProceedAsync)
                    {
                        await args.ProceedAsync();
                        Logger.Trace("OnSuccess");
                    }
                    if ((Flags & EventLoggerFlags.InvokeAsync) == EventLoggerFlags.ProceedAsync)
                    {
                        args.ReturnValue = args.InvokeAsync(args.Arguments);
                        await (args.ReturnValue as Task);
                        Logger.Trace("OnSuccess");
                    }
                }
                catch (Exception)
                {
                    Logger.Trace("OnException");
                    if ((Flags & EventLoggerFlags.Rethrow) == EventLoggerFlags.Rethrow)
                    {
                        throw;
                    }
                }
                finally
                {
                    Logger.Trace("OnExit");
                }
            }
        }

        [EventLogger(EventLoggerFlags.ProceedAsync)]
        private async Task<int> 値型を戻す()
        {
            Logger.Trace("0");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });

            Logger.Trace("3");

            return 4;
        }

        //[Fact]
        //public void 値型を戻す_イベントハンドラーが正しくよばれる()
        //{
        //    var appender = CreateAppender();

        //    var task = 値型を戻す();
        //    Logger.Trace("1");

        //    task.Wait();
        //    var result = task.Result;
        //    Logger.Trace(result.ToString());

        //    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", appender.ToString());
        //}

    }
}
