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

using System.Windows.Input;
using System.Windows.Media;

namespace SwptSaveEditor.Input
{
    /// <summary>
    /// Cotnains information about a keyboard input action
    /// </summary>
    internal class InputAction : InputGesture
    {
        public string Name { get; }

        public string Shortcut { get; }

        public string ToolTip { get; }

        public ImageSource Icon { get; }

        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public ICommand Command { get; }

        public InputAction(string name, ICommand command, Key key, ModifierKeys modifiers, ImageSource icon)
        {
            Name = name;
            Command = command;
            Key = key;
            Modifiers = modifiers;
            Icon = icon;
            Shortcut = GetShortcutString(modifiers, key);
            ToolTip = Name + (string.IsNullOrEmpty(Shortcut) ? string.Empty : $" ({Shortcut})");
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            KeyEventArgs args = inputEventArgs as KeyEventArgs;
            if (args == null) return false;

            return args.KeyboardDevice.Modifiers == Modifiers && (args.Key == Key || args.SystemKey == Key);
        }

        private static string GetShortcutString(ModifierKeys modifiers, Key key)
        {
            KeyConverter kc = new KeyConverter();
            string keyName = key == Key.Enter ? "Enter" : kc.ConvertToString(key);

            if (string.IsNullOrEmpty(keyName))
            {
                return string.Empty;
            }

            ModifierKeysConverter mc = new ModifierKeysConverter();
            string modName = mc.ConvertToString(modifiers);

            if (string.IsNullOrEmpty(modName))
            {
                return keyName;
            }
            return $"{modName}+{keyName}";
        }
    }
}
