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
    /// An undo unit that calls owner-defined execute and undo actions.
    /// </summary>
    internal class DelegateUndoUnit : IUndoUnit
    {
        private readonly Action mExecute;
        private readonly Action mUndo;

        public bool IsDataChange { get; }

        protected DelegateUndoUnit(Action execute, Action undo, bool isDataChange = true)
        {
            mExecute = execute;
            mUndo = undo;
            IsDataChange = isDataChange;
        }

        public static DelegateUndoUnit Create(Action execute, Action undo)
        {
            return new DelegateUndoUnit(execute, undo);
        }

        public static DelegateUndoUnit CreateAndExecute(Action execute, Action undo)
        {
            execute();
            return new DelegateUndoUnit(execute, undo);
        }

        public void Undo()
        {
            mUndo();
        }

        public void Redo()
        {
            mExecute();
        }
    }

    /// <summary>
    /// An undo unit that calls owner-defined execute and undo actions which take a parameter.
    /// </summary>
    internal class DelegateUndoUnit<T> : IUndoUnit
    {
        private readonly Action<T> mExecute;
        private readonly Action<T> mUndo;
        private readonly T mParam;

        public bool IsDataChange { get; }

        protected DelegateUndoUnit(Action<T> execute, Action<T> undo, T param, bool isDataChange = true)
        {
            mExecute = execute;
            mUndo = undo;
            mParam = param;
            IsDataChange = isDataChange;
        }

        public static DelegateUndoUnit<T> Create(Action<T> execute, Action<T> undo, T param)
        {
            return new DelegateUndoUnit<T>(execute, undo, param);
        }

        public static DelegateUndoUnit<T> CreateAndExecute(Action<T> execute, Action<T> undo, T param)
        {
            execute(param);
            return new DelegateUndoUnit<T>(execute, undo, param);
        }

        public void Undo()
        {
            mUndo(mParam);
        }

        public void Redo()
        {
            mExecute(mParam);
        }
    }
}
