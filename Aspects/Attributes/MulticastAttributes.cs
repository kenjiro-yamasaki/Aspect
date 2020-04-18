using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャスト属性 (<see cref="MulticastAttribute"/>) が適用される要素属性。
    /// </summary>
    [Flags]
    public enum MulticastAttributes
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
        /// 任意の抽象性 (抽象メソッド、または非抽象メソッド)。
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
        /// 任意の仮想性 (仮想メソッド、または非仮想メソッド)。
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
    /// <see cref="MulticastAttributes"/> の拡張メソッド。
    /// </summary>
    public static class MulticastAttributesExtensions
    {
        #region メソッド

        /// <summary>
        /// メソッドにマルチキャスト属性を適用できるか判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <param name="method">メソッド。</param>
        /// <returns>メソッドにマルチキャスト属性を適用できるか。</returns>
        public static bool CanApply(this MulticastAttributes attributes, MethodDefinition method)
        {
            // 可視属性によりフィルタリングします。
            if (attributes.HasVisibility())
            {
                if (method.IsPrivate && (attributes & MulticastAttributes.Private) == MulticastAttributes.None)
                {
                    return false;
                }
                if (method.IsFamily && (attributes & MulticastAttributes.Protected) == MulticastAttributes.None)
                {
                    return false;
                }
                if (method.IsAssembly && (attributes & MulticastAttributes.Internal) == MulticastAttributes.None)
                {
                    return false;
                }
                if (method.IsFamilyAndAssembly && (attributes & MulticastAttributes.InternalAndProtected) == MulticastAttributes.None)
                {
                    return false;
                }
                if (method.IsFamilyOrAssembly && (attributes & MulticastAttributes.InternalOrProtected) == MulticastAttributes.None)
                {
                    return false;
                }
                if (method.IsPublic && (attributes & MulticastAttributes.Public) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            // スコープ属性によりフィルタリングします。
            if (attributes.HasScope())
            {
                if (method.IsStatic && (attributes & MulticastAttributes.Static) == MulticastAttributes.None)
                {
                    return false;
                }
                if (!method.IsStatic && (attributes & MulticastAttributes.Instance) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            // 抽象性によりフィルタリングします。
            if (attributes.HasAbstraction())
            {
                if (method.IsAbstract && (attributes & MulticastAttributes.Abstract) == MulticastAttributes.None)
                {
                    return false;
                }
                if (!method.IsAbstract && (attributes & MulticastAttributes.NonAbstract) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            // 仮想性によりフィルタリングします。
            if (attributes.HasVirtuality())
            {
                if (method.IsVirtual && (attributes & MulticastAttributes.Virtual) == MulticastAttributes.None)
                {
                    return false;
                }
                if (!method.IsVirtual && (attributes & MulticastAttributes.NonVirtual) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            // 実装属性によりフィルタリングします。
            if (attributes.HasImplementation())
            {
                if (method.IsManaged && (attributes & MulticastAttributes.Managed) == MulticastAttributes.None)
                {
                    return false;
                }
                if (!method.IsManaged && (attributes & MulticastAttributes.NonManaged) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            // コード生成属性によりフィルタリングします。
            if (attributes.HasGeneration())
            {
                if (method.IsCompilerControlled && (attributes & MulticastAttributes.CompilerGenerated) == MulticastAttributes.None)
                {
                    return false;
                }
                if (!method.IsCompilerControlled && (attributes & MulticastAttributes.UserGenerated) == MulticastAttributes.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 可視属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>可視属性が指定されているか。</returns>
        public static bool HasVisibility(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyVisibility) != MulticastAttributes.None;

        /// <summary>
        /// スコープ属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>スコープ属性が指定されているか。</returns>
        public static bool HasScope(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyScope) != MulticastAttributes.None;

        /// <summary>
        /// 抽象性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>抽象性が指定されているか。</returns>
        public static bool HasAbstraction(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyAbstraction) != MulticastAttributes.None;

        /// <summary>
        /// 仮想性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>仮想性が指定されているか。</returns>
        public static bool HasVirtuality(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyVirtuality) != MulticastAttributes.None;

        /// <summary>
        /// 実装属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>実装属性が指定されているか。</returns>
        public static bool HasImplementation(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyImplementation) != MulticastAttributes.None;

        /// <summary>
        /// リテラル属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>リテラル属性が指定されているか。</returns>
        public static bool HasLiterality(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyLiterality) != MulticastAttributes.None;

        /// <summary>
        /// パラメーター属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>パラメーター属性が指定されているか。</returns>
        public static bool HasParameter(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyParameter) != MulticastAttributes.None;

        /// <summary>
        /// コード生成属性が指定されているかを判断します。
        /// </summary>
        /// <param name="attributes">マルチキャスト属性が適用される要素属性。</param>
        /// <returns>コード生成属性が指定されているか。</returns>
        public static bool HasGeneration(this MulticastAttributes attributes) => (attributes & MulticastAttributes.AnyGeneration) != MulticastAttributes.None;

        #endregion
    }
}
