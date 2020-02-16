using SoftCube.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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

        public class メソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private void 正常()
                {
                    Logger.Trace("A");
                }

                [EventLogger]
                private void 例外()
                {
                    Logger.Trace("A");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    正常();

                    Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var exception = Record.Exception(() => 例外());

                    Assert.IsType<InvalidOperationException>(exception);
                    Assert.Equal($"OnEntry A OnException OnExit ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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

                    Assert.Equal(1,   result0);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
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
                    var    arg0 = 0;
                    var    arg1 = "1";
                    var    arg2 = 2;
                    var    arg3 = "3";
                    var    arg4 = 4;
                    var    arg5 = "5";
                    int    arg6;
                    string arg7;

                    var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
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
                    var    arg0 = 0;
                    var    arg1 = "1";
                    var    arg2 = 2;
                    var    arg3 = "3";
                    var    arg4 = 4;
                    var    arg5 = "5";
                    int    arg6;
                    string arg7;
                    int    arg8 = 8;

                    var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
                    Assert.Equal("8", arg7);
                    Assert.Equal(9,   result8);
                }

                #endregion
            }

            public class 戻り値
            {
                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
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

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
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
                private class ToUpperAspect : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        args.ReturnValue = ((string)args.ReturnValue).ToUpper();
                    }
                }

                private abstract class Base
                {
                    public abstract string 戻り値を大文字に変更(string arg);
                }

                private class Derived : Base
                {
                    [ToUpperAspect]
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

        public class 静的メソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private static void 正常()
                {
                    Logger.Trace("A");
                }

                [EventLogger]
                private static void 例外()
                {
                    Logger.Trace("A");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    正常();

                    Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var exception = Record.Exception(() => 例外());

                    Assert.IsType<InvalidOperationException>(exception);
                    Assert.Equal($"OnEntry A OnException OnExit ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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

                    Assert.Equal(1,   result0);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
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
                    var    arg0 = 0;
                    var    arg1 = "1";
                    var    arg2 = 2;
                    var    arg3 = "3";
                    var    arg4 = 4;
                    var    arg5 = "5";
                    int    arg6;
                    string arg7;

                    var (result0, result1, result2, result3) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
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
                    var    arg0 = 0;
                    var    arg1 = "1";
                    var    arg2 = 2;
                    var    arg3 = "3";
                    var    arg4 = 4;
                    var    arg5 = "5";
                    int    arg6;
                    string arg7;
                    int    arg8 = 8;

                    var (result0, result1, result2, result3, result8) = 引数を変更(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7, arg8);

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   arg4);
                    Assert.Equal("6", arg5);
                    Assert.Equal(7,   arg6);
                    Assert.Equal("8", arg7);
                    Assert.Equal(9,   result8);
                }

                #endregion
            }

            public class 戻り値
            {
                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
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

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
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

        public class イテレーターメソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private IEnumerable<int> 正常()
                {
                    Logger.Trace("A");
                    yield return 0;
                    Logger.Trace("B");
                    yield return 1;
                    Logger.Trace("C");
                }

                [EventLogger]
                private IEnumerable<int> 例外()
                {
                    Logger.Trace("A");
                    yield return 0;
                    Logger.Trace("B");
                    yield return 1;
                    Logger.Trace("C");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    正常().ToList();

                    Assert.Equal($"OnEntry A OnYield OnResume B OnYield OnResume C OnSuccess OnExit ", appender.ToString());
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var exception = Record.Exception(() => 例外().ToList());

                    Assert.IsType<InvalidOperationException>(exception);
                    Assert.Equal($"OnEntry A OnYield OnResume B OnYield OnResume C OnException OnExit ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private IEnumerable<int> メソッド()
                {
                    yield return 0;
                    yield return 1;
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド().ToList();

                    Assert.Same(this, Instance);
                }
            }

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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
                    }
                }

                #region 引数1つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0)
                {
                    yield return arg0;
                }

                [Fact]
                public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;

                    var result = 引数を変更(arg0).ToList();

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                }

                #endregion

                #region 引数2つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1)
                {
                    yield return arg0;
                    yield return arg1;
                }

                [Fact]
                public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";

                    var result = 引数を変更(arg0, arg1).ToList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal(1, result[0]);
                    Assert.Equal("2", result[1]);
                }

                #endregion

                #region 引数3つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                }

                [Fact]
                public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;

                    var result = 引数を変更(arg0, arg1, arg2).ToList();

                    Assert.Equal(3, result.Count);
                    Assert.Equal(1, result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3, result[2]);
                }

                #endregion

                #region 引数4つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                }

                [Fact]
                public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";

                    var result = 引数を変更(arg0, arg1, arg2, arg3).ToList();

                    Assert.Equal(4,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                }

                #endregion

                #region 引数5つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                }

                [Fact]
                public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4).ToList();

                    Assert.Equal(5,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                }

                #endregion

                #region 引数6つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5).ToList();

                    Assert.Equal(6,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                }

                #endregion

                #region 引数7つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6).ToList();

                    Assert.Equal(7,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                }

                #endregion

                #region 引数8つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7).ToList();

                    Assert.Equal(8,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                    Assert.Equal("8", result[7]);
                }

                #endregion

                #region 引数9つ

                [ChangeArguments]
                private IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7, int arg8)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).ToList();

                    Assert.Equal(9,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                    Assert.Equal("8", result[7]);
                    Assert.Equal(9,   result[8]);
                }

                #endregion
            }

            public class 戻り値
            {
                #region 値型

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        var yieldValue = (int)args.YieldValue;
                        args.YieldValue = yieldValue + 1;
                    }
                }

                [Increment]
                private IEnumerable<int> 戻り値をインクリメント(int arg1)
                {
                    yield return arg1;
                }

                [Fact]
                public void 戻り値をインクリメント_正しくアスペクトが適用される()
                {
                    var result = 戻り値をインクリメント(1).ToList();
                    Assert.Equal(2, result[0]);
                }

                #endregion

                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        var yieldValue = args.YieldValue as string;
                        args.YieldValue = yieldValue.ToUpper();
                    }
                }

                [ToUpper]
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

                #endregion
            }

            public class 仮想関数
            {
                private class ToUpperAspect : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        args.YieldValue = ((string)args.YieldValue).ToUpper();
                    }
                }

                private abstract class Base
                {
                    public abstract IEnumerable<string> 戻り値を大文字に変更(string arg0, string arg1);
                }

                private class Derived : Base
                {
                    [ToUpperAspect]
                    public override IEnumerable<string> 戻り値を大文字に変更(string arg0, string arg1)
                    {
                        yield return arg0;
                        yield return arg1;
                    }
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var arg0 = "a";
                    var arg1 = "b";

                    var result = new Derived().戻り値を大文字に変更(arg0, arg1).ToList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal("A", result[0]);
                    Assert.Equal("B", result[1]);
                }
            }
        }

        public class 静的イテレーターメソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private static IEnumerable<int> 正常()
                {
                    Logger.Trace("A");
                    yield return 0;
                    Logger.Trace("B");
                    yield return 1;
                    Logger.Trace("C");
                }

                [EventLogger]
                private static IEnumerable<int> 例外()
                {
                    Logger.Trace("A");
                    yield return 0;
                    Logger.Trace("B");
                    yield return 1;
                    Logger.Trace("C");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    正常().ToList();

                    Assert.Equal($"OnEntry A OnYield OnResume B OnYield OnResume C OnSuccess OnExit ", appender.ToString());
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var exception = Record.Exception(() => 例外().ToList());

                    Assert.IsType<InvalidOperationException>(exception);
                    Assert.Equal($"OnEntry A OnYield OnResume B OnYield OnResume C OnException OnExit ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private static IEnumerable<int> メソッド()
                {
                    yield return 0;
                    yield return 1;
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド().ToList();

                    Assert.Null(Instance);
                }
            }

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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
                    }
                }

                #region 引数1つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0)
                {
                    yield return arg0;
                }

                [Fact]
                public void 引数を変更_引数1つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;

                    var result = 引数を変更(arg0).ToList();

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                }

                #endregion

                #region 引数2つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1)
                {
                    yield return arg0;
                    yield return arg1;
                }

                [Fact]
                public void 引数を変更_引数2つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";

                    var result = 引数を変更(arg0, arg1).ToList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal(1, result[0]);
                    Assert.Equal("2", result[1]);
                }

                #endregion

                #region 引数3つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                }

                [Fact]
                public void 引数を変更_引数3つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;

                    var result = 引数を変更(arg0, arg1, arg2).ToList();

                    Assert.Equal(3, result.Count);
                    Assert.Equal(1, result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3, result[2]);
                }

                #endregion

                #region 引数4つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                }

                [Fact]
                public void 引数を変更_引数4つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";

                    var result = 引数を変更(arg0, arg1, arg2, arg3).ToList();

                    Assert.Equal(4,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                }

                #endregion

                #region 引数5つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                }

                [Fact]
                public void 引数を変更_引数5つ_正しくアスペクトが適用される()
                {
                    var arg0 = 0;
                    var arg1 = "1";
                    var arg2 = 2;
                    var arg3 = "3";
                    var arg4 = 4;

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4).ToList();

                    Assert.Equal(5,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                }

                #endregion

                #region 引数6つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5).ToList();

                    Assert.Equal(6,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                }

                #endregion

                #region 引数7つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6).ToList();

                    Assert.Equal(7,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                }

                #endregion

                #region 引数8つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7).ToList();

                    Assert.Equal(8,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                    Assert.Equal("8", result[7]);
                }

                #endregion

                #region 引数9つ

                [ChangeArguments]
                private static IEnumerable<object> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7, int arg8)
                {
                    yield return arg0;
                    yield return arg1;
                    yield return arg2;
                    yield return arg3;
                    yield return arg4;
                    yield return arg5;
                    yield return arg6;
                    yield return arg7;
                    yield return arg8;
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

                    var result = 引数を変更(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).ToList();

                    Assert.Equal(9,   result.Count);
                    Assert.Equal(1,   result[0]);
                    Assert.Equal("2", result[1]);
                    Assert.Equal(3,   result[2]);
                    Assert.Equal("4", result[3]);
                    Assert.Equal(5,   result[4]);
                    Assert.Equal("6", result[5]);
                    Assert.Equal(7,   result[6]);
                    Assert.Equal("8", result[7]);
                    Assert.Equal(9,   result[8]);
                }

                #endregion
            }

            public class 戻り値
            {
                #region 値型

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        var yieldValue = (int)args.YieldValue;
                        args.YieldValue = yieldValue + 1;
                    }
                }

                [Increment]
                private static IEnumerable<int> 戻り値をインクリメント(int arg1)
                {
                    yield return arg1;
                }

                [Fact]
                public void 戻り値をインクリメント_正しくアスペクトが適用される()
                {
                    var result = 戻り値をインクリメント(1).ToList();
                    Assert.Equal(2, result[0]);
                }

                #endregion

                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnYield(MethodExecutionArgs args)
                    {
                        var yieldValue = args.YieldValue as string;
                        args.YieldValue = yieldValue.ToUpper();
                    }
                }

                [ToUpper]
                private static IEnumerable<string> 戻り値を大文字に変更(string arg1)
                {
                    yield return arg1;
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var result = 戻り値を大文字に変更("a").ToList();
                    Assert.Equal("A", result[0]);
                }

                #endregion
            }
        }

        public class 非同期メソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private async Task 正常()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("4");
                    });

                    Logger.Trace("5");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("6");
                    });

                    Logger.Trace("7");
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task =  正常();
                    Logger.Trace("1");
                    task.Wait();
                    Logger.Trace("8");

                    Assert.Equal($"OnEntry 0 OnYield 1 2 OnResume 3 OnYield 4 OnResume 5 OnYield 6 OnResume 7 OnSuccess OnExit 8 ", appender.ToString());
                }

                [EventLogger]
                private async Task 例外()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("4");
                    });

                    Logger.Trace("5");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("6");
                    });

                    Logger.Trace("7");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task =  例外();
                    Logger.Trace("1");
                    var exception =  Record.Exception(() => task.Wait());
                    Logger.Trace("8");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 OnYield 1 2 OnResume 3 OnYield 4 OnResume 5 OnYield 6 OnResume 7 OnException OnExit 8 ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private async Task メソッド()
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド().Wait();

                    Assert.Same(this, Instance);
                }
            }

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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

                    Assert.Equal(1,   result0);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
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
                #region 値型

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
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

                    Assert.Equal(2, task.Result);
                }

                #endregion

                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        var returnValue = (string)args.ReturnValue;
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

                    Assert.Equal("A", task.Result);
                }

                #endregion
            }

            public class 仮想関数
            {
                private class ToUpperAspect : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        args.ReturnValue = ((string)args.ReturnValue).ToUpper();
                    }
                }

                private abstract class Base
                {
                    public abstract Task<string> 戻り値を大文字に変更(string arg);
                }

                private class Derived : Base
                {
                    [ToUpperAspect]
                    public async override Task<string> 戻り値を大文字に変更(string arg)
                    {
                        await Task.Run(() =>
                        {
                            Thread.Sleep(10);
                        });

                        return arg;
                    }
                }

                [Fact]
                public void 戻り値を大文字に変更_正しくアスペクトが適用される()
                {
                    var arg = "a";

                    var task = new Derived().戻り値を大文字に変更(arg);
                    task.Wait();

                    Assert.Equal("A", task.Result);
                }
            }
        }

        public class 静的非同期メソッド
        {
            public class イベントハンドラーの呼びだし順序
            {
                private class EventLogger : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnEntry");
                    }

                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnSuccess");
                    }

                    public override void OnException(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnException");
                    }

                    public override void OnExit(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnExit");
                    }

                    public override void OnResume(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnResume");
                    }

                    public override void OnYield(MethodExecutionArgs args)
                    {
                        Logger.Trace("OnYield");
                    }
                }

                [EventLogger]
                private static async Task 正常()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("4");
                    });

                    Logger.Trace("5");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("6");
                    });

                    Logger.Trace("7");
                }

                [Fact]
                public void 正常_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task =  正常();
                    Logger.Trace("1");
                    task.Wait();
                    Logger.Trace("8");

                    Assert.Equal($"OnEntry 0 OnYield 1 2 OnResume 3 OnYield 4 OnResume 5 OnYield 6 OnResume 7 OnSuccess OnExit 8 ", appender.ToString());
                }

                [EventLogger]
                private static async Task 例外()
                {
                    Logger.Trace("0");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("2");
                    });

                    Logger.Trace("3");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("4");
                    });

                    Logger.Trace("5");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        Logger.Trace("6");
                    });

                    Logger.Trace("7");
                    throw new InvalidOperationException();
                }

                [Fact]
                public void 例外_イベントハンドラーが正しくよばれる()
                {
                    var appender = CreateAppender();

                    var task =  例外();
                    Logger.Trace("1");
                    var exception =  Record.Exception(() => task.Wait());
                    Logger.Trace("8");

                    var aggregateException = Assert.IsType<AggregateException>(exception);
                    Assert.Single(aggregateException.InnerExceptions);
                    Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[0]);
                    Assert.Equal($"OnEntry 0 OnYield 1 2 OnResume 3 OnYield 4 OnResume 5 OnYield 6 OnResume 7 OnException OnExit 8 ", appender.ToString());
                }
            }

            public class AspectArgs
            {
                private static object Instance;

                private class OnEntrySpy : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        Instance = args.Instance;
                    }
                }

                [OnEntrySpy]
                private static async Task メソッド()
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });
                }

                [Fact]
                public void メソッド_正しくアスペクトが適用される()
                {
                    メソッド().Wait();

                    Assert.Null(Instance);
                }
            }

            public class 引数
            {
                private class ChangeArguments : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
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
                    }
                }

                #region 引数1つ

                [ChangeArguments]
                private static async Task<int> 引数を変更(int arg0)
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
                private static async Task<(int, string)> 引数を変更(int arg0, string arg1)
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                }

                #endregion

                #region 引数3つ

                [ChangeArguments]
                private static async Task<(int, string, int)> 引数を変更(int arg0, string arg1, int arg2)
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                }

                #endregion

                #region 引数4つ

                [ChangeArguments]
                private static async Task<(int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3)
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                }

                #endregion

                #region 引数5つ

                [ChangeArguments]
                private static async Task<(int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4)
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
                private static async Task<(int, string, int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5)
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

                    Assert.Equal(1,   result0);
                    Assert.Equal("2", result1);
                    Assert.Equal(3,   result2);
                    Assert.Equal("4", result3);
                    Assert.Equal(5,   result4);
                    Assert.Equal("6", result5);
                }

                #endregion

                #region 引数7つ

                [ChangeArguments]
                private static async Task<(int, string, int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6)
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
                private static async Task<(int, string, int, string, int, string, int, string)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7)
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
                private static async Task<(int, string, int, string, int, string, int, string, int)> 引数を変更(int arg0, string arg1, int arg2, string arg3, int arg4, string arg5, int arg6, string arg7, int arg8)
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
                #region 値型

                private class Increment : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        var returnValue = (int)args.ReturnValue;
                        args.ReturnValue = returnValue + 1;
                    }
                }

                [Increment]
                private static async Task<int> 戻り値をインクリメント(int arg1)
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

                    Assert.Equal(2, task.Result);
                }

                #endregion

                #region 参照型

                private class ToUpper : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        var returnValue = (string)args.ReturnValue;
                        args.ReturnValue = returnValue.ToUpper();
                    }
                }

                [ToUpper]
                private static async Task<string> 戻り値を大文字に変更(string arg1)
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

                    Assert.Equal("A", task.Result);
                }

                #endregion
            }
        }
    }
}
