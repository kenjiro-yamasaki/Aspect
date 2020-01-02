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

        #region �߂�l����

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

        #region int

        [TestAspect]
        public int int�̕ϐ�()
        {
            int result = 1;
            Logger.Trace("A");
            return result;
        }

        [Fact]
        public void int�̕ϐ�_��������()
        {
            var appender = InitializeLogger();

            int�̕ϐ�();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        [TestAspect]
        public int int�̒l()
        {
            Logger.Trace("A");
            return 1;
        }

        [Fact]
        public void int�̒l_��������()
        {
            var appender = InitializeLogger();

            int�̒l();

            Assert.Equal($"OnEntry A OnSuccess 1 OnExit ", appender.ToString());
        }

        #endregion


        #endregion

        #endregion
    }

}
