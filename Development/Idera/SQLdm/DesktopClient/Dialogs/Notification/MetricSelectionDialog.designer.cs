using Idera.SQLdm.Common.UI.Controls;
namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class MetricSelectionDialog
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.metricsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.chkbxSelectAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(246, 314);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(165, 314);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // metricsListView
            // 
            this.metricsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.metricsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metricsListView.CheckBoxes = true;
            this.metricsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.metricsListView.FullRowSelect = true;
            this.metricsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.metricsListView.Location = new System.Drawing.Point(12, 59);
            this.metricsListView.MultiSelect = false;
            this.metricsListView.Name = "metricsListView";
            this.metricsListView.Size = new System.Drawing.Size(309, 249);
            this.metricsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.metricsListView.TabIndex = 1;
            this.metricsListView.UseCompatibleStateImageBehavior = false;
            this.metricsListView.View = System.Windows.Forms.View.Details;
            this.metricsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.metricsListView_ItemChecked);
            this.metricsListView.DoubleClick += new System.EventHandler(this.metricsListView_DoubleClick);
            this.metricsListView.SelectedIndexChanged += new System.EventHandler(this.metricsListView_SelectedIndexChanged);
            this.metricsListView.SizeChanged += new System.EventHandler(this.metricsListView_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 280;
            // 
            // chkbxSelectAll
            // 
            this.chkbxSelectAll.AutoSize = true;
            this.chkbxSelectAll.Location = new System.Drawing.Point(18, 36);
            this.chkbxSelectAll.Name = "chkbxSelectAll";
            this.chkbxSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkbxSelectAll.TabIndex = 0;
            this.chkbxSelectAll.Text = "Select All";
            this.chkbxSelectAll.UseVisualStyleBackColor = true;
            this.chkbxSelectAll.CheckedChanged += new System.EventHandler(this.chkbxSelectAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select a metric.";
            // 
            // MetricSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 349);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkbxSelectAll);
            this.Controls.Add(this.metricsListView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetricSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Metrics";
            this.Load += new System.EventHandler(this.MetricSelectionDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListView metricsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox chkbxSelectAll;
        private System.Windows.Forms.Label label1;
    }
}