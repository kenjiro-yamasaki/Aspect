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

        #region 定数の戻り値あり

        #region intの定数

        [TestAspect]
        public int int_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public int int_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public int int_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public int int_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public int int_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public int int_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public int int_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public int int_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public int int_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [TestAspect]
        public int int_n1()
        {
            Logger.Trace("A");
            return -1;
        }

        [TestAspect]
        public int int_n2()
        {
            Logger.Trace("A");
            return -2;
        }

        [TestAspect]
        public int int_n3()
        {
            Logger.Trace("A");
            return -3;
        }

        [TestAspect]
        public int int_n4()
        {
            Logger.Trace("A");
            return -4;
        }

        [TestAspect]
        public int int_n5()
        {
            Logger.Trace("A");
            return -5;
        }

        [TestAspect]
        public int int_n6()
        {
            Logger.Trace("A");
            return -6;
        }

        [TestAspect]
        public int int_n7()
        {
            Logger.Trace("A");
            return -7;
        }

        [TestAspect]
        public int int_n8()
        {
            Logger.Trace("A");
            return -8;
        }

        [TestAspect]
        public int int_n9()
        {
            Logger.Trace("A");
            return -9;
        }

        [Fact]
        public void int_p1_成功する()
        {
            var appender = InitializeLogger();

            int_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p2_成功する()
        {
            var appender = InitializeLogger();

            int_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p3_成功する()
        {
            var appender = InitializeLogger();

            int_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p4_成功する()
        {
            var appender = InitializeLogger();

            int_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p5_成功する()
        {
            var appender = InitializeLogger();

            int_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p6_成功する()
        {
            var appender = InitializeLogger();

            int_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p7_成功する()
        {
            var appender = InitializeLogger();

            int_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p8_成功する()
        {
            var appender = InitializeLogger();

            int_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p9_成功する()
        {
            var appender = InitializeLogger();

            int_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n1_成功する()
        {
            var appender = InitializeLogger();

            int_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n2_成功する()
        {
            var appender = InitializeLogger();

            int_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n3_成功する()
        {
            var appender = InitializeLogger();

            int_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n4_成功する()
        {
            var appender = InitializeLogger();

            int_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n5_成功する()
        {
            var appender = InitializeLogger();

            int_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n6_成功する()
        {
            var appender = InitializeLogger();

            int_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n7_成功する()
        {
            var appender = InitializeLogger();

            int_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n8_成功する()
        {
            var appender = InitializeLogger();

            int_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n9_成功する()
        {
            var appender = InitializeLogger();

            int_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region shortの定数

        [TestAspect]
        public short short_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public short short_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public short short_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public short short_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public short short_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public short short_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public short short_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public short short_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public short short_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [TestAspect]
        public short short_n1()
        {
            Logger.Trace("A");
            return -1;
        }

        [TestAspect]
        public short short_n2()
        {
            Logger.Trace("A");
            return -2;
        }

        [TestAspect]
        public short short_n3()
        {
            Logger.Trace("A");
            return -3;
        }

        [TestAspect]
        public short short_n4()
        {
            Logger.Trace("A");
            return -4;
        }

        [TestAspect]
        public short short_n5()
        {
            Logger.Trace("A");
            return -5;
        }

        [TestAspect]
        public short short_n6()
        {
            Logger.Trace("A");
            return -6;
        }

        [TestAspect]
        public short short_n7()
        {
            Logger.Trace("A");
            return -7;
        }

        [TestAspect]
        public short short_n8()
        {
            Logger.Trace("A");
            return -8;
        }

        [TestAspect]
        public short short_n9()
        {
            Logger.Trace("A");
            return -9;
        }

        [Fact]
        public void short_p1_成功する()
        {
            var appender = InitializeLogger();

            short_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p2_成功する()
        {
            var appender = InitializeLogger();

            short_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p3_成功する()
        {
            var appender = InitializeLogger();

            short_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p4_成功する()
        {
            var appender = InitializeLogger();

            short_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p5_成功する()
        {
            var appender = InitializeLogger();

            short_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p6_成功する()
        {
            var appender = InitializeLogger();

            short_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p7_成功する()
        {
            var appender = InitializeLogger();

            short_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p8_成功する()
        {
            var appender = InitializeLogger();

            short_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p9_成功する()
        {
            var appender = InitializeLogger();

            short_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n1_成功する()
        {
            var appender = InitializeLogger();

            short_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n2_成功する()
        {
            var appender = InitializeLogger();

            short_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n3_成功する()
        {
            var appender = InitializeLogger();

            short_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n4_成功する()
        {
            var appender = InitializeLogger();

            short_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n5_成功する()
        {
            var appender = InitializeLogger();

            short_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n6_成功する()
        {
            var appender = InitializeLogger();

            short_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n7_成功する()
        {
            var appender = InitializeLogger();

            short_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n8_成功する()
        {
            var appender = InitializeLogger();

            short_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n9_成功する()
        {
            var appender = InitializeLogger();

            short_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region longの定数

        [TestAspect]
        public long long_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public long long_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public long long_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public long long_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public long long_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public long long_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public long long_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public long long_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public long long_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [TestAspect]
        public long long_n1()
        {
            Logger.Trace("A");
            return -1;
        }

        [TestAspect]
        public long long_n2()
        {
            Logger.Trace("A");
            return -2;
        }

        [TestAspect]
        public long long_n3()
        {
            Logger.Trace("A");
            return -3;
        }

        [TestAspect]
        public long long_n4()
        {
            Logger.Trace("A");
            return -4;
        }

        [TestAspect]
        public long long_n5()
        {
            Logger.Trace("A");
            return -5;
        }

        [TestAspect]
        public long long_n6()
        {
            Logger.Trace("A");
            return -6;
        }

        [TestAspect]
        public long long_n7()
        {
            Logger.Trace("A");
            return -7;
        }

        [TestAspect]
        public long long_n8()
        {
            Logger.Trace("A");
            return -8;
        }

        [TestAspect]
        public long long_n9()
        {
            Logger.Trace("A");
            return -9;
        }

        [Fact]
        public void long_p1_成功する()
        {
            var appender = InitializeLogger();

            long_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p2_成功する()
        {
            var appender = InitializeLogger();

            long_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p3_成功する()
        {
            var appender = InitializeLogger();

            long_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p4_成功する()
        {
            var appender = InitializeLogger();

            long_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p5_成功する()
        {
            var appender = InitializeLogger();

            long_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p6_成功する()
        {
            var appender = InitializeLogger();

            long_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p7_成功する()
        {
            var appender = InitializeLogger();

            long_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p8_成功する()
        {
            var appender = InitializeLogger();

            long_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p9_成功する()
        {
            var appender = InitializeLogger();

            long_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n1_成功する()
        {
            var appender = InitializeLogger();

            long_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n2_成功する()
        {
            var appender = InitializeLogger();

            long_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n3_成功する()
        {
            var appender = InitializeLogger();

            long_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n4_成功する()
        {
            var appender = InitializeLogger();

            long_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n5_成功する()
        {
            var appender = InitializeLogger();

            long_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n6_成功する()
        {
            var appender = InitializeLogger();

            long_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n7_成功する()
        {
            var appender = InitializeLogger();

            long_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n8_成功する()
        {
            var appender = InitializeLogger();

            long_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n9_成功する()
        {
            var appender = InitializeLogger();

            long_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region uintの定数

        [TestAspect]
        public uint uint_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public uint uint_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public uint uint_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public uint uint_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public uint uint_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public uint uint_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public uint uint_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public uint uint_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public uint uint_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [Fact]
        public void uint_p1_成功する()
        {
            var appender = InitializeLogger();

            uint_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p2_成功する()
        {
            var appender = InitializeLogger();

            uint_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p3_成功する()
        {
            var appender = InitializeLogger();

            uint_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p4_成功する()
        {
            var appender = InitializeLogger();

            uint_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p5_成功する()
        {
            var appender = InitializeLogger();

            uint_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p6_成功する()
        {
            var appender = InitializeLogger();

            uint_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p7_成功する()
        {
            var appender = InitializeLogger();

            uint_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p8_成功する()
        {
            var appender = InitializeLogger();

            uint_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p9_成功する()
        {
            var appender = InitializeLogger();

            uint_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region ushortの定数

        [TestAspect]
        public ushort ushort_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public ushort ushort_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public ushort ushort_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public ushort ushort_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public ushort ushort_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public ushort ushort_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public ushort ushort_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public ushort ushort_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public ushort ushort_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [Fact]
        public void ushort_p1_成功する()
        {
            var appender = InitializeLogger();

            ushort_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p2_成功する()
        {
            var appender = InitializeLogger();

            ushort_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p3_成功する()
        {
            var appender = InitializeLogger();

            ushort_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p4_成功する()
        {
            var appender = InitializeLogger();

            ushort_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p5_成功する()
        {
            var appender = InitializeLogger();

            ushort_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p6_成功する()
        {
            var appender = InitializeLogger();

            ushort_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p7_成功する()
        {
            var appender = InitializeLogger();

            ushort_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p8_成功する()
        {
            var appender = InitializeLogger();

            ushort_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p9_成功する()
        {
            var appender = InitializeLogger();

            ushort_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region ulongの定数

        [TestAspect]
        public ulong ulong_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public ulong ulong_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public ulong ulong_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public ulong ulong_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public ulong ulong_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public ulong ulong_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public ulong ulong_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public ulong ulong_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public ulong ulong_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [Fact]
        public void ulong_p1_成功する()
        {
            var appender = InitializeLogger();

            ulong_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p2_成功する()
        {
            var appender = InitializeLogger();

            ulong_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p3_成功する()
        {
            var appender = InitializeLogger();

            ulong_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p4_成功する()
        {
            var appender = InitializeLogger();

            ulong_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p5_成功する()
        {
            var appender = InitializeLogger();

            ulong_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p6_成功する()
        {
            var appender = InitializeLogger();

            ulong_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p7_成功する()
        {
            var appender = InitializeLogger();

            ulong_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p8_成功する()
        {
            var appender = InitializeLogger();

            ulong_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p9_成功する()
        {
            var appender = InitializeLogger();

            ulong_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region doubleの定数

        [TestAspect]
        public double double_0()
        {
            Logger.Trace("A");
            return 0.0;
        }

        [TestAspect]
        public double double_p05()
        {
            Logger.Trace("A");
            return 0.5;
        }

        [TestAspect]
        public double double_n05()
        {
            Logger.Trace("A");
            return -0.5;
        }

        [Fact]
        public void double_0_成功する()
        {
            var appender = InitializeLogger();

            double_0();

            Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        }

        [Fact]
        public void double_p05_成功する()
        {
            var appender = InitializeLogger();

            double_p05();

            Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        }

        [Fact]
        public void double_n05_成功する()
        {
            var appender = InitializeLogger();

            double_n05();

            Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        }

        #endregion

        #region floatの定数

        [TestAspect]
        public float float_0()
        {
            Logger.Trace("A");
            return 0.0f;
        }

        [TestAspect]
        public float float_p05()
        {
            Logger.Trace("A");
            return 0.5f;
        }

        [TestAspect]
        public float float_n05()
        {
            Logger.Trace("A");
            return -0.5f;
        }

        [Fact]
        public void float_0_成功する()
        {
            var appender = InitializeLogger();

            float_0();

            Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        }

        [Fact]
        public void float_p05_成功する()
        {
            var appender = InitializeLogger();

            float_p05();

            Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        }

        [Fact]
        public void float_n05_成功する()
        {
            var appender = InitializeLogger();

            float_n05();

            Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        }

        #endregion









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

        #region byte

        [TestAspect]
        public byte byte_p1()
        {
            Logger.Trace("A");
            return 1;
        }

        [TestAspect]
        public byte byte_p2()
        {
            Logger.Trace("A");
            return 2;
        }

        [TestAspect]
        public byte byte_p3()
        {
            Logger.Trace("A");
            return 3;
        }

        [TestAspect]
        public byte byte_p4()
        {
            Logger.Trace("A");
            return 4;
        }

        [TestAspect]
        public byte byte_p5()
        {
            Logger.Trace("A");
            return 5;
        }

        [TestAspect]
        public byte byte_p6()
        {
            Logger.Trace("A");
            return 6;
        }

        [TestAspect]
        public byte byte_p7()
        {
            Logger.Trace("A");
            return 7;
        }

        [TestAspect]
        public byte byte_p8()
        {
            Logger.Trace("A");
            return 8;
        }

        [TestAspect]
        public byte byte_p9()
        {
            Logger.Trace("A");
            return 9;
        }

        [Fact]
        public void byte_p1_成功する()
        {
            var appender = InitializeLogger();

            byte_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p2_成功する()
        {
            var appender = InitializeLogger();

            byte_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p3_成功する()
        {
            var appender = InitializeLogger();

            byte_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p4_成功する()
        {
            var appender = InitializeLogger();

            byte_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p5_成功する()
        {
            var appender = InitializeLogger();

            byte_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p6_成功する()
        {
            var appender = InitializeLogger();

            byte_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p7_成功する()
        {
            var appender = InitializeLogger();

            byte_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p8_成功する()
        {
            var appender = InitializeLogger();

            byte_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p9_成功する()
        {
            var appender = InitializeLogger();

            byte_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #endregion

        #endregion
    }

}
