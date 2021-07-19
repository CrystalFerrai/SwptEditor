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

using System.Windows;

namespace SwptSaveEditor.Dialogs
{
    /// <summary>
    /// A dialog prompting the user for which action to take when pasting a property that conflicts with an existing property
    /// </summary>
    internal class PastePropertyDialog
    {
        private PastePropertyDialogResult mResult = PastePropertyDialogResult.Cancel;

        public string PropertyName { get; }

        public PastePropertyDialog(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>A value indicating which option the user selected.</returns>
        public PastePropertyDialogResult ShowDialog(Window owner)
        {
            PastePropertyDialogView dialog = new PastePropertyDialogView()
            {
                Owner = owner,
                DataContext = this
            };

            if (dialog.ShowDialog() == true)
            {
                return mResult;
            }
            return PastePropertyDialogResult.Cancel;
        }

        public void SetResult(PastePropertyDialogResult result)
        {
            mResult = result;
        }
    }

    internal enum PastePropertyDialogResult
    {
        Cancel,
        Replace,
        AddNew
    }
}
