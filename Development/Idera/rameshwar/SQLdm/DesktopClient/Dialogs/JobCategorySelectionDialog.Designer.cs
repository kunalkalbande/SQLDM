namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class JobCategorySelectionDialog
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
            this.jobCategoriesListBox = new Infragistics.Win.UltraWinListView.UltraListView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.commentsInformationBox = new Divelements.WizardFramework.InformationBox();
            this.noneButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.jobCategoriesListBox)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // jobCategoriesListBox
            // 
            this.jobCategoriesListBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.jobCategoriesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraListViewItem1.Key = "1";
            ultraListViewItem2.Key = "2";
            ultraListViewItem3.Key = "3 ";
            this.jobCategoriesListBox.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1,
            ultraListViewItem2,
            ultraListViewItem3});
            this.jobCategoriesListBox.Location = new System.Drawing.Point(10, 54);
            this.jobCategoriesListBox.Name = "jobCategoriesListBox";
            this.jobCategoriesListBox.Size = new System.Drawing.Size(284, 210);
            this.jobCategoriesListBox.TabIndex = 5;
            this.jobCategoriesListBox.Text = "ultraListView1";
            this.jobCategoriesListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.jobCategoriesListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.jobCategoriesListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.jobCategoriesListBox.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.jobCategoriesListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.jobCategoriesListBox.ViewSettingsList.MultiColumn = false;
            this.jobCategoriesListBox.ItemCheckStateChanged += new Infragistics.Win.UltraWinListView.ItemCheckStateChangedEventHandler(this.jobCategoriesListBox_ItemCheckStateChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(223, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(61, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(156, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(61, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // commentsInformationBox
            // 
            this.commentsInformationBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commentsInformationBox.Location = new System.Drawing.Point(10, 10);
            this.commentsInformationBox.Name = "commentsInformationBox";
            this.commentsInformationBox.Size = new System.Drawing.Size(284, 44);
            this.commentsInformationBox.TabIndex = 7;
            this.commentsInformationBox.Text = "Specify the job categories to be excluded for this alert. ";
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
            // panel3
            // 
            this.panel3.Controls.Add(this.noneButton);
            this.panel3.Controls.Add(this.okButton);
            this.panel3.Controls.Add(this.cancelButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(10, 264);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(284, 33);
            this.panel3.TabIndex = 9;
            // 
            // JobCategorySelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 307);
            this.Controls.Add(this.jobCategoriesListBox);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.commentsInformationBox);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobCategorySelectionDialog";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Excluded Job Categories";
            ((System.ComponentModel.ISupportInitialize)(this.jobCategoriesListBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView jobCategoriesListBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private Divelements.WizardFramework.InformationBox commentsInformationBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button noneButton;
    }
}