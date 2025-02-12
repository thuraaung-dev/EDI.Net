#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

/* Unmerged change from project 'indice.Edi (netstandard1.3)'
Before:
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using System.Globalization;
After:
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
*/

/* Unmerged change from project 'indice.Edi (netstandard1.0)'
Before:
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using System.Globalization;
After:
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
*/

/* Unmerged change from project 'indice.Edi (netstandard2.0)'
Before:
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using System.Globalization;
After:
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
*/

/* Unmerged change from project 'indice.Edi (net5.0)'
Before:
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using System.Globalization;
After:
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
*/
using System.Linq;
using System.Reflection;

namespace indice.Edi.Utilities
{
    internal static class CollectionUtils
    {
        /// <summary>
        /// Determines whether the collection is null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// 	<c>true</c> if the collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(ICollection<T> collection) {
            if (collection != null) {
                return (collection.Count == 0);
            }
            return true;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the specified generic IList.
        /// </summary>
        /// <param name="initial">The list to add to.</param>
        /// <param name="collection">The collection of elements to add.</param>
        public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection) {
            if (initial == null) {
                throw new ArgumentNullException(nameof(initial));
            }

            if (collection == null) {
                return;
            }

            foreach (var value in collection) {
                initial.Add(value);
            }
        }

#if (NET20 || NET35 || PORTABLE40)
        public static void AddRange<T>(this IList<T> initial, IEnumerable collection)
        {
            ValidationUtils.ArgumentNotNull(initial, "initial");

            // because earlier versions of .NET didn't support covariant generics
            initial.AddRange(collection.Cast<T>());
        }
#endif

        public static bool IsDictionaryType(Type type) {
            ValidationUtils.ArgumentNotNull(type, "type");

            if (typeof(IDictionary).IsAssignableFrom(type)) {
                return true;
            }

            if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<,>))) {
                return true;
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IReadOnlyDictionary<,>))) {
                return true;
            }
#endif

            return false;
        }

        public static bool IsCollectionType(this Type type) {
            ValidationUtils.ArgumentNotNull(type, "type");

            if (typeof(IList).IsAssignableFrom(type)) {
                return true;
            }

            if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IList<>))) {
                return true;
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IReadOnlyList<>))) {
                return true;
            }
#endif

            return false;
        }

        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType) {
            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(collectionItemType);
            ConstructorInfo match = null;

            foreach (var constructor in collectionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)) {
                IList<ParameterInfo> parameters = constructor.GetParameters();

                if (parameters.Count == 1) {
                    if (genericEnumerable == parameters[0].ParameterType) {
                        // exact match
                        match = constructor;
                        break;
                    }

                    // incase we can't find an exact match, use first inexact
                    if (match == null) {
                        if (genericEnumerable.IsAssignableFrom(parameters[0].ParameterType)) {
                            match = constructor;
                        }
                    }
                }
            }

            return match;
        }

        public static bool AddDistinct<T>(this IList<T> list, T value) {
            return list.AddDistinct(value, EqualityComparer<T>.Default);
        }

        public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer) {
            if (list.ContainsValue(value, comparer)) {
                return false;
            }

            list.Add(value);
            return true;
        }

        // this is here because LINQ Bridge doesn't support Contains with IEqualityComparer<T>
        public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer) {
            if (comparer == null) {
                comparer = EqualityComparer<TSource>.Default;
            }

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var local in source) {
                if (comparer.Equals(local, value)) {
                    return true;
                }
            }

            return false;
        }

        public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer) {
            var allAdded = true;
            foreach (var value in values) {
                if (!list.AddDistinct(value, comparer)) {
                    allAdded = false;
                }
            }

            return allAdded;
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate) {
            var index = 0;
            foreach (var value in collection) {
                if (predicate(value)) {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static bool Contains(this IEnumerable list, object value, IEqualityComparer comparer) {
            foreach (var item in list) {
                if (comparer.Equals(item, value)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer{TSource}.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="list">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, �1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer) {
            var index = 0;
            foreach (var item in list) {
                if (comparer.Equals(item, value)) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        private static IList<int> GetDimensions(IList values, int dimensionsCount) {
            IList<int> dimensions = new List<int>();

            var currentArray = values;
            while (true) {
                dimensions.Add(currentArray.Count);

                // don't keep calculating dimensions for arrays inside the value array
                if (dimensions.Count == dimensionsCount) {
                    break;
                }

                if (currentArray.Count == 0) {
                    break;
                }

                var v = currentArray[0];
                if (v is IList) {
                    currentArray = (IList)v;
                } else {
                    break;
                }
            }

            return dimensions;
        }

        private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices) {
            var dimension = indices.Length;
            if (dimension == multidimensionalArray.Rank) {
                multidimensionalArray.SetValue(JaggedArrayGetValue(values, indices), indices);
                return;
            }

            var dimensionLength = multidimensionalArray.GetLength(dimension);
            var list = (IList)JaggedArrayGetValue(values, indices);
            var currentValuesLength = list.Count;
            if (currentValuesLength != dimensionLength) {
                throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
            }

            var newIndices = new int[dimension + 1];
            for (var i = 0; i < dimension; i++) {
                newIndices[i] = indices[i];
            }

            for (var i = 0; i < multidimensionalArray.GetLength(dimension); i++) {
                newIndices[dimension] = i;
                CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, newIndices);
            }
        }

        private static object JaggedArrayGetValue(IList values, int[] indices) {
            var currentList = values;
            for (var i = 0; i < indices.Length; i++) {
                var index = indices[i];
                if (i == indices.Length - 1) {
                    return currentList[index];
                } else {
                    currentList = (IList)currentList[index];
                }
            }
            return currentList;
        }

        public static Array ToMultidimensionalArray(IList values, Type type, int rank) {
            var dimensions = GetDimensions(values, rank);

            while (dimensions.Count < rank) {
                dimensions.Add(0);
            }

            var multidimensionalArray = Array.CreateInstance(type, dimensions.ToArray());
            CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, new int[0]);

            return multidimensionalArray;
        }
    }
}