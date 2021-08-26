using Idera.SQLdm.DesktopClient.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class DashboardControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        /// 
       
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (alertToolTip != null)
                    alertToolTip.Dispose();

                if(components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardControl));
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dashboardHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.DashboardHeaderStrip();
            this.headerCaptionButton = new System.Windows.Forms.ToolStripButton();
            this.headerSelectTypeSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.headerSelectTypeDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.headerSelectOptionSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.headerSelectOptionDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.headerActionButtonSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.headerActionButton = new System.Windows.Forms.ToolStripButton();
            this.headerStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.dashboardHeaderStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // dashboardHeaderStrip
            // 
            this.dashboardHeaderStrip.AllowDrop = true;
            this.dashboardHeaderStrip.AutoSize = false;
            this.dashboardHeaderStrip.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dashboardHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.dashboardHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.headerCaptionButton,
            this.headerSelectTypeSeparator,
            this.headerSelectTypeDropDownButton,
            this.headerSelectOptionSeparator,
            this.headerSelectOptionDropDownButton,
            this.headerActionButtonSeparator,
            this.headerActionButton,
            this.headerStatusLabel});
            this.dashboardHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.dashboardHeaderStrip.Name = "dashboardHeaderStrip";
            this.dashboardHeaderStrip.Size = new System.Drawing.Size(600, 19);
            this.dashboardHeaderStrip.TabIndex = 0;
            this.dashboardHeaderStrip.Text = "dashboardHeaderStrip";
            this.dashboardHeaderStrip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DashboardControl_MouseMove);
            // 
            // headerCaptionButton
            // 
            this.headerCaptionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.headerCaptionButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerCaptionButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerCaptionButton.Image = ((System.Drawing.Image)(resources.GetObject("headerCaptionButton.Image")));
            this.headerCaptionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.headerCaptionButton.Name = "headerCaptionButton";
            this.headerCaptionButton.Size = new System.Drawing.Size(116, 16);
            this.headerCaptionButton.Text = "Dashboard Control";
            this.headerCaptionButton.Click += new System.EventHandler(this.headerCaptionButton_Click);
            // 
            // headerSelectTypeSeparator
            // 
            this.headerSelectTypeSeparator.ForeColor = System.Drawing.Color.White;
            this.headerSelectTypeSeparator.Name = "headerSelectTypeSeparator";
            this.headerSelectTypeSeparator.Size = new System.Drawing.Size(6, 19);
            this.headerSelectTypeSeparator.Visible = false;
            // 
            // headerSelectTypeDropDownButton
            // 
            this.headerSelectTypeDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.headerSelectTypeDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerSelectTypeDropDownButton.Margin = new System.Windows.Forms.Padding(0);
            this.headerSelectTypeDropDownButton.Name = "headerSelectTypeDropDownButton";
            this.headerSelectTypeDropDownButton.Size = new System.Drawing.Size(108, 19);
            this.headerSelectTypeDropDownButton.Text = "Select a Chart...";
            this.headerSelectTypeDropDownButton.Visible = false;
            // 
            // headerSelectOptionSeparator
            // 
            this.headerSelectOptionSeparator.ForeColor = System.Drawing.Color.White;
            this.headerSelectOptionSeparator.Name = "headerSelectOptionSeparator";
            this.headerSelectOptionSeparator.Size = new System.Drawing.Size(6, 19);
            this.headerSelectOptionSeparator.Visible = false;
            // 
            // headerSelectOptionDropDownButton
            // 
            this.headerSelectOptionDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.headerSelectOptionDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerSelectOptionDropDownButton.Margin = new System.Windows.Forms.Padding(0);
            this.headerSelectOptionDropDownButton.Name = "headerSelectOptionDropDownButton";
            this.headerSelectOptionDropDownButton.Size = new System.Drawing.Size(121, 19);
            this.headerSelectOptionDropDownButton.Text = "Select an Option...";
            this.headerSelectOptionDropDownButton.Visible = false;
            // 
            // headerActionButtonSeparator
            // 
            this.headerActionButtonSeparator.Name = "headerActionButtonSeparator";
            this.headerActionButtonSeparator.Size = new System.Drawing.Size(6, 19);
            this.headerActionButtonSeparator.Visible = false;
            // 
            // headerActionButton
            // 
            this.headerActionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.headerActionButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.headerActionButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerActionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.headerActionButton.Margin = new System.Windows.Forms.Padding(0);
            this.headerActionButton.Name = "headerActionButton";
            this.headerActionButton.Size = new System.Drawing.Size(50, 19);
            this.headerActionButton.Text = "Show...";
            this.headerActionButton.Visible = false;
            // 
            // headerStatusLabel
            // 
            this.headerStatusLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.headerStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.headerStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerStatusLabel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
            this.headerStatusLabel.Name = "headerStatusLabel";
            this.headerStatusLabel.Size = new System.Drawing.Size(16, 16);
            this.headerStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.headerStatusLabel.Visible = false;
            // 
            // DashboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.dashboardHeaderStrip);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DashboardControl";
            this.Size = new System.Drawing.Size(600, 184);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DashboardControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DashboardControl_DragEnter);
            this.DragLeave += new System.EventHandler(this.DashboardControl_DragLeave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DashboardControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DashboardControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DashboardControl_MouseUp);
            this.dashboardHeaderStrip.ResumeLayout(false);
            this.dashboardHeaderStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal DashboardHeaderStrip dashboardHeaderStrip;
        private System.Windows.Forms.ToolStripButton headerCaptionButton;
        protected internal System.Windows.Forms.ToolStripDropDownButton headerSelectTypeDropDownButton;
        protected internal System.Windows.Forms.ToolStripSeparator headerSelectTypeSeparator;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        protected internal System.Windows.Forms.ToolStripDropDownButton headerSelectOptionDropDownButton;
        protected internal System.Windows.Forms.ToolStripSeparator headerSelectOptionSeparator;
        protected internal System.Windows.Forms.ToolStripButton headerActionButton;
        protected internal System.Windows.Forms.ToolStripLabel headerStatusLabel;
        protected internal System.Windows.Forms.ToolStripSeparator headerActionButtonSeparator;


    }
}
