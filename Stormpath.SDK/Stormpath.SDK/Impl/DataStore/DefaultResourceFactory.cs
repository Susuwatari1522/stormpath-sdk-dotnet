﻿// <copyright file="DefaultResourceFactory.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceFactory : IResourceFactory
    {
        private readonly IInternalDataStore dataStore;
        private readonly IIdentityMap<ResourceData> identityMap;
        private bool isDisposed = false; // To detect redundant calls

        public DefaultResourceFactory(IInternalDataStore dataStore, IIdentityMap<ResourceData> identityMap)
        {
            this.dataStore = dataStore;
            this.identityMap = identityMap;
        }

        private IResourceFactory AsInterface => this;

        T IResourceFactory.Create<T>(ILinkable original)
            => (T)this.Create(typeof(T), null, original);

        private object Create(Type type, ILinkable original)
            => this.Create(type, null, original);

        T IResourceFactory.Create<T>(IDictionary<string, object> properties, ILinkable original)
            => (T)this.Create(typeof(T), properties, original);

        private object Create(Type type, IDictionary<string, object> properties, ILinkable original)
        {
            if (ResourceTypeLookup.IsCollectionResponse(type))
                return this.InstantiateCollection(type, properties);

            return this.InstantiateSingle(type, properties, original);
        }

        private object InstantiateSingle(Type type, IDictionary<string, object> properties, ILinkable original)
        {
            var targetType = new ResourceTypeLookup().GetConcrete(type);
            if (targetType == null)
                throw new ApplicationException($"Unknown resource type {type.Name}");

            var identityMapOptions = new IdentityMapOptionsResolver().GetOptions(type);

            AbstractResource targetObject;
            try
            {
                string id = RandomResourceId(type.Name);

                if (properties == null)
                    properties = new Dictionary<string, object>();

                object href = null;
                bool propertiesContainsHref =
                    properties.TryGetValue("href", out href) &&
                    href != null;
                if (propertiesContainsHref)
                    id = $"{type.Name}/{href.ToString()}";

                if (!propertiesContainsHref)
                    properties["href"] = id;

                var resourceData = identityMapOptions.SkipIdentityMap
                    ? new ResourceData(this.dataStore)
                    : this.identityMap.GetOrAdd(id, () => new ResourceData(this.dataStore), identityMapOptions.StoreWithInfiniteExpiration);

                if (properties != null)
                    resourceData.Update(properties);

                targetObject = Activator.CreateInstance(targetType, new object[] { resourceData }) as AbstractResource;

                var notifyableTarget = targetObject as INotifiable;
                if (notifyableTarget != null)
                    notifyableTarget.OnUpdate(properties, this.dataStore);

                if (original != null)
                    original.Link(resourceData);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error creating resource type {targetType.Name}", e);
            }

            return targetObject;
        }

        private object InstantiateCollection(Type collectionType, IDictionary<string, object> properties)
        {
            Type innerType = new ResourceTypeLookup().GetInnerCollectionInterface(collectionType);
            var targetType = new ResourceTypeLookup().GetConcrete(innerType);
            if (innerType == null || targetType == null)
                throw new ApplicationException($"Error creating collection resource: unknown inner type '{innerType?.Name}'.");

            if (properties == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: no properties to materialize with.");

            long offset, limit, size;
            try
            {
                offset = Convert.ToInt64(properties["offset"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'offset' value.", ex);
            }

            try
            {
                limit = Convert.ToInt64(properties["limit"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'limit' value.", ex);
            }

            try
            {
                size = Convert.ToInt64(properties["size"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'size' value.", ex);
            }

            var href = properties["href"]?.ToString();
            if (string.IsNullOrEmpty(href))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'href' value.");

            var items = properties["items"] as IEnumerable<IDictionary<string, object>>;
            if (items == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: items subcollection is invalid.");

            try
            {
                Type listOfInnerType = typeof(List<>).MakeGenericType(innerType);
                var materializedItems = listOfInnerType.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
                var addMethod = listOfInnerType.GetMethod("Add", new Type[] { innerType });

                foreach (var itemMap in items)
                {
                    var materialized = this.InstantiateSingle(innerType, itemMap, original: null);
                    addMethod.Invoke(materializedItems, new object[] { materialized });
                }

                object targetObject;
                targetObject = Activator.CreateInstance(collectionType, new object[] { href, offset, limit, size, materializedItems });

                return targetObject;
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: failed to add items to collection.", e);
            }
        }

        private static string RandomResourceId(string typeName)
            => $"autogen://{typeName}/{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", string.Empty)}";

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.identityMap.Dispose();
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
