﻿// <copyright file="DefaultClient.ITenantActions.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed partial class DefaultClient
    {
        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, creationOptionsAction, cancellationToken).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, creationOptions, cancellationToken).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, cancellationToken).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(name, createDirectory, cancellationToken).ConfigureAwait(false);
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateDirectoryAsync(directory, cancellationToken).ConfigureAwait(false);
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateDirectoryAsync(directory, creationOptionsAction, cancellationToken).ConfigureAwait(false);
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, IDirectoryCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateDirectoryAsync(directory, creationOptions, cancellationToken).ConfigureAwait(false);
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(string name, string description, DirectoryStatus status, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateDirectoryAsync(name, description, status, cancellationToken).ConfigureAwait(false);
        }

        async Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            await this.EnsureTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.VerifyAccountEmailAsync(token, cancellationToken).ConfigureAwait(false);
        }
    }
}