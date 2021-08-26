namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.Common.Objects;

    partial class AffectedServersConfirmationDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.informationBox = new Divelements.WizardFramework.InformationBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.serverList = new System.Windows.Forms.ListBox();
            this.noneSelectedLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.stackLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(243, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(71, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(322, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // informationBox
            // 
            this.informationBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox.Location = new System.Drawing.Point(10, 10);
            this.informationBox.Name = "informationBox";
            this.informationBox.Size = new System.Drawing.Size(396, 69);
            this.informationBox.TabIndex = 2;
            this.informationBox.Text = "The following Custom Counters are currently monitoring the \'{0}\' counter.  Click " +
                "OK to delete the custom counter anyway.";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(10, 240);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 30);
            this.panel1.TabIndex = 7;
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = null;
            this.stackLayoutPanel1.Controls.Add(this.serverList);
            this.stackLayoutPanel1.Controls.Add(this.noneSelectedLabel);
            this.stackLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackLayoutPanel1.Location = new System.Drawing.Point(10, 79);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Size = new System.Drawing.Size(396, 161);
            this.stackLayoutPanel1.TabIndex = 8;
            // 
            // serverList
            // 
            this.serverList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverList.FormattingEnabled = true;
            this.serverList.Location = new System.Drawing.Point(0, 0);
            this.serverList.Name = "serverList";
            this.serverList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.serverList.Size = new System.Drawing.Size(396, 160);
            this.serverList.Sorted = true;
            this.serverList.TabIndex = 8;
            // 
            // noneSelectedLabel
            // 
            this.noneSelectedLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.noneSelectedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noneSelectedLabel.Location = new System.Drawing.Point(0, 0);
            this.noneSelectedLabel.Name = "noneSelectedLabel";
            this.noneSelectedLabel.Size = new System.Drawing.Size(396, 189);
            this.noneSelectedLabel.TabIndex = 7;
            this.noneSelectedLabel.Text = "None";
            this.noneSelectedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AffectedServersConfirmationDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(416, 280);
            this.Controls.Add(this.stackLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.informationBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AffectedServersConfirmationDialog";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Affected Instances";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AffectedServersConfirmationDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AffectedServersConfirmationDialog_HelpRequested);
            this.panel1.ResumeLayout(false);
            this.stackLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Divelements.WizardFramework.InformationBox informationBox;
        private System.Windows.Forms.Panel panel1;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private System.Windows.Forms.Label noneSelectedLabel;
        private System.Windows.Forms.ListBox serverList;
    }
}