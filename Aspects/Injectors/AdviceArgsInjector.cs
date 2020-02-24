using Mono.Cecil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// アドバイス引数への注入。
    /// </summary>
    public class AdviceArgsInjector
    {
        #region プロパティ

        /// <summary>
        /// アスペクト。
        /// </summary>
        public CustomAttribute Aspect { get; }

        /// <summary>
        /// アスペクトの型。
        /// </summary>
        public TypeDefinition AspectType { get; }

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module { get; }

        /// <summary>
        /// ターゲットメソッドの宣言型。
        /// </summary>
        public TypeDefinition DeclaringType { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public AdviceArgsInjector(MethodDefinition targetMethod, CustomAttribute aspect)
        {
            Aspect        = aspect ?? throw new ArgumentNullException(nameof(aspect));
            AspectType    = Aspect.AttributeType.Resolve();
            TargetMethod  = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module        = TargetMethod.Module;
            DeclaringType = TargetMethod.DeclaringType;
        }

        #endregion
    }
}
