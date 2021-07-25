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
using System.IO;

namespace SwptSaveLib
{
    /// <summary>
    /// Represents a single property from a save file
    /// </summary>
    public class SaveProperty : ObservableObject
    {
        private const byte PrefixByte = 0x7e;
        private const byte PostfixByte = 0x7b;
        private const byte ArrayStart = 0x53;
        private const byte TypePrefix = 0xff;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        private string _name;

        public SaveValue Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
        private SaveValue _value;

        public SaveProperty(string name, SaveValue value)
        {
            Name = name;
            Value = value;
        }

        public SaveProperty Clone()
        {
            SaveProperty property = new SaveProperty(Name, SaveValue.Create(Value.Type));
            property.Value.Data = Value.CloneData();
            return property;
        }

        public static SaveProperty Load(BinaryReader reader)
        {
            if (reader.ReadByte() != PrefixByte) throw new FormatException("Unexpected value in save file");

            string name = reader.ReadPrefixedString();
            int size = reader.ReadInt32();
            long startPos = reader.BaseStream.Position;
            bool isArrayType;

            switch (reader.ReadByte())
            {
                case ArrayStart:
                    isArrayType = true;
                    if (reader.ReadByte() != TypePrefix) throw new FormatException("Unexpected value in save file");
                    break;
                case TypePrefix:
                    isArrayType = false;
                    break;
                default:
                    throw new FormatException("Unexpected value in save file");
            }

            SaveValueType type = SaveValueTypes.Instance[reader.ReadInt32()];
            SaveValue value = SaveValue.Load(reader, type, isArrayType);

            if (reader.ReadByte() != PostfixByte) throw new FormatException("Unexpected value in save file");

            if (reader.BaseStream.Position != startPos + size) throw new FormatException("Unexpected property length");

            return new SaveProperty(name, value);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(PrefixByte);
            writer.WritePrefixedString(Name);

            writer.Write(0); // size placeholder

            long startPos = writer.BaseStream.Position;

            if (Value.Type == SaveValueTypes.Array)
            {
                writer.Write(ArrayStart);
            }
            writer.Write(TypePrefix);

            if (Value.Type == SaveValueTypes.Array)
            {
                writer.Write(((ArrayValue)Value).ItemType.ID);
            }
            else
            {
                writer.Write(Value.Type.ID);
            }

            Value.Serialize(writer);

            writer.Write(PostfixByte);

            long endPos = writer.BaseStream.Position;

            // Go back and write the size of the property
            writer.BaseStream.Seek(startPos - 4, SeekOrigin.Begin);
            writer.Write((int)(endPos - startPos));
            writer.BaseStream.Seek(endPos, SeekOrigin.Begin);
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
