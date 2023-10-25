/////////////////////////////////////////////////////////////////////////////////////////////////
//
// NamingFormatter - String format library with key-valued replacer.
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
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NamingFormatter.Tests
{
	[TestFixture]
	public class WriteFormatTests
	{
		[Test]
		public void DictionaryOverloadTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void DictionaryOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abx}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabxCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void DictionaryWithComparerOverloadTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>(
				StringComparer.InvariantCultureIgnoreCase)
			{
				{ "aBc", 123 },
				{ "dEFgh", now },
				{ "ijKl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void DictionaryWithComparerOverloadAndFallbackTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>(
				StringComparer.InvariantCultureIgnoreCase)
			{
				{ "aBc", 123 },
				{ "dEFgh", now },
				{ "ijKl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

#if !NET35 && !NET40
		[Test]
		public void ReadOnlyDictionaryOverloadTest()
		{
			var now = DateTime.Now;
			IReadOnlyDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void ReadOnlyDictionaryOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IReadOnlyDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}
#endif
		
		[Test]
		public void EnumerableOverloadTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithArrayTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithArrayAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithNoListTest()
		{
			var now = DateTime.Now;
			var keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", (object?)123),
				new KeyValuePair<string, object?>("defgh", (object?)now),
				new KeyValuePair<string, object?>("ijkl", (object?)"XYZ")
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithNoListAndFallbackTest()
		{
			var now = DateTime.Now;
			var keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", (object?)123),
				new KeyValuePair<string, object?>("defgh", (object?)now),
				new KeyValuePair<string, object?>("ijkl", (object?)"XYZ")
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithComparerTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("aBc", 123),
				new KeyValuePair<string, object?>("deFgH", now),
				new KeyValuePair<string, object?>("iJKl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void EnumerableOverloadWithComparerAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("aBc", 123),
				new KeyValuePair<string, object?>("deFgH", now),
				new KeyValuePair<string, object?>("iJKl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

#if !NET35 && !NET40
		[Test]
		public void TupleEnumerableOverloadWithArrayTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				( "abc", 123 ),
				( "defgh", now ),
				( "ijkl", "XYZ" )
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void TupleEnumerableOverloadWithArrayAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				( "abc", 123 ),
				( "defgh", now ),
				( "ijkl", "XYZ" )
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void TupleEnumerableOverloadWithNoListTest()
		{
			var now = DateTime.Now;
			var keyValues = new (string key, object? value)[]
			{
				( "abc", 123 ),
				( "defgh", now ),
				( "ijkl", "XYZ" )
			}.Select(entry => (key: entry.Item1, value: entry.Item2));

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void TupleEnumerableOverloadWithNoListAndFallbackTest()
		{
			var now = DateTime.Now;
			var keyValues = new (string key, object? value)[]
			{
				( "abc", 123 ),
				( "defgh", now ),
				( "ijkl", "XYZ" )
			}.Select(entry => (key: entry.Item1, value: entry.Item2));

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}

		[Test]
		public void TupleEnumerableOverloadWithComparerTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("aBc", 123),
				("deFgH", now),
				("iJKl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", tw.ToString());
		}

		[Test]
		public void TupleEnumerableOverloadWithComparerAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("aBc", 123),
				("deFgH", now),
				("iJKl", "XYZ"),
			};

			var tw = new StringWriter();
			tw.WriteFormat(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", tw.ToString());
		}
#endif
	}
}
