using SoftCube.Logging;
using SoftCube.Profiling;
using System;
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

        public class 引数と戻り値
        {
            #region 引数と戻り値なし

            [OnMethodBoundaryAspectLogger]
            private void 引数と戻り値なし()
            {
                Logger.Trace("A");
            }

            [Fact]
            public void 引数と戻り値なし_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    引数と戻り値なし();

                    Assert.Equal($"OnEntry A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region 引数のみ

            [OnMethodBoundaryAspectLogger]
            private void 引数のみ(int value)
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
            public void 引数のみ_正しくアスペクトが適用される(int value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    引数のみ(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region 複数の引数

            [OnMethodBoundaryAspectLogger]
            private void 複数の引数(int value, string @string)
            {
                Logger.Trace("A");
            }

            [Fact]
            public void 複数の引数_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    複数の引数(7, "あ");

                    Assert.Equal($"OnEntry 7 \"あ\" A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region 戻り値のみ

            [OnMethodBoundaryAspectLogger]
            private int 戻り値のみ()
            {
                Logger.Trace("A");
                return 7;
            }

            [Fact]
            public void 戻り値のみ_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = 戻り値のみ();

                    Assert.Equal(7, result);
                    Assert.Equal($"OnEntry A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region さまざまな型の引数と戻り値

            #region int

            [OnMethodBoundaryAspectLogger]
            private int @int(int value)
            {
                Logger.Trace("A");
                return value;
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
            public void int_正しくアスペクトが適用される(int value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @int(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region short

            [OnMethodBoundaryAspectLogger]
            private short @short(short value)
            {
                Logger.Trace("A");
                return value;
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
            public void short_正しくアスペクトが適用される(short value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @short(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region long

            [OnMethodBoundaryAspectLogger]
            private long @long(long value)
            {
                Logger.Trace("A");
                return value;
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
            public void long_正しくアスペクトが適用される(long value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @long(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region uint

            [OnMethodBoundaryAspectLogger]
            private uint @uint(uint value)
            {
                Logger.Trace("A");
                return value;
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
            public void uint_正しくアスペクトが適用される(uint value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @uint(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region ushort

            [OnMethodBoundaryAspectLogger]
            private ushort @ushort(ushort value)
            {
                Logger.Trace("A");
                return value;
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
            public void ushort_正しくアスペクトが適用される(ushort value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @ushort(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region ulong

            [OnMethodBoundaryAspectLogger]
            private ulong @ulong(ulong value)
            {
                Logger.Trace("A");
                return value;
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
            public void ulong_正しくアスペクトが適用される(ulong value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @ulong(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region byte

            [OnMethodBoundaryAspectLogger]
            private byte @byte(byte value)
            {
                Logger.Trace("A");
                return value;
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
            public void byte_正しくアスペクトが適用される(byte value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @byte(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region sbyte

            [OnMethodBoundaryAspectLogger]
            private sbyte @sbyte(sbyte value)
            {
                Logger.Trace("A");
                return value;
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
            public void sbyte_正しくアスペクトが適用される(sbyte value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @sbyte(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region bool

            [OnMethodBoundaryAspectLogger]
            private bool @bool(bool value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void bool_正しくアスペクトが適用される(bool value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @bool(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region double

            [OnMethodBoundaryAspectLogger]
            private double @double(double value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void double_正しくアスペクトが適用される(double value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @double(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region float

            [OnMethodBoundaryAspectLogger]
            private float @float(float value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void float_正しくアスペクトが適用される(float value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @float(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region decimal

            [OnMethodBoundaryAspectLogger]
            private decimal @decimal(decimal value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData(0.0)]
            [InlineData(0.5)]
            [InlineData(1.0)]
            [InlineData(100.0)]
            [InlineData(-0.5)]
            [InlineData(-1.0)]
            [InlineData(-100.0)]
            public void decimal_正しくアスペクトが適用される(decimal value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @decimal(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region char

            [OnMethodBoundaryAspectLogger]
            private char @char(char value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData('a')]
            [InlineData('あ')]
            public void char_正しくアスペクトが適用される(char value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @char(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry '{value}' A OnSuccess '{result}' OnExit ", appender.ToString());
                }
            }

            #endregion

            #region string

            [OnMethodBoundaryAspectLogger]
            private string @string(string value)
            {
                Logger.Trace("A");
                return value;
            }

            [Theory]
            [InlineData("a")]
            [InlineData("あ")]
            public void string_正しくアスペクトが適用される(string value)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = @string(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry \"{value}\" A OnSuccess \"{result}\" OnExit ", appender.ToString());
                }
            }

            #endregion

            #region class

            public class Class
            {
                public string Property { get; set; }

                public override string ToString() => Property;
            }

            [OnMethodBoundaryAspectLogger]
            private Class @class(Class value)
            {
                Logger.Trace("A");
                return value;
            }

            [Fact]
            public void class_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var value = new Class() { Property = "a" };

                    var result = @class(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region struct

            public struct Struct
            {
                public string Property { get; set; }

                public override string ToString() => Property;
            }

            [OnMethodBoundaryAspectLogger]
            private Struct @struct(Struct value)
            {
                Logger.Trace("A");
                return value;
            }

            [Fact]
            public void struct_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var value = new Struct() { Property = "a" };

                    var result = @struct(value);

                    Assert.Equal($"OnEntry {value} A OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            #endregion

            #region Collection

            [OnMethodBoundaryAspectLogger]
            private IEnumerable IEnumerable(IEnumerable value)
            {
                Logger.Trace("A");
                return value;
            }

            [OnMethodBoundaryAspectLogger]
            private IEnumerable<int> IEnumerableT(IEnumerable<int> value)
            {
                Logger.Trace("A");
                return value;
            }

            [OnMethodBoundaryAspectLogger]
            private List<int> ListT(List<int> value)
            {
                Logger.Trace("A");
                return value;
            }

            [Fact]
            public void IEnumerable_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var value = new List<int>() { 0 };

                    var result = IEnumerable(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void IEnumerableT_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var value = new List<int>() { 0 };

                    var result = IEnumerableT(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void ListT_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var value = new List<int>() { 0 };

                    var result = ListT(value);

                    Assert.Equal(value, result);
                    Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                }
            }

            #endregion

            #endregion
        }

        public class 制御文
        {
            #region if文

            [OnMethodBoundaryAspectLogger]
            private bool If戻り値あり(bool condition)
            {
                if (condition)
                {
                    Logger.Trace("A");
                    return true;
                }

                Logger.Trace("B");
                return false;
            }

            [OnMethodBoundaryAspectLogger]
            private bool If戻り値あり_Else(bool condition)
            {
                if (condition)
                {
                    Logger.Trace("A");
                    return true;
                }
                else
                {
                    Logger.Trace("B");
                    return false;
                }
            }

            [OnMethodBoundaryAspectLogger]
            private (bool, bool) If戻り値あり_Nest(bool condition0, bool condition1)
            {
                if (condition0)
                {
                    if (condition1)
                    {
                        Logger.Trace("A");
                        return (true, true);
                    }
                    else
                    {
                        Logger.Trace("B");
                        return (true, false);
                    }
                }
                else
                {
                    if (condition1)
                    {
                        Logger.Trace("C");
                        return (false, true);
                    }
                    else
                    {
                        Logger.Trace("D");
                        return (false, false);
                    }
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void If戻り値なし(bool condition)
            {
                if (condition)
                {
                    Logger.Trace("A");
                    return;
                }

                Logger.Trace("B");
            }

            [OnMethodBoundaryAspectLogger]
            private void If戻り値なし_Else(bool condition)
            {
                if (condition)
                {
                    Logger.Trace("A");
                    return;
                }
                else
                {
                    Logger.Trace("B");
                    return;
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void If戻り値なし_Nest(bool condition0, bool condition1)
            {
                if (condition0)
                {
                    if (condition1)
                    {
                        Logger.Trace("A");
                        return;
                    }
                    else
                    {
                        Logger.Trace("B");
                        return;
                    }
                }
                else
                {
                    if (condition1)
                    {
                        Logger.Trace("C");
                        return;
                    }
                    else
                    {
                        Logger.Trace("D");
                        return;
                    }
                }
            }

            [Theory]
            [InlineData(true, "A")]
            [InlineData(false, "B")]
            public void If戻り値あり_正しくアスペクトが適用される(bool condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = If戻り値あり(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(true, "A")]
            [InlineData(false, "B")]
            public void If戻り値あり_Else_正しくアスペクトが適用される(bool condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = If戻り値あり_Else(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(true, true, "A")]
            [InlineData(true, false, "B")]
            [InlineData(false, true, "C")]
            [InlineData(false, false, "D")]
            public void If戻り値あり_Nest_正しくアスペクトが適用される(bool condition0, bool condition1, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = If戻り値あり_Nest(condition0, condition1);

                    Assert.Equal($"OnEntry {condition0} {condition1} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(true, "A")]
            [InlineData(false, "B")]
            public void If戻り値なし_正しくアスペクトが適用される(bool condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    If戻り値なし(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(true, "A")]
            [InlineData(false, "B")]
            public void If戻り値なし_Else_正しくアスペクトが適用される(bool condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    If戻り値なし_Else(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Theory]
            [InlineData(true, true, "A")]
            [InlineData(true, false, "B")]
            [InlineData(false, true, "C")]
            [InlineData(false, false, "D")]
            public void If戻り値なし_Nest_正しくアスペクトが適用される(bool condition0, bool condition1, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    If戻り値なし_Nest(condition0, condition1);

                    Assert.Equal($"OnEntry {condition0} {condition1} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion

            #region switch文

            public enum Enum
            {
                A,
                B,
                C,
            }

            [OnMethodBoundaryAspectLogger]
            private void Switch戻り値なし_Break(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        break;

                    case Enum.B:
                        Logger.Trace("B");
                        break;

                    case Enum.C:
                        Logger.Trace("C");
                        break;

                    default:
                        Logger.Trace("D");
                        break;
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            [InlineData((Enum)3, "D")]
            public void Switch戻り値なし_Break_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Switch戻り値なし_Break(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void Switch戻り値なし_Return(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        return;

                    case Enum.B:
                        Logger.Trace("B");
                        return;

                    case Enum.C:
                        Logger.Trace("C");
                        return;

                    default:
                        Logger.Trace("D");
                        return;
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            [InlineData((Enum)3, "D")]
            public void Switch戻り値なし_Return_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Switch戻り値なし_Return(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void Switch戻り値なし_BreakWithDefaultThrow(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        break;

                    case Enum.B:
                        Logger.Trace("B");
                        break;

                    case Enum.C:
                        Logger.Trace("C");
                        break;

                    default:
                        Logger.Trace("D");
                        throw new NotSupportedException("D");
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            public void Switch戻り値なし_BreakWithDefaultThrow_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Switch戻り値なし_BreakWithDefaultThrow(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void Switch戻り値なし_BreakWithDefaultThrow_default_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var condition = (Enum)3;
                    var log = "D";

                    var ex = Record.Exception(() => Switch戻り値なし_BreakWithDefaultThrow(condition));

                    Assert.IsType<NotSupportedException>(ex);
                    Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void Switch戻り値なし_ReturnWithDefaultThrow(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        return;

                    case Enum.B:
                        Logger.Trace("B");
                        return;

                    case Enum.C:
                        Logger.Trace("C");
                        return;

                    default:
                        Logger.Trace("D");
                        throw new NotSupportedException("D");
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            public void Switch戻り値なし_ReturnWithDefaultThrow_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Switch戻り値なし_ReturnWithDefaultThrow(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void Switch戻り値なし_ReturnWithDefaultThrow_default_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var condition = (Enum)3;
                    var log = "D";

                    var ex = Record.Exception(() => Switch戻り値なし_BreakWithDefaultThrow(condition));

                    Assert.IsType<NotSupportedException>(ex);
                    Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private Enum Switch戻り値あり_Break(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        break;

                    case Enum.B:
                        Logger.Trace("B");
                        break;

                    case Enum.C:
                        Logger.Trace("C");
                        break;

                    default:
                        Logger.Trace("D");
                        break;
                }

                return condition;
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            [InlineData((Enum)3, "D")]
            public void Switch戻り値あり_Break_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = Switch戻り値あり_Break(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private Enum Switch戻り値あり_Return(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        return condition;

                    case Enum.B:
                        Logger.Trace("B");
                        return condition;

                    case Enum.C:
                        Logger.Trace("C");
                        return condition;

                    default:
                        Logger.Trace("D");
                        return condition;
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            [InlineData((Enum)3, "D")]
            public void Switch戻り値あり_Return_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = Switch戻り値あり_Return(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private Enum Switch戻り値あり_BreakWithDefaultThrow(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        break;

                    case Enum.B:
                        Logger.Trace("B");
                        break;

                    case Enum.C:
                        Logger.Trace("C");
                        break;

                    default:
                        Logger.Trace("D");
                        throw new NotSupportedException("D");
                }

                return condition;
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            public void Switch戻り値あり_BreakWithDefaultThrow_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = Switch戻り値あり_BreakWithDefaultThrow(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void Switch戻り値あり_BreakWithDefaultThrow_default_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var condition = (Enum)3;
                    var log = "D";

                    var ex = Record.Exception(() => Switch戻り値あり_BreakWithDefaultThrow(condition));

                    Assert.IsType<NotSupportedException>(ex);
                    Assert.Equal($"OnEntry {condition} {log} OnException {log} OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private Enum Switch戻り値あり_ReturnWithDefaultThrow(Enum condition)
            {
                switch (condition)
                {
                    case Enum.A:
                        Logger.Trace("A");
                        return condition;

                    case Enum.B:
                        Logger.Trace("B");
                        return condition;

                    case Enum.C:
                        Logger.Trace("C");
                        return condition;

                    default:
                        Logger.Trace("D");
                        throw new NotSupportedException("D");
                }
            }

            [Theory]
            [InlineData(Enum.A, "A")]
            [InlineData(Enum.B, "B")]
            [InlineData(Enum.C, "C")]
            public void Switch戻り値あり_ReturnWithDefaultThrow_正しくアスペクトが適用される(Enum condition, string log)
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var result = Switch戻り値あり_ReturnWithDefaultThrow(condition);

                    Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void Switch戻り値あり_ReturnWithDefaultThrow_default_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();
                    var condition = (Enum)3;
                    var log = "D";

                    var ex = Record.Exception(() => Switch戻り値あり_BreakWithDefaultThrow(condition));

                    Assert.IsType<NotSupportedException>(ex);
                    Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                }
            }

            #endregion

            #region 特殊制御

            [OnMethodBoundaryAspectLogger]
            private void Throw()
            {
                throw new Exception("A");
            }

            [Fact]
            public void Throw_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => Throw());

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry OnException A OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void ThrowIfTrue(bool condition)
            {
                if (condition)
                {
                    throw new Exception("A");
                }

                Logger.Trace("B");
                return;
            }

            [Fact]
            public void ThrowIfTrue_true_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => ThrowIfTrue(true));

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry True OnException A OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void ThrowIfTrue_false_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    ThrowIfTrue(false);

                    Assert.Equal($"OnEntry False B OnSuccess null OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void ThrowIfFalse(bool condition)
            {
                if (condition)
                {
                    Logger.Trace("A");
                    return;
                }

                throw new Exception("B");
            }

            [Fact]
            public void ThrowIfFalse_true_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    ThrowIfFalse(true);

                    Assert.Equal($"OnEntry True A OnSuccess null OnExit ", appender.ToString());
                }
            }

            [Fact]
            public void ThrowIfFalse_false_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => ThrowIfFalse(false));

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry False OnException B OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void TryCatch()
            {
                try
                {
                    throw new Exception("A");
                }
                catch (Exception ex)
                {
                    Logger.Trace(ex.Message);
                }
            }

            [Fact]
            public void TryCatch_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    TryCatch();

                    Assert.Equal($"OnEntry A OnSuccess null OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void TryCatchRethrow()
            {
                try
                {
                    throw new Exception("A");
                }
                catch (Exception ex)
                {
                    Logger.Trace(ex.Message);
                    throw ex;
                }
            }

            [Fact]
            public void TryCatchRethrow_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => TryCatchRethrow());

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry A OnException A OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void TryCatchFinally()
            {
                try
                {
                    throw new Exception("A");
                }
                catch (Exception ex)
                {
                    Logger.Trace(ex.Message);
                    throw ex;
                }
                finally
                {
                    Logger.Trace("B");
                }
            }

            [Fact]
            public void TryCatchFinally_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    var ex = Record.Exception(() => TryCatchFinally());

                    Assert.IsType<Exception>(ex);
                    Assert.Equal($"OnEntry A B OnException A OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void Using()
            {
                using (var transaction = Profiler.Start("Temp"))
                {
                    Logger.Trace("A");
                }
            }

            [Fact]
            public void Using_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Using();

                    Assert.Equal($"OnEntry A OnSuccess null OnExit ", appender.ToString());
                }
            }

            [OnMethodBoundaryAspectLogger]
            private void Lock()
            {
                lock (LockObject)
                {
                    Logger.Trace("A");
                }
            }

            [Fact]
            public void Lock_正しくアスペクトが適用される()
            {
                lock (LockObject)
                {
                    var appender = CreateAppender();

                    Lock();

                    Assert.Equal($"OnEntry A OnSuccess null OnExit ", appender.ToString());
                }
            }

            #endregion
        }
    }
}
