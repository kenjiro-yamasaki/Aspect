using System;

namespace SoftCube.Aspects.Sample
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
            new LoggerTest().Test("BBB", 101, DateTime.Now);

            Console.Read();
        }
    }

    class LoggerTest
    {
        //[LoggerAspect]
        //public void Test(string arg0, int arg1, DateTime arg2)
        //{
        //    Console.WriteLine($"{arg0},{arg1},{arg2}");
        //    throw new ArgumentNullException(nameof(arg1));
        //}

        [LoggerAspect]
        public string Test(string arg0, int arg1, DateTime arg2)
        {
            Console.WriteLine($"{arg0},{arg1},{arg2}");
            return arg0;
            throw new Exception();
        }

        //[LoggerAspect]
        //public string Test(string arg0, int arg1, DateTime arg2)
        //{
        //    try
        //    {
        //        return arg0;
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //    //Console.WriteLine($"{arg0},{arg1},{arg2}");
        //    //throw new ArgumentNullException(nameof(arg1));
        //}

        //[LoggerAspect]
        //public void Test(string arg0, int arg1, DateTime arg2)
        //{
        //    var aspect = new LoggerAspect();
        //    var args = new MethodExecutionArgs(this, new Arguments(arg0, arg1, arg2));
        //    args.Method = MethodBase.GetCurrentMethod();

        //    aspect.OnEntry(args);
        //    try
        //    {
        //        Console.WriteLine($"{arg0},{arg1},{arg2}");

        //        aspect.OnSuccess(args);
        //    }
        //    catch (Exception e)
        //    {
        //        args.Exception = e;
        //        aspect.OnException(args);
        //    }
        //    //finally
        //    //{
        //    //    aspect.OnExit(args);
        //    //}
        //}

        //[LoggerAspect]
        //public void Test(string arg0, int arg1, DateTime arg2)
        //{
        //    Console.WriteLine("OnEntry");
        //    try
        //    {
        //        Console.WriteLine("OnSuccess");
        //        ThrowException();
        //        //throw new Exception();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("OnException");
        //        throw;
        //    }
        //    //finally
        //    //{
        //    //    Console.WriteLine("OnExit");
        //    //}
        //}
    }
}
