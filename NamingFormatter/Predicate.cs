﻿/////////////////////////////////////////////////////////////////////////////////////////////////
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
using System.Globalization;
using System.IO;
using System.Linq;

#if !NET35 && !NET40
using System.Threading.Tasks;
#endif

using NamingFormatter.Internal;

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
        /// <param name="options">Options</param>
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
            IEnumerable<(string key, object? value)> keyValues,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            switch (keyValues)
            {
                case (string key, object? value)[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.key, key)).value,
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<(string key, object? value)> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.key, key)).value,
                        options);
                    break;
#endif
                case ICollection<(string key, object? value)> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.key, key)).value,
                        options);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.key, key)).value,
                        options);
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
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            switch (keyValues)
            {
                case (string key, object? value)[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => (Array.FindIndex(array, kv => predicate(kv.key, key)) is { } index && index >= 0) ?
                            array[index].value :
                            fallback(key),
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<(string key, object? value)> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => (rcoll.FirstOrDefault(kv => predicate(kv.key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                    break;
#endif
                case ICollection<(string key, object? value)> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => (coll.FirstOrDefault(kv => predicate(kv.key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => (Array.FindIndex(fixedKeyValues, kv => predicate(kv.key, key)) is { } index && index >= 0) ?
                            fixedKeyValues[index].value :
                            fallback(key),
                        options);
                    break;
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
            FormatOptions options,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
        static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
        static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params (string key, object? value)[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
        /// var result = await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            switch (keyValues)
            {
                case (string key, object? value)[] array:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.key, key)).value,
                        options);
                case IReadOnlyCollection<(string key, object? value)> rcoll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.key, key)).value,
                        options);
                case ICollection<(string key, object? value)> coll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.key, key)).value,
                        options);
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.key, key)).value,
                        options);
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
        /// var result = await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            switch (keyValues)
            {
                case (string key, object? value)[] array:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (Array.FindIndex(array, kv => predicate(kv.key, key)) is { } index && index >= 0) ?
                            array[index].value :
                            fallback(key),
                        options);
                case IReadOnlyCollection<(string key, object? value)> rcoll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (rcoll.FirstOrDefault(kv => predicate(kv.key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                case ICollection<(string key, object? value)> coll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (coll.FirstOrDefault(kv => predicate(kv.key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (Array.FindIndex(fixedKeyValues, kv => predicate(kv.key, key)) is { } index && index >= 0) ?
                            fixedKeyValues[index].value :
                            fallback(key),
                        options);
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     ("abcde", 123),
        ///		("fgh", DateTime.Now),
        ///		("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     ("abcde", 123),
        ///		("fgh", DateTime.Now),
        ///		("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            FormatOptions options,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     ("abcde", 123),
        ///		("fgh", DateTime.Now),
        ///		("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     ("abcde", 123),
        ///		("fgh", DateTime.Now),
        ///		("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params (string key, object? value)[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
            IEnumerable<(string key, object? value)> keyValues,
            FormatOptions options = default)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
            IEnumerable<(string key, object? value)> keyValues,
            FormatOptions options = default)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
            Func<string, string, bool> predicate,
            IEnumerable<(string key, object? value)> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
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
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
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
        static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            FormatOptions options,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
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

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
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
        static string Format(
            string format,
            Func<string, string, bool> predicate,
            FormatOptions options,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
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
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params (string key, object? value)[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
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
        /// <param name="options">Options</param>
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
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            switch (keyValues)
            {
                case KeyValuePair<string, object?>[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.Key, key)).Value,
                        options);
                    break;
                case Dictionary<string, object?> dict:
                    WriteFormat(
                        tw,
                        format,
                        key => dict[key],
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyDictionary<string, object?> rdict:
                    WriteFormat(
                        tw,
                        format,
                        key => rdict[key],
                        options);
                    break;
#endif
                case IDictionary<string, object?> idict:
                    WriteFormat(
                        tw,
                        format,
                        key => idict[key],
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<KeyValuePair<string, object?>> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.Key, key)).Value,
                        options);
                    break;
#endif
                case ICollection<KeyValuePair<string, object?>> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.Key, key)).Value,
                        options);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.Key, key)).Value,
                        options);
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
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            switch (keyValues)
            {
                case KeyValuePair<string, object?>[] array:
                    WriteFormat(
                        tw,
                        format,
                        key => (Array.FindIndex(array, kv => predicate(kv.Key, key)) is { } index && index >= 0) ?
                            array[index].Value :
                            fallback(key),
                        options);
                    break;
                case Dictionary<string, object?> dict:
                    WriteFormat(
                        tw,
                        format,
                        key => dict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyDictionary<string, object?> rdict:
                    WriteFormat(
                        tw,
                        format,
                        key => rdict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                    break;
#endif
                case IDictionary<string, object?> idict:
                    WriteFormat(
                        tw,
                        format,
                        key => idict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                    break;
#if !NET35 && !NET40
                case IReadOnlyCollection<KeyValuePair<string, object?>> rcoll:
                    WriteFormat(
                        tw,
                        format,
                        key => (rcoll.FirstOrDefault(kv => predicate(kv.Key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                    break;
#endif
                case ICollection<KeyValuePair<string, object?>> coll:
                    WriteFormat(
                        tw,
                        format,
                        key => (coll.FirstOrDefault(kv => predicate(kv.Key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                    break;
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    WriteFormat(
                        tw,
                        format,
                        key => (Array.FindIndex(fixedKeyValues, kv => predicate(kv.Key, key)) is { } index && index >= 0) ?
                            fixedKeyValues[index].Value :
                            fallback(key),
                        options);
                    break;
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
        /// var result = await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            switch (keyValues)
            {
                case KeyValuePair<string, object?>[] array:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => Array.Find(array, kv => predicate(kv.Key, key)).Value,
                        options);
                case Dictionary<string, object?> dict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => dict[key],
                        options);
                case IReadOnlyDictionary<string, object?> rdict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => rdict[key],
                        options);
                case IDictionary<string, object?> idict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => idict[key],
                        options);
                case IReadOnlyCollection<KeyValuePair<string, object?>> rcoll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => rcoll.First(kv => predicate(kv.Key, key)).Value,
                        options);
                case ICollection<KeyValuePair<string, object?>> coll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => coll.First(kv => predicate(kv.Key, key)).Value,
                        options);
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => Array.Find(fixedKeyValues, kv => predicate(kv.Key, key)).Value,
                        options);
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
        /// var result = await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            switch (keyValues)
            {
                case KeyValuePair<string, object?>[] array:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (Array.FindIndex(array, kv => predicate(kv.Key, key)) is { } index && index >= 0) ?
                            array[index].Value :
                            fallback(key),
                        options);
                case Dictionary<string, object?> dict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => dict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                case IReadOnlyDictionary<string, object?> rdict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => rdict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                case IDictionary<string, object?> idict:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => idict.TryGetValue(key, out var value) ?
                            value :
                            fallback(key),
                        options);
                case IReadOnlyCollection<KeyValuePair<string, object?>> rcoll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (rcoll.FirstOrDefault(kv => predicate(kv.Key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                case ICollection<KeyValuePair<string, object?>> coll:
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (coll.FirstOrDefault(kv => predicate(kv.Key, key)) is ({ } k, { } value) && k != null) ?
                            value :
                            fallback(key),
                        options);
                default:
                    var fixedKeyValues = keyValues.ToArray();
                    return WriteFormatAsync(
                        tw,
                        format,
                        key => (Array.FindIndex(fixedKeyValues, kv => predicate(kv.Key, key)) is { } index && index >= 0) ?
                            fixedKeyValues[index].Value :
                            fallback(key),
                        options);
            }
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable());

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues) =>
            WriteFormatAsync(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            FormatOptions options = default)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="options">Options</param>
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
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            FormatOptions options = default)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value enumerator.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
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
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, string, bool> predicate,
            IEnumerable<KeyValuePair<string, object?>> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues,
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);
            return tw.ToString();
        }
        
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter(formatProvider);
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
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

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
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
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                options);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="predicate">format-key equality predicate delegate.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <param name="keyValues">Key-value arguments.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values with custom comparator expression.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     (key0, key1) => key0 == key1,
        ///     key => "***",
        ///     Named.Pair("abcde", 123),
        ///		Named.Pair("fgh", DateTime.Now),
        ///		Named.Pair("ijkl", 456.789));
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, string, bool> predicate,
            Func<string, object?> fallback,
            FormatOptions options,
            params KeyValuePair<string, object?>[] keyValues)
        {
            var tw = new StringWriter();
            WriteFormat(
                tw,
                format,
                predicate,
                keyValues.AsEnumerable(),
                fallback,
                options);
            return tw.ToString();
        }
    }
}
