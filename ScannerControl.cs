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
		Bitmap myBitmap;
//		private EventLog ScnCtrlEventLog;
		private ScnMngrLog scnMngrLog;
		
		/// <summary>
		/// Constructor. 
		/// </summary>
		public ScannerControl()
		{
			//System.Diagnostics.Debug.WriteLine(@"DeviceManager = new DeviceManager();");
			DeviceManager = new DeviceManager();
			//System.Diagnostics.Debug.WriteLine(@"DeviceInfoCollection = DeviceManager.DeviceInfos  ;");
			DeviceInfoCollection = DeviceManager.DeviceInfos  ;
			//System.Diagnostics.Debug.WriteLine(@"objScannerPowerManager = new ScannerPowerManager();");
			objScannerPowerManager = new ScannerPowerManager();
//			ScnCtrlEventLog = new EventLog();
//			ScnCtrlEventLog.Log =  "Application";
//			ScnCtrlEventLog.Source = "ScannerControl";
			scnMngrLog = new ScnMngrLog();
			
		}

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
			//System.Diagnostics.Debug.WriteLine(@"myDeviceInfo = (DeviceInfo)_myDeviceInfo;");
			myDeviceInfo = (DeviceInfo)_myDeviceInfo;
			ScanningDPI = _ScanningDPI;
		}
		
		/// <summary>
		/// Connects to a specific scanner and initializes parameters
		/// </summary>
		/// <param name="_myDeviceInfo">selected scanner</param>
		private void InitScanner(DeviceInfo _myDeviceInfo)
		{	

			object np = "Name";
			//System.Diagnostics.Debug.WriteLine(@"ScannerName = (string)_myDeviceInfo.Properties.get_Item(ref np).get_Value();");
			ScannerName = (string)_myDeviceInfo.Properties.get_Item(ref np).get_Value();
			objScannerPowerManager.InitScannerPowerManager(ScannerName);
			Enable();
			//System.Diagnostics.Debug.WriteLine(@"Scanner = _myDeviceInfo.Connect();");
			Scanner = _myDeviceInfo.Connect();
			//System.Diagnostics.Debug.WriteLine(@"wiaItem = Scanner.Items[1];");
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
						//System.Diagnostics.Debug.WriteLine(@"ImageFile IF = (ImageFile)wiaItem.Transfer(FormatID.wiaFormatBMP);");
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
						
						
						Disable();
						RT = true;
						return myBitmap;
					}
					else
					{
						//throw new ApplicationException("Device should be Selected First");
						throw new ScnCtrlException("Device should be Selected First");
					}
				}
				catch(ScnrPwrMngrException e)
				{			
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
				
					trial++;	
				}
				catch(System.Runtime.InteropServices.COMException e)
				{
					string errMsg = e.ToString() + " \nTrial: " + trial.ToString();
					scnMngrLog.LogError(errMsg);
					
					trial++;
				}
			}
//			if (!RT)
//			{
//				throw new ScnCtrlException("Unable to scan: " + ScannerName);
//			}
			return new Bitmap(CreateDefaultPicture());
				
		}
		
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
		/// Disables the selected scanner
		/// </summary>
		private void Disable()
		{
			try
			{
				System.Threading.Thread.Sleep(10000);
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
				(long)EncoderValue.CompressionNone);
			    myEncoderParameters.Param[0] = myEncoderParameter;
			    myBitmap.Save(FileName, myImageCodecInfo, myEncoderParameters);			
		}
		
		/// <summary>
		/// Generates a default picture
		/// </summary>
		/// <returns>Picture of Error Scanning</returns>
		private Bitmap CreateDefaultPicture()
		{
			Bitmap B = new Bitmap(100,100);
			Graphics g = Graphics.FromImage(B);
			
			// Create pen.
			Pen blackPen = new Pen(Color.Red, 3);
			
			// Create points that define line.
			Point point1 ;
			Point point2;
					
			point1 = new Point(0, 0);
			point2 = new Point(100, 100);
			
			// Draw line to screen.
			g.DrawLine(blackPen, point1, point2);
			point1 = new Point(100, 0);
			point2 = new Point(0, 100);
			
			// Draw line to screen.
			g.DrawLine(blackPen, point1, point2);
			
			// Create string to draw.
			String drawString = "Error";
			
			// Create font and brush.
			Font drawFont = new Font("Arial", 16);
			SolidBrush drawBrush = new SolidBrush(Color.Black);
			
			// Create point for upper-left corner of drawing.
			PointF drawPoint = new PointF(20, 20);
			
			// Draw string to screen.
			g.DrawString(drawString, drawFont, drawBrush, drawPoint);
			
			drawString = "Scanning";
			drawPoint = new PointF(5, 40);
			g.DrawString(drawString, drawFont, drawBrush, drawPoint);

			
			return B;
		}
		
	}
}




