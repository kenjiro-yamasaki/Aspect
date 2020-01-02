using SoftCube.Log;
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

        public static object @lock = new object();

        internal static StringAppender InitializeLogger()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            return appender;
        }

        #endregion

        #region メソッド

        #region 引数と戻り値なし

        //[TestAspect]
        //public void 引数と戻り値なし()
        //{
        //    Console.WriteLine("A");
        //}

        //[Fact]
        //public void 引数と戻り値なし_成功する()
        //{
        //    var appender = new StringAppender();
        //    appender.LogFormat = "{Message} ";
        //    Logger.Add(appender);

        //    引数と戻り値なし();

        //    Assert.Equal($"OnEntry OnSuccess OnExit ", appender.ToString());
        //}

        #endregion

        #region 引数あり

        //[TestAspect]
        //public void 引数あり(string message)
        //{
        //    Console.WriteLine(message);
        //}

        //[Fact]
        //public void 引数あり_成功する()
        //{
        //    var appender = new StringAppender();
        //    appender.LogFormat = "{Message} ";
        //    Logger.Add(appender);

        //    引数あり("A");

        //    Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
        //}

        #endregion

        #region 定数の戻り値あり

        public class 定数の戻り値あり
        {
            public class @int
            {
                [TestAspect]
                public int p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public int p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public int p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public int p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public int p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public int p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public int p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public int p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public int p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public int p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [TestAspect]
                public int n1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [TestAspect]
                public int n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [TestAspect]
                public int n3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [TestAspect]
                public int n4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [TestAspect]
                public int n5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [TestAspect]
                public int n6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [TestAspect]
                public int n7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [TestAspect]
                public int n8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [TestAspect]
                public int n9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [TestAspect]
                public int n10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @short
            {
                [TestAspect]
                public short p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public short p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public short p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public short p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public short p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public short p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public short p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public short p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public short p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public short p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [TestAspect]
                public short n1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [TestAspect]
                public short n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [TestAspect]
                public short n3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [TestAspect]
                public short n4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [TestAspect]
                public short n5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [TestAspect]
                public short n6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [TestAspect]
                public short n7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [TestAspect]
                public short n8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [TestAspect]
                public short n9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [TestAspect]
                public short n10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @long
            {
                [TestAspect]
                public long p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public long p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public long p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public long p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public long p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public long p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public long p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public long p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public long p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public long p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [TestAspect]
                public long n1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [TestAspect]
                public long n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [TestAspect]
                public long n3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [TestAspect]
                public long n4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [TestAspect]
                public long n5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [TestAspect]
                public long n6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [TestAspect]
                public long n7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [TestAspect]
                public long n8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [TestAspect]
                public long n9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [TestAspect]
                public long n10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @uint
            {
                [TestAspect]
                public uint p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public uint p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public uint p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public uint p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public uint p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public uint p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public uint p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public uint p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public uint p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public uint p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @ushort
            {
                [TestAspect]
                public ushort p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public ushort p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public ushort p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public ushort p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public ushort p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public ushort p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public ushort p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public ushort p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public ushort p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public ushort p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @ulong
            {
                [TestAspect]
                public ulong p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [TestAspect]
                public ulong p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [TestAspect]
                public ulong p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [TestAspect]
                public ulong p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [TestAspect]
                public ulong p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [TestAspect]
                public ulong p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [TestAspect]
                public ulong p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [TestAspect]
                public ulong p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [TestAspect]
                public ulong p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [TestAspect]
                public ulong p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }
        }








        //#region doubleの定数

        //[TestAspect]
        //public double double_0()
        //{
        //    Logger.Trace("A");
        //    return 0.0;
        //}

        //[TestAspect]
        //public double double_p05()
        //{
        //    Logger.Trace("A");
        //    return 0.5;
        //}

        //[TestAspect]
        //public double double_n05()
        //{
        //    Logger.Trace("A");
        //    return -0.5;
        //}

        //[Fact]
        //public void double_0_成功する()
        //{
        //    var appender = InitializeLogger();

        //    double_0();

        //    Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void double_p05_成功する()
        //{
        //    var appender = InitializeLogger();

        //    double_p05();

        //    Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void double_n05_成功する()
        //{
        //    var appender = InitializeLogger();

        //    double_n05();

        //    Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        //}

        //#endregion

        //#region floatの定数

        //[TestAspect]
        //public float float_0()
        //{
        //    Logger.Trace("A");
        //    return 0.0f;
        //}

        //[TestAspect]
        //public float float_p05()
        //{
        //    Logger.Trace("A");
        //    return 0.5f;
        //}

        //[TestAspect]
        //public float float_n05()
        //{
        //    Logger.Trace("A");
        //    return -0.5f;
        //}

        //[Fact]
        //public void float_0_成功する()
        //{
        //    var appender = InitializeLogger();

        //    float_0();

        //    Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void float_p05_成功する()
        //{
        //    var appender = InitializeLogger();

        //    float_p05();

        //    Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void float_n05_成功する()
        //{
        //    var appender = InitializeLogger();

        //    float_n05();

        //    Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        //}

        //#endregion









        //#region string

        //[TestAspect]
        //public string stringの変数()
        //{
        //    string result = "B";
        //    Logger.Trace("A");
        //    return result;
        //}

        //[Fact]
        //public void stringの変数_成功する()
        //{
        //    var appender = InitializeLogger();

        //    stringの変数();

        //    Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        //}

        //[TestAspect]
        //public string stringの値()
        //{
        //    Logger.Trace("A");
        //    return "B";
        //}

        //[Fact]
        //public void stringの値_成功する()
        //{
        //    var appender = InitializeLogger();

        //    stringの値();

        //    Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        //}

        //#endregion

        //#region byte

        //[TestAspect]
        //public byte byte_p1()
        //{
        //    Logger.Trace("A");
        //    return 1;
        //}

        //[TestAspect]
        //public byte byte_p2()
        //{
        //    Logger.Trace("A");
        //    return 2;
        //}

        //[TestAspect]
        //public byte byte_p3()
        //{
        //    Logger.Trace("A");
        //    return 3;
        //}

        //[TestAspect]
        //public byte byte_p4()
        //{
        //    Logger.Trace("A");
        //    return 4;
        //}

        //[TestAspect]
        //public byte byte_p5()
        //{
        //    Logger.Trace("A");
        //    return 5;
        //}

        //[TestAspect]
        //public byte byte_p6()
        //{
        //    Logger.Trace("A");
        //    return 6;
        //}

        //[TestAspect]
        //public byte byte_p7()
        //{
        //    Logger.Trace("A");
        //    return 7;
        //}

        //[TestAspect]
        //public byte byte_p8()
        //{
        //    Logger.Trace("A");
        //    return 8;
        //}

        //[TestAspect]
        //public byte byte_p9()
        //{
        //    Logger.Trace("A");
        //    return 9;
        //}

        //[Fact]
        //public void byte_p1_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p1();

        //    Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p2_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p2();

        //    Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p3_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p3();

        //    Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p4_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p4();

        //    Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p5_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p5();

        //    Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p6_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p6();

        //    Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p7_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p7();

        //    Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p8_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p8();

        //    Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p9_成功する()
        //{
        //    var appender = InitializeLogger();

        //    byte_p9();

        //    Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        //}

        //#endregion

        #endregion

        #endregion
    }

}
