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

            var task = program.戻り値なし();
            task.Wait();

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
                    if ((Flags & EventLoggerFlags.InvokeAsync) == EventLoggerFlags.InvokeAsync)
                    {
                        await args.InvokeAsync(args.Arguments);
                        args.ReturnValue = args.GetTaskResult();
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


        [EventLogger(EventLoggerFlags.InvokeAsync)]
        private async Task 戻り値なし()
        {
            Logger.Trace("0");

            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Logger.Trace("2");
            });

            Logger.Trace("3");
        }

        //[Fact]
        //public void 戻り値なし_イベントハンドラーが正しくよばれる()
        //{
        //    var appender = CreateAppender();

        //    var task = 戻り値なし();
        //    Logger.Trace("1");

        //    task.Wait();
        //    Logger.Trace("4");

        //    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", appender.ToString());
        //}

    }
}
