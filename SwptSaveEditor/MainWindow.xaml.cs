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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SwptSaveEditor
{
    /// <summary>
    /// The main application window
    /// </summary>
    internal partial class MainWindow : Window
    {
        private MainWindowVM ViewModel => (MainWindowVM)DataContext;

        public MainWindow(MainWindowVM viewModel)
        {
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;

            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            ViewModel.OnFirstLoad();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.OnMainWindowClosing(e);
            base.OnClosing(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            // Consider implementing a proper shortcut key service at some point

            if (e.Handled == false)
            {
                SaveDocument doc = ViewModel.DocumentService.ActiveDocument;

                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.F5:
                            if (doc.ReloadCommand.CanExecute(null))
                            {
                                doc.ReloadCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                    }
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    switch (e.Key)
                    {
                        case Key.F4:
                            if (ViewModel.CloseCommand.CanExecute(null))
                            {
                                ViewModel.CloseCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                        case Key.N:
                            if (ViewModel.SaveAllAsCommand.CanExecute(null))
                            {
                                ViewModel.SaveAllAsCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                        case Key.O:
                            if (ViewModel.OpenCommand.CanExecute(null))
                            {
                                ViewModel.OpenCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                        case Key.R:
                            if (ViewModel.ReloadCommand.CanExecute(null))
                            {
                                ViewModel.ReloadCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                        case Key.S:
                            Keyboard.Focus(this);
                            doc.Save();
                            e.Handled = true;
                            break;
                        case Key.Y:
                            Keyboard.Focus(this);
                            doc.UndoService.Redo();
                            e.Handled = true;
                            break;
                        case Key.Z:
                            Keyboard.Focus(this);
                            doc.UndoService.Undo();
                            e.Handled = true;
                            break;
                    }
                }
                else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                {
                    switch (e.Key)
                    {
                        case Key.S:
                            if (ViewModel.SaveAllCommand.CanExecute(null))
                            {
                                Keyboard.Focus(this);
                                ViewModel.SaveAllCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                    }
                }
                else if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    switch (e.SystemKey)
                    {
                        case Key.Down:
                            if (doc.MovePropertyDownCommand.CanExecute(null))
                            {
                                doc.MovePropertyDownCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                        case Key.Up:
                            if (doc.MovePropertyUpCommand.CanExecute(null))
                            {
                                doc.MovePropertyUpCommand.Execute(null);
                            }
                            e.Handled = true;
                            break;
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Consider implementing a proper shortcut key service at some point

            if (e.Handled == false)
            {
                SaveDocument doc = ViewModel.DocumentService.ActiveDocument;

                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.Insert:
                            if (doc.AddPropertyCommand.CanExecute(null))
                            {
                                doc.AddPropertyCommand.Execute(null);
                                e.Handled = true;
                            }
                            break;
                        case Key.Delete:
                            // This key is also handled in SaveDocumentView.xaml.cs due to DataGrid eating Delete key presses while they have focus.
                            // This code only runs when the grid does not have keyboard focus.
                            if (doc.RemovePropertyCommand.CanExecute(null))
                            {
                                doc.RemovePropertyCommand.Execute(null);
                                e.Handled = true;
                            }    
                            break;
                    }
                }
            }
        }

        // The following block of functions works around a bug related to DataGrids within TabControls where switching tabs does not commit
        // pending edits in data grids. The basic idea is to intercept the inputs that can trigger a tab switch while editing a cell and
        // commit any pending edits before allowing the input to trigger the actual tab switch.

        private void TabControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            for (DependencyObject current = e.OriginalSource as DependencyObject; current != null && current is Visual; current = VisualTreeHelper.GetParent(current))
            {
                if (current is TabItem)
                {
                    CommitDataGrids((TabControl)sender);
                    break;
                }
            }
        }

        private void TabControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.Tab)
            {
                CommitDataGrids((TabControl)sender);
            }
        }

        private void CommitDataGrids(TabControl control)
        {
            // Focus on the tab control itself to unfocus the data grids and commit pending edits. This also makes TAB and Sift+TAB behave
            // properly for cycling through tabs.
            Keyboard.Focus(control);

            // Manually find and commit data grids starting with the most inner ones and working outwards.
            Stack<DependencyObject> objects = new Stack<DependencyObject>();
            objects.Push(control);

            Stack<DataGrid> grids = new Stack<DataGrid>();

            while (objects.Count > 0)
            {
                DependencyObject current = objects.Pop();
                if (current is DataGrid grid)
                {
                    grids.Push(grid);
                }

                int childCount = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < childCount; ++i)
                {
                    objects.Push(VisualTreeHelper.GetChild(current, i));
                }
            }

            while (grids.Count > 0)
            {
                DataGrid grid = grids.Pop();
                grid.CommitEdit(DataGridEditingUnit.Cell, true);
            }
        }
    }
}
