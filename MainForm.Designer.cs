/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 1/29/2008
 * Time: 1:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
namespace ScaningManager
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override  void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.fbdSaveTo = new System.Windows.Forms.FolderBrowserDialog();
			this.btnScan = new System.Windows.Forms.Button();
			this.btnSelectOutputFolder = new System.Windows.Forms.Button();
			this.tbOutputPath = new System.Windows.Forms.TextBox();
			this.picLastScan = new System.Windows.Forms.PictureBox();
			this.gbExperimentConfiguration = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbStartGap = new System.Windows.Forms.TextBox();
			this.lblStartGap = new System.Windows.Forms.Label();
			this.lMinutes = new System.Windows.Forms.Label();
			this.tbTimeGap = new System.Windows.Forms.TextBox();
			this.tbRepetitions = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.dtpEndDateTime = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.dtpStartDateTime = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.gbScanningConfiguration = new System.Windows.Forms.GroupBox();
			this.lbScannersList = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.tbFileName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.ScanningTimer = new System.Windows.Forms.Timer(this.components);
			this.StartTimer = new System.Windows.Forms.Timer(this.components);
			this.gbExperimentStatus = new System.Windows.Forms.GroupBox();
			this.progTimeToNextScan = new System.Windows.Forms.ProgressBar();
			this.progExperimentProgress = new System.Windows.Forms.ProgressBar();
			this.lblProgress = new System.Windows.Forms.Label();
			this.cmbActiveScanners = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.lblTimeToNextScan = new System.Windows.Forms.Label();
			this.btnStop = new System.Windows.Forms.Button();
			this.UpdateProgressTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.EventLogger = new System.Diagnostics.EventLog();
			((System.ComponentModel.ISupportInitialize)(this.picLastScan)).BeginInit();
			this.gbExperimentConfiguration.SuspendLayout();
			this.gbScanningConfiguration.SuspendLayout();
			this.gbExperimentStatus.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.EventLogger)).BeginInit();
			this.SuspendLayout();
			// 
			// btnScan
			// 
			this.btnScan.Enabled = false;
			this.btnScan.Location = new System.Drawing.Point(76, 173);
			this.btnScan.Name = "btnScan";
			this.btnScan.Size = new System.Drawing.Size(131, 25);
			this.btnScan.TabIndex = 2;
			this.btnScan.Text = "Scan";
			this.btnScan.UseVisualStyleBackColor = true;
			this.btnScan.Click += new System.EventHandler(this.BtnScanClick);
			// 
			// btnSelectOutputFolder
			// 
			this.btnSelectOutputFolder.Location = new System.Drawing.Point(239, 17);
			this.btnSelectOutputFolder.Name = "btnSelectOutputFolder";
			this.btnSelectOutputFolder.Size = new System.Drawing.Size(25, 23);
			this.btnSelectOutputFolder.TabIndex = 4;
			this.btnSelectOutputFolder.Text = "...";
			this.btnSelectOutputFolder.UseVisualStyleBackColor = true;
			this.btnSelectOutputFolder.Click += new System.EventHandler(this.BtnSelectOutputFolderClick);
			// 
			// tbOutputPath
			// 
			this.tbOutputPath.Location = new System.Drawing.Point(76, 19);
			this.tbOutputPath.Name = "tbOutputPath";
			this.tbOutputPath.Size = new System.Drawing.Size(158, 20);
			this.tbOutputPath.TabIndex = 5;
			this.tbOutputPath.Text = "D:\\Irit";
			// 
			// picLastScan
			// 
			this.picLastScan.Location = new System.Drawing.Point(7, 182);
			this.picLastScan.Name = "picLastScan";
			this.picLastScan.Size = new System.Drawing.Size(217, 182);
			this.picLastScan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picLastScan.TabIndex = 7;
			this.picLastScan.TabStop = false;
			// 
			// gbExperimentConfiguration
			// 
			this.gbExperimentConfiguration.Controls.Add(this.label8);
			this.gbExperimentConfiguration.Controls.Add(this.tbStartGap);
			this.gbExperimentConfiguration.Controls.Add(this.lblStartGap);
			this.gbExperimentConfiguration.Controls.Add(this.lMinutes);
			this.gbExperimentConfiguration.Controls.Add(this.tbTimeGap);
			this.gbExperimentConfiguration.Controls.Add(this.tbRepetitions);
			this.gbExperimentConfiguration.Controls.Add(this.label4);
			this.gbExperimentConfiguration.Controls.Add(this.dtpEndDateTime);
			this.gbExperimentConfiguration.Controls.Add(this.label3);
			this.gbExperimentConfiguration.Controls.Add(this.dtpStartDateTime);
			this.gbExperimentConfiguration.Controls.Add(this.label2);
			this.gbExperimentConfiguration.Controls.Add(this.label1);
			this.gbExperimentConfiguration.Location = new System.Drawing.Point(12, 12);
			this.gbExperimentConfiguration.Name = "gbExperimentConfiguration";
			this.gbExperimentConfiguration.Size = new System.Drawing.Size(273, 159);
			this.gbExperimentConfiguration.TabIndex = 9;
			this.gbExperimentConfiguration.TabStop = false;
			this.gbExperimentConfiguration.Text = "Experiment Configuration";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(121, 77);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 17);
			this.label8.TabIndex = 16;
			this.label8.Text = "minutes";
			// 
			// tbStartGap
			// 
			this.tbStartGap.Location = new System.Drawing.Point(76, 73);
			this.tbStartGap.Name = "tbStartGap";
			this.tbStartGap.Size = new System.Drawing.Size(39, 20);
			this.tbStartGap.TabIndex = 15;
			this.tbStartGap.Leave += new System.EventHandler(this.calcStartEndTime);
			// 
			// lblStartGap
			// 
			this.lblStartGap.Location = new System.Drawing.Point(6, 77);
			this.lblStartGap.Name = "lblStartGap";
			this.lblStartGap.Size = new System.Drawing.Size(100, 18);
			this.lblStartGap.TabIndex = 14;
			this.lblStartGap.Text = "Start after";
			// 
			// lMinutes
			// 
			this.lMinutes.Location = new System.Drawing.Point(121, 50);
			this.lMinutes.Name = "lMinutes";
			this.lMinutes.Size = new System.Drawing.Size(100, 23);
			this.lMinutes.TabIndex = 13;
			this.lMinutes.Text = "minutes";
			// 
			// tbTimeGap
			// 
			this.tbTimeGap.Location = new System.Drawing.Point(76, 47);
			this.tbTimeGap.Name = "tbTimeGap";
			this.tbTimeGap.Size = new System.Drawing.Size(39, 20);
			this.tbTimeGap.TabIndex = 12;
			this.tbTimeGap.Text = "15";
			this.tbTimeGap.Leave += new System.EventHandler(this.calcExperimentEnd);
			// 
			// tbRepetitions
			// 
			this.tbRepetitions.Location = new System.Drawing.Point(76, 21);
			this.tbRepetitions.Name = "tbRepetitions";
			this.tbRepetitions.Size = new System.Drawing.Size(39, 20);
			this.tbRepetitions.TabIndex = 11;
			this.tbRepetitions.Text = "2";
			this.tbRepetitions.Leave += new System.EventHandler(this.calcExperimentEnd);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 129);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 23);
			this.label4.TabIndex = 10;
			this.label4.Text = "End Time";
			// 
			// dtpEndDateTime
			// 
			this.dtpEndDateTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpEndDateTime.Enabled = false;
			this.dtpEndDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpEndDateTime.Location = new System.Drawing.Point(76, 125);
			this.dtpEndDateTime.Name = "dtpEndDateTime";
			this.dtpEndDateTime.Size = new System.Drawing.Size(118, 20);
			this.dtpEndDateTime.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Start Time";
			// 
			// dtpStartDateTime
			// 
			this.dtpStartDateTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpStartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpStartDateTime.Location = new System.Drawing.Point(76, 99);
			this.dtpStartDateTime.Name = "dtpStartDateTime";
			this.dtpStartDateTime.Size = new System.Drawing.Size(118, 20);
			this.dtpStartDateTime.TabIndex = 5;
			this.dtpStartDateTime.Leave += new System.EventHandler(this.calcStartGapEndTime);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Time Gap";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Repetitions";
			// 
			// gbScanningConfiguration
			// 
			this.gbScanningConfiguration.Controls.Add(this.lbScannersList);
			this.gbScanningConfiguration.Controls.Add(this.label7);
			this.gbScanningConfiguration.Controls.Add(this.label6);
			this.gbScanningConfiguration.Controls.Add(this.tbFileName);
			this.gbScanningConfiguration.Controls.Add(this.label5);
			this.gbScanningConfiguration.Controls.Add(this.btnSelectOutputFolder);
			this.gbScanningConfiguration.Controls.Add(this.btnScan);
			this.gbScanningConfiguration.Controls.Add(this.tbOutputPath);
			this.gbScanningConfiguration.Location = new System.Drawing.Point(12, 178);
			this.gbScanningConfiguration.Name = "gbScanningConfiguration";
			this.gbScanningConfiguration.Size = new System.Drawing.Size(273, 204);
			this.gbScanningConfiguration.TabIndex = 10;
			this.gbScanningConfiguration.TabStop = false;
			this.gbScanningConfiguration.Text = "Scanning Configuration";
			// 
			// lbScannersList
			// 
			this.lbScannersList.DisplayMember = "Name";
			this.lbScannersList.FormattingEnabled = true;
			this.lbScannersList.Location = new System.Drawing.Point(76, 71);
			this.lbScannersList.Name = "lbScannersList";
			this.lbScannersList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbScannersList.Size = new System.Drawing.Size(188, 95);
			this.lbScannersList.TabIndex = 11;
			this.lbScannersList.ValueMember = "Name";
			this.lbScannersList.Leave += new System.EventHandler(this.LbScannersListLeave);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 71);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(59, 23);
			this.label7.TabIndex = 10;
			this.label7.Text = "Scanners";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(7, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 23);
			this.label6.TabIndex = 9;
			this.label6.Text = "File Name";
			// 
			// tbFileName
			// 
			this.tbFileName.Location = new System.Drawing.Point(76, 45);
			this.tbFileName.Name = "tbFileName";
			this.tbFileName.Size = new System.Drawing.Size(158, 20);
			this.tbFileName.TabIndex = 8;
			this.tbFileName.Text = "img";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 23);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 23);
			this.label5.TabIndex = 6;
			this.label5.Text = "Folder";
			// 
			// ScanningTimer
			// 
			this.ScanningTimer.Tick += new System.EventHandler(this.ScanningTimerTick);
			// 
			// StartTimer
			// 
			this.StartTimer.Tick += new System.EventHandler(this.StartTimerTick);
			// 
			// gbExperimentStatus
			// 
			this.gbExperimentStatus.Controls.Add(this.progTimeToNextScan);
			this.gbExperimentStatus.Controls.Add(this.progExperimentProgress);
			this.gbExperimentStatus.Controls.Add(this.lblProgress);
			this.gbExperimentStatus.Controls.Add(this.cmbActiveScanners);
			this.gbExperimentStatus.Controls.Add(this.label10);
			this.gbExperimentStatus.Controls.Add(this.lblTimeToNextScan);
			this.gbExperimentStatus.Controls.Add(this.picLastScan);
			this.gbExperimentStatus.Enabled = false;
			this.gbExperimentStatus.Location = new System.Drawing.Point(291, 12);
			this.gbExperimentStatus.Name = "gbExperimentStatus";
			this.gbExperimentStatus.Size = new System.Drawing.Size(236, 370);
			this.gbExperimentStatus.TabIndex = 11;
			this.gbExperimentStatus.TabStop = false;
			this.gbExperimentStatus.Text = "Experiment Status";
			// 
			// progTimeToNextScan
			// 
			this.progTimeToNextScan.Location = new System.Drawing.Point(7, 47);
			this.progTimeToNextScan.Name = "progTimeToNextScan";
			this.progTimeToNextScan.Size = new System.Drawing.Size(218, 23);
			this.progTimeToNextScan.TabIndex = 16;
			// 
			// progExperimentProgress
			// 
			this.progExperimentProgress.Location = new System.Drawing.Point(7, 96);
			this.progExperimentProgress.Name = "progExperimentProgress";
			this.progExperimentProgress.Size = new System.Drawing.Size(218, 23);
			this.progExperimentProgress.TabIndex = 15;
			// 
			// lblProgress
			// 
			this.lblProgress.Location = new System.Drawing.Point(6, 77);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(218, 23);
			this.lblProgress.TabIndex = 14;
			this.lblProgress.Text = "Time Left:";
			// 
			// cmbActiveScanners
			// 
			this.cmbActiveScanners.DisplayMember = "Name";
			this.cmbActiveScanners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbActiveScanners.FormattingEnabled = true;
			this.cmbActiveScanners.Location = new System.Drawing.Point(6, 155);
			this.cmbActiveScanners.Name = "cmbActiveScanners";
			this.cmbActiveScanners.Size = new System.Drawing.Size(218, 21);
			this.cmbActiveScanners.TabIndex = 13;
			this.cmbActiveScanners.ValueMember = "Name";
			this.cmbActiveScanners.SelectedIndexChanged += new System.EventHandler(this.CmbActiveScannersSelectedIndexChanged);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 136);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(156, 23);
			this.label10.TabIndex = 12;
			this.label10.Text = "last picture that was taken by";
			// 
			// lblTimeToNextScan
			// 
			this.lblTimeToNextScan.Location = new System.Drawing.Point(6, 24);
			this.lblTimeToNextScan.Name = "lblTimeToNextScan";
			this.lblTimeToNextScan.Size = new System.Drawing.Size(219, 23);
			this.lblTimeToNextScan.TabIndex = 8;
			this.lblTimeToNextScan.Text = "Time To Next Scan:";
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(452, 386);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 12;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.BtnExitClick);
			// 
			// UpdateProgressTimer
			// 
			this.UpdateProgressTimer.Interval = 10000;
			this.UpdateProgressTimer.Tick += new System.EventHandler(this.UpdateProgressTimerTick);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.StatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 579);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(543, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 14;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(132, 17);
			this.StatusLabel.Text = "Reading scanners list        ";
			// 
			// EventLogger
			// 
			this.EventLogger.SynchronizingObject = this;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 601);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.gbExperimentStatus);
			this.Controls.Add(this.gbScanningConfiguration);
			this.Controls.Add(this.gbExperimentConfiguration);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Scaning Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Load += new System.EventHandler(this.MainFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.picLastScan)).EndInit();
			this.gbExperimentConfiguration.ResumeLayout(false);
			this.gbExperimentConfiguration.PerformLayout();
			this.gbScanningConfiguration.ResumeLayout(false);
			this.gbScanningConfiguration.PerformLayout();
			this.gbExperimentStatus.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.EventLogger)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Diagnostics.EventLog EventLogger;
		private System.Windows.Forms.Label lblStartGap;
		private System.Windows.Forms.TextBox tbStartGap;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Timer UpdateProgressTimer;
		
		private System.Windows.Forms.ProgressBar progExperimentProgress;
		private System.Windows.Forms.Button   btnScan;
		private System.Windows.Forms.Label  label5;
		private System.Windows.Forms.GroupBox  gbExperimentStatus;
		private System.Windows.Forms.DateTimePicker  dtpEndDateTime;
		private System.Windows.Forms.ProgressBar progTimeToNextScan;
		private System.Windows.Forms.ListBox lbScannersList;
		private System.Windows.Forms.Timer ScanningTimer;
		private System.Windows.Forms.Timer StartTimer;
		private System.Windows.Forms.PictureBox picLastScan;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox cmbActiveScanners;
		private System.Windows.Forms.TextBox tbFileName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox gbScanningConfiguration;
		private System.Windows.Forms.TextBox tbRepetitions;
		private System.Windows.Forms.TextBox tbTimeGap;
		private System.Windows.Forms.DateTimePicker dtpStartDateTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox gbExperimentConfiguration;
		private System.Windows.Forms.Button btnSelectOutputFolder;
		private System.Windows.Forms.FolderBrowserDialog fbdSaveTo;
		private System.Windows.Forms.Label lMinutes;
		private System.Windows.Forms.Label lblTimeToNextScan;
		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.TextBox tbOutputPath;
		private System.Windows.Forms.Button btnStop;
	}
}
