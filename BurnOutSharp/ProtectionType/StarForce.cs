﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetLegalCopyright(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Protection Technology"))
                return $"StarForce {Utilities.GetFileVersion(pex)}";

            // TODO: Find what fvinfo field actually maps to this
            name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("Protected Module"))
                return $"StarForce 5";

            // Get the .rsrc section, if it exists
            var rsrcSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rsrc"));
            if (rsrcSection != null)
            {
                int sectionAddr = (int)rsrcSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rsrcSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + e + (char)0x00 + d + (char)0x00 +   + (char)0x00 + M + (char)0x00 + o + (char)0x00 + d + (char)0x00 + u + (char)0x00 + l + (char)0x00 + e + (char)0x00
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x74, 0x00,
                            0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00,
                            0x64, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x6f, 0x00,
                            0x64, 0x00, 0x75, 0x00, 0x6c, 0x00, 0x65, 0x00
                        }, start: sectionAddr, end: sectionEnd),
                    "StarForce 5"),

                    // TODO: Confirm that these additonal checks do anything
                    // new ContentMatchSet(new List<ContentMatch>
                    // {
                    //     // P + (char)0x00 + r + (char)0x00 + o + (char)0x00 + t + (char)0x00 + e + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + T + (char)0x00 + e + (char)0x00 + c + (char)0x00 + h + (char)0x00 + n + (char)0x00 + o + (char)0x00 + l + (char)0x00 + o + (char)0x00 + g + (char)0x00 + y + (char)0x00
                    //     new ContentMatch(new byte?[]
                    //     {
                    //         0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00,
                    //         0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                    //         0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00,
                    //         0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00,
                    //         0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00,
                    //         0x79, 0x00
                    //     }, start: sectionAddr, end: sectionEnd),

                    //     // // PSA_GetDiscLabel
                    //     // new ContentMatch(new byte?[]
                    //     // {
                    //     //     0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44,
                    //     //     0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C
                    //     // }, start: sectionAddr, end: sectionEnd),

                    //     // (c) Protection Technology
                    //     // new ContentMatch(new byte?[]
                    //     // {
                    //     //     0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74,
                    //     //     0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54,
                    //     //     0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67,
                    //     //     0x79
                    //     // }, start: sectionAddr, end: sectionEnd),

                    //     // TradeName
                    //     new ContentMatch(new byte?[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 }, start: sectionAddr, end: sectionEnd),
                    // }, GetVersion, "StarForce"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .brick section, if it exists
            var brickSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".brick"));
            if (brickSection != null)
                return "StarForce 3-5";

            // Get the .sforce* section, if it exists
            var sforceSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".sforce"));
            if (sforceSection != null)
                return "StarForce 3-5";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("protect.dll", useEndsWith: true), "StarForce"),
                new PathMatchSet(new PathMatch("protect.exe", useEndsWith: true), "StarForce"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // These have too high of a chance of over-matching by themselves
            // var matchers = new List<PathMatchSet>
            // {
            //     // TODO: Re-consolidate these once path matching is improved
            //     new PathMatchSet(new PathMatch("/protect.dll", useEndsWith: true), "StarForce"),
            //     new PathMatchSet(new PathMatch("/protect.exe", useEndsWith: true), "StarForce"),
            // };

            // return MatchUtil.GetFirstMatch(path, matchers, any: true);
            return null;
        }
    
        // public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        // {
        //     return $"{Utilities.GetFileVersion(fileContent)} ({fileContent.Skip(positions[1] + 22).TakeWhile(c => c != 0x00)})";
        // }
    }
}
