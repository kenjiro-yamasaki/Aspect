using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// アドバイス引数の書き換え。
    /// </summary>
    public class AdviceArgsRewriter
    {
        #region プロパティ

        /// <summary>
        /// アスペクト属性。
        /// </summary>
        public CustomAttribute AspectAttribute { get; }

        /// <summary>
        /// アスペクト属性の型。
        /// </summary>
        public TypeDefinition AspectAttributeType => AspectAttribute.AttributeType.Resolve();

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// ターゲットメソッドの宣言型。
        /// </summary>
        public TypeDefinition DeclaringType => TargetMethod.DeclaringType;

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module => TargetMethod.Module;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public AdviceArgsRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
        {
            AspectAttribute = aspectAttribute ?? throw new ArgumentNullException(nameof(aspectAttribute));
            TargetMethod    = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
        }

        #endregion
    }
}
