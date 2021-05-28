namespace VideoHub
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ScannerPreview = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advisorLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advisorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startHubDocumentCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopHubDocumentCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanHubDocumentCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.checkBoxSwapMainCamera = new System.Windows.Forms.CheckBox();
            this.numericDocumentCameraIndex = new System.Windows.Forms.NumericUpDown();
            this.btnSettingsSave = new System.Windows.Forms.Button();
            this.btnSettingsCancel = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCommandsFolder = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxScansFolder = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxHubID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMeetingURL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDashboardURL = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxSharepointPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSharepointUsername = new System.Windows.Forms.TextBox();
            this.textBoxSharepointRelativeURL = new System.Windows.Forms.TextBox();
            this.textBoxSharepointURL = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lstSPImages = new System.Windows.Forms.ListBox();
            this.SPStatus = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ScannerPreview)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDocumentCameraIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ScannerPreview
            // 
            this.ScannerPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScannerPreview.Location = new System.Drawing.Point(822, 12);
            this.ScannerPreview.Name = "ScannerPreview";
            this.ScannerPreview.Size = new System.Drawing.Size(400, 712);
            this.ScannerPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ScannerPreview.TabIndex = 2;
            this.ScannerPreview.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.controlHubToolStripMenuItem,
            this.configToolStripMenuItem,
            this.closeImageToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advisorLoginToolStripMenuItem,
            this.advisorToolStripMenuItem,
            this.hubToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // advisorLoginToolStripMenuItem
            // 
            this.advisorLoginToolStripMenuItem.Name = "advisorLoginToolStripMenuItem";
            this.advisorLoginToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.advisorLoginToolStripMenuItem.Text = "Advisor Dashboard";
            this.advisorLoginToolStripMenuItem.Click += new System.EventHandler(this.advisorLoginToolStripMenuItem_Click);
            // 
            // advisorToolStripMenuItem
            // 
            this.advisorToolStripMenuItem.Name = "advisorToolStripMenuItem";
            this.advisorToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.advisorToolStripMenuItem.Text = "Advisor Meeting";
            this.advisorToolStripMenuItem.Click += new System.EventHandler(this.advisorToolStripMenuItem_Click);
            // 
            // hubToolStripMenuItem
            // 
            this.hubToolStripMenuItem.Checked = true;
            this.hubToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hubToolStripMenuItem.Name = "hubToolStripMenuItem";
            this.hubToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.hubToolStripMenuItem.Text = "Hub Meeting";
            this.hubToolStripMenuItem.Click += new System.EventHandler(this.hubToolStripMenuItem_Click);
            // 
            // controlHubToolStripMenuItem
            // 
            this.controlHubToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginHubToolStripMenuItem,
            this.logoutHubToolStripMenuItem,
            this.resetHubToolStripMenuItem,
            this.startHubDocumentCameraToolStripMenuItem,
            this.stopHubDocumentCameraToolStripMenuItem,
            this.scanHubDocumentCameraToolStripMenuItem});
            this.controlHubToolStripMenuItem.Name = "controlHubToolStripMenuItem";
            this.controlHubToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.controlHubToolStripMenuItem.Text = "Control Hub";
            // 
            // loginHubToolStripMenuItem
            // 
            this.loginHubToolStripMenuItem.Enabled = false;
            this.loginHubToolStripMenuItem.Name = "loginHubToolStripMenuItem";
            this.loginHubToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.loginHubToolStripMenuItem.Text = "Login Hub";
            this.loginHubToolStripMenuItem.Click += new System.EventHandler(this.loginHubToolStripMenuItem_Click);
            // 
            // logoutHubToolStripMenuItem
            // 
            this.logoutHubToolStripMenuItem.Enabled = false;
            this.logoutHubToolStripMenuItem.Name = "logoutHubToolStripMenuItem";
            this.logoutHubToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.logoutHubToolStripMenuItem.Text = "Logout Hub";
            this.logoutHubToolStripMenuItem.Click += new System.EventHandler(this.logoutHubToolStripMenuItem_Click);
            // 
            // resetHubToolStripMenuItem
            // 
            this.resetHubToolStripMenuItem.Enabled = false;
            this.resetHubToolStripMenuItem.Name = "resetHubToolStripMenuItem";
            this.resetHubToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.resetHubToolStripMenuItem.Text = "Reset Hub";
            this.resetHubToolStripMenuItem.Click += new System.EventHandler(this.resetHubToolStripMenuItem_Click);
            // 
            // startHubDocumentCameraToolStripMenuItem
            // 
            this.startHubDocumentCameraToolStripMenuItem.Enabled = false;
            this.startHubDocumentCameraToolStripMenuItem.Name = "startHubDocumentCameraToolStripMenuItem";
            this.startHubDocumentCameraToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.startHubDocumentCameraToolStripMenuItem.Text = "Start Hub Document Camera";
            this.startHubDocumentCameraToolStripMenuItem.Click += new System.EventHandler(this.startHubDocumentCameraToolStripMenuItem_Click);
            // 
            // stopHubDocumentCameraToolStripMenuItem
            // 
            this.stopHubDocumentCameraToolStripMenuItem.Enabled = false;
            this.stopHubDocumentCameraToolStripMenuItem.Name = "stopHubDocumentCameraToolStripMenuItem";
            this.stopHubDocumentCameraToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.stopHubDocumentCameraToolStripMenuItem.Text = "Stop Hub Document Camera";
            this.stopHubDocumentCameraToolStripMenuItem.Click += new System.EventHandler(this.stopHubDocumentCameraToolStripMenuItem_Click);
            // 
            // scanHubDocumentCameraToolStripMenuItem
            // 
            this.scanHubDocumentCameraToolStripMenuItem.Enabled = false;
            this.scanHubDocumentCameraToolStripMenuItem.Name = "scanHubDocumentCameraToolStripMenuItem";
            this.scanHubDocumentCameraToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.scanHubDocumentCameraToolStripMenuItem.Text = "Scan Hub Document";
            this.scanHubDocumentCameraToolStripMenuItem.Click += new System.EventHandler(this.scanHubDocumentCameraToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentCameraToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // documentCameraToolStripMenuItem
            // 
            this.documentCameraToolStripMenuItem.Name = "documentCameraToolStripMenuItem";
            this.documentCameraToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.documentCameraToolStripMenuItem.Text = "Document Camera 0";
            this.documentCameraToolStripMenuItem.Click += new System.EventHandler(this.documentCameraToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // closeImageToolStripMenuItem
            // 
            this.closeImageToolStripMenuItem.Enabled = false;
            this.closeImageToolStripMenuItem.Name = "closeImageToolStripMenuItem";
            this.closeImageToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.closeImageToolStripMenuItem.Text = "Close Image";
            this.closeImageToolStripMenuItem.Click += new System.EventHandler(this.closeImageToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.Controls.Add(this.checkBoxSwapMainCamera);
            this.SettingsPanel.Controls.Add(this.numericDocumentCameraIndex);
            this.SettingsPanel.Controls.Add(this.btnSettingsSave);
            this.SettingsPanel.Controls.Add(this.btnSettingsCancel);
            this.SettingsPanel.Controls.Add(this.label12);
            this.SettingsPanel.Controls.Add(this.label11);
            this.SettingsPanel.Controls.Add(this.label10);
            this.SettingsPanel.Controls.Add(this.textBoxCommandsFolder);
            this.SettingsPanel.Controls.Add(this.label9);
            this.SettingsPanel.Controls.Add(this.textBoxScansFolder);
            this.SettingsPanel.Controls.Add(this.label8);
            this.SettingsPanel.Controls.Add(this.textBoxHubID);
            this.SettingsPanel.Controls.Add(this.label7);
            this.SettingsPanel.Controls.Add(this.textBoxMeetingURL);
            this.SettingsPanel.Controls.Add(this.label6);
            this.SettingsPanel.Controls.Add(this.textBoxDashboardURL);
            this.SettingsPanel.Controls.Add(this.label5);
            this.SettingsPanel.Controls.Add(this.textBoxSharepointPassword);
            this.SettingsPanel.Controls.Add(this.label4);
            this.SettingsPanel.Controls.Add(this.label3);
            this.SettingsPanel.Controls.Add(this.label2);
            this.SettingsPanel.Controls.Add(this.label1);
            this.SettingsPanel.Controls.Add(this.textBoxSharepointUsername);
            this.SettingsPanel.Controls.Add(this.textBoxSharepointRelativeURL);
            this.SettingsPanel.Controls.Add(this.textBoxSharepointURL);
            this.SettingsPanel.Location = new System.Drawing.Point(0, 27);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(354, 362);
            this.SettingsPanel.TabIndex = 6;
            // 
            // checkBoxSwapMainCamera
            // 
            this.checkBoxSwapMainCamera.AutoSize = true;
            this.checkBoxSwapMainCamera.Location = new System.Drawing.Point(161, 194);
            this.checkBoxSwapMainCamera.Name = "checkBoxSwapMainCamera";
            this.checkBoxSwapMainCamera.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSwapMainCamera.TabIndex = 26;
            this.checkBoxSwapMainCamera.UseVisualStyleBackColor = true;
            // 
            // numericDocumentCameraIndex
            // 
            this.numericDocumentCameraIndex.Location = new System.Drawing.Point(161, 164);
            this.numericDocumentCameraIndex.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericDocumentCameraIndex.Name = "numericDocumentCameraIndex";
            this.numericDocumentCameraIndex.Size = new System.Drawing.Size(120, 20);
            this.numericDocumentCameraIndex.TabIndex = 25;
            // 
            // btnSettingsSave
            // 
            this.btnSettingsSave.Location = new System.Drawing.Point(197, 333);
            this.btnSettingsSave.Name = "btnSettingsSave";
            this.btnSettingsSave.Size = new System.Drawing.Size(88, 19);
            this.btnSettingsSave.TabIndex = 24;
            this.btnSettingsSave.Text = "Save";
            this.btnSettingsSave.UseVisualStyleBackColor = true;
            this.btnSettingsSave.Click += new System.EventHandler(this.btnSettingsSave_Click);
            // 
            // btnSettingsCancel
            // 
            this.btnSettingsCancel.Location = new System.Drawing.Point(50, 333);
            this.btnSettingsCancel.Name = "btnSettingsCancel";
            this.btnSettingsCancel.Size = new System.Drawing.Size(88, 19);
            this.btnSettingsCancel.TabIndex = 23;
            this.btnSettingsCancel.Text = "Cancel";
            this.btnSettingsCancel.UseVisualStyleBackColor = true;
            this.btnSettingsCancel.Click += new System.EventHandler(this.btnSettingsCancel_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(135, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Settings";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 197);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Swap Main Camera";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 146);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Commands Folder";
            // 
            // textBoxCommandsFolder
            // 
            this.textBoxCommandsFolder.Location = new System.Drawing.Point(161, 139);
            this.textBoxCommandsFolder.Name = "textBoxCommandsFolder";
            this.textBoxCommandsFolder.ReadOnly = true;
            this.textBoxCommandsFolder.Size = new System.Drawing.Size(174, 20);
            this.textBoxCommandsFolder.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Scans Folder";
            // 
            // textBoxScansFolder
            // 
            this.textBoxScansFolder.Location = new System.Drawing.Point(161, 113);
            this.textBoxScansFolder.Name = "textBoxScansFolder";
            this.textBoxScansFolder.ReadOnly = true;
            this.textBoxScansFolder.Size = new System.Drawing.Size(174, 20);
            this.textBoxScansFolder.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "HubID";
            // 
            // textBoxHubID
            // 
            this.textBoxHubID.Location = new System.Drawing.Point(161, 87);
            this.textBoxHubID.Name = "textBoxHubID";
            this.textBoxHubID.Size = new System.Drawing.Size(174, 20);
            this.textBoxHubID.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Meeting URL";
            // 
            // textBoxMeetingURL
            // 
            this.textBoxMeetingURL.Location = new System.Drawing.Point(161, 35);
            this.textBoxMeetingURL.Name = "textBoxMeetingURL";
            this.textBoxMeetingURL.Size = new System.Drawing.Size(174, 20);
            this.textBoxMeetingURL.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Dashboard URL";
            // 
            // textBoxDashboardURL
            // 
            this.textBoxDashboardURL.Location = new System.Drawing.Point(161, 61);
            this.textBoxDashboardURL.Name = "textBoxDashboardURL";
            this.textBoxDashboardURL.ReadOnly = true;
            this.textBoxDashboardURL.Size = new System.Drawing.Size(174, 20);
            this.textBoxDashboardURL.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Document Camera Index";
            // 
            // textBoxSharepointPassword
            // 
            this.textBoxSharepointPassword.Location = new System.Drawing.Point(161, 295);
            this.textBoxSharepointPassword.Name = "textBoxSharepointPassword";
            this.textBoxSharepointPassword.Size = new System.Drawing.Size(174, 20);
            this.textBoxSharepointPassword.TabIndex = 7;
            this.toolTip1.SetToolTip(this.textBoxSharepointPassword, "Minimum 7 characters");
            this.textBoxSharepointPassword.Enter += new System.EventHandler(this.textBoxSharepointPassword_Enter);
            this.textBoxSharepointPassword.Leave += new System.EventHandler(this.textBoxSharepointPassword_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 302);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sharepoint Password";
            this.toolTip1.SetToolTip(this.label4, "Minimum 7 characters");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Sharepoint Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 250);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sharepoint Relative URL";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sharepoint URL";
            // 
            // textBoxSharepointUsername
            // 
            this.textBoxSharepointUsername.Location = new System.Drawing.Point(161, 269);
            this.textBoxSharepointUsername.Name = "textBoxSharepointUsername";
            this.textBoxSharepointUsername.Size = new System.Drawing.Size(174, 20);
            this.textBoxSharepointUsername.TabIndex = 2;
            // 
            // textBoxSharepointRelativeURL
            // 
            this.textBoxSharepointRelativeURL.Location = new System.Drawing.Point(161, 243);
            this.textBoxSharepointRelativeURL.Name = "textBoxSharepointRelativeURL";
            this.textBoxSharepointRelativeURL.ReadOnly = true;
            this.textBoxSharepointRelativeURL.Size = new System.Drawing.Size(174, 20);
            this.textBoxSharepointRelativeURL.TabIndex = 1;
            // 
            // textBoxSharepointURL
            // 
            this.textBoxSharepointURL.Location = new System.Drawing.Point(161, 217);
            this.textBoxSharepointURL.Name = "textBoxSharepointURL";
            this.textBoxSharepointURL.ReadOnly = true;
            this.textBoxSharepointURL.Size = new System.Drawing.Size(174, 20);
            this.textBoxSharepointURL.TabIndex = 0;
            // 
            // lstSPImages
            // 
            this.lstSPImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSPImages.FormattingEnabled = true;
            this.lstSPImages.HorizontalScrollbar = true;
            this.lstSPImages.ItemHeight = 20;
            this.lstSPImages.Location = new System.Drawing.Point(590, 130);
            this.lstSPImages.Name = "lstSPImages";
            this.lstSPImages.Size = new System.Drawing.Size(226, 164);
            this.lstSPImages.TabIndex = 7;
            this.lstSPImages.Visible = false;
            this.lstSPImages.DoubleClick += new System.EventHandler(this.lstSPImages_DoubleClick);
            this.lstSPImages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstSPImages_Click);
            // 
            // SPStatus
            // 
            this.SPStatus.AutoSize = true;
            this.SPStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SPStatus.ForeColor = System.Drawing.Color.Red;
            this.SPStatus.Location = new System.Drawing.Point(380, 0);
            this.SPStatus.Name = "SPStatus";
            this.SPStatus.Size = new System.Drawing.Size(311, 20);
            this.SPStatus.TabIndex = 8;
            this.SPStatus.Text = "Sharepoint Offline - check your credentials!";
            this.SPStatus.Visible = false;
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(726, 4);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(266, 246);
            this.txtError.TabIndex = 9;
            this.txtError.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 761);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.SPStatus);
            this.Controls.Add(this.lstSPImages);
            this.Controls.Add(this.SettingsPanel);
            this.Controls.Add(this.ScannerPreview);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "VideoHub";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ScannerPreview)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDocumentCameraIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox ScannerPreview;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advisorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advisorLoginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlHubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginHubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutHubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startHubDocumentCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopHubDocumentCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanHubDocumentCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetHubToolStripMenuItem;
        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSharepointUsername;
        private System.Windows.Forms.TextBox textBoxSharepointRelativeURL;
        private System.Windows.Forms.TextBox textBoxSharepointURL;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxCommandsFolder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxScansFolder;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxHubID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMeetingURL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDashboardURL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxSharepointPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSettingsSave;
        private System.Windows.Forms.Button btnSettingsCancel;
        private System.Windows.Forms.CheckBox checkBoxSwapMainCamera;
        private System.Windows.Forms.NumericUpDown numericDocumentCameraIndex;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListBox lstSPImages;
        private System.Windows.Forms.ToolStripMenuItem closeImageToolStripMenuItem;
        private System.Windows.Forms.Label SPStatus;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.TextBox txtError;
    }
}

