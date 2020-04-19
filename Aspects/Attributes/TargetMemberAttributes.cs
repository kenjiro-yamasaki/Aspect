using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャスト属性を適用するメンバー属性。
    /// </summary>
    [Flags]
    public enum TargetMemberAttributes
    {
        /// <summary>
        /// 未定義。
        /// </summary>
        None = 0,

        /// <summary>
        /// private。
        /// </summary>
        Private = 1 << 0,

        /// <summary>
        /// protected。
        /// </summary>
        Protected = 1 << 1,

        /// <summary>
        /// internal。
        /// </summary>
        Internal = 1 << 2,

        /// <summary>
        /// internal かつ protected (現在のアセンブリで定義されている派生型から可視)。
        /// </summary>
        InternalAndProtected = 1 << 3,

        /// <summary>
        /// internal または protected (現在のアセンブリとすべての派生型から可視)。
        /// </summary>
        InternalOrProtected = 1 << 4,

        /// <summary>
        /// public。
        /// </summary>
        Public = 1 << 5,

        /// <summary>
        /// 任意の可視属性 (private、protected、internal、internal かつ protected、internal または protected、または public)。
        /// </summary>
        AnyVisibility = Private | Protected | InternalAndProtected | InternalOrProtected | Public,

        /// <summary>
        /// 静的スコープ。
        /// </summary>
        Static = 1 << 6,

        /// <summary>
        /// インスタンススコープ。
        /// </summary>
        Instance = 1 << 7,

        /// <summary>
        /// 任意のスコープ属性。
        /// </summary>
        AnyScope = Static | Instance,

        /// <summary>
        /// 抽象メソッド。
        /// </summary>
        Abstract = 1 << 8,

        /// <summary>
        /// 非抽象メソッド。
        /// </summary>
        NonAbstract = 1 << 9,

        /// <summary>
        /// 任意の抽象属性 (抽象メソッド、または非抽象メソッド)。
        /// </summary>
        AnyAbstraction = Abstract | NonAbstract,

        /// <summary>
        /// 仮想メソッド。
        /// </summary>
        Virtual = 1 << 10,

        /// <summary>
        /// 非仮想メソッド。
        /// </summary>
        NonVirtual = 1 << 11,

        /// <summary>
        /// 任意の仮想属性 (仮想メソッド、または非仮想メソッド)。
        /// </summary>
        AnyVirtuality = Virtual | NonVirtual,

        /// <summary>
        /// マネージドコード。
        /// </summary>
        Managed = 1 << 12,

        /// <summary>
        /// 非マネージドコード (external または system)。
        /// </summary>
        NonManaged = 1 << 13,

        /// <summary>
        /// 任意の実装属性 (マネージドコード、または非マネージドコード)。
        /// </summary>
        AnyImplementation = Managed | NonManaged,

        /// <summary>
        /// リテラルフィールド。
        /// </summary>
        Literal = 1 << 14,

        /// <summary>
        /// 非リテラルフィールド。
        /// </summary>
        NonLiteral = 1 << 15,

        /// <summary>
        /// 任意のリテラル属性 (リテラルフィールド、または非リテラルフィールド)。
        /// </summary>
        AnyLiterality = Literal | NonLiteral,

        /// <summary>
        /// 入力パラメーター。
        /// </summary>
        InParameter = 1 << 16,

        /// <summary>
        /// 出力パラメーター。
        /// </summary>
        OutParameter = 1 << 17,

        /// <summary>
        /// 入出力パラメーター (C# では ref パラメーター)。
        /// </summary>
        RefParameter = 1 << 18,

        /// <summary>
        /// 任意のパラメーター属性 (入力パラメータ、出力パラメーター、または入出力パラメーター)。
        /// </summary>
        AnyParameter = InParameter | OutParameter | RefParameter,

        /// <summary>
        /// コンパイラーにより生成されたコード。
        /// </summary>
        CompilerGenerated = 1 << 19,

        /// <summary>
        /// ユーザーにより生成されたコード。
        /// </summary>
        UserGenerated = 1 << 20,

        /// <summary>
        /// 任意のコード生成属性 (コンパイラーにより生成されたコード、またはユーザーにより生成されたコード)。
        /// </summary>
        AnyGeneration = CompilerGenerated | UserGenerated,

        /// <summary>
        /// すべてのメンバー。
        /// </summary>
        All = AnyVisibility | AnyScope | AnyAbstraction | AnyVirtuality | AnyImplementation | AnyLiterality | AnyParameter | AnyGeneration
    }

    /// <summary>
    /// <see cref="TargetMemberAttributes"/> の拡張メソッド。
    /// </summary>
    public static class TargetMemberAttributesExtensions
    {
        #region メソッド

        /// <summary>
        /// メソッドにマルチキャスト属性を適用できるかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <param name="method">メソッド。</param>
        /// <returns>メソッドにマルチキャスト属性を適用できるか。</returns>
        public static bool CanApply(this TargetMemberAttributes attributes, MethodDefinition method)
        {
            // 可視属性でフィルタリングします。
            if (attributes.HasAnyVisibility())
            {
                if (method.IsPrivate && (attributes & TargetMemberAttributes.Private) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (method.IsFamily && (attributes & TargetMemberAttributes.Protected) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (method.IsAssembly && (attributes & TargetMemberAttributes.Internal) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (method.IsFamilyAndAssembly && (attributes & TargetMemberAttributes.InternalAndProtected) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (method.IsFamilyOrAssembly && (attributes & TargetMemberAttributes.InternalOrProtected) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (method.IsPublic && (attributes & TargetMemberAttributes.Public) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            // スコープ属性でフィルタリングします。
            if (attributes.HasAnyScope())
            {
                if (method.IsStatic && (attributes & TargetMemberAttributes.Static) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (!method.IsStatic && (attributes & TargetMemberAttributes.Instance) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            // 抽象属性でフィルタリングします。
            if (attributes.HasAnyAbstraction())
            {
                if (method.IsAbstract && (attributes & TargetMemberAttributes.Abstract) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (!method.IsAbstract && (attributes & TargetMemberAttributes.NonAbstract) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            // 仮想属性でフィルタリングします。
            if (attributes.HasAnyVirtuality())
            {
                if (method.IsVirtual && (attributes & TargetMemberAttributes.Virtual) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (!method.IsVirtual && (attributes & TargetMemberAttributes.NonVirtual) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            // 実装属性でフィルタリングします。
            if (attributes.HasAnyImplementation())
            {
                if (method.IsManaged && (attributes & TargetMemberAttributes.Managed) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (!method.IsManaged && (attributes & TargetMemberAttributes.NonManaged) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            // コード生成属性でフィルタリングします。
            if (attributes.HasAnyGeneration())
            {
                if (method.IsCompilerControlled && (attributes & TargetMemberAttributes.CompilerGenerated) == TargetMemberAttributes.None)
                {
                    return false;
                }
                if (!method.IsCompilerControlled && (attributes & TargetMemberAttributes.UserGenerated) == TargetMemberAttributes.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 任意の可視属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意の可視属性が指定されているか。</returns>
        private static bool HasAnyVisibility(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyVisibility) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意のスコープ属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意のスコープ属性が指定されているか。</returns>
        private static bool HasAnyScope(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyScope) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意の抽象属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意の抽象属性が指定されているか。</returns>
        private static bool HasAnyAbstraction(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyAbstraction) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意の仮想属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意の仮想属性が指定されているか。</returns>
        private static bool HasAnyVirtuality(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyVirtuality) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意の実装属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意の実装属性が指定されているか。</returns>
        private static bool HasAnyImplementation(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyImplementation) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意のリテラル属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意のリテラル属性が指定されているか。</returns>
        private static bool HasAnyLiterality(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyLiterality) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意のパラメーター属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意のパラメーター属性が指定されているか。</returns>
        private static bool HasAnyParameter(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyParameter) != TargetMemberAttributes.None;

        /// <summary>
        /// 任意のコード生成属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性を適用するメンバー属性。</param>
        /// <returns>任意のコード生成属性が指定されているか。</returns>
        private static bool HasAnyGeneration(this TargetMemberAttributes attributes) => (attributes & TargetMemberAttributes.AnyGeneration) != TargetMemberAttributes.None;

        #endregion
    }
}
