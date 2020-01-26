using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非同期ステートマシン。
    /// </summary>
    public class AsyncStateMachine : StateMachine
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
        /// <param name="aspect">アスペクト。</param>
        /// <param name="targetMethod">ステートマシンのターゲットメソッド。</param>
        public AsyncStateMachine(CustomAttribute aspect, MethodDefinition targetMethod)
            : base(aspect, targetMethod)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="MethodArgs.ReturnValue" に戻り値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="returnVariable">戻り値のローカル変数。</param>
        public void SetReturnValue(ILProcessor processor, Instruction insert, int returnVariable)
        {
            if (TargetMethod.ReturnType is GenericInstanceType genericReturnType)
            {
                var returnType = genericReturnType.GenericArguments[0];

                processor.EmitBefore(insert, OpCodes.Ldarg_0);
                processor.EmitBefore(insert, OpCodes.Ldfld, AspectArgsField);
                processor.EmitBefore(insert, OpCodes.Ldloc, returnVariable);
                if (returnType.IsValueType)
                {
                    processor.EmitBefore(insert, OpCodes.Box, returnType);
                }
                processor.EmitBefore(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetSetMethod()));
            }
        }

        #endregion
    }
}
