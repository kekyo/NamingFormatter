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
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            if ((keyValues is ICollection<KeyValuePair<string, object>>) == false)
            {
#if !NET35 && !NET40
                if ((keyValues is IReadOnlyCollection<KeyValuePair<string, object>>) == false)
                {
                    var fixedKeyValues = keyValues.ToList();
                    tw.WriteFormat(
                        format,
                        key => fixedKeyValues.First(kv => predicate(kv.Key, key)).Value);
                }
#else
                var fixedKeyValues = keyValues.ToList();
                tw.WriteFormat(
                    format,
                    key => fixedKeyValues.First(kv => predicate(kv.Key, key)).Value);
#endif
                return;
            }

            tw.WriteFormat(
                format,
                key => keyValues.First(kv => predicate(kv.Key, key)).Value);
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
            params KeyValuePair<string, object>[] keyValues)
        {
            tw.WriteFormat(
                format,
                predicate,
                keyValues.AsEnumerable());
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
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            if (formatProvider == null)
            {
                throw new ArgumentNullException("formatProvider");
            }

            var tw = new StringWriter(formatProvider);
            tw.WriteFormat(
                format,
                key => keyValues.First(kv => predicate(kv.Key, key)).Value);
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
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            return Format(
                CultureInfo.CurrentCulture,
                format,
                predicate,
                keyValues);
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
            params KeyValuePair<string, object>[] keyValues)
        {
            return Format(
                formatProvider,
                format,
                predicate,
                keyValues.AsEnumerable());
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
            params KeyValuePair<string, object>[] keyValues)
        {
            return Format(
                format,
                predicate,
                keyValues.AsEnumerable());
        }
    }
}
