// ignore spelling: implementaciones

using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Extensions
{
    public static class IEnumerableExtensions
    {
#if !NET6_0_OR_GREATER
        /* Estas funciones se definen para versiones de .net anteriores a .net6.0
         * ya que a partir de .net6.0 ya se encuentran implementadas en el framework
         * y es conveniente usar esas implementaciones.
         * Se copia de https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Distinct.cs
         * License:
         * // Licensed to the .NET Foundation under one or more agreements.
         * // The .NET Foundation licenses this file to you under the MIT license.
         */

        /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains distinct elements from the source sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
        /// <para>The <see cref="DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" /> method returns an unordered sequence that contains no duplicate values. The default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
        /// </remarks>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => DistinctBy(source, keySelector, null);

        /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}" /> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains distinct elements from the source sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
        /// <para>The <see cref="DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)" /> method returns an unordered sequence that contains no duplicate values. If <paramref name="comparer" /> is <see langword="null" />, the default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
        /// </remarks>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return DistinctByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var set = new HashSet<TKey>(comparer);
                    do
                    {
                        TSource element = enumerator.Current;
                        if (set.Add(keySelector(element)))
                        {
                            yield return element;
                        }
                    }
                    while (enumerator.MoveNext());
                }
            }
        }
#endif

        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int tamanioBatch)
        {
            var batch = new List<T>(tamanioBatch);
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == tamanioBatch)
                {
                    yield return batch;
                    batch = new List<T>(tamanioBatch);
                }
            }
            if (batch.Count > 0)
            {
                yield return batch;
            }
        }
    }
}
