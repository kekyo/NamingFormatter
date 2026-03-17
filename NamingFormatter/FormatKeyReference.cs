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

namespace NamingFormatter
{
    /// <summary>
    /// Represents a named placeholder reference contained in a format string.
    /// </summary>
    /// <remarks>
    /// The reference preserves the original key path, such as <c>commit.Author.Name</c>,
    /// and exposes the root key for callers that need to resolve only the first segment.
    /// </remarks>
    public readonly struct FormatKeyReference
    {
        private readonly string keyPath;
        private readonly string rootKey;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="keyPath">The parsed key path.</param>
        /// <param name="placeholderStartIndex">The placeholder start index in the original format string.</param>
        /// <param name="placeholderLength">The placeholder length in the original format string.</param>
        public FormatKeyReference(
            string keyPath,
            int placeholderStartIndex,
            int placeholderLength)
        {
            if (keyPath == null)
            {
                throw new ArgumentNullException(nameof(keyPath));
            }
            if (placeholderStartIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(placeholderStartIndex));
            }
            if (placeholderLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(placeholderLength));
            }

            this.keyPath = keyPath;
            this.rootKey =
                (keyPath.IndexOf('.') is var dotIndex) && (dotIndex >= 0) ?
                    keyPath.Substring(0, dotIndex) :
                    keyPath;
            this.PlaceholderStartIndex = placeholderStartIndex;
            this.PlaceholderLength = placeholderLength;
        }

        /// <summary>
        /// Gets the full key path.
        /// </summary>
        public string KeyPath =>
            this.keyPath;

        /// <summary>
        /// Gets the first segment of the key path.
        /// </summary>
        public string RootKey =>
            this.rootKey;

        /// <summary>
        /// Gets the placeholder start index in the original format string.
        /// </summary>
        public int PlaceholderStartIndex
        {
            get;
        }

        /// <summary>
        /// Gets the placeholder length in the original format string.
        /// </summary>
        public int PlaceholderLength
        {
            get;
        }

        /// <summary>
        /// Returns the key path.
        /// </summary>
        /// <returns>The key path.</returns>
        public override string ToString() =>
            this.KeyPath;
    }
}
