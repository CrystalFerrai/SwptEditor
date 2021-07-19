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

using SwptSaveLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace SwptSaveLib
{
    /// <summary>
    /// Represents a single file in a save game folder
    /// </summary>
    public class SaveFile : ObservableObject
    {
        private readonly BatchObservableCollection<SaveProperty> mProperties;

        private string mPath;

        public string Name { get; private set; }

        public IReadOnlyList<SaveProperty> Properties => mProperties;

        internal SaveFile(string path)
        {
            mProperties = new BatchObservableCollection<SaveProperty>();
            SetPath(path);
        }

        public static SaveFile Load(string path)
        {
            SaveFile file = new SaveFile(path);
            file.LoadFrom(path);
            return file;
        }

        public SaveFile Clone(string newName)
        {
            SaveFile file = new SaveFile(Path.Combine(Path.GetDirectoryName(mPath), newName, Path.GetExtension(mPath)));
            foreach (SaveProperty property in mProperties)
            {
                file.mProperties.Add(property.Clone());
            }
            return file;
        }

        public void Save()
        {
            using (FileStream stream = File.Create(mPath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                foreach (SaveProperty property in mProperties)
                {
                    property.Save(writer);
                }
            }
        }

        public void Save(string directory)
        {
            SetPath(Path.Combine(directory, Path.GetFileName(mPath)));
            Save();
        }

        public void Reload()
        {
            LoadFrom(mPath);
        }

        public void AddProperty(SaveProperty property)
        {
            mProperties.Add(property);
            mProperties.NotifyCollectionChanged();
        }

        public void InsertProperty(int index, SaveProperty property)
        {
            mProperties.Insert(index, property);
            mProperties.NotifyCollectionChanged();
        }

        public void RemoveProperty(int index)
        {
            mProperties.RemoveAt(index);
            mProperties.NotifyCollectionChanged();
        }

        public void MovePropertyDown(int index)
        {
            if (!CanMovePropertyDown(index)) return;

            SaveProperty property = mProperties[index];
            mProperties.RemoveAt(index);
            mProperties.Insert(index + 1, property);

            mProperties.NotifyCollectionChanged();
        }

        public void MovePropertyUp(int index)
        {
            if (!CanMovePropertyUp(index)) return;

            SaveProperty property = mProperties[index];
            mProperties.RemoveAt(index);
            mProperties.Insert(index - 1, property);

            mProperties.NotifyCollectionChanged();
        }

        public bool CanMovePropertyDown(int index)
        {
            return index < mProperties.Count - 1;
        }

        public bool CanMovePropertyUp(int index)
        {
            return index >= 1 && index < mProperties.Count;
        }

        public int IndexOfProperty(SaveProperty property)
        {
            return mProperties.IndexOf(property);
        }

        public int IndexOfProperty(string propertyName, int startIndex = 0)
        {
            for (int i = startIndex; i < mProperties.Count; ++i)
            {
                if (mProperties[i].Name.Equals(propertyName, StringComparison.Ordinal))
                {
                    return i;
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return Name;
        }

        private void SetPath(string path)
        {
            mPath = path;
            Name = Path.GetFileNameWithoutExtension(path);
            NotifyPropertyChanged(nameof(Name));
        }

        private void LoadFrom(string path)
        {
            mProperties.Clear();

            using (FileStream stream = File.OpenRead(path))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (stream.Position != stream.Length)
                {
                    mProperties.Add(SaveProperty.Load(reader));
                }
            }

            mProperties.NotifyCollectionChanged();
        }
    }
}
