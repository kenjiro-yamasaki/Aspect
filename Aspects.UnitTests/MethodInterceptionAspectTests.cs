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
            private void void_Invoke()
            {
                Logger.Trace("A");
            }

            [MethodInterceptionAspectLogger(MethodInterceptionType.Proceed)]
            private void void_Proceed()
            {
                Logger.Trace("A");
            }

            [Fact]
            public void void_Invoke_成功する()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    void_Invoke();

                    Assert.Equal($"OnInvoke Invoke A null ", appender.ToString());
                }
            }

            [Fact]
            public void void_Proceed_成功する()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    void_Proceed();

                    Assert.Equal($"OnInvoke Proceed A null ", appender.ToString());
                }
            }

            #endregion

            #region int

            [MethodInterceptionAspectLogger(MethodInterceptionType.Invoke)]
            private int int_Invoke(int value)
            {
                Logger.Trace("A");
                return value;
            }

            [MethodInterceptionAspectLogger(MethodInterceptionType.Proceed)]
            private int int_Proceed(int value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(10)]
            [InlineData(-1)]
            [InlineData(-2)]
            [InlineData(-3)]
            [InlineData(-4)]
            [InlineData(-5)]
            [InlineData(-6)]
            [InlineData(-7)]
            [InlineData(-8)]
            [InlineData(-10)]
            public void int_Invoke_成功する(int value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = int_Invoke(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnInvoke {value} Invoke A {result} ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(10)]
            [InlineData(-1)]
            [InlineData(-2)]
            [InlineData(-3)]
            [InlineData(-4)]
            [InlineData(-5)]
            [InlineData(-6)]
            [InlineData(-7)]
            [InlineData(-8)]
            [InlineData(-10)]
            public void int_Proceed_成功する(int value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = int_Proceed(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnInvoke {value} Proceed A {value} ", appender.ToString());
                }
            }

            #endregion

            #region short

            [MethodInterceptionAspectLogger(MethodInterceptionType.Invoke)]
            private short short_Invoke(short value)
            {
                Logger.Trace("A");
                return value;
            }

            [MethodInterceptionAspectLogger(MethodInterceptionType.Proceed)]
            private short short_Proceed(short value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(10)]
            [InlineData(-1)]
            [InlineData(-2)]
            [InlineData(-3)]
            [InlineData(-4)]
            [InlineData(-5)]
            [InlineData(-6)]
            [InlineData(-7)]
            [InlineData(-8)]
            [InlineData(-10)]
            public void short_Invoke_成功する(short value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = short_Invoke(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnInvoke {value} Invoke A {value} ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(10)]
            [InlineData(-1)]
            [InlineData(-2)]
            [InlineData(-3)]
            [InlineData(-4)]
            [InlineData(-5)]
            [InlineData(-6)]
            [InlineData(-7)]
            [InlineData(-8)]
            [InlineData(-10)]
            public void short_Proceed_成功する(short value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = short_Proceed(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnInvoke {value} Proceed A {value} ", appender.ToString());
                }
            }

            #endregion


        }


    }
}
