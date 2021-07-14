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

namespace SwptSaveEditor.Undo
{
    /// <summary>
    /// Interface for an undo unit
    /// </summary>
    internal interface IUndoUnit
    {
        /// <summary>
        /// Returns whether this unit modifies data
        /// </summary>
        /// <remarks>
        /// Units that do not modify data are considered insignficant by the undo service. When performing undo or redo,
        /// the service will continue processing units from the stack until it reaches a data change unit.
        /// </remarks>
        bool IsDataChange { get; }

        /// <summary>
        /// Undo the change that this unit is responsible for
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo the change that this unit is responsible for
        /// </summary>
        void Redo();
    }
}
