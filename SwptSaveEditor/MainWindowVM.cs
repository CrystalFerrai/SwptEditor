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

using Microsoft.WindowsAPICodePack.Dialogs;
using SwptSaveEditor.Document;
using SwptSaveEditor.Input;
using SwptSaveEditor.Settings;
using SwptSaveEditor.Utils;
using SwptSaveLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SwptSaveEditor
{
    /// <summary>
    /// The view model for the main application window
    /// </summary>
    internal class MainWindowVM : ViewModelBase
    {
        private const string BaseWindowTitle = "Swpt Save Editor";

        private const int MaxRecentSaveGames = 5;

        private static readonly string DefaultSaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\LocalLow\\L2 Games\\She Will Punish Them");

        private readonly ServiceRegistry mServices;

        private readonly DocumentService mDocumentService;
        private readonly SettingsService mSettingsService;
        private readonly InputService mInputService;

        private readonly ObservableCollection<SaveDocument> mDocuments;
        private readonly ObservableCollection<SaveGameInfo> mRecentSaveGames;

        private readonly DelegateInputAction mOpenAction;
        private readonly DelegateInputAction mCloseAction;
        private readonly DelegateInputAction mReloadAction;
        private readonly DelegateInputAction mSaveAllAction;
        private readonly DelegateInputAction mSaveAllAsAction;

        private readonly DelegateCommand<string> mOpenRecentCommand;

        private SaveGame mSaveGame;

        private DelegateSettingsProvider mSettings;

        public string WindowTitle
        {
            get => _windowTitle;
            set => Set(ref _windowTitle, value);
        }
        private string _windowTitle = BaseWindowTitle;

        public IEnumerable<SaveDocument> Documents => mDocuments;

        public DocumentService DocumentService => mDocumentService;

        public IEnumerable<SaveGameInfo> RecentSaveGames => mRecentSaveGames;

        public int RecentSaveIndex
        {
            get => -1;
            set
            {
                LoadSaveGame(mRecentSaveGames[value].Path);
                NotifyPropertyChanged(nameof(RecentSaveIndex));
            }
        }

        public bool IsSaveOpen => mSaveGame != null;

        public bool HasRecentSave => mRecentSaveGames.Count > 0;

        public WindowState WindowState
        {
            get => _windowState;
            set { if (Set(ref _windowState, value) && value != WindowState.Minimized) mSettings.SetSetting(nameof(WindowState), value); }
        }
        private WindowState _windowState = WindowState.Normal;

        public double WindowLeft
        {
            get => _windowLeft;
            set { if (Set(ref _windowLeft, value)) mSettings.SetSetting(nameof(WindowLeft), value); }
        }
        private double _windowLeft = 100.0;

        public double WindowTop
        {
            get => _windowTop;
            set { if (Set(ref _windowTop, value)) mSettings.SetSetting(nameof(WindowTop), value); }
        }
        private double _windowTop = 100.0;

        public double WindowWidth
        {
            get => _windowWidth;
            set { if (Set(ref _windowWidth, value)) mSettings.SetSetting(nameof(WindowWidth), value); }
        }
        private double _windowWidth = 1000.0;

        public double WindowHeight
        {
            get => _windowHeight;
            set { if (Set(ref _windowHeight, value)) mSettings.SetSetting(nameof(WindowHeight), value); }
        }
        private double _windowHeight = 800.0;

        public InputService InputService => mInputService;

        public InputAction OpenAction => mOpenAction;

        public InputAction CloseAction => mCloseAction;

        public InputAction ReloadAction => mReloadAction;

        public InputAction SaveAllAction => mSaveAllAction;

        public InputAction SaveAllAsAction => mSaveAllAsAction;

        public ICommand OpenRecentCommand => mOpenRecentCommand;

        public MainWindowVM(IServiceProvider services)
        {
            services.Inject(out mDocumentService);
            services.Inject(out mSettingsService);

            mServices = new ServiceRegistry(services);

            mInputService = new InputService(mServices);
            mServices.AddService(mInputService);

            mDocuments = new ObservableCollection<SaveDocument>();
            mRecentSaveGames = new ObservableCollection<SaveGameInfo>();

            mSettings = new DelegateSettingsProvider("MainWindow", mSettingsService);
            mSettings.SettingsLoaded += Settings_SettingsLoaded;
            mSettingsService.RegisterProvider(mSettings);

            mInputService.GlobalActions.Add(mOpenAction = new DelegateInputAction("Open Save Game Folder", Key.O, ModifierKeys.Control, OpenSaveGame));
            mInputService.GlobalActions.Add(mCloseAction = new DelegateInputAction("Close All", Key.F4, ModifierKeys.Control, CloseAll, () => IsSaveOpen));
            mInputService.GlobalActions.Add(mReloadAction = new DelegateInputAction("Reload All from Disk", Key.R, ModifierKeys.Control, ReloadFromDisk, () => IsSaveOpen));
            mInputService.GlobalActions.Add(mSaveAllAction = new DelegateInputAction("Save All Changed Files", Key.S, ModifierKeys.Control | ModifierKeys.Shift, SaveAllChanges, AnyUnsavedChanges));
            mInputService.GlobalActions.Add(mSaveAllAsAction = new DelegateInputAction("Save All Files to a New Location", Key.N, ModifierKeys.Control, SaveAllAs, () => IsSaveOpen));

            mOpenRecentCommand = new DelegateCommand<string>((path) => LoadSaveGame(path));
        }

        public void OnFirstLoad()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                LoadSaveGame(args[1]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseDocuments();
            }

            base.Dispose(disposing);
        }

        public void OnMainWindowClosing(CancelEventArgs e)
        {
            if (!HandleUnsavedChanges())
            {
                e.Cancel = true;
            }
            else
            {
                mSettingsService.FlushPendingSave();
            }
        }

        private void SaveAllChanges()
        {
            foreach (SaveDocument doc in mDocuments.Where(d => !d.UndoService.IsSavePoint))
            {
                doc.Save();
            }
        }

        private void OpenSaveGame()
        {
            if (!HandleUnsavedChanges()) return;

            string path = null;

            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Select Save Game Folder")
            {
                EnsureFileExists = true,
                InitialDirectory = DefaultSaveDirectory,
                IsFolderPicker = true
            })
            {
                CommonFileDialogResult result = dialog.ShowDialog(Application.Current.MainWindow);
                if (result == CommonFileDialogResult.Ok)
                {
                    path = dialog.FileName;
                }
            }

            if (path != null)
            {
                LoadSaveGame(path);
            }
        }

        private void LoadSaveGame(string path)
        {
            CloseDocuments();

            mSaveGame = SaveGame.Load(path);
            RefreshLoadedState();
            if (mSaveGame == null)
            {
                MessageBox.Show(Application.Current.MainWindow, "Did not find any accessible save game files in the specified folder.", "No Files Found", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                WindowTitle = BaseWindowTitle;
                return;
            }

            WindowTitle = $"{BaseWindowTitle} - {mSaveGame.Name}";

            foreach (SaveFile file in mSaveGame.Files)
            {
                SaveDocument document = new SaveDocument(mServices, file);
                document.UndoService.StateChanged += Document_StateChanged;
                mDocuments.Add(document);
            }

            SaveDocument selectedDocument = mDocuments.FirstOrDefault(f => f.Name.Equals("Global", StringComparison.InvariantCultureIgnoreCase));
            if (selectedDocument == null) selectedDocument = mDocuments.FirstOrDefault();
            mDocumentService.ActiveDocument = selectedDocument;

            AddToRecentList(path);
        }

        private void ReloadFromDisk()
        {
            if (AnyUnsavedChanges())
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Reloading from disk will discard unsaved changes. This action cannot be undone. Are you sure?", "Discard Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }

            LoadSaveGame(mSaveGame.Directory);
        }

        private void SaveAllAs()
        {
            bool shouldSave = false;

            string path = DefaultSaveDirectory;

            while (true)
            {
                using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Select Output Folder")
                {
                    EnsureFileExists = true,
                    InitialDirectory = path,
                    IsFolderPicker = true
                })
                {
                    CommonFileDialogResult result = dialog.ShowDialog(Application.Current.MainWindow);
                    if (result == CommonFileDialogResult.Ok)
                    {
                        path = dialog.FileName;
                        if (Directory.GetFileSystemEntries(path).Length > 0)
                        {
                            MessageBoxResult mbResult = MessageBox.Show(Application.Current.MainWindow, "Selected directory is not empty. Are you sure you want to save here? Exisiting files with matching names will be overwritten.", "Confirm Directory", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                            if (mbResult == MessageBoxResult.Yes)
                            {
                                shouldSave = true;
                                break;
                            }
                        }
                        else
                        {
                            shouldSave = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (shouldSave)
            {
                mSaveGame.Save(path);
                foreach (SaveDocument document in mDocuments)
                {
                    document.UndoService.SetSavePoint();
                }

                WindowTitle = $"{BaseWindowTitle} - {mSaveGame.Name}";

                AddToRecentList(path);
            }
        }

        private bool AnyUnsavedChanges()
        {
            return mDocuments.Any(d => !d.UndoService.IsSavePoint);
        }

        private void CloseAll()
        {
            if (HandleUnsavedChanges())
            {
                CloseDocuments();
                mSaveGame = null;
                WindowTitle = BaseWindowTitle;
                RefreshLoadedState();
            }
        }

        private bool HandleUnsavedChanges()
        {
            if (AnyUnsavedChanges())
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "You have unsaved changes. Would you like to save before continuing?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return false;
                    case MessageBoxResult.Yes:
                        SaveAllChanges();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            return true;
        }

        private void CloseDocuments()
        {
            foreach (SaveDocument document in mDocuments)
            {
                document.UndoService.StateChanged -= Document_StateChanged;
                document.Dispose();
            }
            mDocuments.Clear();

            mDocumentService.ActiveDocument = null;
        }

        private void AddToRecentList(string path)
        {
            SaveGameInfo info = new SaveGameInfo(path);
            int recentIndex = mRecentSaveGames.IndexOf(info);
            if (recentIndex > 0)
            {
                mRecentSaveGames.RemoveAt(recentIndex);
            }
            if (recentIndex != 0)
            {
                mRecentSaveGames.Insert(0, info);
            }

            while (mRecentSaveGames.Count > MaxRecentSaveGames)
            {
                mRecentSaveGames.RemoveAt(mRecentSaveGames.Count - 1);
            }

            mSettings.SetSetting(nameof(RecentSaveGames), string.Join("|", mRecentSaveGames));

            NotifyPropertyChanged(nameof(HasRecentSave));
        }

        private void RefreshLoadedState()
        {
            NotifyPropertyChanged(nameof(IsSaveOpen));

            mCloseAction.RaiseCanExecuteChanged();
            mReloadAction.RaiseCanExecuteChanged();
            mSaveAllAction.RaiseCanExecuteChanged();
            mSaveAllAsAction.RaiseCanExecuteChanged();
        }

        private void Settings_SettingsLoaded(object sender, EventArgs e)
        {
            if (mSettings.Settings.TryGetValue(nameof(RecentSaveGames), out string paths))
            {
                foreach (string path in paths.Split('|'))
                {
                    mRecentSaveGames.Add(new SaveGameInfo(path));
                }
            }

            WindowLeft = mSettings.GetTypedSetting(nameof(WindowLeft), WindowLeft);
            WindowTop = mSettings.GetTypedSetting(nameof(WindowTop), WindowTop);
            WindowWidth = mSettings.GetTypedSetting(nameof(WindowWidth), WindowWidth);
            WindowHeight = mSettings.GetTypedSetting(nameof(WindowHeight), WindowHeight);

            WindowState state = mSettings.GetTypedSetting(nameof(WindowState), WindowState);
            if (state == WindowState.Minimized) state = WindowState.Normal;
            WindowState = state;
        }

        private void Document_StateChanged(object sender, EventArgs e)
        {
            mSaveAllAction.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Meta information pertaining to a save game
    /// </summary>
    internal class SaveGameInfo
    {
        public string Name { get; }

        public string Path { get; }

        public SaveGameInfo(string path)
        {
            Path = path.Replace("\\", "/");
            Name = Path.Substring(Path.LastIndexOf('/') + 1);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SaveGameInfo other && Path.Equals(other.Path, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
