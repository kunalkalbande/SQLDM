namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class StateChangeDialog
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("");
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.previousListView = new Idera.SQLdm.Common.UI.Controls.CheckedListView();
            this.previousSeverityHeader = new System.Windows.Forms.ColumnHeader();
            this.newListView = new Idera.SQLdm.Common.UI.Controls.CheckedListView();
            this.newSeverityHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(261, 111);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(180, 111);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // previousListView
            // 
            this.previousListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.previousListView.CheckBoxes = true;
            this.previousListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.previousSeverityHeader});
            this.previousListView.FullRowSelect = true;
            this.previousListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            this.previousListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.previousListView.Location = new System.Drawing.Point(12, 17);
            this.previousListView.Name = "previousListView";
            this.previousListView.Size = new System.Drawing.Size(156, 78);
            this.previousListView.TabIndex = 7;
            this.previousListView.UseCompatibleStateImageBehavior = false;
            this.previousListView.View = System.Windows.Forms.View.Details;
            this.previousListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.previousListView_ItemChecked);
            // 
            // previousSeverityHeader
            // 
            this.previousSeverityHeader.Text = "Previous Severity";
            this.previousSeverityHeader.Width = 150;
            // 
            // newListView
            // 
            this.newListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newListView.CheckBoxes = true;
            this.newListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.newSeverityHeader});
            this.newListView.FullRowSelect = true;
            this.newListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            this.newListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.newListView.Location = new System.Drawing.Point(180, 17);
            this.newListView.Name = "newListView";
            this.newListView.Size = new System.Drawing.Size(156, 78);
            this.newListView.TabIndex = 8;
            this.newListView.UseCompatibleStateImageBehavior = false;
            this.newListView.View = System.Windows.Forms.View.Details;
            this.newListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.newListView_ItemChecked);
            // 
            // newSeverityHeader
            // 
            this.newSeverityHeader.Text = "New Severity";
            this.newSeverityHeader.Width = 150;
            // 
            // StateChangeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 146);
            this.Controls.Add(this.newListView);
            this.Controls.Add(this.previousListView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StateChangeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "State Change";
            this.Load += new System.EventHandler(this.StateChangeDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.Common.UI.Controls.CheckedListView previousListView;
        private System.Windows.Forms.ColumnHeader previousSeverityHeader;
        private Idera.SQLdm.Common.UI.Controls.CheckedListView newListView;
        private System.Windows.Forms.ColumnHeader newSeverityHeader;
    }
}