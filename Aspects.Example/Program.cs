using SoftCube.Logging;
using System;
using System.Collections.Generic;

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

            var result = new Program().IEnumerableInt();
            Logger.Trace(result.ToString());
            Console.Read();
        }

        [OnMethodBoundaryAspectLogger]
        private IEnumerable<int> IEnumerableInt()
        {
            //return new int[] {0,1,2};

            Logger.Trace("A");
            yield return 0;
            Logger.Trace("B");
            yield return 1;
            Logger.Trace("C");
            yield return 2;
            Logger.Trace("D");
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
