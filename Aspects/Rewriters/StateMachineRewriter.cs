using Mono.Cecil;
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
        /// カスタム属性。
        /// </summary>
        public CustomAttribute CustomAttribute { get; }

        /// <summary>
        /// アスペクト属性の型。
        /// </summary>
        public TypeDefinition AspectAttributeType => CustomAttribute.AttributeType.Resolve();

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
        public FieldDefinition ThisField { get; }

        /// <summary>
        /// Method フィールド。
        /// </summary>
        public FieldDefinition MethodField { get; }

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

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="customAttribute">アスペクト属性。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public StateMachineRewriter(MethodDefinition targetMethod, CustomAttribute customAttribute, Type aspectArgsType)
        {
            CustomAttribute  = customAttribute ?? throw new ArgumentNullException(nameof(customAttribute));
            AspectArgsType   = aspectArgsType ?? throw new ArgumentNullException(nameof(aspectArgsType));
            TargetMethod     = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module           = TargetMethod.Module;
            StateMachineType = (TypeDefinition)StateMachineAttribute.ConstructorArguments[0].Value;

            StateField       = StateMachineType.Fields.Single(f => f.Name == "<>1__state");
            ResumeFlagField  = CreateField("*resumeFlag", FieldAttributes.Private, Module.TypeSystem.Boolean);
            MethodField      = CreateField("*method", FieldAttributes.Public, Module.ImportReference(typeof(System.Reflection.MethodBase)), reuseIfExist: true);
            ThisField        = StateMachineType.Fields.SingleOrDefault(f => f.Name == "<>4__this");
            if (ThisField == null && !TargetMethod.IsStatic)
            {
                ThisField = CreateField("<>4__this", FieldAttributes.Public, Module.ImportReference(targetMethod.DeclaringType));
            }

            MoveNextMethod   = StateMachineType.Methods.Single(m => m.Name == "MoveNext");
        }

        #endregion

        #region メソッド

        /// <summary>
        /// フィールドを生成します。
        /// </summary>
        /// <param name="fieldName">フィールドの名前。</param>
        /// <param name="fieldAttributes">フィールドの属性。</param>
        /// <param name="fieldType">フィールドの型。</param>
        /// <returns>フィールド。</returns>
        public FieldDefinition CreateField(string fieldName, FieldAttributes fieldAttributes, TypeReference fieldType, bool reuseIfExist = false)
        {
            var field = StateMachineType.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
            {
                field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
                StateMachineType.Fields.Add(field);
                return field;
            }

            if (reuseIfExist)
            {
                return field;
            }
            else
            {
                for (int number = 2; true; number++)
                {
                    var fieldNameWithNumber = $"{fieldName}{number}";

                    if (!StateMachineType.Fields.Any(f => f.Name == fieldNameWithNumber))
                    {
                        field = new FieldDefinition(fieldNameWithNumber, fieldAttributes, fieldType);
                        StateMachineType.Fields.Add(field);
                        return field;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldDefinition GetField(string fieldName)
        {
            return StateMachineType.Fields.SingleOrDefault(f => f.Name == fieldName);
        }

        #endregion
    }
}
