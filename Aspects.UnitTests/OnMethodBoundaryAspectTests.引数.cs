using SoftCube.Log;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using static SoftCube.Aspects.Constants;

namespace SoftCube.Aspects
{
    public partial class OnMethodBoundaryAspectTests
    {
        public class 引数
        {
            #region int

            [TestAspect]
            public void @int(int value)
            {
                Logger.Trace("A");
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
            public void int_成功する(int value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @int(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region short

            [TestAspect]
            public void @short(short value)
            {
                Logger.Trace("A");
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
            public void short_成功する(short value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @short(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region long

            [TestAspect]
            public void @long(long value)
            {
                Logger.Trace("A");
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
            public void long_成功する(long value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @long(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region uint

            [TestAspect]
            public void @uint(uint value)
            {
                Logger.Trace("A");
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
            public void uint_成功する(uint value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @uint(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region ushort

            [TestAspect]
            public void @ushort(ushort value)
            {
                Logger.Trace("A");
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
            public void ushort_成功する(ushort value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @ushort(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region ulong

            [TestAspect]
            public void @ulong(ulong value)
            {
                Logger.Trace("A");
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
            public void ulong_成功する(ulong value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @ulong(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region byte

            [TestAspect]
            public void @byte(byte value)
            {
                Logger.Trace("A");
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
            public void byte_成功する(byte value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @byte(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region sbyte

            [TestAspect]
            public void @sbyte(sbyte value)
            {
                Logger.Trace("A");
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
            public void sbyte_成功する(sbyte value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @sbyte(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region bool

            [TestAspect]
            public void @bool(bool value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void bool_成功する(bool value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @bool(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region double

            [TestAspect]
            public void @double(double value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void double_成功する(double value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @double(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region float

            [TestAspect]
            public void @float(float value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void float_成功する(float value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @float(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region decimal

            [TestAspect]
            public void @decimal(decimal value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void decimal_成功する(decimal value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @decimal(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region char

            [TestAspect]
            public void @char(char value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData('a')]
            [InlineData('あ')]
            public void char_成功する(char value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @char(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region string

            [TestAspect]
            public void @string(string value)
            {
                Logger.Trace("A");
            }

            [Theory]
            [InlineData("a")]
            [InlineData("あ")]
            public void string_成功する(string value)
            {
                lock (Lock)
                {
                    var appender = CreateAppender();

                    @string(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region class

            public class Class
            {
                public string Property { get; set; }

                public override string ToString() => Property;
            }

            [TestAspect]
            public void @class(Class value)
            {
                Logger.Trace("A");
            }

            [Fact]
            public void class_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();
                    var value = new Class() { Property = "a" };

                    @class(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region struct

            public struct Struct
            {
                public string Property { get; set; }

                public override string ToString() => Property;
            }

            [TestAspect]
            public void @struct(Struct value)
            {
                Logger.Trace("A");
            }

            [Fact]
            public void struct_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();
                    var value = new Struct() { Property = "a" };

                    @struct(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region Collection

            [TestAspect]
            public void IEnumerable(IEnumerable value)
            {
                Logger.Trace("A");
            }

            [TestAspect]
            public void IEnumerableT(IEnumerable<int> value)
            {
                Logger.Trace("A");
            }

            [TestAspect]
            public void ListT(List<int> value)
            {
                Logger.Trace("A");
            }

            [Fact]
            public void IEnumerable_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();
                    var value    = new List<int>() { 0 };

                    IEnumerable(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void IEnumerableT_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();
                    var value    = new List<int>() { 0 };

                    IEnumerableT(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void ListT_成功する()
            {
                lock (Lock)
                {
                    var appender = CreateAppender();
                    var value    = new List<int>() { 0 };

                    ListT(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion
        }
    }
}
