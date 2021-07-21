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
using SwptSaveEditor.Settings;
using SwptSaveEditor.Utils;
using System;
using System.Windows;

namespace SwptSaveEditor
{
    /// <summary>
    /// The main application class
    /// </summary>
    internal class App : Application
    {
        private readonly ServiceRegistry mServices;

        private DocumentService mDocumentService;
        private SettingsService mSettingsService;

        private MainWindowVM mMainWindowVM;

        public DocumentService DocumentService => mDocumentService;

        /// <summary>
        /// The entry point of the application
        /// </summary>
        /// <param name="args">Command line parameters passed to the application</param>
        /// <returns>The application exit code</returns>
        [STAThread]
        public static int Main(string[] args)
        {
            App app = new App();
            return app.Run();
        }

        public App()
        {
            mServices = new ServiceRegistry();

            // ToolbarStyles must be explicitly loaded before Resources (rather than included by Resources) due to
            // annoying ResourceDictionary loading issues
            Resources.MergedDictionaries.Add(LoadResourceDictionary("ToolbarStyles.xaml"));
            Resources.MergedDictionaries.Add(LoadResourceDictionary("Resources.xaml"));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mDocumentService = new DocumentService();
            mServices.AddService(mDocumentService);

            mSettingsService = new SettingsService();
            mSettingsService.LoadSettings();
            mServices.AddService(mSettingsService);

            mMainWindowVM = new MainWindowVM(mServices);
            MainWindow = new MainWindow(mMainWindowVM);
            MainWindow.Show();

            // Calling base fires the Startup event
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mMainWindowVM.Dispose();

            mServices.RemoveService(mSettingsService);
            mServices.RemoveService(mDocumentService);

            // Calling base fires the Exit event
            base.OnExit(e);
        }

        /// <summary>
        /// Helper method for loading resource dictionaries that are embedded as resources in the application
        /// </summary>
        /// <param name="path">The relative path from the project root to the xaml file containing the resource dictionary</param>
        /// <returns>The loaded resource dictionary</returns>
        private static ResourceDictionary LoadResourceDictionary(string path)
        {
            return (ResourceDictionary)LoadComponent(ResourceHelper.GetResourceUri(path, true));
        }
    }
}
