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

using SwptSaveEditor.Undo;
using SwptSaveEditor.Utils;
using SwptSaveLib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// View model for SaveFile instances being displayed in the editor
    /// </summary>
    internal class SaveDocument : ViewModelBase
    {
        private readonly SaveFile mFile;

        private readonly UndoService mUndoService;

        private readonly DelegateCommand mUndoCommand;
        private readonly DelegateCommand mRedoCommand;
        private readonly DelegateCommand mSaveCommand;
        private readonly DelegateCommand mReloadCommand;

        public string Name => mFile.Name;

        public IReadOnlyList<SaveProperty> Properties => mFile.Properties;

        public IUndoService UndoService => mUndoService;

        public ICommand UndoCommand => mUndoCommand;

        public ICommand RedoCommand => mRedoCommand;

        public ICommand SaveCommand => mSaveCommand;

        public ICommand ReloadCommand => mReloadCommand;

        public SaveDocument(IServiceProvider services, SaveFile file)
        {
            mFile = file;
            mUndoService = new UndoService();

            mUndoCommand = new DelegateCommand(mUndoService.Undo, () => mUndoService.CanUndo);
            mRedoCommand = new DelegateCommand(mUndoService.Redo, () => mUndoService.CanRedo);
            mSaveCommand = new DelegateCommand(Save, () => !mUndoService.IsSavePoint);
            mReloadCommand = new DelegateCommand(Reload);

            mUndoService.StateChanged += UndoService_StateChanged;
        }

        private void UndoService_StateChanged(object sender, EventArgs e)
        {
            mUndoCommand.RaiseCanExecuteChanged();
            mRedoCommand.RaiseCanExecuteChanged();
            mSaveCommand.RaiseCanExecuteChanged();
        }

        public void RecordValueEdit(SaveValue value, object oldData)
        {
            if (value.CompareData(oldData)) return;

            object newData = value.Data;
            DelegateUndoUnit unit = DelegateUndoUnit.Create(() => value.Data = newData, () => value.Data = oldData);
            mUndoService.PushUndoUnit(unit);
        }

        public void Save()
        {
            mFile.Save();
            mUndoService.SetSavePoint();
            mSaveCommand.RaiseCanExecuteChanged();
        }

        private void Reload()
        {
            if (!mUndoService.IsSavePoint)
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Reloading from disk will discard unsaved changes and clear your undo history for this file. This action cannot be undone. Are you sure?", "Confirm Reload", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }
            else if (mUndoService.CanUndo || mUndoService.CanRedo)
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Reloading from disk will clear your undo history for this file. Are you sure?", "Confirm Reload", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }
            mFile.Reload();
            mUndoService.Clear();
        }
    }
}
