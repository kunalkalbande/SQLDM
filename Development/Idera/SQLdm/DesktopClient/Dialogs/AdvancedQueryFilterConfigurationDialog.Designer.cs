namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AdvancedQueryFilterConfigurationDialog
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.propertiesControl1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesControl(isDarkThemeSelected);
            this.propertyPage1 = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.chkExcludeDM = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip4 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.rowcountPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.rowcountLimited = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.rowcountUnlimited = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.rowcountUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.propertiesHeaderStrip3 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.rowcountHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sqlTextFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.sqlTextIncludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.sqlTextExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databasesFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databasesIncludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.databasesExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.applicationsFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.pnlApplication = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.applicationsIncludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.applicationExcludeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.applicationsExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertyPage1.SuspendLayout();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.rowcountPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rowcountUpDown)).BeginInit();
            this.sqlTextFilterPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.databasesFilterPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.applicationsFilterPanel.SuspendLayout();
            this.pnlApplication.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(545, 697);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(464, 697);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // propertiesControl1
            // 
            this.propertiesControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesControl1.Location = new System.Drawing.Point(12, 12);
            this.propertiesControl1.Name = "propertiesControl1";
            this.propertiesControl1.PropertyPages.Add(this.propertyPage1);
            this.propertiesControl1.SelectedPropertyPageIndex = 0;
            this.propertiesControl1.Size = new System.Drawing.Size(608, 679);
            this.propertiesControl1.TabIndex = 4;
            // 
            // propertyPage1
            // 
            this.propertyPage1.Controls.Add(this.office2007PropertyPage1);
            this.propertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPage1.Location = new System.Drawing.Point(0, 0);
            this.propertyPage1.Name = "propertyPage1";
            this.propertyPage1.Size = new System.Drawing.Size(454, 679);
            this.propertyPage1.TabIndex = 0;
            this.propertyPage1.Text = "Filters";
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.panel1);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.rowcountHeaderStrip);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.sqlTextFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.databasesFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.applicationsFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(452, 622);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(454, 679);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Supply additional {0} filtering criteria.";
            this.office2007PropertyPage1.ContentPanel.AutoScroll = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkExcludeDM);
            this.panel1.Controls.Add(this.propertiesHeaderStrip4);
            this.panel1.Controls.Add(this.rowcountPanel);
            this.panel1.Location = new System.Drawing.Point(1, 455);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 166);
            this.panel1.TabIndex = 15;
            // 
            // chkExcludeDM
            // 
            this.chkExcludeDM.AutoSize = true;
            this.chkExcludeDM.Location = new System.Drawing.Point(22, 34);
            this.chkExcludeDM.Name = "chkExcludeDM";
            this.chkExcludeDM.Size = new System.Drawing.Size(220, 17);
            this.chkExcludeDM.TabIndex = 6;
            this.chkExcludeDM.Text = "Exclude SQL Diagnostic Manager queries";
            this.chkExcludeDM.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip4
            // 
            this.propertiesHeaderStrip4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip4.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip4.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip4.Name = "propertiesHeaderStrip4";
            this.propertiesHeaderStrip4.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip4.TabIndex = 5;
            this.propertiesHeaderStrip4.Text = "Include SQL Diagnostic Manager queries in the results?";
            this.propertiesHeaderStrip4.WordWrap = false;
            // 
            // rowcountPanel
            // 
            this.rowcountPanel.Controls.Add(this.rowcountLimited);
            this.rowcountPanel.Controls.Add(this.rowcountUnlimited);
            this.rowcountPanel.Controls.Add(this.rowcountUpDown);
            this.rowcountPanel.Controls.Add(this.propertiesHeaderStrip3);
            this.rowcountPanel.Location = new System.Drawing.Point(4, 57);
            this.rowcountPanel.Name = "rowcountPanel";
            this.rowcountPanel.Size = new System.Drawing.Size(439, 106);
            this.rowcountPanel.TabIndex = 4;
            this.rowcountPanel.Visible = false;
            // 
            // rowcountLimited
            // 
            this.rowcountLimited.AutoSize = true;
            this.rowcountLimited.Location = new System.Drawing.Point(23, 61);
            this.rowcountLimited.Name = "rowcountLimited";
            this.rowcountLimited.Size = new System.Drawing.Size(58, 17);
            this.rowcountLimited.TabIndex = 6;
            this.rowcountLimited.Text = "Limited";
            this.rowcountLimited.UseVisualStyleBackColor = true;
            this.rowcountLimited.CheckedChanged += new System.EventHandler(this.rowcountLimited_CheckedChanged);
            // 
            // rowcountUnlimited
            // 
            this.rowcountUnlimited.AutoSize = true;
            this.rowcountUnlimited.Checked = true;
            this.rowcountUnlimited.Location = new System.Drawing.Point(23, 38);
            this.rowcountUnlimited.Name = "rowcountUnlimited";
            this.rowcountUnlimited.Size = new System.Drawing.Size(68, 17);
            this.rowcountUnlimited.TabIndex = 5;
            this.rowcountUnlimited.TabStop = true;
            this.rowcountUnlimited.Text = "Unlimited";
            this.rowcountUnlimited.UseVisualStyleBackColor = true;
            this.rowcountUnlimited.CheckedChanged += new System.EventHandler(this.rowcountUnlimited_CheckedChanged);
            // 
            // rowcountUpDown
            // 
            this.rowcountUpDown.Enabled = false;
            this.rowcountUpDown.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rowcountUpDown.Location = new System.Drawing.Point(96, 59);
            this.rowcountUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.rowcountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rowcountUpDown.Name = "rowcountUpDown";
            this.rowcountUpDown.Size = new System.Drawing.Size(66, 20);
            this.rowcountUpDown.TabIndex = 7;
            this.rowcountUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rowcountUpDown.ValueChanged += new System.EventHandler(this.rowcountUpDown_ValueChanged);
            // 
            // propertiesHeaderStrip3
            // 
            this.propertiesHeaderStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip3.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip3.Location = new System.Drawing.Point(3, 2);
            this.propertiesHeaderStrip3.Name = "propertiesHeaderStrip3";
            this.propertiesHeaderStrip3.Size = new System.Drawing.Size(437, 25);
            this.propertiesHeaderStrip3.TabIndex = 1;
            this.propertiesHeaderStrip3.Text = "How many rows should be collected at a time?";
            this.propertiesHeaderStrip3.WordWrap = false;
            // 
            // rowcountHeaderStrip
            // 
            this.rowcountHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rowcountHeaderStrip.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.rowcountHeaderStrip.Location = new System.Drawing.Point(8, 425);
            this.rowcountHeaderStrip.Name = "rowcountHeaderStrip";
            this.rowcountHeaderStrip.Size = new System.Drawing.Size(436, 25);
            this.rowcountHeaderStrip.TabIndex = 14;
            this.rowcountHeaderStrip.Text = "Which SQL text would you like to exclude?";
            this.rowcountHeaderStrip.Visible = false;
            this.rowcountHeaderStrip.WordWrap = false;
            // 
            // sqlTextFilterPanel
            // 
            this.sqlTextFilterPanel.Controls.Add(this.panel6);
            this.sqlTextFilterPanel.Controls.Add(this.panel7);
            this.sqlTextFilterPanel.Location = new System.Drawing.Point(1, 292);
            this.sqlTextFilterPanel.Name = "sqlTextFilterPanel";
            this.sqlTextFilterPanel.Size = new System.Drawing.Size(450, 127);
            this.sqlTextFilterPanel.TabIndex = 13;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.sqlTextIncludeFilterTextBox);
            this.panel6.Controls.Add(this.sqlTextExcludeFilterTextBox);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.propertiesHeaderStrip2);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(450, 104);
            this.panel6.TabIndex = 5;
            // 
            // sqlTextIncludeFilterTextBox
            // 
            this.sqlTextIncludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextIncludeFilterTextBox.Location = new System.Drawing.Point(78, 66);
            this.sqlTextIncludeFilterTextBox.Multiline = true;
            this.sqlTextIncludeFilterTextBox.Name = "sqlTextIncludeFilterTextBox";
            this.sqlTextIncludeFilterTextBox.Size = new System.Drawing.Size(365, 24);
            this.sqlTextIncludeFilterTextBox.TabIndex = 9;
            // 
            // sqlTextExcludeFilterTextBox
            // 
            this.sqlTextExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextExcludeFilterTextBox.Location = new System.Drawing.Point(78, 33);
            this.sqlTextExcludeFilterTextBox.Multiline = true;
            this.sqlTextExcludeFilterTextBox.Name = "sqlTextExcludeFilterTextBox";
            this.sqlTextExcludeFilterTextBox.Size = new System.Drawing.Size(365, 24);
            this.sqlTextExcludeFilterTextBox.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 24);
            this.label6.TabIndex = 7;
            this.label6.Text = "Include:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip2.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip2.TabIndex = 1;
            this.propertiesHeaderStrip2.Text = "Which SQL text would you like to include or exclude?";
            this.propertiesHeaderStrip2.WordWrap = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "Exclude:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel7
            // 
            this.panel7.AutoSize = true;
            this.panel7.Controls.Add(this.label4);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 107);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(450, 20);
            this.panel7.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label4.Size = new System.Drawing.Size(450, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "use semicolons to separate names; use % for wildcards";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // databasesFilterPanel
            // 
            this.databasesFilterPanel.Controls.Add(this.panel2);
            this.databasesFilterPanel.Controls.Add(this.panel3);
            this.databasesFilterPanel.Location = new System.Drawing.Point(-1, 142);
            this.databasesFilterPanel.Name = "databasesFilterPanel";
            this.databasesFilterPanel.Size = new System.Drawing.Size(452, 121);
            this.databasesFilterPanel.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.databasesIncludeFilterTextBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.propertiesHeaderStrip1);
            this.panel2.Controls.Add(this.databasesExcludeFilterTextBox);
            this.panel2.Location = new System.Drawing.Point(2, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(450, 91);
            this.panel2.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(3, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 24);
            this.label7.TabIndex = 5;
            this.label7.Text = "Include:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // databasesIncludeFilterTextBox
            // 
            this.databasesIncludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesIncludeFilterTextBox.Location = new System.Drawing.Point(78, 60);
            this.databasesIncludeFilterTextBox.Multiline = true;
            this.databasesIncludeFilterTextBox.Name = "databasesIncludeFilterTextBox";
            this.databasesIncludeFilterTextBox.Size = new System.Drawing.Size(365, 24);
            this.databasesIncludeFilterTextBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Exclude:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip1.TabIndex = 1;
            this.propertiesHeaderStrip1.Text = "Which databases would you like to include or exclude?";
            this.propertiesHeaderStrip1.WordWrap = false;
            // 
            // databasesExcludeFilterTextBox
            // 
            this.databasesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesExcludeFilterTextBox.Location = new System.Drawing.Point(78, 30);
            this.databasesExcludeFilterTextBox.Multiline = true;
            this.databasesExcludeFilterTextBox.Name = "databasesExcludeFilterTextBox";
            this.databasesExcludeFilterTextBox.Size = new System.Drawing.Size(365, 24);
            this.databasesExcludeFilterTextBox.TabIndex = 3;
            this.databasesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 101);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(452, 20);
            this.panel3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label2.Size = new System.Drawing.Size(452, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "use semicolons to separate names; use % for wildcards";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // applicationsFilterPanel
            // 
            this.applicationsFilterPanel.Controls.Add(this.pnlApplication);
            this.applicationsFilterPanel.Controls.Add(this.panel8);
            this.applicationsFilterPanel.Location = new System.Drawing.Point(1, 1);
            this.applicationsFilterPanel.Name = "applicationsFilterPanel";
            this.applicationsFilterPanel.Size = new System.Drawing.Size(450, 135);
            this.applicationsFilterPanel.TabIndex = 11;
            // 
            // pnlApplication
            // 
            this.pnlApplication.Controls.Add(this.label8);
            this.pnlApplication.Controls.Add(this.applicationsIncludeFilterTextBox);
            this.pnlApplication.Controls.Add(this.label12);
            this.pnlApplication.Controls.Add(this.applicationExcludeHeaderStrip);
            this.pnlApplication.Controls.Add(this.applicationsExcludeFilterTextBox);
            this.pnlApplication.Location = new System.Drawing.Point(-2, 0);
            this.pnlApplication.Name = "pnlApplication";
            this.pnlApplication.Size = new System.Drawing.Size(452, 95);
            this.pnlApplication.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(3, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 24);
            this.label8.TabIndex = 6;
            this.label8.Text = "Include:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applicationsIncludeFilterTextBox
            // 
            this.applicationsIncludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationsIncludeFilterTextBox.Location = new System.Drawing.Point(78, 62);
            this.applicationsIncludeFilterTextBox.Multiline = true;
            this.applicationsIncludeFilterTextBox.Name = "applicationsIncludeFilterTextBox";
            this.applicationsIncludeFilterTextBox.Size = new System.Drawing.Size(367, 24);
            this.applicationsIncludeFilterTextBox.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(3, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 24);
            this.label12.TabIndex = 3;
            this.label12.Text = "Exclude:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applicationExcludeHeaderStrip
            // 
            this.applicationExcludeHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationExcludeHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.applicationExcludeHeaderStrip.Location = new System.Drawing.Point(7, 3);
            this.applicationExcludeHeaderStrip.Name = "applicationExcludeHeaderStrip";
            this.applicationExcludeHeaderStrip.Size = new System.Drawing.Size(438, 25);
            this.applicationExcludeHeaderStrip.TabIndex = 1;
            this.applicationExcludeHeaderStrip.Text = "Which applications would you like to include or exclude?";
            this.applicationExcludeHeaderStrip.WordWrap = false;
            // 
            // applicationsExcludeFilterTextBox
            // 
            this.applicationsExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationsExcludeFilterTextBox.Location = new System.Drawing.Point(78, 32);
            this.applicationsExcludeFilterTextBox.Multiline = true;
            this.applicationsExcludeFilterTextBox.Name = "applicationsExcludeFilterTextBox";
            this.applicationsExcludeFilterTextBox.Size = new System.Drawing.Size(367, 24);
            this.applicationsExcludeFilterTextBox.TabIndex = 2;
            this.applicationsExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // panel8
            // 
            this.panel8.AutoSize = true;
            this.panel8.Controls.Add(this.label9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 101);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(450, 34);
            this.panel8.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.White;
            this.label9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label9.Size = new System.Drawing.Size(450, 34);
            this.label9.TabIndex = 4;
            this.label9.Text = "use semicolons to separate names; use % for wildcards";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // AdvancedQueryFilterConfigurationDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(632, 732);
            this.Controls.Add(this.propertiesControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(580, 400);
            this.Name = "AdvancedQueryFilterConfigurationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced {0} Configuration";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AdvancedQueryMonitorConfigurationDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AdvancedQueryMonitorConfigurationDialog_HelpRequested);
            this.propertyPage1.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.rowcountPanel.ResumeLayout(false);
            this.rowcountPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rowcountUpDown)).EndInit();
            this.sqlTextFilterPanel.ResumeLayout(false);
            this.sqlTextFilterPanel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.databasesFilterPanel.ResumeLayout(false);
            this.databasesFilterPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.applicationsFilterPanel.ResumeLayout(false);
            this.applicationsFilterPanel.PerformLayout();
            this.pnlApplication.ResumeLayout(false);
            this.pnlApplication.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void OnCurrentThemeChanged(object sender, System.EventArgs e)
        {
            SetPropertiesTheme();
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(propertiesControl1);
            propertiesThemeManager.UpdatePropertyTheme(office2007PropertyPage1);
        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesControl propertiesControl1;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage propertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sqlTextFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip2;
        
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  databasesFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox databasesExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  applicationsFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  pnlApplication;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip applicationExcludeHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox applicationsExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip rowcountHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown rowcountUpDown;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  rowcountPanel;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton rowcountLimited;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton rowcountUnlimited;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkExcludeDM;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlTextExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlTextIncludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox databasesIncludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox applicationsIncludeFilterTextBox;

    }
}
