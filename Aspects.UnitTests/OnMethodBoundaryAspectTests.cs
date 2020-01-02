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
        #region �e�X�g���[�e�B���e�B

        internal StringAppender InitializeLogger()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            return appender;
        }

        #endregion

        #region ���\�b�h

        #region �����Ɩ߂�l�Ȃ�

        [TestAspect]
        public void �����Ɩ߂�l�Ȃ�()
        {
            Console.WriteLine("A");
        }

        [Fact]
        public void �����Ɩ߂�l�Ȃ�_��������()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            �����Ɩ߂�l�Ȃ�();

            Assert.Equal($"OnEntry OnSuccess OnExit ", appender.ToString());
        }

        #endregion

        #region ��������

        [TestAspect]
        public void ��������(string message)
        {
            Console.WriteLine(message);
        }

        [Fact]
        public void ��������_��������()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message} ";
            Logger.Add(appender);

            ��������("A");

            Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
        }

        #endregion

        #region �萔�̖߂�l����

        #region int�̒萔

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
        public void int_p1_��������()
        {
            var appender = InitializeLogger();

            int_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p2_��������()
        {
            var appender = InitializeLogger();

            int_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p3_��������()
        {
            var appender = InitializeLogger();

            int_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p4_��������()
        {
            var appender = InitializeLogger();

            int_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p5_��������()
        {
            var appender = InitializeLogger();

            int_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p6_��������()
        {
            var appender = InitializeLogger();

            int_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p7_��������()
        {
            var appender = InitializeLogger();

            int_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p8_��������()
        {
            var appender = InitializeLogger();

            int_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_p9_��������()
        {
            var appender = InitializeLogger();

            int_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n1_��������()
        {
            var appender = InitializeLogger();

            int_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n2_��������()
        {
            var appender = InitializeLogger();

            int_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n3_��������()
        {
            var appender = InitializeLogger();

            int_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n4_��������()
        {
            var appender = InitializeLogger();

            int_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n5_��������()
        {
            var appender = InitializeLogger();

            int_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n6_��������()
        {
            var appender = InitializeLogger();

            int_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n7_��������()
        {
            var appender = InitializeLogger();

            int_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n8_��������()
        {
            var appender = InitializeLogger();

            int_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void int_n9_��������()
        {
            var appender = InitializeLogger();

            int_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region short�̒萔

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
        public void short_p1_��������()
        {
            var appender = InitializeLogger();

            short_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p2_��������()
        {
            var appender = InitializeLogger();

            short_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p3_��������()
        {
            var appender = InitializeLogger();

            short_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p4_��������()
        {
            var appender = InitializeLogger();

            short_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p5_��������()
        {
            var appender = InitializeLogger();

            short_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p6_��������()
        {
            var appender = InitializeLogger();

            short_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p7_��������()
        {
            var appender = InitializeLogger();

            short_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p8_��������()
        {
            var appender = InitializeLogger();

            short_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_p9_��������()
        {
            var appender = InitializeLogger();

            short_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n1_��������()
        {
            var appender = InitializeLogger();

            short_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n2_��������()
        {
            var appender = InitializeLogger();

            short_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n3_��������()
        {
            var appender = InitializeLogger();

            short_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n4_��������()
        {
            var appender = InitializeLogger();

            short_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n5_��������()
        {
            var appender = InitializeLogger();

            short_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n6_��������()
        {
            var appender = InitializeLogger();

            short_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n7_��������()
        {
            var appender = InitializeLogger();

            short_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n8_��������()
        {
            var appender = InitializeLogger();

            short_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void short_n9_��������()
        {
            var appender = InitializeLogger();

            short_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region long�̒萔

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
        public void long_p1_��������()
        {
            var appender = InitializeLogger();

            long_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p2_��������()
        {
            var appender = InitializeLogger();

            long_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p3_��������()
        {
            var appender = InitializeLogger();

            long_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p4_��������()
        {
            var appender = InitializeLogger();

            long_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p5_��������()
        {
            var appender = InitializeLogger();

            long_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p6_��������()
        {
            var appender = InitializeLogger();

            long_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p7_��������()
        {
            var appender = InitializeLogger();

            long_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p8_��������()
        {
            var appender = InitializeLogger();

            long_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_p9_��������()
        {
            var appender = InitializeLogger();

            long_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n1_��������()
        {
            var appender = InitializeLogger();

            long_n1();

            Assert.Equal($"OnEntry A OnSuccess -1 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n2_��������()
        {
            var appender = InitializeLogger();

            long_n2();

            Assert.Equal($"OnEntry A OnSuccess -2 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n3_��������()
        {
            var appender = InitializeLogger();

            long_n3();

            Assert.Equal($"OnEntry A OnSuccess -3 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n4_��������()
        {
            var appender = InitializeLogger();

            long_n4();

            Assert.Equal($"OnEntry A OnSuccess -4 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n5_��������()
        {
            var appender = InitializeLogger();

            long_n5();

            Assert.Equal($"OnEntry A OnSuccess -5 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n6_��������()
        {
            var appender = InitializeLogger();

            long_n6();

            Assert.Equal($"OnEntry A OnSuccess -6 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n7_��������()
        {
            var appender = InitializeLogger();

            long_n7();

            Assert.Equal($"OnEntry A OnSuccess -7 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n8_��������()
        {
            var appender = InitializeLogger();

            long_n8();

            Assert.Equal($"OnEntry A OnSuccess -8 OnExit ", appender.ToString());
        }

        [Fact]
        public void long_n9_��������()
        {
            var appender = InitializeLogger();

            long_n9();

            Assert.Equal($"OnEntry A OnSuccess -9 OnExit ", appender.ToString());
        }

        #endregion

        #region uint�̒萔

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
        public void uint_p1_��������()
        {
            var appender = InitializeLogger();

            uint_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p2_��������()
        {
            var appender = InitializeLogger();

            uint_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p3_��������()
        {
            var appender = InitializeLogger();

            uint_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p4_��������()
        {
            var appender = InitializeLogger();

            uint_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p5_��������()
        {
            var appender = InitializeLogger();

            uint_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p6_��������()
        {
            var appender = InitializeLogger();

            uint_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p7_��������()
        {
            var appender = InitializeLogger();

            uint_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p8_��������()
        {
            var appender = InitializeLogger();

            uint_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void uint_p9_��������()
        {
            var appender = InitializeLogger();

            uint_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region ushort�̒萔

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
        public void ushort_p1_��������()
        {
            var appender = InitializeLogger();

            ushort_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p2_��������()
        {
            var appender = InitializeLogger();

            ushort_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p3_��������()
        {
            var appender = InitializeLogger();

            ushort_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p4_��������()
        {
            var appender = InitializeLogger();

            ushort_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p5_��������()
        {
            var appender = InitializeLogger();

            ushort_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p6_��������()
        {
            var appender = InitializeLogger();

            ushort_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p7_��������()
        {
            var appender = InitializeLogger();

            ushort_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p8_��������()
        {
            var appender = InitializeLogger();

            ushort_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void ushort_p9_��������()
        {
            var appender = InitializeLogger();

            ushort_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region ulong�̒萔

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
        public void ulong_p1_��������()
        {
            var appender = InitializeLogger();

            ulong_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p2_��������()
        {
            var appender = InitializeLogger();

            ulong_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p3_��������()
        {
            var appender = InitializeLogger();

            ulong_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p4_��������()
        {
            var appender = InitializeLogger();

            ulong_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p5_��������()
        {
            var appender = InitializeLogger();

            ulong_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p6_��������()
        {
            var appender = InitializeLogger();

            ulong_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p7_��������()
        {
            var appender = InitializeLogger();

            ulong_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p8_��������()
        {
            var appender = InitializeLogger();

            ulong_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void ulong_p9_��������()
        {
            var appender = InitializeLogger();

            ulong_p9();

            Assert.Equal($"OnEntry A OnSuccess 9 OnExit ", appender.ToString());
        }

        #endregion

        #region double�̒萔

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
        public void double_0_��������()
        {
            var appender = InitializeLogger();

            double_0();

            Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        }

        [Fact]
        public void double_p05_��������()
        {
            var appender = InitializeLogger();

            double_p05();

            Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        }

        [Fact]
        public void double_n05_��������()
        {
            var appender = InitializeLogger();

            double_n05();

            Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        }

        #endregion

        #region float�̒萔

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
        public void float_0_��������()
        {
            var appender = InitializeLogger();

            float_0();

            Assert.Equal($"OnEntry A OnSuccess 0 OnExit ", appender.ToString());
        }

        [Fact]
        public void float_p05_��������()
        {
            var appender = InitializeLogger();

            float_p05();

            Assert.Equal($"OnEntry A OnSuccess 0.5 OnExit ", appender.ToString());
        }

        [Fact]
        public void float_n05_��������()
        {
            var appender = InitializeLogger();

            float_n05();

            Assert.Equal($"OnEntry A OnSuccess -0.5 OnExit ", appender.ToString());
        }

        #endregion









        #region string

        [TestAspect]
        public string string�̕ϐ�()
        {
            string result = "B";
            Logger.Trace("A");
            return result;
        }

        [Fact]
        public void string�̕ϐ�_��������()
        {
            var appender = InitializeLogger();

            string�̕ϐ�();

            Assert.Equal($"OnEntry A OnSuccess B OnExit ", appender.ToString());
        }

        [TestAspect]
        public string string�̒l()
        {
            Logger.Trace("A");
            return "B";
        }

        [Fact]
        public void string�̒l_��������()
        {
            var appender = InitializeLogger();

            string�̒l();

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
        public void byte_p1_��������()
        {
            var appender = InitializeLogger();

            byte_p1();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p2_��������()
        {
            var appender = InitializeLogger();

            byte_p2();

            Assert.Equal($"OnEntry A OnSuccess 2 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p3_��������()
        {
            var appender = InitializeLogger();

            byte_p3();

            Assert.Equal($"OnEntry A OnSuccess 3 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p4_��������()
        {
            var appender = InitializeLogger();

            byte_p4();

            Assert.Equal($"OnEntry A OnSuccess 4 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p5_��������()
        {
            var appender = InitializeLogger();

            byte_p5();

            Assert.Equal($"OnEntry A OnSuccess 5 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p6_��������()
        {
            var appender = InitializeLogger();

            byte_p6();

            Assert.Equal($"OnEntry A OnSuccess 6 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p7_��������()
        {
            var appender = InitializeLogger();

            byte_p7();

            Assert.Equal($"OnEntry A OnSuccess 7 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p8_��������()
        {
            var appender = InitializeLogger();

            byte_p8();

            Assert.Equal($"OnEntry A OnSuccess 8 OnExit ", appender.ToString());
        }

        [Fact]
        public void byte_p9_��������()
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
