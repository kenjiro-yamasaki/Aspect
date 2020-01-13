using SoftCube.Logging;
using Xunit;
using static SoftCube.Aspects.Constants;

namespace SoftCube.Aspects
{
    public class MethodInterceptionAspectTests
    {
        #region テストユーティリティ

        internal static StringAppender CreateAppender()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            return appender;
        }

        #endregion

        public class 引数
        {
            #region void

            [MethodInterceptionAspectLogger(MethodInterceptionType.Invoke)]
            private void @void()
            {
                Logger.Trace("A");
            }

            [Fact]
            public void @void_成功する()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    @void();

                    Assert.Equal($"OnInvoke A null ", appender.ToString());
                }
            }

            #endregion
        }


    }
}
