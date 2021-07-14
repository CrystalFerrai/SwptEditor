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

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// A registry of services to use for service location
    /// </summary>
    public class ServiceRegistry : IServiceRegistry
    {
        /// <summary>
        /// The parent service locator used to locate services that this instance does not have registered
        /// </summary>
        private IServiceProvider mParent;

        /// <summary>
        /// A dictionary mapping types to registered services
        /// </summary>
        private Dictionary<Type, object> mServices;

        /// <summary>
        /// Creates a new instance of the ServiceRegistry class
        /// </summary>
        /// <param name="parentLocator">An optional service locator to fallback on if a requested service is not available in this registry</param>
        public ServiceRegistry(IServiceProvider parentLocator = null)
        {
            mParent = parentLocator;
            mServices = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Adds a service to the registry
        /// </summary>
        /// <param name="serviceType">The type to register the service as</param>
        /// <param name="service">The service to register</param>
        /// <exception cref="ArgumentNullException">One of the passed in parameters is null</exception>
        /// <exception cref="ArgumentException">The passed in service is not assignable to the specified type</exception>
        /// <exception cref="InvalidOperationException">A service of the specified type is already registered with this registry</exception>
        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (service == null) throw new ArgumentNullException("service");
            if (!serviceType.IsAssignableFrom(service.GetType())) throw new ArgumentException(string.Format("A service of type [{0}] cannot be registered as type [{1}].", service.GetType().FullName, serviceType.FullName));
            if (mServices.ContainsKey(serviceType)) throw new InvalidOperationException(string.Format("A service of type [{0}] is already registered.", serviceType.FullName));

            mServices.Add(serviceType, service);
        }

        /// <summary>
        /// Removes a service from the registry
        /// </summary>
        /// <param name="serviceType">The registered type of the service to remove</param>
        /// <param name="service">The service to remove</param>
        /// <returns>true if the service was found and removed, false if it was not found</returns>
        /// <exception cref="ArgumentNullException">One of the passed in parameters is null</exception>
        public bool RemoveService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (service == null) throw new ArgumentNullException("service");
            object registeredService;
            if (mServices.TryGetValue(serviceType, out registeredService) && registeredService == service)
            {
                return mServices.Remove(serviceType);
            }
            return false;
        }

        /// <summary>
        /// Gets a service of the specified type
        /// </summary>
        /// <param name="serviceType">The type of the service to get</param>
        /// <returns>The service instance if it was found, otherwise null</returns>
        /// <exception cref="ArgumentNullException">serviceType is null</exception>
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            object value;
            if (mServices.TryGetValue(serviceType, out value))
            {
                return value;
            }
            return mParent?.GetService(serviceType);
        }
    }
}
