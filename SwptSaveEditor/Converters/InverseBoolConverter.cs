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

namespace SwptSaveEditor.Converters
{
    /// <summary>
    /// Converts a boolean value to the opposite boolean value
    /// </summary>
    internal class InverseBoolConverter : IValueConverter
    {
        /// <summary>
        /// Static instance of this converter
        /// </summary>
        public static InverseBoolConverter Instance { get; }

        static InverseBoolConverter()
        {
            Instance = new InverseBoolConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return DependencyProperty.UnsetValue;
        }
    }
}
