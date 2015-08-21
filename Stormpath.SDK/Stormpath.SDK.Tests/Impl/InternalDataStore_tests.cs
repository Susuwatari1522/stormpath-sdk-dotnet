﻿// <copyright file="InternalDataStore_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Fakes;
using Xunit;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Application;

namespace Stormpath.SDK.Tests.Impl
{
    public class InternalDataStore_tests
    {
        private readonly IRequestExecutor fakeRequestExecutor;
        private readonly IDataStore dataStore;

        public InternalDataStore_tests()
        {
            fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            dataStore = new InternalDataStore(fakeRequestExecutor, "http://foobar");
        }

        [Fact]
        public async Task Instantiating_Account_from_JSON()
        {
            var href = "http://foobar/account";
            fakeRequestExecutor.GetAsync(new Uri(href), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(FakeJson.Account));

            var account = await dataStore.GetResourceAsync<IAccount>(href);

            // Verify against data from FakeJson.Account
            account.CreatedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.078Z"));
            account.Email.ShouldBe("han.solo@corellia.core");
            account.FullName.ShouldBe("Han Solo");
            account.GivenName.ShouldBe("Han");
            account.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount");
            account.MiddleName.ShouldBe(null);
            account.ModifiedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.078Z"));
            account.Status.ShouldBe(AccountStatus.Enabled);
            account.Surname.ShouldBe("Solo");
            account.Username.ShouldBe("han.solo@corellia.core");

            (account as DefaultAccount).AccessTokens.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/accessTokens");
            (account as DefaultAccount).ApiKeys.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/apiKeys");
            (account as DefaultAccount).Applications.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/applications");
            (account as DefaultAccount).CustomData.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/customData");
            (account as DefaultAccount).Directory.Href.ShouldBe("https://api.stormpath.com/v1/directories/foobarDirectory");
            (account as DefaultAccount).EmailVerificationToken.Href.ShouldBe(null);
            (account as DefaultAccount).GroupMemberships.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/groupMemberships");
            (account as DefaultAccount).Groups.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/groups");
            (account as DefaultAccount).ProviderData.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/providerData");
            (account as DefaultAccount).RefreshTokens.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/refreshTokens");
            (account as DefaultAccount).Tenant.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foobarTenant");
        }

        [Fact]
        public async Task Instantiating_Tenant_from_JSON()
        {
            var href = "http://foobar/tenant";
            fakeRequestExecutor.GetAsync(new Uri(href))
                .Returns(Task.FromResult(FakeJson.Tenant));

            var tenant = await dataStore.GetResourceAsync<ITenant>(href);

            // Verify against data from FakeJson.Tenant
            tenant.CreatedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.058Z"));
            tenant.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar");
            tenant.Key.ShouldBe("foo-bar");
            tenant.ModifiedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.579Z"));
            tenant.Name.ShouldBe("foo-bar");

            (tenant as DefaultTenant).Accounts.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/accounts");
            (tenant as DefaultTenant).Agents.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/agents");
            (tenant as DefaultTenant).Applications.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/applications");
            (tenant as DefaultTenant).CustomData.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/customData");
            (tenant as DefaultTenant).Directories.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/directories");
            (tenant as DefaultTenant).Groups.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/groups");
            (tenant as DefaultTenant).IdSites.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/idSites");
            (tenant as DefaultTenant).Organizations.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/organizations");
        }

        [Fact]
        public async Task Instantiating_Application_from_JSON()
        {
            var href = "http://foobar/application";
            fakeRequestExecutor.GetAsync(new Uri(href))
                .Returns(Task.FromResult(FakeJson.Application));

            var application = await dataStore.GetResourceAsync<IApplication>(href);

            // Verify against data from FakeJson.Application
            application.CreatedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.563Z"));
            application.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication");
            application.Description.ShouldBe("This application is so awesome, you don't even know.");
            application.ModifiedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.622Z"));
            application.Name.ShouldBe("Lightsabers Galore");
            application.Status.ShouldBe(ApplicationStatus.Enabled);

            (application as DefaultApplication).AccountStoreMappings.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/accountStoreMappings");
            (application as DefaultApplication).Accounts.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/accounts");
            (application as DefaultApplication).ApiKeys.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/apiKeys");
            (application as DefaultApplication).AuthTokens.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/authTokens");
            (application as DefaultApplication).CustomData.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/customData");
            (application as DefaultApplication).DefaultAccountStoreMapping.Href.ShouldBe("https://api.stormpath.com/v1/accountStoreMappings/foobarASM");
            (application as DefaultApplication).DefaultGroupStoreMapping.Href.ShouldBe("https://api.stormpath.com/v1/accountStoreMappings/foobarASM");
            (application as DefaultApplication).Groups.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/groups");
            (application as DefaultApplication).LoginAttempts.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/loginAttempts");
            (application as DefaultApplication).OAuthPolicy.Href.ShouldBe("https://api.stormpath.com/v1/oAuthPolicies/foobarApplication");
            (application as DefaultApplication).PasswordResetToken.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/passwordResetTokens");
            (application as DefaultApplication).Tenant.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foobarTenant");
            (application as DefaultApplication).VerificationEmails.Href.ShouldBe("https://api.stormpath.com/v1/applications/foobarApplication/verificationEmails");

        }

        [Fact]
        public async Task Instantiating_application_list_from_JSON()
        {
            var href = "http://foobar/applications";
            fakeRequestExecutor.GetAsync(new Uri(href))
                .Returns(Task.FromResult(FakeJson.ApplicationList));

            var applicationList = await dataStore.GetCollectionAsync<IApplication>(href);

            applicationList.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foobarTenant/applications");
            applicationList.Size.ShouldBe(2);
            applicationList.Offset.ShouldBe(0);
            applicationList.Limit.ShouldBe(25);
            applicationList.Items.Count.ShouldBe(2);
        }
    }
}
