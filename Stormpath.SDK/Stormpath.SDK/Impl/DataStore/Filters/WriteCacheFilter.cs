﻿// <copyright file="WriteCacheFilter.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class WriteCacheFilter : IAsynchronousFilter, ISynchronousFilter
    {
        private readonly ICacheResolver cacheResolver;

        public WriteCacheFilter(ICacheResolver cacheResolver)
        {
            this.cacheResolver = cacheResolver;
        }

        IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}