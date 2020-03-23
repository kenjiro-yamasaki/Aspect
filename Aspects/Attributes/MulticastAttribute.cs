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
    public abstract class MulticastAttribute : Attribute
    {
        #region プロパティ。

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
        /// 属性のターゲット型。
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

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected MulticastAttribute()
        {
        }

        #endregion
    }
}
