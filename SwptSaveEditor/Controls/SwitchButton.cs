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
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SwptSaveEditor.Controls
{
    /// <summary>
    /// A toggle button that presents as a two-state switch
    /// </summary>
    [TemplatePart(Name = "PART_OffPresenter", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_OnPresenter", Type = typeof(FrameworkElement))]
    internal class SwitchButton : ToggleButton
    {
        private double mWidth;

        private FrameworkElement mOffPresenter;
        private FrameworkElement mOnPresenter;

        public object OffContent
        {
            get { return GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }
        public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(nameof(OffContent), typeof(object), typeof(SwitchButton),
            new PropertyMetadata(null));

        public object OnContent
        {
            get { return GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }
        public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(nameof(OnContent), typeof(object), typeof(SwitchButton),
            new PropertyMetadata(null));

        public Style ContentSelectedStyle
        {
            get { return (Style)GetValue(ContentSelectedStyleProperty); }
            set { SetValue(ContentSelectedStyleProperty, value); }
        }
        public static readonly DependencyProperty ContentSelectedStyleProperty = DependencyProperty.Register(nameof(ContentSelectedStyle), typeof(Style), typeof(SwitchButton),
            new PropertyMetadata((Style)null));

        public double WidthPadding
        {
            get { return (double)GetValue(WidthPaddingProperty); }
            set { SetValue(WidthPaddingProperty, value); }
        }
        public static readonly DependencyProperty WidthPaddingProperty = DependencyProperty.Register(nameof(WidthPadding), typeof(double), typeof(SwitchButton),
            new PropertyMetadata(10.0));

        static SwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchButton), new FrameworkPropertyMetadata(typeof(SwitchButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mOffPresenter = GetTemplateChild("PART_OffPresenter") as FrameworkElement;
            mOnPresenter = GetTemplateChild("PART_OnPresenter") as FrameworkElement;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);

            // In order for the toggle part of the control to stay in a fixed position, we need equal space on both sides of it
            if (size.Width > mWidth)
            {
                if (mOffPresenter != null && mOnPresenter != null)
                {
                    mWidth = Math.Ceiling(size.Width + Math.Abs(mOffPresenter.DesiredSize.Width - mOnPresenter.DesiredSize.Width));
                }
                else
                {
                    mWidth = Math.Ceiling(size.Width);
                }    
            }
            size.Width = mWidth + WidthPadding;
            return size;
        }
    }
}
