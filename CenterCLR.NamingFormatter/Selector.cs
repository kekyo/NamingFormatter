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
using System.IO;
using NamingFormatter.Internal;

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
        /// <param name="tw">Formatted text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="selector">format-key to value selector delegate.</param>
        /// <example>
        /// <code>
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// tw.WriteFormat(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     key =>
        ///         (key == "abcde") ? 123 :
        ///         (key == "fgh") ? DateTime.Now :
        ///         (key == "ijkl") ? 456.789 :
        ///         "(Unknown)");
        /// </code>
        /// </example>
        public static void WriteFormat(
            this TextWriter tw,
            string format,
            Func<string, object?> selector)
        {
            if (tw == null)
            {
                throw new ArgumentNullException(nameof(tw));
            }
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var (formatted, args) = Formatter.PreFormat(format, selector, PreFormatOptions.IgnoreBoth);
            tw.Write(formatted, args);
        }

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Formatted text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="selector">format-key to value selector delegate.</param>
        /// <example>
        /// <code>
        /// // Format string by format-key-values.
        /// var tw = new StringWriter();
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     key =>
        ///         (key == "abcde") ? 123 :
        ///         (key == "fgh") ? DateTime.Now :
        ///         (key == "ijkl") ? 456.789 :
        ///         "(Unknown)");
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Func<string, object?> selector)
        {
            if (tw == null)
            {
                throw new ArgumentNullException(nameof(tw));
            }
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var (formatted, args) = Formatter.PreFormat(format, selector, PreFormatOptions.IgnoreBoth);
            return tw.WriteAsync(string.Format(formatted, args));
        }
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="selector">format-key to value selector delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <example>
        /// <code>
        /// // Format string by format-key-values.
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     key =>
        ///         (key == "abcde") ? 123 :
        ///         (key == "fgh") ? DateTime.Now :
        ///         (key == "ijkl") ? 456.789 :
        ///         "(Unknown)");
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Func<string, object?> selector)
        {
            if (formatProvider == null)
            {
                throw new ArgumentNullException(nameof(formatProvider));
            }

            var tw = new StringWriter(formatProvider);
            tw.WriteFormat(format, selector);
            return tw.ToString();
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="selector">format-key to value selector delegate.</param>
        /// <returns>Formatted string.</returns>
        /// <remarks>
        /// This method is minimum basic interface.
        /// "selector" delegate will be past "format-key" from argument,
        /// you must return a value of pairing format-key.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Format string by format-key-values.
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     key =>
        ///         (key == "abcde") ? 123 :
        ///         (key == "fgh") ? DateTime.Now :
        ///         (key == "ijkl") ? 456.789 :
        ///         "(Unknown)");
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Func<string, object?> selector)
        {
            var tw = new StringWriter();
            tw.WriteFormat(format, selector);
            return tw.ToString();
        }
    }
}
