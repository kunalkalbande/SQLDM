using Idera.SQLdm.Common.UI.Controls;
namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    partial class RecommendationDetails
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.footer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.lblRecommendationType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblRecommendationDescription = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.priority = new PriorityBar();
            this.multiGradientPanel1 = new Idera.SQLdm.Common.UI.Controls.Panels.MultiGradientPanel();
            this.menuRichText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCopyRecommendationText = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.txtRecommendation = new RichTextBoxWithLinks();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSelectAll = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.footer.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.menuRichText.SuspendLayout();
            this.SuspendLayout();
            // 
            // footer
            // 
            this.footer.Controls.Add(this.flowLayoutPanel1);
            this.footer.Controls.Add(this.priority);
            this.footer.Controls.Add(this.multiGradientPanel1);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 223);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(499, 18);
            this.footer.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lblRecommendationType);
            this.flowLayoutPanel1.Controls.Add(this.lblRecommendationDescription);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(850, 17);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // lblRecommendationType
            // 
            this.lblRecommendationType.AutoSize = true;
            this.lblRecommendationType.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecommendationType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblRecommendationType.Location = new System.Drawing.Point(3, 2);
            this.lblRecommendationType.Name = "lblRecommendationType";
            this.lblRecommendationType.Size = new System.Drawing.Size(47, 14);
            this.lblRecommendationType.TabIndex = 0;
            this.lblRecommendationType.Text = "SDR-I00";
            this.lblRecommendationType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRecommendationDescription
            // 
            this.lblRecommendationDescription.AutoSize = true;
            this.lblRecommendationDescription.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecommendationDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblRecommendationDescription.Location = new System.Drawing.Point(56, 2);
            this.lblRecommendationDescription.Name = "lblRecommendationDescription";
            this.lblRecommendationDescription.Size = new System.Drawing.Size(47, 14);
            this.lblRecommendationDescription.TabIndex = 1;
            this.lblRecommendationDescription.Text = "SDR-I00";
            this.lblRecommendationDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // priority
            // 
            this.priority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.priority.Location = new System.Drawing.Point(416, 7);
            this.priority.Name = "priority";
            this.priority.Size = new System.Drawing.Size(80, 8);
            this.priority.TabIndex = 3;
            this.priority.Value = 20F;
            // 
            // multiGradientPanel1
            // 
            this.multiGradientPanel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.multiGradientPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.multiGradientPanel1.GradientColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(226)))), ((int)(((byte)(236))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(226)))), ((int)(((byte)(236))))),
        System.Drawing.Color.White};
            this.multiGradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.multiGradientPanel1.Name = "multiGradientPanel1";
            this.multiGradientPanel1.Rotation = 0F;
            this.multiGradientPanel1.Size = new System.Drawing.Size(499, 1);
            this.multiGradientPanel1.TabIndex = 2;
            // 
            // menuRichText
            // 
            this.menuRichText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopyRecommendationText,
            this.toolStripSeparator1,
            this.menuSelectAll});
            this.menuRichText.Name = "menuRichText";
            this.menuRichText.Size = new System.Drawing.Size(165, 76);
            this.menuRichText.Opening += new System.ComponentModel.CancelEventHandler(this.menuRichText_Opening);
            // 
            // menuCopyRecommendationText
            // 
            this.menuCopyRecommendationText.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            this.menuCopyRecommendationText.Name = "menuCopyRecommendationText";
            this.menuCopyRecommendationText.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopyRecommendationText.Size = new System.Drawing.Size(164, 22);
            this.menuCopyRecommendationText.Text = "Copy";
            this.menuCopyRecommendationText.Click += new System.EventHandler(this.menuCopyRecommendationText_Click);
            // 
            // txtRecommendation
            // 
            this.txtRecommendation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRecommendation.ContextMenuStrip = this.menuRichText;
            this.txtRecommendation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecommendation.Font = new System.Drawing.Font("Arial", 9F);
            this.txtRecommendation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.txtRecommendation.Location = new System.Drawing.Point(0, 0);
            this.txtRecommendation.Name = "txtRecommendation";
            this.txtRecommendation.ReadOnly = true;
            this.txtRecommendation.Size = new System.Drawing.Size(499, 223);
            this.txtRecommendation.TabIndex = 1;
            this.txtRecommendation.Text = "";
            this.txtRecommendation.ObjectLinkClicked += new RichTextBoxWithLinks.ObjectLinkClickedEventHandler(this.txtRecommendation_ObjectLinkClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            // 
            // menuSelectAll
            // 
            this.menuSelectAll.Name = "menuSelectAll";
            this.menuSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.menuSelectAll.Size = new System.Drawing.Size(164, 22);
            this.menuSelectAll.Text = "Select All";
            this.menuSelectAll.Click += new System.EventHandler(this.menuSelectAll_Click);
            // 
            // RecommendationDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.txtRecommendation);
            this.Controls.Add(this.footer);
            this.Name = "RecommendationDetails";
            this.Size = new System.Drawing.Size(499, 241);
            this.footer.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.menuRichText.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RichTextBoxWithLinks txtRecommendation;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  footer;
        private Idera.SQLdm.Common.UI.Controls.Panels.MultiGradientPanel multiGradientPanel1;
        private PriorityBar priority;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblRecommendationType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblRecommendationDescription;
        private System.Windows.Forms.ContextMenuStrip menuRichText;
        private System.Windows.Forms.ToolStripMenuItem menuCopyRecommendationText;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSelectAll;

    }
}
