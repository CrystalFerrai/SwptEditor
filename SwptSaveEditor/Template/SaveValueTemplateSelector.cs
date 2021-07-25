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

using SwptSaveLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SwptSaveEditor.Template
{
    /// <summary>
    /// Template selector for displaying and editing SaveValue types in a DataGrid
    /// </summary>
    internal class SaveValueTemplateSelector : DataTemplateSelector
    {
        private readonly bool mIsEditMode;

        private static bool sResourcesSearched;

        private static Dictionary<SaveValueType, DataTemplate> sDisplayTemplateMap;
        private static Dictionary<SaveValueType, DataTemplate> sEditTemplateMap;

        // Templates for displaying values
        private static DataTemplate sArrayTemplate;
        private static DataTemplate sStringTemplate;
        private static DataTemplate sBoolTemplate;
        private static DataTemplate sInt32Template;
        private static DataTemplate sSingleTemplate;
        private static DataTemplate sVector2Template;
        private static DataTemplate sVector3Template;
        private static DataTemplate sLinearColorTemplate;

        // Templates for editing values
        private static DataTemplate sEditArrayTemplate;
        private static DataTemplate sEditStringTemplate;
        private static DataTemplate sEditBoolTemplate;
        private static DataTemplate sEditInt32Template;
        private static DataTemplate sEditSingleTemplate;
        private static DataTemplate sEditVector2Template;
        private static DataTemplate sEditVector3Template;
        private static DataTemplate sEditLinearColorTemplate;

        // Resource keys for display templates
        public static readonly ComponentResourceKey ArrayTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(ArrayTemplateKey));
        public static readonly ComponentResourceKey StringTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(StringTemplateKey));
        public static readonly ComponentResourceKey BoolTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(BoolTemplateKey));
        public static readonly ComponentResourceKey Int32TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(Int32TemplateKey));
        public static readonly ComponentResourceKey SingleTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(SingleTemplateKey));
        public static readonly ComponentResourceKey Vector2TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(Vector2TemplateKey));
        public static readonly ComponentResourceKey Vector3TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(Vector3TemplateKey));
        public static readonly ComponentResourceKey LinearColorTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(LinearColorTemplateKey));

        // Resource keys for edit templates
        public static readonly ComponentResourceKey EditArrayTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditArrayTemplateKey));
        public static readonly ComponentResourceKey EditStringTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditStringTemplateKey));
        public static readonly ComponentResourceKey EditBoolTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditBoolTemplateKey));
        public static readonly ComponentResourceKey EditInt32TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditInt32TemplateKey));
        public static readonly ComponentResourceKey EditSingleTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditSingleTemplateKey));
        public static readonly ComponentResourceKey EditVector2TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditVector2TemplateKey));
        public static readonly ComponentResourceKey EditVector3TemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditVector3TemplateKey));
        public static readonly ComponentResourceKey EditLinearColorTemplateKey = new ComponentResourceKey(typeof(SaveValueTemplateSelector), nameof(EditLinearColorTemplateKey));

        /// <summary>
        /// An instance of the template selector for displaying values
        /// </summary>
        public static SaveValueTemplateSelector Instance = new SaveValueTemplateSelector(false);

        /// <summary>
        /// An instance of the template selector for editing values
        /// </summary>
        public static SaveValueTemplateSelector EditInstance = new SaveValueTemplateSelector(true);

        public SaveValueTemplateSelector(bool isEditMode)
        {
            mIsEditMode = isEditMode;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            SearchResources();

            SaveValue val = item as SaveValue;
            if (val == null)
            {
                SaveProperty prop = item as SaveProperty;
                if (prop == null) return null;

                val = prop.Value;
            }

            if (mIsEditMode)
            {
                DataTemplate template;
                if (sEditTemplateMap.TryGetValue(val.Type, out template))
                {
                    return template;
                }
                return sEditStringTemplate;
            }
            else
            {
                DataTemplate template;
                if (sDisplayTemplateMap.TryGetValue(val.Type, out template))
                {
                    return template;
                }
                return sStringTemplate;
            }
        }

        private static void SearchResources()
        {
            if (!sResourcesSearched)
            {
                sArrayTemplate = (DataTemplate)Application.Current.FindResource(ArrayTemplateKey);
                sStringTemplate = (DataTemplate)Application.Current.FindResource(StringTemplateKey);
                sBoolTemplate = (DataTemplate)Application.Current.FindResource(BoolTemplateKey);
                sInt32Template = (DataTemplate)Application.Current.FindResource(Int32TemplateKey);
                sSingleTemplate = (DataTemplate)Application.Current.FindResource(SingleTemplateKey);
                sVector2Template = (DataTemplate)Application.Current.FindResource(Vector2TemplateKey);
                sVector3Template = (DataTemplate)Application.Current.FindResource(Vector3TemplateKey);
                sLinearColorTemplate = (DataTemplate)Application.Current.FindResource(LinearColorTemplateKey);

                sDisplayTemplateMap = new Dictionary<SaveValueType, DataTemplate>()
                {
                    { SaveValueTypes.Array, sArrayTemplate },
                    { SaveValueTypes.String, sStringTemplate },
                    { SaveValueTypes.Bool, sBoolTemplate },
                    { SaveValueTypes.Int32, sInt32Template },
                    { SaveValueTypes.Single, sSingleTemplate },
                    { SaveValueTypes.Vector2, sVector2Template },
                    { SaveValueTypes.Vector3, sVector3Template },
                    { SaveValueTypes.LinearColor, sLinearColorTemplate },
                };

                sEditArrayTemplate = (DataTemplate)Application.Current.FindResource(EditArrayTemplateKey);
                sEditStringTemplate = (DataTemplate)Application.Current.FindResource(EditStringTemplateKey);
                sEditBoolTemplate = (DataTemplate)Application.Current.FindResource(EditBoolTemplateKey);
                sEditInt32Template = (DataTemplate)Application.Current.FindResource(EditInt32TemplateKey);
                sEditSingleTemplate = (DataTemplate)Application.Current.FindResource(EditSingleTemplateKey);
                sEditVector2Template = (DataTemplate)Application.Current.FindResource(EditVector2TemplateKey);
                sEditVector3Template = (DataTemplate)Application.Current.FindResource(EditVector3TemplateKey);
                sEditLinearColorTemplate = (DataTemplate)Application.Current.FindResource(EditLinearColorTemplateKey);

                sEditTemplateMap = new Dictionary<SaveValueType, DataTemplate>()
                {
                    { SaveValueTypes.Array, sEditArrayTemplate },
                    { SaveValueTypes.String, sEditStringTemplate },
                    { SaveValueTypes.Bool, sEditBoolTemplate },
                    { SaveValueTypes.Int32, sEditInt32Template },
                    { SaveValueTypes.Single, sEditSingleTemplate },
                    { SaveValueTypes.Vector2, sEditVector2Template },
                    { SaveValueTypes.Vector3, sEditVector3Template },
                    { SaveValueTypes.LinearColor, sEditLinearColorTemplate },
                };

                sResourcesSearched = true;
            }
        }
    }
}
