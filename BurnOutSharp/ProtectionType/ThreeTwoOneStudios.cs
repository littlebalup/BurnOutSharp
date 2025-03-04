﻿using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class ThreeTwoOneStudios : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            var resource = Utilities.FindResourceInSection(pex.ResourceSection, dataContains: "3\02\01\0S\0t\0u\0d\0i\0o\0s\0 \0A\0c\0t\0i\0v\0a\0t\0i\0o\0n\0");
            if (resource != null)
                return $"321Studios Online Activation";

            return null;
        }
    }
}
