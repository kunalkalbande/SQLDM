namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    partial class ResourcesFileActivity
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.operationalStatusPanel = new System.Windows.Forms.Panel();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new System.Windows.Forms.Label();
            this.msgPanel = new System.Windows.Forms.Panel();
            this.msgLabel = new System.Windows.Forms.Label();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbSortDescending = new System.Windows.Forms.RadioButton();
            this.rbSortAscending = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbSortByDatabasename = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.rbSortbyFilename = new System.Windows.Forms.RadioButton();
            this.rbSortbyReads = new System.Windows.Forms.RadioButton();
            this.rbSortbyWrites = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbFiletypeOther = new System.Windows.Forms.CheckBox();
            this.cbFiletypeLog = new System.Windows.Forms.CheckBox();
            this.cbFiletypeData = new System.Windows.Forms.CheckBox();
            this.queryMonitorFiltersHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.hideFilterButton = new System.Windows.Forms.ToolStripButton();
            this.clearQueryMonitorFiltersButton = new System.Windows.Forms.ToolStripButton();
            this.useWildcardLabel = new System.Windows.Forms.ToolStripLabel();
            this.databasesComboBox = new Idera.SQLdm.DesktopClient.Controls.CheckedComboBox();
            this.drivesComboBox = new Idera.SQLdm.DesktopClient.Controls.CheckedComboBox();
            this.filepathLike = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.filenameLike = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fileActivityPanel1 = new Idera.SQLdm.DesktopClient.Controls.FileActivityPanel();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            this.msgPanel.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.queryMonitorFiltersHeaderStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(973, 24);
            this.operationalStatusPanel.TabIndex = 19;
            this.operationalStatusPanel.Visible = false;
            // 
            // operationalStatusImage
            // 
            this.operationalStatusImage.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.operationalStatusImage.Location = new System.Drawing.Point(4, 3);
            this.operationalStatusImage.Name = "operationalStatusImage";
            this.operationalStatusImage.Size = new System.Drawing.Size(16, 16);
            this.operationalStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.operationalStatusImage.TabIndex = 5;
            this.operationalStatusImage.TabStop = false;
            // 
            // operationalStatusLabel
            // 
            this.operationalStatusLabel.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.operationalStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusLabel.Name = "operationalStatusLabel";
            this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.operationalStatusLabel.Size = new System.Drawing.Size(973, 24);
            this.operationalStatusLabel.TabIndex = 4;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // msgPanel
            // 
            this.msgPanel.Controls.Add(this.msgLabel);
            this.msgPanel.Location = new System.Drawing.Point(488, 146);
            this.msgPanel.Name = "msgPanel";
            this.msgPanel.Size = new System.Drawing.Size(146, 66);
            this.msgPanel.TabIndex = 9;
            // 
            // msgLabel
            // 
            this.msgLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msgLabel.Location = new System.Drawing.Point(0, 0);
            this.msgLabel.Name = "msgLabel";
            this.msgLabel.Size = new System.Drawing.Size(146, 66);
            this.msgLabel.TabIndex = 0;
            this.msgLabel.Text = "Please wait...";
            this.msgLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // filterPanel
            // 
            this.filterPanel.AutoScroll = true;
            this.filterPanel.Controls.Add(this.panel2);
            this.filterPanel.Controls.Add(this.panel1);
            this.filterPanel.Controls.Add(this.label8);
            this.filterPanel.Controls.Add(this.label6);
            this.filterPanel.Controls.Add(this.cbFiletypeOther);
            this.filterPanel.Controls.Add(this.cbFiletypeLog);
            this.filterPanel.Controls.Add(this.cbFiletypeData);
            this.filterPanel.Controls.Add(this.queryMonitorFiltersHeaderStrip);
            this.filterPanel.Controls.Add(this.databasesComboBox);
            this.filterPanel.Controls.Add(this.drivesComboBox);
            this.filterPanel.Controls.Add(this.filepathLike);
            this.filterPanel.Controls.Add(this.label5);
            this.filterPanel.Controls.Add(this.filenameLike);
            this.filterPanel.Controls.Add(this.label4);
            this.filterPanel.Controls.Add(this.label3);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 24);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(973, 118);
            this.filterPanel.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.rbSortDescending);
            this.panel2.Controls.Add(this.rbSortAscending);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(794, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(169, 89);
            this.panel2.TabIndex = 26;
            // 
            // rbSortDescending
            // 
            this.rbSortDescending.AutoSize = true;
            this.rbSortDescending.Location = new System.Drawing.Point(81, 30);
            this.rbSortDescending.Name = "rbSortDescending";
            this.rbSortDescending.Size = new System.Drawing.Size(82, 17);
            this.rbSortDescending.TabIndex = 26;
            this.rbSortDescending.Text = "Descending";
            this.rbSortDescending.UseVisualStyleBackColor = true;
            this.rbSortDescending.CheckedChanged += new System.EventHandler(this.rbSortDirection_CheckedChanged);
            // 
            // rbSortAscending
            // 
            this.rbSortAscending.AutoSize = true;
            this.rbSortAscending.Checked = true;
            this.rbSortAscending.Location = new System.Drawing.Point(81, 6);
            this.rbSortAscending.Name = "rbSortAscending";
            this.rbSortAscending.Size = new System.Drawing.Size(75, 17);
            this.rbSortAscending.TabIndex = 25;
            this.rbSortAscending.TabStop = true;
            this.rbSortAscending.Text = "Ascending";
            this.rbSortAscending.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Sort Direction:";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.rbSortByDatabasename);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.rbSortbyFilename);
            this.panel1.Controls.Add(this.rbSortbyReads);
            this.panel1.Controls.Add(this.rbSortbyWrites);
            this.panel1.Location = new System.Drawing.Point(628, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(159, 89);
            this.panel1.TabIndex = 25;
            // 
            // rbSortByDatabasename
            // 
            this.rbSortByDatabasename.AutoSize = true;
            this.rbSortByDatabasename.Checked = true;
            this.rbSortByDatabasename.Location = new System.Drawing.Point(54, 5);
            this.rbSortByDatabasename.Name = "rbSortByDatabasename";
            this.rbSortByDatabasename.Size = new System.Drawing.Size(102, 17);
            this.rbSortByDatabasename.TabIndex = 23;
            this.rbSortByDatabasename.TabStop = true;
            this.rbSortByDatabasename.Text = "Database Name";
            this.rbSortByDatabasename.UseVisualStyleBackColor = true;
            this.rbSortByDatabasename.CheckedChanged += new System.EventHandler(this.rbSortby_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Sort By:";
            // 
            // rbSortbyFilename
            // 
            this.rbSortbyFilename.AutoSize = true;
            this.rbSortbyFilename.Location = new System.Drawing.Point(54, 25);
            this.rbSortbyFilename.Name = "rbSortbyFilename";
            this.rbSortbyFilename.Size = new System.Drawing.Size(72, 17);
            this.rbSortbyFilename.TabIndex = 19;
            this.rbSortbyFilename.Text = "File Name";
            this.rbSortbyFilename.UseVisualStyleBackColor = true;
            this.rbSortbyFilename.CheckedChanged += new System.EventHandler(this.rbSortby_CheckedChanged);
            // 
            // rbSortbyReads
            // 
            this.rbSortbyReads.AutoSize = true;
            this.rbSortbyReads.Location = new System.Drawing.Point(54, 46);
            this.rbSortbyReads.Name = "rbSortbyReads";
            this.rbSortbyReads.Size = new System.Drawing.Size(86, 17);
            this.rbSortbyReads.TabIndex = 20;
            this.rbSortbyReads.Text = "Reads / Sec";
            this.rbSortbyReads.UseVisualStyleBackColor = true;
            this.rbSortbyReads.CheckedChanged += new System.EventHandler(this.rbSortby_CheckedChanged);
            // 
            // rbSortbyWrites
            // 
            this.rbSortbyWrites.AutoSize = true;
            this.rbSortbyWrites.Location = new System.Drawing.Point(54, 67);
            this.rbSortbyWrites.Name = "rbSortbyWrites";
            this.rbSortbyWrites.Size = new System.Drawing.Size(85, 17);
            this.rbSortbyWrites.TabIndex = 21;
            this.rbSortbyWrites.Text = "Writes / Sec";
            this.rbSortbyWrites.UseVisualStyleBackColor = true;
            this.rbSortbyWrites.CheckedChanged += new System.EventHandler(this.rbSortby_CheckedChanged);
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.label8.Location = new System.Drawing.Point(625, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(1, 59);
            this.label8.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.label6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label6.Location = new System.Drawing.Point(0, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(973, 1);
            this.label6.TabIndex = 18;
            // 
            // cbFiletypeOther
            // 
            this.cbFiletypeOther.AutoSize = true;
            this.cbFiletypeOther.Location = new System.Drawing.Point(573, 74);
            this.cbFiletypeOther.Name = "cbFiletypeOther";
            this.cbFiletypeOther.Size = new System.Drawing.Size(52, 17);
            this.cbFiletypeOther.TabIndex = 17;
            this.cbFiletypeOther.Text = "Other";
            this.cbFiletypeOther.UseVisualStyleBackColor = true;
            this.cbFiletypeOther.CheckedChanged += new System.EventHandler(this.cbFiletypeOther_CheckedChanged);
            // 
            // cbFiletypeLog
            // 
            this.cbFiletypeLog.AutoSize = true;
            this.cbFiletypeLog.Location = new System.Drawing.Point(573, 53);
            this.cbFiletypeLog.Name = "cbFiletypeLog";
            this.cbFiletypeLog.Size = new System.Drawing.Size(44, 17);
            this.cbFiletypeLog.TabIndex = 16;
            this.cbFiletypeLog.Text = "Log";
            this.cbFiletypeLog.UseVisualStyleBackColor = true;
            this.cbFiletypeLog.CheckedChanged += new System.EventHandler(this.cbFiletypeLog_CheckedChanged);
            // 
            // cbFiletypeData
            // 
            this.cbFiletypeData.AutoSize = true;
            this.cbFiletypeData.Location = new System.Drawing.Point(573, 32);
            this.cbFiletypeData.Name = "cbFiletypeData";
            this.cbFiletypeData.Size = new System.Drawing.Size(49, 17);
            this.cbFiletypeData.TabIndex = 15;
            this.cbFiletypeData.Text = "Data";
            this.cbFiletypeData.UseVisualStyleBackColor = true;
            this.cbFiletypeData.CheckedChanged += new System.EventHandler(this.cbFiletypeData_CheckedChanged);
            // 
            // queryMonitorFiltersHeaderStrip
            // 
            this.queryMonitorFiltersHeaderStrip.AutoSize = false;
            this.queryMonitorFiltersHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.queryMonitorFiltersHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.queryMonitorFiltersHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.queryMonitorFiltersHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.queryMonitorFiltersHeaderStrip.HotTrackEnabled = false;
            this.queryMonitorFiltersHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.hideFilterButton,
            this.clearQueryMonitorFiltersButton,
            this.useWildcardLabel});
            this.queryMonitorFiltersHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorFiltersHeaderStrip.Name = "queryMonitorFiltersHeaderStrip";
            this.queryMonitorFiltersHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.queryMonitorFiltersHeaderStrip.Size = new System.Drawing.Size(973, 19);
            this.queryMonitorFiltersHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.queryMonitorFiltersHeaderStrip.TabIndex = 14;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(42, 16);
            this.toolStripLabel2.Text = "Filters";
            // 
            // hideFilterButton
            // 
            this.hideFilterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideFilterButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.hideFilterButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.hideFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideFilterButton.Name = "hideFilterButton";
            this.hideFilterButton.Size = new System.Drawing.Size(23, 16);
            this.hideFilterButton.ToolTipText = "Hide Filter";
            this.hideFilterButton.Click += new System.EventHandler(this.hideFilterButton_Click);
            // 
            // clearQueryMonitorFiltersButton
            // 
            this.clearQueryMonitorFiltersButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearQueryMonitorFiltersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearQueryMonitorFiltersButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.funnel_delete;
            this.clearQueryMonitorFiltersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearQueryMonitorFiltersButton.Name = "clearQueryMonitorFiltersButton";
            this.clearQueryMonitorFiltersButton.Size = new System.Drawing.Size(23, 16);
            this.clearQueryMonitorFiltersButton.Text = "Reset Filter";
            this.clearQueryMonitorFiltersButton.Click += new System.EventHandler(this.clearFilterButton_Click);
            // 
            // useWildcardLabel
            // 
            this.useWildcardLabel.Font = new System.Drawing.Font("Segoe UI", 6.25F);
            this.useWildcardLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.useWildcardLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.useWildcardLabel.Name = "useWildcardLabel";
            this.useWildcardLabel.Size = new System.Drawing.Size(80, 15);
            this.useWildcardLabel.Text = "(use % as wildcard)";
            // 
            // databasesComboBox
            // 
            this.databasesComboBox.CheckOnClick = true;
            this.databasesComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.databasesComboBox.DropDownHeaderText = "Database Name";
            this.databasesComboBox.DropDownHeight = 1;
            this.databasesComboBox.DropDownWidth = 500;
            this.databasesComboBox.FormattingEnabled = true;
            this.databasesComboBox.IntegralHeight = false;
            this.databasesComboBox.Location = new System.Drawing.Point(63, 63);
            this.databasesComboBox.Name = "databasesComboBox";
            this.databasesComboBox.Size = new System.Drawing.Size(185, 21);
            this.databasesComboBox.TabIndex = 12;
            this.databasesComboBox.ValueSeparator = ", ";
            this.databasesComboBox.DropDownClosed += new System.EventHandler(this.databasesComboBox_DropDownClosed);
            // 
            // drivesComboBox
            // 
            this.drivesComboBox.CheckOnClick = true;
            this.drivesComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.drivesComboBox.DropDownHeaderText = "Drive Name";
            this.drivesComboBox.DropDownHeight = 1;
            this.drivesComboBox.DropDownWidth = 300;
            this.drivesComboBox.FormattingEnabled = true;
            this.drivesComboBox.IntegralHeight = false;
            this.drivesComboBox.Location = new System.Drawing.Point(63, 31);
            this.drivesComboBox.Name = "drivesComboBox";
            this.drivesComboBox.Size = new System.Drawing.Size(185, 21);
            this.drivesComboBox.TabIndex = 11;
            this.drivesComboBox.ValueSeparator = ", ";
            this.drivesComboBox.DropDownClosed += new System.EventHandler(this.drivesComboBox_DropDownClosed);
            // 
            // filepathLike
            // 
            this.filepathLike.AcceptsReturn = true;
            this.filepathLike.Location = new System.Drawing.Point(340, 31);
            this.filepathLike.Name = "filepathLike";
            this.filepathLike.Size = new System.Drawing.Size(164, 20);
            this.filepathLike.TabIndex = 10;
            this.filepathLike.TextChanged += new System.EventHandler(this.filepathLike_TextChanged);
            this.filepathLike.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filepathLike_KeyUp);
            this.filepathLike.Leave += new System.EventHandler(this.filepathLike_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(254, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "File Path Like:";
            // 
            // filenameLike
            // 
            this.filenameLike.AcceptsReturn = true;
            this.filenameLike.Location = new System.Drawing.Point(340, 63);
            this.filenameLike.Name = "filenameLike";
            this.filenameLike.Size = new System.Drawing.Size(164, 20);
            this.filenameLike.TabIndex = 8;
            this.filenameLike.TextChanged += new System.EventHandler(this.filenameLike_TextChanged);
            this.filenameLike.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filenameLike_KeyUp);
            this.filenameLike.Leave += new System.EventHandler(this.filenameLike_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(254, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "File Name Like:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(511, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "File Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drive:";
            // 
            // fileActivityPanel1
            // 
            this.fileActivityPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileActivityPanel1.InstanceId = 0;
            this.fileActivityPanel1.Location = new System.Drawing.Point(0, 142);
            this.fileActivityPanel1.Name = "fileActivityPanel1";
            this.fileActivityPanel1.Size = new System.Drawing.Size(973, 601);
            this.fileActivityPanel1.TabIndex = 21;
            // 
            // ResourcesFileActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.fileActivityPanel1);
            this.Controls.Add(this.filterPanel);
            this.Controls.Add(this.operationalStatusPanel);
            this.Controls.Add(this.msgPanel);
            this.DoubleBuffered = true;
            this.Name = "ResourcesFileActivity";
            this.Size = new System.Drawing.Size(973, 743);
            this.Load += new System.EventHandler(this.ResourcesFileActivity_Load);
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.msgPanel.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.queryMonitorFiltersHeaderStrip.ResumeLayout(false);
            this.queryMonitorFiltersHeaderStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private System.Windows.Forms.Label operationalStatusLabel;
        private System.Windows.Forms.Panel msgPanel;
        private System.Windows.Forms.Label msgLabel;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox filenameLike;
        private System.Windows.Forms.TextBox filepathLike;
        private System.Windows.Forms.Label label5;
        private Idera.SQLdm.DesktopClient.Controls.CheckedComboBox databasesComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CheckedComboBox drivesComboBox;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip queryMonitorFiltersHeaderStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton hideFilterButton;
        private System.Windows.Forms.ToolStripButton clearQueryMonitorFiltersButton;
        private System.Windows.Forms.ToolStripLabel useWildcardLabel;
        private System.Windows.Forms.CheckBox cbFiletypeData;
        private System.Windows.Forms.CheckBox cbFiletypeOther;
        private System.Windows.Forms.CheckBox cbFiletypeLog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbSortbyWrites;
        private System.Windows.Forms.RadioButton rbSortbyReads;
        private System.Windows.Forms.RadioButton rbSortbyFilename;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private Idera.SQLdm.DesktopClient.Controls.FileActivityPanel fileActivityPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbSortDescending;
        private System.Windows.Forms.RadioButton rbSortAscending;
        private System.Windows.Forms.RadioButton rbSortByDatabasename;

    }
}
