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

namespace SwptSaveEditor.Undo
{
    /// <summary>
    /// Interface for an undo/redo service
    /// </summary>
    internal interface IUndoService
    {
        /// <summary>
        /// Is there an available undo unit?
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Is there an available redo unit?
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Is the current position in the undo stack marked as the save point of the document?
        /// </summary>
        bool IsSavePoint { get; }

        /// <summary>
        /// Event fired when the state of the service changes in any way
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Adds an undo unit to the undo stack and clears the redo stack
        /// </summary>
        void PushUndoUnit(IUndoUnit unit);

        /// <summary>
        /// Pops the undo unit from the top of the undo stack, calls Undo() on it, and pushes it onto the redo stack
        /// </summary>
        void Undo();

        /// <summary>
        /// Pops the undo unit from the top of the redo stack, calls Redo() on it, and pushes it onto the undo stack
        /// </summary>
        void Redo();

        /// <summary>
        /// Clears the undo and redo stacks as well as the save point
        /// </summary>
        void Clear();

        /// <summary>
        /// Marks the current stack position as a document save point
        /// </summary>
        void SetSavePoint();
    }
}
