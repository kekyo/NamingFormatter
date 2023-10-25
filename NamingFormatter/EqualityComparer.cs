/////////////////////////////////////////////////////////////////////////////////////////////////
//
// NamingFormatter - String format library with key-valued replacer.
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
using System.IO;
using System.Linq;

#if !NET35 && !NET40
using System.Threading.Tasks;
#endif

namespace NamingFormatter
{
    partial class Named
    {
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
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
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
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
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789));
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
            IEqualityComparer<string> comparer,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789));
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
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     ("abCDe", 123),
        ///     ("Fgh", DateTime.Now),
        ///     ("IjKl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
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
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
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
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
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
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
#if NET35 || NET40
        private
#else
        public
#endif
        static string Format(
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
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
        static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            params (string key, object? value)[] keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
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
        static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
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
        static string Format(
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues);
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value array.
        /// var keyValues = new[]
        /// {
        ///     new KeyValuePair&lt;string, object&gt;("abCDe", 123),
        ///     new KeyValuePair&lt;string, object&gt;("Fgh", DateTime.Now),
        ///     new KeyValuePair&lt;string, object&gt;("IjKl", 456.789),
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format(
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            params KeyValuePair<string, object?>[] keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            Format(
                formatProvider,
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            string format,
            IEqualityComparer<string> comparer,
            params KeyValuePair<string, object?>[] keyValues) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="comparer">format-key equality comparer.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with key ignoring case.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     StringComparer.CurrentCultureIgnoreCase,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            string format,
            IEqualityComparer<string> comparer,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            Format(
                format,
                (key1, key2) => comparer.Equals(key1, key2),
                keyValues.AsEnumerable(),
                fallback);
    }
}
