using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftCube.Aspects
{
    public class AspectTests
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
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private void TrueArg() { }

                [BoolArgLogger(false)]
                private void FalseArg() { }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private void SByteArg() { }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private void ShortArg() { }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private void IntArg() { }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private void LongArg() { }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private void ByteArg() { }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private void UShortArg() { }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private void UIntArg() { }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private void ULongArg() { }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private void SingleArg() { }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private void DoubleArg() { }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private void CharArg() { }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private void StringArg() { }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private void TypeArg() { }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private void EnumArg() { }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private void TrueProperty() { }

                [BoolPropertyLogger(Property = false)]
                private void FalseProperty() { }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private void SByteProperty() { }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private void ShortProperty() { }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private void IntProperty() { }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private void LongProperty() { }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private void ByteProperty() { }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private void UShortProperty() { }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private void UIntProperty() { }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private void ULongProperty() { }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private void SingleProperty() { }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private void DoubleProperty() { }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private void CharProperty() { }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private void StringProperty() { }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private void TypeProperty() { }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private void EnumProperty() { }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }

        public class 静的メソッド
        {
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private static void TrueArg() { }

                [BoolArgLogger(false)]
                private static void FalseArg() { }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private static void SByteArg() { }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private static void ShortArg() { }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private static void IntArg() { }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private static void LongArg() { }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private static void ByteArg() { }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private static void UShortArg() { }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private static void UIntArg() { }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private static void ULongArg() { }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private static void SingleArg() { }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private static void DoubleArg() { }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private static void CharArg() { }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private static void StringArg() { }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private static void TypeArg() { }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private static void EnumArg() { }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private static void TrueProperty() { }

                [BoolPropertyLogger(Property = false)]
                private static void FalseProperty() { }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private static void SByteProperty() { }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private static void ShortProperty() { }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private static void IntProperty() { }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private static void LongProperty() { }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private static void ByteProperty() { }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private static void UShortProperty() { }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private static void UIntProperty() { }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private static void ULongProperty() { }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private static void SingleProperty() { }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private static void DoubleProperty() { }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private static void CharProperty() { }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private static void StringProperty() { }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private static void TypeProperty() { }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private static void EnumProperty() { }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }

        public class イテレーターメソッド
        {
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private IEnumerable<int> TrueArg() { yield break; }

                [BoolArgLogger(false)]
                private IEnumerable<int> FalseArg() { yield break; }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg().ToList();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg().ToList();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private IEnumerable<int> SByteArg() { yield break; }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private IEnumerable<int> ShortArg() { yield break; }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private IEnumerable<int> IntArg() { yield break; }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private IEnumerable<int> LongArg() { yield break; }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private IEnumerable<int> ByteArg() { yield break; }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg().ToList();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private IEnumerable<int> UShortArg() { yield break; }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg().ToList();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private IEnumerable<int> UIntArg() { yield break; }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg().ToList();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private IEnumerable<int> ULongArg() { yield break; }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg().ToList();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private IEnumerable<int> SingleArg() { yield break; }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private IEnumerable<int> DoubleArg() { yield break; }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private IEnumerable<int> CharArg() { yield break; }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private IEnumerable<int> StringArg() { yield break; }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private IEnumerable<int> TypeArg() { yield break; }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg().ToList();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private IEnumerable<int> EnumArg() { yield break; }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg().ToList();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private IEnumerable<int> TrueProperty() { yield break; }

                [BoolPropertyLogger(Property = false)]
                private IEnumerable<int> FalseProperty() { yield break; }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty().ToList();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty().ToList();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private IEnumerable<int> SByteProperty() { yield break; }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private IEnumerable<int> ShortProperty() { yield break; }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private IEnumerable<int> IntProperty() { yield break; }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private IEnumerable<int> LongProperty() { yield break; }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private IEnumerable<int> ByteProperty() { yield break; }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty().ToList();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private IEnumerable<int> UShortProperty() { yield break; }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty().ToList();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private IEnumerable<int> UIntProperty() { yield break; }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty().ToList();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private IEnumerable<int> ULongProperty() { yield break; }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty().ToList();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private IEnumerable<int> SingleProperty() { yield break; }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private IEnumerable<int> DoubleProperty() { yield break; }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private IEnumerable<int> CharProperty() { yield break; }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private IEnumerable<int> StringProperty() { yield break; }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private IEnumerable<int> TypeProperty() { yield break; }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty().ToList();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private IEnumerable<int> EnumProperty() { yield break; }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty().ToList();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }

        public class 静的イテレーターメソッド
        {
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private static IEnumerable<int> TrueArg() { yield break; }

                [BoolArgLogger(false)]
                private static IEnumerable<int> FalseArg() { yield break; }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg().ToList();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg().ToList();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private static IEnumerable<int> SByteArg() { yield break; }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private static IEnumerable<int> ShortArg() { yield break; }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private static IEnumerable<int> IntArg() { yield break; }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private static IEnumerable<int> LongArg() { yield break; }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private static IEnumerable<int> ByteArg() { yield break; }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg().ToList();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private static IEnumerable<int> UShortArg() { yield break; }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg().ToList();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private static IEnumerable<int> UIntArg() { yield break; }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg().ToList();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private static IEnumerable<int> ULongArg() { yield break; }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg().ToList();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private static IEnumerable<int> SingleArg() { yield break; }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private static IEnumerable<int> DoubleArg() { yield break; }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private static IEnumerable<int> CharArg() { yield break; }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private static IEnumerable<int> StringArg() { yield break; }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private static IEnumerable<int> TypeArg() { yield break; }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg().ToList();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private static IEnumerable<int> EnumArg() { yield break; }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg().ToList();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private static IEnumerable<int> TrueProperty() { yield break; }

                [BoolPropertyLogger(Property = false)]
                private static IEnumerable<int> FalseProperty() { yield break; }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty().ToList();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty().ToList();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private static IEnumerable<int> SByteProperty() { yield break; }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private static IEnumerable<int> ShortProperty() { yield break; }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private static IEnumerable<int> IntProperty() { yield break; }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private static IEnumerable<int> LongProperty() { yield break; }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty().ToList();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private static IEnumerable<int> ByteProperty() { yield break; }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty().ToList();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private static IEnumerable<int> UShortProperty() { yield break; }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty().ToList();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private static IEnumerable<int> UIntProperty() { yield break; }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty().ToList();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private static IEnumerable<int> ULongProperty() { yield break; }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty().ToList();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private static IEnumerable<int> SingleProperty() { yield break; }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private static IEnumerable<int> DoubleProperty() { yield break; }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty().ToList();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private static IEnumerable<int> CharProperty() { yield break; }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private static IEnumerable<int> StringProperty() { yield break; }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty().ToList();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private static IEnumerable<int> TypeProperty() { yield break; }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty().ToList();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private static IEnumerable<int> EnumProperty() { yield break; }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty().ToList();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }

        public class 非同期メソッド
        {
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private  async Task TrueArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [BoolArgLogger(false)]
                private  async Task FalseArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg().Wait();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg().Wait();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private  async Task SByteArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private  async Task ShortArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private  async Task IntArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private  async Task LongArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private  async Task ByteArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg().Wait();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private  async Task UShortArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg().Wait();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private  async Task UIntArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg().Wait();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private  async Task ULongArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg().Wait();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private  async Task SingleArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private  async Task DoubleArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private  async Task CharArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private  async Task StringArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private  async Task TypeArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg().Wait();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private  async Task EnumArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg().Wait();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private  async Task TrueProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [BoolPropertyLogger(Property = false)]
                private  async Task FalseProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty().Wait();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty().Wait();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private  async Task SByteProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private  async Task ShortProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private  async Task IntProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private  async Task LongProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private  async Task ByteProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty().Wait();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private  async Task UShortProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty().Wait();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private  async Task UIntProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty().Wait();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private  async Task ULongProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty().Wait();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private  async Task SingleProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private  async Task DoubleProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private  async Task CharProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private  async Task StringProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private  async Task TypeProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty().Wait();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private  async Task EnumProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty().Wait();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }

        public class 静的非同期メソッド
        {
            public class アスペクトの初期化
            {
                #region コンストラクター引数

                #region bool

                private class BoolArgLogger : OnMethodBoundaryAspect
                {
                    public bool Arg { get; }
                    public BoolArgLogger(bool arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [BoolArgLogger(true)]
                private static async Task TrueArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [BoolArgLogger(false)]
                private static async Task FalseArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TrueArgTest()
                {
                    var appender = CreateAppender();

                    TrueArg().Wait();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalseArgTest()
                {
                    var appender = CreateAppender();

                    FalseArg().Wait();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SByteArgLogger : OnMethodBoundaryAspect
                {
                    public sbyte Arg { get; }
                    public SByteArgLogger(sbyte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SByteArgLogger(-1)]
                private static async Task SByteArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SByteArgTest()
                {
                    var appender = CreateAppender();

                    SByteArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortArgLogger : OnMethodBoundaryAspect
                {
                    public short Arg { get; }
                    public ShortArgLogger(short arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ShortArgLogger(-1)]
                private static async Task ShortArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ShortArgTest()
                {
                    var appender = CreateAppender();

                    ShortArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntArgLogger : OnMethodBoundaryAspect
                {
                    public int Arg { get; }
                    public IntArgLogger(int arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [IntArgLogger(-1)]
                private static async Task IntArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void IntArgTest()
                {
                    var appender = CreateAppender();

                    IntArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongArgLogger : OnMethodBoundaryAspect
                {
                    public long Arg { get; }
                    public LongArgLogger(long arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [LongArgLogger(-1)]
                private static async Task LongArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void LongArgTest()
                {
                    var appender = CreateAppender();

                    LongArg().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class ByteArgLogger : OnMethodBoundaryAspect
                {
                    public byte Arg { get; }
                    public ByteArgLogger(byte arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ByteArgLogger(0xFF)]
                private static async Task ByteArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ByteArgTest()
                {
                    var appender = CreateAppender();

                    ByteArg().Wait();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortArgLogger : OnMethodBoundaryAspect
                {
                    public ushort Arg { get; }
                    public UShortArgLogger(ushort arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UShortArgLogger(0xFFFF)]
                private static async Task UShortArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UShortArgTest()
                {
                    var appender = CreateAppender();

                    UShortArg().Wait();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntArgLogger : OnMethodBoundaryAspect
                {
                    public uint Arg { get; }
                    public UIntArgLogger(uint arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [UIntArgLogger(0xFFFF_FFFF)]
                private static async Task UIntArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UIntArgTest()
                {
                    var appender = CreateAppender();

                    UIntArg().Wait();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongArgLogger : OnMethodBoundaryAspect
                {
                    public ulong Arg { get; }
                    public ULongArgLogger(ulong arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [ULongArgLogger(0xFFFF_FFFF_FFFF_FFFF)]
                private static async Task ULongArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ULongArgTest()
                {
                    var appender = CreateAppender();

                    ULongArg().Wait();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SingleArgLogger : OnMethodBoundaryAspect
                {
                    public float Arg { get; }
                    public SingleArgLogger(float arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [SingleArgLogger(1.5f)]
                private static async Task SingleArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SingleArgTest()
                {
                    var appender = CreateAppender();

                    SingleArg().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoubleArgLogger : OnMethodBoundaryAspect
                {
                    public double Arg { get; }
                    public DoubleArgLogger(double arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [DoubleArgLogger(1.5)]
                private static async Task DoubleArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void DoubleArgTest()
                {
                    var appender = CreateAppender();

                    DoubleArg().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharArgLogger : OnMethodBoundaryAspect
                {
                    public char Arg { get; }
                    public CharArgLogger(char arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [CharArgLogger('a')]
                private static async Task CharArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void CharArgTest()
                {
                    var appender = CreateAppender();

                    CharArg().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringArgLogger : OnMethodBoundaryAspect
                {
                    public string Arg { get; }
                    public StringArgLogger(string arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [StringArgLogger("a")]
                private static async Task StringArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void StringArgTest()
                {
                    var appender = CreateAppender();

                    StringArg().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region type

                private class TypeArgLogger : OnMethodBoundaryAspect
                {
                    public Type Arg { get; }
                    public TypeArgLogger(Type arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [TypeArgLogger(typeof(int))]
                private static async Task TypeArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TypeArgTest()
                {
                    var appender = CreateAppender();

                    TypeArg().Wait();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                public enum Enum
                {
                    Value,
                }

                private class EnumArgLogger : OnMethodBoundaryAspect
                {
                    public Enum Arg { get; }
                    public EnumArgLogger(Enum arg) => Arg = arg;
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
                }

                [EnumArgLogger(Enum.Value)]
                private static async Task EnumArg()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void EnumArgTest()
                {
                    var appender = CreateAppender();

                    EnumArg().Wait();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion

                #region プロパティ

                #region bool

                private class BoolPropertyLogger : OnMethodBoundaryAspect
                {
                    public bool Property { get; set; }
                    public BoolPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BoolPropertyLogger(Property = true)]
                private static async Task TrueProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [BoolPropertyLogger(Property = false)]
                private static async Task FalseProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TruePropertyTest()
                {
                    var appender = CreateAppender();

                    TrueProperty().Wait();

                    Assert.Equal("True ", appender.ToString());
                }

                [Fact]
                public void FalsePropertyTest()
                {
                    var appender = CreateAppender();

                    FalseProperty().Wait();

                    Assert.Equal("False ", appender.ToString());
                }

                #endregion

                #region sbyte

                private class SBytePropertyLogger : OnMethodBoundaryAspect
                {
                    public sbyte Property { get; set; }
                    public SBytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SBytePropertyLogger(Property = -1)]
                private static async Task SByteProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SBytePropertyTest()
                {
                    var appender = CreateAppender();

                    SByteProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region short

                private class ShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public short Property { get; set; }
                    public ShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ShortPropertyLogger(Property = -1)]
                private static async Task ShortProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ShortPropertyTest()
                {
                    var appender = CreateAppender();

                    ShortProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region int

                private class IntPropertyLogger : OnMethodBoundaryAspect
                {
                    public int Property { get; set; }
                    public IntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [IntPropertyLogger(Property = -1)]
                private static async Task IntProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void IntPropertyTest()
                {
                    var appender = CreateAppender();

                    IntProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region long

                private class LongPropertyLogger : OnMethodBoundaryAspect
                {
                    public long Property { get; set; }
                    public LongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [LongPropertyLogger(Property = -1)]
                private static async Task LongProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void LongPropertyTest()
                {
                    var appender = CreateAppender();

                    LongProperty().Wait();

                    Assert.Equal("-1 ", appender.ToString());
                }

                #endregion

                #region byte

                private class BytePropertyLogger : OnMethodBoundaryAspect
                {
                    public byte Property { get; set; }
                    public BytePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [BytePropertyLogger(Property = 0xFF)]
                private static async Task ByteProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void BytePropertyTest()
                {
                    var appender = CreateAppender();

                    ByteProperty().Wait();

                    Assert.Equal("255 ", appender.ToString());
                }

                #endregion

                #region ushort

                private class UShortPropertyLogger : OnMethodBoundaryAspect
                {
                    public ushort Property { get; set; }
                    public UShortPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UShortPropertyLogger(Property = 0xFFFF)]
                private static async Task UShortProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UShortPropertyTest()
                {
                    var appender = CreateAppender();

                    UShortProperty().Wait();

                    Assert.Equal("65535 ", appender.ToString());
                }

                #endregion

                #region uint

                private class UIntPropertyLogger : OnMethodBoundaryAspect
                {
                    public uint Property { get; set; }
                    public UIntPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [UIntPropertyLogger(Property = 0xFFFFFFFF)]
                private static async Task UIntProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void UIntPropertyTest()
                {
                    var appender = CreateAppender();

                    UIntProperty().Wait();

                    Assert.Equal("4294967295 ", appender.ToString());
                }

                #endregion

                #region ulong

                private class ULongPropertyLogger : OnMethodBoundaryAspect
                {
                    public ulong Property { get; set; }
                    public ULongPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [ULongPropertyLogger(Property = 0xFFFFFFFFFFFFFFFF)]
                private static async Task ULongProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void ULongPropertyTest()
                {
                    var appender = CreateAppender();

                    ULongProperty().Wait();

                    Assert.Equal("18446744073709551615 ", appender.ToString());
                }

                #endregion

                #region float

                private class SinglePropertyLogger : OnMethodBoundaryAspect
                {
                    public float Property { get; set; }
                    public SinglePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [SinglePropertyLogger(Property = 1.5f)]
                private static async Task SingleProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void SinglePropertyTest()
                {
                    var appender = CreateAppender();

                    SingleProperty().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region double

                private class DoublePropertyLogger : OnMethodBoundaryAspect
                {
                    public double Property { get; set; }
                    public DoublePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [DoublePropertyLogger(Property = 1.5)]
                private static async Task DoubleProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void DoublePropertyTest()
                {
                    var appender = CreateAppender();

                    DoubleProperty().Wait();

                    Assert.Equal("1.5 ", appender.ToString());
                }

                #endregion

                #region char

                private class CharPropertyLogger : OnMethodBoundaryAspect
                {
                    public char Property { get; set; }
                    public CharPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [CharPropertyLogger(Property = 'a')]
                private static async Task CharProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void CharPropertyTest()
                {
                    var appender = CreateAppender();

                    CharProperty().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region string

                private class StringPropertyLogger : OnMethodBoundaryAspect
                {
                    public string Property { get; set; }
                    public StringPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [StringPropertyLogger(Property = "a")]
                private static async Task StringProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void StringPropertyTest()
                {
                    var appender = CreateAppender();

                    StringProperty().Wait();

                    Assert.Equal("a ", appender.ToString());
                }

                #endregion

                #region Type

                private class TypePropertyLogger : OnMethodBoundaryAspect
                {
                    public Type Property { get; set; }
                    public TypePropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [TypePropertyLogger(Property = typeof(int))]
                private static async Task TypeProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void TypePropertyTest()
                {
                    var appender = CreateAppender();

                    TypeProperty().Wait();

                    Assert.Equal("System.Int32 ", appender.ToString());
                }

                #endregion

                #region enum

                private class EnumPropertyLogger : OnMethodBoundaryAspect
                {
                    public Enum Property { get; set; }
                    public EnumPropertyLogger() { }
                    public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Property.ToString());
                }

                [EnumPropertyLogger(Property = Enum.Value)]
                private static async Task EnumProperty()
                {
                    await Task.Run(() =>Thread.Sleep(10));
                }

                [Fact]
                public void EnumPropertyTest()
                {
                    var appender = CreateAppender();

                    EnumProperty().Wait();

                    Assert.Equal("Value ", appender.ToString());
                }

                #endregion

                #endregion
            }
        }
    }
}
