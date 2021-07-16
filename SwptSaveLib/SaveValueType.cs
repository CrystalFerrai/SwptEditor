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

namespace SwptSaveLib
{
    /// <summary>
    /// Possible types for a SaveValue
    /// </summary>
    /// <remarks>
    /// The specific values used in this enumeration are not arbitrary. They need to match the values the game uses to identify
    /// the corresponding type in save files.
    /// </remarks>
    public enum SaveValueType : uint
    {
        Array = 0x53,
        String = 0xfde9f1ee,
        Bool = 0xad4d7c9c,
        Int32 = 0xe2a80856,
        Single = 0x6e3ed76b,
        Vector2 = 0xb59a00d5,
        Vector3 = 0xec66dc46,
        LinearColor = 0x32cf4b31
    }

    public static class SaveValueTypeExtensions
    {
        public static string GetDisplayName(this SaveValueType value)
        {
            switch (value)
            {
                case SaveValueType.Array: return "Array";
                case SaveValueType.String: return "Text";
                case SaveValueType.Bool: return "Boolean";
                case SaveValueType.Int32: return "Integer";
                case SaveValueType.Single: return "Number";
                case SaveValueType.Vector2: return "2D Vector";
                case SaveValueType.Vector3: return "3D Vector";
                case SaveValueType.LinearColor: return "Color";
                default: return "Unknown";
            }
        }
    }
}
