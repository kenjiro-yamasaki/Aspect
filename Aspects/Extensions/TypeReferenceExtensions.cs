using Mono.Cecil;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="TypeReference"/> の拡張メソッド。
    /// </summary>
    public static class TypeReferenceExtensions
    {
        #region メソッド

        /// <summary>
        /// TypeReference を Type に変換します。
        /// </summary>
        /// <param name="typeReference">TypeReference。</param>
        /// <returns>Type。</returns>
        internal static Type ToSystemType(this TypeReference typeReference, bool removePointer = false)
        {
            if (typeReference is GenericInstanceType genericInstanceType)
            {
                var elementType   = genericInstanceType.ElementType.ToSystemType();
                var typeArguments = genericInstanceType.GenericArguments.Select(ga => ga.ToSystemType(removePointer : true)).ToArray();

                return  elementType.MakeGenericType(typeArguments);
            }
            else
            {
                var typeFullName = removePointer ? typeReference.FullName.Replace("/", "+").Replace("&", "") : typeReference.FullName.Replace("/", "+");
                {
                    if (Type.GetType(typeFullName) is Type result)
                    {
                        return result;
                    }
                }

                var assembly =  AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.FullName == typeReference.Module.Assembly.FullName);
                if (assembly != null)
                {
                    if (assembly.GetType(typeFullName) is Type result)
                    {
                        return result;
                    }
                }
            }

            throw new NotSupportedException($"型[{typeReference.FullName}]の取得に失敗しました。");
        }

        /// <summary>
        /// Task、Task<> の TypeReference を AsyncTaskMethodBuilder、AsyncTaskMethodBuilder<> の Type に変換します。
        /// </summary>
        /// <param name="taskTypeReference">Task、Task<> の TypeReference。</param>
        /// <returns>AsyncTaskMethodBuilder、AsyncTaskMethodBuilder<> の Type。</returns>
        internal static Type ToAsyncTaskMethodBuilderType(this TypeReference taskTypeReference)
        {
            if (taskTypeReference is GenericInstanceType taskType)
            {
                var returnType = taskType.GenericArguments[0].ToSystemType();
                return typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);
            }
            else
            {
                return typeof(AsyncTaskMethodBuilder);
            }
        }

        #endregion
    }
}
