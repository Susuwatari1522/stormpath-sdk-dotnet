﻿// <copyright file="DefaultRequest.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Http.Support;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultRequest : IHttpRequest
    {
        private readonly HttpMethod method;
        private readonly ICanonicalUri canonicalUri;
        private readonly HttpHeaders headers;
        private readonly string body;

        // Copy constructor
        public DefaultRequest(IHttpRequest existingRequest, Uri overrideUri = null)
        {
            this.body = existingRequest.Body;
            this.headers = new HttpHeaders(existingRequest.Headers);
            this.method = HttpMethod.Parse(existingRequest.Method);
            this.canonicalUri = new DefaultCanonicalUri(existingRequest.CanonicalUri, overrideResourcePath: overrideUri);
        }

        public DefaultRequest(HttpMethod method, ICanonicalUri canonicalUri)
            : this(method, canonicalUri, null, null, string.Empty)
        {
        }

        public DefaultRequest(HttpMethod method, ICanonicalUri canonicalUri, QueryString queryParams, HttpHeaders headers, string body)
        {
            this.method = method;
            this.canonicalUri = canonicalUri;

            bool queryParamsWerePassed = queryParams?.Any() ?? false;
            if (queryParamsWerePassed)
            {
                var mergedQueryString = this.canonicalUri.QueryString.Merge(queryParams);
                this.canonicalUri = new DefaultCanonicalUri(this.canonicalUri.ResourcePath.ToString(), mergedQueryString);
            }

            this.headers = headers;
            if (headers == null)
                this.headers = new HttpHeaders();

            this.body = body;
        }

        public string Body => body;

        public bool HasBody => !string.IsNullOrEmpty(body);

        public HttpHeaders Headers => headers;

        public HttpMethod Method => method;

        public ICanonicalUri CanonicalUri => canonicalUri;
    }
}