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


            var result = new Program().TryCatchFinally();
            Logger.Trace(result.ToString());
            Console.Read();
        }

        [LoggerAspect]
        private int TryCatchFinally()
        {
            Logger.Trace("Z");
            return 3;
        }
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
