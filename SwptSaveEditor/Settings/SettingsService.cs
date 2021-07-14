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
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;

namespace SwptSaveEditor.Settings
{
    /// <summary>
    /// A service for managing application user settings
    /// </summary>
    internal class SettingsService
    {
        private static readonly string SettingsPath;

        private readonly Dispatcher mDispatcher;
        private readonly DispatcherTimer mSaveTimer;

        private readonly List<ISettingsProvider> mProviders;

        private readonly Dictionary<string, Dictionary<string, string>> mSettings;

        static SettingsService()
        {
            SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SwptSaveEditor", "Settings.cfg");
        }

        public SettingsService()
        {
            mDispatcher = Dispatcher.CurrentDispatcher;
            mSaveTimer = new DispatcherTimer(DispatcherPriority.Normal, mDispatcher) { IsEnabled = false };
            mSaveTimer.Tick += (s, e) => InternalSaveSettings();

            mProviders = new List<ISettingsProvider>();
            mSettings = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Register a settings provider with the service
        /// </summary>
        public void RegisterProvider(ISettingsProvider provider)
        {
            mProviders.Add(provider);
            PushSettingsToProvider(provider);
            PullSettingsFromProvider(provider);
        }

        /// <summary>
        /// Instruct the service to load settings from disk and inform providers about any loaded settings
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(SettingsPath)) return;

            try
            {
                using (FileStream stream = File.OpenRead(SettingsPath))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        int dotIndex = line.IndexOf('.');
                        if (dotIndex < 0) continue;

                        string provider = line.Substring(0, dotIndex);
                        string setting = line.Substring(dotIndex + 1);

                        int assignIndex = setting.IndexOf('=');
                        if (assignIndex < 0) continue;

                        string name = setting.Substring(0, assignIndex);
                        string value = setting.Substring(assignIndex + 1);

                        IDictionary<string, string> providerSettings = GetOrCreateProviderSettings(provider);

                        providerSettings[name] = value;
                    }
                }

                foreach (ISettingsProvider provider in mProviders)
                {
                    PushSettingsToProvider(provider);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Instruct the service to persist all settings to disk after a given delay
        /// </summary>
        public void SaveSettings(uint delayInMs = 500)
        {
            if (delayInMs == 0)
            {
                InternalSaveSettings();
            }
            else
            {
                // Keep restarting delay until requests stop coming in
                mSaveTimer.Stop();
                mSaveTimer.Interval = TimeSpan.FromMilliseconds(delayInMs);
                mSaveTimer.Start();
            }
        }

        /// <summary>
        /// Immediately flush a pending save request, if one exists.
        /// </summary>
        public void FlushPendingSave()
        {
            if (mSaveTimer.IsEnabled)
            {
                InternalSaveSettings();
            }
        }

        private void InternalSaveSettings()
        {
            mSaveTimer.Stop();

            System.Diagnostics.Debug.WriteLine("[SettingsService] Saving settings");

            foreach (ISettingsProvider provider in mProviders)
            {
                PullSettingsFromProvider(provider);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

            using (FileStream stream = File.Create(SettingsPath))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (var providerSettings in mSettings)
                {
                    foreach (var setting in providerSettings.Value)
                    {
                        writer.WriteLine($"{providerSettings.Key}.{setting.Key}={setting.Value}");
                    }
                }
            }
        }

        private void PullSettingsFromProvider(ISettingsProvider provider)
        {
            IDictionary<string, string> providerSettings = GetOrCreateProviderSettings(provider.ProviderUniqueId);
            foreach (var setting in provider.Settings)
            {
                providerSettings[setting.Key] = setting.Value;
            }
        }

        private void PushSettingsToProvider(ISettingsProvider provider)
        {
            Dictionary<string, string> providerSettings;
            if (mSettings.TryGetValue(provider.ProviderUniqueId, out providerSettings))
            {
                provider.SettingsLoaded(providerSettings);
            }
        }

        private IDictionary<string, string> GetOrCreateProviderSettings(string providerId)
        {
            Dictionary<string, string> providerSettings;
            if (!mSettings.TryGetValue(providerId, out providerSettings))
            {
                providerSettings = new Dictionary<string, string>();
                mSettings.Add(providerId, providerSettings);
            }
            return providerSettings;
        }
    }
}
