using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftCube.Aspects
{
    public class MethodInterceptionAspectTests
    {
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
                private readonly StringBuilder StringBuilder = new StringBuilder();

                private class EventLogger : MethodInterceptionAspect
                {
                    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                    {
                        var instance = args.Instance as イベントハンドラーの呼びだし順序;
                        var stringBuilder = instance.StringBuilder;

                        stringBuilder.Append("OnEntry ");
                        try
                        {
                            await args.ProceedAsync();
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

                [EventLogger]
                private async Task 正常()
                {
                    StringBuilder.Append("0 ");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        StringBuilder.Append("2 ");
                    });

                    StringBuilder.Append("3 ");
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var task = 正常();
                    StringBuilder.Append("1 ");

                    task.Wait();
                    StringBuilder.Append("4 ");

                    Assert.Equal($"OnEntry 0 1 2 3 OnSuccess OnExit 4 ", StringBuilder.ToString());
                }

                [EventLogger]
                private async Task 例外()
                {
                    StringBuilder.Append("0 ");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        StringBuilder.Append("2 ");
                    });

                    StringBuilder.Append("3 ");

                    throw new InvalidOperationException();
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var task = 例外();
                    StringBuilder.Append("1 ");

                    var exception = Record.Exception(() => task.Wait());
                    StringBuilder.Append("4 ");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 1 2 3 OnException OnExit 4 ", StringBuilder.ToString());
                }
            }
        }
    }
}
