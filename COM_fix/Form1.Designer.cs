namespace COM_fix
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnScan = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            btnFix = new Button();
            btnOpenDevMgr = new Button();
            cmbPorts = new ComboBox();
            cmbBaudRate = new ComboBox();
            btnConnect = new Button();
            groupBox1 = new GroupBox();
            btnCleanComMap = new Button();
            btnRestartDevice = new Button();
            btnForceKill = new Button();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            btnElevate_Click = new Button();
            lblDeviceInfor = new Label();
            listLog = new RichTextBox();
            chkHexMode = new CheckBox();
            chkPauseScroll = new CheckBox();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnScan
            // 
            btnScan.Cursor = Cursors.Hand;
            btnScan.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 238);
            btnScan.Location = new Point(12, 24);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(200, 30);
            btnScan.TabIndex = 0;
            btnScan.Text = "Scan USB-COM port";
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += btnScan_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 439);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(834, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(819, 17);
            toolStripStatusLabel1.Spring = true;
            toolStripStatusLabel1.Text = "System Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 18);
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.Visible = false;
            // 
            // btnFix
            // 
            btnFix.Cursor = Cursors.Hand;
            btnFix.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 238);
            btnFix.Location = new Point(12, 60);
            btnFix.Name = "btnFix";
            btnFix.Size = new Size(200, 30);
            btnFix.TabIndex = 3;
            btnFix.Text = "FIX";
            btnFix.UseVisualStyleBackColor = true;
            btnFix.Click += btnFix_Click;
            // 
            // btnOpenDevMgr
            // 
            btnOpenDevMgr.Location = new Point(12, 96);
            btnOpenDevMgr.Name = "btnOpenDevMgr";
            btnOpenDevMgr.Size = new Size(200, 30);
            btnOpenDevMgr.TabIndex = 4;
            btnOpenDevMgr.Text = "Open Device Manager";
            btnOpenDevMgr.UseVisualStyleBackColor = true;
            btnOpenDevMgr.Click += btnOpenDevMgr_Click;
            // 
            // cmbPorts
            // 
            cmbPorts.FormattingEnabled = true;
            cmbPorts.Location = new Point(6, 31);
            cmbPorts.Name = "cmbPorts";
            cmbPorts.Size = new Size(151, 25);
            cmbPorts.TabIndex = 5;
            cmbPorts.Text = "Ports";
            // 
            // cmbBaudRate
            // 
            cmbBaudRate.FormattingEnabled = true;
            cmbBaudRate.Items.AddRange(new object[] { "9600", "115200" });
            cmbBaudRate.Location = new Point(163, 31);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new Size(151, 25);
            cmbBaudRate.TabIndex = 6;
            cmbBaudRate.Text = "115200";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(330, 31);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(178, 25);
            btnConnect.TabIndex = 7;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnCleanComMap);
            groupBox1.Controls.Add(btnRestartDevice);
            groupBox1.Controls.Add(btnForceKill);
            groupBox1.Controls.Add(btnFix);
            groupBox1.Controls.Add(btnScan);
            groupBox1.Controls.Add(btnOpenDevMgr);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(227, 253);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "System & Hardware Control";
            // 
            // btnCleanComMap
            // 
            btnCleanComMap.Location = new Point(12, 204);
            btnCleanComMap.Name = "btnCleanComMap";
            btnCleanComMap.Size = new Size(200, 30);
            btnCleanComMap.TabIndex = 7;
            btnCleanComMap.Text = " Clear COM";
            btnCleanComMap.UseVisualStyleBackColor = true;
            btnCleanComMap.Click += btnResetCom_Click;
            // 
            // btnRestartDevice
            // 
            btnRestartDevice.Location = new Point(12, 168);
            btnRestartDevice.Name = "btnRestartDevice";
            btnRestartDevice.Size = new Size(200, 30);
            btnRestartDevice.TabIndex = 6;
            btnRestartDevice.Text = "Restart USB Device";
            btnRestartDevice.UseVisualStyleBackColor = true;
            btnRestartDevice.Click += btnRestartDevice_Click;
            // 
            // btnForceKill
            // 
            btnForceKill.Location = new Point(12, 132);
            btnForceKill.Name = "btnForceKill";
            btnForceKill.Size = new Size(200, 30);
            btnForceKill.TabIndex = 5;
            btnForceKill.Text = "Kill Port Lockers";
            btnForceKill.UseVisualStyleBackColor = true;
            btnForceKill.Click += btnForceKill_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cmbPorts);
            groupBox2.Controls.Add(cmbBaudRate);
            groupBox2.Controls.Add(btnConnect);
            groupBox2.Location = new Point(256, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(566, 66);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Serial Terminal Settings";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnElevate_Click);
            groupBox3.Location = new Point(256, 84);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(323, 181);
            groupBox3.TabIndex = 10;
            groupBox3.TabStop = false;
            groupBox3.Text = "Admin Control";
            // 
            // btnElevate_Click
            // 
            btnElevate_Click.Image = (Image)resources.GetObject("btnElevate_Click.Image");
            btnElevate_Click.ImageAlign = ContentAlignment.MiddleRight;
            btnElevate_Click.Location = new Point(7, 24);
            btnElevate_Click.Name = "btnElevate_Click";
            btnElevate_Click.Size = new Size(150, 30);
            btnElevate_Click.TabIndex = 0;
            btnElevate_Click.Text = "Run as Admin";
            btnElevate_Click.UseVisualStyleBackColor = true;
            btnElevate_Click.Click += btnRunAsAdmin_Click;
            // 
            // lblDeviceInfor
            // 
            lblDeviceInfor.AutoSize = true;
            lblDeviceInfor.Location = new Point(596, 97);
            lblDeviceInfor.Name = "lblDeviceInfor";
            lblDeviceInfor.Size = new Size(45, 19);
            lblDeviceInfor.TabIndex = 11;
            lblDeviceInfor.Text = "label1";
            // 
            // listLog
            // 
            listLog.BackColor = SystemColors.Window;
            listLog.BorderStyle = BorderStyle.None;
            listLog.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 238);
            listLog.Location = new Point(12, 271);
            listLog.Name = "listLog";
            listLog.ReadOnly = true;
            listLog.Size = new Size(810, 159);
            listLog.TabIndex = 12;
            listLog.Text = "";
            // 
            // chkHexMode
            // 
            chkHexMode.AutoSize = true;
            chkHexMode.Font = new Font("Segoe UI", 10F);
            chkHexMode.Location = new Point(586, 84);
            chkHexMode.Name = "chkHexMode";
            chkHexMode.Size = new Size(93, 23);
            chkHexMode.TabIndex = 13;
            chkHexMode.Text = "HEX Mode";
            chkHexMode.UseVisualStyleBackColor = true;
            // 
            // chkPauseScroll
            // 
            chkPauseScroll.AutoSize = true;
            chkPauseScroll.Location = new Point(585, 108);
            chkPauseScroll.Name = "chkPauseScroll";
            chkPauseScroll.Size = new Size(100, 23);
            chkPauseScroll.TabIndex = 14;
            chkPauseScroll.Text = "Pause Scroll";
            chkPauseScroll.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 461);
            Controls.Add(chkPauseScroll);
            Controls.Add(chkHexMode);
            Controls.Add(listLog);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(statusStrip1);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "COM_fix Universal Rescue Tool";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnScan;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private Button btnFix;
        private Button btnOpenDevMgr;
        private ComboBox cmbPorts;
        private ComboBox cmbBaudRate;
        private Button btnConnect;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Button btnCleanComMap;
        private Button btnRestartDevice;
        private Button btnForceKill;
        private Label lblDeviceInfor;
        private RichTextBox listLog;
        private Button btnElevate_Click;
        private CheckBox chkHexMode;
        private CheckBox chkPauseScroll;
    }
}
