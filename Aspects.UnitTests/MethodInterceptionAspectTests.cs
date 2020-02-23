using SoftCube.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftCube.Aspects
{
    public class MethodInterceptionAspectTests
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

        public class 通常のメソッド
        {
            public class インスタンス
            {
                private static object Instance;

                private class OnEntrySpy : MethodInterceptionAspect
                {
                    public override void OnInvoke(MethodInterceptionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private void メソッド()
                {
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド();

                    Assert.Same(this, Instance);
                }
            }

            public class Proceed
            {
                public class イベントハンドラーの呼びだし順序
                {
                    private StringBuilder StringBuilder = new StringBuilder();

                    private class EventSpy : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            var instance = args.Instance as イベントハンドラーの呼びだし順序;
                            var stringBuilder = instance.StringBuilder;

                            stringBuilder.Append("OnEntry ");
                            try
                            {
                                args.Proceed();
                                stringBuilder.Append("OnSuccess ");
                            }
                            catch (Exception)
                            {
                                stringBuilder.Append("OnException ");
                                throw;
                            }
                            finally
                            {
                                stringBuilder.Append("OnExit ");
                            }
                        }
                    }

                    [EventSpy]
                    private void 正常()
                    {
                        StringBuilder.Append("1 ");
                    }

                    [EventSpy]
                    private void 例外()
                    {
                        StringBuilder.Append("1 ");
                        throw new InvalidOperationException();
                    }

                    [Fact]
                    public void 正常_イベントハンドラーが正しくよばれる()
                    {
                        正常();

                        Assert.Equal($"OnEntry 1 OnSuccess OnExit ", StringBuilder.ToString());
                    }

                    [Fact]
                    public void 例外_イベントハンドラーが正しくよばれる()
                    {
                        var exception = Record.Exception(() => 例外());

                        Assert.IsType<InvalidOperationException>(exception);
                        Assert.Equal($"OnEntry 1 OnException OnExit ", StringBuilder.ToString());
                    }
                }

                public class 引数
                {
                    private class ChangeArguments : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                            {
                                switch (args.Arguments[argumentIndex])
                                {
                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case string argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                                        break;

                                    case null:
                                        break;

                                    default:
                                        throw new NotSupportedException();
                                }
                            }

                            args.Proceed();
                        }
                    }

                    #region 引数1つ

                    [ChangeArguments]
                    private int 引数を変更(int arg0)
                    {
                        return arg0;
                    }

                    [Fact]
                    public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;

                        var result = 引数を変更(arg0);

                        Assert.Equal(1, result);
                    }

                    #endregion

                    #region 引数2つ

                    [ChangeArguments]
                    private (int, string) 引数を変更(int arg0, string arg1)
                    {
                        return (arg0, arg1);
                    }

                    [Fact]
                    public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";

                        var (result0, result1) = 引数を変更(arg0, arg1);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                    }

                    #endregion

                    #region 引数3つ

                    [ChangeArguments]
                    private (int, string, int) 引数を変更(int arg0, string arg1, in int arg2)
                    {
                        return (arg0, arg1, arg2);
                    }

                    [Fact]
                    public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;

                        var (result0, result1, result2) = 引数を変更(arg0, arg1, arg2);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                    }

                    #endregion

                    #region 引数4つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                    }

                    #endregion

                    #region 引数5つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                    }

                    #endregion

                    #region 引数6つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数6つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                    }

                    #endregion

                    #region 引数7つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6)
                    {
                        arg6 = 7;
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数7つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                    }

                    #endregion

                    #region 引数8つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数8つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                    }

                    #endregion

                    #region 引数9つ

                    [ChangeArguments]
                    private (int, string, int, string, int) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7, int arg8)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3, arg8);
                    }

                    [Fact]
                    public void 引数を変更_引数9つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;
                        int arg8 = 8;

                        var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal(9, result8);
                    }

                    #endregion
                }

                public class 戻り値
                {
                    #region 参照型

                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.Proceed();
                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    [ToUpper]
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

                    #endregion

                    #region 値型

                    private class Increment : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.Proceed();
                            var returnValue = (int)args.ReturnValue;
                            args.ReturnValue = returnValue + 1;
                        }
                    }

                    [Increment]
                    private int 戻り値をインクリメント(int arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値をインクリメント_正しくアスペクトが適用される()
                    {
                        var result = 戻り値をインクリメント(1);
                        Assert.Equal(2, result);
                    }

                    #endregion
                }

                public class 仮想関数
                {
                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.Proceed();
                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    private abstract class Base
                    {
                        public abstract string 戻り値を大文字に変更(string arg);
                    }

                    private class Derived : Base
                    {
                        [ToUpper]
                        public override string 戻り値を大文字に変更(string arg)
                        {
                            return arg;
                        }
                    }

                    [Fact]
                    public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                    {
                        var result = new Derived().戻り値を大文字に変更("a");
                        Assert.Equal("A", result);
                    }
                }
            }

            public class Invoke
            {
                public class イベントハンドラーの呼びだし順序
                {
                    private StringBuilder StringBuilder = new StringBuilder();

                    private class EventSpy : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            var instance = args.Instance as イベントハンドラーの呼びだし順序;
                            var stringBuilder = instance.StringBuilder;

                            stringBuilder.Append("OnEntry ");
                            try
                            {
                                args.ReturnValue = args.Invoke(args.Arguments);
                                stringBuilder.Append("OnSuccess ");
                            }
                            catch (Exception)
                            {
                                stringBuilder.Append("OnException ");
                                throw;
                            }
                            finally
                            {
                                stringBuilder.Append("OnExit ");
                            }
                        }
                    }

                    [EventSpy]
                    private void 正常()
                    {
                        StringBuilder.Append("1 ");
                    }

                    [EventSpy]
                    private void 例外()
                    {
                        StringBuilder.Append("1 ");
                        throw new InvalidOperationException();
                    }

                    [Fact]
                    public void 正常_イベントハンドラーが正しくよばれる()
                    {
                        正常();

                        Assert.Equal($"OnEntry 1 OnSuccess OnExit ", StringBuilder.ToString());
                    }

                    [Fact]
                    public void 例外_イベントハンドラーが正しくよばれる()
                    {
                        var exception = Record.Exception(() => 例外());

                        Assert.IsType<InvalidOperationException>(exception);
                        Assert.Equal($"OnEntry 1 OnException OnExit ", StringBuilder.ToString());
                    }
                }

                public class 引数
                {
                    private class ChangeArguments : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                            {
                                switch (args.Arguments[argumentIndex])
                                {
                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case string argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                                        break;

                                    case null:
                                        break;

                                    default:
                                        throw new NotSupportedException();
                                }
                            }

                            args.ReturnValue = args.Invoke(args.Arguments);
                        }
                    }

                    #region 引数1つ

                    [ChangeArguments]
                    private int 引数を変更(int arg0)
                    {
                        return arg0;
                    }

                    [Fact]
                    public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;

                        var result = 引数を変更(arg0);

                        Assert.Equal(1, result);
                    }

                    #endregion

                    #region 引数2つ

                    [ChangeArguments]
                    private (int, string) 引数を変更(int arg0, string arg1)
                    {
                        return (arg0, arg1);
                    }

                    [Fact]
                    public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";

                        var (result0, result1) = 引数を変更(arg0, arg1);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                    }

                    #endregion

                    #region 引数3つ

                    [ChangeArguments]
                    private (int, string, int) 引数を変更(int arg0, string arg1, in int arg2)
                    {
                        return (arg0, arg1, arg2);
                    }

                    [Fact]
                    public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;

                        var (result0, result1, result2) = 引数を変更(arg0, arg1, arg2);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                    }

                    #endregion

                    #region 引数4つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                    }

                    #endregion

                    #region 引数5つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                    }

                    #endregion

                    #region 引数6つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数6つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                    }

                    #endregion

                    #region 引数7つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6)
                    {
                        arg6 = 7;
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数7つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                    }

                    #endregion

                    #region 引数8つ

                    [ChangeArguments]
                    private (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数8つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                    }

                    #endregion

                    #region 引数9つ

                    [ChangeArguments]
                    private (int, string, int, string, int) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7, int arg8)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3, arg8);
                    }

                    [Fact]
                    public void 引数を変更_引数9つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;
                        int arg8 = 8;

                        var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal(9, result8);
                    }

                    #endregion
                }

                public class 戻り値
                {
                    #region 参照型

                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.ReturnValue = args.Invoke(args.Arguments);

                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    [ToUpper]
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

                    #endregion

                    #region 値型

                    private class Increment : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.ReturnValue = args.Invoke(args.Arguments);

                            var returnValue = (int)args.ReturnValue;
                            args.ReturnValue = returnValue + 1;
                        }
                    }

                    [Increment]
                    private int 戻り値をインクリメント(int arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値をインクリメント_正しくアスペクトが適用される()
                    {
                        var result = 戻り値をインクリメント(1);
                        Assert.Equal(2, result);
                    }

                    #endregion
                }

                public class 仮想関数
                {
                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.ReturnValue = args.Invoke(args.Arguments);

                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    private abstract class Base
                    {
                        public abstract string 戻り値を大文字に変更(string arg);
                    }

                    private class Derived : Base
                    {
                        [ToUpper]
                        public override string 戻り値を大文字に変更(string arg)
                        {
                            return arg;
                        }
                    }

                    [Fact]
                    public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                    {
                        var result = new Derived().戻り値を大文字に変更("a");
                        Assert.Equal("A", result);
                    }
                }
            }
        }

        public class 静的メソッド
        {
            public class インスタンス
            {
                private static object Instance;

                private class OnEntrySpy : MethodInterceptionAspect
                {
                    public override void OnInvoke(MethodInterceptionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private static void メソッド()
                {
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド();

                    Assert.Null(Instance);
                }
            }

            public class Proceed
            {
                public class イベントハンドラーの呼びだし順序
                {
                    private static StringBuilder StringBuilder = new StringBuilder();

                    private class EventSpy : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            StringBuilder.Append("OnEntry ");
                            try
                            {
                                args.Proceed();
                                StringBuilder.Append("OnSuccess ");
                            }
                            catch (Exception)
                            {
                                StringBuilder.Append("OnException ");
                                throw;
                            }
                            finally
                            {
                                StringBuilder.Append("OnExit ");
                            }
                        }
                    }

                    [EventSpy]
                    private static void 正常()
                    {
                        StringBuilder.Append("1 ");
                    }

                    [EventSpy]
                    private static void 例外()
                    {
                        StringBuilder.Append("1 ");
                        throw new InvalidOperationException();
                    }

                    [Fact]
                    public void 正常_イベントハンドラーが正しくよばれる()
                    {
                        StringBuilder.Clear();

                        正常();

                        Assert.Equal($"OnEntry 1 OnSuccess OnExit ", StringBuilder.ToString());
                    }

                    [Fact]
                    public void 例外_イベントハンドラーが正しくよばれる()
                    {
                        StringBuilder.Clear();

                        var exception = Record.Exception(() => 例外());

                        Assert.IsType<InvalidOperationException>(exception);
                        Assert.Equal($"OnEntry 1 OnException OnExit ", StringBuilder.ToString());
                    }
                }

                public class 引数
                {
                    private class ChangeArguments : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                            {
                                switch (args.Arguments[argumentIndex])
                                {
                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case string argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                                        break;

                                    case null:
                                        break;

                                    default:
                                        throw new NotSupportedException();
                                }
                            }

                            args.Proceed();
                        }
                    }

                    #region 引数1つ

                    [ChangeArguments]
                    private static int 引数を変更(int arg0)
                    {
                        return arg0;
                    }

                    [Fact]
                    public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;

                        var result = 引数を変更(arg0);

                        Assert.Equal(1, result);
                    }

                    #endregion

                    #region 引数2つ

                    [ChangeArguments]
                    private static (int, string) 引数を変更(int arg0, string arg1)
                    {
                        return (arg0, arg1);
                    }

                    [Fact]
                    public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";

                        var (result0, result1) = 引数を変更(arg0, arg1);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                    }

                    #endregion

                    #region 引数3つ

                    [ChangeArguments]
                    private static (int, string, int) 引数を変更(int arg0, string arg1, in int arg2)
                    {
                        return (arg0, arg1, arg2);
                    }

                    [Fact]
                    public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;

                        var (result0, result1, result2) = 引数を変更(arg0, arg1, arg2);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                    }

                    #endregion

                    #region 引数4つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                    }

                    #endregion

                    #region 引数5つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                    }

                    #endregion

                    #region 引数6つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数6つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                    }

                    #endregion

                    #region 引数7つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6)
                    {
                        arg6 = 7;
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数7つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                    }

                    #endregion

                    #region 引数8つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数8つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                    }

                    #endregion

                    #region 引数9つ

                    [ChangeArguments]
                    private static (int, string, int, string, int) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7, int arg8)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3, arg8);
                    }

                    [Fact]
                    public void 引数を変更_引数9つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;
                        int arg8 = 8;

                        var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal(9, result8);
                    }

                    #endregion
                }

                public class 戻り値
                {
                    #region 参照型

                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.Proceed();
                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    [ToUpper]
                    private static string 戻り値を大文字に変更(string arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                    {
                        var result = 戻り値を大文字に変更("a");

                        Assert.Equal("A", result);
                    }

                    #endregion

                    #region 値型

                    private class Increment : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.Proceed();
                            var returnValue = (int)args.ReturnValue;
                            args.ReturnValue = returnValue + 1;
                        }
                    }

                    [Increment]
                    private static int 戻り値をインクリメント(int arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値をインクリメント_正しくアスペクトが適用される()
                    {
                        var result = 戻り値をインクリメント(1);
                        Assert.Equal(2, result);
                    }

                    #endregion
                }
            }

            public class Invoke
            {
                public class イベントハンドラーの呼びだし順序
                {
                    private static StringBuilder StringBuilder = new StringBuilder();

                    private class EventSpy : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            StringBuilder.Append("OnEntry ");
                            try
                            {
                                args.ReturnValue = args.Invoke(args.Arguments);
                                StringBuilder.Append("OnSuccess ");
                            }
                            catch (Exception)
                            {
                                StringBuilder.Append("OnException ");
                                throw;
                            }
                            finally
                            {
                                StringBuilder.Append("OnExit ");
                            }
                        }
                    }

                    [EventSpy]
                    private static void 正常()
                    {
                        StringBuilder.Append("1 ");
                    }

                    [EventSpy]
                    private static void 例外()
                    {
                        StringBuilder.Append("1 ");
                        throw new InvalidOperationException();
                    }

                    [Fact]
                    public void 正常_イベントハンドラーが正しくよばれる()
                    {
                        StringBuilder.Clear();

                        正常();

                        Assert.Equal($"OnEntry 1 OnSuccess OnExit ", StringBuilder.ToString());
                    }

                    [Fact]
                    public void 例外_イベントハンドラーが正しくよばれる()
                    {
                        StringBuilder.Clear();

                        var exception = Record.Exception(() => 例外());

                        Assert.IsType<InvalidOperationException>(exception);
                        Assert.Equal($"OnEntry 1 OnException OnExit ", StringBuilder.ToString());
                    }
                }

                public class 引数
                {
                    private class ChangeArguments : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                            {
                                switch (args.Arguments[argumentIndex])
                                {
                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case string argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                                        break;

                                    case null:
                                        break;

                                    default:
                                        throw new NotSupportedException();
                                }
                            }

                            args.ReturnValue = args.Invoke(args.Arguments);
                        }
                    }

                    #region 引数1つ

                    [ChangeArguments]
                    private static int 引数を変更(int arg0)
                    {
                        return arg0;
                    }

                    [Fact]
                    public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;

                        var result = 引数を変更(arg0);

                        Assert.Equal(1, result);
                    }

                    #endregion

                    #region 引数2つ

                    [ChangeArguments]
                    private static (int, string) 引数を変更(int arg0, string arg1)
                    {
                        return (arg0, arg1);
                    }

                    [Fact]
                    public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";

                        var (result0, result1) = 引数を変更(arg0, arg1);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                    }

                    #endregion

                    #region 引数3つ

                    [ChangeArguments]
                    private static (int, string, int) 引数を変更(int arg0, string arg1, in int arg2)
                    {
                        return (arg0, arg1, arg2);
                    }

                    [Fact]
                    public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;

                        var (result0, result1, result2) = 引数を変更(arg0, arg1, arg2);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                    }

                    #endregion

                    #region 引数4つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                    }

                    #endregion

                    #region 引数5つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                    }

                    #endregion

                    #region 引数6つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5)
                    {
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数6つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                    }

                    #endregion

                    #region 引数7つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6)
                    {
                        arg6 = 7;
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数7つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                    }

                    #endregion

                    #region 引数8つ

                    [ChangeArguments]
                    private static (int, string, int, string) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3);
                    }

                    [Fact]
                    public void 引数を変更_引数8つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;

                        var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                    }

                    #endregion

                    #region 引数9つ

                    [ChangeArguments]
                    private static (int, string, int, string, int) 引数を変更(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7, int arg8)
                    {
                        arg6 = 7;
                        arg7 = "8";
                        return (arg0, arg1, arg2, arg3, arg8);
                    }

                    [Fact]
                    public void 引数を変更_引数9つ_正しくアスペクトが適用される()
                    {
                        var arg0 = 0;
                        var arg1 = "1";
                        var arg2 = 2;
                        var arg3 = "3";
                        var arg4 = 4;
                        var arg5 = "5";
                        int arg6;
                        string arg7;
                        int arg8 = 8;

                        var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal(9, result8);
                    }

                    #endregion
                }

                public class 戻り値
                {
                    #region 参照型

                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.ReturnValue = args.Invoke(args.Arguments);

                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    [ToUpper]
                    private static string 戻り値を大文字に変更(string arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                    {
                        var result = 戻り値を大文字に変更("a");

                        Assert.Equal("A", result);
                    }

                    #endregion

                    #region 値型

                    private class Increment : MethodInterceptionAspect
                    {
                        public override void OnInvoke(MethodInterceptionArgs args)
                        {
                            args.ReturnValue = args.Invoke(args.Arguments);

                            var returnValue = (int)args.ReturnValue;
                            args.ReturnValue = returnValue + 1;
                        }
                    }

                    [Increment]
                    private static int 戻り値をインクリメント(int arg1)
                    {
                        return arg1;
                    }

                    [Fact]
                    public void 戻り値をインクリメント_正しくアスペクトが適用される()
                    {
                        var result = 戻り値をインクリメント(1);
                        Assert.Equal(2, result);
                    }

                    #endregion
                }
            }
        }

        public class 非同期メソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                [Flags]
                private enum EventLoggerFlags
                {
                    None         = 0,
                    ProceedAsync = 1 << 0,
                    InvokeAsync  = 1 << 1,
                    Rethrow      = 1 << 2,
                }

                private class EventLogger : MethodInterceptionAspect
                {
                    private EventLoggerFlags Flags { get; }

                    public EventLogger(EventLoggerFlags flags)
                    {
                        Flags = flags;
                    }

                    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                    {
                        Logger.Trace("OnEntry");
                        try
                        {
                            if ((Flags & EventLoggerFlags.ProceedAsync) == EventLoggerFlags.ProceedAsync)
                            {
                                await args.ProceedAsync();
                                Logger.Trace("OnSuccess");
                            }
                            if ((Flags & EventLoggerFlags.InvokeAsync) == EventLoggerFlags.ProceedAsync)
                            {
                                args.ReturnValue = args.InvokeAsync(args.Arguments);
                                await (args.ReturnValue as Task);
                                Logger.Trace("OnSuccess");
                            }
                        }
                        catch (Exception)
                        {
                            Logger.Trace("OnException");
                            if ((Flags & EventLoggerFlags.Rethrow) == EventLoggerFlags.Rethrow)
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            Logger.Trace("OnExit");
                        }
                    }
                }

                #region 戻り値なし

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task 戻り値なし()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");
                }

                [Fact]
                public void 戻り値なし_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 戻り値なし();
                    Logger.Trace("1");

                    task.Wait();
                    Logger.Trace("4");

                    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync | EventLoggerFlags.Rethrow)]
                private async Task 戻り値なし_例外を再送出する()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 戻り値なし_例外を再送出する_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 戻り値なし_例外を再送出する();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task 戻り値なし_例外を再送出しない()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 戻り値なし_例外を再送出しない_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 戻り値なし_例外を再送出しない();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    Assert.Null(exception);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                #endregion

                #region 値型を戻す

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task<int> 値型を戻す()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    return 4;
                }

                [Fact]
                public void 値型を戻す_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 値型を戻す();
                    Logger.Trace("1");

                    task.Wait();
                    var result = task.Result;
                    Logger.Trace(result.ToString());

                    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync | EventLoggerFlags.Rethrow)]
                private async Task<int> 値型を戻す_例外を再送出する()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 値型を戻す_例外を再送出する_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 値型を戻す_例外を再送出する();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task<int> 値型を戻す_例外を再送出しない()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 値型を戻す_例外を再送出しない_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 値型を戻す_例外を再送出しない();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    Assert.Null(exception);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                #endregion

                #region 値型を戻す

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task<string> 参照型を戻す()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    return "4";
                }

                [Fact]
                public void 参照型を戻す_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 参照型を戻す();
                    Logger.Trace("1");

                    task.Wait();
                    var result = task.Result;
                    Logger.Trace(result);

                    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync | EventLoggerFlags.Rethrow)]
                private async Task<string> 参照型を戻す_例外を再送出する()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 参照型を戻す_例外を再送出する_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 参照型を戻す_例外を再送出する();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                [EventLogger(EventLoggerFlags.ProceedAsync)]
                private async Task<string> 参照型を戻す_例外を再送出しない()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 参照型を戻す_例外を再送出しない_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task = 参照型を戻す_例外を再送出しない();
                    Logger.Trace("1");

                    var exception = Record.Exception(() => task.Wait());
                    Logger.Trace("4");

                    Assert.Null(exception);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", appender.ToString());
                }

                #endregion
            }

            public class 引数
            {
                private class ChangeArguments : MethodInterceptionAspect
                {
                    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                    {
                        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                        {
                            switch (args.Arguments[argumentIndex])
                            {
                                case int argument:
                                    args.Arguments[argumentIndex] = argument + 1;
                                    break;

                                case string argument:
                                    args.Arguments[argumentIndex] = (int.Parse(argument) + 1).ToString();
                                    break;

                                case null:
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }
                        }

                        await args.ProceedAsync();
                    }
                }

                #region 引数1つ

                [ChangeArguments]
                private async Task<int> 引数を変更(int arg0)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return arg0;
                }

                [Fact]
                public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;

                    var task = 引数を変更(arg0);
                    task.Wait();
                    var result = task.Result;

                    Assert.Equal(1, result);
                }

                #endregion

                #region 引数2つ

                [ChangeArguments]
                private async Task<(int, string)> 引数を変更(int arg0, string arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1);
                }

                [Fact]
                public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";

                    var task = 引数を変更(arg0, arg1);
                    task.Wait();
                    var (result0, result1) = task.Result;

                    Assert.Equal(1, result0);
                    Assert.Equal("2", result1);
                }

                #endregion

                #region 引数3つ

                [ChangeArguments]
                private async Task<(int, string, int)> 引数を変更(int arg0, string arg1, int arg2)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2);
                }

                [Fact]
                public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;

                    var task = 引数を変更(arg0, arg1, arg2);
                    task.Wait();
                    var (result0, result1, result2) = task.Result;

                    Assert.Equal(1, result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3, result2);
                }

                #endregion

                #region 引数4つ

                [ChangeArguments]
                private async Task<(int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3);
                }

                [Fact]
                public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";

                    var task = 引数を変更(arg0, arg1, arg2, arg3);
                    task.Wait();
                    var (result0, result1, result2, result3) = task.Result;

                    Assert.Equal(1, result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3, result2);
                    Assert.Equal("4", result3);
                }

                #endregion

                #region 引数5つ

                [ChangeArguments]
                private async Task<(int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3, arg4);
                }

                [Fact]
                public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;

                    var task = 引数を変更(arg0, arg1, arg2, arg3, arg4);
                    task.Wait();

                    var (result0, result1, result2, result3, result4) = task.Result;

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
                }

                #endregion

                #region 引数6つ

                [ChangeArguments]
                private async Task<(int, string, int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3, arg4, arg5);
                }

                [Fact]
                public void 引数を変更_引数6つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;
                    var arg5 = "5";

                    var task = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5);
                    task.Wait();

                    var (result0, result1, result2, result3, result4, result5) = task.Result;

                    Assert.Equal(1, result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3, result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5, result4);
                    Assert.Equal("6", result5);
                }

                #endregion

                #region 引数7つ

                [ChangeArguments]
                private async Task<(int, string, int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3, arg4, arg5, arg6);
                }

                [Fact]
                public void 引数を変更_引数7つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;
                    var arg5 = "5";
                    var arg6 = 6;

                    var task = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
                    task.Wait();
                    var (result0, result1, result2, result3, result4, result5, result6) = task.Result;

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
                    Assert.Equal("6", result5);
                    Assert.Equal(7,   result6);
                }

                #endregion

                #region 引数8つ

                [ChangeArguments]
                private async Task<(int, string, int, string, int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }

                [Fact]
                public void 引数を変更_引数8つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;
                    var arg5 = "5";
                    var arg6 = 6;
                    var arg7 = "7";

                    var task = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    task.Wait();
                    var (result0, result1, result2, result3, result4, result5, result6, result7) = task.Result;

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
                    Assert.Equal("6", result5);
                    Assert.Equal(7,   result6);
                    Assert.Equal("8", result7);
                }

                #endregion

                #region 引数9つ

                [ChangeArguments]
                private async Task<(int, string, int, string, int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7, int arg8)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }

                [Fact]
                public void 引数を変更_引数9つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;
                    var arg5 = "5";
                    var arg6 = 6;
                    var arg7 = "7";
                    var arg8 = 8;

                    var task = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    task.Wait();
                    var (result0, result1, result2, result3, result4, result5, result6, result7, result8) = task.Result;

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
                    Assert.Equal("6", result5);
                    Assert.Equal(7,   result6);
                    Assert.Equal("8", result7);
                    Assert.Equal(9,   result8);
                }

                #endregion
            }

            public class 戻り値
            {
                #region 参照型

                private class ToUpper : MethodInterceptionAspect
                {
                    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                    {
                        await args.ProceedAsync();

                        var returnValue = args.ReturnValue as string;
                        args.ReturnValue = returnValue.ToUpper();
                    }
                }

                [ToUpper]
                private async Task<string> 戻り値を大文字に変更(string arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return arg1;
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var task = 戻り値を大文字に変更("a");
                    task.Wait();
                    var result = task.Result;

                    Assert.Equal("A", result);
                }

                #endregion

                #region 値型

                private class Increment : MethodInterceptionAspect
                {
                    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                    {
                        await args.ProceedAsync();

                        var returnValue = (int)args.ReturnValue;
                        args.ReturnValue = returnValue + 1;
                    }
                }

                [Increment]
                private async Task<int> 戻り値をインクリメント(int arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return arg1;
                }

                [Fact]
                public void 戻り値をインクリメント_正しくアスペクトが適用される()
                {
                    var task = 戻り値をインクリメント(1);
                    task.Wait();
                    var result = task.Result;

                    Assert.Equal(2, result);
                }

                #endregion
            }

        }
    }
}
