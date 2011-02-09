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
	/// Description of ScannerControl.
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
		Bitmap myBitmap;
		private EventLog ScnCtrlEventLog;
		
		public ScannerControl()
		{
			DeviceManager = new DeviceManager();
			DeviceInfoCollection = DeviceManager.DeviceInfos  ;
			objScannerPowerManager = new ScannerPowerManager();
			ScnCtrlEventLog = new EventLog();
			ScnCtrlEventLog.Log =  "Application";
			ScnCtrlEventLog.Source = "ScannerControl";
			
		}

		public  DeviceInfos  GetConnectedDevices()
		{
			return DeviceInfoCollection ;			
		}
		
		public void SelectDevice(object _myDeviceInfo,int _ScanningDPI )
		{
			myDeviceInfo = (DeviceInfo)_myDeviceInfo;
			ScanningDPI = _ScanningDPI;
		}
		
		private void InitScanner(DeviceInfo _myDeviceInfo)
		{
			//try
			//{
				object np = "Name";
				ScannerName = (string)_myDeviceInfo.Properties.get_Item(ref np).get_Value();
				Enable();
				Scanner = _myDeviceInfo.Connect();
				wiaItem = Scanner.Items[1];
				SelectPicsProperties(ScanningDPI);
			//}
			//catch(System.Runtime.InteropServices.COMException e)
			//{
					
			//}
		}
		
		public Bitmap Scan(string FileName)
		{	
			InitScanner((DeviceInfo)myDeviceInfo);
			
			if (Scanner!=null)
			{
				// this call shows the common WIA dialog to let
				// the user select a picture:

				// enumerate all the pictures the user selected
				
				
				//SelectDeviceFromUI();
				//wiaPics = (CollectionClass)wiaRoot.Children;
				
				//ItemClass  wiaItem = (ItemClass) Marshal.CreateWrapperOfType( wiaPics[0], typeof(ItemClass) );

				
				
				
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
				
				//myBitmap.Save(FileName  ,ImageFormat.Tiff);
				//myBitmap.Save(FileName+"bmp"  ,ImageFormat.Bmp);
				SaveBitmapAsTiff(myBitmap, FileName);

				// Don't leave junk behind!
				File.Delete(currFilename);
				
				
				Disable();
				
				return myBitmap;
			}
			else
			{
				throw new ApplicationException("Device should be Selected First");
			}
		}
		
		private void SelectPicsProperties(int DPI)
		{
			object hr = "Horizontal Resolution";
			object vr = "Vertical Resolution";
			object res = DPI;
			
			
			Properties Prop = wiaItem.Properties;
			
			((WIA.Property)Prop.get_Item(ref hr)).set_Value(ref res);
			((WIA.Property)Prop.get_Item(ref vr)).set_Value(ref res);
		}
		
		private void Disable()
		{
			System.Threading.Thread.Sleep(10000);
			objScannerPowerManager.DisableScanner(ScannerName);
			System.Threading.Thread.Sleep(10000);
		}
		public void Enable()
		{
			objScannerPowerManager.EnableScanner(ScannerName);
			System.Threading.Thread.Sleep(10000);
			
		}
		
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
				(long)EncoderValue.CompressionNone);
			myEncoderParameters.Param[0] = myEncoderParameter;
			myBitmap.Save(FileName, myImageCodecInfo, myEncoderParameters);			
		}
		
	}
}




