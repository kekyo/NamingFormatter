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
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NamingFormatter.Internal
{
    [Flags]
    internal enum PreFormatOptions
    {
        None = 0x00,
        IgnoreInvalidPropertyPath = 0x01,
        IgnoreIfTerminated = 0x02,
        IgnoreBoth = 0x03
    }

    internal static class Formatter
    {
        private static readonly char[] finishFormatChars_ = { '}', ':', ',' };
        private static readonly char[] splitDotNotationChars_ = { '.' };

        private enum States
        {
            Normal,
            EnterKey
        }

        private static bool TryGetPropertyValue(this Type type, object instance, string name, out object? value)
        {
            try
            {
#if NETSTANDARD1_0
                var pi = type.GetRuntimeProperty(name);
#else
                var pi = type.GetProperty(name);
#endif
                if (pi != null)
                {
                    value = pi.GetValue(instance, null);
                    return true;
                }
            }
            catch
            {
            }

            value = default;
            return false;
        }

        private static bool TryGetFieldValue(this Type type, object instance, string name, out object? value)
        {
            try
            {
#if NETSTANDARD1_0
                var fi = type.GetRuntimeField(name);
#else
                var fi = type.GetField(name);
#endif
                if (fi != null)
                {
                    value = fi.GetValue(instance);
                    return true;
                }
            }
            catch
            {
            }

            value = default;
            return false;
        }

        private enum Results
        {
            InvalidPropertyPath,
            Terminated,
            Got
        }

        private static Results TryGetValueBySelector(
            Func<string, object?> selector,
            string dotNotatedKey,
            out object? value)
        {
            // Traverse dot-notated properties
            var split = dotNotatedKey.Split(splitDotNotationChars_);
            Debug.Assert(split.Length >= 1);
            var v = selector(split[0]);
            if ((v == null) && (split.Length == 1))
            {
                value = default;
                return Results.Got;
            }
            for (var index = 1; index < split.Length; index++)
            {
                if (v == null)
                {
                    value = default;
                    return Results.Terminated;
                }
                var type = v.GetType();
                if (type.TryGetPropertyValue(v, split[index], out var v1))
                {
                    v = v1;
                    continue;
                }
                if (type.TryGetFieldValue(v, split[index], out var v2))
                {
                    v = v2;
                    continue;
                }
                value = default;
                return Results.InvalidPropertyPath;
            }
            value = v;
            return Results.Got;
        }

        public static (string format, object?[] args) PreFormat(
            string format,
            Func<string, object?> selector,
            PreFormatOptions options)
        {
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
                    throw new FormatException("Couldn't find close bracket.");
                }

                var key = format.Substring(currentIndex, finishIndex - currentIndex);
                switch (TryGetValueBySelector(selector, key, out var value))
                {
                    case Results.InvalidPropertyPath when
                        (options & PreFormatOptions.IgnoreInvalidPropertyPath) != PreFormatOptions.IgnoreInvalidPropertyPath:
                    case Results.Terminated when
                        (options & PreFormatOptions.IgnoreIfTerminated) != PreFormatOptions.IgnoreIfTerminated:
                        throw new ArgumentException($"Couldn't find a key: {key}");
                }

                cooked.Append(args.Count);
                args.Add(value);
                currentIndex = finishIndex;

                state = States.Normal;
            }

            return (cooked.ToString(), args.ToArray());
        }
    }
}
