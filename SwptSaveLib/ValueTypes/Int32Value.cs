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
using System.IO;

namespace SwptSaveLib.ValueTypes
{
    /// <summary>
    /// Represents a SaveProperty value of type Int32
    /// </summary>
    public class Int32Value : SaveValue<Int32>
    {
        public Int32Value()
            : base(SaveValueTypes.Int32)
        {
        }

        protected internal override void Deserialize(BinaryReader reader)
        {
            TypedData = reader.ReadInt32();
        }

        protected internal override void Serialize(BinaryWriter writer)
        {
            writer.Write(TypedData);
        }
    }
}
