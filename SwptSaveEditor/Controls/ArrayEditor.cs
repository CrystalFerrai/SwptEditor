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
using SwptSaveLib.ValueTypes;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwptSaveEditor.Controls
{
    /// <summary>
    /// Control for editing a <see cref="ArrayValue"/> instance in a data grid
    /// </summary>
    internal class ArrayEditor : Control
    {
        private readonly DelegateCommand mAddItemCommand;
        private readonly DelegateCommand<IEnumerable> mDeleteItemsCommand;
        private readonly DelegateCommand mClearItemsCommand;

        private ArrayValue Data => DataContext as ArrayValue;

        public ICommand AddItemCommand => mAddItemCommand;

        public ICommand DeleteItemsCommand => mDeleteItemsCommand;

        public ICommand ClearItemsCommand => mClearItemsCommand;

        public ArrayEditor()
        {
            mAddItemCommand = new DelegateCommand(AddNewItem);
            mDeleteItemsCommand = new DelegateCommand<IEnumerable>(DeleteItems);
            mClearItemsCommand = new DelegateCommand(ClearItems);
        }

        private void AddNewItem()
        {
            Data?.AddNewItem();
        }

        private void DeleteItems(IEnumerable items)
        {
            Data?.DeleteItems(items);
        }

        private void ClearItems()
        {
            Data?.ClearItems();
        }
    }
}
