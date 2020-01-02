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
        #region �e�X�g���[�e�B���e�B

        public static object @lock = new object();

        internal static StringAppender InitializeLogger()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            return appender;
        }

        #endregion

        #region ���\�b�h

        #region �����Ɩ߂�l�Ȃ�

        //[TestAspect]
        //public void �����Ɩ߂�l�Ȃ�()
        //{
        //    Console.WriteLine("A");
        //}

        //[Fact]
        //public void �����Ɩ߂�l�Ȃ�_��������()
        //{
        //    var appender = new StringAppender();
        //    appender.LogFormat = "{Message} ";
        //    Logger.Add(appender);

        //    �����Ɩ߂�l�Ȃ�();

        //    Assert.Equal($"OnEntry OnSuccess OnExit ", appender.ToString());
        //}

        #endregion

        #region ��������

        //[TestAspect]
        //public void ��������(string message)
        //{
        //    Console.WriteLine(message);
        //}

        //[Fact]
        //public void ��������_��������()
        //{
        //    var appender = new StringAppender();
        //    appender.LogFormat = "{Message} ";
        //    Logger.Add(appender);

        //    ��������("A");

        //    Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
        //}

        #endregion

        #region �萔�̖߂�l����

        public class �萔�̖߂�l����
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_��������()
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_��������()
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        n9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n10_��������()
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
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
                public void p1_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_��������()
                {
                    lock (@lock)
                    {
                        var appender = InitializeLogger();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_��������()
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








        //#region double�̒萔

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
        //public void double_0_��������()
        //{
        //    var appender = InitializeLogger();

        //    double_0();

        //    Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void double_p05_��������()
        //{
        //    var appender = InitializeLogger();

        //    double_p05();

        //    Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void double_n05_��������()
        //{
        //    var appender = InitializeLogger();

        //    double_n05();

        //    Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        //}

        //#endregion

        //#region float�̒萔

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
        //public void float_0_��������()
        //{
        //    var appender = InitializeLogger();

        //    float_0();

        //    Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void float_p05_��������()
        //{
        //    var appender = InitializeLogger();

        //    float_p05();

        //    Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void float_n05_��������()
        //{
        //    var appender = InitializeLogger();

        //    float_n05();

        //    Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        //}

        //#endregion









        //#region string

        //[TestAspect]
        //public string string�̕ϐ�()
        //{
        //    string result = "B";
        //    Logger.Trace("A");
        //    return result;
        //}

        //[Fact]
        //public void string�̕ϐ�_��������()
        //{
        //    var appender = InitializeLogger();

        //    string�̕ϐ�();

        //    Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        //}

        //[TestAspect]
        //public string string�̒l()
        //{
        //    Logger.Trace("A");
        //    return "B";
        //}

        //[Fact]
        //public void string�̒l_��������()
        //{
        //    var appender = InitializeLogger();

        //    string�̒l();

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
        //public void byte_p1_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p1();

        //    Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p2_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p2();

        //    Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p3_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p3();

        //    Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p4_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p4();

        //    Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p5_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p5();

        //    Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p6_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p6();

        //    Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p7_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p7();

        //    Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p8_��������()
        //{
        //    var appender = InitializeLogger();

        //    byte_p8();

        //    Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        //}

        //[Fact]
        //public void byte_p9_��������()
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
