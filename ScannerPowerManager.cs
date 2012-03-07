/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 12-Feb-08
 * Time: 6:41 PM
 * 
 */

/* Bug Fix:
 * --------
 * 10.03.11 Irit Levin-Reisman: Checking the status of connection after disabling for each method
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
using System.Threading ;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace ScanningManager
{
	/// <summary>
	/// Controls the scanners connected
	/// </summary>
	/// 
	public class ScannerPowerManager
	{
		List<ImagingDevice> ImagingDevicesList;
		ScnDevcon Devcon;
		string InstanceID;
		string ScannerName;
		int    PowerMethod;
		ScnMngrLog scnMngrLog;
		
		
		/// <summary>
		/// Constructor. Gets Imaging Devices list.
		/// </summary>
		public ScannerPowerManager()
		{
			Devcon = new ScnDevcon();
			ImagingDevicesList = Devcon.GetImagingDevices();
			scnMngrLog = new ScnMngrLog();	
		}

		/// <summary>
		/// Constructor. Gets Imaging Devices list.
		/// </summary>
		/// <param name="_LogFileName">Log file name</param>
		public ScannerPowerManager(string _LogFileName)
		{
			Devcon = new ScnDevcon();
			ImagingDevicesList = Devcon.GetImagingDevices();
			scnMngrLog = new ScnMngrLog(_LogFileName);	
		}
		
		/// <summary>
		/// Initializes Scanner Power Manager parameters
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		public void InitScannerPowerManager(string _ScannerName)
		{
			ScannerName = _ScannerName;
			ScannerName2InstanceId();
			GetPowerMethod();
		}
		
		
		/// <summary>
		/// Turns off a specific scanner, by disabling the usb or by relay
		/// </summary>
		public void DisableScanner()
		{
			int trial = 1;
			int MaxTrials = 3;
			bool RT = false;
			
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					RT = true;
					
					// Disconnect
					if (PowerMethod == (int)ScnPowerMethod.DEVCON)
					{
						Devcon.DevconDisable(InstanceID);
						System.Threading.Thread.Sleep(500);
						if (Devcon.GetDeviceStatus(InstanceID)!=(int)ConnectionStatus.Disabled)
						{
							RT = false;
						}
					}
					else if (PowerMethod == (int)ScnPowerMethod.RELAY)
					{
						Devcon.DevconDisable(InstanceID);
						PowerOff();
						System.Threading.Thread.Sleep(500);
						if (Devcon.GetDeviceStatus(InstanceID)!=(int)ConnectionStatus.Disconnected)
						{
							RT = false;
						}
					}	
					
					// Check if got disconnected
					if (RT == false)
					{	
						string errMsg = "Scanner "+ScannerName+" was not disconnected. Trial: " + trial.ToString();
						scnMngrLog.LogWarn(errMsg);
						System.Threading.Thread.Sleep(1000);
					}
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + Environment.NewLine + "Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					RT = false;	
					System.Threading.Thread.Sleep(1000);
				}
				trial++;
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not turn off scanner " + ScannerName);
			}
		}
		
		
		/// <summary>
		/// Turns on a specific scanner, by enbling the usb or by relay
		/// </summary>
		public void EnableScanner()
		{
			int trial = 1;
			int MaxTrials = 3;
			bool RT = false;
			
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					RT = true;
					 
					// Connect
					if (PowerMethod == (int)ScnPowerMethod.DEVCON)
					{
						Devcon.DevconEnable(InstanceID);						
					}
					else if (PowerMethod == (int)ScnPowerMethod.RELAY)
					{
						Devcon.DevconEnable(InstanceID);
						PowerOn();
					}	
					
					// Check if got connected
					if (Devcon.GetDeviceStatus(InstanceID)!=(int)ConnectionStatus.Connected)
					{
						RT = false;		
						string errMsg = "Scanner "+ScannerName+" was not connected. Trial: " + trial.ToString();
						scnMngrLog.LogWarn(errMsg);
						System.Threading.Thread.Sleep(1000);
					}					
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + Environment.NewLine + "Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					RT = false;
					System.Threading.Thread.Sleep(1000);
				}
				trial++;	
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not connect scanner " + ScannerName);
			}
		}
		
		
		/// <summary>
		/// Initializes InstanceId of a specific scanner
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		private void ScannerName2InstanceId()
		{
			int i = FindImagingDevice(ImagingDevicesList);
			if (i != -1)
			{
				InstanceID = @"@" + ImagingDevicesList[i].InstanceID;
			}
			else
			{
				throw new ScnrPwrMngrException("Can not find instance id " + ScannerName);
			}
			
//			for (int i=0; i<ImagingDevicesList.Count; i++)
//			{
//				if (ImagingDevicesList[i].Name == ScannerName)
//				{
//					InstanceID = @"@" + ImagingDevicesList[i].InstanceID;
//				}
//			}
		}
		
		/// <summary>
		/// Getting the index of a scanner from a ImagingDevice list
		/// </summary>
		/// <param name="_ImagingDevicesList">List of imaging devices</param>
		/// <returns>The index of the scanner in the list. Return -1 if scanner is not on the list.</returns>
		private int FindImagingDevice(List<ImagingDevice> _ImagingDevicesList)
		{
			int index=-1;
			
			for (int i=0; i<_ImagingDevicesList.Count; i++)
			{
				
				if (ScannerName.IndexOf(_ImagingDevicesList[i].Name) !=-1)
				{
					index = i;
				}


//				if (_ImagingDevicesList[i].Name == ScannerName)
//				{
//					index = i;
//				}
			}
			return index;
		}
		
		/// <summary>
		/// returns Imaging Devices List
		/// </summary>
		/// <returns>ImagingDevices List</returns>
		public List<ImagingDevice> GetImagingDevicesList()
		{
			return ImagingDevicesList;
		}
			
		/// <summary>
		/// Gets the relay name and the port of a specific scanner
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		/// <returns>ScannerPowerPort</returns>
		private ScannerPowerPort GetScannerPortRelay()
		{
			
			string[] ScannerNames = ConfigurationManager.AppSettings["ScannerNames"].Split(',');
			string[] strScannerPowerPorts = ConfigurationManager.AppSettings["ScannerPowerPorts"].Split(',');
			string[] ScannerRelay = ConfigurationManager.AppSettings["ScannerRelay"].Split(',');
						
			
			System.Collections.ArrayList ScannerNamesList = new ArrayList();
			ScannerNamesList.AddRange(ScannerNames);
			int ind = ScannerNamesList.IndexOf(ScannerName);
			return new ScannerPowerPort(new UsbRelay(ScannerRelay[ind]),Convert.ToInt32(strScannerPowerPorts[ind]));
			
		}
		
		/// <summary>
		/// Initializing the power managment method
		/// </summary>
		private void GetPowerMethod()
		{
			string[] ScannerNames = ConfigurationManager.AppSettings["ScannerNames"].Split(',');
			string[] ScannerRelay = ConfigurationManager.AppSettings["ScannerRelay"].Split(',');
			
			System.Collections.ArrayList ScannerNamesList = new ArrayList();
			ScannerNamesList.AddRange(ScannerNames);
			int ind = ScannerNamesList.IndexOf(ScannerName);
			string Method  = ScannerRelay[ind].ToUpper();
			
			if (Method == "NONE")
			{
				PowerMethod = (int)ScnPowerMethod.NONE;
			}
			else if (Method == "DEVCON")
			{
				PowerMethod = (int)ScnPowerMethod.DEVCON;
			}
			else
			{
				PowerMethod = (int)ScnPowerMethod.RELAY;
			}
		}
		
		
		
		/// <summary>
		/// Shutting off the power of the scanner with the relay
		/// </summary>
		private void PowerOff()
		{
			ScannerPowerPort ScannerPowerPort = GetScannerPortRelay();
			UsbRelay UsbRelayHardware = ScannerPowerPort.UsbRelayHardware;
			string CloseCommand = "sk" + ScannerPowerPort.PortId.ToString();
			string OpenCommand = "rk" + ScannerPowerPort.PortId.ToString();
			
			UsbRelayHardware.SendRelayCommand(OpenCommand);

			System.Threading.Thread.Sleep(500);
			UsbRelayHardware.SendRelayCommand(CloseCommand);
			
			System.Threading.Thread.Sleep(5000);
			UsbRelayHardware.SendRelayCommand(OpenCommand);
			
			System.Threading.Thread.Sleep(5000);

		}

		
		
		/// <summary>
		/// Turns on the power of the scanner with the relay
		/// </summary>
		private void PowerOn()
		{		
			ScannerPowerPort ScannerPowerPort = GetScannerPortRelay();
			UsbRelay UsbRelayHardware = ScannerPowerPort.UsbRelayHardware;
			string CloseCommand = "sk" + ScannerPowerPort.PortId.ToString();
			string OpenCommand = "rk" + ScannerPowerPort.PortId.ToString();
			
			UsbRelayHardware.SendRelayCommand(OpenCommand);
			UsbRelayHardware.SendRelayCommand(CloseCommand);
			System.Threading.Thread.Sleep(500);
			UsbRelayHardware.SendRelayCommand(OpenCommand);
			System.Threading.Thread.Sleep(28000);	
		}
		
		
	}
	
	
	enum ScnPowerMethod {NONE, DEVCON, RELAY};
	enum ConnectionStatus {Connected, Disconnected, Disabled};
	
	/// <summary>
	/// Scanner name and instance ID
	/// </summary>
	public struct ImagingDevice
	{
		public string InstanceID;
		public string Name;
		
	}
	
}
