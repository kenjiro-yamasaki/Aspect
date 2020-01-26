using Mono.Cecil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// イテレーターステートマシン。
    /// </summary>
    public class IteratorStateMachine : StateMachine
    {
        #region プロパティ

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");

        #region フィールド

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
        public IteratorStateMachine(CustomAttribute aspect, MethodDefinition targetMethod)
            : base(aspect, targetMethod)
        {
            ExitFlagField    = CreateField("*exitFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);
            IsDisposingField = CreateField("*isDisposing*", FieldAttributes.Private, Module.TypeSystem.Int32);
        }

        #endregion
    }
}
