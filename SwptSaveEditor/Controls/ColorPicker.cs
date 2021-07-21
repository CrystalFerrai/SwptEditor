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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SwptSaveEditor.Controls
{
    /// <summary>
    /// A control for displaying a color value which can open a color picker popup to edit the bound color
    /// </summary>
    [TemplatePart(Name = "PART_ColorText", Type = typeof(TextBlock))]
    internal class ColorPicker : Control
    {
        private TextBlock mColorText;

        /// <summary>
        /// The color to display/edit
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorPicker),
            new FrameworkPropertyMetadata(Colors.Fuchsia, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((ColorPicker)d).OnColorChanged((Color)e.OldValue, (Color)e.NewValue)));

        /// <summary>
        /// If true, this control will not allow changing the color value
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ColorPicker),
            new FrameworkPropertyMetadata(false));

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        /// <summary>
        /// Opens the color picker popup if this control is not set to read only
        /// </summary>
        public void ShowPopup()
        {
            if (IsReadOnly) return;

            ColorPickerPopup popup = new ColorPickerPopup()
            {
                Color = Color
            };

            Window window = new Window()
            {
                Content = popup,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                Title = "Pick Color",
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (window.ShowDialog() == true)
            {
                SetCurrentValue(ColorProperty, popup.Color);
                CommitChanges();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mColorText = GetTemplateChild("PART_ColorText") as TextBlock;

            UpdateColorText();
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            UpdateColorText();
        }

        private void UpdateColorText()
        {
            if (mColorText != null)
            {
                mColorText.Text = $"#{Color.A:x2}{Color.R:x2}{Color.G:x2}{Color.B:x2}";
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsReadOnly)
            {
                ShowPopup();
                e.Handled = true;
            }
            base.OnMouseDown(e);
        }

        private void CommitChanges()
        {
            for (DependencyObject current = VisualTreeHelper.GetParent(this); current is Visual; current = VisualTreeHelper.GetParent(current))
            {
                if (current is DataGrid grid)
                {
                    grid.CommitEdit(DataGridEditingUnit.Row, true);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Popup for picking a color, intended to be used by <see cref="ColorPicker"/>
    /// </summary>
    [TemplatePart(Name = "PART_OkButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_HPicker", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_HMarker", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_SVPicker", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_SVMarker", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_APicker", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_AMarker", Type = typeof(FrameworkElement))]
    internal class ColorPickerPopup : Control
    {
        private ButtonBase mOkButton;

        private PickerInputHandler mHPicker;
        private FrameworkElement mHMarker;
        private Canvas mHMarkerParent;

        private PickerInputHandler mSVPicker;
        private FrameworkElement mSVMarker;
        private Canvas mSVMarkerParent;

        private PickerInputHandler mAPicker;
        private FrameworkElement mAMarker;
        private Canvas mAMarkerParent;

        private bool mIsInPropertyChange;

        /// <summary>
        /// The colot to modify
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(Colors.Fuchsia, (d, e) => ((ColorPickerPopup)d).OnColorChanged((Color)e.OldValue, (Color)e.NewValue)));

        // All properties after this point are implementation details only relevant to the control template. Nothing should externally bind to or modify any of these properties.

        public Color PreviewColor
        {
            get { return (Color)GetValue(PreviewColorProperty); }
            set { SetValue(PreviewColorProperty, value); }
        }
        public static readonly DependencyProperty PreviewColorProperty = DependencyProperty.Register(nameof(PreviewColor), typeof(Color), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(Colors.Fuchsia, (d, e) => ((ColorPickerPopup)d).OnPreviewColorChanged((Color)e.OldValue, (Color)e.NewValue)));

        public Color OpaquePreviewColor
        {
            get { return (Color)GetValue(OpaquePreviewColorProperty); }
            set { SetValue(OpaquePreviewColorProperty, value); }
        }
        public static readonly DependencyProperty OpaquePreviewColorProperty = DependencyProperty.Register(nameof(OpaquePreviewColor), typeof(Color), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(Colors.Fuchsia));

        public Color HueColor
        {
            get { return (Color)GetValue(HueColorProperty); }
            set { SetValue(HueColorProperty, value); }
        }
        public static readonly DependencyProperty HueColorProperty = DependencyProperty.Register(nameof(HueColor), typeof(Color), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(Colors.Fuchsia));

        public float Hue
        {
            get { return (float)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register(nameof(Hue), typeof(float), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(0.83333f, (d, e) => ((ColorPickerPopup)d).OnHChanged((float)e.OldValue, (float)e.NewValue), (d, v) => ClampAndRound((float)v, 0.0f, 359.0f)));

        public float Saturation
        {
            get { return (float)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(nameof(Saturation), typeof(float), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(1.0f, (d, e) => ((ColorPickerPopup)d).OnSVChanged((float)e.OldValue, (float)e.NewValue), (d, v) => ClampAndRound((float)v, 0.0f, 100.0f)));

        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(float), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata(1.0f, (d, e) => ((ColorPickerPopup)d).OnSVChanged((float)e.OldValue, (float)e.NewValue), (d, v) => ClampAndRound((float)v, 0.0f, 100.0f)));

        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }
        public static readonly DependencyProperty RedProperty = DependencyProperty.Register(nameof(Red), typeof(byte), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata((byte)255, (d, e) => ((ColorPickerPopup)d).OnArgbChanged()));

        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }
        public static readonly DependencyProperty GreenProperty = DependencyProperty.Register(nameof(Green), typeof(byte), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata((byte)0, (d, e) => ((ColorPickerPopup)d).OnArgbChanged()));

        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }
        public static readonly DependencyProperty BlueProperty = DependencyProperty.Register(nameof(Blue), typeof(byte), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata((byte)255, (d, e) => ((ColorPickerPopup)d).OnArgbChanged()));

        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register(nameof(Alpha), typeof(byte), typeof(ColorPickerPopup),
            new FrameworkPropertyMetadata((byte)255, (d, e) => ((ColorPickerPopup)d).OnArgbChanged()));

        static ColorPickerPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerPopup), new FrameworkPropertyMetadata(typeof(ColorPickerPopup)));
        }

        public ColorPickerPopup()
        {
            mOkButton = null;
            mIsInPropertyChange = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mHPicker = null;
            mSVPicker = null;
            mAPicker = null;

            if (mOkButton != null)
            {
                mOkButton.Click -= OkButton_Click;
            }
            if (mHMarker != null && mHMarkerParent != null)
            {
                mHMarker.SizeChanged -= HMarker_SizeChanged;
                mHMarkerParent.SizeChanged -= HMarker_SizeChanged;
            }
            if (mSVMarker != null && mSVMarkerParent != null)
            {
                mSVMarker.SizeChanged -= SVMarker_SizeChanged;
                mSVMarkerParent.SizeChanged -= SVMarker_SizeChanged;
            }
            if (mAMarker != null && mAMarkerParent != null)
            {
                mAMarker.SizeChanged -= AMarker_SizeChanged;
                mAMarkerParent.SizeChanged -= AMarker_SizeChanged;
            }

            var hPicker = GetTemplateChild("PART_HPicker") as FrameworkElement;
            var svPicker = GetTemplateChild("PART_SVPicker") as FrameworkElement;
            var aPicker = GetTemplateChild("PART_APicker") as FrameworkElement;

            mOkButton = GetTemplateChild("PART_OkButton") as ButtonBase;

            mHMarker = GetTemplateChild("PART_HMarker") as FrameworkElement;
            mHMarkerParent = mHMarker == null ? null : VisualTreeHelper.GetParent(mHMarker) as Canvas;
            
            mSVMarker = GetTemplateChild("PART_SVMarker") as FrameworkElement;
            mSVMarkerParent = mSVMarker == null ? null : VisualTreeHelper.GetParent(mSVMarker) as Canvas;
            
            mAMarker = GetTemplateChild("PART_AMarker") as FrameworkElement;
            mAMarkerParent = mAMarker == null ? null : VisualTreeHelper.GetParent(mAMarker) as Canvas;

            if (hPicker != null)
            {
                mHPicker = new PickerInputHandler(hPicker);
                mHPicker.Picked += HPicker_Picked;
            }
            if (svPicker != null)
            {
                mSVPicker = new PickerInputHandler(svPicker);
                mSVPicker.Picked += SVPicker_Picked;
            }
            if (aPicker != null)
            {
                mAPicker = new PickerInputHandler(aPicker);
                mAPicker.Picked += APicker_Picked;
            }

            if (mOkButton != null)
            {
                mOkButton.Click += OkButton_Click;
            }
            if (mHMarker != null && mHMarkerParent != null)
            {
                mHMarker.SizeChanged += HMarker_SizeChanged;
                mHMarkerParent.SizeChanged += HMarker_SizeChanged;
            }
            if (mSVMarker != null && mSVMarkerParent != null)
            {
                mSVMarker.SizeChanged += SVMarker_SizeChanged;
                mSVMarkerParent.SizeChanged += SVMarker_SizeChanged;
            }
            if (mAMarker != null && mAMarkerParent != null)
            {
                mAMarker.SizeChanged += AMarker_SizeChanged;
                mAMarkerParent.SizeChanged += AMarker_SizeChanged;
            }

            UpdateHMarker();
            UpdateSVMarker();
            UpdateAMarker();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            mIsInPropertyChange = true;
            Color = PreviewColor;
            mIsInPropertyChange = false;

            Window.GetWindow(this).DialogResult = true;
        }

        private void HPicker_Picked(object sender, PickedEventArgs e)
        {
            SetCurrentValue(HueProperty, (float)e.RelativePosition.Y * 360.0f);
        }

        private void SVPicker_Picked(object sender, PickedEventArgs e)
        {
            SetCurrentValue(SaturationProperty, (float)e.RelativePosition.X * 100.0f);
            SetCurrentValue(ValueProperty, (1.0f - (float)e.RelativePosition.Y) * 100.0f);
        }

        private void APicker_Picked(object sender, PickedEventArgs e)
        {
            SetCurrentValue(AlphaProperty, (byte)(e.RelativePosition.X * 255.0));
        }

        private void HMarker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHMarker();
        }

        private void SVMarker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSVMarker();
        }

        private void AMarker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateAMarker();
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            if (newValue != PreviewColor)
            {
                PreviewColor = newValue;
            }
        }

        private void OnPreviewColorChanged(Color oldValue, Color newValue)
        {
            if (!mIsInPropertyChange)
            {
                mIsInPropertyChange = true;
                UpdateOpaquePreviewColor();

                UpdateArgb(PreviewColor);
                UpdateHSV(PreviewColor);
                UpdateHueColor();

                mIsInPropertyChange = false;
            }
        }

        private void OnArgbChanged()
        {
            if (!mIsInPropertyChange)
            {
                mIsInPropertyChange = true;

                SetCurrentValue(PreviewColorProperty, Color.FromArgb(Alpha, Red, Green, Blue));
                UpdateOpaquePreviewColor();

                UpdateHSV(PreviewColor);
                UpdateHueColor();

                mIsInPropertyChange = false;
            }
            UpdateAMarker();
        }

        private void OnHChanged(float oldValue, float newValue)
        {
            if (!mIsInPropertyChange)
            {
                mIsInPropertyChange = true;

                UpdatePreviewColorFromAhsl();
                UpdateArgb(PreviewColor);
                UpdateHueColor();

                mIsInPropertyChange = false;
            }
            UpdateHMarker();
        }

        private void OnSVChanged(float oldValue, float newValue)
        {

            if (!mIsInPropertyChange)
            {
                mIsInPropertyChange = true;

                UpdatePreviewColorFromAhsl();
                UpdateArgb(PreviewColor);

                mIsInPropertyChange = false;
            }
            UpdateSVMarker();
        }

        private void UpdateHMarker()
        {
            if (mHMarker == null || mHMarkerParent == null) return;

            mHMarker.SetValue(Canvas.TopProperty, mHMarkerParent.ActualHeight * (Hue / 360.0f) - mHMarker.ActualHeight * 0.5);
        }

        private void UpdateSVMarker()
        {
            if (mSVMarker == null || mSVMarkerParent == null) return;

            mSVMarker.SetValue(Canvas.LeftProperty, mSVMarkerParent.ActualWidth * (Saturation * 0.01f) - mSVMarker.ActualWidth * 0.5);
            mSVMarker.SetValue(Canvas.TopProperty, mSVMarkerParent.ActualHeight * (1.0f - Value * 0.01f) - mSVMarker.ActualHeight * 0.5);
        }

        private void UpdateAMarker()
        {
            if (mAMarker == null || mAMarkerParent == null) return;

            mAMarker.SetValue(Canvas.LeftProperty, mAMarkerParent.ActualWidth * (Alpha / 255.0f) - mAMarker.ActualWidth * 0.5);
        }

        private void UpdateHSV(Color color)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(b, b));
            float delta = max - min;

            float h = 0.0f;
            float s = 0.0f;
            float v = max;

            if (delta != 0.0f)
            {
                if (max == r) h = (g - b) / delta % 6.0f;
                else if (max == g) h = (b - r) / delta + 2.0f;
                else h = (r - g) / delta + 4.0f;

                if (h < 0.0f) h += 6.0f;

                s = delta / max;
            }

            SetCurrentValue(HueProperty, h * 60.0f);
            SetCurrentValue(SaturationProperty, s * 100.0f);
            SetCurrentValue(ValueProperty, v * 100.0f);
        }

        private void UpdatePreviewColorFromAhsl()
        {
            SetCurrentValue(PreviewColorProperty, AhsvToArgb(Alpha, Hue / 60.0f, Saturation * 0.01f, Value * 0.01f));
            UpdateOpaquePreviewColor();
        }

        private void UpdateOpaquePreviewColor()
        {
            SetCurrentValue(OpaquePreviewColorProperty, Color.FromRgb(PreviewColor.R, PreviewColor.G, PreviewColor.B));
        }

        private void UpdateHueColor()
        {
            SetCurrentValue(HueColorProperty, AhsvToArgb(255, Hue / 60.0f, 1.0f, 1.0f));
        }

        private void UpdateArgb(Color color)
        {
            SetCurrentValue(AlphaProperty, color.A);
            SetCurrentValue(RedProperty, color.R);
            SetCurrentValue(GreenProperty, color.G);
            SetCurrentValue(BlueProperty, color.B);
        }

        private static Color AhsvToArgb(byte a, float h, float s, float v)
        {
            float f = h - (float)Math.Floor(h);
            float p = v * (1.0f - s);
            float q = v * (1.0f - s * f);
            float t = v * (1.0f - s * (1.0f - f));

            float r = 0.0f, g = 0.0f, b = 0.0f;

            switch ((int)h)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            return Color.FromArgb(a, (byte)(r * 255.0f), (byte)(g * 255.0f), (byte)(b * 255.0f));
        }

        private static float ClampAndRound(float value, float min, float max)
        {
            return (float)Math.Floor(Math.Max(min, Math.Min((float)Math.Round(value, MidpointRounding.AwayFromZero), max)));
        }

        private class PickerInputHandler
        {
            private FrameworkElement mPicker;

            public event EventHandler<PickedEventArgs> Picked;

            public PickerInputHandler(FrameworkElement picker)
            {
                mPicker = picker;
                mPicker.MouseDown += Picker_MouseDown;
                mPicker.MouseUp += Picker_MouseUp;
                mPicker.MouseMove += Picker_MouseMove;
            }

            private void Picker_MouseDown(object sender, MouseButtonEventArgs e)
            {
                mPicker.Focus();
                mPicker.CaptureMouse();
                HandlePick(e.GetPosition(mPicker));
            }

            private void Picker_MouseUp(object sender, MouseButtonEventArgs e)
            {
                mPicker.ReleaseMouseCapture();
            }

            private void Picker_MouseMove(object sender, MouseEventArgs e)
            {
                if (mPicker.IsMouseCaptured)
                {
                    HandlePick(e.GetPosition(mPicker));
                }
            }

            private void HandlePick(Point position)
            {
                var handler = Picked;
                if (handler != null)
                {
                    Vector relativePosition = new Vector();
                    relativePosition.X = Math.Max(0.0, Math.Min(position.X / mPicker.ActualWidth, 1.0));
                    relativePosition.Y = Math.Max(0.0, Math.Min(position.Y / mPicker.ActualHeight, 1.0));
                    handler.Invoke(this, new PickedEventArgs(relativePosition));
                }
            }
        }

        private class PickedEventArgs : EventArgs
        {
            public Vector RelativePosition { get; }

            public PickedEventArgs(Vector relativePosition)
            {
                RelativePosition = relativePosition;
            }
        }
    }
}
