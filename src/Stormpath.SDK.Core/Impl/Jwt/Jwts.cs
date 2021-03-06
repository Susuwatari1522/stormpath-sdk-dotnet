﻿// <copyright file="Jwts.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Jwt;

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Utility class for creating <see cref="IJwtClaimsBuilder"/> instances, used for constructing <see cref="IJwtClaims"/> instances.
    /// </summary>
    [Obsolete("Use the JWT methods available on IClient.")]
    public static class Jwts
    {
        /// <summary>
        /// Creates a new <see cref="IJwtClaimsBuilder"/>, used to construct <see cref="IJwtClaims"/> instances.
        /// </summary>
        /// <returns>A new <see cref="IJwtClaimsBuilder"/>.</returns>
        [Obsolete("Use IClient.NewJwtBuilder() instead.")]
        public static IJwtClaimsBuilder NewClaimsBuilder()
            => new DefaultJwtClaimsBuilder();
    }
}
