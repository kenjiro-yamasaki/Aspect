using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// アドバイス引数への注入。
    /// </summary>
    public class AdviceArgsInjector
    {
        #region プロパティ

        /// <summary>
        /// アスペクトの属性。
        /// </summary>
        public CustomAttribute Aspect { get; }

        /// <summary>
        /// アスペクトの型。
        /// </summary>
        public TypeDefinition AspectType => Aspect.AttributeType.Resolve();

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module => TargetMethod.Module;

        /// <summary>
        /// ターゲットメソッドの宣言型。
        /// </summary>
        public TypeDefinition DeclaringType => TargetMethod.DeclaringType;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト属性。</param>
        public AdviceArgsInjector(MethodDefinition targetMethod, CustomAttribute aspect)
        {
            Aspect       = aspect ?? throw new ArgumentNullException(nameof(aspect));
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
        }

        #endregion
    }
}
