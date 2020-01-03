using System;
using Xunit;
using static SoftCube.Aspects.Constants;

namespace SoftCube.Aspects
{
    public partial class OnMethodBoundaryAspectTests
    {
        public class 例外
        {
            [TestAspect]
            private void Exception()
            {
                throw new Exception("A");
            }

            [Fact]
            public void Exception_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => Exception());

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry OnException A OnExit ", appender.ToString());
                }
            }
        }
    }
}
