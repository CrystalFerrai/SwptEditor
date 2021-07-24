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

using SwptSaveEditor.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SwptSaveEditor
{
    /// <summary>
    /// Specifies image sources for all of the images included in this module
    /// </summary>
    internal static class Images
    {
        /// <summary>
        /// Images used to mark things
        /// </summary>
        internal static class Markers
        {
            public static ImageSource ArrowDown { get; private set; }
            public static ImageSource ArrowLeft { get; private set; }
            public static ImageSource ArrowRight { get; private set; }
            public static ImageSource ArrowUp { get; private set; }
            public static ImageSource Crosshair { get; private set; }

            static Markers()
            {
                LoadImages(typeof(Markers));
            }
        }

        /// <summary>
        /// Icons used by buttons in toolbars
        /// </summary>
        internal static class ToolbarIcons
        {
            public static ImageSource Add { get; private set; }
            public static ImageSource AddFolder { get; private set; }
            public static ImageSource Checkmark { get; private set; }
            public static ImageSource Close { get; private set; }
            public static ImageSource CloseAlt { get; private set; }
            public static ImageSource Copy { get; private set; }
            public static ImageSource MoveDown { get; private set; }
            public static ImageSource MoveUp { get; private set; }
            public static ImageSource OpenFolder { get; private set; }
            public static ImageSource Paste { get; private set; }
            public static ImageSource Question { get; private set; }
            public static ImageSource Redo { get; private set; }
            public static ImageSource Refresh { get; private set; }
            public static ImageSource Remove { get; private set; }
            public static ImageSource Rename { get; private set; }
            public static ImageSource Save { get; private set; }
            public static ImageSource SaveAll { get; private set; }
            public static ImageSource Undo { get; private set; }

            static ToolbarIcons()
            {
                LoadImages(typeof(ToolbarIcons));
            }
        }

        /// <summary>
        /// Loads images and sets the image properties for a type
        /// </summary>
        /// <param name="type">The type to set properties on</param>
        private static void LoadImages(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType.IsAssignableFrom(typeof(BitmapImage))))
            {
                property.SetValue(null, new BitmapImage(ResourceHelper.GetResourceUri(string.Format("/Images/{0}/{1}.png", type.Name, property.Name))), null);
            }
        }
    }
}
