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

using System.Collections.Generic;

namespace SwptSaveEditor.Settings
{
    /// <summary>
    /// Interface for objects that provide settings to the settings service
    /// </summary>
    internal interface ISettingsProvider
    {
        /// <summary>
        /// Gets the unique ID for this provider
        /// </summary>
        string ProviderUniqueId { get; }

        /// <summary>
        /// Gets the provided settings
        /// </summary>
        IReadOnlyDictionary<string, string> Settings { get; }

        /// <summary>
        /// Called from the settings service to inform the provider that settings have been loaded from disk
        /// </summary>
        /// <param name="settings">The settings that were loaded for this provider</param>
        void SettingsLoaded(IReadOnlyDictionary<string, string> settings);
    }
}
