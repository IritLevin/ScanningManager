/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 1/29/2008
 * Time: 1:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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


namespace ScaningManager
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
		private EventLog ScnMngrEventLog;
		private ScnMngrLog scnMngrLog;
		
		public MainForm()
		{

			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerControl";
			myScannerControl = new ScannerControl();
			
		}
		
		void calcExperimentEnd(object sender, EventArgs e)
		{
			//dtpStartDateTime.Value = DateTime.Now.AddMinutes(Convert.ToInt32(tbStartGap.Text));
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
			NumberOfScanners = lbScannersList.SelectedItems.Count;
			Scanners = new ScannerControl[NumberOfScanners];
			LastScans = new Bitmap[NumberOfScanners];
			
			ListBox.SelectedIndexCollection SelectedInd  = lbScannersList.SelectedIndices;
			
			for ( int i=0;i<NumberOfScanners;i++)
			{
				Scanners[i] = new ScannerControl();
				Scanners[i].SelectDevice( ScannersList[SelectedInd[i]],Convert.ToInt32(ConfigurationManager.AppSettings["ScanningDPI"]));
				
				cmbActiveScanners.Items.Add(lbScannersList.SelectedItems[i]);
				
			}
			
			// taking the fisrt picture
			// -------------------------
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
		
		string GetDateString (DateTime _DateTime)
		{
			//return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2")  + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") ;
			//return _DateTime.ToString("yyyyMMdd_HHmmss");
			return _DateTime.ToString("yyyyMMdd_HHmm");
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
		
		
		//private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		void MainFormLoad(object sender, EventArgs e)
		{
			dtpStartDateTime.Value = DateTime.Now;
			calcExperimentEnd(this, new EventArgs());
			tbOutputPath.Text =System.Configuration.ConfigurationManager.AppSettings["ImagesFolder"];
			
			DeviceInfos DIs =  myScannerControl.GetConnectedDevices();
			////System.Diagnostics.Debug.WriteLine(@"ScannersList = new DeviceInfo[DIs.Count];");
			ScannersList = new DeviceInfo[DIs.Count];
			for (int i=0;i<DIs.Count;i++)
			{
				object ind = i+1;
				////System.Diagnostics.Debug.WriteLine(@"ScannersList[i]=DIs.get_Item(ref ind);");
				ScannersList[i]=DIs.get_Item(ref ind);
				object propname = "Name";
				////System.Diagnostics.Debug.WriteLine(@"lbScannersList.Items.Add(ScannersList[i].Properties.get_Item(ref propname).get_Value()) ;");
				lbScannersList.Items.Add(ScannersList[i].Properties.get_Item(ref propname).get_Value()) ;
			}
			StatusLabel.Text = "";
			
		}
		
		void ScanningTimerTick(object sender, EventArgs e)
		{
			ScanNow();
			NextScan = DateTime.Now.AddMinutes(Convert.ToInt32(tbTimeGap.Text));
			UpdateExperimentProgress();
			this.Refresh();
		}
		
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
			ScanNow();
			this.Refresh();
			
		}
		
		void ScanNow()
		{
			if (dtpEndDateTime.Value < DateTime.Now)
			{
				ScanningTimer.Stop();
			}
			else
			{
				btnStop.Enabled = false;
				for (int i=0 ; i<NumberOfScanners;i++)
				{
					try
					{
						//lbStatusBar.Text = @"Scanning is in progress (scanner " + (i+1).ToString() + @"/" + NumberOfScanners.ToString() + ")";
						StatusLabel.Text = @"Scanning is in progress (scanner " + (i+1).ToString() + @"/" + NumberOfScanners.ToString() + ")";
						this.Refresh();
						LastScans[i]= Scanners[i].Scan(tbOutputPath.Text +  @"\" + tbFileName.Text + @"_" + i.ToString()+ @"_" + GetDateString(DateTime.Now)  +@".tif");
						
						picLastScan.Image = LastScans[i];
						//lbStatusBar.Text = "";
						StatusLabel.Text = "";
					}
					catch (ScnMngrException e)
					{
						ScnMngrEventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						// LastScans[i]= null? empty bitmap?
						scnMngrLog.LogError(e.ToString());
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e.ToString());

						ScnMngrEventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						scnMngrLog.LogError(e.ToString());
						throw e;
					}
				}
				btnStop.Enabled = true;
			}
		}
		
		void BtnExitClick(object sender, EventArgs e)
		{
			//lbStatusBar.Text = "please wait while scanners are reconnected";
			StatusLabel.Text = "please wait while scanners are reconnected";
			this.Refresh();
			ScanningTimer.Stop();
			StartTimer.Stop();
			UpdateProgressTimer.Stop();
			gbScanningConfiguration.Enabled   = true;
			gbExperimentConfiguration.Enabled = true;
			gbExperimentStatus.Enabled        = true;
			EnableAllScanners();
			//lbStatusBar.Text = "Process was stopped by user at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
			//if (StatusLabel.Text != "Experiment Ended")
			//{
			StatusLabel.Text = "Process was stopped by user at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
			//}
		}
		
		private void EnableAllScanners()
		{
			if (Scanners!=null)
			{
				try
				{
					for ( int i=0;i<Scanners.Length;i++)
					{
						Scanners[i].Enable();
					}
				}
				catch (ScnMngrException e)
				{
					ScnMngrEventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					scnMngrLog.LogError(e.ToString());
				}
				
			}
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
		
		//-------------------------------
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
			}
			else
			{
				progExperimentProgress.Value = progExperimentProgress.Maximum - TimeLeft;
				lblProgress.Text = @"Time Left: " + Seconds2hhmmssString(TimeLeft);//TimeLeft.ToString() + " minutes";
			}
		}
		
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
				}
			}
			else
			{
				EnableAllScanners();
			}
		}
		
		void LbScannersListLeave(object sender, EventArgs e)
		{
			if (lbScannersList.SelectedItems.Count != 0)
			{
				btnScan.Enabled = true;
			}
		}
		
		void StartLogging()
		{
			string LogFile = tbOutputPath.Text +  @"\LogFile.txt";
			string ExpParameters;
			
			scnMngrLog = new ScnMngrLog(LogFile);			
			ExpParameters = @"Repetitions: " + tbRepetitions.Text +
				@"   Time Gap: " + tbTimeGap.Text +
				@"   Start After: " + tbStartGap.Text;
			scnMngrLog.LogInfo(ExpParameters);
		}
		
		
	}
}
