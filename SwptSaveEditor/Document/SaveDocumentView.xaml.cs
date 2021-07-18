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
            ViewModel.FilterElement = FilterTextBox;
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // DataGrid eats delete key presses when it has focus. We intercept it here first and decide if we want to delete the selected row.
            // This is also handled in MainWindow.xaml.cs for the case where the grid does not have focus.
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Delete)
            {
                DataGrid grid = (DataGrid)sender;
                DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(grid.SelectedIndex);
                if (!row.IsEditing)
                {
                    if (ViewModel.RemovePropertyCommand.CanExecute(null))
                    {
                        ViewModel.RemovePropertyCommand.Execute(null);
                        e.Handled = true;
                        grid.Focus();
                    }
                }
            }
        }
    }
}
