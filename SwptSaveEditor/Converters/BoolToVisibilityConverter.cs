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
    /// Convers from a bool to a Visibility
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Static instance with true=visible and false=collapsed
        /// </summary>
        public static BoolToVisibilityConverter CollapseInstance;

        /// <summary>
        /// Static instance with true=visible and false=hidden
        /// </summary>
        public static BoolToVisibilityConverter HideInstance;

        /// <summary>
        /// Static instance with true=collapsed and false=visible
        /// </summary>
        public static BoolToVisibilityConverter InvertedInstance;

        /// <summary>
        /// The visibility to convert from a true value
        /// </summary>
        public Visibility TrueState { get; set; } = Visibility.Visible;

        /// <summary>
        /// The visibility to convert from a false value
        /// </summary>
        public Visibility FalseState { get; set; } = Visibility.Collapsed;

        static BoolToVisibilityConverter()
        {
            CollapseInstance = new BoolToVisibilityConverter();
            HideInstance = new BoolToVisibilityConverter() { FalseState = Visibility.Hidden };
            InvertedInstance = new BoolToVisibilityConverter() { TrueState = Visibility.Collapsed, FalseState = Visibility.Visible };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? TrueState : FalseState;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v == TrueState;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
