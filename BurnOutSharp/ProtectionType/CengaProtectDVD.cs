﻿using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // .cenega
                new Matcher(new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 }, "Cenega ProtectDVD"),
            };

            return Utilities.GetContentMatches(file, fileContent, matchers, includePosition);
        }
    }
}
