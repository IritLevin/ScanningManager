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


namespace ScaningManager
{
	/// <summary>
	/// Description of ScannerPowerManager.
	/// </summary>
	/// 
	public class ScannerPowerManager
	{
		public string ReturnedData=string.Empty;
		System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
		Thread processThread;
		List<ImagingDevice> ImagingDevicesList;
		string InstanceID;
		
		public ScannerPowerManager()
		{

			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.RedirectStandardOutput = true;
			ImagingDevicesList = GetImagingDevices();
		}
		
		
		private List<ImagingDevice> GetImagingDevices()
		{
			processThread = new Thread(new ThreadStart(RunDevCon2FindImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
			return AnalayzeReturnedData();
		}
		
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
		
		public void DisableScanner(string _ScannerName)
		{
			ScannerName2InstanceId(_ScannerName);
			processThread = new Thread(new ThreadStart(RunDevCon2DisableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
		}
		
		private void RunDevCon2DisableImagingDevice()
		{
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.Arguments = @" disable " + InstanceID;
			myProcess.Start();
		}
		
		public void EnableScanner(string _ScannerName)
		{
			ScannerName2InstanceId(_ScannerName);
			processThread = new Thread(new ThreadStart(RunDevCon2EnableImagingDevice));
			processThread.IsBackground = true;
			processThread.Start();
			processThread.Join();
		}
		
		private void RunDevCon2EnableImagingDevice()
		{
			myProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["DevconPath"];
			myProcess.StartInfo.Arguments = @" enable " + InstanceID;
			myProcess.Start();
		}
		
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
	}
	
	
	
	public struct ImagingDevice
	{
		public string InstanceID;
		public string Name;
		
	}
}
