/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 12/10/2008
 * Time: 10:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

// ADU needs to interoperate with the legacy DLL
using System.Runtime.InteropServices;

namespace TestADU200X2
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private IntPtr hAdu1;
		private IntPtr hAdu2;
		
		[DllImport("aduhid.dll")]
		public static extern IntPtr OpenAduDevice(UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern IntPtr OpenAduDeviceBySerialNumber(string psSerialNumber,UInt32 iTimeout);
		
		[DllImport("aduhid.dll")]
		public static extern bool WriteAduDevice(IntPtr hFile,
		                                         [MarshalAs (UnmanagedType.LPStr)]string lpBuffer,
		                                         UInt32 nNumberOfBytesToWrite,
		                                         out UInt32 lpNumberOfBytesWritten,
		                                         UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern bool ReadAduDevice(IntPtr hFile,
		                                        StringBuilder lpBuffer,
		                                        UInt32 nNumberOfBytesToRead,
		                                        out UInt32 lpNumberOfBytesRead,
		                                        UInt32 iTimeout);

		[DllImport("aduhid.dll")]
		public static extern void CloseAduDevice(IntPtr hFile);
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void BtnOpen1Click(object sender, EventArgs e)
		{
			hAdu1 = OpenAduDeviceBySerialNumber(txtDeviceID1.Text,500);
			if (hAdu1.ToInt32()<0)
			{
				string errMsg = "unable to open " + txtDeviceID1.Text;
				throw new Exception(errMsg);
			}
			ckbl1.Enabled=true;
		}
		
		void BtnOpen2Click(object sender, EventArgs e)
		{
			hAdu2 = OpenAduDeviceBySerialNumber(txtDeviceID2.Text,500);
			if (hAdu2.ToInt32()<0)
			{
				string errMsg = "unable to open " + txtDeviceID2.Text;
				throw new Exception(errMsg);
			}
			ckbl2.Enabled=true;
		}
		
		void BtnClose1Click(object sender, EventArgs e)
		{
			CloseAduDevice(hAdu1);
			ckbl1.Enabled=false;
		}
		
		void BtnClose2Click(object sender, EventArgs e)
		{
			CloseAduDevice(hAdu2);
			ckbl2.Enabled=false;
		}
		
		
		void Ckbl1ItemCheck(object sender, ItemCheckEventArgs e)
		{
			string txtCommand = string.Empty;
			if(e.CurrentValue==CheckState.Unchecked )
			{
				txtCommand = "sk";
			}
			else
			{
				txtCommand = "rk";
			}
			txtCommand += e.Index.ToString();
			bool bRC = false;
			uint uiWritten = 0xdead;
			uint uiLength = (uint)txtCommand.Length;

			bRC = WriteAduDevice(hAdu1, txtCommand, uiLength, out uiWritten, 500);
			if (!bRC)
			{
				throw new Exception("unable to open/close port");
			}
			txtStatus1.Text = CheckStatus(hAdu1);
		}
		
		
		
		void Ckbl2ItemCheck(object sender, ItemCheckEventArgs e)
		{
			string txtCommand = string.Empty;
			if(e.CurrentValue==CheckState.Unchecked)
			{
				txtCommand = "sk";
			}
			else
			{
				txtCommand = "rk";
			}
			txtCommand += e.Index.ToString();
			bool bRC = false;
			uint uiWritten = 0xdead;
			uint uiLength = (uint)txtCommand.Length;

			bRC = WriteAduDevice(hAdu2, txtCommand, uiLength, out uiWritten, 500);
			if (!bRC)
			{
				throw new Exception("unable to open/close port");
			}
			txtStatus2.Text = CheckStatus(hAdu2);
			
		}
		
		void BtnCheckStat1Click(object sender, EventArgs e)
		{			
    		txtStatus1.Text = CheckStatus(hAdu1);
		}
		
		void BtnCheckStat2Click(object sender, EventArgs e)
		{
			txtStatus2.Text = CheckStatus(hAdu2);
		}
		
		string CheckStatus(IntPtr hAdu)
		{
			string txtCommand = string.Empty;
			txtCommand = "rpk";
			bool bRC = false;
			uint uiWritten = 0xdead;
			uint uiLength = (uint)txtCommand.Length;
			
			bRC = WriteAduDevice(hAdu, txtCommand, uiLength, out uiWritten, 500);
			
			uint uiRead = 0;
			uiLength = 7;
			
			StringBuilder sBuffer = new StringBuilder(8);
 
			bRC = ReadAduDevice(hAdu, sBuffer, uiLength, out uiRead, 500);			
    		return sBuffer.ToString();
		}
	}
}
