using Mono.Cecil;
using SoftCube.Asserts;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// ステートマシンへの注入。
    /// </summary>
    public abstract class StateMachineRewriter
    {
        #region プロパティ

        /// <summary>
        /// アスペクト属性。
        /// </summary>
        public CustomAttribute AspectAttribute { get; }

        /// <summary>
        /// アスペクト引数の型。
        /// </summary>
        public Type AspectArgsType { get; }

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module { get; }

        #region ステートマシン

        /// <summary>
        /// ステートマシン属性。
        /// </summary>
        public abstract CustomAttribute StateMachineAttribute { get; }

        /// <summary>
        /// ステートマシンの型。
        /// </summary>
        public TypeDefinition StateMachineType { get; }

        #endregion

        #region フィールド

        /// <summary>
        /// This フィールド。
        /// </summary>
        public FieldDefinition ThisField => StateMachineType.Fields.SingleOrDefault(f => f.Name == "<>4__this");

        /// <summary>
        /// State フィールド。
        /// </summary>
        protected FieldDefinition StateField { get; }

        /// <summary>
        /// ResumeFlag フィールド。
        /// </summary>
        protected FieldDefinition ResumeFlagField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// MoveNext メソッド。
        /// </summary>
        public MethodDefinition MoveNextMethod { get; }

        /// <summary>
        /// オリジナル MoveNext メソッド。
        /// </summary>
        /// <remarks>
        /// MoveNext メソッドの元々のコードをコピーしたメソッド。
        /// </remarks>
        /// <seealso cref="CreateOriginalMoveNextMethod"/>
        public MethodDefinition OriginalMoveNextMethod { get; private set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public StateMachineRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute, Type aspectArgsType)
        {
            AspectAttribute  = aspectAttribute ?? throw new ArgumentNullException(nameof(aspectAttribute));
            AspectArgsType   = aspectArgsType ?? throw new ArgumentNullException(nameof(aspectArgsType));
            TargetMethod     = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module           = TargetMethod.Module;
            StateMachineType = (TypeDefinition)StateMachineAttribute.ConstructorArguments[0].Value;

            StateField       = StateMachineType.Fields.Single(f => f.Name == "<>1__state");
            ResumeFlagField  = CreateField("*resumeFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);

            MoveNextMethod   = StateMachineType.Methods.Single(m => m.Name == "MoveNext");
        }

        #endregion

        #region メソッド

        /// <summary>
        /// オリジナル MoveNext メソッド (MoveNext メソッドの元々のコード) を生成します。
        /// </summary>
        /// <seealso cref="OriginalMoveNextMethod"/>
        public void CreateOriginalMoveNextMethod()
        {
            Assert.Null(OriginalMoveNextMethod);

            var moveNextMethod = MoveNextMethod;

            OriginalMoveNextMethod = new MethodDefinition(moveNextMethod.Name + "<Original>", moveNextMethod.Attributes, moveNextMethod.ReturnType);
            foreach (var parameter in moveNextMethod.Parameters)
            {
                OriginalMoveNextMethod.Parameters.Add(parameter);
            }

            OriginalMoveNextMethod.Body = moveNextMethod.Body;

            foreach (var sequencePoint in moveNextMethod.DebugInformation.SequencePoints)
            {
                OriginalMoveNextMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            StateMachineType.Methods.Add(OriginalMoveNextMethod);
        }

        /// <summary>
        /// フィールドを生成します。
        /// </summary>
        /// <param name="fieldName">フィールドの名前。</param>
        /// <param name="fieldAttributes">フィールドの属性。</param>
        /// <param name="fieldType">フィールドの型。</param>
        /// <returns>フィールド。</returns>
        public FieldDefinition CreateField(string fieldName, FieldAttributes fieldAttributes, TypeReference fieldType)
        {
            var field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
            StateMachineType.Fields.Add(field);
            return field;
        }

        #endregion
    }
}
