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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Provides functionality useful when implementing IDataErrorInfo or INotifyDataErrorInfo
    /// </summary>
    internal class DataErrorHelper
    {
        /// <summary>
        /// The object that owns this instance, acts as the sender when raising events.
        /// </summary>
        private readonly object mOwner;

        /// <summary>
        /// Mapping of property names to errors
        /// </summary>
        private readonly Dictionary<string, string> mErrors;

        /// <summary>
        /// Gets whether any errors are set for any property
        /// </summary>
        public bool HasErrors => mErrors.Any();

        /// <summary>
        /// Raised when the error for a given property has changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Creates a new instance of the DataErrorHelper class
        /// </summary>
        /// <param name="owner">The object that owns this instance. Events will be raised with the owning object acting as the sender of the events.</param>
        public DataErrorHelper(object owner)
        {
            mOwner = owner;
            mErrors = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns the error registered for the given property.
        /// </summary>
        /// <param name="propertyName">The property to get the error from</param>
        /// <remarks>
        /// This class only supports one error per property. This method returns an IEnumerable to facilitate implementation of DataErrorInfo related interfaces.
        /// </remarks>
        public IEnumerable<string> GetErrors(string propertyName)
        {
            string error;
            if (mErrors.TryGetValue(propertyName, out error))
            {
                yield return error;
            }
        }

        /// <summary>
        /// Sets the error for a given property
        /// </summary>
        /// <param name="propertyName">The name of the property to set an error on</param>
        /// <param name="error">The error message</param>
        public void SetError(string propertyName, string error)
        {
            mErrors[propertyName] = error;
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clears the error from a given property
        /// </summary>
        /// <param name="propertyName">The name of the property to clear the error from</param>
        public void ClearError(string propertyName)
        {
            mErrors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Helper method to raise the ErrorsChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property for which errors have changed</param>
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(mOwner, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
