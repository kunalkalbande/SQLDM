using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class AddActionDialog
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
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.providerListBox = new Idera.SQLdm.DesktopClient.Dialogs.Notification.CustomCheckedListBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(191, 306);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(272, 306);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // providerListBox
            //
            this.providerListBox.CheckBoxes = true;
            this.providerListBox.FullRowSelect = true;
            this.providerListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.providerListBox.LabelWrap = false;
            this.providerListBox.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.providerListBox.Location = new System.Drawing.Point(12, 12);
            this.providerListBox.Name = "providerListBox";
            this.providerListBox.OwnerDraw = true;
            this.providerListBox.Size = new System.Drawing.Size(335, 277);
            this.providerListBox.TabIndex = 2;
            this.providerListBox.UseCompatibleStateImageBehavior = false;
            this.providerListBox.View = System.Windows.Forms.View.Details;
            this.providerListBox.BackColor = backColor;
            this.providerListBox.ForeColor = foreColor;
            // 
            // AddActionDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(359, 341);
            this.Controls.Add(this.providerListBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddActionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Action Provider";
            this.BackColor = backColor;
            this.ForeColor = foreColor;
            this.Load += new System.EventHandler(this.AddActionDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private CustomCheckedListBox providerListBox;
    }
}