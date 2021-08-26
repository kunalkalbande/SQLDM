namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class GenericFilterDialog
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
           this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
           this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
           this.filterPropertiesGrid = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPropertyGrid();
           this.resetButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
           this.SuspendLayout();
           // 
           // cancelButton
           // 
           this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           this.cancelButton.Location = new System.Drawing.Point(347, 320);
           this.cancelButton.Name = "cancelButton";
           this.cancelButton.Size = new System.Drawing.Size(75, 23);
           this.cancelButton.TabIndex = 1;
           this.cancelButton.Text = "Cancel";
           this.cancelButton.UseVisualStyleBackColor = true;
           // 
           // okButton
           // 
           this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
           this.okButton.Location = new System.Drawing.Point(185, 320);
           this.okButton.Name = "okButton";
           this.okButton.Size = new System.Drawing.Size(75, 23);
           this.okButton.TabIndex = 0;
           this.okButton.Text = "OK";
           this.okButton.UseVisualStyleBackColor = true;
           this.okButton.Click += new System.EventHandler(this.okButton_Click);
           // 
           // filterPropertiesGrid
           // 
           this.filterPropertiesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                       | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.filterPropertiesGrid.Location = new System.Drawing.Point(0, 0);
           this.filterPropertiesGrid.Name = "filterPropertiesGrid";
           this.filterPropertiesGrid.Size = new System.Drawing.Size(434, 314);
           this.filterPropertiesGrid.TabIndex = 2;
           // 
           // resetButton
           // 
           this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.resetButton.Location = new System.Drawing.Point(266, 320);
           this.resetButton.Name = "resetButton";
           this.resetButton.Size = new System.Drawing.Size(75, 23);
           this.resetButton.TabIndex = 3;
           this.resetButton.Text = "Reset";
           this.resetButton.UseVisualStyleBackColor = true;
           this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
           // 
           // GenericFilterDialog
           // 
           this.AcceptButton = this.okButton;
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.CancelButton = this.cancelButton;
           this.ClientSize = new System.Drawing.Size(434, 351);
           this.Controls.Add(this.resetButton);
           this.Controls.Add(this.filterPropertiesGrid);
           this.Controls.Add(this.okButton);
           this.Controls.Add(this.cancelButton);
           this.HelpButton = true;
           this.MaximizeBox = false;
           this.MinimizeBox = false;
           this.Name = "GenericFilterDialog";
           this.ShowInTaskbar = false;
           this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Filter Settings";
           this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.GenericFilterDialog_HelpButtonClicked);
           this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.GenericFilterDialog_HelpRequested);
           this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private System.Windows.Forms.PropertyGrid filterPropertiesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton resetButton;
    }
}