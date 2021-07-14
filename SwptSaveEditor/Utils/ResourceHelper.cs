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
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Provides helper methods for accessing application resources
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Returns the Uri of a resouce embedded into the application
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the application</param>
        /// <param name="relative">Whether to get only the relative portion of the Uri, needed for Application.LoadComponent</param>
        public static Uri GetResourceUri(string resourcePath, bool relative = false)
        {
            return GetResourceUri(resourcePath, Assembly.GetCallingAssembly(), relative);
        }

        /// <summary>
        /// Returns a bitmap resource from the given resource path.
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the application</param>
        /// <returns>A bitmap image</returns>
        public static BitmapImage GetBitmap(string resourcePath)
        {
            return new BitmapImage(GetResourceUri(resourcePath, Assembly.GetCallingAssembly(), false));
        }

        /// <summary>
        /// Returns an icon resource from the given resource path.
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the application</param>
        /// <returns>An icon (BitmapFrame)</returns>
        public static BitmapFrame GetIcon(string resourcePath)
        {
            return BitmapFrame.Create(GetResourceUri(resourcePath, Assembly.GetCallingAssembly(), false));
        }

        /// <summary>
        /// Creates a new bitmap at the target DPI with the pixel data from the source bitmap
        /// </summary>
        /// <param name="source">The bitmap to convert</param>
        /// <param name="dpi">The desired DPI</param>
        /// <returns>The new bitmap at the target DPI</returns>
        public static BitmapSource ConvertToDesiredDpi(BitmapSource source, double dpi = 96.0)
        {
            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * source.Format.BitsPerPixel;
            byte[] data = new byte[stride * height];
            source.CopyPixels(data, stride, 0);
            return BitmapSource.Create(width, height, dpi, dpi, source.Format, null, data, stride);
        }

        /// <summary>
        /// Helper method to construct a resource Uri based on the provided assembly.
        /// </summary>
        /// <param name="resourcePath">The path to the resource within the application</param>
        /// <param name="assembly">The assembly that contains the resource</param>
        /// <param name="relative">Whether to get only the relative portion of the Uri</param>
        private static Uri GetResourceUri(string resourcePath, Assembly assembly, bool relative)
        {
            string path = resourcePath.Replace('\\', '/');
            if (!path.StartsWith("/")) path = "/" + path;
            return new Uri(string.Format("{0}/{1};component{2}", relative ? string.Empty : "pack://application:,,,", assembly.GetName().Name, path), relative ? UriKind.Relative : UriKind.Absolute);
        }
    }
}
