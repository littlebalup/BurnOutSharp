﻿using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.ProtectionType
{
    // CodeLock / CodeLok / CopyLok
    public class CodeLock : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;
            
            // If there are more than 2 icd-prefixed sections, then we have a match
            int icdSectionCount = pex.GetSectionNames().Count(s => s.StartsWith("icd"));
            if (icdSectionCount >= 2)
                return "CodeLock / CodeLok / CopyLok";

            return null;
            
            // TODO: Could not be confirmed with any sample, so disabling for now
            // var contentMatchSets = new List<ContentMatchSet>
            // {
            //     // CODE-LOCK.OCX
            //     new ContentMatchSet(new byte?[]
            //     {
            //         0x43, 0x4F, 0x44, 0x45, 0x2D, 0x4C, 0x4F, 0x43,
            //         0x4B, 0x2E, 0x4F, 0x43, 0x58
            //     }, "CodeLock / CodeLok / CopyLok"),
            // };
            
            // return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
        }
    }
}
