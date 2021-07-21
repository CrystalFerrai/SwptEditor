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

using SwptSaveEditor.Behaviors;
using SwptSaveEditor.Input;
using SwptSaveEditor.Undo;
using SwptSaveLib;
using SwptSaveLib.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SwptSaveEditor.Controls
{
    /// <summary>
    /// Control for editing a <see cref="ArrayValue"/> instance in a data grid
    /// </summary>
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    internal class ArrayEditor : Control, IDataGridOwner
    {
        private readonly UndoService mUndoService;

        private readonly List<InputAction> mActions;

        private readonly DelegateInputAction mCommitAction;
        private readonly DelegateInputAction mUndoAction;
        private readonly DelegateInputAction mRedoAction;
        private readonly DelegateInputAction mMoveItemDownAction;
        private readonly DelegateInputAction mMoveItemUpAction;
        private readonly DelegateInputAction mAddItemAction;
        private readonly DelegateInputAction mDeleteItemsAction;
        private readonly DelegateInputAction mClearItemsAction;

        private DataGrid mDataGrid;
        private bool mIsEditing;

        private ArrayValue Data => DataContext as ArrayValue;

        public InputAction CommitAction => mCommitAction;

        public InputAction UndoAction => mUndoAction;

        public InputAction RedoAction => mRedoAction;

        public InputAction MoveItemDownAction => mMoveItemDownAction;

        public InputAction MoveItemUpAction => mMoveItemUpAction;

        public InputAction AddItemAction => mAddItemAction;

        public InputAction DeleteItemsAction => mDeleteItemsAction;

        public InputAction ClearItemsAction => mClearItemsAction;

        public ArrayEditor()
        {
            mUndoService = new UndoService();
            mActions = new List<InputAction>();

            mActions.Add(mCommitAction = new DelegateInputAction("Commit Changes", Key.Enter, ModifierKeys.Alt, Images.ToolbarIcons.Checkmark, CommitChanges, () => !mUndoService.IsSavePoint));
            mActions.Add(mUndoAction = new DelegateInputAction("Undo", Key.Z, ModifierKeys.Control, Images.ToolbarIcons.Undo, mUndoService.Undo, () => mUndoService.CanUndo));
            mActions.Add(mRedoAction = new DelegateInputAction("Redo", Key.Y, ModifierKeys.Control, Images.ToolbarIcons.Redo, mUndoService.Redo, () => mUndoService.CanRedo));
            mActions.Add(mMoveItemDownAction = new DelegateInputAction("Move Item Down", Key.Down, ModifierKeys.Alt, Images.ToolbarIcons.MoveDown, MoveItemDown, () => mDataGrid?.SelectedItems.Count == 1 && mDataGrid?.SelectedIndex < Data?.Count - 1));
            mActions.Add(mMoveItemUpAction = new DelegateInputAction("Move Item Up", Key.Up, ModifierKeys.Alt, Images.ToolbarIcons.MoveUp, MoveItemUp, () => mDataGrid?.SelectedItems.Count == 1 && mDataGrid?.SelectedIndex > 0));
            mActions.Add(mAddItemAction = new DelegateInputAction("Add Item", Key.Insert, ModifierKeys.None, Images.ToolbarIcons.Add, AddNewItem));
            mActions.Add(mDeleteItemsAction = new DelegateInputAction("Delete Selected Items", Key.Delete, ModifierKeys.None, Images.ToolbarIcons.Remove, DeleteItems, () => mDataGrid?.SelectedItems.Count >= 0));
            mActions.Add(mClearItemsAction = new DelegateInputAction("Delete All Items", Key.Delete, ModifierKeys.Control, Images.ToolbarIcons.Close, ClearItems, () => Data?.Count > 0));

            Focusable = true;
            DataContextChanged += ArrayEditor_DataContextChanged;
            mUndoService.StateChanged += UndoService_StateChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (mDataGrid != null)
            {
                mDataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                mDataGrid.BeginningEdit -= DataGrid_BeginningEdit;
                mDataGrid.RowEditEnding -= DataGrid_RowEditEnding;
            }

            mDataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;

            if (mDataGrid != null)
            {
                mDataGrid.SelectionChanged += DataGrid_SelectionChanged;
                mDataGrid.BeginningEdit += DataGrid_BeginningEdit;
                mDataGrid.RowEditEnding += DataGrid_RowEditEnding;
            }
        }

        private void ArrayEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ArrayValue ov)
            {
                if (ov.Data is INotifyCollectionChanged cc)
                {
                    WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.RemoveHandler(cc, nameof(INotifyCollectionChanged.CollectionChanged), Data_CollectionChanged);
                }
            }
            if (e.NewValue is ArrayValue nv)
            {
                if (nv.Data is INotifyCollectionChanged cc)
                {
                    WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.AddHandler(cc, nameof(INotifyCollectionChanged.CollectionChanged), Data_CollectionChanged);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!mIsEditing)
            {
                InputService.ProcessActions(mActions, e);
            }

            base.OnPreviewKeyDown(e);
        }

        private void UndoService_StateChanged(object sender, EventArgs e)
        {
            mCommitAction.RaiseCanExecuteChanged();
            mUndoAction.RaiseCanExecuteChanged();
            mRedoAction.RaiseCanExecuteChanged();
            ResetFocus();
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            mClearItemsAction.RaiseCanExecuteChanged();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mMoveItemDownAction.RaiseCanExecuteChanged();
            mMoveItemUpAction.RaiseCanExecuteChanged();
            mDeleteItemsAction.RaiseCanExecuteChanged();
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            mIsEditing = true;
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            mIsEditing = false;
        }

        private void MoveItemDown()
        {
            if (Data == null || mDataGrid == null) return;

            int index = mDataGrid.SelectedIndex;

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    Data.MoveItemDown(index);
                    mDataGrid.SelectedIndex = index + 1;
                },
                () =>
                {
                    Data.MoveItemUp(index + 1);
                    mDataGrid.SelectedIndex = index;
                });
            mUndoService.PushUndoUnit(unit);

            ResetFocus();
        }

        private void MoveItemUp()
        {
            if (Data == null || mDataGrid == null) return;

            int index = mDataGrid.SelectedIndex;

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    Data.MoveItemUp(index);
                    mDataGrid.SelectedIndex = index - 1;
                },
                () =>
                {
                    Data.MoveItemDown(index - 1);
                    mDataGrid.SelectedIndex = index;
                });
            mUndoService.PushUndoUnit(unit);

            ResetFocus();
        }

        private void AddNewItem()
        {
            if (Data == null || mDataGrid == null) return;

            int index = mDataGrid.SelectedIndex + 1;
            if (index <= 0) index = Data.Count;

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    Data.InsertNewItem(index);
                    mDataGrid.SelectedIndex = index;
                },
                () =>
                {
                    Data.RemoveItem(index);
                    mDataGrid.SelectedIndex = index;
                });
            mUndoService.PushUndoUnit(unit);

            ResetFocus();
        }

        private void DeleteItems()
        {
            if (Data == null || mDataGrid == null) return;

            int index = mDataGrid.SelectedIndex;
            int[] indices = mDataGrid.SelectedItems.Cast<SaveValue>().Select(i => Data.IndexOfItem(i)).ToArray();
            IList items = new ArrayList(mDataGrid.SelectedItems);
            object oldData = Data.CloneData();
            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    Data.RemoveItems(items);
                    if (index >= 0)
                    {
                        if (index < Data.Count) mDataGrid.SelectedIndex = index;
                        else mDataGrid.SelectedIndex = Data.Count - 1;
                    }
                    else
                    {
                        if (Data.Count > 0) mDataGrid.SelectedIndex = 0;
                    }
                },
                () =>
                {
                    Data.Data = oldData;
                    mDataGrid.SelectedItems.Clear();
                    foreach (int i in indices)
                    {
                        mDataGrid.SelectedItems.Add(Data[i]);
                    }
                });
            mUndoService.PushUndoUnit(unit);

            ResetFocus();
        }

        private void ClearItems()
        {
            int[] indices = mDataGrid.SelectedItems.Cast<SaveValue>().Select(i => Data.IndexOfItem(i)).ToArray();
            IList items = new ArrayList(mDataGrid.SelectedItems);
            object oldData = Data.CloneData();
            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    Data.ClearItems();
                },
                () =>
                {
                    Data.Data = oldData;
                    foreach (int i in indices)
                    {
                        mDataGrid.SelectedItems.Add(Data[i]);
                    }
                });
            mUndoService.PushUndoUnit(unit);

            ResetFocus();
        }

        private void ResetFocus()
        {
            if (mDataGrid == null || mDataGrid.SelectedIndex < 0)
            {
                Keyboard.Focus(this);
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                mDataGrid.UpdateLayout();
                mDataGrid.ScrollIntoView(mDataGrid.SelectedItem);
                DataGridRow row = (DataGridRow)mDataGrid.ItemContainerGenerator.ContainerFromIndex(mDataGrid.SelectedIndex);
                if (row != null)
                {
                    DataGridCell cell = mDataGrid.Columns[0].GetCellContent(row)?.Parent as DataGridCell;
                    if (cell != null)
                    {
                        Keyboard.Focus(cell);
                    }
                }
            }));
        }

        private void CommitChanges()
        {
            for (DependencyObject current = VisualTreeHelper.GetParent(this); current is Visual; current = VisualTreeHelper.GetParent(current))
            {
                if (current is DataGrid grid)
                {
                    grid.CommitEdit(DataGridEditingUnit.Row, true);
                    break;
                }
            }
        }

        void IDataGridOwner.RecordValueEdit(SaveValue value, object oldData)
        {
            if (value.CompareData(oldData)) return;

            object newData = value.Data;
            DelegateUndoUnit unit = DelegateUndoUnit.Create(() => value.Data = newData, () => value.Data = oldData);
            mUndoService.PushUndoUnit(unit);
        }
    }
}
