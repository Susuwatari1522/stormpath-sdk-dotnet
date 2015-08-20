﻿// <copyright file="WindowsVersionHelper.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
    internal static class WindowsVersionHelper
    {
        // List from https://msdn.microsoft.com/en-us/library/windows/desktop/ms724833(v=vs.85).aspx
        private static readonly int NTWorkstation = 1;
        private static readonly int NTDomainController = 2;
        private static readonly int NTServer = 3;
        private static readonly Dictionary<WindowsVersion, string> WindowsVersionLookupTable =
            new Dictionary<WindowsVersion, string>()
            {
                { new WindowsVersion(5, 0), "2000" },
                { new WindowsVersion(5, 1), "XP" },
                { new WindowsVersion(5, 2), "Server-2003" },
                { new WindowsVersion(6, 0, NTWorkstation), "Vista" },
                { new WindowsVersion(6, 0, NTDomainController), "Server-2008" },
                { new WindowsVersion(6, 0, NTServer), "Server-2008" },
                { new WindowsVersion(6, 1, NTWorkstation), "7" },
                { new WindowsVersion(6, 1, NTDomainController), "Server-2008-R2" },
                { new WindowsVersion(6, 1, NTServer), "Server-2008-R2" },
                { new WindowsVersion(6, 2, NTWorkstation), "8" },
                { new WindowsVersion(6, 2, NTDomainController), "Server-2012" },
                { new WindowsVersion(6, 2, NTServer), "Server-2012" },
                { new WindowsVersion(6, 3, NTWorkstation), "8.1" },
                { new WindowsVersion(6, 3, NTDomainController), "Server-2012-R2" },
                { new WindowsVersion(6, 3, NTServer), "Server-2012-R2" },
                { new WindowsVersion(10, 0), "10" },
            };

        public static string GetWindowsOSVersion()
        {
            var major = Environment.OSVersion.Version.Major;
            var minor = Environment.OSVersion.Version.Minor;
            var productType = GetProductType();

            string version;
            if (!WindowsVersionLookupTable.TryGetValue(new WindowsVersion(major, minor, productType), out version))
                return $"unknown-{major}.{minor}.{productType}";

            return version;
        }

        private static int? GetProductType()
        {
            var osVersionInfo = new SafeNativeMethods.OSVERSIONINFOEX();
            osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(SafeNativeMethods.OSVERSIONINFOEX));

            if (!SafeNativeMethods.GetVersionEx(ref osVersionInfo))
                return null;

            return osVersionInfo.wProductType;
        }
    }
}