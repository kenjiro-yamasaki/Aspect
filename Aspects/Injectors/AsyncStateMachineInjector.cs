using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非同期ステートマシンへの注入。
    /// </summary>
    public class AsyncStateMachineInjector : StateMachineInjector
    {
        #region プロパティ

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public AsyncStateMachineInjector(MethodDefinition targetMethod, CustomAttribute aspect)
            : base(targetMethod, aspect)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="MethodArgs.ReturnValue"/> に戻り値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="returnVariable">戻り値のローカル変数。</param>
        public void SetReturnValue(ILProcessor processor, Instruction insert, int returnVariable)
        {
            if (TargetMethod.ReturnType is GenericInstanceType genericReturnType)
            {
                var returnType = genericReturnType.GenericArguments[0];

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, AspectArgsField);
                processor.InsertBefore(insert, OpCodes.Ldloc, returnVariable);
                if (returnType.IsValueType)
                {
                    processor.InsertBefore(insert, OpCodes.Box, returnType);
                }
                processor.InsertBefore(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetSetMethod()));
            }
        }

        /// <summary>
        /// 戻り値に <see cref="MethodArgs.ReturnValue"/> を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="returnVariable">戻り値のローカル変数。</param>
        public void SetReturnVariable(ILProcessor processor, Instruction insert, int returnVariable)
        {
            if (TargetMethod.ReturnType is GenericInstanceType genericReturnType)
            {
                var returnType = genericReturnType.GenericArguments[0];

                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, AspectArgsField);
                processor.InsertBefore(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetGetMethod()));
                if (returnType.IsValueType)
                {
                    processor.InsertBefore(insert, OpCodes.Unbox_Any, returnType);
                }
                processor.InsertBefore(insert, OpCodes.Stloc, returnVariable);
            }
        }

        #endregion
    }
}
