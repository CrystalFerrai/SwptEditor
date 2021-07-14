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

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Provides a registry of services to use for service location
    /// </summary>
    public interface IServiceRegistry : IServiceProvider
    {
        /// <summary>
        /// Adds a service to the registry
        /// </summary>
        /// <param name="serviceType">The type to register the service as</param>
        /// <param name="service">The service to register</param>
        void AddService(Type serviceType, object service);

        /// <summary>
        /// Removes a service from the registry
        /// </summary>
        /// <param name="serviceType">The registered type of the service to remove</param>
        /// <param name="service">The service to remove</param>
        /// <returns>true if the service was found and removed, false if it was not found</returns>
        bool RemoveService(Type serviceType, object service);
    }

    /// <summary>
    /// Extension methods for IServiceRegistry
    /// </summary>
    public static class IServiceRegistryExtensions
    {
        /// <summary>
        /// Adds a service to the registry
        /// </summary>
        /// <typeparam name="T">The type to register the service as</typeparam>
        /// <param name="registry">The registry to add the service to</param>
        /// <param name="service">The service to register</param>
        public static void AddService<T>(this IServiceRegistry registry, T service)
        {
            registry.AddService(typeof(T), service);
        }

        /// <summary>
        /// Removes a service from the registry
        /// </summary>
        /// <typeparam name="T">The registered type of the service to remove</typeparam>
        /// <param name="registry">The registry to remove the service from</param>
        /// <param name="service">The service to remove</param>
        /// <returns>true if the service was found and removed, false if it was not found</returns>
        public static bool RemoveService<T>(this IServiceRegistry registry, T service)
        {
            return registry.RemoveService(typeof(T), service);
        }
    }
}
