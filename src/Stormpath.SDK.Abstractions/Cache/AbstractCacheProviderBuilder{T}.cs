﻿// <copyright file="AbstractCacheProviderBuilder{T}.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Base class for implementations of <see cref="ICacheProviderBuilder"/>.
    /// </summary>
    /// <typeparam name="T">The cache provider type.</typeparam>
    public abstract class AbstractCacheProviderBuilder<T> : ICacheProviderBuilder
        where T : AbstractCacheProvider, new()
    {
        private readonly List<ICacheConfiguration> cacheConfigs
            = new List<ICacheConfiguration>();

        private TimeSpan? defaultTimeToLive;
        private TimeSpan? defaultTimeToIdle;

        /// <inheritdoc/>
        ICacheProviderBuilder ICacheProviderBuilder.WithDefaultTimeToIdle(TimeSpan tti)
        {
            this.defaultTimeToIdle = tti;
            return this;
        }

        /// <inheritdoc/>
        ICacheProviderBuilder ICacheProviderBuilder.WithDefaultTimeToLive(TimeSpan ttl)
        {
            this.defaultTimeToLive = ttl;
            return this;
        }

        /// <inheritdoc/>
        ICacheProviderBuilder ICacheProviderBuilder.WithCache(ICacheConfigurationBuilder builder)
        {
            var cacheConfig = builder.Build();
            if (cacheConfig == null)
            {
                throw new Exception("The cache configuration is not valid.");
            }

            this.cacheConfigs.Add(cacheConfig);
            return this;
        }

        /// <summary>
        /// Fired when the builder is constructing a provider.
        /// </summary>
        /// <remarks>Override in a derived class to perform additional configuration on the provider.</remarks>
        /// <param name="provider">The constructed provider.</param>
        /// <returns>The provider.</returns>
        protected virtual ICacheProvider OnBuilding(T provider)
        {
            return provider;
        }

        /// <inheritdoc/>
        ICacheProvider ICacheProviderBuilder.Build()
        {
            var provider = new T();

            if (this.defaultTimeToLive.HasValue)
            {
                provider.SetDefaultTimeToLive(this.defaultTimeToLive.Value);
            }

            if (this.defaultTimeToIdle.HasValue)
            {
                provider.SetDefaultTimeToIdle(this.defaultTimeToIdle.Value);
            }

            if (this.cacheConfigs.Any())
            {
                provider.SetCacheConfigurations(this.cacheConfigs);
            }

            return this.OnBuilding(provider);
        }
    }
}
