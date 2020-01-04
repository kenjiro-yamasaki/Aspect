using SoftCube.Log;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using static SoftCube.Aspects.Constants;

namespace SoftCube.Aspects
{
    public partial class OnMethodBoundaryAspectTests
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

        public class 戻り値
        {
            public class @int
            {
                [LoggerAspect]
                public int p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public int p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public int p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public int p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public int p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public int p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public int p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public int p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public int p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public int p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public int p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [LoggerAspect]
                public int m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [LoggerAspect]
                public int m2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [LoggerAspect]
                public int m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [LoggerAspect]
                public int m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [LoggerAspect]
                public int m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [LoggerAspect]
                public int m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [LoggerAspect]
                public int m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [LoggerAspect]
                public int m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [LoggerAspect]
                public int m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [LoggerAspect]
                public int m10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @short
            {
                [LoggerAspect]
                public short p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public short p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public short p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public short p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public short p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public short p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public short p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public short p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public short p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public short p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public short p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [LoggerAspect]
                public short m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [LoggerAspect]
                public short n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [LoggerAspect]
                public short m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [LoggerAspect]
                public short m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [LoggerAspect]
                public short m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [LoggerAspect]
                public short m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [LoggerAspect]
                public short m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [LoggerAspect]
                public short m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [LoggerAspect]
                public short m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [LoggerAspect]
                public short m10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @long
            {
                [LoggerAspect]
                public long p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public long p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public long p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public long p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public long p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public long p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public long p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public long p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public long p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public long p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public long p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [LoggerAspect]
                public long m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [LoggerAspect]
                public long n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [LoggerAspect]
                public long m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [LoggerAspect]
                public long m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [LoggerAspect]
                public long m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [LoggerAspect]
                public long m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [LoggerAspect]
                public long m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [LoggerAspect]
                public long m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [LoggerAspect]
                public long m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [LoggerAspect]
                public long m10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        n2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @uint
            {
                [LoggerAspect]
                public uint p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public uint p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public uint p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public uint p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public uint p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public uint p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public uint p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public uint p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public uint p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public uint p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public uint p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @ushort
            {
                [LoggerAspect]
                public ushort p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public ushort p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public ushort p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public ushort p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public ushort p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public ushort p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public ushort p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public ushort p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public ushort p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public ushort p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public ushort p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @ulong
            {
                [LoggerAspect]
                public ulong p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public ulong p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public ulong p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public ulong p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public ulong p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public ulong p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public ulong p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public ulong p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public ulong p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public ulong p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public ulong p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @byte
            {
                [LoggerAspect]
                public byte p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public byte p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public byte p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public byte p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public byte p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public byte p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public byte p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public byte p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public byte p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public byte p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public byte p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @sbyte
            {
                [LoggerAspect]
                public sbyte p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [LoggerAspect]
                public sbyte p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [LoggerAspect]
                public sbyte p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [LoggerAspect]
                public sbyte p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [LoggerAspect]
                public sbyte p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [LoggerAspect]
                public sbyte p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [LoggerAspect]
                public sbyte p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [LoggerAspect]
                public sbyte p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [LoggerAspect]
                public sbyte p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [LoggerAspect]
                public sbyte p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [LoggerAspect]
                public sbyte p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [LoggerAspect]
                public sbyte m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [LoggerAspect]
                public sbyte m2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [LoggerAspect]
                public sbyte m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [LoggerAspect]
                public sbyte m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [LoggerAspect]
                public sbyte m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [LoggerAspect]
                public sbyte m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [LoggerAspect]
                public sbyte m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [LoggerAspect]
                public sbyte m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [LoggerAspect]
                public sbyte m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [LoggerAspect]
                public sbyte m10()
                {
                    Logger.Trace("A");
                    return -10;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p2();

                        Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p3();

                        Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p4();

                        Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p5();

                        Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p6();

                        Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p7();

                        Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p8();

                        Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p9();

                        Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p10();

                        Assert.Equal($"OnEntry A OnSuccess 10 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void n2_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m2();

                        Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m3_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m3();

                        Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m4_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m4();

                        Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m5_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m5();

                        Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m6_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m6();

                        Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m7_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m7();

                        Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m8_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m8();

                        Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m9_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m9();

                        Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m10_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m10();

                        Assert.Equal($"OnEntry A OnSuccess -10 OnExit ", appender.ToString());
                    }
                }
            }

            public class @bool
            {
                [LoggerAspect]
                public bool @true()
                {
                    Logger.Trace("A");
                    return true;
                }

                [LoggerAspect]
                public bool @false()
                {
                    Logger.Trace("A");
                    return false;
                }

                [Fact]
                public void true_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        @true();

                        Assert.Equal($"OnEntry A OnSuccess True OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void false_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        @false();

                        Assert.Equal($"OnEntry A OnSuccess False OnExit ", appender.ToString());
                    }
                }
            }

            public class @double
            {
                [LoggerAspect]
                public double p0()
                {
                    Logger.Trace("A");
                    return 0.0;
                }

                [LoggerAspect]
                public double p05()
                {
                    Logger.Trace("A");
                    return 0.5;
                }

                [LoggerAspect]
                public double p1()
                {
                    Logger.Trace("A");
                    return 1.0;
                }

                [LoggerAspect]
                public double p100()
                {
                    Logger.Trace("A");
                    return 100.0;
                }

                [LoggerAspect]
                public double m0()
                {
                    Logger.Trace("A");
                    return -0.0;
                }

                [LoggerAspect]
                public double m05()
                {
                    Logger.Trace("A");
                    return -0.5;
                }

                [LoggerAspect]
                public double m1()
                {
                    Logger.Trace("A");
                    return -1.0;
                }

                [LoggerAspect]
                public double m100()
                {
                    Logger.Trace("A");
                    return -100.0;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p05();

                        Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p100();

                        Assert.Equal($"OnEntry A OnSuccess 100 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m05();

                        Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m100();

                        Assert.Equal($"OnEntry A OnSuccess -100 OnExit ", appender.ToString());
                    }
                }
            }

            public class @float
            {
                [LoggerAspect]
                public float p0()
                {
                    Logger.Trace("A");
                    return 0.0f;
                }

                [LoggerAspect]
                public float p05()
                {
                    Logger.Trace("A");
                    return 0.5f;
                }

                [LoggerAspect]
                public float p1()
                {
                    Logger.Trace("A");
                    return 1.0f;
                }

                [LoggerAspect]
                public float p100()
                {
                    Logger.Trace("A");
                    return 100.0f;
                }

                [LoggerAspect]
                public float m0()
                {
                    Logger.Trace("A");
                    return -0.0f;
                }

                [LoggerAspect]
                public float m05()
                {
                    Logger.Trace("A");
                    return -0.5f;
                }

                [LoggerAspect]
                public float m1()
                {
                    Logger.Trace("A");
                    return -1.0f;
                }

                [LoggerAspect]
                public float m100()
                {
                    Logger.Trace("A");
                    return -100.0f;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p05();

                        Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p100();

                        Assert.Equal($"OnEntry A OnSuccess 100 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m0();

                        Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m05();

                        Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m100();

                        Assert.Equal($"OnEntry A OnSuccess -100 OnExit ", appender.ToString());
                    }
                }
            }

            public class @decimal
            {
                [LoggerAspect]
                public decimal p0()
                {
                    Logger.Trace("A");
                    return 0.0m;
                }

                [LoggerAspect]
                public decimal p05()
                {
                    Logger.Trace("A");
                    return 0.5m;
                }

                [LoggerAspect]
                public decimal p1()
                {
                    Logger.Trace("A");
                    return 1.0m;
                }

                [LoggerAspect]
                public decimal p100()
                {
                    Logger.Trace("A");
                    return 100.0m;
                }

                [LoggerAspect]
                public decimal m0()
                {
                    Logger.Trace("A");
                    return -0.0m;
                }

                [LoggerAspect]
                public decimal m05()
                {
                    Logger.Trace("A");
                    return -0.5m;
                }

                [LoggerAspect]
                public decimal m1()
                {
                    Logger.Trace("A");
                    return -1.0m;
                }

                [LoggerAspect]
                public decimal m100()
                {
                    Logger.Trace("A");
                    return -100.0m;
                }

                [Fact]
                public void p0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p0();

                        Assert.Equal($"OnEntry A OnSuccess 0.0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p05();

                        Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p1();

                        Assert.Equal($"OnEntry A OnSuccess 1.0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void p100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        p100();

                        Assert.Equal($"OnEntry A OnSuccess 100.0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m0_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m0();

                        Assert.Equal($"OnEntry A OnSuccess 0.0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m05_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m05();

                        Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m1_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m1();

                        Assert.Equal($"OnEntry A OnSuccess -1.0 OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void m100_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        m100();

                        Assert.Equal($"OnEntry A OnSuccess -100.0 OnExit ", appender.ToString());
                    }
                }
            }

            public class @char
            {
                [LoggerAspect]
                public char a()
                {
                    Logger.Trace("A");
                    return 'a';
                }

                [LoggerAspect]
                public char あ()
                {
                    Logger.Trace("A");
                    return 'あ';
                }

                [Fact]
                public void a_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        a();

                        Assert.Equal($"OnEntry A OnSuccess a OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void あ_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        あ();

                        Assert.Equal($"OnEntry A OnSuccess あ OnExit ", appender.ToString());
                    }
                }
            }

            public class @string
            {
                [LoggerAspect]
                public string a()
                {
                    Logger.Trace("A");
                    return "a";
                }

                [LoggerAspect]
                public string あ()
                {
                    Logger.Trace("A");
                    return "あ";
                }

                [Fact]
                public void a_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        a();

                        Assert.Equal($"OnEntry A OnSuccess a OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void あ_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        あ();

                        Assert.Equal($"OnEntry A OnSuccess あ OnExit ", appender.ToString());
                    }
                }
            }

            public class @class
            {
                public class Class
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [LoggerAspect]
                public Class a()
                {
                    Logger.Trace("A");
                    return new Class() { Property = "a" };
                }

                [Fact]
                public void a_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        a();

                        Assert.Equal($"OnEntry A OnSuccess a OnExit ", appender.ToString());
                    }
                }
            }

            public class @struct
            {
                public struct Struct
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [LoggerAspect]
                public Struct a()
                {
                    Logger.Trace("A");
                    return new Struct() { Property = "a" };
                }

                [Fact]
                public void a_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        a();

                        Assert.Equal($"OnEntry A OnSuccess a OnExit ", appender.ToString());
                    }
                }
            }

            public class Collection
            {
                [LoggerAspect]
                public IEnumerable IEnumerable()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [LoggerAspect]
                public IEnumerable<int> IEnumerableT()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [LoggerAspect]
                public List<int> ListT()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [LoggerAspect]
                public IEnumerable<int> 遅延評価()
                {
                    Logger.Trace("A");

                    yield return 0;
                    yield return 1;
                }

                [Fact]
                public void IEnumerable_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        IEnumerable();

                        Assert.Equal($"OnEntry A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void IEnumerableT_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        IEnumerableT();

                        Assert.Equal($"OnEntry A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void ListT_成功する()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        ListT();

                        Assert.Equal($"OnEntry A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                //[Fact]
                //public void 遅延評価_成功する()
                //{
                //    lock (Lock)
                //    {
                //        var appender = InitializeLogger();

                //        foreach (var item in 遅延評価())
                //        {
                //        }

                //        Assert.Equal($"OnEntry A OnSuccess [0] OnExit ", appender.ToString());
                //    }
                //}
            }
        }
    }
}
