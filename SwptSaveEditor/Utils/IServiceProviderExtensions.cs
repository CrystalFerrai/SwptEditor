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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/>
    /// </summary>
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// Returns the service of the specified type
        /// </summary>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Finds the service of the specified type and injects it into a target field. Throws <see cref="ServiceNotFoundException"/> on failure.
        /// </summary>
        /// <exception cref="ServiceNotFoundException">Thrown if a service of the given type could not be located</exception>
        public static void Inject<T>(this IServiceProvider provider, out T target) where T : class
        {
            target = provider.GetService<T>();
            if (target == null) throw new ServiceNotFoundException(typeof(T));
        }
    }

    /// <summary>
    /// Exception thrown when a service could not be located
    /// </summary>
    [Serializable]
    public class ServiceNotFoundException : Exception
    {
        public Type Type { get; private set; }

        public ServiceNotFoundException(Type type)
            : this(type, $"Service of type {type.FullName} could not be located")
        {
        }

        public ServiceNotFoundException(Type type, string message)
            : this(type, message, null)
        {
        }

        public ServiceNotFoundException(Type type, string message, Exception inner)
            : base(message, inner)
        {
            Type = type;
        }

        protected ServiceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Type = (Type)info.GetValue(nameof(Type), typeof(Type));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Type), Type);
        }
    }
}
