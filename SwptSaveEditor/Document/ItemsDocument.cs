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
using SwptSaveEditor.Utils;
using SwptSaveLib;
using SwptSaveLib.ValueTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// Basic mode document for managing the full list of saved items
    /// </summary>
    internal class ItemsDocument : DocumentBase
    {
        private readonly InputService mInputService;
        private readonly DocumentService mDocumentService;

        private readonly ObservableCollection<GameItem> mGameItems;

        private readonly Dictionary<int, int> mIdToIndexMap;

        public override string Name => "Items";

        public bool SuppressInputActions
        {
            get => mInputService.SuppressActions;
            set
            {
                if (mInputService.SuppressActions != value)
                {
                    mInputService.SuppressActions = value;
                    NotifyPropertyChanged(nameof(SuppressInputActions));
                }
            }
        }

        public IEnumerable<InputAction> ContextMenuItems { get; }

        public IEnumerable<GameItem> GameItems => mGameItems;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }
        private int _selectedIndex;

        public ItemsDocument(IServiceProvider services)
        {
            services.Inject(out mInputService);
            services.Inject(out mDocumentService);

            mGameItems = new ObservableCollection<GameItem>();
            mIdToIndexMap = new Dictionary<int, int>();

            SyncItems();
        }

        private void SyncItems()
        {
            ISaveDocument globalDoc = null;
            ISaveDocument itemsDoc = null;
            ISaveDocument storagesDoc = null;
            List<ISaveDocument> characterDocs = new List<ISaveDocument>();
            foreach (ISaveDocument doc in mDocumentService.AdvancedDocuments)
            {
                switch (doc.Name)
                {
                    case "Global":
                        globalDoc = doc;
                        break;
                    case "Items":
                        itemsDoc = doc;
                        break;
                    case "Storages":
                        storagesDoc = doc;
                        break;
                    case "Player":
                        characterDocs.Add(doc);
                        break;
                }
            }

            if (itemsDoc == null) return;

            foreach (SaveProperty property in itemsDoc.File.Properties)
            {
                int split = property.Name.IndexOf('-');
                if (split < 0) continue;

                int id;
                if (!int.TryParse(property.Name.Substring(split), out id)) continue;

                GameItem item = null;

                int index;
                if (mIdToIndexMap.TryGetValue(id, out index))
                {
                    item = mGameItems[index];
                }
                else
                {
                    item = new GameItem() { ID = id };
                    index = mGameItems.Count;
                    mGameItems.Add(item);
                    mIdToIndexMap.Add(id, index);
                }

                switch (property.Name.Substring(0, split))
                {
                    case "name":
                        item.Name = property.Value.GetData<string>();
                        break;
                    case "xy":
                        item.Position = property.Value.GetData<Vector2>();
                        break;
                    case "prefix":
                        item.Prefix = property.Value.GetData<string>();
                        break;
                    case "surfix":
                        item.Suffix = property.Value.GetData<string>();
                        break;
                }
            }

            if (storagesDoc != null)
            {
                const string IdArray = "idArray";
                foreach (SaveProperty property in storagesDoc.File.Properties)
                {
                    if (!property.Name.StartsWith(IdArray)) continue;
                    if (!(property.Value is ArrayValue)) continue;

                    string storageName = property.Name.Substring(IdArray.Length);

                    foreach(SaveValue value in (ArrayValue)property.Value)
                    {
                        if (!(value.Data is int)) continue;

                        int id = (int)value.Data;
                        int index;
                        if (!mIdToIndexMap.TryGetValue(id, out index)) continue;

                        mGameItems[index].Storage = storageName;
                    }
                }
            }

            if (globalDoc != null)
            {
                ArrayValue companionList = globalDoc.File.Properties.FirstOrDefault(p => p.Name == "companionlist")?.Value as ArrayValue;
                if (companionList != null)
                {
                    foreach(SaveValue value in companionList)
                    {
                        string companionName = value.GetData<string>();
                        if (companionName == null) continue;

                        foreach (ISaveDocument doc in mDocumentService.AdvancedDocuments)
                        {
                            if (companionName == doc.Name)
                            {
                                characterDocs.Add(doc);
                                break;
                            }
                        }
                    }
                }
            }

            foreach(ISaveDocument doc in characterDocs)
            {
                foreach (SaveProperty property in doc.File.Properties)
                {
                    if (property.Value is Int32Value value)
                    {
                        int index;
                        if (!mIdToIndexMap.TryGetValue(value.TypedData, out index)) continue;

                        mGameItems[index].Storage = $"{doc.Name} {property.Name}";
                    }
                }
            }
        }
    }

    internal class GameItem : ObservableObject
    {
        public int ID
        {
            get => _id;
            set => Set(ref _id, value);
        }
        private int _id;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        private string _name;

        public string Prefix
        {
            get => _prefix;
            set => Set(ref _prefix, value);
        }
        private string _prefix;

        public string Suffix
        {
            get => _suffix;
            set => Set(ref _suffix, value);
        }
        private string _suffix;

        public string Storage
        {
            get => _storage;
            set => Set(ref _storage, value);
        }
        private string _storage;

        public Vector2 Position
        {
            get => _position;
            set => Set(ref _position, value);
        }
        private Vector2 _position;
    }
}
