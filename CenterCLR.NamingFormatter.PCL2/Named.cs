/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.NamingFormatter - String format library with key-valued replacer.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

#if NET2
using CenterCLR.NamingFormatter;
#else
using System.Linq;
#endif

namespace CenterCLR
{
	/// <summary>
	/// NamingFormatter library class.
	/// </summary>
	/// <remarks>
	/// NamingFormatter library is extend the String.Format methods.
	/// String.Format replacement "{0}...{n}" is require fixed array index number.
	/// This library can use named key index (format-key: "{abc}...{XYZ}") with key-value collection.
	/// </remarks>
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
	/// var result = Named.Format("AAA{fgh:R}BBB{abcde}CCC{ijkl:E}", keyValues);
	/// </code>
	/// </example>
	public static class Named
	{
		#region Selector
		private static readonly char[] finishFormatChars_ = {'}', ':', ','};
		private static readonly char[] splitDotNotationChars_ = {'.'};

		private enum States
		{
			Normal,
			EnterKey
		}

		private static object GetPropertyValue(this Type type, object instance, string name)
		{
			Debug.Assert(type != null);
			Debug.Assert(instance != null);
			Debug.Assert(name != null);
		 
			try
			{
#if PCL2
				var pi = type.GetRuntimeProperty(name);
#else
				var pi = type.GetProperty(name);
#endif
				return pi.GetValue(instance, null);
			}
			catch
			{
				return null;
			}
		}

		private static object GetValueBySelector(
			Func<string, object> selector,
			string dotNotatedKey)
		{
			Debug.Assert(selector != null);
			Debug.Assert(dotNotatedKey != null);

			// Enabling dot-notated property traverse
			var split = dotNotatedKey.Split(splitDotNotationChars_);
			var value = selector(split.First());
			var type = value.GetType();
			for (var index = 1; index < split.Length; index++)
			{
				value = type.GetPropertyValue(value, split[index]);
				if (value == null)
				{
					break;
				}
				type = value.GetType();
			}

			return value;
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
			Func<string, object> selector)
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
			Func<string, object> selector)
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
			Func<string, object> selector)
		{
			var tw = new StringWriter();
			tw.WriteFormat(format, selector);
			return tw.ToString();
		}
		#endregion

		#region Predicate
		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="tw">Format text writer.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			if ((keyValues is ICollection<KeyValuePair<string, object>>) == false)
			{
#if PCL2
				if ((keyValues is IReadOnlyCollection<KeyValuePair<string, object>>) == false)
				{
					var fixedKeyValues = keyValues.ToList();
					tw.WriteFormat(
						format,
						key => fixedKeyValues.First(kv => predicate(kv.Key, key)).Value);
				}
#else
				var fixedKeyValues = keyValues.ToList();
				tw.WriteFormat(
					format,
					key => fixedKeyValues.First(kv => predicate(kv.Key, key)).Value);
#endif
				return;
			}

			tw.WriteFormat(
				format,
				key => keyValues.First(kv => predicate(kv.Key, key)).Value);
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="tw">Format text writer.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			params KeyValuePair<string, object>[] keyValues)
		{
			tw.WriteFormat(
				format,
				predicate,
				keyValues.AsEnumerable());
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			if (formatProvider == null)
			{
				throw new ArgumentNullException("formatProvider");
			}

			var tw = new StringWriter(formatProvider);
			tw.WriteFormat(
				format,
				key => keyValues.First(kv => predicate(kv.Key, key)).Value);
			return tw.ToString();
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			return Format(
				CultureInfo.CurrentCulture,
				format,
				predicate,
				keyValues);
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			params KeyValuePair<string, object>[] keyValues)
		{
			return Format(
				formatProvider,
				format,
				predicate,
				keyValues.AsEnumerable());
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="predicate">format-key equality predicate delegate.</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			params KeyValuePair<string, object>[] keyValues)
		{
			return Format(
				format,
				predicate,
				keyValues.AsEnumerable());
		}
		#endregion

		#region Dictionary
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
			Dictionary<string, object> keyValues)
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			tw.WriteFormat(
				format,
				key => keyValues[key]);
		}

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
			Dictionary<string, object> keyValues)
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
			Dictionary<string, object> keyValues)
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			return Format(
				format,
				key => keyValues[key]);
		}
		#endregion

		#region IDictionary
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
			IDictionary<string, object> keyValues)
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			tw.WriteFormat(
				format,
				key => keyValues[key]);
		}

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
		/// IDictionary&lt;string, object&gt; keyValues = new Dictionary&lt;string, object&gt;()
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
			IDictionary<string, object> keyValues)
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
		/// IDictionary&lt;string, object&gt; keyValues = new Dictionary&lt;string, object&gt;()
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
			IDictionary<string, object> keyValues)
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			return Format(
				format,
				key => keyValues[key]);
		}
		#endregion

#if PCL2
		#region IReadOnlyDictionary
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
		public static void WriteFormat<TDictionary>(
			this TextWriter tw,
			string format,
			TDictionary keyValues)
			where TDictionary : IReadOnlyDictionary<string, object>
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			tw.WriteFormat(
				format,
				key => keyValues[key]);
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
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
		/// var result = new CultureInfo("fr-FR").Format&lt;Dictionary&lt;string, object&gt;&gt;(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     keyValues);
		/// </code>
		/// </example>
		public static string Format<TDictionary>(
			IFormatProvider formatProvider,
			string format,
			TDictionary keyValues)
			where TDictionary : IReadOnlyDictionary<string, object>
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
		/// <typeparam name="TDictionary">IReadOnlyDictionary derived type.</typeparam>
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
		/// var result = Named.Format&lt;Dictionary&lt;string, object&gt;&gt;(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     keyValues);
		/// </code>
		/// </example>
		public static string Format<TDictionary>(
			string format,
			TDictionary keyValues)
			where TDictionary : IReadOnlyDictionary<string, object>
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}

			return Format(
				format,
				key => keyValues[key]);
		}
		#endregion
#endif

		#region Equality comparer
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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			tw.WriteFormat(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}

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
			params KeyValuePair<string, object>[] keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			tw.WriteFormat(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}

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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return Format(
				formatProvider,
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}

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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return Format(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}

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
			params KeyValuePair<string, object>[] keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return Format(
				formatProvider,
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}

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
			params KeyValuePair<string, object>[] keyValues)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return Format(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value, comparer));
		}
		#endregion

		#region Enumerator
		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="tw">Format text writer.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			tw.WriteFormat(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="tw">Format text writer.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
		/// <returns>Formatted string.</returns>
		/// <example>
		/// <code>
		/// // Format string by format-key-values.
		/// var result = new CultureInfo("fr-FR").Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     Named.Pair("abcde", 123),
		///		Named.Pair("fgh", DateTime.Now),
		///		Named.Pair("ijkl", 456.789));
		/// </code>
		/// </example>
		public static void WriteFormat(
			this TextWriter tw,
			string format,
			params KeyValuePair<string, object>[] keyValues)
		{
			tw.WriteFormat(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
		/// // Format string by format-key-values.
		/// var result = new CultureInfo("fr-FR").Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     keyValues);
		/// </code>
		/// </example>
		public static string Format(
			IFormatProvider formatProvider,
			string format,
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			return Format(
				formatProvider,
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
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
		/// // Format string by format-key-values.
		/// var result = Named.Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     keyValues);
		/// </code>
		/// </example>
		public static string Format(
			string format,
			IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			return Format(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
		/// <returns>Formatted string.</returns>
		/// <example>
		/// <code>
		/// // Format string by format-key-values.
		/// var result = new CultureInfo("fr-FR").Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     Named.Pair("abcde", 123),
		///		Named.Pair("fgh", DateTime.Now),
		///		Named.Pair("ijkl", 456.789));
		/// </code>
		/// </example>
		public static string Format(
			IFormatProvider formatProvider,
			string format,
			params KeyValuePair<string, object>[] keyValues)
		{
			return Format(
				formatProvider,
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// Format string with named format-key.
		/// </summary>
		/// <param name="format">The format string (can include format-key).</param>
		/// <param name="keyValues">Key-value enumerator.</param>
		/// <returns>Formatted string.</returns>
		/// <example>
		/// <code>
		/// // Format string by format-key-values.
		/// var result = Named.Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     Named.Pair("abcde", 123),
		///		Named.Pair("fgh", DateTime.Now),
		///		Named.Pair("ijkl", 456.789));
		/// </code>
		/// </example>
		public static string Format(
			string format,
			params KeyValuePair<string, object>[] keyValues)
		{
			return Format(
				format,
				keyValues.ToDictionary(kv => kv.Key, kv => kv.Value));
		}
		#endregion

		#region Pair
		/// <summary>
		/// Key-value creator (alias KeyValuePair).
		/// </summary>
		/// <param name="key">Key string</param>
		/// <param name="value">Value</param>
		/// <returns>KeyValuePair instance.</returns>
		/// <example>
		/// <code>
		/// // Format string by format-key-values.
		/// var result = Named.Format(
		///     "AAA{fgh:R}BBB{abcde}CCC{ijkl:E}",
		///     Named.Pair("abcde", 123),
		///		Named.Pair("fgh", DateTime.Now),
		///		Named.Pair("ijkl", 456.789));
		/// </code>
		/// </example>
		public static KeyValuePair<string, object> Pair(string key, object value)
		{
			return new KeyValuePair<string, object>(key, value);
		}
		#endregion
	}
}
