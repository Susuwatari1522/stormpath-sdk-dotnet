﻿// <copyright file="MemoryCacheIdentityMap.cs" company="Stormpath, Inc.">
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
using System.Runtime.Caching;
using System.Threading;

namespace Stormpath.SDK.Impl.IdentityMap
{
    internal class MemoryCacheIdentityMap<TKey, TItem> : IIdentityMap<TKey, TItem>, IDisposable
        where TItem : class
    {
        private readonly MemoryCache itemCache;
        private readonly TimeSpan slidingExpiration;
        private long lifetimeItemsAdded;
        private bool isDisposed = false; // To detect redundant calls

        public MemoryCacheIdentityMap(TimeSpan slidingExpiration)
        {
            this.itemCache = new MemoryCache("StormpathSDKIdentityMap");
            this.slidingExpiration = slidingExpiration;
        }

        private static string CreateKey(TKey key)
            => $"idmap-{key.ToString()}";

        public long LifetimeItemsAdded => this.lifetimeItemsAdded;

        public TItem GetOrAdd(TKey key, Func<TItem> itemFactory)
        {
            var lazyItem = new Lazy<TItem>(() => itemFactory());
            var policy = new CacheItemPolicy() { SlidingExpiration = this.slidingExpiration };
            var existing = this.itemCache.AddOrGetExisting(CreateKey(key), lazyItem, policy);

            bool added = existing == null;
            if (added)
                Interlocked.Increment(ref this.lifetimeItemsAdded);

            return existing == null
                ? lazyItem.Value
                : (existing as Lazy<TItem>).Value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.itemCache.Dispose();
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Don't change this method. Change Dispose(bool disposing) instead
            this.Dispose(true);
        }
    }
}