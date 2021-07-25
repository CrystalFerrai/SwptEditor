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
using SwptSaveLib;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SwptSaveEditor.Dialogs
{
    /// <summary>
    /// Dialog for configuring options to create a new SaveProperty
    /// </summary>
    internal class NewPropertyDialog : ViewModelBase
    {
        private readonly SaveValueTypeWrapper[] mTypes;

        /// <summary>
        /// Get the entered name of the property
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

        /// <summary>
        /// Get the selected property type
        /// </summary>
        public SaveValueType PropertyType
        {
            get => _propertyType;
            set
            {
                if (Set(ref _propertyType, value))
                {
                    for (int i = 0; i < mTypes.Length; ++i)
                    {
                        if (mTypes[i].Type == value)
                        {
                            SelectedTypeIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        private SaveValueType _propertyType = SaveValueTypes.String;

        /// <summary>
        /// Get whether the property is an array
        /// </summary>
        public bool PropertyIsArray
        {
            get => _propertyIsArray;
            set => Set(ref _propertyIsArray, value);
        }
        private bool _propertyIsArray = false;

        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                if (Set(ref _selectedTypeIndex, value))
                {
                    PropertyType = mTypes[value].Type;
                }
            }
        }
        private int _selectedTypeIndex = 0;

        public bool IsValid => !string.IsNullOrWhiteSpace(PropertyName);

        public IEnumerable<SaveValueTypeWrapper> Types => mTypes;

        public NewPropertyDialog()
        {
            mTypes = SaveValueTypes.Instance.Enumarable.Select(t => new SaveValueTypeWrapper(t)).ToArray();
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>True if the user pressed OK. False if the user pressed Cancel.</returns>
        public bool? ShowDialog(Window owner)
        {
            NewPropertyDialogView dialog = new NewPropertyDialogView()
            {
                Owner = owner,
                DataContext = this
            };

            return dialog.ShowDialog();
        }

        internal class SaveValueTypeWrapper
        {
            public SaveValueType Type { get; }

            public string DisplayName { get; }

            public SaveValueTypeWrapper(SaveValueType type)
            {
                Type = type;
                DisplayName = type.DisplayName;
            }
        }
    }
}
