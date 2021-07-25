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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwptSaveLib
{
    /// <summary>
    /// Represents a SaveValue's type
    /// </summary>
    public class SaveValueType
    {
        /// <summary>
        /// Gets the unique serialization identifier for this type
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the fully qualified name of the data type represented by this instance
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets the friendly name of the type represented by this instance
        /// </summary>
        public string DisplayName { get; }

        #region Internal Implementation

        internal static SaveValueType ArrayType { get; } = new SaveValueType(0x53, "Array", "Array");

        internal SaveValueType(Type type, string displayName)
            : this(type.ToString(), displayName)
        {
        }

        internal SaveValueType(string typeName, string displayName)
            : this(GetHash(typeName), typeName, displayName)
        {
        }

        private SaveValueType(int id, string typeName, string displayName)
        {
            ID = id;
            TypeName = typeName;
            DisplayName = displayName;
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveValueType other && ID == other.ID;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        // This method is based on ES2Type.GetHash from the game file Assembly-CSharp-firstpass.dll.
        // This method must produce the same value as that method in order for serialization to work.
        private static int GetHash(string value)
        {
            int length = value.Length;
            uint num = (uint)length;
            int num1 = length & 1;
            length >>= 1;
            int num2 = 0;
            while (length > 0)
            {
                num += value[num2];
                uint num3 = (uint)(value[num2 + 1] << '\v' ^ num);
                num = num << 16 ^ num3;
                num2 += 2;
                num = num + (num >> 11);
                length--;
            }
            if (num1 == 1)
            {
                num += value[num2];
                num = num ^ num << 11;
                num = num + (num >> 17);
            }
            num = num ^ num << 3;
            num = num + (num >> 5);
            num = num ^ num << 4;
            num = num + (num >> 17);
            num = num ^ num << 25;
            num = num + (num >> 6);

            return (int)num;
        }

        #endregion
    }

    /// <summary>
    /// Possible types for a SaveValue
    /// </summary>
    public class SaveValueTypes : IReadOnlyDictionary<int, SaveValueType>, IEnumerable<SaveValueType>
    {
        // HOW TO ADD A NEW PROPERTY TYPE
        // 1. Find the exact data type or data type name and add a new public static property here.
        //    - Reference the ES2_* classes if needed from the game file Assembly-CSharp-firstpass.dll
        // 2. Create and implement a new SaveValue extension in the ValueTypes directory.
        // 3. Add an entry to the static type map in the SaveValue base class.

        public static SaveValueType Array = SaveValueType.ArrayType;
        public static SaveValueType String { get; } = new SaveValueType(typeof(string), "Text");
        public static SaveValueType Bool { get; } = new SaveValueType(typeof(bool), "Boolean");
        public static SaveValueType Int32 { get; } = new SaveValueType(typeof(int), "Integer");
        public static SaveValueType Single { get; } = new SaveValueType(typeof(float), "Number");
        public static SaveValueType Vector2 { get; } = new SaveValueType("UnityEngine.Vector2", "2D Vector");
        public static SaveValueType Vector3 { get; } = new SaveValueType("UnityEngine.Vector3", "3d Vector");
        public static SaveValueType LinearColor { get; } = new SaveValueType("UnityEngine.Color", "Color");

        /// <summary>
        /// Returns the singleton instance of this class
        /// </summary>
        public static SaveValueTypes Instance { get; } = new SaveValueTypes();

        /// <summary>
        /// Returns this instance types as an enumerable to resolve ambiguities where needed
        /// </summary>
        public IEnumerable<SaveValueType> Enumarable { get; }

        #region Collection APIs
        public int Count => mOrderedTypes.Length;

        public IEnumerable<int> Keys => mTypeMap.Keys;

        public IEnumerable<SaveValueType> Values => mTypeMap.Values;

        public SaveValueType this[int id] => mTypeMap[id];

        public bool ContainsKey(int key)
        {
            return mTypeMap.ContainsKey(key);
        }

        public bool TryGetValue(int key, out SaveValueType value)
        {
            return mTypeMap.TryGetValue(key, out value);
        }

        public IEnumerator<SaveValueType> GetEnumerator()
        {
            return ((IEnumerable<SaveValueType>)mOrderedTypes).GetEnumerator();
        }
        #endregion

        #region Private Implementations
        private SaveValueTypes()
        {
            Enumarable = this;

            PropertyInfo[] properties = typeof(SaveValueTypes).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(p => p.PropertyType == typeof(SaveValueType)).ToArray();

            mOrderedTypes = new SaveValueType[properties.Length];
            mTypeMap = new Dictionary<int, SaveValueType>(mOrderedTypes.Length);
            for (int i = 0; i < mOrderedTypes.Length; ++i)
            {
                SaveValueType type = (SaveValueType)properties[i].GetValue(null);
                mOrderedTypes[i] = type;
                mTypeMap.Add(type.ID, type);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mOrderedTypes.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, SaveValueType>> IEnumerable<KeyValuePair<int, SaveValueType>>.GetEnumerator()
        {
            return mTypeMap.GetEnumerator();
        }

        private readonly Dictionary<int, SaveValueType> mTypeMap;
        private readonly SaveValueType[] mOrderedTypes;
        #endregion
    }
}
