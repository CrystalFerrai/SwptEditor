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

using SwptSaveLib;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SwptSaveEditor.Converters
{
    /// <summary>
    /// Converts a LinearColor to a Color
    /// </summary>
    internal class ColorConverter : IValueConverter
    {
        /// <summary>
        /// Static instance of this converter
        /// </summary>
        public static ColorConverter Instance;

        static ColorConverter()
        {
            Instance = new ColorConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LinearColor vec)
            {
                return Color.FromArgb((byte)(vec.A * 255.0f), LinearToSrgb(vec.R), LinearToSrgb(vec.G), LinearToSrgb(vec.B));
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color clr)
            {
                return new LinearColor() { A = clr.A / 255.0f, R = SrgbToLinear(clr.R), G = SrgbToLinear(clr.G), B = SrgbToLinear(clr.B) };
            }
            return DependencyProperty.UnsetValue;
        }

        private byte LinearToSrgb(float l)
        {
            if (l < 0.0f) l = 0.0f;
            if (l > 1.0f) l = 1.0f;

            float s = l <= 0.0031308f
                ? l * 12.92f
                : 1.055f * (float)Math.Pow(l, 1.0 / 2.4) - 0.055f;

            return (byte)(s * 255.0f);
        }

        private float SrgbToLinear(byte sb)
        {
            float s = sb / 255.0f;

            float l = s <= 0.04045f
                ? s / 12.92f
                : (float)Math.Pow((s + 0.055)/1.055, 2.4);

            return l;
        }
    }
}
