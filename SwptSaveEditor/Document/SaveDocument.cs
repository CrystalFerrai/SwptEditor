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
using SwptSaveEditor.Dialogs;
using SwptSaveEditor.Input;
using SwptSaveEditor.Undo;
using SwptSaveEditor.Utils;
using SwptSaveLib;
using SwptSaveLib.ValueTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SwptSaveEditor.Document
{
    /// <summary>
    /// View model for SaveFile instances being displayed in the editor
    /// </summary>
    internal class SaveDocument : ViewModelBase, ISaveDocument, IDataGridOwner
    {
        private readonly SaveFile mFile;

        private readonly UndoService mUndoService;
        private readonly InputService mInputService;

        private readonly List<InputAction> mInputActions;

        private readonly DelegateInputAction mUndoAction;
        private readonly DelegateInputAction mRedoAction;
        private readonly DelegateInputAction mSaveAction;
        private readonly DelegateInputAction mReloadAction;

        private readonly DelegateInputAction mFilterAction;
        private readonly DelegateInputAction mClearFilterAction;
        private readonly DelegateInputAction mRenamePropertyAction;
        private readonly DelegateInputAction mCopyPropertyAction;
        private readonly DelegateInputAction mPastePropertyAction;
        private readonly DelegateInputAction mMovePropertyUpAction;
        private readonly DelegateInputAction mAddPropertyAction;
        private readonly DelegateInputAction mRemovePropertyAction;
        private readonly DelegateInputAction mMovePropertyDownAction;

        public string Name => mFile.Name;

        public IReadOnlyList<SaveProperty> Properties => mFile.Properties;

        public SaveFile File => mFile;

        public IEnumerable<InputAction> ContextMenuItems { get; }

        public int SelectedPropertyIndex
        {
            get => _selectedPropertyIndex;
            set
            {
                if (Set(ref _selectedPropertyIndex, value))
                {
                    mRenamePropertyAction.RaiseCanExecuteChanged();
                    mCopyPropertyAction.RaiseCanExecuteChanged();
                    mMovePropertyDownAction.RaiseCanExecuteChanged();
                    mMovePropertyUpAction.RaiseCanExecuteChanged();
                    mRemovePropertyAction.RaiseCanExecuteChanged();
                }
            }
        }
        private int _selectedPropertyIndex;

        public ListSortDirection? NameSortDirection
        {
            get => _nameSortDirection;
            set
            {
                if (Set(ref _nameSortDirection, value))
                {
                    NotifyPropertyChanged(nameof(CanMoveProperties));
                    mMovePropertyDownAction.RaiseCanExecuteChanged();
                    mMovePropertyUpAction.RaiseCanExecuteChanged();
                }
            }
        }
        private ListSortDirection? _nameSortDirection = null;

        public ListSortDirection? TypeSortDirection
        {
            get => _typeSortDirection;
            set
            {
                if (Set(ref _typeSortDirection, value))
                {
                    NotifyPropertyChanged(nameof(CanMoveProperties));
                    mMovePropertyDownAction.RaiseCanExecuteChanged();
                    mMovePropertyUpAction.RaiseCanExecuteChanged();
                }
            }
        }
        private ListSortDirection? _typeSortDirection = null;

        public string PropertyFilter
        {
            get => _propertyFilter;
            set
            {
                if (Set(ref _propertyFilter, value))
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(mFile.Properties);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        view.Filter = null;
                    }
                    else
                    {
                        view.Filter = (obj) => ((SaveProperty)obj).Name.ToLowerInvariant().Contains(value.Trim().ToLowerInvariant());
                    }
                    NotifyPropertyChanged(nameof(CanMoveProperties));
                    mMovePropertyDownAction.RaiseCanExecuteChanged();
                    mMovePropertyUpAction.RaiseCanExecuteChanged();
                    mClearFilterAction.RaiseCanExecuteChanged();
                }
            }
        }
        private string _propertyFilter;

        public bool CanMoveProperties => NameSortDirection == null && TypeSortDirection == null && string.IsNullOrWhiteSpace(PropertyFilter);

        public IInputElement FilterElement { get; set; }

        public IUndoService UndoService => mUndoService;

        public InputAction UndoAction => mUndoAction;

        public InputAction RedoAction => mRedoAction;

        public InputAction SaveAction => mSaveAction;

        public InputAction ReloadAction => mReloadAction;

        public InputAction FilterAction => mFilterAction;

        public InputAction ClearFilterAction => mClearFilterAction;

        public InputAction RenamePropertyAction => mRenamePropertyAction;

        public InputAction CopyPropertyAction => mCopyPropertyAction;

        public InputAction PastePropertyAction => mPastePropertyAction;

        public InputAction MovePropertyDownAction => mMovePropertyDownAction;

        public InputAction MovePropertyUpAction => mMovePropertyUpAction;

        public InputAction AddPropertyAction => mAddPropertyAction;

        public InputAction RemovePropertyAction => mRemovePropertyAction;

        public IEnumerable<InputAction> InputActions => mInputActions;

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

        public event EventHandler ResetFocus;

        public SaveDocument(IServiceProvider services, SaveFile file)
        {
            services.Inject(out mInputService);

            mFile = file;
            mUndoService = new UndoService();

            mInputActions = new List<InputAction>();
            List<InputAction> contextMenuItems = new List<InputAction>();
            ContextMenuItems = contextMenuItems;

            mInputActions.Add(mUndoAction = new DelegateInputAction("Undo", Key.Z, ModifierKeys.Control, Images.ToolbarIcons.Undo, mUndoService.Undo, () => mUndoService.CanUndo));
            mInputActions.Add(mRedoAction = new DelegateInputAction("Redo", Key.Y, ModifierKeys.Control, Images.ToolbarIcons.Redo, mUndoService.Redo, () => mUndoService.CanRedo));
            mInputActions.Add(mSaveAction = new DelegateInputAction("Save", Key.S, ModifierKeys.Control, Images.ToolbarIcons.Save, Save, () => !mUndoService.IsSavePoint));
            mInputActions.Add(mReloadAction = new DelegateInputAction("Reload from Disk", Key.F5, ModifierKeys.None, Images.ToolbarIcons.Refresh, Reload));

            mInputActions.Add(mFilterAction = new DelegateInputAction("Filter Properties by Name", Key.F, ModifierKeys.Control, null, () => Keyboard.Focus(FilterElement)));
            mInputActions.Add(mClearFilterAction = new DelegateInputAction("Clear Filter", Key.Escape, ModifierKeys.None, Images.ToolbarIcons.CloseAlt, ClearFilter, () => !string.IsNullOrEmpty(PropertyFilter)));

            contextMenuItems.Add(mRenamePropertyAction = new DelegateInputAction("Rename Property", Key.F2, ModifierKeys.None, Images.ToolbarIcons.Rename, RenameProperty, () => SelectedPropertyIndex >= 0));
            contextMenuItems.Add(null);
            contextMenuItems.Add(mCopyPropertyAction = new DelegateInputAction("Copy Property", Key.C, ModifierKeys.Control, Images.ToolbarIcons.Copy, CopyProperty, () => SelectedPropertyIndex >= 0));
            contextMenuItems.Add(mPastePropertyAction = new DelegateInputAction("Paste Property", Key.V, ModifierKeys.Control, Images.ToolbarIcons.Paste, PasteProperty));
            contextMenuItems.Add(null);
            contextMenuItems.Add(mMovePropertyDownAction = new DelegateInputAction("Move Property Down", Key.Down, ModifierKeys.Alt, Images.ToolbarIcons.MoveDown, MovePropertyDown, CanMovePropertyDown));
            contextMenuItems.Add(mMovePropertyUpAction = new DelegateInputAction("Move Property Up", Key.Up, ModifierKeys.Alt, Images.ToolbarIcons.MoveUp, MovePropertyUp, CanMovePropertyUp));
            contextMenuItems.Add(null);
            contextMenuItems.Add(mAddPropertyAction = new DelegateInputAction("Add Property", Key.Insert, ModifierKeys.None, Images.ToolbarIcons.Add, AddProperty));
            contextMenuItems.Add(mRemovePropertyAction = new DelegateInputAction("Remove Property", Key.Delete, ModifierKeys.None, Images.ToolbarIcons.Remove, RemoveProperty, () => SelectedPropertyIndex >= 0));
            mInputActions.AddRange(contextMenuItems.Where(item => item != null));

            mUndoService.StateChanged += UndoService_StateChanged;
        }

        private void UndoService_StateChanged(object sender, EventArgs e)
        {
            mUndoAction.RaiseCanExecuteChanged();
            mRedoAction.RaiseCanExecuteChanged();
            mSaveAction.RaiseCanExecuteChanged();
            ResetFocus?.Invoke(this, EventArgs.Empty);
        }

        public void RecordValueEdit(SaveValue value, object oldData)
        {
            if (value.CompareData(oldData)) return;

            object newData = value.Data;
            DelegateUndoUnit unit = DelegateUndoUnit.Create(() => value.Data = newData, () => value.Data = oldData);
            mUndoService.PushUndoUnit(unit);
        }

        public void Save()
        {
            mFile.Save();
            mUndoService.SetSavePoint();
            mSaveAction.RaiseCanExecuteChanged();
        }

        public override string ToString()
        {
            return Name;
        }

        private void Reload()
        {
            if (!mUndoService.IsSavePoint)
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Reloading from disk will discard unsaved changes and clear your undo history for this file. This action cannot be undone. Are you sure?", "Confirm Reload", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        ResetFocus?.Invoke(this, EventArgs.Empty);
                        return;
                }
            }
            else if (mUndoService.CanUndo || mUndoService.CanRedo)
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Reloading from disk will clear your undo history for this file. Are you sure?", "Confirm Reload", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        ResetFocus?.Invoke(this, EventArgs.Empty);
                        return;
                }
            }
            mFile.Reload();
            mUndoService.Clear();
        }

        private void ClearFilter()
        {
            PropertyFilter = string.Empty;
        }

        private void RenameProperty()
        {
            SaveProperty property = GetSelectedProperty();
            string oldName = property.Name;

            RenamePropertyDialog dialog = new RenamePropertyDialog(oldName);
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                    () => property.Name = dialog.PropertyName,
                    () => property.Name = oldName);

                mUndoService.PushUndoUnit(unit);
            }
        }

        private void CopyProperty()
        {
            SaveProperty property = GetSelectedProperty();

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                property.Save(writer);

                for (int tries = 3; tries > 0; --tries)
                {
                    try
                    {
                        Clipboard.SetData(DataFormats.Serializable, stream.ToArray());
                        tries = 0;
                    }
                    catch (ExternalException)
                    {
                        Thread.Sleep(1);
                    }
                }
            }

            ResetFocus.Invoke(this, EventArgs.Empty);
        }

        private void PasteProperty()
        {
            try
            {
                byte[] data = Clipboard.GetData(DataFormats.Serializable) as byte[];
                if (data != null)
                {
                    using (MemoryStream stream = new MemoryStream(data))
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        SaveProperty property = SaveProperty.Load(reader);

                        UndoGroup group = new UndoGroup();

                        int index = mFile.IndexOfProperty(property.Name);
                        if (index >= 0)
                        {
                            int nextIndex = mFile.IndexOfProperty(property.Name, index + 1);
                            if (nextIndex >= 0)
                            {
                                // More than one property already exists with the same name, so just add another
                                index = GetInsertionIndex();
                            }
                            else
                            {
                                // Found a property with the same name. Replace it or add as new (prompt user)?
                                PastePropertyDialog dialog = new PastePropertyDialog(property.Name);
                                switch (dialog.ShowDialog(Application.Current.MainWindow))
                                {
                                    case PastePropertyDialogResult.Cancel:
                                        ResetFocus?.Invoke(this, EventArgs.Empty);
                                        return;
                                    case PastePropertyDialogResult.Replace:
                                        {
                                            SaveProperty oldProperty = mFile.Properties[index];
                                            group.Add(DelegateUndoUnit.CreateAndExecute(
                                                () => mFile.RemoveProperty(index),
                                                () =>
                                                {
                                                    mFile.InsertProperty(index, oldProperty);
                                                    int viewIndex = IndexOfViewProperty(oldProperty);
                                                    if (viewIndex >= 0) SelectedPropertyIndex = viewIndex;
                                                }));
                                        }
                                        break;
                                    case PastePropertyDialogResult.AddNew:
                                        index = GetInsertionIndex();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            index = GetInsertionIndex();
                        }

                        group.Add(DelegateUndoUnit.CreateAndExecute(
                            () =>
                            {
                                mFile.InsertProperty(index, property);
                                int viewIndex = IndexOfViewProperty(property);
                                if (viewIndex >= 0) SelectedPropertyIndex = viewIndex;
                            },
                            () =>
                            {
                                int viewIndex = IndexOfViewProperty(property);
                                mFile.RemoveProperty(index);
                                if (viewIndex >= 0) SelectedPropertyIndex = Math.Max(0, viewIndex - 1);
                            }));

                        mUndoService.PushUndoUnit(group);
                    }
                }
            }
            catch
            {
            }
        }

        private void MovePropertyDown()
        {
            int index = SelectedPropertyIndex;

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () => mFile.MovePropertyDown(index),
                () => mFile.MovePropertyUp(index + 1));

            mUndoService.PushUndoUnit(unit);
        }

        private void MovePropertyUp()
        {
            int index = SelectedPropertyIndex;

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () => mFile.MovePropertyUp(index),
                () => mFile.MovePropertyDown(index - 1));

            mUndoService.PushUndoUnit(unit);
        }

        private bool CanExecutePropertyMove()
        {
            return CanMoveProperties && SelectedPropertyIndex >= 0;
        }

        private bool CanMovePropertyDown()
        {
            return CanExecutePropertyMove() && mFile.CanMovePropertyDown(SelectedPropertyIndex);
        }

        private bool CanMovePropertyUp()
        {
            return CanExecutePropertyMove() && mFile.CanMovePropertyUp(SelectedPropertyIndex);
        }

        private void AddProperty()
        {
            NewPropertyDialog dialog = new NewPropertyDialog();
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                SaveProperty property = new SaveProperty(dialog.PropertyName, dialog.PropertyIsArray ? new ArrayValue(dialog.PropertyType) : SaveValue.Create(dialog.PropertyType));
                int index = GetInsertionIndex();

                DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                    () =>
                    {
                        mFile.InsertProperty(index, property);
                        SelectedPropertyIndex = IndexOfViewProperty(property);
                    },
                    () =>
                    {
                        int viewIndex = IndexOfViewProperty(property);
                        mFile.RemoveProperty(index);
                        if (viewIndex >= 0) SelectedPropertyIndex = Math.Max(0, viewIndex - 1);
                    });

                mUndoService.PushUndoUnit(unit);
            }
            else
            {
                ResetFocus?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RemoveProperty()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(mFile.Properties);
            view.MoveCurrentToPosition(SelectedPropertyIndex);
            
            SaveProperty property = (SaveProperty)view.CurrentItem;
            int index = mFile.IndexOfProperty(property);

            DelegateUndoUnit unit = DelegateUndoUnit.CreateAndExecute(
                () =>
                {
                    view.MoveCurrentToNext();
                    mFile.RemoveProperty(index);
                    if (view.IsCurrentAfterLast) view.MoveCurrentToLast();
                    SelectedPropertyIndex = view.CurrentPosition;
                },
                () =>
                {
                    mFile.InsertProperty(index, property);
                    view.MoveCurrentTo(property);
                    SelectedPropertyIndex = view.CurrentPosition;
                });
            
            mUndoService.PushUndoUnit(unit);
        }

        private SaveProperty GetSelectedProperty()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(mFile.Properties);
            view.MoveCurrentToPosition(SelectedPropertyIndex);
            return (SaveProperty)view.CurrentItem;
        }

        private int IndexOfViewProperty(SaveProperty property)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(mFile.Properties);
            view.MoveCurrentTo(property);
            return view.CurrentPosition;
        }

        private int GetInsertionIndex()
        {
            return SelectedPropertyIndex >= 0 ? mFile.IndexOfProperty(GetSelectedProperty()) + 1 : mFile.Properties.Count;
        }
    }
}
