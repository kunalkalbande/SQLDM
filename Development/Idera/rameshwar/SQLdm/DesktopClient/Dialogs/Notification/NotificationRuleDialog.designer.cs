namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class NotificationRuleDialog
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Info, null, Infragistics.Win.DefaultableBoolean.Default);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Item 2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Item 3");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Item 4");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Item 5");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Item 6");
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.providersListBrowser = new System.Windows.Forms.WebBrowser();
            this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.rulePreview = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.addActionButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nameErrorPictureBox = new System.Windows.Forms.PictureBox();
            this.actionErrorPictureBox = new System.Windows.Forms.PictureBox();
            this.ruleErrorPictureBox = new System.Windows.Forms.PictureBox();
            this.tooltipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.conditionListBox = new Idera.SQLdm.DesktopClient.Dialogs.Notification.CustomCheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
            this.ultraGroupBox3.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nameErrorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionErrorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ruleErrorPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(340, 518);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(421, 518);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGroupBox1.ContentAreaAppearance = appearance1;
            this.ultraGroupBox1.ContentPadding.Bottom = 1;
            this.ultraGroupBox1.ContentPadding.Left = 1;
            this.ultraGroupBox1.ContentPadding.Right = 1;
            this.ultraGroupBox1.ContentPadding.Top = 1;
            this.ultraGroupBox1.Controls.Add(this.conditionListBox);
            this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 34);
            this.ultraGroupBox1.Margin = new System.Windows.Forms.Padding(30);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(488, 121);
            this.ultraGroupBox1.TabIndex = 2;
            this.ultraGroupBox1.Text = "Step 1:  Add condition(s)";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGroupBox2.ContentAreaAppearance = appearance2;
            this.ultraGroupBox2.ContentPadding.Bottom = 1;
            this.ultraGroupBox2.ContentPadding.Left = 1;
            this.ultraGroupBox2.ContentPadding.Right = 1;
            this.ultraGroupBox2.ContentPadding.Top = 1;
            this.ultraGroupBox2.Controls.Add(this.providersListBrowser);
            this.ultraGroupBox2.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(481, 147);
            this.ultraGroupBox2.TabIndex = 3;
            this.ultraGroupBox2.Text = "Step 2:  Select action(s)";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // providersListBrowser
            // 
            this.providersListBrowser.AllowWebBrowserDrop = false;
            this.providersListBrowser.CausesValidation = false;
            this.providersListBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.providersListBrowser.Location = new System.Drawing.Point(3, 21);
            this.providersListBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.providersListBrowser.Name = "providersListBrowser";
            this.providersListBrowser.Size = new System.Drawing.Size(475, 123);
            this.providersListBrowser.TabIndex = 2;
            this.providersListBrowser.WebBrowserShortcutsEnabled = false;
            this.providersListBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.providersListBrowser_Navigating);
            this.providersListBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.providersListBrowser_PreviewKeyDown);
            // 
            // ultraGroupBox3
            // 
            this.ultraGroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox3.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGroupBox3.ContentAreaAppearance = appearance3;
            this.ultraGroupBox3.ContentPadding.Bottom = 1;
            this.ultraGroupBox3.ContentPadding.Left = 1;
            this.ultraGroupBox3.ContentPadding.Right = 1;
            this.ultraGroupBox3.ContentPadding.Top = 1;
            this.ultraGroupBox3.Controls.Add(this.rulePreview);
            this.ultraGroupBox3.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
            this.ultraGroupBox3.Location = new System.Drawing.Point(0, 3);
            this.ultraGroupBox3.Name = "ultraGroupBox3";
            this.ultraGroupBox3.Size = new System.Drawing.Size(488, 170);
            this.ultraGroupBox3.TabIndex = 4;
            this.ultraGroupBox3.Text = "Step 3:  Edit the rule description (click an underlined value)";
            this.ultraGroupBox3.UseAppStyling = false;
            // 
            // rulePreview
            // 
            this.rulePreview.AllowWebBrowserDrop = false;
            this.rulePreview.CausesValidation = false;
            this.rulePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulePreview.IsWebBrowserContextMenuEnabled = false;
            this.rulePreview.Location = new System.Drawing.Point(3, 21);
            this.rulePreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.rulePreview.Name = "rulePreview";
            this.rulePreview.ScriptErrorsSuppressed = true;
            this.rulePreview.Size = new System.Drawing.Size(482, 146);
            this.rulePreview.TabIndex = 1;
            this.rulePreview.WebBrowserShortcutsEnabled = false;
            this.rulePreview.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.rulePreview_Navigating);
            this.rulePreview.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.rulePreview_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(17, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(61, 10);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(439, 20);
            this.txtDescription.TabIndex = 1;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // addActionButton
            // 
            this.addActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addActionButton.Location = new System.Drawing.Point(409, 153);
            this.addActionButton.Name = "addActionButton";
            this.addActionButton.Size = new System.Drawing.Size(75, 23);
            this.addActionButton.TabIndex = 7;
            this.addActionButton.Text = "Add";
            this.addActionButton.UseVisualStyleBackColor = true;
            this.addActionButton.Click += new System.EventHandler(this.addActionButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 155);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ultraGroupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(488, 356);
            this.splitContainer1.SplitterDistance = 178;
            this.splitContainer1.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ultraGroupBox2);
            this.panel1.Controls.Add(this.addActionButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(488, 178);
            this.panel1.TabIndex = 0;
            // 
            // nameErrorPictureBox
            // 
            this.nameErrorPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.nameErrorPictureBox.Location = new System.Drawing.Point(501, 12);
            this.nameErrorPictureBox.Name = "nameErrorPictureBox";
            this.nameErrorPictureBox.Size = new System.Drawing.Size(16, 16);
            this.nameErrorPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.nameErrorPictureBox.TabIndex = 9;
            this.nameErrorPictureBox.TabStop = false;
            ultraToolTipInfo1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            this.tooltipManager.SetUltraToolTip(this.nameErrorPictureBox, ultraToolTipInfo1);
            this.nameErrorPictureBox.Visible = false;
            // 
            // actionErrorPictureBox
            // 
            this.actionErrorPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.actionErrorPictureBox.Location = new System.Drawing.Point(501, 179);
            this.actionErrorPictureBox.Name = "actionErrorPictureBox";
            this.actionErrorPictureBox.Size = new System.Drawing.Size(16, 16);
            this.actionErrorPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.actionErrorPictureBox.TabIndex = 10;
            this.actionErrorPictureBox.TabStop = false;
            this.actionErrorPictureBox.Visible = false;
            // 
            // ruleErrorPictureBox
            // 
            this.ruleErrorPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.ruleErrorPictureBox.Location = new System.Drawing.Point(501, 362);
            this.ruleErrorPictureBox.Name = "ruleErrorPictureBox";
            this.ruleErrorPictureBox.Size = new System.Drawing.Size(16, 16);
            this.ruleErrorPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ruleErrorPictureBox.TabIndex = 11;
            this.ruleErrorPictureBox.TabStop = false;
            this.ruleErrorPictureBox.Visible = false;
            // 
            // tooltipManager
            // 
            this.tooltipManager.AutoPopDelay = 0;
            this.tooltipManager.ContainingControl = this;
            this.tooltipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            // 
            // conditionListBox
            // 
            this.conditionListBox.BoldLinks = true;
            this.conditionListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.conditionListBox.CheckBoxes = true;
            this.conditionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionListBox.FullRowSelect = true;
            this.conditionListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem1.Checked = true;
            listViewItem1.StateImageIndex = 1;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.Checked = true;
            listViewItem3.StateImageIndex = 1;
            listViewItem6.Checked = true;
            listViewItem6.StateImageIndex = 1;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.Checked = true;
            listViewItem5.StateImageIndex = 1;
            this.conditionListBox.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem6,
            listViewItem4,
            listViewItem5});
            this.conditionListBox.LabelWrap = false;
            this.conditionListBox.LinkColor = System.Drawing.SystemColors.WindowText;
            this.conditionListBox.Location = new System.Drawing.Point(3, 21);
            this.conditionListBox.Name = "conditionListBox";
            this.conditionListBox.OwnerDraw = true;
            this.conditionListBox.Size = new System.Drawing.Size(482, 97);
            this.conditionListBox.TabIndex = 2;
            this.conditionListBox.UseCompatibleStateImageBehavior = false;
            this.conditionListBox.View = System.Windows.Forms.View.Details;
            this.conditionListBox.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.conditionListBox_ItemChecked);
            // 
            // NotificationRuleDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(519, 553);
            this.Controls.Add(this.ruleErrorPictureBox);
            this.Controls.Add(this.actionErrorPictureBox);
            this.Controls.Add(this.nameErrorPictureBox);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NotificationRuleDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Response";
            this.Load += new System.EventHandler(this.NotificationRuleDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
            this.ultraGroupBox3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nameErrorPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionErrorPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ruleErrorPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private CustomCheckedListBox conditionListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.WebBrowser rulePreview;
        private System.Windows.Forms.Button addActionButton;
        private System.Windows.Forms.WebBrowser providersListBrowser;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox nameErrorPictureBox;
        private System.Windows.Forms.PictureBox actionErrorPictureBox;
        private System.Windows.Forms.PictureBox ruleErrorPictureBox;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager tooltipManager;
    }
}