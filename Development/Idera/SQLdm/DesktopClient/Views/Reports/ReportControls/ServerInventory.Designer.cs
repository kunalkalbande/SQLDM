using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ServerInventory
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
                if (searchServerName     != null) searchServerName.Dispose();
                if (searchServerVersion  != null) searchServerVersion.Dispose();
                if (searchOwner          != null) searchOwner.Dispose();
                if (searchClustered      != null) searchClustered.Dispose();
                if (searchOSVersion      != null) searchOSVersion.Dispose();
                if (searchNumProcessors  != null) searchNumProcessors.Dispose();
                if (searchPhysicalMemory != null) searchPhysicalMemory.Dispose();
                if (searchServerTag      != null) searchServerTag.Dispose();
                if (selectSearchItem     != null) selectSearchItem.Dispose();

                if (components != null)
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.searchItem1Combo = new System.Windows.Forms.ComboBox();
            this.searchValue1Textbox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.searchItem2Combo = new System.Windows.Forms.ComboBox();
            this.searchItem3Combo = new System.Windows.Forms.ComboBox();
            this.searchItem4Combo = new System.Windows.Forms.ComboBox();
            this.searchValue2Textbox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.searchValue3Textbox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.searchValue4Textbox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.like1Label = new System.Windows.Forms.Label();
            this.like2Label = new System.Windows.Forms.Label();
            this.like3Label = new System.Windows.Forms.Label();
            this.like4Label = new System.Windows.Forms.Label();
            this.wildCardLabel1 = new System.Windows.Forms.Label();
            this.wildCardLabel2 = new System.Windows.Forms.Label();
            this.wildCardLabel3 = new System.Windows.Forms.Label();
            this.wildCardLabel4 = new System.Windows.Forms.Label();
            this.memoryUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.memoryUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.memoryUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.memoryUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.lblUpdown1 = new System.Windows.Forms.Label();
            this.lblUpdown2 = new System.Windows.Forms.Label();
            this.lblUpdown3 = new System.Windows.Forms.Label();
            this.lblUpdown4 = new System.Windows.Forms.Label();
            this.memory1Panel = new System.Windows.Forms.Panel();
            this.clusteredCombo3 = new System.Windows.Forms.ComboBox();
            this.clusteredCombo4 = new System.Windows.Forms.ComboBox();
            this.clusteredCombo2 = new System.Windows.Forms.ComboBox();
            this.clusteredCombo1 = new System.Windows.Forms.ComboBox();
            this.memory2Panel = new System.Windows.Forms.Panel();
            this.memory3Panel = new System.Windows.Forms.Panel();
            this.memory4Panel = new System.Windows.Forms.Panel();
            this.lessThanLabel2 = new System.Windows.Forms.Label();
            this.lessThanLabel1 = new System.Windows.Forms.Label();
            this.lessThanLabel3 = new System.Windows.Forms.Label();
            this.lessThanLabel4 = new System.Windows.Forms.Label();
            this.tagsCombo1 = new System.Windows.Forms.ComboBox();
            this.tagsCombo2 = new System.Windows.Forms.ComboBox();
            this.tagsCombo3 = new System.Windows.Forms.ComboBox();
            this.tagsCombo4 = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown4)).BeginInit();
            this.memory1Panel.SuspendLayout();
            this.memory2Panel.SuspendLayout();
            this.memory3Panel.SuspendLayout();
            this.memory4Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 173);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 431);
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.searchItem1Combo);
            this.filterPanel.Controls.Add(this.like1Label);
            this.filterPanel.Controls.Add(this.searchItem4Combo);
            this.filterPanel.Controls.Add(this.searchItem3Combo);
            this.filterPanel.Controls.Add(this.searchItem2Combo);
            this.filterPanel.Controls.Add(this.like2Label);
            this.filterPanel.Controls.Add(this.lessThanLabel1);
            this.filterPanel.Controls.Add(this.lessThanLabel2);
            this.filterPanel.Controls.Add(this.like3Label);
            this.filterPanel.Controls.Add(this.lessThanLabel3);
            this.filterPanel.Controls.Add(this.like4Label);
            this.filterPanel.Controls.Add(this.lessThanLabel4);
            this.filterPanel.Controls.Add(this.memory1Panel);
            this.filterPanel.Controls.Add(this.memory2Panel);
            this.filterPanel.Controls.Add(this.memory3Panel);
            this.filterPanel.Controls.Add(this.wildCardLabel4);
            this.filterPanel.Controls.Add(this.wildCardLabel2);
            this.filterPanel.Controls.Add(this.wildCardLabel3);
            this.filterPanel.Controls.Add(this.memory4Panel);
            this.filterPanel.Controls.Add(this.wildCardLabel1);
            this.filterPanel.Controls.Add(this.searchValue1Textbox);
            this.filterPanel.Controls.Add(this.searchValue2Textbox);
            this.filterPanel.Controls.Add(this.searchValue3Textbox);
            this.filterPanel.Controls.Add(this.searchValue4Textbox);
            this.filterPanel.Controls.Add(this.tagsCombo1);
            this.filterPanel.Controls.Add(this.tagsCombo2);
            this.filterPanel.Controls.Add(this.tagsCombo3);
            this.filterPanel.Controls.Add(this.tagsCombo4);
            this.filterPanel.Controls.Add(this.clusteredCombo1);
            this.filterPanel.Controls.Add(this.clusteredCombo2);
            this.filterPanel.Controls.Add(this.clusteredCombo3);
            this.filterPanel.Controls.Add(this.clusteredCombo4);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Size = new System.Drawing.Size(752, 173);
            this.filterPanel.Controls.SetChildIndex(this.label1, 0);
            this.filterPanel.Controls.SetChildIndex(this.label2, 0);
            this.filterPanel.Controls.SetChildIndex(this.clusteredCombo4, 0);
            this.filterPanel.Controls.SetChildIndex(this.clusteredCombo3, 0);
            this.filterPanel.Controls.SetChildIndex(this.clusteredCombo2, 0);
            this.filterPanel.Controls.SetChildIndex(this.clusteredCombo1, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsCombo4, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsCombo3, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsCombo2, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsCombo1, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchValue4Textbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchValue3Textbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchValue2Textbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchValue1Textbox, 0);
            this.filterPanel.Controls.SetChildIndex(this.wildCardLabel1, 0);
            this.filterPanel.Controls.SetChildIndex(this.memory4Panel, 0);
            this.filterPanel.Controls.SetChildIndex(this.wildCardLabel3, 0);
            this.filterPanel.Controls.SetChildIndex(this.wildCardLabel2, 0);
            this.filterPanel.Controls.SetChildIndex(this.wildCardLabel4, 0);
            this.filterPanel.Controls.SetChildIndex(this.memory3Panel, 0);
            this.filterPanel.Controls.SetChildIndex(this.memory2Panel, 0);
            this.filterPanel.Controls.SetChildIndex(this.memory1Panel, 0);
            this.filterPanel.Controls.SetChildIndex(this.lessThanLabel4, 0);
            this.filterPanel.Controls.SetChildIndex(this.like4Label, 0);
            this.filterPanel.Controls.SetChildIndex(this.lessThanLabel3, 0);
            this.filterPanel.Controls.SetChildIndex(this.like3Label, 0);
            this.filterPanel.Controls.SetChildIndex(this.lessThanLabel2, 0);
            this.filterPanel.Controls.SetChildIndex(this.lessThanLabel1, 0);
            this.filterPanel.Controls.SetChildIndex(this.like2Label, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchItem2Combo, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchItem3Combo, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchItem4Combo, 0);
            this.filterPanel.Controls.SetChildIndex(this.like1Label, 0);
            this.filterPanel.Controls.SetChildIndex(this.searchItem1Combo, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsComboBox, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.instanceLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.tagsLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            // 
            // tagsLabel
            // 
            this.tagsLabel.Location = new System.Drawing.Point(60, 280);
            this.tagsLabel.Visible = false;
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(54, 307);
            this.periodLabel.Visible = false;
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(316, 307);
            // 
            // instanceLabel
            // 
            this.instanceLabel.Location = new System.Drawing.Point(321, 280);
            this.instanceLabel.Visible = false;
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(101, 304);
            this.periodCombo.Visible = false;
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(367, 304);
            // 
            // instanceCombo
            // 
            this.instanceCombo.Location = new System.Drawing.Point(367, 277);
            this.instanceCombo.Visible = false;
            // 
            // tagsComboBox
            // 
            this.tagsComboBox.Location = new System.Drawing.Point(100, 280);
            this.tagsComboBox.Visible = false;
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = "1. Choose one or more Search Items to search for in your server inventory.\r\n2. Sp" +
                "ecify target Search Values for the selected Search Items.\r\n3. Click Run Report.";
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 431);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(522, 152);
            this.rangeLabel.Visible = false;
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(474, 156);
            this.rangeInfoLabel.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Search Item";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(262, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Search Value";
            // 
            // searchItem1Combo
            // 
            this.searchItem1Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchItem1Combo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchItem1Combo.FormattingEnabled = true;
            this.searchItem1Combo.Location = new System.Drawing.Point(13, 28);
            this.searchItem1Combo.Name = "searchItem1Combo";
            this.searchItem1Combo.Size = new System.Drawing.Size(200, 21);
            this.searchItem1Combo.TabIndex = 0;
            this.searchItem1Combo.SelectedIndexChanged += new System.EventHandler(this.searchItemCombo_IndexChanged);
            // 
            // searchValue1Textbox
            // 
            this.searchValue1Textbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchValue1Textbox.Location = new System.Drawing.Point(265, 28);
            this.searchValue1Textbox.MaxLength = 256;
            this.searchValue1Textbox.Name = "searchValue1Textbox";
            this.searchValue1Textbox.Size = new System.Drawing.Size(200, 20);
            this.searchValue1Textbox.TabIndex = 1;
            // 
            // searchItem2Combo
            // 
            this.searchItem2Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchItem2Combo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchItem2Combo.FormattingEnabled = true;
            this.searchItem2Combo.Location = new System.Drawing.Point(13, 55);
            this.searchItem2Combo.Name = "searchItem2Combo";
            this.searchItem2Combo.Size = new System.Drawing.Size(200, 21);
            this.searchItem2Combo.TabIndex = 2;
            this.searchItem2Combo.SelectedIndexChanged += new System.EventHandler(this.searchItemCombo_IndexChanged);
            // 
            // searchItem3Combo
            // 
            this.searchItem3Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchItem3Combo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchItem3Combo.FormattingEnabled = true;
            this.searchItem3Combo.Location = new System.Drawing.Point(13, 82);
            this.searchItem3Combo.Name = "searchItem3Combo";
            this.searchItem3Combo.Size = new System.Drawing.Size(200, 21);
            this.searchItem3Combo.TabIndex = 4;
            this.searchItem3Combo.SelectedIndexChanged += new System.EventHandler(this.searchItemCombo_IndexChanged);
            // 
            // searchItem4Combo
            // 
            this.searchItem4Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchItem4Combo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchItem4Combo.FormattingEnabled = true;
            this.searchItem4Combo.Location = new System.Drawing.Point(13, 109);
            this.searchItem4Combo.Name = "searchItem4Combo";
            this.searchItem4Combo.Size = new System.Drawing.Size(200, 21);
            this.searchItem4Combo.TabIndex = 6;
            this.searchItem4Combo.SelectedIndexChanged += new System.EventHandler(this.searchItemCombo_IndexChanged);
            // 
            // searchValue2Textbox
            // 
            this.searchValue2Textbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchValue2Textbox.Location = new System.Drawing.Point(265, 55);
            this.searchValue2Textbox.MaxLength = 256;
            this.searchValue2Textbox.Name = "searchValue2Textbox";
            this.searchValue2Textbox.Size = new System.Drawing.Size(200, 20);
            this.searchValue2Textbox.TabIndex = 3;
            this.searchValue2Textbox.Visible = false;
            // 
            // searchValue3Textbox
            // 
            this.searchValue3Textbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchValue3Textbox.Location = new System.Drawing.Point(265, 82);
            this.searchValue3Textbox.MaxLength = 256;
            this.searchValue3Textbox.Name = "searchValue3Textbox";
            this.searchValue3Textbox.Size = new System.Drawing.Size(200, 20);
            this.searchValue3Textbox.TabIndex = 5;
            this.searchValue3Textbox.Visible = false;
            // 
            // searchValue4Textbox
            // 
            this.searchValue4Textbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchValue4Textbox.Location = new System.Drawing.Point(265, 109);
            this.searchValue4Textbox.MaxLength = 256;
            this.searchValue4Textbox.Name = "searchValue4Textbox";
            this.searchValue4Textbox.Size = new System.Drawing.Size(200, 20);
            this.searchValue4Textbox.TabIndex = 7;
            this.searchValue4Textbox.Visible = false;
            // 
            // like1Label
            // 
            this.like1Label.AutoSize = true;
            this.like1Label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.like1Label.Location = new System.Drawing.Point(226, 31);
            this.like1Label.Name = "like1Label";
            this.like1Label.Size = new System.Drawing.Size(27, 13);
            this.like1Label.TabIndex = 35;
            this.like1Label.Text = "Like";
            // 
            // like2Label
            // 
            this.like2Label.AutoSize = true;
            this.like2Label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.like2Label.Location = new System.Drawing.Point(226, 58);
            this.like2Label.Name = "like2Label";
            this.like2Label.Size = new System.Drawing.Size(27, 13);
            this.like2Label.TabIndex = 36;
            this.like2Label.Text = "Like";
            this.like2Label.Visible = false;
            // 
            // like3Label
            // 
            this.like3Label.AutoSize = true;
            this.like3Label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.like3Label.Location = new System.Drawing.Point(226, 85);
            this.like3Label.Name = "like3Label";
            this.like3Label.Size = new System.Drawing.Size(27, 13);
            this.like3Label.TabIndex = 37;
            this.like3Label.Text = "Like";
            this.like3Label.Visible = false;
            // 
            // like4Label
            // 
            this.like4Label.AutoSize = true;
            this.like4Label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.like4Label.Location = new System.Drawing.Point(226, 112);
            this.like4Label.Name = "like4Label";
            this.like4Label.Size = new System.Drawing.Size(27, 13);
            this.like4Label.TabIndex = 38;
            this.like4Label.Text = "Like";
            this.like4Label.Visible = false;
            // 
            // wildCardLabel1
            // 
            this.wildCardLabel1.AutoSize = true;
            this.wildCardLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.wildCardLabel1.Location = new System.Drawing.Point(471, 30);
            this.wildCardLabel1.Name = "wildCardLabel1";
            this.wildCardLabel1.Size = new System.Drawing.Size(91, 13);
            this.wildCardLabel1.TabIndex = 56;
            this.wildCardLabel1.Text = "use % as wildcard";
            // 
            // wildCardLabel2
            // 
            this.wildCardLabel2.AutoSize = true;
            this.wildCardLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.wildCardLabel2.Location = new System.Drawing.Point(471, 58);
            this.wildCardLabel2.Name = "wildCardLabel2";
            this.wildCardLabel2.Size = new System.Drawing.Size(91, 13);
            this.wildCardLabel2.TabIndex = 57;
            this.wildCardLabel2.Text = "use % as wildcard";
            this.wildCardLabel2.Visible = false;
            // 
            // wildCardLabel3
            // 
            this.wildCardLabel3.AutoSize = true;
            this.wildCardLabel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.wildCardLabel3.Location = new System.Drawing.Point(471, 85);
            this.wildCardLabel3.Name = "wildCardLabel3";
            this.wildCardLabel3.Size = new System.Drawing.Size(91, 13);
            this.wildCardLabel3.TabIndex = 58;
            this.wildCardLabel3.Text = "use % as wildcard";
            this.wildCardLabel3.Visible = false;
            // 
            // wildCardLabel4
            // 
            this.wildCardLabel4.AutoSize = true;
            this.wildCardLabel4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.wildCardLabel4.Location = new System.Drawing.Point(471, 112);
            this.wildCardLabel4.Name = "wildCardLabel4";
            this.wildCardLabel4.Size = new System.Drawing.Size(91, 13);
            this.wildCardLabel4.TabIndex = 59;
            this.wildCardLabel4.Text = "use % as wildcard";
            this.wildCardLabel4.Visible = false;
            // 
            // memoryUpDown1
            // 
            this.memoryUpDown1.Location = new System.Drawing.Point(0, 0);
            this.memoryUpDown1.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.memoryUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.memoryUpDown1.Name = "memoryUpDown1";
            this.memoryUpDown1.ReadOnly = true;
            this.memoryUpDown1.Size = new System.Drawing.Size(59, 20);
            this.memoryUpDown1.TabIndex = 61;
            this.memoryUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.memoryUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // memoryUpDown2
            // 
            this.memoryUpDown2.Location = new System.Drawing.Point(0, 1);
            this.memoryUpDown2.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.memoryUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.memoryUpDown2.Name = "memoryUpDown2";
            this.memoryUpDown2.ReadOnly = true;
            this.memoryUpDown2.Size = new System.Drawing.Size(59, 20);
            this.memoryUpDown2.TabIndex = 62;
            this.memoryUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.memoryUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // memoryUpDown3
            // 
            this.memoryUpDown3.Location = new System.Drawing.Point(0, 0);
            this.memoryUpDown3.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.memoryUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.memoryUpDown3.Name = "memoryUpDown3";
            this.memoryUpDown3.ReadOnly = true;
            this.memoryUpDown3.Size = new System.Drawing.Size(59, 20);
            this.memoryUpDown3.TabIndex = 63;
            this.memoryUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.memoryUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // memoryUpDown4
            // 
            this.memoryUpDown4.Location = new System.Drawing.Point(0, 0);
            this.memoryUpDown4.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.memoryUpDown4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.memoryUpDown4.Name = "memoryUpDown4";
            this.memoryUpDown4.ReadOnly = true;
            this.memoryUpDown4.Size = new System.Drawing.Size(59, 20);
            this.memoryUpDown4.TabIndex = 64;
            this.memoryUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.memoryUpDown4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblUpdown1
            // 
            this.lblUpdown1.AutoSize = true;
            this.lblUpdown1.Location = new System.Drawing.Point(65, 4);
            this.lblUpdown1.Name = "lblUpdown1";
            this.lblUpdown1.Size = new System.Drawing.Size(22, 13);
            this.lblUpdown1.TabIndex = 65;
            this.lblUpdown1.Text = "GB";
            // 
            // lblUpdown2
            // 
            this.lblUpdown2.AutoSize = true;
            this.lblUpdown2.Location = new System.Drawing.Point(65, 4);
            this.lblUpdown2.Name = "lblUpdown2";
            this.lblUpdown2.Size = new System.Drawing.Size(22, 13);
            this.lblUpdown2.TabIndex = 66;
            this.lblUpdown2.Text = "GB";
            // 
            // lblUpdown3
            // 
            this.lblUpdown3.AutoSize = true;
            this.lblUpdown3.Location = new System.Drawing.Point(65, 4);
            this.lblUpdown3.Name = "lblUpdown3";
            this.lblUpdown3.Size = new System.Drawing.Size(22, 13);
            this.lblUpdown3.TabIndex = 67;
            this.lblUpdown3.Text = "GB";
            // 
            // lblUpdown4
            // 
            this.lblUpdown4.AutoSize = true;
            this.lblUpdown4.Location = new System.Drawing.Point(65, 4);
            this.lblUpdown4.Name = "lblUpdown4";
            this.lblUpdown4.Size = new System.Drawing.Size(22, 13);
            this.lblUpdown4.TabIndex = 68;
            this.lblUpdown4.Text = "GB";
            // 
            // memory1Panel
            // 
            this.memory1Panel.Controls.Add(this.lblUpdown1);
            this.memory1Panel.Controls.Add(this.memoryUpDown1);
            this.memory1Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.memory1Panel.Location = new System.Drawing.Point(265, 28);
            this.memory1Panel.Name = "memory1Panel";
            this.memory1Panel.Size = new System.Drawing.Size(95, 21);
            this.memory1Panel.TabIndex = 69;
            this.memory1Panel.Visible = false;
            // 
            // clusteredCombo3
            // 
            this.clusteredCombo3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clusteredCombo3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clusteredCombo3.FormattingEnabled = true;
            this.clusteredCombo3.Items.AddRange(new object[] {
            "True",
            "False"});
            this.clusteredCombo3.Location = new System.Drawing.Point(265, 82);
            this.clusteredCombo3.Name = "clusteredCombo3";
            this.clusteredCombo3.Size = new System.Drawing.Size(54, 21);
            this.clusteredCombo3.TabIndex = 79;
            this.clusteredCombo3.Visible = false;
            // 
            // clusteredCombo4
            // 
            this.clusteredCombo4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clusteredCombo4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clusteredCombo4.FormattingEnabled = true;
            this.clusteredCombo4.Items.AddRange(new object[] {
            "True",
            "False"});
            this.clusteredCombo4.Location = new System.Drawing.Point(265, 108);
            this.clusteredCombo4.Name = "clusteredCombo4";
            this.clusteredCombo4.Size = new System.Drawing.Size(54, 21);
            this.clusteredCombo4.TabIndex = 80;
            this.clusteredCombo4.Visible = false;
            // 
            // clusteredCombo2
            // 
            this.clusteredCombo2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clusteredCombo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clusteredCombo2.FormattingEnabled = true;
            this.clusteredCombo2.Items.AddRange(new object[] {
            "True",
            "False"});
            this.clusteredCombo2.Location = new System.Drawing.Point(265, 55);
            this.clusteredCombo2.Name = "clusteredCombo2";
            this.clusteredCombo2.Size = new System.Drawing.Size(54, 21);
            this.clusteredCombo2.TabIndex = 78;
            this.clusteredCombo2.Visible = false;
            // 
            // clusteredCombo1
            // 
            this.clusteredCombo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clusteredCombo1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clusteredCombo1.FormattingEnabled = true;
            this.clusteredCombo1.Items.AddRange(new object[] {
            "True",
            "False"});
            this.clusteredCombo1.Location = new System.Drawing.Point(265, 28);
            this.clusteredCombo1.Name = "clusteredCombo1";
            this.clusteredCombo1.Size = new System.Drawing.Size(54, 21);
            this.clusteredCombo1.TabIndex = 77;
            this.clusteredCombo1.Visible = false;
            // 
            // memory2Panel
            // 
            this.memory2Panel.Controls.Add(this.lblUpdown2);
            this.memory2Panel.Controls.Add(this.memoryUpDown2);
            this.memory2Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.memory2Panel.Location = new System.Drawing.Point(265, 55);
            this.memory2Panel.Name = "memory2Panel";
            this.memory2Panel.Size = new System.Drawing.Size(95, 21);
            this.memory2Panel.TabIndex = 70;
            this.memory2Panel.Visible = false;
            // 
            // memory3Panel
            // 
            this.memory3Panel.Controls.Add(this.memoryUpDown3);
            this.memory3Panel.Controls.Add(this.lblUpdown3);
            this.memory3Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.memory3Panel.Location = new System.Drawing.Point(265, 82);
            this.memory3Panel.Name = "memory3Panel";
            this.memory3Panel.Size = new System.Drawing.Size(95, 21);
            this.memory3Panel.TabIndex = 71;
            this.memory3Panel.Visible = false;
            // 
            // memory4Panel
            // 
            this.memory4Panel.Controls.Add(this.memoryUpDown4);
            this.memory4Panel.Controls.Add(this.lblUpdown4);
            this.memory4Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.memory4Panel.Location = new System.Drawing.Point(265, 109);
            this.memory4Panel.Name = "memory4Panel";
            this.memory4Panel.Size = new System.Drawing.Size(95, 21);
            this.memory4Panel.TabIndex = 72;
            this.memory4Panel.Visible = false;
            // 
            // lessThanLabel2
            // 
            this.lessThanLabel2.AutoSize = true;
            this.lessThanLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lessThanLabel2.Location = new System.Drawing.Point(229, 56);
            this.lessThanLabel2.Name = "lessThanLabel2";
            this.lessThanLabel2.Size = new System.Drawing.Size(24, 17);
            this.lessThanLabel2.TabIndex = 73;
            this.lessThanLabel2.Text = "<=";
            this.lessThanLabel2.Visible = false;
            // 
            // lessThanLabel1
            // 
            this.lessThanLabel1.AutoSize = true;
            this.lessThanLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lessThanLabel1.Location = new System.Drawing.Point(229, 31);
            this.lessThanLabel1.Name = "lessThanLabel1";
            this.lessThanLabel1.Size = new System.Drawing.Size(24, 17);
            this.lessThanLabel1.TabIndex = 74;
            this.lessThanLabel1.Text = "<=";
            this.lessThanLabel1.Visible = false;
            // 
            // lessThanLabel3
            // 
            this.lessThanLabel3.AutoSize = true;
            this.lessThanLabel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lessThanLabel3.Location = new System.Drawing.Point(229, 85);
            this.lessThanLabel3.Name = "lessThanLabel3";
            this.lessThanLabel3.Size = new System.Drawing.Size(24, 17);
            this.lessThanLabel3.TabIndex = 75;
            this.lessThanLabel3.Text = "<=";
            this.lessThanLabel3.Visible = false;
            // 
            // lessThanLabel4
            // 
            this.lessThanLabel4.AutoSize = true;
            this.lessThanLabel4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lessThanLabel4.Location = new System.Drawing.Point(226, 111);
            this.lessThanLabel4.Name = "lessThanLabel4";
            this.lessThanLabel4.Size = new System.Drawing.Size(24, 17);
            this.lessThanLabel4.TabIndex = 76;
            this.lessThanLabel4.Text = "<=";
            this.lessThanLabel4.Visible = false;
            // 
            // tagsCombo1
            // 
            this.tagsCombo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagsCombo1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tagsCombo1.FormattingEnabled = true;
            this.tagsCombo1.Location = new System.Drawing.Point(265, 28);
            this.tagsCombo1.Name = "tagsCombo1";
            this.tagsCombo1.Size = new System.Drawing.Size(200, 21);
            this.tagsCombo1.TabIndex = 77;
            this.tagsCombo1.Visible = false;
            // 
            // tagsCombo2
            // 
            this.tagsCombo2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagsCombo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tagsCombo2.FormattingEnabled = true;
            this.tagsCombo2.Location = new System.Drawing.Point(265, 54);
            this.tagsCombo2.Name = "tagsCombo2";
            this.tagsCombo2.Size = new System.Drawing.Size(200, 21);
            this.tagsCombo2.TabIndex = 81;
            this.tagsCombo2.Visible = false;
            // 
            // tagsCombo3
            // 
            this.tagsCombo3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagsCombo3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tagsCombo3.FormattingEnabled = true;
            this.tagsCombo3.Location = new System.Drawing.Point(265, 82);
            this.tagsCombo3.Name = "tagsCombo3";
            this.tagsCombo3.Size = new System.Drawing.Size(200, 21);
            this.tagsCombo3.TabIndex = 82;
            this.tagsCombo3.Visible = false;
            // 
            // tagsCombo4
            // 
            this.tagsCombo4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagsCombo4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tagsCombo4.FormattingEnabled = true;
            this.tagsCombo4.Location = new System.Drawing.Point(265, 109);
            this.tagsCombo4.Name = "tagsCombo4";
            this.tagsCombo4.Size = new System.Drawing.Size(200, 21);
            this.tagsCombo4.TabIndex = 83;
            this.tagsCombo4.Visible = false;
            // 
            // ServerInventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ServerInventory";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryUpDown4)).EndInit();
            this.memory1Panel.ResumeLayout(false);
            this.memory1Panel.PerformLayout();
            this.memory2Panel.ResumeLayout(false);
            this.memory2Panel.PerformLayout();
            this.memory3Panel.ResumeLayout(false);
            this.memory3Panel.PerformLayout();
            this.memory4Panel.ResumeLayout(false);
            this.memory4Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox searchItem1Combo;
        private System.Windows.Forms.Label label2;
        private Controls.AllowDeleteTextBox searchValue1Textbox;
        private Controls.AllowDeleteTextBox searchValue4Textbox;
        private Controls.AllowDeleteTextBox searchValue3Textbox;
        private Controls.AllowDeleteTextBox searchValue2Textbox;
        private System.Windows.Forms.ComboBox searchItem4Combo;
        private System.Windows.Forms.ComboBox searchItem3Combo;
        private System.Windows.Forms.ComboBox searchItem2Combo;
        private System.Windows.Forms.Label like1Label;
        private System.Windows.Forms.Label like4Label;
        private System.Windows.Forms.Label like3Label;
        private System.Windows.Forms.Label like2Label;
        private System.Windows.Forms.Label wildCardLabel4;
        private System.Windows.Forms.Label wildCardLabel3;
        private System.Windows.Forms.Label wildCardLabel2;
        private System.Windows.Forms.Label wildCardLabel1;
        private System.Windows.Forms.NumericUpDown memoryUpDown1;
        private System.Windows.Forms.Panel memory3Panel;
        private System.Windows.Forms.Panel memory2Panel;
        private System.Windows.Forms.Panel memory1Panel;
        private System.Windows.Forms.Label lblUpdown4;
        private System.Windows.Forms.Label lblUpdown3;
        private System.Windows.Forms.Label lblUpdown2;
        private System.Windows.Forms.Label lblUpdown1;
        private System.Windows.Forms.NumericUpDown memoryUpDown4;
        private System.Windows.Forms.NumericUpDown memoryUpDown3;
        private System.Windows.Forms.NumericUpDown memoryUpDown2;
        private System.Windows.Forms.Panel memory4Panel;
        private System.Windows.Forms.Label lessThanLabel4;
        private System.Windows.Forms.Label lessThanLabel3;
        private System.Windows.Forms.Label lessThanLabel1;
        private System.Windows.Forms.Label lessThanLabel2;
        private System.Windows.Forms.ComboBox clusteredCombo4;
        private System.Windows.Forms.ComboBox clusteredCombo3;
        private System.Windows.Forms.ComboBox clusteredCombo2;
        private System.Windows.Forms.ComboBox clusteredCombo1;
        private System.Windows.Forms.ComboBox tagsCombo1;
        private System.Windows.Forms.ComboBox tagsCombo4;
        private System.Windows.Forms.ComboBox tagsCombo3;
        private System.Windows.Forms.ComboBox tagsCombo2;
    }
}
