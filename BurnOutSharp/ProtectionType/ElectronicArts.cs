﻿using System;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Do more research into the Cucko protection:
    //      - Reference to `EASTL` and `EAStdC` are standard for EA products and does not indicate Cucko by itself
    //      - There's little information outside of PiD detection that actually knows about Cucko
    //      - Look into `ccinstall`, `Services/EACOM`, `TSLHost`, `SIGS/UploadThread/exchangeAuthToken`,
    //          `blazeURL`, `psapi.dll`, `DasmX86Dll.dll`, `NVCPL.dll`, `iphlpapi.dll`, `dbghelp.dll`,
    //          `WS2_32.dll`, 
    public class ElectronicArts : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("EReg MFC Application"))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.Contains("Registration code installer program"))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";
            else if (!string.IsNullOrWhiteSpace(name) && name.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase))
                return $"EA DRM Protection {Utilities.GetFileVersion(pex)}";

            name = Utilities.GetInternalName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CDCode", StringComparison.Ordinal))
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";

            var resource = Utilities.FindResourceInSection(pex.ResourceSection, dataContains: "A\0b\0o\0u\0t\0 \0C\0D\0K\0e\0y");
            if (resource != null)
                return $"EA CdKey Registration Module {Utilities.GetFileVersion(pex)}";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // EReg Config Form
                    new ContentMatchSet(new byte?[]
                    {
                        0x45, 0x52, 0x65, 0x67, 0x20, 0x43, 0x6F, 0x6E,
                        0x66, 0x69, 0x67, 0x20, 0x46, 0x6F, 0x72, 0x6D
                    }, Utilities.GetFileVersion, "EA CdKey Registration Module"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                        0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                        0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                    }, "EA DRM Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .text section, if it exists
            if (pex.TextSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45,
                        0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69,
                        0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E
                    }, "EA DRM Protection"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.TextSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
