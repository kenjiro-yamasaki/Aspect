using SoftCube.Log;
using System;
using Xunit;
using static SoftCube.Aspects.Constants;

namespace SoftCube.Aspects
{
    public partial class OnMethodBoundaryAspectTests
    {
        public class 制御文
        {
            public class If文_戻り値あり
            {
                [TestAspect]
                private bool If(bool condition)
                {
                    if (condition)
                    {
                        Logger.Trace("A");
                        return true;
                    }

                    Logger.Trace("B");
                    return false;
                }

                [TestAspect]
                private bool IfElse(bool condition)
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

                [TestAspect]
                private (bool, bool) NestIfElse(bool condition0, bool condition1)
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

                [Theory]
                [InlineData(true, "A")]
                [InlineData(false, "B")]
                public void If_正しくコードが注入される(bool condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = If(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [Theory]
                [InlineData(true, "A")]
                [InlineData(false, "B")]
                public void IfElse_正しくコードが注入される(bool condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = IfElse(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [Theory]
                [InlineData(true, true, "A")]
                [InlineData(true, false, "B")]
                [InlineData(false, true, "C")]
                [InlineData(false, false, "D")]
                public void NestIf_正しくコードが注入される(bool condition0, bool condition1, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = NestIfElse(condition0, condition1);

                        Assert.Equal($"OnEntry {condition0} {condition1} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }
            }

            public class If文_戻り値なし
            {
                [TestAspect]
                private void If(bool condition)
                {
                    if (condition)
                    {
                        Logger.Trace("A");
                        return;
                    }

                    Logger.Trace("B");
                }

                [TestAspect]
                private void IfElse(bool condition)
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

                [TestAspect]
                private void NestIfElse(bool condition0, bool condition1)
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
                public void If_正しくコードが注入される(bool condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        If(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Theory]
                [InlineData(true, "A")]
                [InlineData(false, "B")]
                public void IfElse_正しくコードが注入される(bool condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        IfElse(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Theory]
                [InlineData(true, true, "A")]
                [InlineData(true, false, "B")]
                [InlineData(false, true, "C")]
                [InlineData(false, false, "D")]
                public void NestIf_正しくコードが注入される(bool condition0, bool condition1, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        NestIfElse(condition0, condition1);

                        Assert.Equal($"OnEntry {condition0} {condition1} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }
            }

            public class Switch_戻り値なし
            {
                public enum Enum
                {
                    A,
                    B,
                    C,
                }

                [TestAspect]
                private void Break(Enum condition)
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
                public void Break_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        Break(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private void Return(Enum condition)
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
                public void Return_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        Return(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private void BreakWithDefaultThrow(Enum condition)
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
                public void BreakWithDefaultThrow_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        BreakWithDefaultThrow(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void BreakWithDefaultThrow_default_正しくコードが注入される()
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();
                        var condition = (Enum)3;
                        var log = "D";

                        var ex = Record.Exception(() => BreakWithDefaultThrow(condition));

                        Assert.IsType<NotSupportedException>(ex);
                        Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private void ReturnWithDefaultThrow(Enum condition)
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
                public void ReturnWithDefaultThrow_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        ReturnWithDefaultThrow(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess null OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void ReturnWithDefaultThrow_default_正しくコードが注入される()
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();
                        var condition = (Enum)3;
                        var log = "D";

                        var ex = Record.Exception(() => BreakWithDefaultThrow(condition));

                        Assert.IsType<NotSupportedException>(ex);
                        Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                    }
                }
            }

            public class Switch_戻り値あり
            {
                public enum Enum
                {
                    A,
                    B,
                    C,
                }

                [TestAspect]
                private Enum Break(Enum condition)
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
                public void Break_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = Break(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private Enum Return(Enum condition)
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
                public void Return_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = Return(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private Enum BreakWithDefaultThrow(Enum condition)
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
                public void BreakWithDefaultThrow_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = BreakWithDefaultThrow(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void BreakWithDefaultThrow_default_正しくコードが注入される()
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();
                        var condition = (Enum)3;
                        var log = "D";

                        var ex = Record.Exception(() => BreakWithDefaultThrow(condition));

                        Assert.IsType<NotSupportedException>(ex);
                        Assert.Equal($"OnEntry {condition} {log} OnException {log} OnExit ", appender.ToString());
                    }
                }

                [TestAspect]
                private Enum ReturnWithDefaultThrow(Enum condition)
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
                public void ReturnWithDefaultThrow_正しくコードが注入される(Enum condition, string log)
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();

                        var result = ReturnWithDefaultThrow(condition);

                        Assert.Equal($"OnEntry {condition} {log} OnSuccess {result} OnExit ", appender.ToString());
                    }
                }

                [Fact]
                public void ReturnWithDefaultThrow_default_正しくコードが注入される()
                {
                    lock (Lock)
                    {
                        var appender = CreateAppender();
                        var condition = (Enum)3;
                        var log = "D";

                        var ex = Record.Exception(() => BreakWithDefaultThrow(condition));

                        Assert.IsType<NotSupportedException>(ex);
                        Assert.Equal($"OnEntry {condition} {log} OnException D OnExit ", appender.ToString());
                    }
                }
            }

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
