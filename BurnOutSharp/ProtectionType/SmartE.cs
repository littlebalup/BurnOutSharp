﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // BITARTS
                new Matcher(new byte?[] { 0x42, 0x49, 0x54, 0x41, 0x52, 0x54, 0x53 }, "SmartE"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("00001.TMP", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("00002.TMP", StringComparison.OrdinalIgnoreCase)))
            {
                return "SmartE";
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("00001.TMP", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("00002.TMP", StringComparison.OrdinalIgnoreCase))
            {
                return "SmartE";
            }

            return null;
        }
    }
}
