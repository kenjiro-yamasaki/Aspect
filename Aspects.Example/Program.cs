using SoftCube.Log;
using System;

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
            var result = new Program().戻り値();
            Logger.Info(result.ToString());

            Console.Read();
        }

        //public struct Class
        //{
        //    public string Property { get; set; }
        //}

        //[LoggerAspect]
        //public Class 戻り値()
        //{
        //    Logger.Trace("A");
        //    return new Class() { Property = "a" };
        //}

        [LoggerAspect]
        public int 戻り値()
        {
            Logger.Trace("A");
            return 0;
        }
    }

    //class LoggerTest
    //{
    //    public class Nest
    //    {
    //        [LoggerAspect]
    //        public char 戻り値あり()
    //        {
    //            Logger.Trace("A");
    //            return 'あ';
    //        }
    //    }

    //    //[LoggerAspect]
    //    //public void Test(string arg0, int arg1, DateTime arg2)
    //    //{
    //    //    Console.WriteLine($"{arg0},{arg1},{arg2}");
    //    //    throw new ArgumentNullException(nameof(arg1));
    //    //}

    //    //[LoggerAspect]
    //    //public string Test(string arg0, int arg1, DateTime arg2)
    //    //{
    //    //    Logger.Info($"{arg0},{arg1},{arg2}");
    //    //    return arg0;
    //    //    throw new Exception();
    //    //}

    //    //[LoggerAspect]
    //    //public string Test(string arg0, int arg1, DateTime arg2)
    //    //{
    //    //    try
    //    //    {
    //    //        return arg0;
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        throw;
    //    //    }
    //    //    //Console.WriteLine($"{arg0},{arg1},{arg2}");
    //    //    //throw new ArgumentNullException(nameof(arg1));
    //    //}

    //    //[LoggerAspect]
    //    //public void Test(string arg0, int arg1, DateTime arg2)
    //    //{
    //    //    var aspect = new LoggerAspect();
    //    //    var args = new MethodExecutionArgs(this, new Arguments(arg0, arg1, arg2));
    //    //    args.Method = MethodBase.GetCurrentMethod();

    //    //    aspect.OnEntry(args);
    //    //    try
    //    //    {
    //    //        Console.WriteLine($"{arg0},{arg1},{arg2}");

    //    //        aspect.OnSuccess(args);
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        args.Exception = e;
    //    //        aspect.OnException(args);
    //    //    }
    //    //    //finally
    //    //    //{
    //    //    //    aspect.OnExit(args);
    //    //    //}
    //    //}

    //    //[LoggerAspect]
    //    //public void Test(string arg0, int arg1, DateTime arg2)
    //    //{
    //    //    Console.WriteLine("OnEntry");
    //    //    try
    //    //    {
    //    //        Console.WriteLine("OnSuccess");
    //    //        ThrowException();
    //    //        //throw new Exception();
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        Console.WriteLine("OnException");
    //    //        throw;
    //    //    }
    //    //    //finally
    //    //    //{
    //    //    //    Console.WriteLine("OnExit");
    //    //    //}
    //    //}
    //}
}
