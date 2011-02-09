/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 1/29/2008
 * Time: 1:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using WIA ;
using System.Threading ;
using System.Diagnostics;

namespace ScaningManager
{
	/// <summary>
	/// Controls the scanning process
	/// </summary>
	public class ScannerControl
	{
		private	DeviceManager  DeviceManager;
		private ScannerPowerManager  objScannerPowerManager;
		private DeviceInfos  DeviceInfoCollection;
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
			//System.Diagnostics.Debug.WriteLine(@"DeviceManager = new DeviceManager();");
			DeviceManager = new DeviceManager();
			DeviceInfoCollection = DeviceManager.DeviceInfos  ;
			objScannerPowerManager = new ScannerPowerManager();
			scnMngrLog = new ScnMngrLog();
			
		}

		#region scanners list handling
		
		/// <summary>
		/// Gets a list of scanners
		/// </summary>
		/// <returns>Connected scanners</returns>
		public  DeviceInfos  GetConnectedDevices()
		{
			return DeviceInfoCollection ;			
		}
		
		/// <summary>
		/// Picks a scanner from the connected scanners list
		/// </summary>
		/// <param name="_myDeviceInfo">selected scanner</param>
		/// <param name="_ScanningDPI">scanning resolution</param>
		public void SelectDevice(object _myDeviceInfo,int _ScanningDPI )
		{
			myDeviceInfo = (DeviceInfo)_myDeviceInfo;
			ScanningDPI = _ScanningDPI;
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
			SelectPicsProperties(ScanningDPI);	
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
					string errMsg = e.ToString() + Environment.NewLine + " Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;	
				}
				catch(System.Runtime.InteropServices.COMException e)
				{
					string errMsg = e.ToString() + Environment.NewLine + " Trial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					
					trial++;
				}
			}
			if (!RT)
			{
				throw new ScnCtrlException("Unable to scan: " + ScannerName);
			}
			
			return myBitmap;				
		}
				
		/// <summary>
		/// Disables the selected scanner
		/// </summary>
		private void Disable()
		{
			try
			{			
				objScannerPowerManager.DisableScanner();
				System.Threading.Thread.Sleep(1000);
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
			objScannerPowerManager.EnableScanner();
			System.Threading.Thread.Sleep(1000);
		}
		
		#endregion
		
		
		#region image handling
		
		/// <summary>
		/// Sets picture properties
		/// </summary>
		/// <param name="DPI">resolution</param>
		private void SelectPicsProperties(int DPI)
		{
			object hr = "Horizontal Resolution";
			object vr = "Vertical Resolution";
			object res = DPI;
			
			
			Properties Prop = wiaItem.Properties;
			
			((WIA.Property)Prop.get_Item(ref hr)).set_Value(ref res);
			((WIA.Property)Prop.get_Item(ref vr)).set_Value(ref res);
		}
		
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




