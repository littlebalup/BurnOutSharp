using System;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(pex)}";
            }

            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(pex)}";
            }

            return null;
        }
    }
}
