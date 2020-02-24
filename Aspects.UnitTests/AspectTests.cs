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
    }
}
