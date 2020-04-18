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
        /// 任意の可視属性。
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
        /// 具象メソッド (非抽象メソッド)。
        /// </summary>
        NonAbstract = 1 << 9,

        /// <summary>
        /// 任意の抽象性 (抽象メソッド、または非抽象メソッド)。
        /// </summary>
        AnyAbstraction = Abstract | NonAbstract,

        /// <summary>
        /// 仮想メソッド。
        /// </summary>
        Virtual = 2048,

        /// <summary>
        /// 非仮想メソッド。
        /// </summary>
        NonVirtual = 4096,

        //
        // 概要:
        //     Any virtuality (PostSharp.Extensibility.MulticastAttributes.Virtual | PostSharp.Extensibility.MulticastAttributes.NonVirtual).
        AnyVirtuality = 6144,

        //
        // 概要:
        //     Managed code implementation.
        Managed = 8192,
        //
        // 概要:
        //     Non-managed code implementation (external or system).
        NonManaged = 16384,
        //
        // 概要:
        //     Any implementation (PostSharp.Extensibility.MulticastAttributes.Managed | PostSharp.Extensibility.MulticastAttributes.NonManaged).
        AnyImplementation = 24576,
        //
        // 概要:
        //     Literal fields.
        Literal = 32768,
        //
        // 概要:
        //     Non-literal fields.
        NonLiteral = 65536,
        //
        // 概要:
        //     Any field literality (PostSharp.Extensibility.MulticastAttributes.Literal | PostSharp.Extensibility.MulticastAttributes.NonLiteral).
        AnyLiterality = 98304,
        //
        // 概要:
        //     Input parameters.
        InParameter = 131072,
        //
        // 概要:
        //     Compiler-generated code.
        CompilerGenerated = 262144,
        //
        // 概要:
        //     User-generated code (anything expected PostSharp.Extensibility.MulticastAttributes.CompilerGenerated).
        UserGenerated = 524288,
        //
        // 概要:
        //     Any code generation (PostSharp.Extensibility.MulticastAttributes.CompilerGenerated
        //     | PostSharp.Extensibility.MulticastAttributes.UserGenerated)l
        AnyGeneration = 786432,
        //
        // 概要:
        //     Output (out in C#) parameters.
        OutParameter = 1048576,
        //
        // 概要:
        //     Input/Output (ref in C#) parameters.
        RefParameter = 2097152,
        //
        // 概要:
        //     Any kind of parameter passing (PostSharp.Extensibility.MulticastAttributes.InParameter
        //     | PostSharp.Extensibility.MulticastAttributes.OutParameter | PostSharp.Extensibility.MulticastAttributes.RefParameter).
        AnyParameter = 3276800,
        //
        // 概要:
        //     All members.
        All = 4194302
    }
}
