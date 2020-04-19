using Mono.Cecil;
using System;

namespace SoftCube.Aspects
{
    /// <summary>
    /// マルチキャスト属性。
    /// </summary>
    /// <remarks>
    /// ワイルドカードを使用して複数の要素に適用できるカスタム属性です。
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class MulticastAttribute : Attribute
    {
        #region プロパティ

        /// <summary>
        /// カスタム属性。
        /// </summary>
        public CustomAttribute CustomAttribute { get; set; }

        /// <summary>
        /// アスペクト属性の型。
        /// </summary>
        public TypeDefinition CustomAttributeType => CustomAttribute.AttributeType.Resolve();

        /// <summary>
        /// 属性の優先度。
        /// </summary>
        /// <remarks>
        /// 同じターゲットに複数のインスタンスが定義されている場合に、現在の属性の優先度を指定します (優先度の低い属性は前に処理されます)。
        /// </remarks>
        public int AttributePriority { get; set; }

        /// <summary>
        /// 属性を置き換えるか。
        /// </summary>
        /// <remarks>
        /// ターゲットで見つかった他のインスタンスを、このインスタンスで置き換えるかどうかを決定します。
        /// <c>true</c> の場合、このインスタンスが優先度の低いインスタンスを置き換えます。
        /// <c>false</c> の場合、優先度の低いインスタンスに追加されます。
        /// </remarks>
        public bool AttributeReplace { get; set; }

        /// <summary>
        /// 属性を排除するか。
        /// </summary>
        /// <remarks>
        /// <c>true</c> の場合、このインスタンスおよび優先度の低いインスタンスをすべて排除します。
        /// </remarks>
        public bool AttributeExclude { get; set; }

        /// <summary>
        /// ターゲットの型。
        /// </summary>
        /// <value>
        /// このインスタンスを適用する型をワイルドカードまたは正規表現で指定します。
        /// また <c>null</c> の場合、このインスタンスがすべてのタイプに適用されます。
        /// 正規表現は、<c>regex:</c> プレフィックスで始まる必要があります。
        /// </value>
        /// <remarks>
        /// ワイルドカードまたは正規表現を使用しない限り、型の完全修飾名を指定する必要があります。
        /// ネストされた型は、ドット「.」の代わりにプラス記号「+」で区切られます。
        /// ジェネリック型の場合、バッククォート「`」とその型パラメーター数を最後に追加します。
        /// 指定例 :
        /// ・Namespace.OuterType`1+NestedType`2
        /// ・regex:Namespac.*Nested.*
        /// </remarks>
        public string AttributeTargetTypes { get; set; }

        /// <summary>
        /// ターゲットの要素種類。
        /// </summary>
        public TargetElements TargetElements { get; set; }

        /// <summary>
        /// ターゲットのメンバー属性。
        /// </summary>
        /// <remarks>
        /// <see cref="TargetElements"/> にモジュール、アセンブリ、タイプのみが指定された場合は無視されます。
        /// <see cref="TargetMemberAttributes"/> はマルチパーツフラグです (可視性、スコープ、仮想性などのパーツがあります)。
        /// パーツ内で 1 つでも値を指定すると、その値でターゲットをフィルタリングします。
        /// 指定しない場合、任意の値をターゲットとします。
        /// </remarks>
        public TargetMemberAttributes TargetMemberAttributes { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected MulticastAttribute()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メソッドにマルチキャスト属性を適用できるか判断します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <returns>メソッドにマルチキャスト属性を適用できるか。</returns>
        internal bool CanApply(MethodDefinition method)
        {
            if (!TargetElements.CanApply(method))
            {
                return false;
            }
            if (!TargetMemberAttributes.CanApply(method))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
