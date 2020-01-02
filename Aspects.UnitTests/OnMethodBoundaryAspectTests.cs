using SoftCube.Log;
using System;
using Xunit;

namespace SoftCube.Aspects
{
    public class TestAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Info("OnEntry");

            foreach (var arg in args.Arguments)
            {
                Logger.Trace(arg.ToString());
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Trace("OnSuccess");

            if (args.ReturnValue != null)
            {
                Logger.Trace(args.ReturnValue.ToString());
            }
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Logger.Trace("OnException");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Trace("OnExit");
        }
    }




    public class OnMethodBoundaryAspectTests
    {
        #region テストユーティリティ

        internal StringAppender InitializeLogger()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            return appender;
        }

        #endregion




        #region メソッド

        #region 引数と戻り値なし

        [TestAspect]
        public void 引数と戻り値なし()
        {
            Console.WriteLine("A");
        }

        [Fact]
        public void 引数と戻り値なし_成功する()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            引数と戻り値なし();

            Assert.Equal($"OnEntry OnSuccess OnExit ", appender.ToString());
        }

        #endregion

        #region 引数あり

        [TestAspect]
        public void 引数あり(string message)
        {
            Console.WriteLine(message);
        }

        [Fact]
        public void 引数あり_成功する()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            引数あり("A");

            Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
        }

        #endregion

        #region 戻り値あり

        #region string

        [TestAspect]
        public string stringの変数()
        {
            string result = "B";
            Logger.Trace("A");
            return result;
        }

        [Fact]
        public void stringの変数_成功する()
        {
            var appender = InitializeLogger();

            stringの変数();

            Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        }

        [TestAspect]
        public string stringの値()
        {
            Logger.Trace("A");
            return "B";
        }

        [Fact]
        public void stringの値_成功する()
        {
            var appender = InitializeLogger();

            stringの値();

            Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        }

        #endregion

        #region int

        [TestAspect]
        public int intの変数()
        {
            int result = 1;
            Logger.Trace("A");
            return result;
        }

        [Fact]
        public void intの変数_成功する()
        {
            var appender = InitializeLogger();

            intの変数();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [TestAspect]
        public int intの値()
        {
            Logger.Trace("A");
            return 1;
        }

        [Fact]
        public void intの値_成功する()
        {
            var appender = InitializeLogger();

            intの値();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        #endregion


        #endregion

        #endregion
    }

}
