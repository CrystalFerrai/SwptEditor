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

using SwptSaveEditor.Utils;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace SwptSaveEditor.Input
{
    /// <summary>
    /// Helper to reduce boilerplate for owners of input actions which use delegate commands
    /// </summary>
    internal class DelegateInputAction : InputAction
    {
        public DelegateInputAction(string name, Key key, ModifierKeys modifiers, ImageSource icon, Action execute, Func<bool> canExecute = null)
            : base(name, new DelegateCommand(execute, canExecute), key, modifiers, icon)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            ((DelegateCommand)Command).RaiseCanExecuteChanged();
        }
    }
}
