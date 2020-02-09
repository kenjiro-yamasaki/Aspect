using SoftCube.Logging;
using SoftCube.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            Logger.ClearAndDisposeAppenders();
            Logger.Add(appender);

            return appender;
        }

        #endregion

        public class 通常のメソッド
        {
            public class 署名
            {
                #region 引数と戻り値なし

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数のみ(int arg)
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
                public void 引数のみ_正しくアスペクトが適用される(int arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数のみ(arg);

                        Assert.Equal($"OnEntry {arg} A OnSuccess null OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region 引数の個数

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が1つ(string arg1)
                {
                    Logger.Trace(arg1);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が2つ(string arg1, string arg2)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が3つ(string arg1, string arg2, string arg3)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が4つ(string arg1, string arg2, string arg3, string arg4)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が5つ(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                    Logger.Trace(arg5);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が6つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                    Logger.Trace(arg5);
                    Logger.Trace(arg6);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が7つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                    Logger.Trace(arg5);
                    Logger.Trace(arg6);
                    Logger.Trace(arg7);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が8つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                    Logger.Trace(arg5);
                    Logger.Trace(arg6);
                    Logger.Trace(arg7);
                    Logger.Trace(arg8);
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void 引数が9つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    Logger.Trace(arg1);
                    Logger.Trace(arg2);
                    Logger.Trace(arg3);
                    Logger.Trace(arg4);
                    Logger.Trace(arg5);
                    Logger.Trace(arg6);
                    Logger.Trace(arg7);
                    Logger.Trace(arg8);
                    Logger.Trace(arg9);
                }

                [Fact]
                public void 引数が1つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が1つ("a");

                        Assert.Equal($"OnEntry \"a\" a OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が2つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が2つ("a", "b");

                        Assert.Equal($"OnEntry \"a\" \"b\" a b OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が3つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が3つ("a", "b", "c");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" a b c OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が4つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が4つ("a", "b", "c", "d");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" a b c d OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が5つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が5つ("a", "b", "c", "d", "e");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" a b c d e OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が6つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が6つ("a", "b", "c", "d", "e", "f");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" a b c d e f OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が7つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が7つ("a", "b", "c", "d", "e", "f", "g");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" a b c d e f g OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が8つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が8つ("a", "b", "c", "d", "e", "f", "g", "h");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" \"h\" a b c d e f g h OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が9つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が9つ("a", "b", "c", "d", "e", "f", "g", "h", "i");

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" \"h\" \"i\" a b c d e f g h i OnSuccess null OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region 戻り値のみ

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private int @int(int arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void int_正しくアスペクトが適用される(int arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @int(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region short

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private short @short(short arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void short_正しくアスペクトが適用される(short arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @short(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region long

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private long @long(long arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void long_正しくアスペクトが適用される(long arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @long(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region uint

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private uint @uint(uint arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void uint_正しくアスペクトが適用される(uint arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @uint(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region ushort

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private ushort @ushort(ushort arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void ushort_正しくアスペクトが適用される(ushort arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @ushort(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region ulong

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private ulong @ulong(ulong arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void ulong_正しくアスペクトが適用される(ulong arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @ulong(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region byte

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private byte @byte(byte arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void byte_正しくアスペクトが適用される(byte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @byte(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region sbyte

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private sbyte @sbyte(sbyte arg)
                {
                    Logger.Trace("A");
                    return arg;
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
                public void sbyte_正しくアスペクトが適用される(sbyte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @sbyte(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region bool

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private bool @bool(bool arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData(true)]
                [InlineData(false)]
                public void bool_正しくアスペクトが適用される(bool arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @bool(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region double

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private double @double(double arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void double_正しくアスペクトが適用される(double arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @double(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region float

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private float @float(float arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void float_正しくアスペクトが適用される(float arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @float(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region decimal

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private decimal @decimal(decimal arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void decimal_正しくアスペクトが適用される(decimal arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @decimal(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region char

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private char @char(char arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData('a')]
                [InlineData('あ')]
                public void char_正しくアスペクトが適用される(char arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @char(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry '{arg}' A OnSuccess '{result}' OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region string

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private string @string(string arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Theory]
                [InlineData("a")]
                [InlineData("あ")]
                public void string_正しくアスペクトが適用される(string arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var result = @string(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry \"{arg}\" A OnSuccess \"{result}\" OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region class

                public class Class
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private Class @class(Class arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Fact]
                public void class_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Class() { Property = "a" };

                        var result = @class(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region struct

                public struct Struct
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private Struct @struct(Struct arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Fact]
                public void struct_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Struct() { Property = "a" };

                        var result = @struct(arg);

                        Assert.Equal($"OnEntry {arg} A OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region Collection

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private IEnumerable IEnumerable(IEnumerable arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private IEnumerable<int> IEnumerableT(IEnumerable<int> arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private List<int> ListT(List<int> arg)
                {
                    Logger.Trace("A");
                    return arg;
                }

                [Fact]
                public void IEnumerable_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 0 };

                        var result = IEnumerable(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void IEnumerableT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 0 };

                        var result = IEnumerableT(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void ListT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 0 };

                        var result = ListT(arg);

                        Assert.Equal(arg, result);
                        Assert.Equal($"OnEntry [0] A OnSuccess [0] OnExit ", appender.ToString());
                    }
                }

                #endregion

                #endregion
            }

            public class 制御文
            {
                #region if文

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
                private void If戻り値なし(bool condition)
                {
                    if (condition)
                    {
                        Logger.Trace("A");
                        return;
                    }

                    Logger.Trace("B");
                }

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

                [OnMethodBoundaryAspectLogger(MethodType.NormalMethod)]
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

            public class 引数の変更
            {
                private class ChangeArgAspect : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                        {
                            var argument = args.Arguments[argumentIndex] as string;
                            args.Arguments[argumentIndex] = argument.ToUpper();
                        }
                    }
                }

                [ChangeArgAspect]
                private string 引数を大文字に変更(string arg1)
                {
                    return arg1;
                }

                [ChangeArgAspect]
                private (string, string) 引数を大文字に変更(string arg1, string arg2)
                {
                    return (arg1, arg2);
                }

                [ChangeArgAspect]
                private (string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3)
                {
                    return (arg1, arg2, arg3);
                }

                [ChangeArgAspect]
                private (string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4)
                {
                    return (arg1, arg2, arg3, arg4);
                }

                [ChangeArgAspect]
                private (string, string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    return (arg1, arg2, arg3, arg4, arg5);
                }

                [ChangeArgAspect]
                private (string, string, string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    return (arg1, arg2, arg3, arg4, arg5, arg6);
                }

                [ChangeArgAspect]
                private (string, string, string, string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }

                [ChangeArgAspect]
                private (string, string, string, string, string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }

                [ChangeArgAspect]
                private (string, string, string, string, string, string, string, string, string) 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                }

                [Fact]
                public void 引数を大文字に変更_引数が1つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a");
                    Assert.Equal("A", result);
                }

                [Fact]
                public void 引数を大文字に変更_引数が2つ_正しくアスペクトが適用される()
                {
                    var (result1, result2) = 引数を大文字に変更("a", "b");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                }

                [Fact]
                public void 引数を大文字に変更_引数が3つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3) = 引数を大文字に変更("a", "b", "c");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                }

                [Fact]
                public void 引数を大文字に変更_引数が4つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4) = 引数を大文字に変更("a", "b", "c", "d");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                }

                [Fact]
                public void 引数を大文字に変更_引数が5つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4, result5) = 引数を大文字に変更("a", "b", "c", "d", "e");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                }

                [Fact]
                public void 引数を大文字に変更_引数が6つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4, result5, result6) = 引数を大文字に変更("a", "b", "c", "d", "e", "f");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                    Assert.Equal("F", result6);
                }

                [Fact]
                public void 引数を大文字に変更_引数が7つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4, result5, result6, result7) = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                    Assert.Equal("F", result6);
                    Assert.Equal("G", result7);
                }

                [Fact]
                public void 引数を大文字に変更_引数が8つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4, result5, result6, result7, result8) = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                    Assert.Equal("F", result6);
                    Assert.Equal("G", result7);
                    Assert.Equal("H", result8);
                }

                [Fact]
                public void 引数を大文字に変更_引数が9つ_正しくアスペクトが適用される()
                {
                    var (result1, result2, result3, result4, result5, result6, result7, result8, result9) = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h", "i");
                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                    Assert.Equal("F", result6);
                    Assert.Equal("G", result7);
                    Assert.Equal("H", result8);
                    Assert.Equal("I", result9);
                }
            }

            public class 戻り値の変更
            {
                private class ChangeReturnValueAspect : OnMethodBoundaryAspect
                {
                    public override void OnExit(MethodExecutionArgs args)
                    {
                        var returnValue = args.ReturnValue as string;
                        args.ReturnValue = returnValue.ToUpper();
                    }
                }

                [ChangeReturnValueAspect]
                private string 戻り値を大文字に変更(string arg1)
                {
                    return arg1;
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var result = 戻り値を大文字に変更("a");
                    Assert.Equal("A", result);
                }
            }
        }

        public class イテレーターメソッド
        {
            public class 署名
            {
                #region 引数の個数

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数なし()
                {
                    yield break;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が1つ(string arg1)
                {
                    yield return arg1;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が2つ(string arg1, string arg2)
                {
                    yield return arg1;
                    yield return arg2;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が3つ(string arg1, string arg2, string arg3)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が4つ(string arg1, string arg2, string arg3, string arg4)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が5つ(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が6つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が7つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が8つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> 引数が9つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
                    yield return arg9;
                }

                [Fact]
                public void 引数なし_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数なし())
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が1つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が1つ("a"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" OnYield \"a\" a OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が2つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が2つ("a", "b"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が3つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が3つ("a", "b", "c"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が4つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が4つ("a", "b", "c", "d"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が5つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が5つ("a", "b", "c", "d", "e"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnYield \"e\" e OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が6つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が6つ("a", "b", "c", "d", "e", "f"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnYield \"e\" e OnResume OnYield \"f\" f OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が7つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が7つ("a", "b", "c", "d", "e", "f", "g"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnYield \"e\" e OnResume OnYield \"f\" f OnResume OnYield \"g\" g OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が8つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が8つ("a", "b", "c", "d", "e", "f", "g", "h"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" \"h\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnYield \"e\" e OnResume OnYield \"f\" f OnResume OnYield \"g\" g OnResume OnYield \"h\" h OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が9つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var @int in 引数が9つ("a", "b", "c", "d", "e", "f", "g", "h", "i"))
                        {
                            Logger.Trace(@int.ToString());
                        }

                        Assert.Equal($"OnEntry \"a\" \"b\" \"c\" \"d\" \"e\" \"f\" \"g\" \"h\" \"i\" OnYield \"a\" a OnResume OnYield \"b\" b OnResume OnYield \"c\" c OnResume OnYield \"d\" d OnResume OnYield \"e\" e OnResume OnYield \"f\" f OnResume OnYield \"g\" g OnResume OnYield \"h\" h OnResume OnYield \"i\" i OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region 引数と戻り値の型

                #region int

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<int> @int(int arg)
                {
                    yield return arg;
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
                public void int_正しくアスペクトが適用される(int arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @int(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region short

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<short> @short(short arg)
                {
                    yield return arg;
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
                public void short_正しくアスペクトが適用される(short arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @short(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region long

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<long> @long(long arg)
                {
                    yield return arg;
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
                public void long_正しくアスペクトが適用される(long arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @long(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region sbyte

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<sbyte> @sbyte(sbyte arg)
                {
                    yield return arg;
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
                public void sbyte_正しくアスペクトが適用される(sbyte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @sbyte(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region uint

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<uint> @uint(uint arg)
                {
                    yield return arg;
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
                public void uint_正しくアスペクトが適用される(uint arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @uint(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region ushort

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<ushort> @ushort(ushort arg)
                {
                    yield return arg;
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
                public void ushort_正しくアスペクトが適用される(ushort arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @ushort(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region ulong

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<ulong> @ulong(ulong arg)
                {
                    yield return arg;
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
                public void ulong_正しくアスペクトが適用される(ulong arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @ulong(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region byte

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<byte> @byte(byte arg)
                {
                    yield return arg;
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
                public void byte_正しくアスペクトが適用される(byte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @byte(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region bool

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<bool> @bool(bool arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData(true)]
                [InlineData(false)]
                public void bool_正しくアスペクトが適用される(bool arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @bool(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region double

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<double> @double(double arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void double_正しくアスペクトが適用される(double arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @double(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region float

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<float> @float(float arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void float_正しくアスペクトが適用される(float arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @float(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region decimal

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<decimal> @decimal(decimal arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void decimal_正しくアスペクトが適用される(decimal arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @decimal(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region char

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<char> @char(char arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData('a')]
                [InlineData('あ')]
                public void char_正しくアスペクトが適用される(char arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @char(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry '{arg}' OnYield '{arg}' {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region string

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<string> @string(string arg)
                {
                    yield return arg;
                }

                [Theory]
                [InlineData("a")]
                [InlineData("あ")]
                public void string_正しくアスペクトが適用される(string arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        foreach (var result in @string(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry \"{arg}\" OnYield \"{arg}\" {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region class

                public class Class
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<Class> @class(Class arg)
                {
                    yield return arg;
                }

                [Fact]
                public void class_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Class() { Property = "a" };

                        foreach (var result in @class(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region struct

                public struct Struct
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<Struct> @struct(Struct arg)
                {
                    yield return arg;
                }

                [Fact]
                public void struct_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Struct() { Property = "a" };

                        foreach (var result in @struct(arg))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry {arg} OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region IEnumerable

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable IEnumerable(IEnumerable collection)
                {
                    foreach (var item in collection)
                    {
                        yield return item;
                    }
                }

                [Fact]
                public void IEnumerable_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        int arg = 7;

                        foreach (var result in IEnumerable(new List<int>() { arg }))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry [{arg}] OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region IEnumerableT

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<int> IEnumerableT(IEnumerable<int> collection)
                {
                    foreach (var item in collection)
                    {
                        yield return item;
                    }
                }

                [Fact]
                public void IEnumerableT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        int arg = 7;

                        foreach (var result in IEnumerableT(new List<int>() { arg }))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry [{arg}] OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #region ListT

                [OnMethodBoundaryAspectLogger(MethodType.IteratorMethod)]
                private IEnumerable<int> ListT(List<int> collection)
                {
                    foreach (var item in collection)
                    {
                        yield return item;
                    }
                }

                [Fact]
                public void ListT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        int arg = 7;

                        foreach (var result in ListT(new List<int>() { arg }))
                        {
                            Logger.Trace(result.ToString());
                        }

                        Assert.Equal($"OnEntry [{arg}] OnYield {arg} {arg} OnResume OnSuccess OnExit ", appender.ToString());
                    }
                }

                #endregion

                #endregion
            }

            public class 引数の変更
            {
                private class ChangeArgAspect : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                        {
                            var argument = args.Arguments[argumentIndex] as string;
                            args.Arguments[argumentIndex] = argument.ToUpper();
                        }
                    }
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1)
                {
                    yield return arg1;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2)
                {
                    yield return arg1;
                    yield return arg2;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
                }

                [ChangeArgAspect]
                private IEnumerable<string> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
                    yield return arg9;
                }

                [Fact]
                public void 引数を大文字に変更_引数が1つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a").ToList();

                    Assert.Equal("A", result[0]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が2つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が3つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が4つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が5つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d", "e").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                    Assert.Equal("E", result[4]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が6つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d", "e", "f").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                    Assert.Equal("E", result[4]);
                    Assert.Equal("F", result[5]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が7つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                    Assert.Equal("E", result[4]);
                    Assert.Equal("F", result[5]);
                    Assert.Equal("G", result[6]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が8つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                    Assert.Equal("E", result[4]);
                    Assert.Equal("F", result[5]);
                    Assert.Equal("G", result[6]);
                    Assert.Equal("H", result[7]);
                }

                [Fact]
                public void 引数を大文字に変更_引数が9つ_正しくアスペクトが適用される()
                {
                    var result = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h", "i").ToList();

                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                    Assert.Equal("C", result[2]);
                    Assert.Equal("D", result[3]);
                    Assert.Equal("E", result[4]);
                    Assert.Equal("F", result[5]);
                    Assert.Equal("G", result[6]);
                    Assert.Equal("H", result[7]);
                    Assert.Equal("I", result[8]);
                }
            }

            public class 戻り値の変更
            {
                private class ChangeReturnValueAspect : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        var yieldValue = args.YieldValue as string;
                        args.YieldValue = yieldValue.ToUpper();
                    }
                }

                [ChangeReturnValueAspect]
                private IEnumerable<string> 戻り値を大文字に変更(string arg1)
                {
                    yield return arg1;
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var result = 戻り値を大文字に変更("a").ToList();
                    Assert.Equal("A", result[0]);
                }
            }
        }

        public class 非同期メソッド
        {
            public class 署名
            {
                #region 引数の個数

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数なし()
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が1つ(string arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が2つ(string arg1, string arg2)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が3つ(string arg1, string arg2, string arg3)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が4つ(string arg1, string arg2, string arg3, string arg4)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が5つ(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                        Logger.Trace(arg5);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が6つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                        Logger.Trace(arg5);
                        Logger.Trace(arg6);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が7つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                        Logger.Trace(arg5);
                        Logger.Trace(arg6);
                        Logger.Trace(arg7);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が8つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                        Logger.Trace(arg5);
                        Logger.Trace(arg6);
                        Logger.Trace(arg7);
                        Logger.Trace(arg8);
                    });
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task 引数が9つ(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg1);
                        Logger.Trace(arg2);
                        Logger.Trace(arg3);
                        Logger.Trace(arg4);
                        Logger.Trace(arg5);
                        Logger.Trace(arg6);
                        Logger.Trace(arg7);
                        Logger.Trace(arg8);
                        Logger.Trace(arg9);
                    });
                }

                [Fact]
                public void 引数なし_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数なし().Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry OnYield null OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が1つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が1つ("1").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" OnYield null 1 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が2つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が2つ("1", "2").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" OnYield null 1 2 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が3つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が3つ("1", "2", "3").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" OnYield null 1 2 3 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が4つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が4つ("1", "2", "3", "4").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" OnYield null 1 2 3 4 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が5つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が5つ("1", "2", "3", "4", "5").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" \"5\" OnYield null 1 2 3 4 5 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が6つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が6つ("1", "2", "3", "4", "5", "6").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" \"5\" \"6\" OnYield null 1 2 3 4 5 6 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が7つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が7つ("1", "2", "3", "4", "5", "6", "7").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" \"5\" \"6\" \"7\" OnYield null 1 2 3 4 5 6 7 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が8つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が8つ("1", "2", "3", "4", "5", "6", "7", "8").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" \"5\" \"6\" \"7\" \"8\" OnYield null 1 2 3 4 5 6 7 8 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                [Fact]
                public void 引数が9つ_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        引数が9つ("1", "2", "3", "4", "5", "6", "7", "8", "9").Wait();

                        Logger.Trace("A");

                        Assert.Equal($"OnEntry \"1\" \"2\" \"3\" \"4\" \"5\" \"6\" \"7\" \"8\" \"9\" OnYield null 1 2 3 4 5 6 7 8 9 OnResume OnSuccess OnExit A ", appender.ToString());
                    }
                }

                #endregion

                #region 引数と戻り値の型

                #region int

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<int> @int(int arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void int_正しくアスペクトが適用される(int arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @int(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region sbyte

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<sbyte> @sbyte(sbyte arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void sbyte_正しくアスペクトが適用される(sbyte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @sbyte(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region short

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<short> @short(short arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void short_正しくアスペクトが適用される(short arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @short(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region long

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<long> @long(long arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void long_正しくアスペクトが適用される(long arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @long(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region uint

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<uint> @uint(uint arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void uint_正しくアスペクトが適用される(uint arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @uint(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region byte

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<byte> @byte(byte arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void byte_正しくアスペクトが適用される(byte arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @byte(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region ushort

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<ushort> @ushort(ushort arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void ushort_正しくアスペクトが適用される(ushort arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @ushort(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region ulong

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<ulong> @ulong(ulong arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
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
                public void ulong_正しくアスペクトが適用される(ulong arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @ulong(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region double

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<double> @double(double arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void double_正しくアスペクトが適用される(double arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @double(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region float

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<float> @float(float arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void float_正しくアスペクトが適用される(float arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @float(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region decimal

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<decimal> @decimal(decimal arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Theory]
                [InlineData(0.0)]
                [InlineData(0.5)]
                [InlineData(1.0)]
                [InlineData(100.0)]
                [InlineData(-0.5)]
                [InlineData(-1.0)]
                [InlineData(-100.0)]
                public void decimal_正しくアスペクトが適用される(decimal arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @decimal(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region char

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<char> @char(char arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Theory]
                [InlineData('a')]
                [InlineData('あ')]
                public void char_正しくアスペクトが適用される(char arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @char(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry '{arg}' OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region string

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<string> @string(string arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Theory]
                [InlineData("a")]
                [InlineData("あ")]
                public void string_正しくアスペクトが適用される(string arg)
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();

                        var task = @string(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry \"{arg}\" OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region class

                public class Class
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<Class> @class(Class arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Fact]
                public void class_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Class() { Property = "a" };

                        var task = @class(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region struct

                public struct Struct
                {
                    public string Property { get; set; }

                    public override string ToString() => Property;
                }

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<Struct> @struct(Struct arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(arg.ToString());
                    });

                    return arg;
                }

                [Fact]
                public void struct_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new Struct() { Property = "a" };

                        var task = @struct(arg);
                        task.Wait();
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry {arg} OnYield null {arg} OnResume OnSuccess OnExit {arg} ", appender.ToString());
                    }
                }

                #endregion

                #region IEnumerable

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<IEnumerable> IEnumerable(IEnumerable arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));
                    });

                    return arg;
                }

                [Fact]
                public void IEnumerable_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 7 };

                        var task = IEnumerable(arg);
                        task.Wait();
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));


                        Assert.Equal($"OnEntry [7] OnYield null [7] OnResume OnSuccess OnExit [7] ", appender.ToString());
                    }
                }

                #endregion

                #region IEnumerableT

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<IEnumerable<int>> IEnumerableT(IEnumerable<int> arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));
                    });

                    return arg;
                }

                [Fact]
                public void IEnumerableT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 7 };

                        var task = IEnumerableT(arg);
                        task.Wait();
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));

                        Assert.Equal($"OnEntry [7] OnYield null [7] OnResume OnSuccess OnExit [7] ", appender.ToString());
                    }
                }

                #endregion

                #region ListT

                [OnMethodBoundaryAspectLogger(MethodType.AsyncMethod)]
                private async Task<List<int>> ListT(List<int> arg)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));
                    });

                    return arg;
                }

                [Fact]
                public void ListT_正しくアスペクトが適用される()
                {
                    lock (LockObject)
                    {
                        var appender = CreateAppender();
                        var arg = new List<int>() { 7 };

                        var task = ListT(arg);
                        task.Wait();
                        Logger.Trace(Asserts.ArgumentFormatter.Format(arg));

                        Assert.Equal($"OnEntry [7] OnYield null [7] OnResume OnSuccess OnExit [7] ", appender.ToString());
                    }
                }

                #endregion

                #endregion
            }
        }
    }
}
