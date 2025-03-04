using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Headers
{
    /// <summary>
    /// The MS-DOS EXE format, also known as MZ after its signature (the initials of Microsoft engineer Mark Zbykowski),
    /// was introduced with MS-DOS 2.0 (version 1.0 only sported the simple COM format). It is designed as a relocatable
    /// executable running under real mode. As such, only DOS and Windows 9x can use this format natively, but there are
    /// several free DOS emulators (e.g., DOSBox) that support it and that run under various operating systems (e.g.,
    /// Linux, Amiga, Windows NT, etc.). Although they can exist on their own, MZ executables are embedded in all NE, LE,
    /// and PE executables, usually as stubs so that when they are ran under DOS, they display a warning.
    /// </summary>
    /// <remarks>https://wiki.osdev.org/MZ</remarks>
    public class MSDOSExecutableHeader
    {
        #region Standard Fields

        /// <summary>
        /// 0x5A4D (ASCII for 'M' and 'Z') [00]
        /// </summary>
        public ushort Magic;
        
        /// <summary>
        /// Number of bytes in the last page. [02]
        /// </summary>
        public ushort LastPageBytes;
        
        /// <summary>
        /// Number of whole/partial pages. [04]
        /// </summary>
        public ushort Pages;
        
        /// <summary>
        /// Number of entries in the relocation table. [06]
        /// </summary>
        public ushort Relocations;
        
        /// <summary>
        /// The number of paragraphs taken up by the header.It can be any value, as the loader
        /// just uses it to find where the actual executable data starts. It may be larger than
        /// what the "standard" fields take up, and you may use it if you want to include your
        /// own header metadata, or put the relocation table there, or use it for any other purpose. [08]
        /// </summary>
        public ushort HeaderParagraphSize;
        
        /// <summary>
        /// The number of paragraphs required by the program, excluding the PSP and program image.
        /// If no free block is big enough, the loading stops. [0A]
        /// </summary>
        public ushort MinimumExtraParagraphs;
        
        /// <summary>
        /// The number of paragraphs requested by the program.
        /// If no free block is big enough, the biggest one possible is allocated. [0C]
        /// </summary>
        public ushort MaximumExtraParagraphs;
        
        /// <summary>
        /// Relocatable segment address for SS. [0E]
        /// </summary>
        public ushort InitialSSValue;
        
        /// <summary>
        /// Initial value for SP. [10]
        /// </summary>
        public ushort InitialSPValue;
        
        /// <summary>
        /// When added to the sum of all other words in the file, the result should be zero. [12]
        /// </summary>
        public ushort Checksum;
        
        /// <summary>
        /// Initial value for IP. [14]
        /// </summary>
        public ushort InitialIPValue;
        
        /// <summary>
        /// Relocatable segment address for CS. [16]
        /// </summary>
        public ushort InitialCSValue;
        
        /// <summary>
        /// The (absolute) offset to the relocation table. [18]
        /// </summary>
        public ushort RelocationTableAddr;
        
        /// <summary>
        /// Value used for overlay management.
        /// If zero, this is the main executable. [1A]
        /// </summary>
        public ushort OverlayNumber;

        #endregion

        #region PE Extensions
        
        /// <summary>
        /// Reserved words [1C]
        /// </summary>
        public ushort[] Reserved1;
        
        /// <summary>
        /// Defined by name but no other information is given; typically zeroes [24]
        /// </summary>
        public ushort OEMIdentifier;
        
        /// <summary>
        /// Defined by name but no other information is given; typically zeroes [26]
        /// </summary>
        public ushort OEMInformation;
        
        /// <summary>
        /// Reserved words [28]
        /// </summary>
        public ushort[] Reserved2;
        
        /// <summary>
        /// Starting address of the PE header [3C]
        /// </summary>
        public int NewExeHeaderAddr;

        #endregion

        /// <summary>
        /// All data after the last item in the header but before the new EXE header address
        /// </summary>
        public byte[] ExecutableData;

        public static MSDOSExecutableHeader Deserialize(Stream stream, bool asStub = true)
        {
            MSDOSExecutableHeader idh = new MSDOSExecutableHeader();

            idh.Magic = stream.ReadUInt16();
            idh.LastPageBytes = stream.ReadUInt16();
            idh.Pages = stream.ReadUInt16();
            idh.Relocations = stream.ReadUInt16();
            idh.HeaderParagraphSize = stream.ReadUInt16();
            idh.MinimumExtraParagraphs = stream.ReadUInt16();
            idh.MaximumExtraParagraphs = stream.ReadUInt16();
            idh.InitialSSValue = stream.ReadUInt16();
            idh.InitialSPValue = stream.ReadUInt16();
            idh.Checksum = stream.ReadUInt16();
            idh.InitialIPValue = stream.ReadUInt16();
            idh.InitialCSValue = stream.ReadUInt16();
            idh.RelocationTableAddr = stream.ReadUInt16();
            idh.OverlayNumber = stream.ReadUInt16();

            // If we're not reading as a stub, return now
            if (!asStub)
                return idh;

            idh.Reserved1 = new ushort[Constants.ERES1WDS];
            for (int i = 0; i < Constants.ERES1WDS; i++)
            {
                idh.Reserved1[i] = stream.ReadUInt16();
            }

            idh.OEMIdentifier = stream.ReadUInt16();
            idh.OEMInformation = stream.ReadUInt16();
            idh.Reserved2 = new ushort[Constants.ERES2WDS];
            for (int i = 0; i < Constants.ERES2WDS; i++)
            {
                idh.Reserved2[i] = stream.ReadUInt16();
            }

            idh.NewExeHeaderAddr = stream.ReadInt32();
            idh.ExecutableData = stream.ReadBytes(idh.NewExeHeaderAddr - (int)stream.Position);

            return idh;
        }

        public static MSDOSExecutableHeader Deserialize(byte[] content, ref int offset, bool asStub = true)
        {
            MSDOSExecutableHeader idh = new MSDOSExecutableHeader();

            idh.Magic = content.ReadUInt16(ref offset);
            idh.LastPageBytes = content.ReadUInt16(ref offset);
            idh.Pages = content.ReadUInt16(ref offset);
            idh.Relocations = content.ReadUInt16(ref offset);
            idh.HeaderParagraphSize = content.ReadUInt16(ref offset);
            idh.MinimumExtraParagraphs = content.ReadUInt16(ref offset);
            idh.MaximumExtraParagraphs = content.ReadUInt16(ref offset);
            idh.InitialSSValue = content.ReadUInt16(ref offset);
            idh.InitialSPValue = content.ReadUInt16(ref offset);
            idh.Checksum = content.ReadUInt16(ref offset);
            idh.InitialIPValue = content.ReadUInt16(ref offset);
            idh.InitialCSValue = content.ReadUInt16(ref offset);
            idh.RelocationTableAddr = content.ReadUInt16(ref offset);
            idh.OverlayNumber = content.ReadUInt16(ref offset);

            // If we're not reading as a stub, return now
            if (!asStub)
                return idh;

            idh.Reserved1 = new ushort[Constants.ERES1WDS];
            for (int i = 0; i < Constants.ERES1WDS; i++)
            {
                idh.Reserved1[i] = content.ReadUInt16(ref offset);
            }

            idh.OEMIdentifier = content.ReadUInt16(ref offset);
            idh.OEMInformation = content.ReadUInt16(ref offset);
            idh.Reserved2 = new ushort[Constants.ERES2WDS];
            for (int i = 0; i < Constants.ERES2WDS; i++)
            {
                idh.Reserved2[i] = content.ReadUInt16(ref offset);
            }

            idh.NewExeHeaderAddr = content.ReadInt32(ref offset);
            idh.ExecutableData = content.ReadBytes(ref offset, idh.NewExeHeaderAddr - offset);

            return idh;
        }
    }
}
