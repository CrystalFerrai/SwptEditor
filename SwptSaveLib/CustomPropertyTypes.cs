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

        public override int GetHashCode()
        {
            int hash = 17;
            hash += X.GetHashCode() * 23;
            hash += Y.GetHashCode() * 23;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 other && X == other.X && Y == other.Y;
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

        public override int GetHashCode()
        {
            int hash = 17;
            hash += X.GetHashCode() * 23;
            hash += Y.GetHashCode() * 23;
            hash += Z.GetHashCode() * 23;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 other && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    /// <summary>
    /// A color in linear space
    /// </summary>
    public class LinearColor
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public LinearColor()
        {
            R = 1.0f;
            G = 0.0f;
            B = 1.0f;
            A = 1.0f;
        }

        public LinearColor Clone()
        {
            return new LinearColor() { R = R, G = G, B = B, A = A };
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash += R.GetHashCode() * 23;
            hash += G.GetHashCode() * 23;
            hash += B.GetHashCode() * 23;
            hash += A.GetHashCode() * 23;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is LinearColor other && R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override string ToString()
        {
            return $"({R}, {G}, {B}, {A})";
        }
    }
}
