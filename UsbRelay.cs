/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 03-Dec-08
 * Time: 12:22 PM
 * 
 */


// ScanningManager controls an array of scanners for time lapsed serial scanning.
// Copyright 2010 Irit Levin Reisman published under GPLv3,
// this software was developed in Prof. Nathalie Q. Balaban's lab, at the Hebrew University , Jerusalem , Israel .
// 
//	  This file is part of ScanningManager.
//
//    ScanningManager is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    ScanningManager is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with ScanningManager.  If not, see <http://www.gnu.org/licenses/>.

using System;

// ADU needs to interoperate with the legacy DLL
using System.Runtime.InteropServices;
using System.Text;

namespace ScanningManager
{
	/// <summary>
	/// Interacts with the ADU USB Relay
	/// </summary>
	public class UsbRelay
	{
		[DllImport("aduhid.dll")]
		public static extern IntPtr OpenAduDevice(UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern IntPtr OpenAduDeviceBySerialNumber(string psSerialNumber,UInt32 iTimeout);
		
		[DllImport("aduhid.dll")]
		public static extern bool WriteAduDevice(IntPtr hFile,
		                                         [MarshalAs (UnmanagedType.LPStr)]string lpBuffer,
		                                         UInt32 nNumberOfBytesToWrite,
		                                         out UInt32 lpNumberOfBytesWritten,
		                                         UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern bool ReadAduDevice(IntPtr hFile,
		                                        StringBuilder lpBuffer,
		                                        UInt32 nNumberOfBytesToRead,
		                                        out UInt32 lpNumberOfBytesRead,
		                                        UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern void CloseAduDevice(IntPtr hFile);
		
		private IntPtr hAdu;
		private string UsbRelayHardwareId;
		
		/// <summary>
		/// Constructor. Opens Relay
		/// </summary>
		/// <param name="_UsbRelayHardwareId">Relay hardware ID</param>
		public UsbRelay(string _UsbRelayHardwareId)
		{
			UsbRelayHardwareId = _UsbRelayHardwareId;
			hAdu = OpenAduDeviceBySerialNumber(UsbRelayHardwareId,500);
			if (hAdu.ToInt32()<0)
			{
				string errMsg = "Unable to open relay " + UsbRelayHardwareId;
				throw new RelayException(errMsg);
			}
		}
		
		/// <summary>
		/// Destructor. Closes relay.
		/// </summary>
		/// <param name="Command"></param>
		~UsbRelay()
		{
			CloseAduDevice(hAdu);
		}
		
		/// <summary>
		/// Gives a command to the relay
		/// </summary>
		/// <param name="Command">command</param>
		public void SendRelayCommand(string Command)
		{
			bool bRC = false;
			uint uiWritten = 0xdead;
			uint uiLength = (uint)Command.Length;

			bRC = WriteAduDevice(hAdu, Command, uiLength, out uiWritten, 500);
			if (!bRC)
			{
				string errMsg = "Command: " + Command + " on relay " + UsbRelayHardwareId + " failed";
				throw new RelayException(errMsg);
			}
			
		}
		
		/// <summary>
		/// Reads the status of all ports
		/// </summary>
		/// <returns>port 4321 0 open 1 closed</returns>
		public string ReadRelayStatus()
		{
			bool bRC = false;
			uint uiRead = 0;
			uint uiLength = 7;
			string isClosed;
			
			StringBuilder sBuffer = new StringBuilder(8);
 
			SendRelayCommand("rpk");
			
			bRC = ReadAduDevice(hAdu, sBuffer, uiLength, out uiRead, 500);		
			if (!bRC)
			{
				string errMsg = "Reading from relay " + UsbRelayHardwareId + " faild";
				throw new RelayException(errMsg);
			}
			isClosed = sBuffer.ToString();
			return isClosed;
		}		

	}
}
