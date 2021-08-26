namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class CheckedListDialog<ItemType>
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ultraListView1 = new Infragistics.Win.UltraWinListView.UltraListView();
            this.cbCheckitems = new System.Windows.Forms.CheckBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(110, 273);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(190, 273);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ultraListView1
            // 
            this.ultraListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraListView1.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            ultraListViewItem1.Key = "1";
            ultraListViewItem2.Key = "2";
            ultraListViewItem3.Key = "3 ";
            this.ultraListView1.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1,
            ultraListViewItem2,
            ultraListViewItem3});
            this.ultraListView1.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.None;
            this.ultraListView1.Location = new System.Drawing.Point(7, 33);
            this.ultraListView1.Name = "ultraListView1";
            this.ultraListView1.Size = new System.Drawing.Size(258, 231);
            this.ultraListView1.TabIndex = 2;
            this.ultraListView1.Text = "ultraListView1";
            this.ultraListView1.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.ultraListView1.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.ultraListView1.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.ultraListView1.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.ultraListView1.ViewSettingsList.MultiColumn = false;
            this.ultraListView1.ItemCheckStateChanged += new Infragistics.Win.UltraWinListView.ItemCheckStateChangedEventHandler(this.ultraListView1_ItemCheckStateChanged);
            // 
            // cbCheckitems
            // 
            this.cbCheckitems.BackColor = System.Drawing.Color.Transparent;
            this.cbCheckitems.Location = new System.Drawing.Point(9, 14);
            this.cbCheckitems.Name = "cbCheckitems";
            this.cbCheckitems.Size = new System.Drawing.Size(13, 19);
            this.cbCheckitems.TabIndex = 5;
            this.cbCheckitems.UseVisualStyleBackColor = false;
            this.cbCheckitems.CheckedChanged += new System.EventHandler(this.cbCheckitems_CheckedChanged);
            // 
            // ultraLabel1
            // 
            appearance1.BackColor = System.Drawing.Color.Silver;
            appearance1.BorderColor = System.Drawing.Color.Gray;
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.BorderStyleInner = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraLabel1.Location = new System.Drawing.Point(7, 11);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(258, 23);
            this.ultraLabel1.TabIndex = 4;
            this.ultraLabel1.Text = "      Drive(s) / Mount point(s)";
            this.ultraLabel1.WrapText = false;
            // 
            // CheckedListDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(271, 303);
            this.Controls.Add(this.cbCheckitems);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.ultraListView1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckedListDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.CheckedListDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Infragistics.Win.UltraWinListView.UltraListView ultraListView1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.CheckBox cbCheckitems;
    }
}