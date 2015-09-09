﻿// <copyright file="FilterChain_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.DataStore.FilterChain;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class FilterChain_tests
    {
        public class Sync_tests
        {
            internal class CreateInterceptorFilter : ISynchronousFilter
            {
                IResourceDataResult ISynchronousFilter.Execute(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Action == ResourceAction.Create)
                    {
                        return new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            request.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } });
                    }
                    else
                    {
                        return chain.Execute(request, logger);
                    }
                }
            }

            internal class DeleteInterceptorFilter : ISynchronousFilter
            {
                IResourceDataResult ISynchronousFilter.Execute(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Action == ResourceAction.Delete)
                        return new DefaultResourceDataResult(ResourceAction.Delete, null, null, 204, null);
                    else
                        return chain.Execute(request, logger);
                }
            }

            [Fact]
            public void Sync_chain_terminating_on_first()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Create, new CanonicalUri("http://api.foo.bar"));
                var result = filterChain.Execute(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }

            [Fact]
            public void Sync_chain_terminating_on_second()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Delete, new CanonicalUri("http://api.foo.bar"));
                var result = filterChain.Execute(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Delete);
                result.Body.ShouldBe(null);
            }

            [Fact]
            public void Sync_with_inline_filter()
            {
                ISynchronousFilterChain defaultFilterChain = new DefaultSynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                ISynchronousFilterChain finalChain = new DefaultSynchronousFilterChain(defaultFilterChain as DefaultSynchronousFilterChain)
                    .Add(new DefaultSynchronousFilter((req, next, logger) =>
                    {
                        return new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            req.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } });
                    }));

                var request = new DefaultResourceDataRequest(ResourceAction.Create, new CanonicalUri("http://api.foo.bar"));
                var result = finalChain.Execute(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }
        }

        public class Async_tests
        {
            internal class CreateInterceptorFilter : IAsynchronousFilter
            {
                Task<IResourceDataResult> IAsynchronousFilter.ExecuteAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Action == ResourceAction.Create)
                    {
                        return Task.FromResult<IResourceDataResult>(new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            request.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } }));
                    }
                    else
                    {
                        return chain.ExecuteAsync(request, logger, cancellationToken);
                    }
                }
            }

            internal class DeleteInterceptorFilter : IAsynchronousFilter
            {
                Task<IResourceDataResult> IAsynchronousFilter.ExecuteAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Action == ResourceAction.Delete)
                    {
                        return Task.FromResult<IResourceDataResult>(
                            new DefaultResourceDataResult(ResourceAction.Delete, null, null, 204, null));
                    }
                    else
                    {
                        return chain.ExecuteAsync(request, logger, cancellationToken);
                    }
                }
            }

            [Fact]
            public async Task Async_chain_terminating_on_first()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Create, new CanonicalUri("http://api.foo.bar"));
                var result = await filterChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }

            [Fact]
            public async Task Async_chain_terminating_on_second()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Delete, new CanonicalUri("http://api.foo.bar"));
                var result = await filterChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Delete);
                result.Body.ShouldBe(null);
            }

            [Fact]
            public async Task Async_with_inline_filter()
            {
                IAsynchronousFilterChain defaultFilterChain = new DefaultAsynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                IAsynchronousFilterChain finalChain = new DefaultAsynchronousFilterChain(defaultFilterChain as DefaultAsynchronousFilterChain)
                    .Add(new DefaultAsynchronousFilter((req, next, logger, ct) =>
                    {
                        return Task.FromResult<IResourceDataResult>(new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            req.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } }));
                    }));

                var request = new DefaultResourceDataRequest(ResourceAction.Create, new CanonicalUri("http://api.foo.bar"));
                var result = await finalChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }
        }
    }
}
