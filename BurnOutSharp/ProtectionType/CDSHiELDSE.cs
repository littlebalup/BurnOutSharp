﻿using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the code/CODE section, if it exists
            var codeSectionRaw = pex.ReadRawSection(fileContent, "code", first: true) ?? pex.ReadRawSection(fileContent, "CODE", first: true);
            if (codeSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // ~0017.tmp
                    new ContentMatchSet(new byte?[] { 0x7E, 0x30, 0x30, 0x31, 0x37, 0x2E, 0x74, 0x6D, 0x70 }, "CDSHiELD SE"),
                };

                string match = MatchUtil.GetFirstMatch(file, codeSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }
    }
}
