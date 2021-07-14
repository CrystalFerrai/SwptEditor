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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SwptSaveEditor.Undo
{
    /// <summary>
    /// An undo unit that bundles multiple undo units together
    /// </summary>
    internal class UndoGroup : IUndoUnit, IList<IUndoUnit>, IReadOnlyList<IUndoUnit>
    {
        private readonly List<IUndoUnit> mUnits;

        public bool IsDataChange => mUnits.Any(u => u.IsDataChange);

        public int Count => mUnits.Count;

        public bool IsReadOnly => false;

        public IUndoUnit this[int index]
        {
            get => mUnits[index];
            set => mUnits[index] = value;
        }

        public UndoGroup()
        {
            mUnits = new List<IUndoUnit>();
        }

        public UndoGroup(int capacity)
        {
            mUnits = new List<IUndoUnit>(capacity);
        }

        public UndoGroup(IEnumerable<IUndoUnit> units)
        {
            mUnits = new List<IUndoUnit>(units);
        }

        public void Undo()
        {
            for (int i = mUnits.Count - 1; i >= 0; -- i)
            {
                mUnits[i].Undo();
            }
        }

        public void Redo()
        {
            for (int i = 0; i < mUnits.Count; ++i)
            {
                mUnits[i].Redo();
            }
        }

        public int IndexOf(IUndoUnit item)
        {
            return mUnits.IndexOf(item);
        }

        public void Insert(int index, IUndoUnit item)
        {
            mUnits.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            mUnits.RemoveAt(index);
        }

        public void Add(IUndoUnit item)
        {
            mUnits.Add(item);
        }

        public void AddRange(IEnumerable<IUndoUnit> collection)
        {
            mUnits.AddRange(collection);
        }

        public void Clear()
        {
            mUnits.Clear();
        }

        public bool Contains(IUndoUnit item)
        {
            return mUnits.Contains(item);
        }

        public void CopyTo(IUndoUnit[] array, int arrayIndex)
        {
            mUnits.CopyTo(array, arrayIndex);
        }

        public bool Remove(IUndoUnit item)
        {
            return mUnits.Remove(item);
        }

        public void RemoveRange(int index, int count)
        {
            mUnits.RemoveRange(index, count);
        }

        public IEnumerator<IUndoUnit> GetEnumerator()
        {
            return mUnits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mUnits.GetEnumerator();
        }
    }
}
