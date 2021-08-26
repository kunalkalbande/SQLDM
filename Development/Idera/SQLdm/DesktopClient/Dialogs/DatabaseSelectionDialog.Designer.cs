namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class DatabaseSelectionDialog
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
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem1 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 1", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem2 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 2", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem3 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 3", null, null);
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.noneButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.databaseListBox = new Infragistics.Win.UltraWinListView.UltraListView();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.commentsInformationBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            ((System.ComponentModel.ISupportInitialize)(this.databaseListBox)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(158, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(61, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(225, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(61, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // noneButton
            // 
            this.noneButton.Location = new System.Drawing.Point(3, 6);
            this.noneButton.Name = "noneButton";
            this.noneButton.Size = new System.Drawing.Size(61, 23);
            this.noneButton.TabIndex = 0;
            this.noneButton.Text = "Clear All";
            this.noneButton.UseVisualStyleBackColor = true;
            this.noneButton.Click += new System.EventHandler(this.noneButton_Click);
            // 
            // databaseListBox
            // 
            this.databaseListBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.databaseListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraListViewItem1.Key = "1";
            ultraListViewItem2.Key = "2";
            ultraListViewItem3.Key = "3 ";
            this.databaseListBox.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1,
            ultraListViewItem2,
            ultraListViewItem3});
            this.databaseListBox.Location = new System.Drawing.Point(10, 54);
            this.databaseListBox.MainColumn.ShowSortIndicators = Infragistics.Win.DefaultableBoolean.False;
            this.databaseListBox.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.databaseListBox.Name = "databaseListBox";
            this.databaseListBox.Size = new System.Drawing.Size(286, 246);
            this.databaseListBox.TabIndex = 10;
            this.databaseListBox.Text = "ultraListView1";
            this.databaseListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.databaseListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.databaseListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.databaseListBox.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.databaseListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.databaseListBox.ViewSettingsList.MultiColumn = false;
            this.databaseListBox.ItemCheckStateChanged += new Infragistics.Win.UltraWinListView.ItemCheckStateChangedEventHandler(this.databaseListBox_ItemCheckStateChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.noneButton);
            this.panel3.Controls.Add(this.okButton);
            this.panel3.Controls.Add(this.cancelButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(10, 300);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(286, 33);
            this.panel3.TabIndex = 12;
            // 
            // commentsInformationBox
            // 
            this.commentsInformationBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commentsInformationBox.Location = new System.Drawing.Point(10, 10);
            this.commentsInformationBox.Name = "commentsInformationBox";
            this.commentsInformationBox.Size = new System.Drawing.Size(286, 44);
            this.commentsInformationBox.TabIndex = 11;
            this.commentsInformationBox.Text = "Specify the databases to be excluded for this alert. ";
            // 
            // DatabaseSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 343);
            this.Controls.Add(this.databaseListBox);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.commentsInformationBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseSelectionDialog";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Excluded Databases";
            ((System.ComponentModel.ISupportInitialize)(this.databaseListBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton noneButton;
        private Infragistics.Win.UltraWinListView.UltraListView databaseListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox commentsInformationBox;
    }
}