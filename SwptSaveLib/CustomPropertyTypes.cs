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
    /// A two-dimensional vector
    /// </summary>
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2 Clone()
        {
            return new Vector2() { X = X, Y = Y };
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    /// <summary>
    /// A three-dimensional vector
    /// </summary>
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 Clone()
        {
            return new Vector3() { X = X, Y = Y, Z = Z };
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    /// <summary>
    /// A four-dimensional vector
    /// </summary>
    public class Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4 Clone()
        {
            return new Vector4() { X = X, Y = Y, Z = Z, W = W };
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
