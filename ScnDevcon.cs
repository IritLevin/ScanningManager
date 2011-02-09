/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 22/03/09
 * Time: 11:20 AM
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
	/// This class replaces the computer device manager. 
	/// It allows us to get the connected devices and anable and disable them.
	/// </summary>
	public class ScnDevcon
	{
		public string ReturnedData=string.Empty;
		System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
		Thread processThread;
		//List<ImagingDevice> ImagingDevicesList;
		string InstanceID;
		//string ScannerName;
		
		
		public ScnDevcon()
		{
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.RedirectStandardOutput = true;
//			ImagingDevicesList = GetImagingDevices();
		}
		
		/// <summary>
		/// Gets connected imaging devices.
		/// </summary>
		/// <returns>A list of ImagingDevice</returns>
		public List<ImagingDevice> GetImagingDevices()
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
		/// Gets the connection status of the device
		/// </summary>
		/// <param name="_InstanceID">Device instance id</param>
		/// <returns>enum ConnectionStatus: Connected, Disconnected, Disabled</returns>
		public int GetDeviceStatus(string _InstanceID)
		{
			InstanceID = _InstanceID;
			processThread = new Thread(new ThreadStart(RunDevCon2FindDeviceStatus));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
			return AnalayzeDeviceStatus();
		}
		
		/// <summary>
		/// A thread that runs devcon and finds the status of a device
		/// </summary>
		private void RunDevCon2FindDeviceStatus()
		{
			string data;
			string allData= string.Empty ;
			
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.Arguments = @" status " + InstanceID;
			myProcess.Start();
	
			while ((data = myProcess.StandardOutput.ReadLine()) != null)
			{
				allData += ("\n" + data);
			}
			ReturnedData = allData;
		}
		
		/// <summary>
		/// Analyzes the returned data and determines the status
		/// </summary>
		/// <returns>enum ConnectionStatus: Connected, Disconnected, Disabled</returns>
		private int AnalayzeDeviceStatus()
		{
			int CnctStatus = (int)ConnectionStatus.Disconnected;
			int NOR =  ReturnedData.Split('\n').Length ;		// Number Of Rows
			string[] ReturnedDataArray = new string[NOR];
			
			ReturnedDataArray = ReturnedData.Split('\n');
			
			for (int i=0; i<NOR; i++)
			{
				if (ReturnedDataArray[i].IndexOf(@"Driver is running")>0)
				{
					CnctStatus = (int)ConnectionStatus.Connected;
				}				
				else if (ReturnedDataArray[i].IndexOf(@"Device is disabled")>0)
				{
					CnctStatus = (int)ConnectionStatus.Disabled;
				}
			}
			
			return CnctStatus;
		}
		
		/// <summary>
		/// Shutting off a specific scanner, by disbling the usb using devcon
		/// </summary>
		public void DevconDisable(string _InstanceID)
		{
			//ScannerName2InstanceId();
			InstanceID = _InstanceID;
			processThread = new Thread(new ThreadStart(RunDevCon2DisableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
			System.Threading.Thread.Sleep(1000);
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
		/// Turns on a specific scanner, by enbling the usb using devcon
		/// </summary>
		public void DevconEnable(string _InstanceID)
		{
			InstanceID = _InstanceID;
			processThread = new Thread(new ThreadStart(RunDevCon2EnableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
			System.Threading.Thread.Sleep(1000);
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
	}
}
