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

using SwptSaveEditor.Input;
using SwptSaveEditor.Undo;
using SwptSaveEditor.Utils;
using System.Collections.Generic;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// Optional base class for documents that helps implement the <see cref="IDocument"/> interface
    /// </summary>
    internal abstract class DocumentBase : ViewModelBase, IDocument
    {
        protected readonly UndoService mUndoService;
        protected readonly List<InputAction> mInputActions;

        public abstract string Name { get; }

        public IEnumerable<InputAction> InputActions => mInputActions;

        public IUndoService UndoService => mUndoService;

        protected DocumentBase()
        {
            mUndoService = new UndoService();
            mInputActions = new List<InputAction>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
