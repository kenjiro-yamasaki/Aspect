using SoftCube.Logging;
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
            //var @class = new Class();
            //@class.Instance = new Instance();
            //var result = @class.Func("1");

            var result = new Program().TryCatchFinally(7);
            Logger.Trace(result.ToString());
            Console.Read();
        }

        //[MethodInterceptionAspectLogger(MethodInterceptionType.Invoke, Name = "テスト")]
        [MethodInterceptionAspectLogger(MethodInterceptionType.Invoke)]
        private int TryCatchFinally(int value)
        {
            Logger.Trace("1");
            Logger.Trace("2");
            Logger.Trace("3");

            return value;
        }

        //[LoggerAspect]
        //private int TryCatchFinally(string value)
        //{
        //    Logger.Trace("1");
        //    Logger.Trace("2");
        //    Logger.Trace("3");
        //    return int.Parse(value);
        //}
    }

    //public class Instance
    //{
    //    public int Func(string value)
    //    {
    //        return int.Parse(value);
    //    }
    //}

    //public class Class
    //{
    //    public Instance Instance { get; set; }

    //    public object ReturnValue { get; set; }

    //    public Arguments Arguments { get; set; }

    //    [LoggerAspect]
    //    public object Func(string value)
    //    {
    //        ReturnValue = Instance.Func((string)Arguments[0]);
    //        return ReturnValue;
    //    }
    //}
}
