﻿// <copyright file="SyncGroupMembershipExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IGroupMembership">Group Membership</see>.
    /// </summary>
    public static class SyncGroupMembershipExtensions
    {
        /// <summary>
        /// Synchronously gets this membership's <see cref="IAccount">Account</see> resource.
        /// </summary>
        /// <param name="groupMembership">The group membership object.</param>
        /// <returns>This membership's <see cref="IAccount">Account</see> resource.</returns>
        public static IAccount GetAccount(this IGroupMembership groupMembership)
            => (groupMembership as IGroupMembershipSync).GetAccount();

        /// <summary>
        /// Synchronously gets this membership's <see cref="Group.IGroup">Group</see> resource.
        /// </summary>
        /// <param name="groupMembership">The group membership object.</param>
        /// <returns>This membership's <see cref="Group.IGroup">Group</see> resource.</returns>
        public static IGroup GetGroup(this IGroupMembership groupMembership)
            => (groupMembership as IGroupMembershipSync).GetGroup();
    }
}
