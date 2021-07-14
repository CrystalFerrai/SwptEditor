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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwptSaveLib.Utils
{
    /// <summary>
    /// A base class for things that need to notify when their property values have changed
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// Event fired when the the value of a property is about to change
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Event fired when the value of a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires an event indicating that a property value is about the change
        /// </summary>
        /// <param name="propertyName">The name of the property that is about to change</param>
        protected void NotifyPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Fires an event indicating that one of more property values are about the change
        /// </summary>
        /// <param name="propertyName">The names of the properties that are about to change</param>
        protected void NotifyPropertyChanging(params string[] propertyNames)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                foreach (String propertyName in propertyNames)
                {
                    handler.Invoke(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Fires an event indicating that a property value has changed
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Fires an event indicating that one of more property values have changed
        /// </summary>
        /// <param name="propertyName">The names of the properties have changed</param>
        protected void NotifyPropertyChanged(params string[] propertyNames)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                foreach (String propertyName in propertyNames)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// </remarks>
        /// <typeparam name="T">The type of the field and value to set</typeparam>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (field != null && !field.Equals(value) || field == null && value != null)
            {
                NotifyPropertyChanging(propertyName);
                field = value;
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// Special case handling considers two float.NaN values to be equal.
        /// </remarks>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set(ref float field, float value, [CallerMemberName] string propertyName = null)
        {
            if (field != value && !(float.IsNaN(field) && float.IsNaN(value)))
            {
                NotifyPropertyChanging(propertyName);
                field = value;
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// Special case handling considers two double.NaN values to be equal.
        /// </remarks>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set(ref double field, double value, [CallerMemberName] string propertyName = null)
        {
            if (field != value && !(double.IsNaN(field) && double.IsNaN(value)))
            {
                NotifyPropertyChanging(propertyName);
                field = value;
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// </remarks>
        /// <typeparam name="T">The type of the field and value to set</typeparam>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="changedCallback">A callback to call when the property changes, before notifying listeners about the change</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set<T>(ref T field, T value, PropertyChangedCallback<T> changedCallback, [CallerMemberName] string propertyName = null)
        {
            if (field != null && !field.Equals(value) || field == null && value != null)
            {
                NotifyPropertyChanging(propertyName);
                T oldValue = field;
                field = value;
                if (changedCallback != null) changedCallback(oldValue, value);
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// Special case handling considers two float.NaN values to be equal.
        /// </remarks>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="changedCallback">A callback to call when the property changes, before notifying listeners about the change</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set(ref float field, float value, PropertyChangedCallback<float> changedCallback, [CallerMemberName] string propertyName = null)
        {
            if (field != value && !(float.IsNaN(field) && float.IsNaN(value)))
            {
                NotifyPropertyChanging(propertyName);
                float oldValue = field;
                field = value;
                if (changedCallback != null) changedCallback(oldValue, value);
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call from a property setter to set the value of a field and fire notifications about the change.
        /// </summary>
        /// <remarks>
        /// If the field and value parameters are equal, then the field is not set and notifications are not fired.
        /// Special case handling considers two double.NaN values to be equal.
        /// </remarks>
        /// <param name="field">A reference to the field to set</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="changedCallback">A callback to call when the property changes, before notifying listeners about the change</param>
        /// <param name="propertyName">The name of the property to fire a notification about</param>
        /// <returns>Whether the new property value differs from the previous value</returns>
        protected bool Set(ref double field, double value, PropertyChangedCallback<double> changedCallback, [CallerMemberName] string propertyName = null)
        {
            if (field != value && !(double.IsNaN(field) && double.IsNaN(value)))
            {
                NotifyPropertyChanging(propertyName);
                double oldValue = field;
                field = value;
                if (changedCallback != null) changedCallback(oldValue, value);
                NotifyPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Represents a method that is called when a property value changes
        /// </summary>
        /// <typeparam name="T">The type of the property that changed</typeparam>
        /// <param name="oldValue">The value of the propery before the change</param>
        /// <param name="newValue">The value of the property after the change</param>
        protected delegate void PropertyChangedCallback<T>(T oldValue, T newValue);
    }
}
