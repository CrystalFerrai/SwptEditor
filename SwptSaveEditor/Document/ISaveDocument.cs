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

using SwptSaveLib;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// Interface for a document that has savable content
    /// </summary>
    interface ISaveDocument : IDocument
    {
        /// <summary>
        /// Gets the data model for this document
        /// </summary>
        SaveFile File { get; }

        /// <summary>
        /// Saves the document
        /// </summary>
        void Save();
    }
}
