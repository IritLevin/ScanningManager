/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 03-Dec-08
 * Time: 12:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

// ADU needs to interoperate with the legacy DLL
using System.Runtime.InteropServices;
using System.Text;

namespace ScaningManager
{
	/// <summary>
	/// Description of UsbRelay.
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
