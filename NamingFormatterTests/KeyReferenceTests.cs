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

using NUnit.Framework;

namespace NamingFormatter.Tests
{
    [TestFixture]
    public class KeyReferenceTests
    {
        [Test]
        public void StandardScenarioTest()
        {
            const string format = "AAA{defgh}BBB{abc}CCC{ijkl}DDD";

            var actual = Named.GetKeyReferences(format);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("defgh", actual[0].KeyPath);
            Assert.AreEqual("abc", actual[1].KeyPath);
            Assert.AreEqual("ijkl", actual[2].KeyPath);
            Assert.AreEqual("defgh", actual[0].RootKey);
            Assert.AreEqual("abc", actual[1].RootKey);
            Assert.AreEqual("ijkl", actual[2].RootKey);
            Assert.AreEqual(format.IndexOf("{defgh}"), actual[0].PlaceholderStartIndex);
            Assert.AreEqual("{defgh}".Length, actual[0].PlaceholderLength);
            Assert.AreEqual(format.IndexOf("{abc}"), actual[1].PlaceholderStartIndex);
            Assert.AreEqual("{abc}".Length, actual[1].PlaceholderLength);
            Assert.AreEqual(format.IndexOf("{ijkl}"), actual[2].PlaceholderStartIndex);
            Assert.AreEqual("{ijkl}".Length, actual[2].PlaceholderLength);
        }

        [Test]
        public void TraversePropertyAndFormatOptionTest()
        {
            const string format = "{commitDate:F} {commitDate.Offset:hhmm} {Configuration,10}";

            var actual = Named.GetKeyReferences(format);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("commitDate", actual[0].KeyPath);
            Assert.AreEqual("commitDate.Offset", actual[1].KeyPath);
            Assert.AreEqual("Configuration", actual[2].KeyPath);
            Assert.AreEqual("commitDate", actual[0].RootKey);
            Assert.AreEqual("commitDate", actual[1].RootKey);
            Assert.AreEqual("Configuration", actual[2].RootKey);
        }

        [Test]
        public void CustomBracketTest()
        {
            const string format = "AAA@[defgh:yyyMMdd#$]BBB@[abc:X#$]CCC@[ijkl#$]DDD";

            var actual = Named.GetKeyReferences(
                format,
                new FormatOptions("@[", "#$]"));

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("defgh", actual[0].KeyPath);
            Assert.AreEqual("abc", actual[1].KeyPath);
            Assert.AreEqual("ijkl", actual[2].KeyPath);
        }

        [Test]
        public void DoubleBracketTest()
        {
            var actual = Named.GetKeyReferences("{{defgh}}");

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void EmptyKeyTest()
        {
            const string format = "{} {:}";

            var actual = Named.GetKeyReferences(format);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(string.Empty, actual[0].KeyPath);
            Assert.AreEqual(string.Empty, actual[0].RootKey);
            Assert.AreEqual(string.Empty, actual[1].KeyPath);
            Assert.AreEqual(string.Empty, actual[1].RootKey);
        }

        [Test]
        public void GetKeysPreservesOrderAndDuplicatesTest()
        {
            var actual = Named.GetKeys("{value}{value.Detail}{value}");

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("value", actual[0]);
            Assert.AreEqual("value.Detail", actual[1]);
            Assert.AreEqual("value", actual[2]);
        }
    }
}
