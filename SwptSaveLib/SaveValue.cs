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

using SwptSaveLib.Utils;
using SwptSaveLib.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SwptSaveLib
{
    /// <summary>
    /// Represents the value of a SaveProperty
    /// </summary>
    public abstract class SaveValue : ObservableObject
    {
        private static Dictionary<SaveValueType, Type> sTypeMap;

        public SaveValueType Type { get; }

        public virtual object Data
        {
            get => _data;
            set => Set(ref _data, value);
        }
        private object _data;

        public virtual string DisplayType => Type.DisplayName;

        static SaveValue()
        {
            sTypeMap = new Dictionary<SaveValueType, Type>()
            {
                { SaveValueTypes.Array,   typeof(ArrayValue) },
                { SaveValueTypes.String,  typeof(StringValue) },
                { SaveValueTypes.Bool,    typeof(BoolValue) },
                { SaveValueTypes.Int32,   typeof(Int32Value) },
                { SaveValueTypes.Single,  typeof(SingleValue) },
                { SaveValueTypes.Vector2, typeof(Vector2Value) },
                { SaveValueTypes.Vector3, typeof(Vector3Value) },
                { SaveValueTypes.LinearColor, typeof(LinearColorValue) }
            };
        }

        protected SaveValue(SaveValueType type)
        {
            Type = type;
        }

        public abstract object CloneData();

        protected internal abstract void Deserialize(BinaryReader reader);

        protected internal abstract void Serialize(BinaryWriter writer);

        public virtual bool CompareData(object data)
        {
            return Data?.Equals(data) ?? (data == null);
        }

        public static SaveValue Create(SaveValueType type)
        {
            return (SaveValue)Activator.CreateInstance(sTypeMap[type]);
        }

        public static SaveValue Load(BinaryReader reader, SaveValueType type, bool isArrayType)
        {
            SaveValue value;

            if (isArrayType)
            {
                value = new ArrayValue(type);
            }
            else
            {
                value = Create(type);
            }

            value.Deserialize(reader);

            return value;
        }

        public T GetData<T>()
        {
            return Data is T ? (T)Data : default;
        }
    }

    public abstract class SaveValue<T> : SaveValue
    {
        public T TypedData
        {
            get => (T)base.Data;
            set => base.Data = value;
        }

        protected SaveValue(SaveValueType type)
            : base(type)
        {
            TypedData = default;
        }

        public override object CloneData()
        {
            return TypedData;
        }

        public override string ToString()
        {
            return TypedData?.ToString() ?? string.Empty;
        }
    }
}
