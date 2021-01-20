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
using System.Linq;
using NUnit.Framework;

namespace NamingFormatter.Tests
{
	[TestFixture]
	public class OverloadTests
	{
		[Test]
		public void DictionaryOverloadTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void DictionaryOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

		[Test]
		public void IDictionaryOverloadTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void IDictionaryOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

#if !NET35 && !NET40
		[Test]
		public void IReadOnlyDictionaryOverloadTest()
		{
			var now = DateTime.Now;
			IReadOnlyDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void IReadOnlyDictionaryOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IReadOnlyDictionary<string, object?> keyValues = new Dictionary<string, object?>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}
#endif
		
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

		[Test]
		public void EnumerableOverloadTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void EnumerableOverloadWithFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

		[Test]
		public void EnumerableOverloadWithPredicateTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				(key1, key2) => key1 == key2,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void EnumerableOverloadWithPredicateAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<KeyValuePair<string, object?>> keyValues = new[]
			{
				new KeyValuePair<string, object?>("abc", 123),
				new KeyValuePair<string, object?>("defgh", now),
				new KeyValuePair<string, object?>("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				(key1, key2) => key1 == key2,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

#if !NET35 && !NET40
		[Test]
		public void TupleEnumerableOverloadWithArrayTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("abc", 123),
				("defgh", now),
				("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void TupleEnumerableOverloadWithArrayAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("abc", 123),
				("defgh", now),
				("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}

		[Test]
		public void TupleEnumerableOverloadWithPredicateTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("abc", 123),
				("defgh", now),
				("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abc}CCC{ijkl}DDD",
				(key1, key2) => key1 == key2,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
		}

		[Test]
		public void TupleEnumerableOverloadWithPredicateAndFallbackTest()
		{
			var now = DateTime.Now;
			IEnumerable<(string key, object? value)> keyValues = new (string key, object? value)[]
			{
				("abc", 123),
				("defgh", now),
				("ijkl", "XYZ"),
			};

			var actual = Named.Format(
				"AAA{defgh}BBB{abX}CCC{ijkl}DDD",
				(key1, key2) => key1 == key2,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abC}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues);

			Assert.AreEqual("AAA" + now + "BBB123CCCXYZDDD", actual);
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

			var actual = Named.Format(
				"AAA{Defgh}BBB{abX}CCC{IjkL}DDD",
				StringComparer.InvariantCultureIgnoreCase,
				keyValues,
				key => key);

			Assert.AreEqual("AAA" + now + "BBBabXCCCXYZDDD", actual);
		}
#endif
	}
}
