using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャスト属性を適用する要素。
    /// </summary>
    [Flags]
    public enum TargetElements
    {
        /// <summary>
        /// 未定義。
        /// </summary>
        None = 0,

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
        InstanceConstructor = 1 << 7,

        /// <summary>
        /// 静的コンストラクター。
        /// </summary>
        StaticConstructor = 1 << 8,

        /// <summary>
        /// プロパティ (ただし、プロパティ内のメソッドは含みません)。
        /// </summary>
        Property = 1 << 9,

        /// <summary>
        /// イベント (ただし、イベント内のメソッドは含みません)。
        /// </summary>
        Event = 1 << 10,

        /// <summary>
        /// 任意のメンバー (フィールド、メソッド、インスタンスコンストラクター、静的コンストラクター、プロパティ、イベント)。
        /// </summary>
        AnyMember = Field | Method | InstanceConstructor | StaticConstructor | Property | Event,

        /// <summary>
        /// アセンブリ。
        /// </summary>
        Assembly = 1 << 11,

        /// <summary>
        /// メソッドまたはプロパティのパラメーター。
        /// </summary>
        Parameter = 1 << 12,

        /// <summary>
        /// メソッドまたはプロパティの戻り値。
        /// </summary>
        ReturnValue = 1 << 13,

        /// <summary>
        /// すべての種類。
        /// </summary>
        All = AnyType | AnyMember | Assembly | Parameter | ReturnValue
    }

    /// <summary>
    /// <see cref="TargetElements"/> の拡張メソッド。
    /// </summary>
    public static class TargetElementsExtensions
    {
        #region メソッド

        /// <summary>
        /// 型にマルチキャスト属性を適用できるかを判断します。
        /// </summary>
        /// <param name="elements">マルチキャスト属性を適用する要素。</param>
        /// <param name="type">型。</param>
        /// <returns>型にマルチキャスト属性を適用できるか。</returns>
        public static bool CanApply(this TargetElements elements, TypeDefinition type)
        {
            if (elements.HasAnyType())
            {
                if (!type.IsClass && (elements & TargetElements.Class) == TargetElements.None)
                {
                    return false;
                }
                if (!type.IsEnum && (elements & TargetElements.Enum) == TargetElements.None)
                {
                    return false;
                }
                if (!type.IsValueType && (elements & TargetElements.Struct) == TargetElements.None)
                {
                    return false;
                }
                if (!type.IsInterface && (elements & TargetElements.Interface) == TargetElements.None)
                {
                    return false;
                }

                // TODO: デリゲートに対応します。
            }

            return true;
        }

        /// <summary>
        /// メソッドにマルチキャスト属性を適用できるかを判断します。
        /// </summary>
        /// <param name="elements">マルチキャスト属性を適用する要素。</param>
        /// <param name="method">メソッド。</param>
        /// <returns>メソッドにマルチキャスト属性を適用できるか。</returns>
        public static bool CanApply(this TargetElements elements, MethodDefinition method)
        {
            // メンバーでフィルタリングします。
            if (elements.HasAnyMember())
            {
                if (!method.IsStatic && method.IsConstructor && (elements & TargetElements.InstanceConstructor) == TargetElements.None)
                {
                    return false;
                }
                if (method.IsStatic && method.IsConstructor && (elements & TargetElements.StaticConstructor) == TargetElements.None)
                {
                    return false;
                }
                if ((elements & TargetElements.Method) == TargetElements.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 任意の型が指定されているかを判断します。
        /// </summary>
        /// <param name="elements">マルチキャスト属性を適用する要素。</param>
        /// <returns>任意の型が指定されているか。</returns>
        private static bool HasAnyType(this TargetElements elements) => (elements & TargetElements.AnyType) != TargetElements.None;

        /// <summary>
        /// 任意のメンバーが指定されているかを判断します。
        /// </summary>
        /// <param name="elements">マルチキャスト属性を適用する要素。</param>
        /// <returns>任意のメンバーが指定されているか。</returns>
        private static bool HasAnyMember(this TargetElements elements) => (elements & TargetElements.AnyMember) != TargetElements.None;

        #endregion
    }
}
