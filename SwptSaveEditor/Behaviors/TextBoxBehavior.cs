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
using System.Windows.Threading;

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

        public static readonly DependencyProperty CommitOnEnterProperty = DependencyProperty.RegisterAttached("CommitOnEnter", typeof(bool), typeof(TextBoxBehavior),
            new FrameworkPropertyMetadata(false, (d, e) => OnCommitOnEnterChanged(d as TextBoxBase, (bool)e.OldValue, (bool)e.NewValue)));

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
                textBox.KeyDown -= CommitOnEnter_KeyDown;
                textBox.KeyUp -= CommitOnEnter_KeyUp;
            }
            if (newValue)
            {
                textBox.KeyDown += CommitOnEnter_KeyDown;
                textBox.KeyUp += CommitOnEnter_KeyUp;
            }
        }

        private static void CommitOnEnter_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private static void CommitOnEnter_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null) return;

            if (e.Key == Key.Enter)
            {
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                e.Handled = true;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------
        // SelectAllOnFocus
        // Select all text n the text box when it receives focus
        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextBoxBehavior),
            new FrameworkPropertyMetadata(false, (d, e) => OnSelectAllOnFocusChanged(d as TextBoxBase, (bool)e.OldValue, (bool)e.NewValue)));

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static bool GetSelectAllOnFocus(TextBoxBase textBox)
        {
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));
            return (bool)textBox.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetSelectAllOnFocus(TextBoxBase textBox, bool value)
        {
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));
            textBox.SetValue(SelectAllOnFocusProperty, value);
        }

        private static void OnSelectAllOnFocusChanged(TextBoxBase textBox, bool oldValue, bool newValue)
        {
            if (textBox == null) return;

            if (oldValue)
            {
                textBox.GotKeyboardFocus -= SelectAllOnFocus_GotKeyboardFocus;
            }
            if (newValue)
            {
                textBox.GotKeyboardFocus += SelectAllOnFocus_GotKeyboardFocus;
            }
        }

        private static void SelectAllOnFocus_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBoxBase textBox = sender as TextBoxBase;
            if (textBox == null) return;

            // SelectAll fails if the control has just been loaded. Putting the call at the end of the current
            // dispatcher queue seems to fix that issue.
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(textBox.SelectAll));
        }
    }
}
