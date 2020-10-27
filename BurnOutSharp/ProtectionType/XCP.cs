﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.FileType;

namespace BurnOutSharp.ProtectionType
{
    public class XCP
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // XCP.DAT
            byte[] check = new byte[] { 0x58, 0x43, 0x50, 0x2E, 0x44, 0x41, 0x54 };
            if (fileContent.Contains(check, out int position))
                return "XCP" + (includePosition ? $" (Index {position})" : string.Empty);
        
            // XCPPlugins.dll
            check = new byte[] { 0x58, 0x43, 0x50, 0x50, 0x6C, 0x75, 0x67, 0x69, 0x6E, 0x73, 0x2E, 0x64, 0x6C, 0x6C };
            if (fileContent.Contains(check, out position))
                return "XCP" + (includePosition ? $" (Index {position})" : string.Empty);
            
            // XCPPhoenix.dll
            check = new byte[] { 0x58, 0x43, 0x50, 0x50, 0x68, 0x6F, 0x65, 0x6E, 0x69, 0x78, 0x2E, 0x64, 0x6C, 0x6C };
            if (fileContent.Contains(check, out position))
                return "XCP" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // INI-like file that can be parsed out
                string xcpDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(xcpDatPath))
                {
                    string xcpVersion = GetXCPVersion(xcpDatPath);
                    if (!string.IsNullOrWhiteSpace(xcpVersion))
                        return xcpVersion;
                }

                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("go.exe", StringComparison.OrdinalIgnoreCase))) // Path.Combine("contents", "go.exe")
                {
                    return "XCP";
                }
            }
            else
            {
                // INI-like file that can be parsed out
                if (Path.GetFileName(path).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase))
                {
                    string xcpVersion = GetXCPVersion(path);
                    if (!string.IsNullOrWhiteSpace(xcpVersion))
                        return xcpVersion;
                }

                if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("go.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "XCP";
                }
            }

            return null;
        }

        private static string GetXCPVersion(string path)
        {
            try
            {
                var xcpIni = new IniFile(path);
                return xcpIni["XCP.VERSION"];
            }
            catch
            {
               return null;
            }
        }
    }
}
