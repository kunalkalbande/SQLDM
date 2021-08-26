namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    partial class FiltersSettingsTab
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
            this.labelApplicationName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._applicationNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.propertiesHeaderStrip1 = new PropertiesHeaderStrip();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._databaseFilterTypeComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.propertiesHeaderStrip2 = new PropertiesHeaderStrip();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // labelApplicationName
            // 
            this.labelApplicationName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelApplicationName.AutoSize = true;
            this.labelApplicationName.Location = new System.Drawing.Point(3, 3);
            this.labelApplicationName.Name = "labelApplicationName";
            this.labelApplicationName.Size = new System.Drawing.Size(78, 13);
            this.labelApplicationName.TabIndex = 3;
            this.labelApplicationName.Text = "Application name (% wildcard):";
            // 
            // _analysisResultsLocationTextBox
            // 
            this._applicationNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._applicationNameTextBox.Location = new System.Drawing.Point(155, 40);
            this._applicationNameTextBox.Name = "_analysisResultsLocationTextBox";
            this._applicationNameTextBox.Size = new System.Drawing.Size(200, 20);
            this._applicationNameTextBox.TabIndex = 0;
            this._applicationNameTextBox.TextChanged += new System.EventHandler(this.filterApplicationText_TextChanged);
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(422, 25);
            this.propertiesHeaderStrip1.TabIndex = 2;
            this.propertiesHeaderStrip1.Text = "Would you like to analyze workload from a specific application?";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._databaseFilterTypeComboBox, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 99);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(419, 79);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database Name:";
            // 
            // _authenticationTypeComboBox
            // 
            this._databaseFilterTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._databaseFilterTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._databaseFilterTypeComboBox.FormattingEnabled = true;
            this._databaseFilterTypeComboBox.Location = new System.Drawing.Point(94, 3);
            this._databaseFilterTypeComboBox.Name = "_authenticationTypeComboBox";
            this._databaseFilterTypeComboBox.Size = new System.Drawing.Size(322, 21);
            this._databaseFilterTypeComboBox.TabIndex = 0;
            this._databaseFilterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._databaseFilterComboBox_SelectedIndexChanged);
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertiesHeaderStrip2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(3, 68);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(422, 25);
            this.propertiesHeaderStrip2.TabIndex = 13;
            this.propertiesHeaderStrip2.Text = "Which Database should be used to filter?";
            // 
            // GeneralSettingsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertiesHeaderStrip2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.labelApplicationName);
            this.Controls.Add(this._applicationNameTextBox);
            this.Controls.Add(this.propertiesHeaderStrip1);
            this.Name = "FiltersSettingsTab";
            this.Size = new System.Drawing.Size(428, 335);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel labelApplicationName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox _applicationNameTextBox;
        private PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox _databaseFilterTypeComboBox;
        private PropertiesHeaderStrip propertiesHeaderStrip2;
       
    }
}
