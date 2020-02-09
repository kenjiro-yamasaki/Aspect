using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
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

        /// <summary>
        /// Dispose メソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalDisposeMethod { get; private set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public IteratorStateMachineInjector(MethodDefinition targetMethod, CustomAttribute aspect)
            : base(targetMethod, aspect)
        {
            ExitFlagField    = CreateField("*exitFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);
            IsDisposingField = CreateField("*isDisposing*", FieldAttributes.Private, Module.TypeSystem.Int32);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 新たなメソッドを生成し、Dispose メソッドの内容を移動します。
        /// </summary>
        public void ReplaceDispose()
        {
            Assert.NotNull(DisposeMethod);
            Assert.Null(OriginalDisposeMethod);

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            OriginalDisposeMethod = new MethodDefinition(DisposeMethod.Name + "<Original>", DisposeMethod.Attributes, DisposeMethod.ReturnType);
            foreach (var parameter in DisposeMethod.Parameters)
            {
                OriginalDisposeMethod.Parameters.Add(parameter);
            }

            OriginalDisposeMethod.Body = DisposeMethod.Body;
            foreach (var sequencePoint in DisposeMethod.DebugInformation.SequencePoints)
            {
                OriginalDisposeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            StateMachineType.Methods.Add(OriginalDisposeMethod);
        }

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

        /// <summary>
        /// Current フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetCurrentField(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldarg_0);

            processor.Emit(OpCodes.Dup);
            processor.Emit(OpCodes.Ldfld, AspectArgsField);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetGetMethod()));
            if (CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Stfld, CurrentField);
        }

        #endregion
    }
}
