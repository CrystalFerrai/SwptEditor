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

namespace SwptSaveEditor.Settings
{
    /// <summary>
    /// A helper to manage settings on behalf of another object
    /// </summary>
    internal class DelegateSettingsProvider : ISettingsProvider
    {
        private readonly SettingsService mSettingsService;

        private readonly Dictionary<string, string> mSettings;

        public string ProviderUniqueId { get; }

        public IReadOnlyDictionary<string, string> Settings => mSettings;

        public event EventHandler SettingsLoaded;

        public DelegateSettingsProvider(string uniqueId, SettingsService settingsService, bool registerSelf = true)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));

            mSettingsService = settingsService;
            mSettings = new Dictionary<string, string>();

            ProviderUniqueId = uniqueId;

            if (registerSelf)
            {
                mSettingsService.RegisterProvider(this);
            }
        }

        public T GetTypedSetting<T>(string name, T defaultValue)
        {
            if (mSettings.TryGetValue(name, out string val))
            {
                try
                {
                    Type type = typeof(T);
                    if (type.IsEnum) return (T)Enum.Parse(type, val);
                    return (T)Convert.ChangeType(val, type);
                }
                catch
                {
                }
            }
            return defaultValue;
        }

        public void SetSetting(string name, object value)
        {
            mSettings[name] = value.ToString();

            mSettingsService.SaveSettings();
        }

        void ISettingsProvider.SettingsLoaded(IReadOnlyDictionary<string, string> settings)
        {
            foreach (var setting in settings)
            {
                mSettings[setting.Key] = setting.Value;
            }

            SettingsLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
