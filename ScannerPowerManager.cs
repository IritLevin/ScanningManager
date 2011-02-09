/*
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
//		public string ReturnedData=string.Empty;
//		System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
//		Thread processThread;
		List<ImagingDevice> ImagingDevicesList;
		ScnDevcon Devcon;
		string InstanceID;
		string ScannerName;
		int    PowerMethod;
		ScnMngrLog scnMngrLog;
		
//		private EventLog ScnrPwrMngrEventLog;

		
		/// <summary>
		/// Constructor. Gets Imaging Devices list.
		/// </summary>
		public ScannerPowerManager()
		{
//			ScnrPwrMngrEventLog = new EventLog();
//			ScnrPwrMngrEventLog.Log =  "Application";
//			ScnrPwrMngrEventLog.Source = "PowerManager";
//			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
//			myProcess.StartInfo.UseShellExecute = false;
//			myProcess.StartInfo.CreateNoWindow = true;
//			myProcess.StartInfo.RedirectStandardOutput = true;
			Devcon = new ScnDevcon();
			ImagingDevicesList = Devcon.GetImagingDevices();
			scnMngrLog = new ScnMngrLog();
			//ImagingDevicesList = GetImagingDevices();			
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
		
//		/// <summary>
//		/// Gets connected imaging devices.
//		/// </summary>
//		/// <returns>A list of ImagingDevice</returns>
//		private List<ImagingDevice> GetImagingDevices()
//		{
//			processThread = new Thread(new ThreadStart(RunDevCon2FindImagingDevice));
//			processThread.IsBackground = true;
//			processThread.Start();
//			processThread.Join();
//			return AnalayzeReturnedData();
//		}
		
//		/// <summary>
//		/// A thread that runs devcon and finds all connected imaging devices
//		/// </summary>
//		void RunDevCon2FindImagingDevice()
//		{
//			string data;
//			string allData= string.Empty ;
//			
//			myProcess.StartInfo.Arguments =  "hwids =Image";
//			myProcess.Start();
//			while ((data = myProcess.StandardOutput.ReadLine()) != null)
//			{
//				allData += ("\n" + data);
//				//System.Threading.Thread.Sleep(100);
//			}
//			ReturnedData = allData;
//		}
		
//		/// <summary>
//		/// Analyzes the text returned from devcon 
//		/// </summary>
//		/// <returns>A list of ImagingDevice</returns>
//		private List<ImagingDevice> AnalayzeReturnedData()
//		{
//			List<ImagingDevice> ImagingDeviceList = new List<ImagingDevice>();
//			int NOR =  ReturnedData.Split('\n').Length ;		// Number Of Rows
//			string[] ReturnedDataArray = new string[NOR];
//			ImagingDevice tmpImagingDevice;
//			
//			ReturnedDataArray = ReturnedData.Split('\n');
//			
//			for (int i=0; i<NOR; i++)
//			{
//				if (ReturnedDataArray[i].IndexOf(@"Name:")>0)
//				{
//					tmpImagingDevice.InstanceID = ReturnedDataArray[i-1].Trim();
//					tmpImagingDevice.Name = ReturnedDataArray[i].Trim().Remove(0,6);
//					ImagingDeviceList.Add(tmpImagingDevice);
//				}
//				
//			}
//			
//			return ImagingDeviceList;
//		}
		
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
					}
					else if (PowerMethod == (int)ScnPowerMethod.RELAY)
					{
						Devcon.DevconDisable(InstanceID);
						PowerOff();
					}	
					
					// Check if got disconnected
					if (Devcon.GetDeviceStatus(InstanceID)==(int)ConnectionStatus.Connected)
					{
						RT = false;		
						string errMsg = "Scanner "+ScannerName+" was not disconnected. \nTrial: " + trial.ToString();
						scnMngrLog.LogWarn(errMsg);
					}
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					RT = false;	
				}
				trial++;
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not turn off scanner " + ScannerName);
			}
		}
		
//		/// <summary>
//		/// Shutting off a specific scanner, by disbling the usb using devcon
//		/// </summary>
//		private void DevconDisable()
//		{
//			if (PowerMethod == "DEVCON")
//			{
//				ScannerName2InstanceId();
//				processThread = new Thread(new ThreadStart(RunDevCon2DisableImagingDevice));
//				processThread.IsBackground = true;
//				processThread.Start();
//				processThread.Join();
//			}
//		}
//		
//		/// <summary>
//		/// A thread that runs devcon and disables a scanner
//		/// </summary>
//		private void RunDevCon2DisableImagingDevice()
//		{
//			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
//			myProcess.StartInfo.Arguments = @" disable " + InstanceID;
//			myProcess.Start();
//		}
		
		
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
						string errMsg = "Scanner "+ScannerName+" was not connected. \nTrial: " + trial.ToString();
						scnMngrLog.LogWarn(errMsg);
					}					
				}
				catch(RelayException e)
				{			
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				}
				trial++;	
			}
			if (!RT)
			{
				throw new ScnrPwrMngrException("Can not connect scanner " + ScannerName);
			}
		}
		
//		/// <summary>
//		/// Turns on a specific scanner, by enbling the usb using devcon
//		/// </summary>
//		private void DevconEnable()
//		{
//			if (PowerMethod == "DEVCON")
//			{
//				ScannerName2InstanceId();
//				processThread = new Thread(new ThreadStart(RunDevCon2EnableImagingDevice));
//				processThread.IsBackground = true;
//				processThread.Start();
//				processThread.Join();
//			}
//		}
//		
//		/// <summary>
//		/// A thread that runs devcon and enables a scanner
//		/// </summary>
//		private void RunDevCon2EnableImagingDevice()
//		{
//			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
//			myProcess.StartInfo.Arguments = @" enable " + InstanceID;
//			myProcess.Start();
//		}
		
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
				if (_ImagingDevicesList[i].Name == ScannerName)
				{
					index = i;
				}
			}
			return index;
		}
		
/*		/// <summary>
		/// Checks if a certain scanner is connected
		/// </summary>
		/// <param name="_ScannerName">Name of scanner</param>
		/// <returns>Scanner is connected or not</returns>
		private bool ScannerExist()
		{
			bool scnrExist = false;
			int  ScannerIndex;
			List<ImagingDevice> currImagingDevicesList;
			
			currImagingDevicesList = Devcon.GetImagingDevices();
			ScannerIndex = FindImagingDevice(currImagingDevicesList);
			if (ScannerIndex!=-1)
			{
				scnrExist = true;
			}
			return scnrExist;
			
		}
*/
		
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
//			System.Diagnostics.Debug.WriteLine(UsbRelayHardware.ReadRelayStatus());

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
	
	public struct ImagingDevice
	{
		public string InstanceID;
		public string Name;
		
	}
	
}
