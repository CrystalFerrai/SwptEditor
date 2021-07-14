// Copyright 2021 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Text;

namespace SwptSaveLib
{
    /// <summary>
    /// Extension methods for binary readers and writers
    /// </summary>
    internal static class BinaryIoExtensions
    {
        /// <summary>
        /// Reads an ASCII string that is prefixed with a length in bytes
        /// </summary>
        public static string ReadPrefixedString(this BinaryReader reader)
        {
            byte len = reader.ReadByte();
            byte[] data = reader.ReadBytes(len);
            return Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Writes an ASCII string that is prefixed with a length in bytes
        /// </summary>
        public static void WritePrefixedString(this BinaryWriter writer, string value)
        {
            byte[] data = Encoding.ASCII.GetBytes(value);
            writer.Write((byte)data.Length);
            writer.Write(data);
        }
    }
}
