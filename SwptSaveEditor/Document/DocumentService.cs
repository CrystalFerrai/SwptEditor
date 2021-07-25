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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// Service for managing documents
    /// </summary>
    internal class DocumentService : ObservableObject
    {
        private readonly ObservableCollection<IDocument> mBasicDocuments;
        private readonly ObservableCollection<ISaveDocument> mAdvancedDocuments;
        private readonly Dictionary<string, ISaveDocument> mAdvancedDocumentMap;

        /// <summary>
        /// Gets or sets whether the application is in advanced mode vs basic mode
        /// </summary>
        public bool IsAdvancedMode
        {
            get => _isAdvancedMode;
            set => Set(ref _isAdvancedMode, value);
        }
        private bool _isAdvancedMode;

        /// <summary>
        /// Gets the collection of available documents for basic mode
        /// </summary>
        public IList<IDocument> BasicDocuments => mBasicDocuments;

        /// <summary>
        /// Gets the collection of available documents for advanced mode
        /// </summary>
        public IList<ISaveDocument> AdvancedDocuments => mAdvancedDocuments;

        /// <summary>
        /// Provides a way to lookup advanced docuemtns by name
        /// </summary>
        public IReadOnlyDictionary<string, ISaveDocument> AdvancedDocumentMap => mAdvancedDocumentMap;

        /// <summary>
        /// Gets or sets the currently active basic mode document
        /// </summary>
        public IDocument BasicActiveDocument
        {
            get => _basicActiveDocument;
            set => Set(ref _basicActiveDocument, value);
        }
        private IDocument _basicActiveDocument;

        /// <summary>
        /// Gets or sets the currently active advanced mode document
        /// </summary>
        public ISaveDocument AdvancedActiveDocument
        {
            get => _advancedActiveDocument;
            set => Set(ref _advancedActiveDocument, value);
        }
        private ISaveDocument _advancedActiveDocument;

        public DocumentService()
        {
            mAdvancedDocuments = new ObservableCollection<ISaveDocument>();
            mBasicDocuments = new ObservableCollection<IDocument>();
            mAdvancedDocumentMap = new Dictionary<string, ISaveDocument>();

            BuildAdvancedMap();
            mAdvancedDocuments.CollectionChanged += AdvancedDocuments_CollectionChanged;
        }

        private void AdvancedDocuments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ISaveDocument doc in e.NewItems)
                    {
                        mAdvancedDocumentMap.Add(doc.Name, doc);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ISaveDocument doc in e.OldItems)
                    {
                        mAdvancedDocumentMap.Remove(doc.Name);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (ISaveDocument doc in e.OldItems)
                    {
                        mAdvancedDocumentMap.Remove(doc.Name);
                    }
                    foreach (ISaveDocument doc in e.NewItems)
                    {
                        mAdvancedDocumentMap.Add(doc.Name, doc);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    BuildAdvancedMap();
                    break;
            }
        }

        private void BuildAdvancedMap()
        {
            mAdvancedDocumentMap.Clear();
            foreach (ISaveDocument doc in mAdvancedDocuments)
            {
                mAdvancedDocumentMap.Add(doc.Name, doc);
            }
        }

        /// <summary>
        /// Returns the currently active document, accounting for the current application mode
        /// </summary>
        public IDocument GetActiveDocument()
        {
            return IsAdvancedMode ? AdvancedActiveDocument : BasicActiveDocument;
        }
    }
}
