using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class PropertiesControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(bool isDarkThemeSelected = false)
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.propertyPageListBox = new Idera.SQLdm.DesktopClient.Controls.Office2007ListBox(isDarkThemeSelected);
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.propertyPageListBox);
            this.splitContainer.Size = new System.Drawing.Size(405, 322);
            this.splitContainer.SplitterDistance = 150;
            this.splitContainer.TabIndex = 0;
            // 
            // propertyPageListBox
            // 
            this.propertyPageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPageListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.propertyPageListBox.FormattingEnabled = true;
            this.propertyPageListBox.Location = new System.Drawing.Point(0, 0);
            this.propertyPageListBox.Name = "propertyPageListBox";
            this.propertyPageListBox.Size = new System.Drawing.Size(150, 322);
            this.propertyPageListBox.TabIndex = 0;
            this.propertyPageListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.propertyPageListBox_MouseClick);
            this.propertyPageListBox.SelectedIndexChanged += new System.EventHandler(this.propertyPageListBox_SelectedIndexChanged);
            // 
            // PropertiesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "PropertiesControl";
            this.Size = new System.Drawing.Size(405, 322);
            if (isDarkThemeSelected)
                this.propertyPageListBox.BackColor = ColorTranslator.FromHtml("#012A4F");
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private Office2007ListBox propertyPageListBox;
    }
}
