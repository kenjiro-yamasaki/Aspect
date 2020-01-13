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
        /// <see cref="Type"/> に変換します。
        /// </summary>
        /// <param name="typeReference"><see cref="TypeReference"/>。</param>
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
