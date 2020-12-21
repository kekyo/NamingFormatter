/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.NamingFormatter - String format library with key-valued replacer.
// Copyright (c) 2016-2019 Kouji Matsui (@kekyo2)
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
using System.Globalization;
using System.IO;
using System.Linq;

namespace NamingFormatter
{
    partial class Named
    {
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abcde", 123),
        ///     ("fgh", DateTime.Now),
        ///     ("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// var result = tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            switch (keyValues)
            {
                case (string key, object? value)[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.key, key)).value);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<(string key, object? value)> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.key, key)).value);
                    break;
#endif
                case ICollection<(string key, object? value)> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.key, key)).value);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.key, key)).value);
                    break;
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     ("abcde", 123),
        ///		("fgh", DateTime.Now),
        ///		("ijkl", 456.789));
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abcde", 123),
        ///     ("fgh", DateTime.Now),
        ///     ("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abcde", 123),
        ///     ("fgh", DateTime.Now),
        ///     ("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static string Format(
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static string Format(
            string format,
            Func<string, string, bool> predicate,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());
            return tw.ToString();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abcde", 123),
        ///     new KeyValuePair&lt;string, object&gt;("fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// var result = tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            switch (keyValues)
            {
                case KeyValuePair<string, object?>[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.Key, key)).Value);
                    break;
                case Dictionary<string, object?> dict:
                    WriteFormat(
                        tw,
                        format,
                        key => dict[key]);
                    break;
#if !NET35 && !NET40
                case IReadOnlyDictionary<string, object?> rdict:
                    WriteFormat(
                        tw,
                        format,
                        key => rdict[key]);
                    break;
#endif
                case IDictionary<string, object?> idict:
                    WriteFormat(
                        tw,
                        format,
                        key => idict[key]);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<KeyValuePair<string, object?>> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.Key, key)).Value);
                    break;
#endif
                case ICollection<KeyValuePair<string, object?>> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.Key, key)).Value);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.Key, key)).Value);
                    break;
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abcde", 123),
        ///     new KeyValuePair&lt;string, object&gt;("fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abcde", 123),
        ///     new KeyValuePair&lt;string, object&gt;("fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("ijkl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, string, bool> predicate,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());
            return tw.ToString();
        }
    }
}
