using SoftCube.Logging;
using System;
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
            Logger.Add(appender);

            return appender;
        }

        #endregion

        #region コンストラクター引数

        #region bool

        private class BoolArgLogger : OnMethodBoundaryAspect
        {
            public bool Arg { get; }
            public BoolArgLogger(bool arg) => Arg = arg;
            public override void OnEntry(MethodExecutionArgs args) => Logger.Trace(Arg.ToString());
        }

        [BoolArgLogger(true)]
        private void TrueArg() {}

        [BoolArgLogger(false)]
        private void FalseArg() {}

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
        private void SByteArg() {}

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
        private void ShortArg() {}

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
        private void IntArg() {}

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
        private void LongArg() {}

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
        private void ByteArg() {}

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
        private void UShortArg() {}

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
        private void UIntArg() {}

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
        private void ULongArg() {}

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
        private void SingleArg() {}

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
        private void DoubleArg() {}

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
        private void CharArg() {}

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
        private void StringArg() {}

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
        private void TypeArg() {}

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
        private void EnumArg() {}

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
        private void TrueProperty() {}

        [BoolPropertyLogger(Property = false)]
        private void FalseProperty() {}

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
        private void SByteProperty() {}

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
        private void ShortProperty() {}

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
        private void IntProperty() {}

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
        private void LongProperty() {}

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
        private void ByteProperty() {}

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
        private void UShortProperty() {}

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
        private void UIntProperty() {}

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
        private void ULongProperty() {}

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
        private void SingleProperty() {}

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
        private void DoubleProperty() {}

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
        private void CharProperty() {}

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
        private void StringProperty() {}

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
        private void TypeProperty() {}

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
        private void EnumProperty() {}

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
