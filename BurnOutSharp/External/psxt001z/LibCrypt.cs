﻿using System;
using System.IO;

namespace BurnOutSharp.External.psxt001z
{
	/// <summary>
	/// LibCrypt detection code
	/// Originally written by Dremora: https://github.com/Dremora/psxt001z
	/// Ported and changed by darksabre76
	/// </summary>
	public class LibCrypt
    {
        public static bool CheckSubfile(string subFilePath)
        {
			// opening .sub
			FileStream subfile;
			try
			{
				subfile = File.OpenRead(subFilePath);
			}
			catch
			{
				Console.WriteLine($"Error opening {subFilePath}");
				return false;
			}

			// checking extension
			string ext = Path.GetExtension(subFilePath).TrimStart('.').ToLowerInvariant();
			if (ext != "sub")
			{
				Console.WriteLine($"{ext}: unknown file extension");
				return false;
			}
			
			// filesize
			long size = subfile.Length;
			if (size % 96 != 0)
			{
				Console.WriteLine($"{subFilePath}: wrong size");
				return false;
			}

			return CheckSubfile(subfile);
		}

		public static bool CheckSubfile(Stream subfile)
		{
			// Variables
			byte[] buffer = new byte[16];
			byte[] sub = new byte[16];
			int tpos = 0;
			uint sector, psectors = 0;
			long size = subfile.Length;

			for (sector = 150; sector < ((size / 96) + 150); sector++)
			{
				subfile.Seek(12, SeekOrigin.Current);
				if (subfile.Read(buffer, 0, 12) == 0)
					return true;

				subfile.Seek(72, SeekOrigin.Current);

				// New track
				if ((btoi(buffer[1]) == (btoi(sub[1]) + 1)) && (buffer[2] == 0 || buffer[2] == 1))
				{
					Array.Copy(buffer, sub, 6);
					tpos = ((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
				}

				// New index
				else if (btoi(buffer[2]) == (btoi(sub[2]) + 1) && buffer[1] == sub[1])
				{
					Array.Copy(buffer, 2, sub, 2, 4);
					tpos = ((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
				}

				// MSF1 [3-5]
				else
				{
					if (sub[2] == 0)
						tpos--;
					else
						tpos++;

					sub[3] = itob((byte)(tpos / 60 / 75));
					sub[4] = itob((byte)((tpos / 75) % 60));
					sub[5] = itob((byte)(tpos % 75));
				}

				// MSF2 [7-9]
				sub[7] = itob((byte)(sector / 60 / 75));
				sub[8] = itob((byte)((sector / 75) % 60));
				sub[9] = itob((byte)(sector % 75));

				// CRC-16 [10-11]
				ushort crc = CRC16.Calculate(sub, 0, 10);
				byte[] crcBytes = BitConverter.GetBytes(crc);
				sub[10] = crcBytes[0];
				sub[11] = crcBytes[1];

				// TODO: This *was* a memcmp, but that's harder to do. Fix this for C# later
				if (buffer[10] != sub[10] && buffer[11] != sub[11] && (buffer[3] != sub[3] || buffer[7] != sub[7] || buffer[4] != sub[4] || buffer[8] != sub[8] || buffer[5] != sub[5] || buffer[9] != sub[9]))
				{
					if (buffer[10] != sub[10] || buffer[11] != sub[11] || buffer[3] != sub[3] || buffer[7] != sub[7] || buffer[4] != sub[4] || buffer[8] != sub[8] || buffer[5] != sub[5] || buffer[9] != sub[9])
					{
						Console.Write($"MSF: {sub[7]:x}:{sub[8]:x}:{sub[9]:x} Q-Data: {buffer[0]:x}{buffer[1]:x}{buffer[2]:x} {buffer[3]:x}:{buffer[4]:x}:{buffer[5]:x} {buffer[6]:x} {buffer[7]:x}:{buffer[8]:x}:{buffer[9]:x} {buffer[10]:x}{buffer[11]:x}  xor {crc ^ ((buffer[10] << 8) + buffer[11]):x} % {CRC16.Calculate(buffer, 0, 10) ^ ((buffer[10] << 8) + buffer[11]):x}");
						//Console.Write("\nMSF: %02x:%02x:%02x Q-Data: %02x%02x%02x %02x:%02x:%02x %02x %02x:%02x:%02x %02x%02x", sub[7], sub[8], sub[9], sub[0], sub[1], sub[2], sub[3], sub[4], sub[5], sub[6], sub[7], sub[8], sub[9], sub[10], sub[11]);
						if (buffer[3] != sub[3] && buffer[7] != sub[7] && buffer[4] == sub[4] && buffer[8] == sub[8] && buffer[5] == sub[5] && buffer[9] == sub[9])
							Console.Write($" P1 xor {buffer[3] ^ sub[3]:x} {buffer[7] ^ sub[7]:x}");
						else if (buffer[3] == sub[3] && buffer[7] == sub[7] && buffer[4] != sub[4] && buffer[8] != sub[8] && buffer[5] == sub[5] && buffer[9] == sub[9])
							Console.Write($" P2 xor {buffer[4] ^ sub[4]:x} {buffer[8] ^ sub[8]:x}");
						else if (buffer[3] == sub[3] && buffer[7] == sub[7] && buffer[4] == sub[4] && buffer[8] == sub[8] && buffer[5] != sub[5] && buffer[9] != sub[9])
							Console.Write($" P3 xor {buffer[5] ^ sub[5]:x} {buffer[9] ^ sub[9]:x}");
						else
							Console.Write(" ?");

						Console.Write("\n");
						psectors++;
					}
				}
			}

			Console.WriteLine($"Number of modified sectors: {psectors}");
			return true;
		}

		private static byte btoi(byte b)
		{
			/* BCD to u_char */
			return (byte)((b) / 16 * 10 + (b) % 16);
		}

		private static byte itob(byte i)
		{
			/* u_char to BCD */
			return (byte)((i) / 10 * 16 + (i) % 10);
		}
	}
}
