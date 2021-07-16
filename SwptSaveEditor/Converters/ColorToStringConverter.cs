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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SwptSaveEditor.Converters
{
    /// <summary>
    /// Converts a Color to a hex string
    /// </summary>
    internal class ColorToStringConverter : IValueConverter
    {
        /// <summary>
        /// Static instance of this converter
        /// </summary>
        public static ColorToStringConverter Instance { get; }

        static ColorToStringConverter()
        {
            Instance = new ColorToStringConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return $"{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                s = s.Trim();
                if (s.Length == 6)
                {
                    s = $"ff{s}";
                }
                if (s.Length == 8)
                {
                    byte a, r, g, b;
                    if (byte.TryParse(s.Substring(0, 2), NumberStyles.HexNumber, null, out a) &&
                        byte.TryParse(s.Substring(2, 2), NumberStyles.HexNumber, null, out r) &&
                        byte.TryParse(s.Substring(4, 2), NumberStyles.HexNumber, null, out g) &&
                        byte.TryParse(s.Substring(6, 2), NumberStyles.HexNumber, null, out b))
                    {
                        return Color.FromArgb(a, r, g, b);
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
