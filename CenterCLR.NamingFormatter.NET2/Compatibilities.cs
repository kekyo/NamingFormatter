/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.NamingFormatter - String format library with key-valued replacer.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class ExtensionAttribute : Attribute
    {
    }
}

namespace CenterCLR.NamingFormatter
{
    public delegate TR Func<in T0, TR>(T0 arg0);
    public delegate TR Func<in T0, in T1, TR>(T0 arg0, T1 arg1);

    internal static class Enumerable
    {
        public static TValue First<TValue>(this IEnumerable<TValue> enumerable)
        {
            Debug.Assert(enumerable != null);

            using (var enumerator = enumerable.GetEnumerator())
            {
                if (enumerator.MoveNext() == false)
                {
                    throw new InvalidOperationException();
                }

                return enumerator.Current;
            }
        }

        public static TValue First<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, bool> predicate)
        {
            Debug.Assert(enumerable != null);
            Debug.Assert(predicate != null);

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                }

                throw new InvalidOperationException();
            }
        }

        public static List<TValue> ToList<TValue>(this IEnumerable<TValue> enumerable)
        {
            Debug.Assert(enumerable != null);

            return new List<TValue>(enumerable);
        }

        public static IEnumerable<TValue> AsEnumerable<TValue>(this IEnumerable<TValue> enumerable)
        {
            Debug.Assert(enumerable != null);

            return enumerable;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
        {
            Debug.Assert(enumerable != null);
            Debug.Assert(keySelector != null);
            Debug.Assert(valueSelector != null);

            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in enumerable)
            {
                dictionary.Add(keySelector(entry), valueSelector(entry));
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey> comparer)
        {
            Debug.Assert(enumerable != null);
            Debug.Assert(keySelector != null);
            Debug.Assert(valueSelector != null);
            Debug.Assert(comparer != null);

            var dictionary = new Dictionary<TKey, TValue>(comparer);
            foreach (var entry in enumerable)
            {
                dictionary.Add(keySelector(entry), valueSelector(entry));
            }

            return dictionary;
        }
    }
}
