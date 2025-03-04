﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class MediaMaxCD3 : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            var resource = Utilities.FindResourceInSection(pex.ResourceSection, dataContains: "Cd3Ctl");
            if (resource != null)
                return $"MediaMax CD-3";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // CD3 Launch Error
                    new ContentMatchSet(new byte?[]
                    {
                        0x43, 0x44, 0x33, 0x20, 0x4C, 0x61, 0x75, 0x6E,
                        0x63, 0x68, 0x20, 0x45, 0x72, 0x72, 0x6F, 0x72
                    }, "MediaMax CD-3"),
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
                    // DllInstallSbcp
                    new ContentMatchSet(new byte?[]
                    {
                        0x44, 0x6C, 0x6C, 0x49, 0x6E, 0x73, 0x74, 0x61,
                        0x6C, 0x6C, 0x53, 0x62, 0x63, 0x70
                    }, "MediaMax CD-3"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("LaunchCd.exe", useEndsWith: true), "MediaMax CD-3"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
