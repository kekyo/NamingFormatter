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
using NamingFormatter.Internal;

namespace NamingFormatter
{
    partial class Named
    {
        /// <summary>
        /// Gets the placeholder references contained in a format string without resolving values.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="options">Options.</param>
        /// <returns>The parsed placeholder references in appearance order.</returns>
        /// <remarks>
        /// The returned references preserve duplicate placeholders and keep the same parsing rules
        /// as <see cref="Format(string, Func{string, object?}, FormatOptions)"/>.
        /// </remarks>
        public static FormatKeyReference[] GetKeyReferences(
            string format,
            FormatOptions options = default)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            return Formatter.GetKeyReferences(
                format,
                options.BracketStart ?? "{",
                options.BracketEnd ?? "}");
        }

        /// <summary>
        /// Gets the key paths contained in a format string without resolving values.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="options">Options.</param>
        /// <returns>The parsed key paths in appearance order.</returns>
        public static string[] GetKeys(
            string format,
            FormatOptions options = default)
        {
            var references = GetKeyReferences(format, options);
            var keys = new string[references.Length];

            for (var index = 0; index < references.Length; index++)
            {
                keys[index] = references[index].KeyPath;
            }

            return keys;
        }
    }
}
