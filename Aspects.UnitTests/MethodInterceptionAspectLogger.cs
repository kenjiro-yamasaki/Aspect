using SoftCube.Asserts;
using SoftCube.Logging;

namespace SoftCube.Aspects
{
    public class MethodInterceptionAspectLogger : MethodInterceptionAspect
    {
        #region メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            Logger.Trace("OnInvoke");

            /// 引数をログ出力します。
            foreach (var argument in args.Arguments)
            {
                Logger.Trace(ArgumentFormatter.Format(argument));
            }

            args.ReturnValue = args.Invoke(args.Arguments);

            /// 戻り値をログ出力します。
            Logger.Trace(ArgumentFormatter.Format(args.ReturnValue));
        }

        #endregion
    }
}
