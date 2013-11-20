/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 1/29/2008
 * Time: 1:33 PM
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using WIA;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

namespace ScanningManager
{
	/// <summary>
	/// Controls the scanning process
	/// </summary>
	public class ScannerControl
	{
		private ScannerPowerManager  objScannerPowerManager;
		private Item wiaItem;
		
		private Device Scanner;
		private string ScannerName;
		private DeviceInfo myDeviceInfo;
		private int ScanningDPI;
		Bitmap myBitmap = null;
		private ScnMngrLog scnMngrLog;
		
		/// <summary>
		/// Constructor. 
		/// </summary>
		public ScannerControl()
		{
			objScannerPowerManager = new ScannerPowerManager();
			scnMngrLog = new ScnMngrLog();
			DisableAllScanners();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="_LogFileName">Log file name</param>
		public ScannerControl(string _LogFileName)
		{
			objScannerPowerManager = new ScannerPowerManager(_LogFileName);
			scnMngrLog = new ScnMngrLog(_LogFileName);
		}
				
		#region scanners list handling
		
		/// <summary>
		/// Gets a list of scanners
		/// </summary>
		/// <returns>Connected scanners (DeviceInfos)</returns>
		public  DeviceInfos  GetConnectedDevices()
		{
			DeviceManager DeviceManager = new DeviceManager();
			DeviceInfos DeviceInfoCollection = DeviceManager.DeviceInfos;
			return DeviceInfoCollection ;			
		}
		
		/// <summary>
		/// Gets a list of scanners
		/// </summary>
		/// <returns>Connected scanners (ImagingDevice List)</returns>
		public List<ImagingDevice> GetImagingDevicesList()
		{
			return objScannerPowerManager.GetImagingDevicesList();
		}
		
		/// <summary>
		/// Picks a scanner from the connected scanners list
		/// </summary>
		/// <param name="_myDeviceInfo">selected scanner</param>
		public void SelectDevice(object _myDeviceInfo)
		{
			myDeviceInfo = (DeviceInfo)_myDeviceInfo;
		}
		
		#endregion
		
		#region scanner handling
	
		/// <summary>
		/// Connects to a specific scanner and initializes parameters
		/// </summary>
		/// <param name="_myDeviceInfo">selected scanner</param>
		private void InitScanner(DeviceInfo _myDeviceInfo)
		{	
			object np = "Name";
			ScannerName = (string)_myDeviceInfo.Properties.get_Item(ref np).get_Value();
			objScannerPowerManager.InitScannerPowerManager(ScannerName);
			Enable();
			Scanner = _myDeviceInfo.Connect();
			wiaItem = Scanner.Items[1];	
			InitScannerProperties();
		}
		
		// updated 4.12 - Irit L.Reisman

		private void InitScanner()
		{	
			objScannerPowerManager.InitScannerPowerManager(ScannerName);
			Enable();
			DeviceInfos DeviceInfosList = GetConnectedDevices();
			if (DeviceInfosList.Count>1)
			{
				DisableAllScanners();
				throw new ScnCtrlException("More than one scanner are connected");
			}
			object ind = 1;
			myDeviceInfo = DeviceInfosList.get_Item(ref ind);
			
			Scanner = myDeviceInfo.Connect();
			wiaItem = Scanner.Items[1];	
			InitScannerProperties();
		}
		// end update 4.12
		
		/// <summary>
		/// Setting the properties mentioned in the configuration file
		/// </summary>
		private void InitScannerProperties()
		{
			string[] ScnrPropsNames = ConfigurationManager.AppSettings["ScnrPropsNames"].Split(',');
			string[] ScnrPropsVals = ConfigurationManager.AppSettings["ScnrPropsVals"].Split(',');
			object ScnrProperty;
			object PropVal;
				
			
			if (ScnrPropsNames.Length > ScnrPropsVals.Length)
			{
				throw new ScnCtrlException("Not enough ScnrPropsVals to initialize ScnrPropsNames");
			}
			
			for(int j = 0; j < ScnrPropsNames.Length; ++j)
			{
				ScnrProperty = ScnrPropsNames[j];
				PropVal      = Convert.ToInt32(ScnrPropsVals[j]);
				Properties Prop = wiaItem.Properties;
		
				((WIA.Property)Prop.get_Item(ref ScnrProperty)).set_Value(ref PropVal);						                
			}
		}
		
		/// <summary>
		/// Takes a picture
		/// </summary>
		/// <param name="FileName">the name of the file saved</param>
		/// <returns>bitmap picture</returns>
		public Bitmap Scan(string FileName)
		{	
			int trial = 1;
			int MaxTrials = 2;
			bool RT = false;			
				
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					InitScanner((DeviceInfo)myDeviceInfo);
					
					if (Scanner!=null)
					{
						// transfer picture to our temporary file
						string currFilename = Path.GetTempFileName();
						File.Delete(currFilename);
						
						// transfer picture to our temporary file
						ImageFile IF = (ImageFile)wiaItem.Transfer(FormatID.wiaFormatBMP);
						IF.SaveFile( currFilename);
						// Create a Bitmap from the loaded file (Image.FromFile locks the file...)
						FileStream fs = new FileStream(currFilename, FileMode.Open, FileAccess.Read);
						
						// KLUDGE: Must wrap the FromStream Image with a new Bitmap.
						// Otherwise get OutOfMemoryException later when using ColorMatrix on it.
						myBitmap = 	 new Bitmap(Image.FromStream(fs));
						fs.Close();
						
						SaveBitmapAsTiff(myBitmap, FileName);

						// Don't leave junk behind!
						File.Delete(currFilename);
						
						System.Threading.Thread.Sleep(10000);
						
						Disable();
						RT = true;
					}
					else
					{
						throw new ScnCtrlException("Device should be Selected First");
					}
				}
				catch(ScnrPwrMngrException e)
				{			
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;	
				}
				catch(System.Runtime.InteropServices.COMException e)
				{
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					
					trial++;
				}
				finally
				{
					// updated Irit L.Reisman 5.12 - try catch might be needed
					Disable();
				}
			}
			if (!RT)
			{
				throw new ScnCtrlException("Unable to scan: " + ScannerName);
			}
			
			return myBitmap;				
		}
		
		// update 4.12 - Irit L.Reisman	
		/// <summary>
		/// Takes a picture
		/// </summary>
		/// <param name="ScannerName"></param>
		/// <param name="FileName"></param>
		/// <returns></returns>
		public Bitmap Scan(string _ScannerName, string FileName)
		{	
			ScannerName = _ScannerName;
			
			int trial = 1;
			int MaxTrials = 2;
			bool RT = false;			
				
			while (trial<=MaxTrials && !RT)
			{
				try
				{
					InitScanner();
					
					if (Scanner!=null)
					{
						// transfer picture to our temporary file
						string currFilename = Path.GetTempFileName();
						File.Delete(currFilename);
						
						// transfer picture to our temporary file
						ImageFile IF = (ImageFile)wiaItem.Transfer(FormatID.wiaFormatBMP);
						IF.SaveFile( currFilename);
						// Create a Bitmap from the loaded file (Image.FromFile locks the file...)
						FileStream fs = new FileStream(currFilename, FileMode.Open, FileAccess.Read);
						
						// KLUDGE: Must wrap the FromStream Image with a new Bitmap.
						// Otherwise get OutOfMemoryException later when using ColorMatrix on it.
						myBitmap = 	 new Bitmap(Image.FromStream(fs));
						fs.Close();
						
						SaveBitmapAsTiff(myBitmap, FileName);

						// Don't leave junk behind!
						File.Delete(currFilename);
						
						System.Threading.Thread.Sleep(10000);
						
						Disable();
						RT = true;
					}
					else
					{
						throw new ScnCtrlException("Device should be Selected First");
					}
				}
				catch(ScnrPwrMngrException e)
				{			
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;	
				}
				catch(System.Runtime.InteropServices.COMException e)
				{
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					
					trial++;
				}
				catch(ScnCtrlException e)
				{
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;
				}
				catch(System.NullReferenceException e)
				{
					string errMsg = e.ToString() + Environment.NewLine + "   Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;
				}
				finally
				{
					// updated Irit L.Reisman 5.12 - try catch might be needed
					Disable();
				}
			}
			if (!RT)
			{
				throw new ScnCtrlException("Unable to scan: " + ScannerName);
			}
			
			return myBitmap;				
		}
		// end update
		
		/// <summary>
		/// Disables the selected scanner
		/// </summary>
		private void Disable()
		{
			int Delay = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DisableDelay"]);

			try
			{			
				objScannerPowerManager.DisableScanner();
				// updated Irit L.Reisman 2.5 - trying to avoid so many delays
//				System.Threading.Thread.Sleep(Delay);
			}
			catch(ScnrPwrMngrException e)
			{			
				string errMsg = e.ToString();
				scnMngrLog.LogError(errMsg);
			}
				
		}
		
		/// <summary>
		/// Enables the selected scanner
		/// </summary>
		public void Enable()
		{
			int Delay = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EnableDelay"]);
			
			objScannerPowerManager.EnableScanner();
			// updated Irit L.Reisman 2.5 - trying to avoid so many delays
//			System.Threading.Thread.Sleep(Delay);
		}
		
		/// <summary>
		/// Disables all connected scanners
		/// </summary>
		public void DisableAllScanners()
		{
			List<ImagingDevice> ImgDev = GetImagingDevicesList();
			for (int i=0; i<ImgDev.Count; i++)
			{
				objScannerPowerManager.InitScannerPowerManager(ImgDev[i].Name);
				Disable();
			}
		}
		
		#endregion
		
		
		#region image handling
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for(j = 0; j < encoders.Length; ++j)
			{
				if(encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}
		
		/// <summary>
		/// Saves a file of the picture in Tiff format
		/// </summary>
		/// <param name="myBitmap">bitmap pictures</param>
		/// <param name="FileName">the name of the file saved</param>
		private void SaveBitmapAsTiff(Bitmap myBitmap, string FileName)
		{
			ImageCodecInfo    myImageCodecInfo;
			Encoder           myEncoder;
			EncoderParameter  myEncoderParameter;
			EncoderParameters myEncoderParameters;
			
			// Get an ImageCodecInfo object that represents the TIFF codec.
			myImageCodecInfo = GetEncoderInfo("image/tiff");
			
			// Create an Encoder object based on the GUID
			// for the Compression parameter category.
			myEncoder = Encoder.Compression;
			
			// Create an EncoderParameters object.
			// An EncoderParameters object has an array of EncoderParameter
			// objects. In this case, there is only one
			// EncoderParameter object in the array.
			myEncoderParameters = new EncoderParameters(1);
			
			// Save the bitmap as a TIFF file with LZW compression.
			myEncoderParameter = new EncoderParameter(
				myEncoder,
				(long)EncoderValue.CompressionLZW);
			    myEncoderParameters.Param[0] = myEncoderParameter;
			    myBitmap.Save(FileName, myImageCodecInfo, myEncoderParameters);			
		}
		
		#endregion
	}
}




