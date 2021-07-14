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
using System.Windows.Controls;
using System.Windows.Data;

namespace SwptSaveEditor.Controls
{
    /// <summary>
    /// A DataGrid template column which allows specifying a custom binding for cells in the column
    /// </summary>
    internal class BindingDataGridTemplateColumn : DataGridTemplateColumn
    {
        public BindingBase Binding
        {
            get => _binding;
            set
            {
                if (_binding != value)
                {
                    _binding = value;
                    NotifyPropertyChanged(nameof(Binding));
                }
            }
        }
        private BindingBase _binding;

        public BindingDataGridTemplateColumn()
        {
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter element = (ContentPresenter)base.GenerateElement(cell, dataItem);
            if (Binding != null)
            {
                BindingOperations.ClearBinding(element, ContentPresenter.ContentProperty);
                BindingOperations.SetBinding(element, ContentPresenter.ContentProperty, Binding);
            }
            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter element = (ContentPresenter)base.GenerateEditingElement(cell, dataItem);
            if (Binding != null)
            {
                BindingOperations.ClearBinding(element, ContentPresenter.ContentProperty);
                BindingOperations.SetBinding(element, ContentPresenter.ContentProperty, Binding);
            }
            return element;
        }
    }
}
