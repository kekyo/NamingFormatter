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
using NUnit.Framework;

namespace NamingFormatter.Tests
{
	[TestFixture]
	public class KeyReplaceTests
	{
		[Test]
		public void StandardSenarioTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
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
		public void SingleFormatIdentityTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh}",
				keyValues);

			Assert.AreEqual(now.ToString(), actual);
		}

		[Test]
		public void FrontFormatIdentityTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh}AAA",
				keyValues);

			Assert.AreEqual(now + "AAA", actual);
		}

		[Test]
		public void EndFormatIdentityTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"AAA{defgh}",
				keyValues);

			Assert.AreEqual("AAA" + now, actual);
		}

		[Test]
		public void FormatIdentityTraversePropertyTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh.Year}",
				keyValues);

			Assert.AreEqual(now.Year.ToString(), actual);
		}

		[Test]
		public void FormatIdentityTraversePropertiesTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh.TimeOfDay.TotalMilliseconds}",
				keyValues);

			Assert.AreEqual(now.TimeOfDay.TotalMilliseconds.ToString(), actual);
		}

		[Test]
		public void FormatIdentityTraversePropertyNotFoundTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh.TimeOfDa.TotalMilliseconds}",
				keyValues);

			Assert.AreEqual(string.Empty, actual);
		}

		[Test]
		public void FormatIdentityWithAlignmentTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{abc,10}",
				keyValues);

			Assert.AreEqual("       123", actual);
		}

		[Test]
		public void FormatIdentityWithOptionTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh:yyyyMMddHHmmssfff}",
				keyValues);

			Assert.AreEqual(now.ToString("yyyyMMddHHmmssfff"), actual);
		}

		[Test]
		public void FormatIdentityWithEmptyOptionTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{defgh:}",
				keyValues);

			Assert.AreEqual(now.ToString(), actual);
		}

		[Test]
		public void EmptyKeyIdentityTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{}",
				keyValues);

			Assert.AreEqual(now.ToString(), actual);
		}

		[Test]
		public void EmptyKeyIdentityWithOptionTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{:yyyyMMddHHmmssfff}",
				keyValues);

			Assert.AreEqual(now.ToString("yyyyMMddHHmmssfff"), actual);
		}

		[Test]
		public void EmptyKeyIdentityWithEmptyOptionTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{:}",
				keyValues);

			Assert.AreEqual(now.ToString(), actual);
		}

		[Test]
		public void DoubleBracketTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{{defgh}}",
				keyValues);

			Assert.AreEqual("{defgh}", actual);
		}

		[Test]
		public void DoubleBracketEmptyKeyTest()
		{
			var now = DateTime.Now;
			var keyValues = new Dictionary<string, object>()
			{
				{ "abc", 123 },
				{ "defgh", now },
				{ "ijkl", "XYZ" }
			};

			var actual = Named.Format(
				"{{}}",
				keyValues);

			Assert.AreEqual("{}", actual);
		}
	}
}
