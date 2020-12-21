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

using System.Collections.Generic;

namespace NamingFormatter
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
    public static partial class Named
    {
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
    }
}
