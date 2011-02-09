/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 12/10/2008
 * Time: 10:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TestADU200X2
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
		protected override void Dispose(bool disposing)
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
			this.txtDeviceID1 = new System.Windows.Forms.TextBox();
			this.txtDeviceID2 = new System.Windows.Forms.TextBox();
			this.btnOpen1 = new System.Windows.Forms.Button();
			this.btnOpen2 = new System.Windows.Forms.Button();
			this.ckbl1 = new System.Windows.Forms.CheckedListBox();
			this.ckbl2 = new System.Windows.Forms.CheckedListBox();
			this.btnClose1 = new System.Windows.Forms.Button();
			this.btnClose2 = new System.Windows.Forms.Button();
			this.btnCheckStat1 = new System.Windows.Forms.Button();
			this.btnCheckStat2 = new System.Windows.Forms.Button();
			this.txtStatus1 = new System.Windows.Forms.TextBox();
			this.txtStatus2 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtDeviceID1
			// 
			this.txtDeviceID1.Location = new System.Drawing.Point(30, 13);
			this.txtDeviceID1.Name = "txtDeviceID1";
			this.txtDeviceID1.Size = new System.Drawing.Size(100, 20);
			this.txtDeviceID1.TabIndex = 0;
			this.txtDeviceID1.Text = "A06125";
			// 
			// txtDeviceID2
			// 
			this.txtDeviceID2.Location = new System.Drawing.Point(148, 12);
			this.txtDeviceID2.Name = "txtDeviceID2";
			this.txtDeviceID2.Size = new System.Drawing.Size(100, 20);
			this.txtDeviceID2.TabIndex = 1;
			this.txtDeviceID2.Text = "A06167";
			// 
			// btnOpen1
			// 
			this.btnOpen1.Location = new System.Drawing.Point(30, 38);
			this.btnOpen1.Name = "btnOpen1";
			this.btnOpen1.Size = new System.Drawing.Size(75, 23);
			this.btnOpen1.TabIndex = 2;
			this.btnOpen1.Text = "Open";
			this.btnOpen1.UseVisualStyleBackColor = true;
			this.btnOpen1.Click += new System.EventHandler(this.BtnOpen1Click);
			// 
			// btnOpen2
			// 
			this.btnOpen2.Location = new System.Drawing.Point(148, 38);
			this.btnOpen2.Name = "btnOpen2";
			this.btnOpen2.Size = new System.Drawing.Size(75, 23);
			this.btnOpen2.TabIndex = 3;
			this.btnOpen2.Text = "Open";
			this.btnOpen2.UseVisualStyleBackColor = true;
			this.btnOpen2.Click += new System.EventHandler(this.BtnOpen2Click);
			// 
			// ckbl1
			// 
			this.ckbl1.CheckOnClick = true;
			this.ckbl1.Enabled = false;
			this.ckbl1.FormattingEnabled = true;
			this.ckbl1.Items.AddRange(new object[] {
									"1",
									"2",
									"3",
									"4"});
			this.ckbl1.Location = new System.Drawing.Point(30, 103);
			this.ckbl1.Name = "ckbl1";
			this.ckbl1.Size = new System.Drawing.Size(100, 64);
			this.ckbl1.TabIndex = 8;
			this.ckbl1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Ckbl1ItemCheck);
			// 
			// ckbl2
			// 
			this.ckbl2.CheckOnClick = true;
			this.ckbl2.Enabled = false;
			this.ckbl2.FormattingEnabled = true;
			this.ckbl2.Items.AddRange(new object[] {
									"1",
									"2",
									"3",
									"4"});
			this.ckbl2.Location = new System.Drawing.Point(148, 103);
			this.ckbl2.Name = "ckbl2";
			this.ckbl2.Size = new System.Drawing.Size(100, 64);
			this.ckbl2.TabIndex = 8;
			this.ckbl2.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Ckbl2ItemCheck);
			// 
			// btnClose1
			// 
			this.btnClose1.Location = new System.Drawing.Point(30, 68);
			this.btnClose1.Name = "btnClose1";
			this.btnClose1.Size = new System.Drawing.Size(75, 23);
			this.btnClose1.TabIndex = 9;
			this.btnClose1.Text = "Close";
			this.btnClose1.UseVisualStyleBackColor = true;
			this.btnClose1.Click += new System.EventHandler(this.BtnClose1Click);
			// 
			// btnClose2
			// 
			this.btnClose2.Location = new System.Drawing.Point(148, 67);
			this.btnClose2.Name = "btnClose2";
			this.btnClose2.Size = new System.Drawing.Size(75, 23);
			this.btnClose2.TabIndex = 10;
			this.btnClose2.Text = "Close";
			this.btnClose2.UseVisualStyleBackColor = true;
			this.btnClose2.Click += new System.EventHandler(this.BtnClose2Click);
			// 
			// btnCheckStat1
			// 
			this.btnCheckStat1.Location = new System.Drawing.Point(30, 174);
			this.btnCheckStat1.Name = "btnCheckStat1";
			this.btnCheckStat1.Size = new System.Drawing.Size(100, 23);
			this.btnCheckStat1.TabIndex = 11;
			this.btnCheckStat1.Text = "Check Status";
			this.btnCheckStat1.UseVisualStyleBackColor = true;
			this.btnCheckStat1.Click += new System.EventHandler(this.BtnCheckStat1Click);
			// 
			// btnCheckStat2
			// 
			this.btnCheckStat2.Location = new System.Drawing.Point(148, 174);
			this.btnCheckStat2.Name = "btnCheckStat2";
			this.btnCheckStat2.Size = new System.Drawing.Size(100, 23);
			this.btnCheckStat2.TabIndex = 12;
			this.btnCheckStat2.Text = "Check Status";
			this.btnCheckStat2.UseVisualStyleBackColor = true;
			this.btnCheckStat2.Click += new System.EventHandler(this.BtnCheckStat2Click);
			// 
			// txtStatus1
			// 
			this.txtStatus1.Location = new System.Drawing.Point(30, 203);
			this.txtStatus1.Name = "txtStatus1";
			this.txtStatus1.Size = new System.Drawing.Size(100, 20);
			this.txtStatus1.TabIndex = 13;
			// 
			// txtStatus2
			// 
			this.txtStatus2.Location = new System.Drawing.Point(148, 203);
			this.txtStatus2.Name = "txtStatus2";
			this.txtStatus2.Size = new System.Drawing.Size(100, 20);
			this.txtStatus2.TabIndex = 14;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.txtStatus2);
			this.Controls.Add(this.txtStatus1);
			this.Controls.Add(this.btnCheckStat2);
			this.Controls.Add(this.btnCheckStat1);
			this.Controls.Add(this.btnClose2);
			this.Controls.Add(this.btnClose1);
			this.Controls.Add(this.ckbl2);
			this.Controls.Add(this.ckbl1);
			this.Controls.Add(this.btnOpen2);
			this.Controls.Add(this.btnOpen1);
			this.Controls.Add(this.txtDeviceID2);
			this.Controls.Add(this.txtDeviceID1);
			this.Name = "MainForm";
			this.Text = "TestADU200X2";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox txtStatus2;
		private System.Windows.Forms.TextBox txtStatus1;
		private System.Windows.Forms.Button btnCheckStat2;
		private System.Windows.Forms.Button btnCheckStat1;
		private System.Windows.Forms.Button btnClose2;
		private System.Windows.Forms.Button btnClose1;
		private System.Windows.Forms.CheckedListBox ckbl2;
		private System.Windows.Forms.CheckedListBox ckbl1;
		private System.Windows.Forms.Button btnOpen2;
		private System.Windows.Forms.Button btnOpen1;
		private System.Windows.Forms.TextBox txtDeviceID2;
		private System.Windows.Forms.TextBox txtDeviceID1;
	}
}
