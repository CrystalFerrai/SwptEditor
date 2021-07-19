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

using SwptSaveEditor.Undo;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwptSaveEditor.Document
{
    internal partial class SaveDocumentView : UserControl
    {
        private SaveDocument ViewModel => (SaveDocument)DataContext;

        public SaveDocumentView()
        {
            InitializeComponent();

            Loaded += SaveDocumentView_Loaded;
            DataContextChanged += SaveDocumentView_DataContextChanged;
        }

        private void SaveDocumentView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.FilterElement = FilterTextBox;
        }

        private void SaveDocumentView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                WeakEventManager<SaveDocument, EventArgs>.RemoveHandler((SaveDocument)e.OldValue, nameof(SaveDocument.ResetFocus), ViewModel_ResetFocus);
            }

            ViewModel.FilterElement = FilterTextBox;
            WeakEventManager<SaveDocument, EventArgs>.AddHandler(ViewModel, nameof(SaveDocument.ResetFocus), ViewModel_ResetFocus);
        }

        private void ViewModel_ResetFocus(object sender, EventArgs e)
        {
            if (PropertyGrid.SelectedIndex >= 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PropertyGrid.UpdateLayout();
                    PropertyGrid.ScrollIntoView(PropertyGrid.SelectedItem);
                    DataGridRow row = (DataGridRow)PropertyGrid.ItemContainerGenerator.ContainerFromIndex(PropertyGrid.SelectedIndex);
                    if (row != null)
                    {
                        DataGridCell cell = PropertyGrid.Columns[1].GetCellContent(row)?.Parent as DataGridCell;
                        if (cell != null)
                        {
                            Keyboard.Focus(cell);
                        }
                    }
                }));
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // DataGrid eats certain key presses when it has focus. We intercept it here first and decide if we want to delete the selected row.
            // These are also handled in MainWindow.xaml.cs for the case where the grid does not have focus.

            DataGrid grid = (DataGrid)sender;
            if (grid.SelectedIndex < 0) return;

            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(grid.SelectedIndex);
            if (!row.IsEditing)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Delete)
                {
                    if (ViewModel.RemovePropertyCommand.CanExecute(null))
                    {
                        ViewModel.RemovePropertyCommand.Execute(null);
                        e.Handled = true;
                        grid.Focus();
                    }
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                {
                    if (ViewModel.CopyPropertyCommand.CanExecute(null))
                    {
                        ViewModel.CopyPropertyCommand.Execute(null);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
