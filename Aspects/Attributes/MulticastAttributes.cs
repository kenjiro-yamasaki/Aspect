using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャストカスタム属性 (<see cref="MulticastAttribute"/>) が適用される要素の属性。
    /// </summary>
    [Flags]
    public enum MulticastAttributes
    {
        /// <summary>
        /// 親カスタム属性から継承します。
        /// </summary>
        Default = 0,

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
        /// 任意のスコープ。
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
        /// 任意の実装 (マネージドコード、または非マネージドコード)。
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
        /// 任意のリテラル性 (リテラルフィールド、または非リテラルフィールド)。
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
        /// 任意のパラメーター (入力パラメータ、出力パラメーター、または入出力パラメーター)。
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
        /// 任意のコード生成 (コンパイラーにより生成されたコード、またはユーザーにより生成されたコード)。
        /// </summary>
        AnyGeneration = CompilerGenerated | UserGenerated,

        /// <summary>
        /// すべてのメンバー。
        /// </summary>
        All = AnyVisibility | AnyScope | AnyAbstraction | AnyVirtuality | AnyImplementation | AnyLiterality | AnyParameter | AnyGeneration
    }
}
