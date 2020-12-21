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
using System.Linq;
using System.Reflection;
using System.Text;

namespace NamingFormatter
{
    partial class Named
    {
        private static readonly char[] finishFormatChars_ = { '}', ':', ',' };
        private static readonly char[] splitDotNotationChars_ = { '.' };

        private enum States
        {
            Normal,
            EnterKey
        }

        private static object? GetPropertyValue(this Type type, object instance, string name)
        {
            try
            {
#if NETSTANDARD1_0
                var pi = type.GetRuntimeProperty(name);
#else
                var pi = type.GetProperty(name);
#endif
                return pi?.GetValue(instance, null);
            }
            catch
            {
                return null;
            }
        }

        private static object? GetValueBySelector(
            Func<string, object?> selector,
            string dotNotatedKey)
        {
            // Enabling dot-notated property traverse
            var split = dotNotatedKey.Split(splitDotNotationChars_);
            if (selector(split.First()) is { } value)
            {
                var type = value.GetType();
                for (var index = 1; index < split.Length; index++)
                {
                    value = type.GetPropertyValue(value, split[index]);
                    if (value == null)
                    {
                        return null;
                    }
                    type = value.GetType();
                }
                return value;
            }
            else
            {
                return null;
            }
        }

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
                throw new ArgumentNullException("tw");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            var cooked = new StringBuilder();
            var args = new List<object?>();

            var state = States.Normal;
            var currentIndex = 0;
            while (currentIndex < format.Length)
            {
                if (state == States.Normal)
                {
                    var bracketIndex = format.IndexOf('{', currentIndex);
                    if (bracketIndex == -1)
                    {
                        cooked.Append(format.Substring(currentIndex));
                        break;
                    }

                    var nextIndex = bracketIndex + 1;
                    cooked.Append(format.Substring(currentIndex, nextIndex - currentIndex));
                    currentIndex = nextIndex;

                    state = States.EnterKey;
                    continue;
                }

                if (format[currentIndex] == '{')
                {
                    cooked.Append('{');
                    currentIndex++;

                    state = States.Normal;
                    continue;
                }

                var finishIndex = format.IndexOfAny(finishFormatChars_, currentIndex);
                if (finishIndex == -1)
                {
                    throw new FormatException("Cannot find close bracket.");
                }

                var key = format.Substring(currentIndex, finishIndex - currentIndex);
                var value = GetValueBySelector(selector, key);

                cooked.Append(args.Count);
                args.Add(value);
                currentIndex = finishIndex;

                state = States.Normal;
            }

            tw.Write(cooked.ToString(), args.ToArray());
        }

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
                throw new ArgumentNullException("formatProvider");
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
