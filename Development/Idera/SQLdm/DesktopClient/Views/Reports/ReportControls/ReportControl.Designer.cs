namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ReportContol
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
            if (disposing)
            {
                if (sampleMinutes     != null) sampleMinutes.Dispose();
                if (sampleHours       != null) sampleHours.Dispose();
                if (sampleDays        != null) sampleDays.Dispose();
                if (sampleMonths      != null) sampleMonths.Dispose();
                if (sampleYears       != null) sampleYears.Dispose();
                if (periodToday       != null) periodToday.Dispose();
                if (period7           != null) period7.Dispose();
                if (period30          != null) period30.Dispose();
                if (period365         != null) period365.Dispose();
                if (periodSetCustom   != null) periodSetCustom.Dispose();
                if (tagSelectOne      != null) tagSelectOne.Dispose();
                if (instanceSelectOne != null) instanceSelectOne.Dispose();

                if(components != null)
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
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.filterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.rangeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.rangeInfoLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.runReportButton = new Infragistics.Win.Misc.UltraButton();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tagsComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            //this.serverNameText = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.serverNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.instanceCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.targetCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.sourceCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.sampleSizeCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.periodCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.instanceLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sourceLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.targetLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sampleLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.periodLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tagsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.mainContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.loadingPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.reportInstructionsControl = new Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportInstructionsControl();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.mainContentPanel.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.filterPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(752, 138);
            this.panel1.TabIndex = 0;
            // 
            // filterPanel
            // 
            this.filterPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterPanel.Controls.Add(this.rangeLabel);
            this.filterPanel.Controls.Add(this.rangeInfoLabel);
            this.filterPanel.Controls.Add(this.panel3);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.tagsComboBox);
            this.filterPanel.Controls.Add(this.instanceCombo);
            this.filterPanel.Controls.Add(this.targetCombo);
            this.filterPanel.Controls.Add(this.sourceCombo);
            this.filterPanel.Controls.Add(this.sampleSizeCombo);
            this.filterPanel.Controls.Add(this.periodCombo);
            this.filterPanel.Controls.Add(this.instanceLabel);
            this.filterPanel.Controls.Add(this.sourceLabel);
            this.filterPanel.Controls.Add(this.targetLabel);
            this.filterPanel.Controls.Add(this.sampleLabel);
            this.filterPanel.Controls.Add(this.periodLabel);
            this.filterPanel.Controls.Add(this.tagsLabel);
          //  this.filterPanel.Controls.Add(this.serverNameText);
            this.filterPanel.Controls.Add(this.serverNameLabel);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(752, 200);
            this.filterPanel.TabIndex = 4;
            // 
            // rangeLabel
            // 
            this.rangeLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.rangeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rangeLabel.ForeColor = System.Drawing.Color.White;
            this.rangeLabel.Location = new System.Drawing.Point(81, 62);
            this.rangeLabel.Name = "rangeLabel";
            this.rangeLabel.Size = new System.Drawing.Size(211, 21);
            this.rangeLabel.TabIndex = 29;
            this.rangeLabel.Text = "< Date Range >";
            this.rangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.AutoSize = true;
            this.rangeInfoLabel.Location = new System.Drawing.Point(33, 66);
            this.rangeInfoLabel.Name = "rangeInfoLabel";
            this.rangeInfoLabel.Size = new System.Drawing.Size(42, 13);
            this.rangeInfoLabel.TabIndex = 28;
            this.rangeInfoLabel.Text = "Range:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.runReportButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 104);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(752, 33);
            this.panel3.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(752, 2);
            this.label3.TabIndex = 1;
            // 
            // runReportButton
            // 
            this.runReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runReportButton.Location = new System.Drawing.Point(671, 5);
            this.runReportButton.Name = "runReportButton";
            this.runReportButton.ShowFocusRect = false;
            this.runReportButton.ShowOutline = false;
            this.runReportButton.TabIndex = 0;
            this.runReportButton.Text = "Run Report";
            this.runReportButton.Appearance.ForeColor = System.Drawing.Color.White;
            this.runReportButton.Appearance.BorderColor3DBase = System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.runReportButton.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.runReportButton.Appearance.BorderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.runReportButton.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.runReportButton.UseAppStyling = false;
            this.runReportButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.runReportButton.Click += new System.EventHandler(this.runReportButton_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(0, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(752, 1);
            this.label2.TabIndex = 26;
            this.label2.Text = "label2";
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagsComboBox.FormattingEnabled = true;
            this.tagsComboBox.Location = new System.Drawing.Point(81, 7);
            this.tagsComboBox.Name = "tagsComboBox";
            this.tagsComboBox.Size = new System.Drawing.Size(211, 21);
            this.tagsComboBox.TabIndex = 24;
            this.tagsComboBox.SelectedValueChanged += new System.EventHandler(this.tagsComboBox_SelectionChanged);
            // 
            // ServerNameText
            //
            //this.serverNameText.Location = new System.Drawing.Point(81, 7);
            //this.serverNameText.Name = "ServerNameText";
            //this.serverNameText.Size = new System.Drawing.Size(211, 21);
            //this.serverNameText.TabIndex = 24;
            // 
            // instanceCombo
            // 
            this.instanceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.instanceCombo.FormattingEnabled = false;
            this.instanceCombo.Location = new System.Drawing.Point(349, 7);
            this.instanceCombo.Name = "instanceCombo";
            this.instanceCombo.Size = new System.Drawing.Size(211, 21);
            this.instanceCombo.Sorted = true;
            this.instanceCombo.TabIndex = 23;
            this.instanceCombo.DropDown += new System.EventHandler(this.instanceCombo_BeforeDropDown);
            this.instanceCombo.SelectedValueChanged += new System.EventHandler(this.instanceCombo_ValueChanged);

            // 
            // SourceCombo
            // 
            this.sourceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceCombo.FormattingEnabled = true;
            this.sourceCombo.Location = new System.Drawing.Point(81, 7);
            this.sourceCombo.Name = "sourceCombo";
            this.sourceCombo.Size = new System.Drawing.Size(211, 21);
            this.sourceCombo.Sorted = true;
            this.sourceCombo.TabIndex = 23;
            this.sourceCombo.Visible = false;
            this.sourceCombo.DropDown += new System.EventHandler(this.sourceCombo_BeforeDropDown);
            this.sourceCombo.SelectionChangeCommitted += SourceCombo_SelectionChangeCommitted;
            // 
            // TargetCombo
            // 
            this.targetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetCombo.FormattingEnabled = true;
            this.targetCombo.Location = new System.Drawing.Point(81, 7);
            this.targetCombo.Name = "targetCombo";
            this.targetCombo.Size = new System.Drawing.Size(211, 21);
            this.targetCombo.Sorted = true;
            this.targetCombo.TabIndex = 23;
            this.targetCombo.Visible = false;
            this.targetCombo.DropDown += new System.EventHandler(this.targetCombo_BeforeDropDown);
            this.targetCombo.SelectionChangeCommitted += TargetCombo_SelectionChangeCommitted;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sampleSizeCombo.FormattingEnabled = true;
            this.sampleSizeCombo.Location = new System.Drawing.Point(349, 34);
            this.sampleSizeCombo.Name = "sampleSizeCombo";
            this.sampleSizeCombo.Size = new System.Drawing.Size(211, 21);
            this.sampleSizeCombo.TabIndex = 22;
            this.sampleSizeCombo.SelectedIndexChanged += new System.EventHandler(this.sampleSizeCombo_SelectionChanged);
            // 
            // periodCombo
            // 
            this.periodCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.periodCombo.FormattingEnabled = true;
            this.periodCombo.Location = new System.Drawing.Point(81, 34);
            this.periodCombo.Name = "periodCombo";
            this.periodCombo.Size = new System.Drawing.Size(211, 21);
            this.periodCombo.TabIndex = 21;
            this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectionChanged);
            // 
            // instanceLabel
            // 
            this.instanceLabel.AutoSize = true;
            this.instanceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.instanceLabel.Location = new System.Drawing.Point(303, 10);
            this.instanceLabel.Name = "instanceLabel";
            this.instanceLabel.Size = new System.Drawing.Size(41, 13);
            this.instanceLabel.TabIndex = 17;
            this.instanceLabel.Text = "Server:";
            // 
            // sourceLabel
            // 
            this.sourceLabel.AutoSize = true;
            this.sourceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sourceLabel.Location = new System.Drawing.Point(303, 10);
            this.sourceLabel.Name = "sourceLabel";
            this.sourceLabel.Size = new System.Drawing.Size(41, 13);
            this.sourceLabel.TabIndex = 17;
            this.sourceLabel.Visible = false;
            this.sourceLabel.Text = "Source:";
            // 
            // targetLabel
            // 
            this.targetLabel.AutoSize = true;
            this.targetLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.targetLabel.Location = new System.Drawing.Point(303, 10);
            this.targetLabel.Name = "sourceLabel";
            this.targetLabel.Size = new System.Drawing.Size(41, 13);
            this.targetLabel.TabIndex = 17;
            this.targetLabel.Visible = false;
            this.targetLabel.Text = "Target:";
            // 
            // sampleLabel
            // 
            this.sampleLabel.AutoSize = true;
            this.sampleLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sampleLabel.Location = new System.Drawing.Point(298, 37);
            this.sampleLabel.Name = "sampleLabel";
            this.sampleLabel.Size = new System.Drawing.Size(45, 13);
            this.sampleLabel.TabIndex = 15;
            this.sampleLabel.Text = "Sample:";
            // 
            // periodLabel
            // 
            this.periodLabel.AutoSize = true;
            this.periodLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.periodLabel.Location = new System.Drawing.Point(33, 37);
            this.periodLabel.Name = "periodLabel";
            this.periodLabel.Size = new System.Drawing.Size(40, 13);
            this.periodLabel.TabIndex = 13;
            this.periodLabel.Text = "Period:";
            // 
            // tagsLabel
            // 
            this.tagsLabel.AutoSize = true;
            this.tagsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tagsLabel.Location = new System.Drawing.Point(33, 10);
            this.tagsLabel.Name = "tagsLabel";
            this.tagsLabel.Size = new System.Drawing.Size(29, 13);
            this.tagsLabel.TabIndex = 7;
            this.tagsLabel.Text = "Tag:";
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.serverNameLabel.Location = new System.Drawing.Point(33, 40);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.TabIndex = 4;
            // 
            // reportViewer
            // 
            this.reportViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.Location = new System.Drawing.Point(0, 3);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ShowBackButton = false;
            this.reportViewer.ShowDocumentMapButton = false;
            this.reportViewer.Size = new System.Drawing.Size(752, 466);
            this.reportViewer.TabIndex = 1;
            // 
            // mainContentPanel
            // 
            this.mainContentPanel.AutoScroll = true;
            this.mainContentPanel.AutoSize = true;
            this.mainContentPanel.Controls.Add(this.reportInstructionsControl);
            this.mainContentPanel.Controls.Add(this.reportViewer);
            this.mainContentPanel.Controls.Add(this.loadingPanel);
            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Location = new System.Drawing.Point(0, 138);
            this.mainContentPanel.Name = "mainContentPanel";
            this.mainContentPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mainContentPanel.Size = new System.Drawing.Size(752, 469);
            this.mainContentPanel.TabIndex = 3;
            // 
            // loadingPanel
            // 
            this.loadingPanel.Controls.Add(this.label1);
            this.loadingPanel.Controls.Add(this.loadingCircle);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingPanel.Location = new System.Drawing.Point(0, 3);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(752, 466);
            this.loadingPanel.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(320, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loading data...";
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.Color = System.Drawing.Color.ForestGreen;
            this.loadingCircle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingCircle.InnerCircleRadius = 5;
            this.loadingCircle.Location = new System.Drawing.Point(0, 0);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 12;
            this.loadingCircle.OuterCircleRadius = 11;
            this.loadingCircle.RotationSpeed = 40;
            this.loadingCircle.Size = new System.Drawing.Size(752, 466);
            this.loadingCircle.SpokeThickness = 2;
            this.loadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle.TabIndex = 0;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.AutoScroll = true;
            this.reportInstructionsControl.BackColor = System.Drawing.Color.White;
            this.reportInstructionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportInstructionsControl.Location = new System.Drawing.Point(0, 3);
            this.reportInstructionsControl.Name = "reportInstructionsControl";
            this.reportInstructionsControl.ReportDescription = "< Report Description >";
            this.reportInstructionsControl.ReportInstructions = "1. < Step 1 >\r\n2. < Step 2 >\r\n3. < Step 3 >";
            this.reportInstructionsControl.ReportTitle = "< Report Title >";
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 466);
            this.reportInstructionsControl.TabIndex = 2;
            // 
            // ReportContol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ReportContol";
            this.Size = new System.Drawing.Size(752, 607);
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.mainContentPanel.ResumeLayout(false);
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }      

        #endregion

        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        protected Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  filterPanel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel tagsLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serverNameLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel periodLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel sampleLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel instanceLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel sourceLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel targetLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox periodCombo;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sampleSizeCombo;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox instanceCombo;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox targetCombo;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sourceCombo;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox tagsComboBox;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox serverNameText;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Infragistics.Win.Misc.UltraButton runReportButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  mainContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  loadingPanel;
        private MRG.Controls.UI.LoadingCircle loadingCircle;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        protected ReportInstructionsControl reportInstructionsControl;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel rangeLabel;
        protected Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel rangeInfoLabel;
    }
}
