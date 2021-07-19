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
using System.Windows;

namespace SwptSaveEditor.Dialogs
{
    /// <summary>
    /// A dialog for renaming a property
    /// </summary>
    internal class RenamePropertyDialog : ViewModelBase
    {
        /// <summary>
        /// Get the property name
        /// </summary>
        public string PropertyName
        {
            get => _propertyName;
            set
            {
                if (Set(ref _propertyName, value))
                {
                    NotifyPropertyChanged(nameof(IsValid));
                }
            }
        }
        private string _propertyName;

        public bool IsValid => !string.IsNullOrWhiteSpace(PropertyName);

        public RenamePropertyDialog(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>True if the user pressed OK. False if the user pressed Cancel.</returns>
        public bool? ShowDialog(Window owner)
        {
            RenamePropertyDialogView dialog = new RenamePropertyDialogView()
            {
                Owner = owner,
                DataContext = this
            };

            return dialog.ShowDialog();
        }
    }
}
