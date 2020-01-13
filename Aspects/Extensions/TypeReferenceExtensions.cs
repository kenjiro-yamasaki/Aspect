using Mono.Cecil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="TypeReference"/> の拡張メソッド。
    /// </summary>
    public static class TypeReferenceExtensions
    {
        #region メソッド

        /// <summary>
        /// 型参照を <see cref="Type"/> に変換します。
        /// </summary>
        /// <param name="typeReference">型参照。</param>
        /// <returns><see cref="Type"/>。</returns>
        internal static Type ToSystemType(this TypeReference typeReference)
        {
            {
                if (Type.GetType(typeReference.FullName) is Type result)
                {
                    return result;
                }
            }

            var assembly =  AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.FullName == typeReference.Module.Assembly.FullName);
            if (assembly != null)
            {
                if (assembly.GetType(typeReference.FullName) is Type result)
                {
                    return result;
                }
            }

            throw new NotSupportedException();
        }

        #endregion
    }
}
