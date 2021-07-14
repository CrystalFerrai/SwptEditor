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
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Base class for view models that supports observable properties and property error notifications
    /// </summary>
    internal abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo, IDisposable
    {
        /// <summary>
        /// Helper for setting/clearing property errors
        /// </summary>
        private readonly DataErrorHelper mDataErrorHelper;

        /// <summary>
        /// Gets whether any properties have any errors associated with them
        /// </summary>
        public bool HasErrors => mDataErrorHelper.HasErrors;

        /// <summary>
        /// Raised when errors associated with any property changes
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { mDataErrorHelper.ErrorsChanged += value; }
            remove { mDataErrorHelper.ErrorsChanged -= value; }
        }

        /// <summary>
        /// Creates a new instance of the ViewModelBase class
        /// </summary>
        protected ViewModelBase()
        {
            mDataErrorHelper = new DataErrorHelper(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ViewModelBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Frees all resources associated with this instance
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Returns errors associated with the specified property
        /// </summary>
        /// <param name="propertyName">The name of the property for which to get errors</param>
        public IEnumerable GetErrors(string propertyName)
        {
            return mDataErrorHelper.GetErrors(propertyName);
        }

        /// <summary>
        /// Sets the error associated with the specified property
        /// </summary>
        /// <param name="error">The error message to set</param>
        /// <param name="propertyName">The name of the property to set the error on</param>
        protected void SetError(string error, [CallerMemberName] string propertyName = null)
        {
            mDataErrorHelper.SetError(propertyName, error);
        }

        /// <summary>
        /// Clears the error associated with the specified property
        /// </summary>
        /// <param name="propertyName">The name of the property to clear the error from</param>
        protected void ClearError([CallerMemberName] string propertyName = null)
        {
            mDataErrorHelper.ClearError(propertyName);
        }

        /// <summary>
        /// Overridable method to clean up resources
        /// </summary>
        /// <param name="disposing">Whether managed resources and event registrations should be cleaned up</param>
        /// <remarks>
        /// The disposing parameter indicates whether managed resources should be cleaned up. Unmanaged resources
        /// should always be cleaned up regardless of the value of disposing.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
