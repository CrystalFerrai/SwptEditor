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

using SwptSaveLib.Utils;
using System.Collections.Generic;
using System.IO;

namespace SwptSaveLib
{
    /// <summary>
    /// A save game is a collection of save files that represents a single game save
    /// </summary>
    public class SaveGame : ObservableObject
    {
        private readonly List<SaveFile> mFiles;

        private string mDirectory;

        public string Name { get; private set; }

        public string Directory => mDirectory;

        public IReadOnlyList<SaveFile> Files => mFiles;

        internal SaveGame(string directory)
        {
            mFiles = new List<SaveFile>();
            SetDirectory(directory);
        }

        public static SaveGame Load(string directory)
        {
            SaveGame game = new SaveGame(directory);

            try
            {
                foreach (string path in System.IO.Directory.GetFiles(directory))
                {
                    try
                    {
                        game.mFiles.Add(SaveFile.Load(path));
                    }
                    catch
                    {
                        // Just skip files that don't load
                    }
                }
            }
            catch
            {
                return null;
            }

            if (game.Files.Count == 0) return null;

            return game;
        }

        public void Save()
        {
            foreach (SaveFile file in mFiles)
            {
                file.Save();
            }
        }

        public void Save(string directory)
        {
            SetDirectory(directory);
            foreach (SaveFile file in mFiles)
            {
                file.Save(directory);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private void SetDirectory(string directory)
        {
            mDirectory = directory;
            Name = Path.GetFileNameWithoutExtension(directory);
            NotifyPropertyChanged(nameof(Name), nameof(Directory));
        }
    }
}
