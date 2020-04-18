using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャストカスタム属性 (<see cref="MulticastAttribute"/>) を適用できるターゲットの種類。
    /// </summary>
    [Flags]
    public enum MulticastTargets
    {
        /// <summary>
        /// 親カスタム属性から継承します。
        /// </summary>
        Default = 0,

        /// <summary>
        /// クラス。
        /// </summary>
        Class = 1 << 0,

        /// <summary>
        /// 構造体。
        /// </summary>
        Struct = 1 << 1,

        /// <summary>
        /// 列挙型。
        /// </summary>
        Enum = 1 << 2,

        /// <summary>
        /// デリゲート。
        /// </summary>
        Delegate = 1 << 3,

        /// <summary>
        /// インターフェイス。
        /// </summary>
        Interface = 1 << 4,

        /// <summary>
        ///  任意の型 (クラス、構造体、列挙型、デリゲート、またはインターフェース)。
        /// </summary>
        AnyType = Class | Struct | Enum | Delegate | Interface,

        /// <summary>
        /// フィールド。
        /// </summary>
        Field = 1 << 5,

        /// <summary>
        /// メソッド (ただし、コンストラクターは含みません)。
        /// </summary>
        Method = 1 << 6,

        /// <summary>
        /// インスタンスコンストラクター。
        /// </summary>
        InstanceConstructor = 1 << 6,

        /// <summary>
        /// 静的コンストラクター。
        /// </summary>
        StaticConstructor = 1 << 7,

        /// <summary>
        /// プロパティ (ただし、プロパティ内のメソッドは含みません)。
        /// </summary>
        Property = 1 << 8,

        /// <summary>
        /// イベント (ただし、イベント内のメソッドは含みません)。
        /// </summary>
        Event = 1 << 9,

        /// <summary>
        /// 任意のメンバー (フィールド、メソッド、インスタンスコンストラクター、静的コンストラクター、プロパティ、イベント)。
        /// </summary>
        AnyMember = Field | Method | InstanceConstructor | StaticConstructor | Property | Event,

        /// <summary>
        /// アセンブリ。
        /// </summary>
        Assembly = 1 << 10,

        /// <summary>
        /// メソッドまたはプロパティのパラメーター。
        /// </summary>
        Parameter = 1 << 11,

        /// <summary>
        /// メソッドまたはプロパティの戻り値。
        /// </summary>
        ReturnValue = 1 << 12,

        /// <summary>
        /// すべての種類。
        /// </summary>
        All = AnyType | AnyMember | Assembly | Parameter | ReturnValue
    }
}
