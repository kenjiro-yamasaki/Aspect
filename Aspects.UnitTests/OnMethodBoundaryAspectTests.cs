using SoftCube.Log;
using System;
using Xunit;

namespace SoftCube.Aspects.UnitTests
{
    public class OnMethodBoundaryAspectTests
    {
        #region ÉÅÉ\ÉbÉh

        [Fact]
        public void Test1()
        {
            var appender = new StringAppender();
            appender.LogFormat = "{Message}{NewLine}";
            Logger.Add(appender);

            var aspectTest = new AspectTest();
            aspectTest.Test();

            Assert.Equal($"OnEntry{Environment.NewLine}OnSuccess{Environment.NewLine}OnExit{Environment.NewLine}", appender.ToString());
        }

        #endregion
    }

    public class AspectTest
    {
        [LoggerAspect]
        public void Test()
        {
            Console.WriteLine($"Hellow World");
        }
    }
}
