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

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Proxy to connect a data binding source to the visual tree when the target is diconnected
    /// </summary>
    internal class BindingProxy : Freezable
    {
        public object Context
        {
            get { return GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }
        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(nameof(Context), typeof(object), typeof(BindingProxy),
            new PropertyMetadata(null));

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
