/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 1/29/2008
 * Time: 1:31 PM
 */


// ScanningManager controls an array of scanners for time lapsed serial scanning.
// Copyright 2010 Irit Levin Reisman published under GPLv3,
// this software was developed in Prof. Nathalie Q. Balaban's lab, at the Hebrew University , Jerusalem , Israel .
//
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
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration ;
using System.Diagnostics;
using WIA;
using EnvRoomControler;
using EmailSMSSender;


namespace ScanningManager
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		ScannerControl myScannerControl;
		ScannerControl[] Scanners;
		int NumberOfScanners;
		Bitmap[] LastScans;
		DateTime NextScan;
		int HunderdNano2Sec = 10000000;
		DeviceInfo[] ScannersList;
		ScnMngrLog scnMngrLog=null;
		ScnMngrLog EnvRoomLog=null;
		string EnvLogFileName;
		string LogFileName;
		bool AllScannersEnabled = true;
		
		#region Form methods
		public MainForm()
		{			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			LogFileName = System.Configuration.ConfigurationManager.AppSettings["ImagesFolder"] + @"\LogFile.txt";
			myScannerControl = new ScannerControl(LogFileName);
			
		}
		
		void calcExperimentEnd(object sender, EventArgs e)
		{
			dtpEndDateTime.Value = dtpStartDateTime.Value.AddMinutes(Convert.ToInt32(tbRepetitions.Text)*
			                                                         Convert.ToDouble(tbTimeGap.Text));
		}
		
		void calcStartEndTime(object sender, EventArgs e)
		{
			dtpStartDateTime.Value = DateTime.Now.AddMinutes(Convert.ToInt32(tbStartGap.Text));
			calcExperimentEnd(sender, e);
		}
		
		void calcStartGapEndTime(object sender, EventArgs e)
		{
			tbStartGap.Text = Convert.ToString(Convert.ToInt32((dtpStartDateTime.Value - DateTime.Now).Ticks/HunderdNano2Sec/60));
			calcExperimentEnd(sender, e);
		}
		
		
		void BtnSelectOutputFolderClick(object sender, EventArgs e)
		{
			fbdSaveTo.RootFolder = System.Environment.SpecialFolder.MyComputer;
			fbdSaveTo.ShowNewFolderButton = true;
			fbdSaveTo.SelectedPath  =  System.Configuration.ConfigurationManager.AppSettings["ImagesFolder"];
			if (fbdSaveTo.ShowDialog() == DialogResult.OK)
			{
				tbOutputPath.Text  =  fbdSaveTo.SelectedPath;
			}
		}
		
		void TbOutputPathTextChanged(object sender, EventArgs e)
		{
			LogFileName = tbOutputPath.Text + @"\LogFile.txt";
		}
				
		void MainFormLoad(object sender, EventArgs e)
		{
			dtpStartDateTime.Value = DateTime.Now;
			calcExperimentEnd(this, new EventArgs());
			tbOutputPath.Text =System.Configuration.ConfigurationManager.AppSettings["ImagesFolder"];
			
			// creating available scanners list
			DeviceInfos DIs =  myScannerControl.GetConnectedDevices();
			ScannersList = new DeviceInfo[DIs.Count];
			for (int i=0;i<DIs.Count;i++)
			{
				object ind = i+1;
				////System.Diagnostics.Debug.WriteLine(@"ScannersList[i]=DIs.get_Item(ref ind);");
				ScannersList[i]=DIs.get_Item(ref ind);
				object propname = "Name";
				lbScannersList.Items.Add(ScannersList[i].Properties.get_Item(ref propname).get_Value()) ;
			}
			StatusLabel.Text = "";
			if (System.Configuration.ConfigurationManager.AppSettings["EnvRoomExist"]=="1")
			{
				cbRecordEnvRoom.Checked = true;
			}
			else
			{
				cbRecordEnvRoom.Checked = false;
			}
			
		}
		
		/// <summary>
		/// Closing the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason != CloseReason.WindowsShutDown)
			{
				DialogResult result = MessageBox.Show( @"Are you sure you want to exit?",
				                                      @"Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning  );
				if ( result == DialogResult.No)
				{
					e.Cancel = true;
				}
				else
				{
					StatusLabel.Text = "please wait while scanners are reconnected";
					this.Refresh();
					EnableAllScanners();
					if (scnMngrLog!= null)
					{
						scnMngrLog.LogInfo("Closing ScanningManager");
					}
				}
			}
			else
			{
				EnableAllScanners();
				scnMngrLog.LogWarn("Windows shutdown. Closing ScanningManager");
			}
		}
		
		/// <summary>
		/// A timer between scans
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ScanningTimerTick(object sender, EventArgs e)
		{
			if (cbRecordEnvRoom.Checked)
			{
				LogEnvRoom();
			}
			ScanNow();		
			NextScan = DateTime.Now.AddMinutes(Convert.ToInt32(tbTimeGap.Text));
			UpdateExperimentProgress();
			this.Refresh();
		}
		
		/// <summary>
		/// A timer to the first scan
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void StartTimerTick(object sender, EventArgs e)
		{
			StartTimer.Stop();
			ScanningTimer.Interval = Convert.ToInt32(Decimal.Floor(Convert.ToDecimal(tbTimeGap.Text)*60*1000));
			ScanningTimer.Start();
			
			// progress bar init
			NextScan = DateTime.Now.AddMinutes(Convert.ToInt32(tbTimeGap.Text));
			lblTimeToNextScan.Text = @"Time To Next Scan: " + Seconds2hhmmssString((Convert.ToInt32(tbTimeGap.Text)*60));//(Convert.ToInt32(tbTimeGap.Text)*60).ToString() + " seconds";
			progTimeToNextScan.Maximum = Convert.ToInt32(tbTimeGap.Text)*60;
			progTimeToNextScan.Minimum = 0;
			progTimeToNextScan.Value = 0;

			
			// start experiment
			ScanningTimer.Interval = Convert.ToInt32(Decimal.Floor(Convert.ToDecimal(tbTimeGap.Text)*60*1000));
			if (cbRecordEnvRoom.Checked)
			{
				LogEnvRoom();
			}
			ScanNow();
			this.Refresh();
			
		}
		
		void CmbActiveScannersSelectedIndexChanged(object sender, EventArgs e)
		{
			picLastScan.Image = LastScans[cmbActiveScanners.SelectedIndex];
		}
		
		
		void UpdateProgressTimerTick(object sender, EventArgs e)
		{
			UpdateNextScanProgress();
			UpdateExperimentProgress();
			this.Refresh();
		}
		

		void LbScannersListLeave(object sender, EventArgs e)
		{
			if (lbScannersList.SelectedItems.Count != 0)
			{
				btnScan.Enabled = true;
			}
		}
		
				
		/// <summary>
		/// starts the serial scanning by the parameters given for start time, time gap and number for iterations
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void BtnScanClick(object sender, EventArgs e)
		{
			StartLogging();

			// disabling the configuration and enabling the status group
			// ----------------------------------------------------------
			gbExperimentStatus.Enabled = true;
			gbScanningConfiguration.Enabled = false;
			gbExperimentConfiguration.Enabled = false;

			
			// Initializing the properties of scanning for each scanner
			// ---------------------------------------------------------
			AllScannersEnabled = false;
			NumberOfScanners = lbScannersList.SelectedItems.Count;
			Scanners = new ScannerControl[NumberOfScanners];
			LastScans = new Bitmap[NumberOfScanners];
			
			ListBox.SelectedIndexCollection SelectedInd  = lbScannersList.SelectedIndices;
			cmbActiveScanners.Items.Clear();
			
			for ( int i=0;i<NumberOfScanners;i++)
			{
				Scanners[i] = new ScannerControl(LogFileName);
				Scanners[i].SelectDevice( ScannersList[SelectedInd[i]],Convert.ToInt32(ConfigurationManager.AppSettings["ScanningDPI"]));
				
				cmbActiveScanners.Items.Add(lbScannersList.SelectedItems[i]);
				
			}
			
			// taking the fisrt picture
			// -------------------------			
			if (cbRecordEnvRoom.Checked)
			{
				LogEnvRoom();
			}
			ScanNow();
			
			// total experiment progress bar
			progExperimentProgress.Minimum = 0;
			progExperimentProgress.Maximum = Convert.ToInt32((dtpEndDateTime.Value - DateTime.Now).Ticks/(HunderdNano2Sec));
			progExperimentProgress.Value = 0;
			
			// Setting the timer to the first picture
			// ---------------------------------------
			int TimeToFirstScan = Convert.ToInt32((dtpStartDateTime.Value - DateTime.Now).Ticks/HunderdNano2Sec);
			if (TimeToFirstScan < 0)
			{
				// starting experiment immediatly
				StartTimerTick(this, new EventArgs());
			}
			else
			{
				// starting experiment with delay
				StartTimer.Interval =  TimeToFirstScan*1000 ;
				StartTimer.Start();
				// starting progress bar
				NextScan = dtpStartDateTime.Value;
				progTimeToNextScan.Minimum = 0;
				progTimeToNextScan.Maximum = TimeToFirstScan;
				progTimeToNextScan.Value = 0;
				lblTimeToNextScan.Text = @"Time To Next Scan: " + Seconds2hhmmssString(TimeToFirstScan);//TimeToFirstScan.ToString() + " seconds";
			}
			UpdateProgressTimer.Start();
		}
		
		/// <summary>
		/// Stops the experiment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void BtnExitClick(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show( @"Are you sure you want to stop scanning?",
				                                   @"Stop Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning  );
			if ( result == DialogResult.Yes)
			{
				StatusLabel.Text = "please wait while scanners are reconnected";
				this.Refresh();
				ScanningTimer.Stop();
				StartTimer.Stop();
				UpdateProgressTimer.Stop();
				gbScanningConfiguration.Enabled   = true;
				gbExperimentConfiguration.Enabled = true;
				gbExperimentStatus.Enabled        = true;
				EnableAllScanners();
				string msgText = "Process was stopped by user at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
				StatusLabel.Text = msgText;
				scnMngrLog.LogInfo(msgText);
			}
		}

		#endregion
		
		
		#region scanners handeling
		
		/// <summary>
		/// For each scanner in the list, takes an image
		/// </summary>
		void ScanNow()
		{
			if (dtpEndDateTime.Value < DateTime.Now)
			{
				ScanningTimer.Stop();
			}
			else
			{
				btnStop.Enabled = false;
				gbExperimentOwner.Enabled = false;
				
				// scanning with all the scanners selected
				for (int i=0 ; i<NumberOfScanners;i++)
				{
					try
					{
						StatusLabel.Text = @"Scanning is in progress (scanner " + (i+1).ToString() + @"/" + NumberOfScanners.ToString() + ")";
						this.Refresh();
						LastScans[i]= Scanners[i].Scan(tbOutputPath.Text +  @"\" + tbFileName.Text + @"_" + i.ToString()+ @"_" + GetDateString(DateTime.Now)  +@".tif");												
					}
					catch (ScnMngrException e)
					{
						LastScans[i] = CreateDefaultPicture();
						scnMngrLog.LogError(e.ToString());
						string ErrMsg = DateTime.Now.ToString("dd/MM/yyyy HH:mm") +" - " +
							e.Message.ToString() + 
							Environment.NewLine + tbLog.Text;
						tbLog.Text = ErrMsg;							
						if (tbEmail.Text != string.Empty)
						{
							try
							{
								EmailSMSSender.Sender.SendEmail(tbEmail.Text, 
								                                @"Scanning Manager Alert", 
								                                ErrMsg);
							}
							catch (Exception ex)
							{
								scnMngrLog.LogError("Can not send Email. " + ex.ToString());
							}
						}
						if (tbPhoneNumber.Text != string.Empty)
						{
							try
							{
								EmailSMSSender.Sender.SendSMS(tbPhoneNumber.Text, "Error scanning");
							}
							catch (Exception ex)
							{
								scnMngrLog.LogError("Can not send SMS. " + ex.ToString());
							}
						}
						
					}
					// error handling
					catch (Exception e)
					{
						LastScans[i] = CreateDefaultPicture();
						scnMngrLog.LogFatal(e.ToString());
						string ErrMsg = DateTime.Now.ToString("dd/MM/yyyy HH:mm") +" - " +
							e.Message.ToString() + 
							Environment.NewLine + tbLog.Text;
						tbLog.Text = ErrMsg;
						if (tbEmail.Text != string.Empty)
						{
							try
							{
								EmailSMSSender.Sender.SendEmail(tbEmail.Text, 
							                                @"Scanning Manager FATAL ERROR", 
							                                ErrMsg);
							}
							catch (Exception ex)
							{
								scnMngrLog.LogError("Can not send Email. " + ex.ToString());
							}
						}
						if (tbPhoneNumber.Text != string.Empty)
						{
							try
							{
								EmailSMSSender.Sender.SendSMS(tbPhoneNumber.Text, "Fatal Error");
							}
							catch (Exception ex)
							{
								scnMngrLog.LogError("Can not send SMS. " + ex.ToString());
							}
						}
						throw e;
					}
					
					picLastScan.Image = LastScans[i];
					StatusLabel.Text = "";
				}
				btnStop.Enabled = true;
				gbExperimentOwner.Enabled = true;
			}
		}
						
		/// <summary>
		/// Enable all scanners
		/// </summary>
		private void EnableAllScanners()
		{
			if (Scanners!=null && !AllScannersEnabled)
			{
				try
				{
					for ( int i=0;i<Scanners.Length;i++)
					{
						Scanners[i].Enable();
					}
					AllScannersEnabled = true;
				}
				catch (ScnMngrException e)
				{
					scnMngrLog.LogError(e.ToString());
					tbLog.Text = 
							DateTime.Now.ToShortDateString() + " " +
							DateTime.Now.ToShortTimeString() +" - " +
							e.Message.ToString() + 
							Environment.NewLine + tbLog.Text;
				}
				
			}
		}	
		
		/// <summary>
		/// Updating the progress till next scan
		/// </summary>
		void UpdateNextScanProgress()
		{
			int TimeLeft = Convert.ToInt32((NextScan - DateTime.Now).Ticks/HunderdNano2Sec);
			if (TimeLeft <= 0)
			{
				progTimeToNextScan.Value = progTimeToNextScan.Maximum;
				lblTimeToNextScan.Text = @"Time To Next Scan: 0 seconds";
			}
			else
			{
				progTimeToNextScan.Value = progTimeToNextScan.Maximum - TimeLeft;
				lblTimeToNextScan.Text = @"Time To Next Scan: " + Seconds2hhmmssString(TimeLeft);//TimeLeft.ToString() + " seconds";
				lblProgress.Text = @"Time Left: " + Seconds2hhmmssString(Convert.ToInt32((dtpEndDateTime.Value - DateTime.Now).Ticks/(HunderdNano2Sec)));//+ TimeLeft.ToString() + " minutes";
			}
		}
		
		/// <summary>
		/// Updating the progress till the end of the experiment
		/// </summary>
		void UpdateExperimentProgress()
		{
			int TimeLeft = Convert.ToInt32((dtpEndDateTime.Value - DateTime.Now).Ticks/(HunderdNano2Sec));
			if (TimeLeft <= 0)
			{
				progExperimentProgress.Value = progExperimentProgress.Maximum;
				progTimeToNextScan.Value = progTimeToNextScan.Maximum;
				lblProgress.Text = @"Time Left: 00:00:00";
				lblTimeToNextScan.Text = @"Time To Next Scan: 00:00:00";
				StatusLabel.Text =  @"Experiment Ended";
				EnableAllScanners();
				ScanningTimer.Stop();
				UpdateProgressTimer.Stop();
				scnMngrLog.LogInfo("Experiment Ended");
			}
			else
			{
				progExperimentProgress.Value = progExperimentProgress.Maximum - TimeLeft;
				lblProgress.Text = @"Time Left: " + Seconds2hhmmssString(TimeLeft);//TimeLeft.ToString() + " minutes";
			}
		}
		
		#endregion
		
		
		#region utilities
		
		/// <summary>
		/// Converts time in second to hours:minutes:seconds
		/// </summary>
		/// <param name="TimeInSeconds">Time in seconds</param>
		/// <returns>hhnnss</returns>
		string Seconds2hhmmssString(int TimeInSeconds)
		{
			string TSstr = string.Empty;
			int SecondsInHour = 60*60;
			int SecondsInMinute = 60;
			int sec = 0;
			int min = 0;
			int hr = 0;
			
			hr = TimeInSeconds/SecondsInHour;
			min = (TimeInSeconds - hr*SecondsInHour)/SecondsInMinute;
			sec = TimeInSeconds - hr*SecondsInHour - min*SecondsInMinute;
			
			TSstr = hr.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2");
			return TSstr;
		}
		
		/// <summary>
		/// formating a date to string
		/// </summary>
		/// <param name="_DateTime"></param>
		/// <returns>yyyyMMdd_HHmm</returns>
		string GetDateString (DateTime _DateTime)
		{
			return _DateTime.ToString("yyyyMMdd_HHmm");
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
		
		#endregion
		
		
		#region Logging
		
		/// <summary>
		/// Initializes log file
		/// </summary>
		void StartLogging()
		{
			string ExpParameters;
			
			scnMngrLog = new ScnMngrLog(LogFileName);	
			scnMngrLog.LogInfo(@"ScanningManager version :"+
				System.Reflection.Assembly.GetExecutingAssembly().GetName(false).Version.ToString());
			ExpParameters = 
				@"Repetitions: "    + tbRepetitions.Text +
				@"   Time Gap: "    + tbTimeGap.Text +
				@"   Start After: " + tbStartGap.Text +
				@"   Start Time: "  + dtpStartDateTime.Text +
				@"   End Time: "    + dtpEndDateTime.Text;
			scnMngrLog.LogInfo(ExpParameters);
			
			if (cbRecordEnvRoom.Checked)
			{
				StartEnvRoomLogging();
			}
		}
		
		/// <summary>
		/// Starts logging environmental room
		/// </summary>
		void StartEnvRoomLogging()
		{
			EnvLogFileName = tbOutputPath.Text +  @"\EnvRoom.csv";	
			EnvControlerIO CIO    = new EnvControlerIO();
			List<EnvRoomControler.ControllerEntry> CE = CIO.GetCurentValues();
			string EnvRoomMsg     = ",\t";
			
			EnvRoomLog = new ScnMngrLog(EnvLogFileName);
//			System.IO.FileInfo EnvRoomLogFile = new FileInfo(EnvLogFileName);
//			StreamWriter SR = new StreamWriter(EnvLogFileName,true);
			
            for(int i=0;i<CE.Count;i++)
            {
                EnvRoomMsg += CE[i].LongName + ",\t";
            }
            
            EnvRoomLog.LogLine(EnvRoomMsg);
//            SR.WriteLine(EnvRoomMsg);
//            SR.Close();			
		}
		
		/// <summary>
		/// Writing to log a sample of Environmental Room 
		/// </summary>
		void LogEnvRoom()
		{
			EnvControlerIO CIO = new EnvControlerIO();
			List<EnvRoomControler.ControllerEntry> CE = CIO.GetCurentValues();
			string EnvRoomMsg = ",\t";//DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ",\t";
			
//			System.IO.FileInfo EnvRoomLogFile = new FileInfo(EnvLogFileName);
//			StreamWriter SR = new StreamWriter(EnvLogFileName,true);
			
            for(int i=0;i<CE.Count;i++)
            {
                EnvRoomMsg += CE[i].EntryValue +",\t";
            }
            
            EnvRoomLog.LogLine(EnvRoomMsg);
//            SR.WriteLine(EnvRoomMsg);
//            SR.Close();			
		}
		
		#endregion
		
	}
}
