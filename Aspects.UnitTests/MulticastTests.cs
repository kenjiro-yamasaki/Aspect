using SoftCube.Logging;
using Xunit;

namespace SoftCube.Aspects
{
    public class MulticastTests
    {
        #region テストユーティリティ

        internal static StringAppender CreateAppender()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.ClearAndDisposeAppenders();
            Logger.Add(appender);

            return appender;
        }

        #endregion

        public sealed class TraceAttribute : OnMethodBoundaryAspect
        {
            public string Category { get; set; }

            public override void OnEntry(MethodExecutionArgs args)
            {
                Logger.Trace(args.Method.Name + "_" + Category);
            }
        }

        [Trace(Category = "A")]
        public class MyClass
        {
            // This method will have 1 Trace aspect with Category set to A.
            public void Method1()
            {
            }

            // This method will have 2 Trace aspects with Category set to A, B
            public void Method2()
            {
            }

            // This method will have 3 Trace aspects with Category set to A, B, C.
            [Trace(Category = "C")]
            public void Method3()
            {
            }
        }

        [Fact]
        public void 正常_イベントハンドラーが正しくよばれる()
        {
            var appender = CreateAppender();

            var instance = new MyClass();
            instance.Method1();

            Assert.Equal($".ctor_A Method1_A ", appender.ToString());
        }



    }
}
