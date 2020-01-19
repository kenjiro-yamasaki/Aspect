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
            //new Program().Test(true);
            //Logger.Trace(result.ToString());

            var result = new Program().IEnumerableInt();

            foreach (var item in result)
            {
                Logger.Trace(item.ToString());
            }

            Console.Read();
        }

        [OnMethodBoundaryAspectLogger]
        private IEnumerable<int> IEnumerableInt()
        {
            Logger.Trace("A");
            yield return 0;
            Logger.Trace("B");
            yield return 1;
            Logger.Trace("C");
            yield return 2;
            Logger.Trace("D");
        }

        //[OnMethodBoundaryAspectLogger]
        //private void Test(bool condition)
        //{
        //    if (condition)
        //    {
        //        Logger.Trace("A");
        //        return;
        //    }

        //    Logger.Trace("B");
        //}
        //[OnMethodBoundaryAspectLogger]
        //private bool Test(bool condition)
        //{
        //    if (condition)
        //    {
        //        Logger.Trace("A");
        //        return true;
        //    }
        //    else
        //    {
        //        Logger.Trace("B");
        //        return false;
        //    }
        //}
        //[OnMethodBoundaryAspectLogger]
        //private int Test(string value)
        //{
        //    Logger.Trace("1");
        //    Logger.Trace("2");
        //    Logger.Trace("3");
        //    return int.Parse(value);
        //}
    }
}
