﻿/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 12-Feb-08
 * Time: 6:41 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Threading ;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace ScaningManager
{
	/// <summary>
	/// Controls the scanners connected
	/// </summary>
	/// 
	public class ScannerPowerManager
	{
		public string ReturnedData=string.Empty;
		System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
		Thread processThread;
		List<ImagingDevice> ImagingDevicesList;
		string InstanceID;
		
		private EventLog ScnrPwrMngrEventLog;

		
		/// <summary>
		/// Constructor. Gets Imaging Devices list.
		/// </summary>
		public ScannerPowerManager()
		{
			ScnrPwrMngrEventLog = new EventLog();
			ScnrPwrMngrEventLog.Log =  "Application";
			ScnrPwrMngrEventLog.Source = "PowerManager";
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.RedirectStandardOutput = true;
			ImagingDevicesList = GetImagingDevices();
			
		}
		
		/// <summary>
		/// Gets connected imaging devices.
		/// </summary>
		/// <returns>A list of ImagingDevice</returns>
		private List<ImagingDevice> GetImagingDevices()
		{
			processThread = new Thread(new ThreadStart(RunDevCon2FindImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
			return AnalayzeReturnedData();
		}
		
		/// <summary>
		/// A thread that runs devcon and finds all connected imaging devices
		/// </summary>
		void RunDevCon2FindImagingDevice()
		{
			string data;
			string allData= string.Empty ;
			
			myProcess.StartInfo.Arguments =  "hwids =Image";
			myProcess.Start();
			while ((data = myProcess.StandardOutput.ReadLine()) != null)
			{
				allData += ("\n" + data);
				//System.Threading.Thread.Sleep(100);
			}
			ReturnedData = allData;
		}
		
		/// <summary>
		/// Analyzes the text returned from devcon 
		/// </summary>
		/// <returns>A list of ImagingDevice</returns>
		private List<ImagingDevice> AnalayzeReturnedData()
		{
			List<ImagingDevice> ImagingDeviceList = new List<ImagingDevice>();
			int NOR =  ReturnedData.Split('\n').Length ;		// Number Of Rows
			string[] ReturnedDataArray = new string[NOR];
			ImagingDevice tmpImagingDevice;
			
			ReturnedDataArray = ReturnedData.Split('\n');
			
			for (int i=0; i<NOR; i++)
			{
				if (ReturnedDataArray[i].IndexOf(@"Name:")>0)
				{
					tmpImagingDevice.InstanceID = ReturnedDataArray[i-1].Trim();
					tmpImagingDevice.Name = ReturnedDataArray[i].Trim().Remove(0,6);
					ImagingDeviceList.Add(tmpImagingDevice);
				}
				
			}
			
			return ImagingDeviceList;
		}
		
		/// <summary>
		/// Turns off a specific scanner, by disabling the usb or by relay
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		public void DisableScanner(string _ScannerName)
		{
			int trial = 1;
			int MaxTrials = 3;
			bool RT = false;
			
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					DevconDisable(_ScannerName);					
					PowerOff(_ScannerName);
					RT = true;
					if (ScannerExist(_ScannerName))
					{
						RT = false;			
					}					
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					ScnrPwrMngrEventLog.WriteEntry(errMsg, EventLogEntryType.Error);						
				}
				trial++;
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not disable scanner " + _ScannerName);
			}
		}
		
		/// <summary>
		/// Shutting off a specific scanner, by disbling the usb using devcon
		/// </summary>
		public void DevconDisable(string _ScannerName)
		{
			ScannerName2InstanceId(_ScannerName);
			processThread = new Thread(new ThreadStart(RunDevCon2DisableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
		}
		
		/// <summary>
		/// A thread that runs devcon and disables a scanner
		/// </summary>
		private void RunDevCon2DisableImagingDevice()
		{
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.Arguments = @" disable " + InstanceID;
			myProcess.Start();
		}
		
		
		/// <summary>
		/// Turns on a specific scanner, by enbling the usb or by relay
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		public void EnableScanner(string _ScannerName)
		{
			int trial = 1;
			int MaxTrials = 3;
			bool RT = false;
			
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					PowerOn(_ScannerName);					
					DevconEnable(_ScannerName);	
					RT = true;
					if (!ScannerExist(_ScannerName))
					{
						RT = false;
					}
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					ScnrPwrMngrEventLog.WriteEntry(errMsg, EventLogEntryType.Error);
				}
				trial++;	
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not enable scanner " + _ScannerName);
			}
		}
		
		/// <summary>
		/// Turns on a specific scanner, by enbling the usb using devcon
		/// </summary>
		public void DevconEnable(string _ScannerName)
		{
			ScannerName2InstanceId(_ScannerName);
			processThread = new Thread(new ThreadStart(RunDevCon2EnableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();	
		}
		
		/// <summary>
		/// A thread that runs devcon and enables a scanner
		/// </summary>
		private void RunDevCon2EnableImagingDevice()
		{
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.Arguments = @" enable " + InstanceID;
			myProcess.Start();
		}
		
		/// <summary>
		/// Initializes InstanceId of a specific scanner
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		private void ScannerName2InstanceId(string _ScannerName)
		{
			for (int i=0; i<ImagingDevicesList.Count; i++)
			{
				if (ImagingDevicesList[i].Name == _ScannerName)
				{
					InstanceID = @"@" + ImagingDevicesList[i].InstanceID;
				}
			}
		}
		
		/// <summary>
		/// Getting the index of a scanner from a ImagingDevice list
		/// </summary>
		/// <param name="_ImagingDevicesList">List of imaging devices</param>
		/// <param name="_ScannerName">name of a scanner</param>
		/// <returns>The index of the scanner in the list</returns>
		private int FindImagingDevice(List<ImagingDevice> _ImagingDevicesList, string _ScannerName)
		{
			int index=-1;
			
			for (int i=0; i<_ImagingDevicesList.Count; i++)
			{
				if (_ImagingDevicesList[i].Name == _ScannerName)
				{
					index = i;
				}
			}
			return index;
		}
		
		/// <summary>
		/// Checks if a certain scanner is connected
		/// </summary>
		/// <param name="_ScannerName">Name of scanner</param>
		/// <returns>Scanner is connected or not</returns>
		private bool ScannerExist(string _ScannerName)
		{
			bool scnrExist = false;
			int  ScannerIndex;
			List<ImagingDevice> currImagingDevicesList;
			
			currImagingDevicesList = GetImagingDevices();
			ScannerIndex = FindImagingDevice(currImagingDevicesList, _ScannerName);
			if (ScannerIndex!=-1)
			{
				scnrExist = true;
			}
			return scnrExist;
			
		}
		
		/// <summary>
		/// Gets the relay name and the port of a specific scanner
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		/// <returns>ScannerPowerPort</returns>
		private ScannerPowerPort GetScannerPortRelay(string _ScannerName)
		{
			
			string[] ScannerNames = ConfigurationManager.AppSettings["ScannerNames"].Split(',');
			string[] strScannerPowerPorts = ConfigurationManager.AppSettings["ScannerPowerPorts"].Split(',');
			string[] ScannerRelay = ConfigurationManager.AppSettings["ScannerRelay"].Split(',');
						
			
			System.Collections.ArrayList ScannerNamesList = new ArrayList();
			ScannerNamesList.AddRange(ScannerNames);
			int ind = ScannerNamesList.IndexOf(_ScannerName);
			return new ScannerPowerPort(new UsbRelay(ScannerRelay[ind]),Convert.ToInt32(strScannerPowerPorts[ind]));
			
		}
		
		
		
		/// <summary>
		/// Shutting off the power of the scanner with the relay
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		public void PowerOff(string _ScannerName)
		{
			
			
			ScannerPowerPort ScannerPowerPort = GetScannerPortRelay(_ScannerName);
			UsbRelay UsbRelayHardware = ScannerPowerPort.UsbRelayHardware;
			string CloseCommand = "sk" + ScannerPowerPort.PortId.ToString();
			string OpenCommand = "rk" + ScannerPowerPort.PortId.ToString();
			
			
			UsbRelayHardware.SendRelayCommand(OpenCommand);
			UsbRelayHardware.SendRelayCommand(CloseCommand);
			System.Threading.Thread.Sleep(5000);
			UsbRelayHardware.SendRelayCommand(OpenCommand);
			System.Threading.Thread.Sleep(5000);	
			
		}

		
		
		/// <summary>
		/// Turning on the power of the scanner with the relay
		/// </summary>
		/// <param name="_ScannerName">The name of the scanner as it appears in the scanners list</param>
		public void PowerOn(string _ScannerName)
		{

			ScannerPowerPort ScannerPowerPort = GetScannerPortRelay(_ScannerName);
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
	
	
	
	public struct ImagingDevice
	{
		public string InstanceID;
		public string Name;
		
	}
}
