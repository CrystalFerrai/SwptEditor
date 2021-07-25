﻿// Copyright 2021 Crystal Ferrai
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

namespace SwptSaveLib.ValueTypes
{
    /// <summary>
    /// Represents a SaveProperty value of type LinearColor
    /// </summary>
    public class LinearColorValue : SaveValue<LinearColor>
    {
        public LinearColorValue()
            : base(SaveValueTypes.LinearColor)
        {
            Data = new LinearColor();
        }

        public override object CloneData()
        {
            return TypedData.Clone();
        }

        protected internal override void Deserialize(BinaryReader reader)
        {
            LinearColor v = new LinearColor();
            v.R = reader.ReadSingle();
            v.G = reader.ReadSingle();
            v.B = reader.ReadSingle();
            v.A = reader.ReadSingle();
            TypedData = v;
        }

        protected internal override void Serialize(BinaryWriter writer)
        {
            writer.Write(TypedData.R);
            writer.Write(TypedData.G);
            writer.Write(TypedData.B);
            writer.Write(TypedData.A);
        }
    }
}
