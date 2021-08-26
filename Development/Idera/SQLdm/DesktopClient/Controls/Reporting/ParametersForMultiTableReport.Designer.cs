namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class ParametersForMultiTableReport {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tablesBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.databaseBox = new System.Windows.Forms.ComboBox();
            this.databaseLabel = new System.Windows.Forms.Label();
            this.serverBox = new System.Windows.Forms.ComboBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tablesBox
            // 
            this.tablesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tablesBox.FormattingEnabled = true;
            this.tablesBox.Location = new System.Drawing.Point(45, 28);
            this.tablesBox.Name = "tablesBox";
            this.tablesBox.Size = new System.Drawing.Size(373, 21);
            this.tablesBox.TabIndex = 49;
            this.tablesBox.DropDown += new System.EventHandler(this.tablesBox_DropDown);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "Tables:";
            // 
            // databaseBox
            // 
            this.databaseBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.databaseBox.FormattingEnabled = true;
            this.databaseBox.Items.AddRange(new object[] {
            "CorpDb01",
            "CorpData01",
            "HouDataCenter",
            "SQLDataServer",
            "WebSql"});
            this.databaseBox.Location = new System.Drawing.Point(266, 1);
            this.databaseBox.Name = "databaseBox";
            this.databaseBox.Size = new System.Drawing.Size(152, 21);
            this.databaseBox.TabIndex = 47;
            this.databaseBox.SelectedIndexChanged += new System.EventHandler(this.databaseBox_SelectedIndexChanged);
            // 
            // databaseLabel
            // 
            this.databaseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.Location = new System.Drawing.Point(209, 4);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(56, 13);
            this.databaseLabel.TabIndex = 46;
            this.databaseLabel.Text = "Database:";
            // 
            // serverBox
            // 
            this.serverBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Items.AddRange(new object[] {
            "CorpDb01",
            "CorpData01",
            "HouDataCenter",
            "SQLDataServer",
            "WebSql"});
            this.serverBox.Location = new System.Drawing.Point(45, 1);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(152, 21);
            this.serverBox.TabIndex = 45;
            this.serverBox.SelectedIndexChanged += new System.EventHandler(this.serverBox_SelectedIndexChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(0, 4);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 44;
            this.serverLabel.Text = "Server:";
            // 
            // ParametersForMultiTableReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tablesBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.databaseBox);
            this.Controls.Add(this.databaseLabel);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.serverLabel);
            this.Name = "ParametersForMultiTableReport";
            this.Size = new System.Drawing.Size(418, 52);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox tablesBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox databaseBox;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.ComboBox serverBox;
        private System.Windows.Forms.Label serverLabel;
    }
}
