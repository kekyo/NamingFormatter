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
using System.Diagnostics;
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
        private static readonly char[] separatorChars_ = { ':', ',' };
        private static readonly char[] splitDotNotationChars_ = { '.' };

        private enum States
        {
            Normal,
            EnterKey,
            Closing,
        }

        internal readonly struct ParsedReference
        {
            public readonly FormatKeyReference Reference;
            public readonly string FormatSuffix;
            public readonly bool HasClosingBracket;

            public ParsedReference(
                FormatKeyReference reference,
                string formatSuffix,
                bool hasClosingBracket)
            {
                this.Reference = reference;
                this.FormatSuffix = formatSuffix;
                this.HasClosingBracket = hasClosingBracket;
            }
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

        private static ParsedReference CreateParsedReference(
            string format,
            int placeholderStartIndex,
            int keyStartIndex,
            int finishIndex,
            int bracketEndLength)
        {
            var keyPath = format.Substring(keyStartIndex, finishIndex - keyStartIndex);
            var placeholderLength = finishIndex + bracketEndLength - placeholderStartIndex;

            return new ParsedReference(
                new FormatKeyReference(
                    keyPath,
                    placeholderStartIndex,
                    placeholderLength),
                string.Empty,
                bracketEndLength >= 1);
        }

        private static ParsedReference CreateParsedReference(
            string format,
            int placeholderStartIndex,
            string keyPath,
            int formatSuffixStartIndex,
            int finishIndex,
            int bracketEndLength)
        {
            var formatSuffix = format.Substring(
                formatSuffixStartIndex,
                finishIndex - formatSuffixStartIndex);
            var placeholderLength = finishIndex + bracketEndLength - placeholderStartIndex;

            return new ParsedReference(
                new FormatKeyReference(
                    keyPath,
                    placeholderStartIndex,
                    placeholderLength),
                formatSuffix,
                bracketEndLength >= 1);
        }

        private static ParsedReference[] ParseReferences(
            string format,
            string bracketStart,
            string bracketEnd)
        {
            var references = new List<ParsedReference>();

            var state = States.Normal;
            var currentIndex = 0;
            var placeholderStartIndex = 0;
            var currentKey = string.Empty;
            var formatSuffixStartIndex = 0;
            while (currentIndex < format.Length)
            {
                switch (state)
                {
                    case States.Normal:
                        var bracketIndex = format.IndexOf(
                            bracketStart, currentIndex, StringComparison.Ordinal);
                        if (bracketIndex == -1)
                        {
                            currentIndex = format.Length;
                        }
                        else
                        {
                            currentIndex = bracketIndex + bracketStart.Length;
                            placeholderStartIndex = bracketIndex;
                            state = States.EnterKey;
                        }
                        break;

                    case States.EnterKey:
                        // '{{'
                        if ((bracketStart == "{") &&
                            (currentIndex < format.Length) &&
                            (format[currentIndex] == '{'))
                        {
                            currentIndex++;
                            state = States.Normal;
                            continue;
                        }

                        var separatorIndex = format.IndexOfAny(separatorChars_, currentIndex);
                        var finishIndex = format.IndexOf(
                            bracketEnd, currentIndex, StringComparison.Ordinal);
                        switch (separatorIndex, finishIndex)
                        {
                            case (-1, -1):
                                throw new FormatException("Couldn't find close bracket.");
                            case (-1, _):
                            case (_, _) when finishIndex < separatorIndex:
                                references.Add(
                                    CreateParsedReference(
                                        format,
                                        placeholderStartIndex,
                                        currentIndex,
                                        finishIndex,
                                        bracketEnd.Length));
                                currentIndex = finishIndex + bracketEnd.Length;
                                state = States.Normal;
                                break;
                            default:
                                currentKey = format.Substring(currentIndex, separatorIndex - currentIndex);
                                formatSuffixStartIndex = separatorIndex;
                                currentIndex = separatorIndex + 1;
                                state = States.Closing;
                                break;
                        }
                        break;

                    case States.Closing:
                        var bracketIndex2 = format.IndexOf(
                            bracketEnd, currentIndex, StringComparison.Ordinal);
                        if (bracketIndex2 == -1)
                        {
                            references.Add(
                                CreateParsedReference(
                                    format,
                                    placeholderStartIndex,
                                    currentKey,
                                    formatSuffixStartIndex,
                                    format.Length,
                                    0));
                            currentIndex = format.Length;
                        }
                        else
                        {
                            references.Add(
                                CreateParsedReference(
                                    format,
                                    placeholderStartIndex,
                                    currentKey,
                                    formatSuffixStartIndex,
                                    bracketIndex2,
                                    bracketEnd.Length));
                            currentIndex = bracketIndex2 + bracketEnd.Length;
                        }
                        state = States.Normal;
                        break;
                }
            }

            return references.ToArray();
        }

        public static FormatKeyReference[] GetKeyReferences(
            string format,
            string bracketStart,
            string bracketEnd)
        {
            var parsedReferences = ParseReferences(
                format,
                bracketStart,
                bracketEnd);
            var references = new FormatKeyReference[parsedReferences.Length];

            for (var index = 0; index < parsedReferences.Length; index++)
            {
                references[index] = parsedReferences[index].Reference;
            }

            return references;
        }

        public static (string format, object?[] args) PreFormat(
            string format,
            Func<string, object?> selector,
            PreFormatOptions options,
            string bracketStart,
            string bracketEnd)
        {
            var cooked = new StringBuilder();
            var args = new List<object?>();
            var parsedReferences = ParseReferences(
                format,
                bracketStart,
                bracketEnd);

            var currentIndex = 0;
            for (var index = 0; index < parsedReferences.Length; index++)
            {
                var parsedReference = parsedReferences[index];

                cooked.Append(format.Substring(
                    currentIndex,
                    parsedReference.Reference.PlaceholderStartIndex - currentIndex));

                switch (TryGetValueBySelector(
                    selector,
                    parsedReference.Reference.KeyPath,
                    out var value))
                {
                    case Results.InvalidPropertyPath when
                        (options & PreFormatOptions.IgnoreInvalidPropertyPath) != PreFormatOptions.IgnoreInvalidPropertyPath:
                    case Results.Terminated when
                        (options & PreFormatOptions.IgnoreIfTerminated) != PreFormatOptions.IgnoreIfTerminated:
                        throw new ArgumentException(
                            $"Couldn't find a key: {parsedReference.Reference.KeyPath}");
                }

                cooked.Append('{');
                cooked.Append(args.Count);
                cooked.Append(parsedReference.FormatSuffix);
                if (parsedReference.HasClosingBracket)
                {
                    cooked.Append('}');
                }

                args.Add(value);
                currentIndex = parsedReference.Reference.PlaceholderStartIndex +
                    parsedReference.Reference.PlaceholderLength;
            }

            cooked.Append(format.Substring(currentIndex));

            return (cooked.ToString(), args.ToArray());
        }
    }
}
