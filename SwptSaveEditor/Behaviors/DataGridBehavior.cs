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

using SwptSaveEditor.Document;
using SwptSaveLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SwptSaveEditor.Behaviors
{
    /// <summary>
    /// Attached properties for DataGrid
    /// </summary>
    internal static class DataGridBehavior
    {
        // ----------------------------------------------------------------------------------------------------------------------------------------------
        // UseThreeStateSort
        // Sorting a column in the grid cycles through Ascending, Descending and Unsorted (instead of only Ascending and Descending)
        // ----------------------------------------------------------------------------------------------------------------------------------------------

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static bool GetUseThreeStateSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseThreeStateSortProperty);
        }

        public static void SetUseThreeStateSort(DependencyObject obj, bool value)
        {
            obj.SetValue(UseThreeStateSortProperty, value);
        }

        public static readonly DependencyProperty UseThreeStateSortProperty = DependencyProperty.RegisterAttached("UseThreeStateSort", typeof(bool), typeof(DataGridBehavior),
            new PropertyMetadata(false, UseThreeStateSort_Changed));

        private static void UseThreeStateSort_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid == null) return;

            bool attach = (bool)e.NewValue;
            if (attach)
            {
                grid.Sorting += Grid_Sorting;
            }
            else
            {
                grid.Sorting -= Grid_Sorting;
            }
        }

        private static void Grid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            ListSortDirection? direction = ListSortDirection.Ascending;
            if (e.Column.SortDirection == ListSortDirection.Ascending) direction = ListSortDirection.Descending;
            else if (e.Column.SortDirection == ListSortDirection.Descending) direction = null;

            using (grid.Items.DeferRefresh())
            {
                grid.Items.SortDescriptions.Clear();
                if (direction.HasValue)
                {
                    grid.Items.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, direction.Value));
                }
                e.Column.SortDirection = direction;
            }

            e.Handled = true;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------
        // IsSaveValueColumn
        // Applies special handling to the column to properly support cells containing a SaveValue instance as its data type
        // ----------------------------------------------------------------------------------------------------------------------------------------------

        private struct EditRecord
        {
            public SaveValue Value;
            public object DataBackup;
        }
        private static Stack<EditRecord> sEditBackupStack = new Stack<EditRecord>();

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static bool GetIsSaveValueColumn(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSaveValueColumnProperty);
        }

        public static void SetIsSaveValueColumn(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSaveValueColumnProperty, value);
        }

        public static readonly DependencyProperty IsSaveValueColumnProperty = DependencyProperty.RegisterAttached("IsSaveValueColumn", typeof(bool), typeof(DataGridBehavior),
            new PropertyMetadata(false, IsSaveValueColumn_Changed));

        private static void IsSaveValueColumn_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid == null) return;

            bool attach = (bool)e.NewValue;
            if (attach)
            {
                // Note: Must listen for row edit ending because cell edit ending is skipped when pressing Enter to commit.
                // This will likely break things if a grid has more than one editable column and this property is attached.
                grid.PreparingCellForEdit += Grid_PreparingCellForEdit;
                grid.CellEditEnding += Grid_CellEditEnding;
                grid.RowEditEnding += Grid_RowEditEnding;
            }
            else
            {
                grid.PreparingCellForEdit -= Grid_PreparingCellForEdit;
                grid.CellEditEnding -= Grid_CellEditEnding;
                grid.RowEditEnding -= Grid_RowEditEnding;
            }
        }

        private static void Grid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ContentPresenter container = e.EditingElement as ContentPresenter;
            if (container == null) return;

            SaveValue value = container.Content as SaveValue;
            if (value == null) return;

            sEditBackupStack.Push(new EditRecord() { Value = value, DataBackup = value.CloneData() });
        }

        private static void Grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            // Make every cell edit also a row edit.
            grid.CellEditEnding -= Grid_CellEditEnding;
            if (e.EditAction == DataGridEditAction.Commit)
            {
                grid.CommitEdit(DataGridEditingUnit.Row, true);
            }
            else
            {
                grid.CancelEdit(DataGridEditingUnit.Row);
            }
            grid.CellEditEnding += Grid_CellEditEnding;
        }

        private static void Grid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (sEditBackupStack.Count == 0) return;

            EditRecord record = sEditBackupStack.Pop();

            if (e.EditAction == DataGridEditAction.Commit)
            {
                SaveDocument doc = ((App)Application.Current).DocumentService.ActiveDocument;
                doc.RecordValueEdit(record.Value, record.DataBackup);
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                record.Value.Data = record.DataBackup;
            }
        }
    }
}
