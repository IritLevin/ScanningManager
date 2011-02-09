/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 02-Dec-08
 * Time: 2:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace ScanningManager
{
	/// <summary>
	/// ADU USB Relay
	/// </summary>
	public struct ScannerPowerPort 
	{
		/// <summary>
		/// HardwareId is writen on the device
		/// </summary>
		public UsbRelay UsbRelayHardware;
		/// <summary>
		/// The port where the scanner is connected
		/// </summary>
		public int PortId;
		
		/// <summary>
		/// ScannerPowerPort constructor
		/// </summary>
		/// <param name="_UsbRelayHardwareId">Hardware ID</param>
		/// <param name="_PortId">Port number</param>
		public ScannerPowerPort(UsbRelay _UsbRelayHardware,int _PortId)
		{
			UsbRelayHardware=_UsbRelayHardware;
			PortId=_PortId;
		}
		
	}
}
