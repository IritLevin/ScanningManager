/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 02-Dec-08
 * Time: 2:20 PM
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
