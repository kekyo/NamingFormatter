using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CenterCLR
{
    public static class Named
    {
        private static readonly char[] finishFormatChars_ = {'}', ':'};

        private enum States
        {
            Normal,
            EnterKey
        }

        private static string InternalFormat(
            string format,
            Func<string, object> selector)
        {
            var cooked = new StringBuilder();
            var args = new List<object>();

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
                var value = selector(key);

                cooked.Append(args.Count);
                args.Add(value);
                currentIndex = finishIndex;

                state = States.Normal;
            }

            return string.Format(cooked.ToString(), args.ToArray());
        }

        public static string Format(
            string format,
            IDictionary<string, object> keyValues)
        {
            return InternalFormat(format, key => keyValues[key]);
        }

        public static string Format(
            string format,
            Func<string, string, bool> predict,
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            return InternalFormat(format, key => keyValues.First(kv => predict(kv.Key, key)));
        }

        public static string Format(
            string format,
            IEqualityComparer<string> comparer,
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            return Format(format, comparer.Equals, keyValues);
        }

        public static string Format(
            string format,
            IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            return Format(format, StringComparer.CurrentCulture, keyValues);
        }
    }
}
