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
using System.IO;

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
        /// <param name="keyValues">Key-value dictionary.</param>
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
            Dictionary<string, object?> keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            WriteFormat(
                tw,
                format,
                key => keyValues[key]);
        }

#if !NET35 && !NET40
        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="tw">Format text writer.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
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
        /// await tw.WriteFormatAsync(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static Task WriteFormatAsync(
            this TextWriter tw,
            string format,
            Dictionary<string, object?> keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            return WriteFormatAsync(
                tw,
                format,
                key => keyValues[key]);
        }
#endif

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
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
        /// var result = new CultureInfo("fr-FR").Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            IFormatProvider formatProvider,
            string format,
            Dictionary<string, object?> keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            return Format(
                formatProvider,
                format,
                key => keyValues[key]);
        }

        /// <summary>
        /// Format string with named format-key.
        /// </summary>
        /// <param name="format">The format string (can include format-key).</param>
        /// <param name="keyValues">Key-value dictionary.</param>
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
        /// var result = Named.Format(
        ///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
        ///     keyValues);
        /// </code>
        /// </example>
        public static string Format(
            string format,
            Dictionary<string, object?> keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }

            return Format(
                format,
                key => keyValues[key]);
        }
    }
}
