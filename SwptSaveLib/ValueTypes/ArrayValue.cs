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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace SwptSaveLib.ValueTypes
{
    /// <summary>
    /// Represents a SavePropertry value of type Array
    /// </summary>
    public class ArrayValue : SaveValue, IEnumerable<SaveValue>
    {
        private ObservableCollection<SaveValue> mData;

        public override object Data
        {
            get => mData;
            set
            {
                var oldValue = mData;
                if (Set(ref mData, (ObservableCollection<SaveValue>)value))
                {
                    if (oldValue != null) oldValue.CollectionChanged -= Data_CollectionChanged;
                    if (mData != null) mData.CollectionChanged += Data_CollectionChanged;

                    NotifyPropertyChanged(nameof(DisplayString));
                }
            }
        }

        public string DisplayString
        {
            get
            {
                if (mData.Count > 0 && mData.Count <= 8)
                {
                    return string.Join(", ", mData);
                }
                return $"{mData.Count} Items";
            }
        }

        public SaveValueType ItemType { get; }

        public override string DisplayType => $"{ItemType.DisplayName} {Type.DisplayName}";

        public int Count => mData.Count;

        public SaveValue this[int index]
        {
            get => mData[index];
            set => mData[index] = value;
        }

        public ArrayValue(SaveValueType itemType)
            : base(SaveValueTypes.Array)
        {
            Data = mData = new ObservableCollection<SaveValue>();
            ItemType = itemType;
        }

        protected internal override void Deserialize(BinaryReader reader)
        {
            mData.Clear();

            if (reader.ReadByte() != 0) throw new FormatException("Unexpected value in save file");

            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                mData.Add(Load(reader, ItemType, false));
            }
        }

        protected internal override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write(mData.Count);
            foreach (SaveValue datum in mData)
            {
                datum.Serialize(writer);
            }
        }

        public override object CloneData()
        {
            var clone = new ObservableCollection<SaveValue>();
            foreach (SaveValue value in mData)
            {
                SaveValue newValue = Create(ItemType);
                newValue.Data = value.CloneData();
                clone.Add(newValue);
            }
            return clone;
        }

        public override bool CompareData(object data)
        {
            if (data == null) return mData == null;

            ObservableCollection<SaveValue> other = data as ObservableCollection<SaveValue>;
            if (other == null) return false;
            if (other.Count != mData.Count) return false;

            for (int i = 0; i < mData.Count; ++i)
            {
                if (!mData[i].Data.Equals(other[i].Data)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{mData.Count} items";
        }

        public int IndexOfItem(SaveValue item)
        {
            return mData.IndexOf(item);
        }

        public void MoveItemDown(int index)
        {
            if (index >= mData.Count - 1) return;

            SaveValue val = mData[index];
            mData.RemoveAt(index);
            mData.Insert(index + 1, val);
        }

        public void MoveItemUp(int index)
        {
            if (index < 1 || index >= mData.Count) return;

            SaveValue val = mData[index];
            mData.RemoveAt(index);
            mData.Insert(index - 1, val);
        }

        public SaveValue CreateItem()
        {
            return Create(ItemType);
        }

        public void AddItem(SaveValue item)
        {
            mData.Add(item);
        }

        public void InsertItem(int index, SaveValue item)
        {
            mData.Insert(index, item);
        }

        public void RemoveItem(int index)
        {
            mData.RemoveAt(index);
        }

        public void RemoveItems(IEnumerable items)
        {
            foreach (SaveValue item in items.Cast<SaveValue>().ToArray())
            {
                mData.Remove(item);
            }
        }

        public void ClearItems()
        {
            mData.Clear();
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(DisplayString));
        }

        IEnumerator<SaveValue> IEnumerable<SaveValue>.GetEnumerator()
        {
            return mData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)mData).GetEnumerator();
        }
    }
}
