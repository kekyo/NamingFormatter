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

#if !NET35 && !NET40
using System.Threading.Tasks;
#endif

namespace NamingFormatter
{
#if !NET35 && !NET40
    partial class Named
    {
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IReadOnlyDictionary<string, object?> keyValues,
            FormatOptions options = default)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            WriteFormat(
                tw,
                format,
                key => keyValues[key],
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            IReadOnlyDictionary<string, object?> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            WriteFormat(
                tw,
                format,
                key => keyValues.TryGetValue(key, out var value) ?
                    value :
                    fallback(key),
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IReadOnlyDictionary<string, object?> keyValues,
            FormatOptions options = default)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            return WriteFormatAsync(
                tw,
                format,
                key => keyValues[key],
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            IReadOnlyDictionary<string, object?> keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            return WriteFormatAsync(
                tw,
                format,
                key => keyValues.TryGetValue(key, out var value) ?
                    value :
                    fallback(key),
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var result = new CultureInfo("fr-FR").Format&lt;Dictionary&lt;string, object&gt;&gt;(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format<TDictionary>(
            IFormatProvider formatProvider,
            string format,
            TDictionary keyValues,
            FormatOptions options = default)
            where TDictionary : IReadOnlyDictionary<string, object?>
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            return Format(
                formatProvider,
                format,
                key => keyValues[key],
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var result = new CultureInfo("fr-FR").Format&lt;Dictionary&lt;string, object&gt;&gt;(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format<TDictionary>(
            IFormatProvider formatProvider,
            string format,
            TDictionary keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
            where TDictionary : IReadOnlyDictionary<string, object?>
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            return Format(
                formatProvider,
                format,
                key => keyValues.TryGetValue(key, out var value) ?
                    value :
                    fallback(key),
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="options"></param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var result = Named.Format&lt;Dictionary&lt;string, object&gt;&gt;(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format<TDictionary>(
            string format,
            TDictionary keyValues,
            FormatOptions options = default)
            where TDictionary : IReadOnlyDictionary<string, object?>
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            return Format(
                format,
                key => keyValues[key],
                options);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
        /// <param name="fallback">Fallback delegate.</param>
        /// <param name="options">Options</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // format-key-value dictionary.
        /// var keyValues = new Dictionary&lt;string, object&gt;()
        /// {
        ///     { "abcde", 123 },
        ///     { "fgh", DateTime.Now },
        ///     { "ijkl", 456.789 },
        ///     // ...
        /// };
        /// 
        /// // Format string by format-key-values.
        /// var result = Named.Format&lt;Dictionary&lt;string, object&gt;&gt;(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues,
        ///     key => "***");
        /// </code>
        /// </example>
        public static string Format<TDictionary>(
            string format,
            TDictionary keyValues,
            Func<string, object?> fallback,
            FormatOptions options = default)
            where TDictionary : IReadOnlyDictionary<string, object?>
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            if (fallback == null)
            {
                throw new ArgumentNullException(nameof(fallback));
            }

            return Format(
                format,
                key => keyValues.TryGetValue(key, out var value) ?
                    value :
                    fallback(key),
                options);
        }
    }
#endif
}
