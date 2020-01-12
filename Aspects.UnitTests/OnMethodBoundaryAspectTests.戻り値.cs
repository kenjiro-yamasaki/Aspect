using SoftCube.Logging;
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
                [OnMethodBoundaryAspectLogger]
                public int p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public int p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public int p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public int p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public int p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public int p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public int p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public int p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public int p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public int p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
                public int p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [OnMethodBoundaryAspectLogger]
                public int m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [OnMethodBoundaryAspectLogger]
                public int m2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [OnMethodBoundaryAspectLogger]
                public int m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [OnMethodBoundaryAspectLogger]
                public int m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [OnMethodBoundaryAspectLogger]
                public int m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [OnMethodBoundaryAspectLogger]
                public int m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [OnMethodBoundaryAspectLogger]
                public int m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [OnMethodBoundaryAspectLogger]
                public int m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [OnMethodBoundaryAspectLogger]
                public int m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public short p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public short p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public short p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public short p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public short p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public short p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public short p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public short p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public short p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public short p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
                public short p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [OnMethodBoundaryAspectLogger]
                public short m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [OnMethodBoundaryAspectLogger]
                public short n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [OnMethodBoundaryAspectLogger]
                public short m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [OnMethodBoundaryAspectLogger]
                public short m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [OnMethodBoundaryAspectLogger]
                public short m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [OnMethodBoundaryAspectLogger]
                public short m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [OnMethodBoundaryAspectLogger]
                public short m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [OnMethodBoundaryAspectLogger]
                public short m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [OnMethodBoundaryAspectLogger]
                public short m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public long p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public long p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public long p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public long p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public long p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public long p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public long p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public long p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public long p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public long p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
                public long p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [OnMethodBoundaryAspectLogger]
                public long m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [OnMethodBoundaryAspectLogger]
                public long n2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [OnMethodBoundaryAspectLogger]
                public long m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [OnMethodBoundaryAspectLogger]
                public long m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [OnMethodBoundaryAspectLogger]
                public long m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [OnMethodBoundaryAspectLogger]
                public long m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [OnMethodBoundaryAspectLogger]
                public long m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [OnMethodBoundaryAspectLogger]
                public long m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [OnMethodBoundaryAspectLogger]
                public long m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public uint p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public uint p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public ushort p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public ushort p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public ulong p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public ulong p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public byte p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public byte p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public sbyte p0()
                {
                    Logger.Trace("A");
                    return 0;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p1()
                {
                    Logger.Trace("A");
                    return 1;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p2()
                {
                    Logger.Trace("A");
                    return 2;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p3()
                {
                    Logger.Trace("A");
                    return 3;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p4()
                {
                    Logger.Trace("A");
                    return 4;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p5()
                {
                    Logger.Trace("A");
                    return 5;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p6()
                {
                    Logger.Trace("A");
                    return 6;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p7()
                {
                    Logger.Trace("A");
                    return 7;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p8()
                {
                    Logger.Trace("A");
                    return 8;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p9()
                {
                    Logger.Trace("A");
                    return 9;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte p10()
                {
                    Logger.Trace("A");
                    return 10;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m1()
                {
                    Logger.Trace("A");
                    return -1;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m2()
                {
                    Logger.Trace("A");
                    return -2;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m3()
                {
                    Logger.Trace("A");
                    return -3;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m4()
                {
                    Logger.Trace("A");
                    return -4;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m5()
                {
                    Logger.Trace("A");
                    return -5;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m6()
                {
                    Logger.Trace("A");
                    return -6;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m7()
                {
                    Logger.Trace("A");
                    return -7;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m8()
                {
                    Logger.Trace("A");
                    return -8;
                }

                [OnMethodBoundaryAspectLogger]
                public sbyte m9()
                {
                    Logger.Trace("A");
                    return -9;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public bool @true()
                {
                    Logger.Trace("A");
                    return true;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public double p0()
                {
                    Logger.Trace("A");
                    return 0.0;
                }

                [OnMethodBoundaryAspectLogger]
                public double p05()
                {
                    Logger.Trace("A");
                    return 0.5;
                }

                [OnMethodBoundaryAspectLogger]
                public double p1()
                {
                    Logger.Trace("A");
                    return 1.0;
                }

                [OnMethodBoundaryAspectLogger]
                public double p100()
                {
                    Logger.Trace("A");
                    return 100.0;
                }

                [OnMethodBoundaryAspectLogger]
                public double m0()
                {
                    Logger.Trace("A");
                    return -0.0;
                }

                [OnMethodBoundaryAspectLogger]
                public double m05()
                {
                    Logger.Trace("A");
                    return -0.5;
                }

                [OnMethodBoundaryAspectLogger]
                public double m1()
                {
                    Logger.Trace("A");
                    return -1.0;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public float p0()
                {
                    Logger.Trace("A");
                    return 0.0f;
                }

                [OnMethodBoundaryAspectLogger]
                public float p05()
                {
                    Logger.Trace("A");
                    return 0.5f;
                }

                [OnMethodBoundaryAspectLogger]
                public float p1()
                {
                    Logger.Trace("A");
                    return 1.0f;
                }

                [OnMethodBoundaryAspectLogger]
                public float p100()
                {
                    Logger.Trace("A");
                    return 100.0f;
                }

                [OnMethodBoundaryAspectLogger]
                public float m0()
                {
                    Logger.Trace("A");
                    return -0.0f;
                }

                [OnMethodBoundaryAspectLogger]
                public float m05()
                {
                    Logger.Trace("A");
                    return -0.5f;
                }

                [OnMethodBoundaryAspectLogger]
                public float m1()
                {
                    Logger.Trace("A");
                    return -1.0f;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public decimal p0()
                {
                    Logger.Trace("A");
                    return 0.0m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal p05()
                {
                    Logger.Trace("A");
                    return 0.5m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal p1()
                {
                    Logger.Trace("A");
                    return 1.0m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal p100()
                {
                    Logger.Trace("A");
                    return 100.0m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal m0()
                {
                    Logger.Trace("A");
                    return -0.0m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal m05()
                {
                    Logger.Trace("A");
                    return -0.5m;
                }

                [OnMethodBoundaryAspectLogger]
                public decimal m1()
                {
                    Logger.Trace("A");
                    return -1.0m;
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public char a()
                {
                    Logger.Trace("A");
                    return 'a';
                }

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public string a()
                {
                    Logger.Trace("A");
                    return "a";
                }

                [OnMethodBoundaryAspectLogger]
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

                [OnMethodBoundaryAspectLogger]
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

                [OnMethodBoundaryAspectLogger]
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
                [OnMethodBoundaryAspectLogger]
                public IEnumerable IEnumerable()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [OnMethodBoundaryAspectLogger]
                public IEnumerable<int> IEnumerableT()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [OnMethodBoundaryAspectLogger]
                public List<int> ListT()
                {
                    Logger.Trace("A");

                    var result = new List<int>();
                    result.Add(0);
                    return result;
                }

                [OnMethodBoundaryAspectLogger]
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
