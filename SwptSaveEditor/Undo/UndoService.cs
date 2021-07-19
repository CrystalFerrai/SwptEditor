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
using System.Collections.Generic;
using System.ComponentModel;

namespace SwptSaveEditor.Undo
{
    /// <summary>
    /// Default undo service implementation
    /// </summary>
    internal class UndoService : IUndoService, IDisposable, INotifyPropertyChanged
    {
        private readonly Stack<UndoGroup> mUndoStack;

        private readonly Stack<UndoGroup> mRedoStack;

        private int? mSavePointUndoIndex;

        public bool CanUndo => mUndoStack.Count > 0;

        public bool CanRedo => mRedoStack.Count > 0;

        public bool IsSavePoint
        {
            get
            {
                if (!mSavePointUndoIndex.HasValue)
                {
                    return false;
                }
                int count = mUndoStack.Count > 0 ? (mUndoStack.Peek().IsDataChange ? mUndoStack.Count : mUndoStack.Count - 1) : 0;
                return count == mSavePointUndoIndex.Value;
            }
        }

        public event EventHandler StateChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public UndoService()
        {
            mUndoStack = new Stack<UndoGroup>();
            mRedoStack = new Stack<UndoGroup>();
            mSavePointUndoIndex = 0;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        ~UndoService()
        {
            Dispose(false);
        }

        public void PushUndoUnit(IUndoUnit unit)
        {
            if (mSavePointUndoIndex.HasValue && mUndoStack.Count < mSavePointUndoIndex.Value)
            {
                mSavePointUndoIndex = null;
            }
            mRedoStack.Clear();

            if (unit.IsDataChange || mUndoStack.Count == 0)
            {
                UndoGroup group = new UndoGroup() { unit };
                mUndoStack.Push(group);
            }
            else
            {
                UndoGroup group = mUndoStack.Peek();
                group.Add(unit);
            }

            FireStateChanged();
        }

        public void Undo()
        {
            if (CanUndo)
            {
                UndoGroup unit = mUndoStack.Pop();
                unit.Undo();
                mRedoStack.Push(unit);
                FireStateChanged();
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                UndoGroup unit = mRedoStack.Pop();
                unit.Redo();
                mUndoStack.Push(unit);
                FireStateChanged();
            }
        }

        public void Clear()
        {
            if (CanUndo || CanRedo)
            {
                mUndoStack.Clear();
                mRedoStack.Clear();
                mSavePointUndoIndex = 0;
                FireStateChanged();
            }
        }

        public void SetSavePoint()
        {
            mSavePointUndoIndex = mUndoStack.Count;
            FireStateChanged();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                StateChanged = null;

                mUndoStack.Clear();
                mRedoStack.Clear();
            }
        }

        private void FireStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSavePoint)));
        }
    }
}
