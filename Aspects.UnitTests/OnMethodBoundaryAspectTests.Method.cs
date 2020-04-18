using SoftCube.Logging;
using System;
using System.Reflection;
using Xunit;

namespace SoftCube.Aspects.OnMethodBoundaryAspectTests.Method
{
    public class 呼びだし順序
    {
        internal class EventSpy : OnMethodBoundaryAspect
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

        [EventSpy]
        private void 正常()
        {
            Logger.Trace("A");
        }

        [EventSpy]
        private void 例外()
        {
            Logger.Trace("A");
            throw new InvalidOperationException();
        }

        [Fact]
        public void 正常_呼びだし順序が正しい()
        {
            var appender = TestUtility.CreateAppender();

            正常();

            Assert.Equal($"OnEntry A OnSuccess OnExit ", appender.ToString());
        }

        [Fact]
        public void 例外_呼びだし順序が正しい()
        {
            var appender = TestUtility.CreateAppender();

            var exception = Record.Exception(() => 例外());

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal($"OnEntry A OnException OnExit ", appender.ToString());
        }
    }

    public class Instanceプロパティ
    {
        private static object instance;

        private class InstancePropertySpy : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                instance = args.Instance;
            }
        }

        [InstancePropertySpy]
        private void メソッド()
        {
        }

        [Fact]
        public void Instanceプロパティが正しい()
        {
            メソッド();

            Assert.Same(this, instance);
        }
    }

    public class Methodプロパティ
    {
        private static MethodBase method;

        private class MethodPropertySpy : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                method = args.Method;
            }
        }

        [MethodPropertySpy]
        private void メソッド()
        {
        }

        [Fact]
        public void Methodプロパティが正しい()
        {
            メソッド();

            Assert.Equal("メソッド", method.Name);
        }
    }

    public class 引数を変更
    {
        private class ChangeArguments : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
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
            }
        }

        #region 引数8つ

        [ChangeArguments]
        private (int, string, int, string) 引数8つ(int arg0, string arg1, in int arg2, in string arg3, ref int arg4, ref string arg5, out int arg6, out string arg7)
        {
            arg6 = 7;
            arg7 = "8";
            return (arg0, arg1, arg2, arg3);
        }

        [Fact]
        public void 引数8つ_変更結果が正しい()
        {
            var arg0 = 0;
            var arg1 = "1";
            var arg2 = 2;
            var arg3 = "3";
            var arg4 = 4;
            var arg5 = "5";
            int arg6;
            string arg7;

            var (result0, result1, result2, result3) = 引数8つ(arg0, arg1, arg2, arg3, ref arg4, ref arg5, out arg6, out arg7);

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

        #region ポインタ引数

        #region bool

        [ChangeArguments]
        private void bool型のポインタ引数(ref bool arg0)
        {
        }

        [Fact]
        public void bool型のポインタ引数_変更結果が正しい()
        {
            bool arg0 = false;

            bool型のポインタ引数(ref arg0);

            Assert.True(arg0);
        }

        #endregion

        #region sbyte

        [ChangeArguments]
        private void sbyte型のポインタ引数(ref sbyte arg0)
        {
        }

        [Fact]
        public void sbyte型のポインタ引数_変更結果が正しい()
        {
            sbyte arg0 = 0;

            sbyte型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region short

        [ChangeArguments]
        private void short型のポインタ引数(ref short arg0)
        {
        }

        [Fact]
        public void short型のポインタ引数_変更結果が正しい()
        {
            short arg0 = 0;

            short型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region int

        [ChangeArguments]
        private void int型のポインタ引数(ref int arg0)
        {
        }

        [Fact]
        public void int型のポインタ引数_変更結果が正しい()
        {
            int arg0 = 0;

            int型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region long

        [ChangeArguments]
        private void long型のポインタ引数(ref long arg0)
        {
        }

        [Fact]
        public void long型のポインタ引数_変更結果が正しい()
        {
            long arg0 = 0;

            long型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region byte

        [ChangeArguments]
        private void byte型のポインタ引数(ref byte arg0)
        {
        }

        [Fact]
        public void byte型のポインタ引数_変更結果が正しい()
        {
            byte arg0 = 0;

            byte型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region ushort

        [ChangeArguments]
        private void ushort型のポインタ引数(ref ushort arg0)
        {
        }

        [Fact]
        public void ushort型のポインタ引数_変更結果が正しい()
        {
            ushort arg0 = 0;

            ushort型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region uint

        [ChangeArguments]
        private void uint型のポインタ引数(ref uint arg0)
        {
        }

        [Fact]
        public void uint型のポインタ引数_変更結果が正しい()
        {
            uint arg0 = 0;

            uint型のポインタ引数(ref arg0);

            Assert.True(1 == arg0);
        }

        #endregion

        #region float

        [ChangeArguments]
        private void float型のポインタ引数(ref float arg0)
        {
        }

        [Fact]
        public void float型のポインタ引数_変更結果が正しい()
        {
            float arg0 = 0;

            float型のポインタ引数(ref arg0);

            Assert.True(1 == arg0);
        }

        #endregion

        #region double

        [ChangeArguments]
        private void double型のポインタ引数(ref double arg0)
        {
        }

        [Fact]
        public void double型のポインタ引数_変更結果が正しい()
        {
            double arg0 = 0;

            double型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region decimal

        [ChangeArguments]
        private void decimal型のポインタ引数(ref decimal arg0)
        {
        }

        [Fact]
        public void decimal型のポインタ引数_変更結果が正しい()
        {
            decimal arg0 = 0;

            decimal型のポインタ引数(ref arg0);

            Assert.Equal(1, arg0);
        }

        #endregion

        #region char

        [ChangeArguments]
        private void char型のポインタ引数(ref char arg0)
        {
        }

        [Fact]
        public void char型のポインタ引数_変更結果が正しい()
        {
            char arg0 = '0';

            char型のポインタ引数(ref arg0);

            Assert.Equal('1', arg0);
        }

        #endregion

        #region string

        [ChangeArguments]
        private void string型のポインタ引数(ref string arg0)
        {
        }

        [Fact]
        public void string型のポインタ引数_変更結果が正しい()
        {
            string arg0 = "0";

            string型のポインタ引数(ref arg0);

            Assert.Equal("1", arg0);
        }

        #endregion

        #endregion
    }

    public class 戻り値を変更
    {
        private class ChangeReturnValue : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                switch (args.ReturnValue)
                {
                    case string returnValue:
                        args.ReturnValue = (int.Parse(returnValue) + 1).ToString();
                        break;

                    case int returnValue:
                        args.ReturnValue = returnValue + 1;
                        break;
                }
            }
        }

        #region 参照型

        [ChangeReturnValue]
        private string 参照型の戻り値(string arg1)
        {
            return arg1;
        }

        [Fact]
        public void 参照型の戻り値_変更結果が正しい()
        {
            var result = 参照型の戻り値("1");

            Assert.Equal("2", result);
        }

        #endregion

        #region 値型

        [ChangeReturnValue]
        private int 値型の戻り値(int arg1)
        {
            return arg1;
        }

        [Fact]
        public void 値型の戻り値_変更結果が正しい()
        {
            var result = 値型の戻り値(1);
            Assert.Equal(2, result);
        }

        #endregion
    }

    public class 仮想関数
    {
        private class ChangeReturnValue : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                switch (args.ReturnValue)
                {
                    case string returnValue:
                        args.ReturnValue = (int.Parse(returnValue) + 1).ToString();
                        break;

                    case int returnValue:
                        args.ReturnValue = returnValue + 1;
                        break;
                }
            }
        }

        private abstract class Base
        {
            public abstract string 参照型の戻り値(string arg);
        }

        private class Derived : Base
        {
            [ChangeReturnValue]
            public override string 参照型の戻り値(string arg)
            {
                return arg;
            }
        }

        [Fact]
        public void 参照型の戻り値_変更結果が正しい()
        {
            var result = new Derived().参照型の戻り値("1");
            Assert.Equal("2", result);
        }
    }

    public class マルチキャスト
    {
        //public sealed class Trace : OnMethodBoundaryAspect
        //{
        //    public string Category { get; set; }

        //    public override void OnEntry(MethodExecutionArgs args)
        //    {
        //        Logger.Trace($"{args.Method.Name} {Category}");
        //    }
        //}

        //[Trace(Category = "A")]
        //[Trace(AttributeTargetTypes = "SoftCube.Aspects.OnMethodBoundaryAspectTests.Method.My*", AttributeTargetMemberAttributes = MulticastAttributes.Public, Category = "B")]
        //public class MyClass
        //{
        //    // This method will have 1 Trace aspect with Category set to A.
        //    public void Method1()
        //    {
        //    }

        //    // This method will have 2 Trace aspects with Category set to A, B
        //    public void Method2()
        //    {
        //    }

        //    // This method will have 3 Trace aspects with Category set to A, B, C.
        //    [Trace(Category = "C")]
        //    public void Method3()
        //    {
        //    }
        //}
    }
}
