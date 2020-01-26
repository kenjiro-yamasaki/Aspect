using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// イテレーターステートマシンへの注入。
    /// </summary>
    public class IteratorStateMachineInjector : StateMachineInjector
    {
        #region プロパティ

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");

        #region フィールド

        /// <summary>
        /// Current フィールド。
        /// </summary>
        public FieldDefinition CurrentField => StateMachineType.Fields.Single(f => f.Name == "<>2__current");

        /// <summary>
        /// ExitFlag フィールド。
        /// </summary>
        public FieldDefinition ExitFlagField { get; }

        /// <summary>
        /// IsDisposing フィールド。
        /// </summary>
        public FieldDefinition IsDisposingField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> メソッド。
        /// </summary>
        public MethodDefinition DisposeMethod => StateMachineType.Methods.Single(m => m.Name == "System.IDisposable.Dispose");

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="aspect">アスペクト。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        public IteratorStateMachineInjector(CustomAttribute aspect, MethodDefinition targetMethod)
            : base(aspect, targetMethod)
        {
            ExitFlagField    = CreateField("*exitFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);
            IsDisposingField = CreateField("*isDisposing*", FieldAttributes.Private, Module.TypeSystem.Int32);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="MethodExecutionArgs.YieldValue"/> に値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetYieldValue(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, AspectArgsField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, CurrentField);
            if (CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Box, CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));
        }

        #endregion
    }
}
