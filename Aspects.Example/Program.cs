using SoftCube.Log;
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
            new Program().TryCatchFinally();

            Logger.Trace("B");

            Console.Read();
        }


        [LoggerAspect]
        private void TryCatchFinally()
        {
            System.Diagnostics.Trace.WriteLine("A");
            //try
            //{
            //    System.Diagnostics.Trace.WriteLine("A");
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Trace.WriteLine("B");
            //    throw;
            //}
            //finally
            //{
            //    System.Diagnostics.Trace.WriteLine("C");
            //}
        }

        //public enum Enum
        //{
        //    A,
        //    B,
        //    C,
        //}

        //[LoggerAspect]
        //private Enum Return(Enum condition)
        //{
        //    switch (condition)
        //    {
        //        case Enum.A:
        //            Logger.Trace("A");
        //            return condition;

        //        case Enum.B:
        //            Logger.Trace("B");
        //            return condition;

        //        case Enum.C:
        //            Logger.Trace("C");
        //            return condition;

        //        default:
        //            Logger.Trace("D");
        //            return condition;
        //    }
        //}
    }
}
