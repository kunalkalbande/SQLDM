namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class ParametersForMultiDatabaseReport {
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
            this.databasesBox = new System.Windows.Forms.ComboBox();
            this.databaseLabel = new System.Windows.Forms.Label();
            this.serverBox = new System.Windows.Forms.ComboBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // databasesBox
            // 
            this.databasesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesBox.FormattingEnabled = true;
            this.databasesBox.Location = new System.Drawing.Point(61, 27);
            this.databasesBox.Name = "databasesBox";
            this.databasesBox.Size = new System.Drawing.Size(357, 21);
            this.databasesBox.TabIndex = 53;
            // 
            // databaseLabel
            // 
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.Location = new System.Drawing.Point(0, 30);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(61, 13);
            this.databaseLabel.TabIndex = 52;
            this.databaseLabel.Text = "Databases:";
            // 
            // serverBox
            // 
            this.serverBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Items.AddRange(new object[] {
            "CorpDb01",
            "CorpData01",
            "HouDataCenter",
            "SQLDataServer",
            "WebSql"});
            this.serverBox.Location = new System.Drawing.Point(61, 0);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(357, 21);
            this.serverBox.TabIndex = 51;
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(0, 3);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 50;
            this.serverLabel.Text = "Server:";
            // 
            // ParametersForMultiDatabaseReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.databasesBox);
            this.Controls.Add(this.databaseLabel);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.serverLabel);
            this.Name = "ParametersForMultiDatabaseReport";
            this.Size = new System.Drawing.Size(418, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox databasesBox;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.ComboBox serverBox;
        private System.Windows.Forms.Label serverLabel;
    }
}
