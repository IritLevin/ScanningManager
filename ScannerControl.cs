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
using WIALib ;
using System.Threading ;

namespace ScaningManager
{
	/// <summary>
	/// Description of ScannerControl.
	/// </summary>
	public class ScannerControl
	{
		private ScannerPowerManager  objScannerPowerManager;
		private Wia wiaManager ;
		private Collection  objDeviceInfoCollection;
		private ItemClass wiaRoot;
		private WIALib.Collection  wiaPics;
		
		private string ScannerName;
		Bitmap myBitmap;
		
		public ScannerControl()
		{
			wiaManager = new WIALib.WiaClass() ;
			objDeviceInfoCollection = wiaManager.Devices ;
			objScannerPowerManager = new ScannerPowerManager();
		}
		
		
		public void SelectDeviceFromUI()
		{
			object selectUsingUI = System.Reflection.Missing.Value;

			// let user select device
			wiaRoot = (ItemClass) wiaManager.Create(  ref selectUsingUI );
		}
		
		
		public  WIALib.Collection  GetConnectedDevices()
		{
			return wiaManager.Devices ;
			
		}
		
		public void SelectDevice(object myDeviceInfo )
		{
			ScannerName = ((DeviceInfo) myDeviceInfo).Name;
			wiaRoot = (ItemClass) wiaManager.Create(  ref myDeviceInfo );
		}
		
		public Bitmap Scan(string FileName)
		{
			//Enable();
			
			if (wiaRoot!=null)
			{
				// this call shows the common WIA dialog to let
				// the user select a picture:

				// enumerate all the pictures the user selected
				
				
				//SelectDeviceFromUI();
				//wiaPics = (CollectionClass)wiaRoot.Children;
				
				ItemClass  wiaItem = (ItemClass) Marshal.CreateWrapperOfType( wiaPics[0], typeof(ItemClass) );

				
				// transfer picture to our temporary file
				string currFilename = Path.GetTempFileName();

				// transfer picture to our temporary file
				wiaItem.Transfer(currFilename, false);

				// Create a Bitmap from the loaded file (Image.FromFile locks the file...)
				FileStream fs = new FileStream(currFilename, FileMode.Open, FileAccess.Read);
				
				// KLUDGE: Must wrap the FromStream Image with a new Bitmap.
				// Otherwise get OutOfMemoryException later when using ColorMatrix on it.
				myBitmap = 	 new Bitmap(Image.FromStream(fs));
				fs.Close();
				
				myBitmap.Save(FileName  ,ImageFormat.Tiff);

				// Don't leave junk behind!
				File.Delete(currFilename);
				
				
				//Disable();
				
				return myBitmap;
			}
			else
			{
				throw new ApplicationException("Device should be Selected First");
			}
		}
		public void SelectPicsPropertiesFromUI()
		{
			wiaPics = wiaRoot.GetItemsFromUI( WiaFlag.SingleImage,  WiaIntent.ImageTypeColor ) as CollectionClass;
			if (wiaPics == null)
			{
				throw new ApplicationException("Scanner parameters must be set!");
			}
		}
		
		private void Disable()
		{
			objScannerPowerManager.DisableScanner(ScannerName);
			System.Threading.Thread.Sleep(2000);
		}
		private void Enable()
		{
			objScannerPowerManager.EnableScanner(ScannerName);
			System.Threading.Thread.Sleep(2000);
		}
		
	}
}




