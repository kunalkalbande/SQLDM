namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ToDoPropertiesDialog
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
            if (disposing)
            {
                if (m_MetricValues != null) m_MetricValues.Dispose();

                if(components != null)
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this._btn_Cancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._btn_OK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._propertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this._txtbx_Message = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._lbl_CreatedOn = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._lbl_Server = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._lbl_Metric = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._statusCombo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._txtbx_Comments = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this._txtbx_Owner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._lbl_CompletedOn = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip3 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._propertyPage.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._statusCombo)).BeginInit();
            this.SuspendLayout();
            // 
            // _btn_Cancel
            // 
            this._btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btn_Cancel.Location = new System.Drawing.Point(474, 492);
            this._btn_Cancel.Name = "_btn_Cancel";
            this._btn_Cancel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this._btn_Cancel.TabIndex = 2;
            this._btn_Cancel.Text = "Cancel";
            this._btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // _btn_OK
            // 
            this._btn_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btn_OK.Location = new System.Drawing.Point(393, 492);
            this._btn_OK.Name = "_btn_OK";
            this._btn_OK.Size = new System.Drawing.Size(75, 23);
            this._btn_OK.TabIndex = 1;
            this._btn_OK.Text = "OK";
            this._btn_OK.UseVisualStyleBackColor = true;
            // 
            // _propertyPage
            // 
            this._propertyPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._propertyPage.BackColor = System.Drawing.Color.White;
            this._propertyPage.BorderWidth = 1;
            // 
            // _propertyPage.ContentPanel
            // 
            this._propertyPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this._propertyPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this._propertyPage.ContentPanel.Controls.Add(this._txtbx_Message);
            this._propertyPage.ContentPanel.Controls.Add(this.label9);
            this._propertyPage.ContentPanel.Controls.Add(this._lbl_CreatedOn);
            this._propertyPage.ContentPanel.Controls.Add(this._lbl_Server);
            this._propertyPage.ContentPanel.Controls.Add(this.label2);
            this._propertyPage.ContentPanel.Controls.Add(this._lbl_Metric);
            this._propertyPage.ContentPanel.Controls.Add(this._statusCombo);
            this._propertyPage.ContentPanel.Controls.Add(this.label15);
            this._propertyPage.ContentPanel.Controls.Add(this._txtbx_Comments);
            this._propertyPage.ContentPanel.Controls.Add(this._txtbx_Owner);
            this._propertyPage.ContentPanel.Controls.Add(this.label14);
            this._propertyPage.ContentPanel.Controls.Add(this._lbl_CompletedOn);
            this._propertyPage.ContentPanel.Controls.Add(this.label8);
            this._propertyPage.ContentPanel.Controls.Add(this.label6);
            this._propertyPage.ContentPanel.Controls.Add(this.propertiesHeaderStrip3);
            this._propertyPage.ContentPanel.Controls.Add(this.label13);
            this._propertyPage.ContentPanel.Controls.Add(this.label5);
            this._propertyPage.ContentPanel.Controls.Add(this.propertiesHeaderStrip1);
            this._propertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._propertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this._propertyPage.ContentPanel.Location = new System.Drawing.Point(2, 57);
            this._propertyPage.ContentPanel.Name = "ContentPanel";
            this._propertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this._propertyPage.ContentPanel.ShowBorder = false;
            this._propertyPage.ContentPanel.Size = new System.Drawing.Size(533, 415);
            this._propertyPage.ContentPanel.TabIndex = 1;
            this._propertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            this._propertyPage.Location = new System.Drawing.Point(12, 12);
            this._propertyPage.Name = "_propertyPage";
            this._propertyPage.Size = new System.Drawing.Size(537, 474);
            this._propertyPage.TabIndex = 0;
            this._propertyPage.Text = "Change To Do status.";
            // 
            // _txtbx_Message
            // 
            this._txtbx_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._txtbx_Message.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._txtbx_Message.Location = new System.Drawing.Point(56, 108);
            this._txtbx_Message.Multiline = true;
            this._txtbx_Message.Name = "_txtbx_Message";
            this._txtbx_Message.ReadOnly = true;
            this._txtbx_Message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtbx_Message.Size = new System.Drawing.Size(420, 68);
            this._txtbx_Message.TabIndex = 38;
            this._txtbx_Message.TabStop = false;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(294, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Created:";
            // 
            // _lbl_CreatedOn
            // 
            this._lbl_CreatedOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lbl_CreatedOn.AutoSize = true;
            this._lbl_CreatedOn.Location = new System.Drawing.Point(347, 44);
            this._lbl_CreatedOn.Name = "_lbl_CreatedOn";
            this._lbl_CreatedOn.Size = new System.Drawing.Size(129, 13);
            this._lbl_CreatedOn.TabIndex = 30;
            this._lbl_CreatedOn.Text = "12/31/2008 11:59:59 AM";
            // 
            // _lbl_Server
            // 
            this._lbl_Server.AutoSize = true;
            this._lbl_Server.Location = new System.Drawing.Point(100, 44);
            this._lbl_Server.Name = "_lbl_Server";
            this._lbl_Server.Size = new System.Drawing.Size(51, 13);
            this._lbl_Server.TabIndex = 32;
            this._lbl_Server.Text = "serverTxt";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server:";
            // 
            // _lbl_Metric
            // 
            this._lbl_Metric.AutoSize = true;
            this._lbl_Metric.Location = new System.Drawing.Point(100, 68);
            this._lbl_Metric.Name = "_lbl_Metric";
            this._lbl_Metric.Size = new System.Drawing.Size(50, 13);
            this._lbl_Metric.TabIndex = 33;
            this._lbl_Metric.Text = "metricTxt";
            // 
            // _statusCombo
            // 
            this._statusCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskNotStarted2;
            valueListItem1.Appearance = appearance1;
            valueListItem1.DataValue = "NotStarted";
            valueListItem1.DisplayText = "Not Started";
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskInProgress;
            valueListItem2.Appearance = appearance2;
            valueListItem2.DataValue = "InProgress";
            valueListItem2.DisplayText = "In Progress";
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskOnHold2;
            valueListItem3.Appearance = appearance3;
            valueListItem3.DataValue = "OnHold";
            valueListItem3.DisplayText = "On Hold";
            appearance4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskCompleted;
            valueListItem4.Appearance = appearance4;
            valueListItem4.DataValue = "Completed";
            valueListItem4.DisplayText = "Completed";
            this._statusCombo.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4});
            this._statusCombo.Location = new System.Drawing.Point(103, 214);
            this._statusCombo.Name = "_statusCombo";
            this._statusCombo.Size = new System.Drawing.Size(129, 21);
            this._statusCombo.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(53, 273);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "Comments:";
            // 
            // _txtbx_Comments
            // 
            this._txtbx_Comments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._txtbx_Comments.Location = new System.Drawing.Point(56, 289);
            this._txtbx_Comments.MaxLength = 1024;
            this._txtbx_Comments.Multiline = true;
            this._txtbx_Comments.Name = "_txtbx_Comments";
            this._txtbx_Comments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtbx_Comments.Size = new System.Drawing.Size(420, 117);
            this._txtbx_Comments.TabIndex = 9;
            // 
            // _txtbx_Owner
            // 
            this._txtbx_Owner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._txtbx_Owner.Location = new System.Drawing.Point(103, 241);
            this._txtbx_Owner.Name = "_txtbx_Owner";
            this._txtbx_Owner.Size = new System.Drawing.Size(373, 20);
            this._txtbx_Owner.TabIndex = 7;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(53, 244);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "Owner:";
            // 
            // _lbl_CompletedOn
            // 
            this._lbl_CompletedOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lbl_CompletedOn.AutoSize = true;
            this._lbl_CompletedOn.Location = new System.Drawing.Point(347, 217);
            this._lbl_CompletedOn.Name = "_lbl_CompletedOn";
            this._lbl_CompletedOn.Size = new System.Drawing.Size(129, 13);
            this._lbl_CompletedOn.TabIndex = 5;
            this._lbl_CompletedOn.Text = "12/31/2008 11:59:59 PM";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(281, 217);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Completed:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(53, 217);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Status:";
            // 
            // propertiesHeaderStrip3
            // 
            this.propertiesHeaderStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip3.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip3.Location = new System.Drawing.Point(7, 182);
            this.propertiesHeaderStrip3.Name = "propertiesHeaderStrip3";
            this.propertiesHeaderStrip3.Size = new System.Drawing.Size(516, 25);
            this.propertiesHeaderStrip3.TabIndex = 1;
            this.propertiesHeaderStrip3.TabStop = false;
            this.propertiesHeaderStrip3.Text = "Status";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(53, 92);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = "Details:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Metric:";
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(7, 7);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(516, 25);
            this.propertiesHeaderStrip1.TabIndex = 0;
            this.propertiesHeaderStrip1.TabStop = false;
            this.propertiesHeaderStrip1.Text = "Alert";
            // 
            // ToDoPropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 527);
            this.Controls.Add(this._btn_OK);
            this.Controls.Add(this._btn_Cancel);
            this.Controls.Add(this._propertyPage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToDoPropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "To Do Properties";
            this.Load += new System.EventHandler(this.ToDoPropertiesDialog_Load);
            this._propertyPage.ContentPanel.ResumeLayout(false);
            this._propertyPage.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._statusCombo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _btn_Cancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _btn_OK;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage _propertyPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox _txtbx_Comments;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox _txtbx_Owner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _lbl_CompletedOn;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _lbl_CreatedOn;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor _statusCombo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _lbl_Server;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _lbl_Metric;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox _txtbx_Message;

    }
}