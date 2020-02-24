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
                                    case bool argument:
                                        args.Arguments[argumentIndex] = !argument;
                                        break;

                                    case sbyte argument:
                                        args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                                        break;

                                    case short argument:
                                        args.Arguments[argumentIndex] = (short)(argument + 1);
                                        break;

                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case long argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case byte argument:
                                        args.Arguments[argumentIndex] = (byte)(argument + 1);
                                        break;

                                    case ushort argument:
                                        args.Arguments[argumentIndex] = (ushort)(argument + 1);
                                        break;

                                    case uint argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case ulong argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case float argument:
                                        args.Arguments[argumentIndex] = argument + 1.0f;
                                        break;

                                    case double argument:
                                        args.Arguments[argumentIndex] = argument + 1.0;
                                        break;

                                    case decimal argument:
                                        args.Arguments[argumentIndex] = argument + 1.0m;
                                        break;

                                    case char argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument.ToString()) + 1).ToString()[0];
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

                    #region ポインタ引数

                    #region bool

                    [ChangeArguments]
                    private void 引数を変更(ref bool arg0)
                    {
                    }

                    [Fact]
                    public void bool型_引数1つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;

                        引数を変更(ref arg0);

                        Assert.True(arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref bool arg0, ref bool arg1, ref bool arg2, ref bool arg3, ref bool arg4, ref bool arg5, ref bool arg6, ref bool arg7, ref bool arg8)
                    {
                    }

                    [Fact]
                    public void bool型_引数8つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;
                        bool arg1 = true;
                        bool arg2 = false;
                        bool arg3 = true;
                        bool arg4 = false;
                        bool arg5 = true;
                        bool arg6 = false;
                        bool arg7 = true;
                        bool arg8 = false;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(arg0);
                        Assert.False(arg1);
                        Assert.True(arg2);
                        Assert.False(arg3);
                        Assert.True(arg4);
                        Assert.False(arg5);
                        Assert.True(arg6);
                        Assert.False(arg7);
                        Assert.True(arg8);
                    }

                    #endregion

                    #region sbyte

                    [ChangeArguments]
                    private void 引数を変更(ref sbyte arg0)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数1つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref sbyte arg0, ref sbyte arg1, ref sbyte arg2, ref sbyte arg3, ref sbyte arg4, ref sbyte arg5, ref sbyte arg6, ref sbyte arg7, ref sbyte arg8)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数8つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;
                        sbyte arg1 = 1;
                        sbyte arg2 = 2;
                        sbyte arg3 = 3;
                        sbyte arg4 = 4;
                        sbyte arg5 = 5;
                        sbyte arg6 = 6;
                        sbyte arg7 = 7;
                        sbyte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region short

                    [ChangeArguments]
                    private void 引数を変更(ref short arg0)
                    {
                    }

                    [Fact]
                    public void short型_引数1つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref short arg0, ref short arg1, ref short arg2, ref short arg3, ref short arg4, ref short arg5, ref short arg6, ref short arg7, ref short arg8)
                    {
                    }

                    [Fact]
                    public void short型_引数8つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;
                        short arg1 = 1;
                        short arg2 = 2;
                        short arg3 = 3;
                        short arg4 = 4;
                        short arg5 = 5;
                        short arg6 = 6;
                        short arg7 = 7;
                        short arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region int

                    [ChangeArguments]
                    private void 引数を変更(ref int arg0)
                    {
                    }

                    [Fact]
                    public void int型_引数1つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref int arg0, ref int arg1, ref int arg2, ref int arg3, ref int arg4, ref int arg5, ref int arg6, ref int arg7, ref int arg8)
                    {
                    }

                    [Fact]
                    public void int型_引数8つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;
                        int arg1 = 1;
                        int arg2 = 2;
                        int arg3 = 3;
                        int arg4 = 4;
                        int arg5 = 5;
                        int arg6 = 6;
                        int arg7 = 7;
                        int arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region long

                    [ChangeArguments]
                    private void 引数を変更(ref long arg0)
                    {
                    }

                    [Fact]
                    public void long型_引数1つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref long arg0, ref long arg1, ref long arg2, ref long arg3, ref long arg4, ref long arg5, ref long arg6, ref long arg7, ref long arg8)
                    {
                    }

                    [Fact]
                    public void long型_引数8つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;
                        long arg1 = 1;
                        long arg2 = 2;
                        long arg3 = 3;
                        long arg4 = 4;
                        long arg5 = 5;
                        long arg6 = 6;
                        long arg7 = 7;
                        long arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region byte

                    [ChangeArguments]
                    private void 引数を変更(ref byte arg0)
                    {
                    }

                    [Fact]
                    public void byte型_引数1つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref byte arg0, ref byte arg1, ref byte arg2, ref byte arg3, ref byte arg4, ref byte arg5, ref byte arg6, ref byte arg7, ref byte arg8)
                    {
                    }

                    [Fact]
                    public void byte型_引数8つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;
                        byte arg1 = 1;
                        byte arg2 = 2;
                        byte arg3 = 3;
                        byte arg4 = 4;
                        byte arg5 = 5;
                        byte arg6 = 6;
                        byte arg7 = 7;
                        byte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region ushort

                    [ChangeArguments]
                    private void 引数を変更(ref ushort arg0)
                    {
                    }

                    [Fact]
                    public void ushort型_引数1つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref ushort arg0, ref ushort arg1, ref ushort arg2, ref ushort arg3, ref ushort arg4, ref ushort arg5, ref ushort arg6, ref ushort arg7, ref ushort arg8)
                    {
                    }

                    [Fact]
                    public void ushort型_引数8つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;
                        ushort arg1 = 1;
                        ushort arg2 = 2;
                        ushort arg3 = 3;
                        ushort arg4 = 4;
                        ushort arg5 = 5;
                        ushort arg6 = 6;
                        ushort arg7 = 7;
                        ushort arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region uint

                    [ChangeArguments]
                    private void 引数を変更(ref uint arg0)
                    {
                    }

                    [Fact]
                    public void uint型_引数1つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref uint arg0, ref uint arg1, ref uint arg2, ref uint arg3, ref uint arg4, ref uint arg5, ref uint arg6, ref uint arg7, ref uint arg8)
                    {
                    }

                    [Fact]
                    public void uint型_引数8つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;
                        uint arg1 = 1;
                        uint arg2 = 2;
                        uint arg3 = 3;
                        uint arg4 = 4;
                        uint arg5 = 5;
                        uint arg6 = 6;
                        uint arg7 = 7;
                        uint arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region float

                    [ChangeArguments]
                    private void 引数を変更(ref float arg0)
                    {
                    }

                    [Fact]
                    public void float型_引数1つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref float arg0, ref float arg1, ref float arg2, ref float arg3, ref float arg4, ref float arg5, ref float arg6, ref float arg7, ref float arg8)
                    {
                    }

                    [Fact]
                    public void float型_引数8つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;
                        float arg1 = 1;
                        float arg2 = 2;
                        float arg3 = 3;
                        float arg4 = 4;
                        float arg5 = 5;
                        float arg6 = 6;
                        float arg7 = 7;
                        float arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region double

                    [ChangeArguments]
                    private void 引数を変更(ref double arg0)
                    {
                    }

                    [Fact]
                    public void double型_引数1つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref double arg0, ref double arg1, ref double arg2, ref double arg3, ref double arg4, ref double arg5, ref double arg6, ref double arg7, ref double arg8)
                    {
                    }

                    [Fact]
                    public void double型_引数8つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;
                        double arg1 = 1;
                        double arg2 = 2;
                        double arg3 = 3;
                        double arg4 = 4;
                        double arg5 = 5;
                        double arg6 = 6;
                        double arg7 = 7;
                        double arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region decimal

                    [ChangeArguments]
                    private void 引数を変更(ref decimal arg0)
                    {
                    }

                    [Fact]
                    public void decimal型_引数1つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref decimal arg0, ref decimal arg1, ref decimal arg2, ref decimal arg3, ref decimal arg4, ref decimal arg5, ref decimal arg6, ref decimal arg7, ref decimal arg8)
                    {
                    }

                    [Fact]
                    public void decimal型_引数8つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;
                        decimal arg1 = 1;
                        decimal arg2 = 2;
                        decimal arg3 = 3;
                        decimal arg4 = 4;
                        decimal arg5 = 5;
                        decimal arg6 = 6;
                        decimal arg7 = 7;
                        decimal arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region char

                    [ChangeArguments]
                    private void 引数を変更(ref char arg0)
                    {
                    }

                    [Fact]
                    public void char型_引数1つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';

                        引数を変更(ref arg0);

                        Assert.Equal('1', arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref char arg0, ref char arg1, ref char arg2, ref char arg3, ref char arg4, ref char arg5, ref char arg6, ref char arg7, ref char arg8)
                    {
                    }

                    [Fact]
                    public void char型_引数8つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';
                        char arg1 = '1';
                        char arg2 = '2';
                        char arg3 = '3';
                        char arg4 = '4';
                        char arg5 = '5';
                        char arg6 = '6';
                        char arg7 = '7';
                        char arg8 = '8';

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal('1', arg0);
                        Assert.Equal('2', arg1);
                        Assert.Equal('3', arg2);
                        Assert.Equal('4', arg3);
                        Assert.Equal('5', arg4);
                        Assert.Equal('6', arg5);
                        Assert.Equal('7', arg6);
                        Assert.Equal('8', arg7);
                        Assert.Equal('9', arg8);
                    }

                    #endregion

                    #region string

                    [ChangeArguments]
                    private void 引数を変更(ref string arg0)
                    {
                    }

                    [Fact]
                    public void string型_引数1つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";

                        引数を変更(ref arg0);

                        Assert.Equal("1", arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref string arg0, ref string arg1, ref string arg2, ref string arg3, ref string arg4, ref string arg5, ref string arg6, ref string arg7, ref string arg8)
                    {
                    }

                    [Fact]
                    public void string型_引数8つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";
                        string arg1 = "1";
                        string arg2 = "2";
                        string arg3 = "3";
                        string arg4 = "4";
                        string arg5 = "5";
                        string arg6 = "6";
                        string arg7 = "7";
                        string arg8 = "8";

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal("1", arg0);
                        Assert.Equal("2", arg1);
                        Assert.Equal("3", arg2);
                        Assert.Equal("4", arg3);
                        Assert.Equal("5", arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal("7", arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal("9", arg8);
                    }

                    #endregion

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
                                    case bool argument:
                                        args.Arguments[argumentIndex] = !argument;
                                        break;

                                    case sbyte argument:
                                        args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                                        break;

                                    case short argument:
                                        args.Arguments[argumentIndex] = (short)(argument + 1);
                                        break;

                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case long argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case byte argument:
                                        args.Arguments[argumentIndex] = (byte)(argument + 1);
                                        break;

                                    case ushort argument:
                                        args.Arguments[argumentIndex] = (ushort)(argument + 1);
                                        break;

                                    case uint argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case ulong argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case float argument:
                                        args.Arguments[argumentIndex] = argument + 1.0f;
                                        break;

                                    case double argument:
                                        args.Arguments[argumentIndex] = argument + 1.0;
                                        break;

                                    case decimal argument:
                                        args.Arguments[argumentIndex] = argument + 1.0m;
                                        break;

                                    case char argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument.ToString()) + 1).ToString()[0];
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

                    #region ポインタ引数

                    #region bool

                    [ChangeArguments]
                    private void 引数を変更(ref bool arg0)
                    {
                    }

                    [Fact]
                    public void bool型_引数1つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;

                        引数を変更(ref arg0);

                        Assert.True(arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref bool arg0, ref bool arg1, ref bool arg2, ref bool arg3, ref bool arg4, ref bool arg5, ref bool arg6, ref bool arg7, ref bool arg8)
                    {
                    }

                    [Fact]
                    public void bool型_引数8つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;
                        bool arg1 = true;
                        bool arg2 = false;
                        bool arg3 = true;
                        bool arg4 = false;
                        bool arg5 = true;
                        bool arg6 = false;
                        bool arg7 = true;
                        bool arg8 = false;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(arg0);
                        Assert.False(arg1);
                        Assert.True(arg2);
                        Assert.False(arg3);
                        Assert.True(arg4);
                        Assert.False(arg5);
                        Assert.True(arg6);
                        Assert.False(arg7);
                        Assert.True(arg8);
                    }

                    #endregion

                    #region sbyte

                    [ChangeArguments]
                    private void 引数を変更(ref sbyte arg0)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数1つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref sbyte arg0, ref sbyte arg1, ref sbyte arg2, ref sbyte arg3, ref sbyte arg4, ref sbyte arg5, ref sbyte arg6, ref sbyte arg7, ref sbyte arg8)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数8つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;
                        sbyte arg1 = 1;
                        sbyte arg2 = 2;
                        sbyte arg3 = 3;
                        sbyte arg4 = 4;
                        sbyte arg5 = 5;
                        sbyte arg6 = 6;
                        sbyte arg7 = 7;
                        sbyte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region short

                    [ChangeArguments]
                    private void 引数を変更(ref short arg0)
                    {
                    }

                    [Fact]
                    public void short型_引数1つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref short arg0, ref short arg1, ref short arg2, ref short arg3, ref short arg4, ref short arg5, ref short arg6, ref short arg7, ref short arg8)
                    {
                    }

                    [Fact]
                    public void short型_引数8つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;
                        short arg1 = 1;
                        short arg2 = 2;
                        short arg3 = 3;
                        short arg4 = 4;
                        short arg5 = 5;
                        short arg6 = 6;
                        short arg7 = 7;
                        short arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region int

                    [ChangeArguments]
                    private void 引数を変更(ref int arg0)
                    {
                    }

                    [Fact]
                    public void int型_引数1つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref int arg0, ref int arg1, ref int arg2, ref int arg3, ref int arg4, ref int arg5, ref int arg6, ref int arg7, ref int arg8)
                    {
                    }

                    [Fact]
                    public void int型_引数8つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;
                        int arg1 = 1;
                        int arg2 = 2;
                        int arg3 = 3;
                        int arg4 = 4;
                        int arg5 = 5;
                        int arg6 = 6;
                        int arg7 = 7;
                        int arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region long

                    [ChangeArguments]
                    private void 引数を変更(ref long arg0)
                    {
                    }

                    [Fact]
                    public void long型_引数1つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref long arg0, ref long arg1, ref long arg2, ref long arg3, ref long arg4, ref long arg5, ref long arg6, ref long arg7, ref long arg8)
                    {
                    }

                    [Fact]
                    public void long型_引数8つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;
                        long arg1 = 1;
                        long arg2 = 2;
                        long arg3 = 3;
                        long arg4 = 4;
                        long arg5 = 5;
                        long arg6 = 6;
                        long arg7 = 7;
                        long arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region byte

                    [ChangeArguments]
                    private void 引数を変更(ref byte arg0)
                    {
                    }

                    [Fact]
                    public void byte型_引数1つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref byte arg0, ref byte arg1, ref byte arg2, ref byte arg3, ref byte arg4, ref byte arg5, ref byte arg6, ref byte arg7, ref byte arg8)
                    {
                    }

                    [Fact]
                    public void byte型_引数8つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;
                        byte arg1 = 1;
                        byte arg2 = 2;
                        byte arg3 = 3;
                        byte arg4 = 4;
                        byte arg5 = 5;
                        byte arg6 = 6;
                        byte arg7 = 7;
                        byte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region ushort

                    [ChangeArguments]
                    private void 引数を変更(ref ushort arg0)
                    {
                    }

                    [Fact]
                    public void ushort型_引数1つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref ushort arg0, ref ushort arg1, ref ushort arg2, ref ushort arg3, ref ushort arg4, ref ushort arg5, ref ushort arg6, ref ushort arg7, ref ushort arg8)
                    {
                    }

                    [Fact]
                    public void ushort型_引数8つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;
                        ushort arg1 = 1;
                        ushort arg2 = 2;
                        ushort arg3 = 3;
                        ushort arg4 = 4;
                        ushort arg5 = 5;
                        ushort arg6 = 6;
                        ushort arg7 = 7;
                        ushort arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region uint

                    [ChangeArguments]
                    private void 引数を変更(ref uint arg0)
                    {
                    }

                    [Fact]
                    public void uint型_引数1つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref uint arg0, ref uint arg1, ref uint arg2, ref uint arg3, ref uint arg4, ref uint arg5, ref uint arg6, ref uint arg7, ref uint arg8)
                    {
                    }

                    [Fact]
                    public void uint型_引数8つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;
                        uint arg1 = 1;
                        uint arg2 = 2;
                        uint arg3 = 3;
                        uint arg4 = 4;
                        uint arg5 = 5;
                        uint arg6 = 6;
                        uint arg7 = 7;
                        uint arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region float

                    [ChangeArguments]
                    private void 引数を変更(ref float arg0)
                    {
                    }

                    [Fact]
                    public void float型_引数1つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref float arg0, ref float arg1, ref float arg2, ref float arg3, ref float arg4, ref float arg5, ref float arg6, ref float arg7, ref float arg8)
                    {
                    }

                    [Fact]
                    public void float型_引数8つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;
                        float arg1 = 1;
                        float arg2 = 2;
                        float arg3 = 3;
                        float arg4 = 4;
                        float arg5 = 5;
                        float arg6 = 6;
                        float arg7 = 7;
                        float arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region double

                    [ChangeArguments]
                    private void 引数を変更(ref double arg0)
                    {
                    }

                    [Fact]
                    public void double型_引数1つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref double arg0, ref double arg1, ref double arg2, ref double arg3, ref double arg4, ref double arg5, ref double arg6, ref double arg7, ref double arg8)
                    {
                    }

                    [Fact]
                    public void double型_引数8つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;
                        double arg1 = 1;
                        double arg2 = 2;
                        double arg3 = 3;
                        double arg4 = 4;
                        double arg5 = 5;
                        double arg6 = 6;
                        double arg7 = 7;
                        double arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region decimal

                    [ChangeArguments]
                    private void 引数を変更(ref decimal arg0)
                    {
                    }

                    [Fact]
                    public void decimal型_引数1つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref decimal arg0, ref decimal arg1, ref decimal arg2, ref decimal arg3, ref decimal arg4, ref decimal arg5, ref decimal arg6, ref decimal arg7, ref decimal arg8)
                    {
                    }

                    [Fact]
                    public void decimal型_引数8つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;
                        decimal arg1 = 1;
                        decimal arg2 = 2;
                        decimal arg3 = 3;
                        decimal arg4 = 4;
                        decimal arg5 = 5;
                        decimal arg6 = 6;
                        decimal arg7 = 7;
                        decimal arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region char

                    [ChangeArguments]
                    private void 引数を変更(ref char arg0)
                    {
                    }

                    [Fact]
                    public void char型_引数1つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';

                        引数を変更(ref arg0);

                        Assert.Equal('1', arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref char arg0, ref char arg1, ref char arg2, ref char arg3, ref char arg4, ref char arg5, ref char arg6, ref char arg7, ref char arg8)
                    {
                    }

                    [Fact]
                    public void char型_引数8つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';
                        char arg1 = '1';
                        char arg2 = '2';
                        char arg3 = '3';
                        char arg4 = '4';
                        char arg5 = '5';
                        char arg6 = '6';
                        char arg7 = '7';
                        char arg8 = '8';

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal('1', arg0);
                        Assert.Equal('2', arg1);
                        Assert.Equal('3', arg2);
                        Assert.Equal('4', arg3);
                        Assert.Equal('5', arg4);
                        Assert.Equal('6', arg5);
                        Assert.Equal('7', arg6);
                        Assert.Equal('8', arg7);
                        Assert.Equal('9', arg8);
                    }

                    #endregion

                    #region string

                    [ChangeArguments]
                    private void 引数を変更(ref string arg0)
                    {
                    }

                    [Fact]
                    public void string型_引数1つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";

                        引数を変更(ref arg0);

                        Assert.Equal("1", arg0);
                    }

                    [ChangeArguments]
                    private void 引数を変更(ref string arg0, ref string arg1, ref string arg2, ref string arg3, ref string arg4, ref string arg5, ref string arg6, ref string arg7, ref string arg8)
                    {
                    }

                    [Fact]
                    public void string型_引数8つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";
                        string arg1 = "1";
                        string arg2 = "2";
                        string arg3 = "3";
                        string arg4 = "4";
                        string arg5 = "5";
                        string arg6 = "6";
                        string arg7 = "7";
                        string arg8 = "8";

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal("1", arg0);
                        Assert.Equal("2", arg1);
                        Assert.Equal("3", arg2);
                        Assert.Equal("4", arg3);
                        Assert.Equal("5", arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal("7", arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal("9", arg8);
                    }

                    #endregion

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
                                    case bool argument:
                                        args.Arguments[argumentIndex] = !argument;
                                        break;

                                    case sbyte argument:
                                        args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                                        break;

                                    case short argument:
                                        args.Arguments[argumentIndex] = (short)(argument + 1);
                                        break;

                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case long argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case byte argument:
                                        args.Arguments[argumentIndex] = (byte)(argument + 1);
                                        break;

                                    case ushort argument:
                                        args.Arguments[argumentIndex] = (ushort)(argument + 1);
                                        break;

                                    case uint argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case ulong argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case float argument:
                                        args.Arguments[argumentIndex] = argument + 1.0f;
                                        break;

                                    case double argument:
                                        args.Arguments[argumentIndex] = argument + 1.0;
                                        break;

                                    case decimal argument:
                                        args.Arguments[argumentIndex] = argument + 1.0m;
                                        break;

                                    case char argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument.ToString()) + 1).ToString()[0];
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

                    #region ポインタ引数

                    #region bool

                    [ChangeArguments]
                    private static void 引数を変更(ref bool arg0)
                    {
                    }

                    [Fact]
                    public void bool型_引数1つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;

                        引数を変更(ref arg0);

                        Assert.True(arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref bool arg0, ref bool arg1, ref bool arg2, ref bool arg3, ref bool arg4, ref bool arg5, ref bool arg6, ref bool arg7, ref bool arg8)
                    {
                    }

                    [Fact]
                    public void bool型_引数8つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;
                        bool arg1 = true;
                        bool arg2 = false;
                        bool arg3 = true;
                        bool arg4 = false;
                        bool arg5 = true;
                        bool arg6 = false;
                        bool arg7 = true;
                        bool arg8 = false;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(arg0);
                        Assert.False(arg1);
                        Assert.True(arg2);
                        Assert.False(arg3);
                        Assert.True(arg4);
                        Assert.False(arg5);
                        Assert.True(arg6);
                        Assert.False(arg7);
                        Assert.True(arg8);
                    }

                    #endregion

                    #region sbyte

                    [ChangeArguments]
                    private static void 引数を変更(ref sbyte arg0)
                    {
                    }

                    [Fact]
                    public static void sbyte型_引数1つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref sbyte arg0, ref sbyte arg1, ref sbyte arg2, ref sbyte arg3, ref sbyte arg4, ref sbyte arg5, ref sbyte arg6, ref sbyte arg7, ref sbyte arg8)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数8つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;
                        sbyte arg1 = 1;
                        sbyte arg2 = 2;
                        sbyte arg3 = 3;
                        sbyte arg4 = 4;
                        sbyte arg5 = 5;
                        sbyte arg6 = 6;
                        sbyte arg7 = 7;
                        sbyte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region short

                    [ChangeArguments]
                    private static void 引数を変更(ref short arg0)
                    {
                    }

                    [Fact]
                    public void short型_引数1つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref short arg0, ref short arg1, ref short arg2, ref short arg3, ref short arg4, ref short arg5, ref short arg6, ref short arg7, ref short arg8)
                    {
                    }

                    [Fact]
                    public void short型_引数8つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;
                        short arg1 = 1;
                        short arg2 = 2;
                        short arg3 = 3;
                        short arg4 = 4;
                        short arg5 = 5;
                        short arg6 = 6;
                        short arg7 = 7;
                        short arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region int

                    [ChangeArguments]
                    private static void 引数を変更(ref int arg0)
                    {
                    }

                    [Fact]
                    public void int型_引数1つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref int arg0, ref int arg1, ref int arg2, ref int arg3, ref int arg4, ref int arg5, ref int arg6, ref int arg7, ref int arg8)
                    {
                    }

                    [Fact]
                    public void int型_引数8つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;
                        int arg1 = 1;
                        int arg2 = 2;
                        int arg3 = 3;
                        int arg4 = 4;
                        int arg5 = 5;
                        int arg6 = 6;
                        int arg7 = 7;
                        int arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region long

                    [ChangeArguments]
                    private static void 引数を変更(ref long arg0)
                    {
                    }

                    [Fact]
                    public void long型_引数1つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref long arg0, ref long arg1, ref long arg2, ref long arg3, ref long arg4, ref long arg5, ref long arg6, ref long arg7, ref long arg8)
                    {
                    }

                    [Fact]
                    public void long型_引数8つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;
                        long arg1 = 1;
                        long arg2 = 2;
                        long arg3 = 3;
                        long arg4 = 4;
                        long arg5 = 5;
                        long arg6 = 6;
                        long arg7 = 7;
                        long arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region byte

                    [ChangeArguments]
                    private static void 引数を変更(ref byte arg0)
                    {
                    }

                    [Fact]
                    public void byte型_引数1つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref byte arg0, ref byte arg1, ref byte arg2, ref byte arg3, ref byte arg4, ref byte arg5, ref byte arg6, ref byte arg7, ref byte arg8)
                    {
                    }

                    [Fact]
                    public void byte型_引数8つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;
                        byte arg1 = 1;
                        byte arg2 = 2;
                        byte arg3 = 3;
                        byte arg4 = 4;
                        byte arg5 = 5;
                        byte arg6 = 6;
                        byte arg7 = 7;
                        byte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region ushort

                    [ChangeArguments]
                    private static void 引数を変更(ref ushort arg0)
                    {
                    }

                    [Fact]
                    public void ushort型_引数1つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref ushort arg0, ref ushort arg1, ref ushort arg2, ref ushort arg3, ref ushort arg4, ref ushort arg5, ref ushort arg6, ref ushort arg7, ref ushort arg8)
                    {
                    }

                    [Fact]
                    public void ushort型_引数8つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;
                        ushort arg1 = 1;
                        ushort arg2 = 2;
                        ushort arg3 = 3;
                        ushort arg4 = 4;
                        ushort arg5 = 5;
                        ushort arg6 = 6;
                        ushort arg7 = 7;
                        ushort arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region uint

                    [ChangeArguments]
                    private static void 引数を変更(ref uint arg0)
                    {
                    }

                    [Fact]
                    public void uint型_引数1つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref uint arg0, ref uint arg1, ref uint arg2, ref uint arg3, ref uint arg4, ref uint arg5, ref uint arg6, ref uint arg7, ref uint arg8)
                    {
                    }

                    [Fact]
                    public void uint型_引数8つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;
                        uint arg1 = 1;
                        uint arg2 = 2;
                        uint arg3 = 3;
                        uint arg4 = 4;
                        uint arg5 = 5;
                        uint arg6 = 6;
                        uint arg7 = 7;
                        uint arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region float

                    [ChangeArguments]
                    private static void 引数を変更(ref float arg0)
                    {
                    }

                    [Fact]
                    public void float型_引数1つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref float arg0, ref float arg1, ref float arg2, ref float arg3, ref float arg4, ref float arg5, ref float arg6, ref float arg7, ref float arg8)
                    {
                    }

                    [Fact]
                    public void float型_引数8つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;
                        float arg1 = 1;
                        float arg2 = 2;
                        float arg3 = 3;
                        float arg4 = 4;
                        float arg5 = 5;
                        float arg6 = 6;
                        float arg7 = 7;
                        float arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region double

                    [ChangeArguments]
                    private static void 引数を変更(ref double arg0)
                    {
                    }

                    [Fact]
                    public void double型_引数1つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref double arg0, ref double arg1, ref double arg2, ref double arg3, ref double arg4, ref double arg5, ref double arg6, ref double arg7, ref double arg8)
                    {
                    }

                    [Fact]
                    public void double型_引数8つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;
                        double arg1 = 1;
                        double arg2 = 2;
                        double arg3 = 3;
                        double arg4 = 4;
                        double arg5 = 5;
                        double arg6 = 6;
                        double arg7 = 7;
                        double arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region decimal

                    [ChangeArguments]
                    private static void 引数を変更(ref decimal arg0)
                    {
                    }

                    [Fact]
                    public void decimal型_引数1つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref decimal arg0, ref decimal arg1, ref decimal arg2, ref decimal arg3, ref decimal arg4, ref decimal arg5, ref decimal arg6, ref decimal arg7, ref decimal arg8)
                    {
                    }

                    [Fact]
                    public void decimal型_引数8つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;
                        decimal arg1 = 1;
                        decimal arg2 = 2;
                        decimal arg3 = 3;
                        decimal arg4 = 4;
                        decimal arg5 = 5;
                        decimal arg6 = 6;
                        decimal arg7 = 7;
                        decimal arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region char

                    [ChangeArguments]
                    private static void 引数を変更(ref char arg0)
                    {
                    }

                    [Fact]
                    public void char型_引数1つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';

                        引数を変更(ref arg0);

                        Assert.Equal('1', arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref char arg0, ref char arg1, ref char arg2, ref char arg3, ref char arg4, ref char arg5, ref char arg6, ref char arg7, ref char arg8)
                    {
                    }

                    [Fact]
                    public void char型_引数8つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';
                        char arg1 = '1';
                        char arg2 = '2';
                        char arg3 = '3';
                        char arg4 = '4';
                        char arg5 = '5';
                        char arg6 = '6';
                        char arg7 = '7';
                        char arg8 = '8';

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal('1', arg0);
                        Assert.Equal('2', arg1);
                        Assert.Equal('3', arg2);
                        Assert.Equal('4', arg3);
                        Assert.Equal('5', arg4);
                        Assert.Equal('6', arg5);
                        Assert.Equal('7', arg6);
                        Assert.Equal('8', arg7);
                        Assert.Equal('9', arg8);
                    }

                    #endregion

                    #region string

                    [ChangeArguments]
                    private static void 引数を変更(ref string arg0)
                    {
                    }

                    [Fact]
                    public void string型_引数1つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";

                        引数を変更(ref arg0);

                        Assert.Equal("1", arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref string arg0, ref string arg1, ref string arg2, ref string arg3, ref string arg4, ref string arg5, ref string arg6, ref string arg7, ref string arg8)
                    {
                    }

                    [Fact]
                    public void string型_引数8つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";
                        string arg1 = "1";
                        string arg2 = "2";
                        string arg3 = "3";
                        string arg4 = "4";
                        string arg5 = "5";
                        string arg6 = "6";
                        string arg7 = "7";
                        string arg8 = "8";

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal("1", arg0);
                        Assert.Equal("2", arg1);
                        Assert.Equal("3", arg2);
                        Assert.Equal("4", arg3);
                        Assert.Equal("5", arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal("7", arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal("9", arg8);
                    }

                    #endregion

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
                                    case bool argument:
                                        args.Arguments[argumentIndex] = !argument;
                                        break;

                                    case sbyte argument:
                                        args.Arguments[argumentIndex] = (sbyte)(argument + 1);
                                        break;

                                    case short argument:
                                        args.Arguments[argumentIndex] = (short)(argument + 1);
                                        break;

                                    case int argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case long argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case byte argument:
                                        args.Arguments[argumentIndex] = (byte)(argument + 1);
                                        break;

                                    case ushort argument:
                                        args.Arguments[argumentIndex] = (ushort)(argument + 1);
                                        break;

                                    case uint argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case ulong argument:
                                        args.Arguments[argumentIndex] = argument + 1;
                                        break;

                                    case float argument:
                                        args.Arguments[argumentIndex] = argument + 1.0f;
                                        break;

                                    case double argument:
                                        args.Arguments[argumentIndex] = argument + 1.0;
                                        break;

                                    case decimal argument:
                                        args.Arguments[argumentIndex] = argument + 1.0m;
                                        break;

                                    case char argument:
                                        args.Arguments[argumentIndex] = (int.Parse(argument.ToString()) + 1).ToString()[0];
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

                    #region ポインタ引数

                    #region bool

                    [ChangeArguments]
                    private static void 引数を変更(ref bool arg0)
                    {
                    }

                    [Fact]
                    public void bool型_引数1つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;

                        引数を変更(ref arg0);

                        Assert.True(arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref bool arg0, ref bool arg1, ref bool arg2, ref bool arg3, ref bool arg4, ref bool arg5, ref bool arg6, ref bool arg7, ref bool arg8)
                    {
                    }

                    [Fact]
                    public void bool型_引数8つ_正しくアスペクトが適用される()
                    {
                        bool arg0 = false;
                        bool arg1 = true;
                        bool arg2 = false;
                        bool arg3 = true;
                        bool arg4 = false;
                        bool arg5 = true;
                        bool arg6 = false;
                        bool arg7 = true;
                        bool arg8 = false;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(arg0);
                        Assert.False(arg1);
                        Assert.True(arg2);
                        Assert.False(arg3);
                        Assert.True(arg4);
                        Assert.False(arg5);
                        Assert.True(arg6);
                        Assert.False(arg7);
                        Assert.True(arg8);
                    }

                    #endregion

                    #region sbyte

                    [ChangeArguments]
                    private static void 引数を変更(ref sbyte arg0)
                    {
                    }

                    [Fact]
                    public static void sbyte型_引数1つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref sbyte arg0, ref sbyte arg1, ref sbyte arg2, ref sbyte arg3, ref sbyte arg4, ref sbyte arg5, ref sbyte arg6, ref sbyte arg7, ref sbyte arg8)
                    {
                    }

                    [Fact]
                    public void sbyte型_引数8つ_正しくアスペクトが適用される()
                    {
                        sbyte arg0 = 0;
                        sbyte arg1 = 1;
                        sbyte arg2 = 2;
                        sbyte arg3 = 3;
                        sbyte arg4 = 4;
                        sbyte arg5 = 5;
                        sbyte arg6 = 6;
                        sbyte arg7 = 7;
                        sbyte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region short

                    [ChangeArguments]
                    private static void 引数を変更(ref short arg0)
                    {
                    }

                    [Fact]
                    public void short型_引数1つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref short arg0, ref short arg1, ref short arg2, ref short arg3, ref short arg4, ref short arg5, ref short arg6, ref short arg7, ref short arg8)
                    {
                    }

                    [Fact]
                    public void short型_引数8つ_正しくアスペクトが適用される()
                    {
                        short arg0 = 0;
                        short arg1 = 1;
                        short arg2 = 2;
                        short arg3 = 3;
                        short arg4 = 4;
                        short arg5 = 5;
                        short arg6 = 6;
                        short arg7 = 7;
                        short arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region int

                    [ChangeArguments]
                    private static void 引数を変更(ref int arg0)
                    {
                    }

                    [Fact]
                    public void int型_引数1つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref int arg0, ref int arg1, ref int arg2, ref int arg3, ref int arg4, ref int arg5, ref int arg6, ref int arg7, ref int arg8)
                    {
                    }

                    [Fact]
                    public void int型_引数8つ_正しくアスペクトが適用される()
                    {
                        int arg0 = 0;
                        int arg1 = 1;
                        int arg2 = 2;
                        int arg3 = 3;
                        int arg4 = 4;
                        int arg5 = 5;
                        int arg6 = 6;
                        int arg7 = 7;
                        int arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region long

                    [ChangeArguments]
                    private static void 引数を変更(ref long arg0)
                    {
                    }

                    [Fact]
                    public void long型_引数1つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref long arg0, ref long arg1, ref long arg2, ref long arg3, ref long arg4, ref long arg5, ref long arg6, ref long arg7, ref long arg8)
                    {
                    }

                    [Fact]
                    public void long型_引数8つ_正しくアスペクトが適用される()
                    {
                        long arg0 = 0;
                        long arg1 = 1;
                        long arg2 = 2;
                        long arg3 = 3;
                        long arg4 = 4;
                        long arg5 = 5;
                        long arg6 = 6;
                        long arg7 = 7;
                        long arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region byte

                    [ChangeArguments]
                    private static void 引数を変更(ref byte arg0)
                    {
                    }

                    [Fact]
                    public void byte型_引数1つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref byte arg0, ref byte arg1, ref byte arg2, ref byte arg3, ref byte arg4, ref byte arg5, ref byte arg6, ref byte arg7, ref byte arg8)
                    {
                    }

                    [Fact]
                    public void byte型_引数8つ_正しくアスペクトが適用される()
                    {
                        byte arg0 = 0;
                        byte arg1 = 1;
                        byte arg2 = 2;
                        byte arg3 = 3;
                        byte arg4 = 4;
                        byte arg5 = 5;
                        byte arg6 = 6;
                        byte arg7 = 7;
                        byte arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region ushort

                    [ChangeArguments]
                    private static void 引数を変更(ref ushort arg0)
                    {
                    }

                    [Fact]
                    public void ushort型_引数1つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref ushort arg0, ref ushort arg1, ref ushort arg2, ref ushort arg3, ref ushort arg4, ref ushort arg5, ref ushort arg6, ref ushort arg7, ref ushort arg8)
                    {
                    }

                    [Fact]
                    public void ushort型_引数8つ_正しくアスペクトが適用される()
                    {
                        ushort arg0 = 0;
                        ushort arg1 = 1;
                        ushort arg2 = 2;
                        ushort arg3 = 3;
                        ushort arg4 = 4;
                        ushort arg5 = 5;
                        ushort arg6 = 6;
                        ushort arg7 = 7;
                        ushort arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region uint

                    [ChangeArguments]
                    private static void 引数を変更(ref uint arg0)
                    {
                    }

                    [Fact]
                    public void uint型_引数1つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref uint arg0, ref uint arg1, ref uint arg2, ref uint arg3, ref uint arg4, ref uint arg5, ref uint arg6, ref uint arg7, ref uint arg8)
                    {
                    }

                    [Fact]
                    public void uint型_引数8つ_正しくアスペクトが適用される()
                    {
                        uint arg0 = 0;
                        uint arg1 = 1;
                        uint arg2 = 2;
                        uint arg3 = 3;
                        uint arg4 = 4;
                        uint arg5 = 5;
                        uint arg6 = 6;
                        uint arg7 = 7;
                        uint arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region float

                    [ChangeArguments]
                    private static void 引数を変更(ref float arg0)
                    {
                    }

                    [Fact]
                    public void float型_引数1つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.True(1 == arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref float arg0, ref float arg1, ref float arg2, ref float arg3, ref float arg4, ref float arg5, ref float arg6, ref float arg7, ref float arg8)
                    {
                    }

                    [Fact]
                    public void float型_引数8つ_正しくアスペクトが適用される()
                    {
                        float arg0 = 0;
                        float arg1 = 1;
                        float arg2 = 2;
                        float arg3 = 3;
                        float arg4 = 4;
                        float arg5 = 5;
                        float arg6 = 6;
                        float arg7 = 7;
                        float arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.True(1 == arg0);
                        Assert.True(2 == arg1);
                        Assert.True(3 == arg2);
                        Assert.True(4 == arg3);
                        Assert.True(5 == arg4);
                        Assert.True(6 == arg5);
                        Assert.True(7 == arg6);
                        Assert.True(8 == arg7);
                        Assert.True(9 == arg8);
                    }

                    #endregion

                    #region double

                    [ChangeArguments]
                    private static void 引数を変更(ref double arg0)
                    {
                    }

                    [Fact]
                    public void double型_引数1つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref double arg0, ref double arg1, ref double arg2, ref double arg3, ref double arg4, ref double arg5, ref double arg6, ref double arg7, ref double arg8)
                    {
                    }

                    [Fact]
                    public void double型_引数8つ_正しくアスペクトが適用される()
                    {
                        double arg0 = 0;
                        double arg1 = 1;
                        double arg2 = 2;
                        double arg3 = 3;
                        double arg4 = 4;
                        double arg5 = 5;
                        double arg6 = 6;
                        double arg7 = 7;
                        double arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region decimal

                    [ChangeArguments]
                    private static void 引数を変更(ref decimal arg0)
                    {
                    }

                    [Fact]
                    public void decimal型_引数1つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;

                        引数を変更(ref arg0);

                        Assert.Equal(1, arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref decimal arg0, ref decimal arg1, ref decimal arg2, ref decimal arg3, ref decimal arg4, ref decimal arg5, ref decimal arg6, ref decimal arg7, ref decimal arg8)
                    {
                    }

                    [Fact]
                    public void decimal型_引数8つ_正しくアスペクトが適用される()
                    {
                        decimal arg0 = 0;
                        decimal arg1 = 1;
                        decimal arg2 = 2;
                        decimal arg3 = 3;
                        decimal arg4 = 4;
                        decimal arg5 = 5;
                        decimal arg6 = 6;
                        decimal arg7 = 7;
                        decimal arg8 = 8;

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal(1, arg0);
                        Assert.Equal(2, arg1);
                        Assert.Equal(3, arg2);
                        Assert.Equal(4, arg3);
                        Assert.Equal(5, arg4);
                        Assert.Equal(6, arg5);
                        Assert.Equal(7, arg6);
                        Assert.Equal(8, arg7);
                        Assert.Equal(9, arg8);
                    }

                    #endregion

                    #region char

                    [ChangeArguments]
                    private static void 引数を変更(ref char arg0)
                    {
                    }

                    [Fact]
                    public void char型_引数1つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';

                        引数を変更(ref arg0);

                        Assert.Equal('1', arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref char arg0, ref char arg1, ref char arg2, ref char arg3, ref char arg4, ref char arg5, ref char arg6, ref char arg7, ref char arg8)
                    {
                    }

                    [Fact]
                    public void char型_引数8つ_正しくアスペクトが適用される()
                    {
                        char arg0 = '0';
                        char arg1 = '1';
                        char arg2 = '2';
                        char arg3 = '3';
                        char arg4 = '4';
                        char arg5 = '5';
                        char arg6 = '6';
                        char arg7 = '7';
                        char arg8 = '8';

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal('1', arg0);
                        Assert.Equal('2', arg1);
                        Assert.Equal('3', arg2);
                        Assert.Equal('4', arg3);
                        Assert.Equal('5', arg4);
                        Assert.Equal('6', arg5);
                        Assert.Equal('7', arg6);
                        Assert.Equal('8', arg7);
                        Assert.Equal('9', arg8);
                    }

                    #endregion

                    #region string

                    [ChangeArguments]
                    private static void 引数を変更(ref string arg0)
                    {
                    }

                    [Fact]
                    public void string型_引数1つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";

                        引数を変更(ref arg0);

                        Assert.Equal("1", arg0);
                    }

                    [ChangeArguments]
                    private static void 引数を変更(ref string arg0, ref string arg1, ref string arg2, ref string arg3, ref string arg4, ref string arg5, ref string arg6, ref string arg7, ref string arg8)
                    {
                    }

                    [Fact]
                    public void string型_引数8つ_正しくアスペクトが適用される()
                    {
                        string arg0 = "0";
                        string arg1 = "1";
                        string arg2 = "2";
                        string arg3 = "3";
                        string arg4 = "4";
                        string arg5 = "5";
                        string arg6 = "6";
                        string arg7 = "7";
                        string arg8 = "8";

                        引数を変更(ref arg0, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5, ref arg6, ref arg7, ref arg8);

                        Assert.Equal("1", arg0);
                        Assert.Equal("2", arg1);
                        Assert.Equal("3", arg2);
                        Assert.Equal("4", arg3);
                        Assert.Equal("5", arg4);
                        Assert.Equal("6", arg5);
                        Assert.Equal("7", arg6);
                        Assert.Equal("8", arg7);
                        Assert.Equal("9", arg8);
                    }

                    #endregion

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
            [Flags]
            private enum EventLoggerFlags
            {
                None = 0,
                ProceedAsync = 1 << 0,
                InvokeAsync = 1 << 1,
                Rethrow = 1 << 2,
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
                        if ((Flags & EventLoggerFlags.InvokeAsync) == EventLoggerFlags.InvokeAsync)
                        {
                            await args.InvokeAsync(args.Arguments);
                            args.ReturnValue = args.TaskResult;
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

            public class ProcessAsync
            {
                public class イベントハンドラーの呼びだし順序
                {
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
                        Assert.Equal("8", result7);
                        Assert.Equal(9, result8);
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

                public class 仮想関数
                {
                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                        {
                            await args.ProceedAsync();

                            var returnValue = args.ReturnValue as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    private abstract class Base
                    {
                        public abstract Task<string> 戻り値を大文字に変更(string arg);
                    }

                    private class Derived : Base
                    {
                        [ToUpper]
                        public override async Task<string> 戻り値を大文字に変更(string arg)
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
                        var task = new Derived().戻り値を大文字に変更("a");
                        task.Wait();
                        var result = task.Result;

                        Assert.Equal("A", result);
                    }
                }
            }

            public class InvokeAsync
            {
                public class イベントハンドラーの呼びだし順序
                {
                    #region 戻り値なし

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync | EventLoggerFlags.Rethrow)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync | EventLoggerFlags.Rethrow)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync | EventLoggerFlags.Rethrow)]
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

                    [EventLogger(EventLoggerFlags.InvokeAsync)]
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

                            await args.InvokeAsync(args.Arguments);
                            args.ReturnValue = args.TaskResult;
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
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

                        Assert.Equal(1, result0);
                        Assert.Equal("2", result1);
                        Assert.Equal(3, result2);
                        Assert.Equal("4", result3);
                        Assert.Equal(5, result4);
                        Assert.Equal("6", result5);
                        Assert.Equal(7, result6);
                        Assert.Equal("8", result7);
                        Assert.Equal(9, result8);
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
                            await args.InvokeAsync(args.Arguments);

                            var returnValue = args.TaskResult as string;
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
                            await args.InvokeAsync(args.Arguments);

                            var returnValue = (int)args.TaskResult;
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

                public class 仮想関数
                {
                    private class ToUpper : MethodInterceptionAspect
                    {
                        public override async Task OnInvokeAsync(MethodInterceptionArgs args)
                        {
                            await args.InvokeAsync(args.Arguments);

                            var returnValue = args.TaskResult as string;
                            args.ReturnValue = returnValue.ToUpper();
                        }
                    }

                    private abstract class Base
                    {
                        public abstract Task<string> 戻り値を大文字に変更(string arg);
                    }

                    private class Derived : Base
                    {
                        [ToUpper]
                        public override async Task<string> 戻り値を大文字に変更(string arg)
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
                        var task = new Derived().戻り値を大文字に変更("a");
                        task.Wait();
                        var result = task.Result;

                        Assert.Equal("A", result);
                    }
                }
            }

            public class None
            {
                public class イベントハンドラーの呼びだし順序
                {
                    #region 戻り値なし

                    [EventLogger(EventLoggerFlags.None)]
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

                        戻り値なし().Wait();
                        Logger.Trace("1");

                        Assert.Equal($"OnEntry OnExit 1 ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.Rethrow)]
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

                        戻り値なし_例外を再送出する().Wait();
                        Logger.Trace("1");

                        Assert.Equal($"OnEntry OnExit 1 ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.None)]
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

                        戻り値なし_例外を再送出しない().Wait();
                        Logger.Trace("1");

                        Assert.Equal($"OnEntry OnExit 1 ", appender.ToString());
                    }

                    #endregion

                    #region 値型を戻す

                    [EventLogger(EventLoggerFlags.None)]
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
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry OnExit 0 ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.Rethrow)]
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
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry OnExit 0 ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.None)]
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
                        Logger.Trace(task.Result.ToString());

                        Assert.Equal($"OnEntry OnExit 0 ", appender.ToString());
                    }

                    #endregion

                    #region 値型を戻す

                    [EventLogger(EventLoggerFlags.None)]
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

                        Assert.Null(task.Result);
                        Assert.Equal($"OnEntry OnExit ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.Rethrow)]
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

                        Assert.Null(task.Result);
                        Assert.Equal($"OnEntry OnExit ", appender.ToString());
                    }

                    [EventLogger(EventLoggerFlags.None)]
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

                        Assert.Null(task.Result);
                        Assert.Equal($"OnEntry OnExit ", appender.ToString());
                    }

                    #endregion
                }
            }
        }
    }
}
