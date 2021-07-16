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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SwptSaveEditor.Behaviors
{
    /// <summary>
    /// Attached properties for text boxes
    /// </summary>
    internal class TextBoxBehavior
    {
        // ----------------------------------------------------------------------------------------------------------------------------------------------
        // CommitOnEnter
        // Commit changes to the text in the text box if the enter key is pressed while it has keyboard focus.
        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly DependencyProperty CommitOnEnterProperty = DependencyProperty.RegisterAttached("CommitOnEnter", typeof(bool), typeof(TextBoxBehavior), new FrameworkPropertyMetadata(false, (d, e) => OnCommitOnEnterChanged(d as TextBoxBase, (bool)e.OldValue, (bool)e.NewValue)));

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static bool GetCommitOnEnter(TextBoxBase textBox)
        {
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));
            return (bool)textBox.GetValue(CommitOnEnterProperty);
        }

        public static void SetCommitOnEnter(TextBoxBase textBox, bool value)
        {
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));
            textBox.SetValue(CommitOnEnterProperty, value);
        }

        private static void OnCommitOnEnterChanged(TextBoxBase textBox, bool oldValue, bool newValue)
        {
            if (textBox == null) return;

            if (oldValue)
            {
                textBox.KeyDown -= TextBox_KeyDown;
                textBox.KeyUp -= TextBox_KeyUp;
            }
            if (newValue)
            {
                textBox.KeyDown += TextBox_KeyDown;
                textBox.KeyUp += TextBox_KeyUp;
            }
        }

        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private static void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null) return;

            if (e.Key == Key.Enter)
            {
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                e.Handled = true;
            }
        }
    }
}
