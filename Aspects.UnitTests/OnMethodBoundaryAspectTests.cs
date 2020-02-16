using SoftCube.Logging;
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

            public class 引数の変更
            {
                #region 参照型

                private class ToUpperAspect : OnMethodBoundaryAspect
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

                [ToUpperAspect]
                private async Task<string> 引数を大文字に変更(string arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return arg1;
                }

                [ToUpperAspect]
                private async Task<(string, string)> 引数を大文字に変更(string arg1, string arg2)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2);
                }

                [ToUpperAspect]
                private async Task<(string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string, string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }

                [ToUpperAspect]
                private async Task<(string, string, string, string, string, string, string, string, string)> 引数を大文字に変更(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                }

                [Fact]
                public void 引数を大文字に変更_引数が1つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a");
                    task.Wait();

                    Assert.Equal("A", task.Result);
                }

                [Fact]
                public void 引数を大文字に変更_引数が2つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a", "b");
                    task.Wait();

                    var (result1, result2) = task.Result;

                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                }

                [Fact]
                public void 引数を大文字に変更_引数が3つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a", "b", "c");
                    task.Wait();

                    var (result1, result2, result3) = task.Result;

                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                }

                [Fact]
                public void 引数を大文字に変更_引数が4つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a", "b", "c", "d");
                    task.Wait();

                    var (result1, result2, result3, result4) = task.Result;

                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                }

                [Fact]
                public void 引数を大文字に変更_引数が5つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a", "b", "c", "d", "e");
                    task.Wait();

                    var (result1, result2, result3, result4, result5) = task.Result;

                    Assert.Equal("A", result1);
                    Assert.Equal("B", result2);
                    Assert.Equal("C", result3);
                    Assert.Equal("D", result4);
                    Assert.Equal("E", result5);
                }

                [Fact]
                public void 引数を大文字に変更_引数が6つ_正しくアスペクトが適用される()
                {
                    var task = 引数を大文字に変更("a", "b", "c", "d", "e", "f");
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6) = task.Result;

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
                    var task = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g");
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7) = task.Result;

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
                    var task = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h");
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7, result8) = task.Result;

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
                    var task = 引数を大文字に変更("a", "b", "c", "d", "e", "f", "g", "h", "i");
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7, result8, result9) = task.Result;

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

                #endregion

                #region 値型

                private class IncrementAspect : OnMethodBoundaryAspect
                {
                    public override void OnEntry(MethodExecutionArgs args)
                    {
                        for (int argumentIndex = 0; argumentIndex < args.Arguments.Count; argumentIndex++)
                        {
                            var argument = (int)args.Arguments[argumentIndex];
                            args.Arguments[argumentIndex] = argument + 1;
                        }
                    }
                }

                [IncrementAspect]
                private async Task<int> 引数をインクリメント(int arg1)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return arg1;
                }

                [IncrementAspect]
                private async Task<(int, int)> 引数をインクリメント(int arg1, int arg2)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2);
                }

                [IncrementAspect]
                private async Task<(int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }

                [IncrementAspect]
                private async Task<(int, int, int, int, int, int, int, int, int)> 引数をインクリメント(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(10);
                    });

                    return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                }

                [Fact]
                public void 引数をインクリメント_引数が1つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1);
                    task.Wait();

                    var result1 = task.Result;

                    Assert.Equal(2, result1);
                }

                [Fact]
                public void 引数をインクリメント_引数が2つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2);
                    task.Wait();

                    var (result1, result2) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                }

                [Fact]
                public void 引数をインクリメント_引数が3つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3);
                    task.Wait();

                    var (result1, result2, result3) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                }

                [Fact]
                public void 引数をインクリメント_引数が4つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4);
                    task.Wait();

                    var (result1, result2, result3, result4) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                }

                [Fact]
                public void 引数をインクリメント_引数が5つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4, 5);
                    task.Wait();

                    var (result1, result2, result3, result4, result5) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                    Assert.Equal(6, result5);
                }

                [Fact]
                public void 引数をインクリメント_引数が6つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4, 5, 6);
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                    Assert.Equal(6, result5);
                    Assert.Equal(7, result6);
                }

                [Fact]
                public void 引数をインクリメント_引数が7つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4, 5, 6, 7);
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                    Assert.Equal(6, result5);
                    Assert.Equal(7, result6);
                    Assert.Equal(8, result7);
                }

                [Fact]
                public void 引数をインクリメント_引数が8つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4, 5, 6, 7, 8);
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7, result8) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                    Assert.Equal(6, result5);
                    Assert.Equal(7, result6);
                    Assert.Equal(8, result7);
                    Assert.Equal(9, result8);
                }

                [Fact]
                public void 引数をインクリメント_引数が9つ_正しくアスペクトが適用される()
                {
                    var task = 引数をインクリメント(1, 2, 3, 4, 5, 6, 7, 8, 9);
                    task.Wait();

                    var (result1, result2, result3, result4, result5, result6, result7, result8, result9) = task.Result;

                    Assert.Equal(2, result1);
                    Assert.Equal(3, result2);
                    Assert.Equal(4, result3);
                    Assert.Equal(5, result4);
                    Assert.Equal(6, result5);
                    Assert.Equal(7, result6);
                    Assert.Equal(8, result7);
                    Assert.Equal(9, result8);
                    Assert.Equal(10, result9);
                }

                #endregion
            }

            public class 戻り値の変更
            {
                #region 参照型

                private class ToUpperAspect : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        var returnValue = args.ReturnValue as string;
                        args.ReturnValue = returnValue.ToUpper();
                    }
                }

                [ToUpperAspect]
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

                #region 値型

                private class IncrementAspect : OnMethodBoundaryAspect
                {
                    public override void OnSuccess(MethodExecutionArgs args)
                    {
                        var returnValue = (int)args.ReturnValue;
                        args.ReturnValue = returnValue + 1;
                    }
                }

                [IncrementAspect]
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
            }
        }
    }
}
