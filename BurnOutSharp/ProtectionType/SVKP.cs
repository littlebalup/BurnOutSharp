﻿using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how versions/version ranges work for this protection
    public class SVKProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the image file header from the executable, if possible
            if (pex?.ImageFileHeader == null)
                return null;
            
            // 0x504B5653 is "SVKP"
            if (pex.ImageFileHeader.PointerToSymbolTable == 0x504B5653)
                return "SVKP (Slovak Protector)";

            return null;
        }
    }
}
