using SoftCube.Log;
using System.Collections.Generic;
using System.Text;

namespace SoftCube.Aspects
{
    public class TestAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Info("OnEntry");

            foreach (var arg in args.Arguments)
            {
                Logger.Trace(arg.ToString());
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Trace("OnSuccess");

            if (args.ReturnValue == null)
            {
                Logger.Trace("null");
                return;
            }
            switch (args.ReturnValue)
            {
                case IReadOnlyList<int> list:
                    var builder = new StringBuilder();
                    builder.Append("[");

                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        builder.Append(item.ToString());

                        if (i != list.Count - 1)
                        {
                            builder.Append(",");
                        }
                    }

                    builder.Append("]");
                    Logger.Trace(builder.ToString());
                    break;

                default:
                    Logger.Trace(args.ReturnValue.ToString());
                    break;
            }
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Logger.Trace("OnException");

            Logger.Trace(args.Exception.Message);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Trace("OnExit");
        }
    }
}
