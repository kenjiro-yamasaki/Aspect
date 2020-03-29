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
            var instance = new Program();

            var arg0 = 0;
            var arg1 = "1";

            var task = instance.引数を変更(arg0, arg1);
            task.Wait();
            var (result0, result1) = task.Result;


            Console.ReadKey();
        }

        private class ChangeArguments : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                {
                    switch (args.Arguments[argumentIndex])
                    {
                        case int argument:
                            args.Arguments[argumentIndex] = argument + 1;
                            break;

                        case string argument:
                            args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                            break;

                        case null:
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }

        [ChangeArguments]
        private async Task<(int, string)> 引数を変更(int arg0, string arg1)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
            });

            return (arg0, arg1);
        }

        //[Fact]
        //public void 引数を変更_引数2つ_正しくアスペクトが適用される()
        //{
        //    var arg0 = 0;
        //    var arg1 = "1";

        //    var task = 引数を変更(arg0, arg1);
        //    task.Wait();
        //    var (result0, result1) = task.Result;

        //    Assert.Equal(1, result0);
        //    Assert.Equal("2", result1);
        //}
        //private static object Instance;

        //private class OnEntrySpy : OnMethodBoundaryAspect
        //{
        //    public override void OnEntry(MethodExecutionArgs args)
        //    {
        //        Instance = args.Instance;
        //    }
        //}

        //[OnEntrySpy]
        //private async Task メソッド()
        //{
        //    await Task.Run(() =>
        //    {
        //        Thread.Sleep(10);
        //    });
        //}
    }
}
