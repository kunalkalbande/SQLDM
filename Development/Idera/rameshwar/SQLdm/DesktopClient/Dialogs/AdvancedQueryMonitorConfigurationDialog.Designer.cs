namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AdvancedQueryMonitorConfigurationDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.propertiesControl1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesControl();
            this.propertyPage1 = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.sqlTextFilterPanel = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sqlTextExcludeFilterTextBox = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.databasesFilterPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.databasesExcludeFilterTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.applicationsFilterPanel = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.propertiesHeaderStrip9 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.applicationsExcludeFilterTextBox = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.propertyPage1.SuspendLayout();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            this.sqlTextFilterPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.databasesFilterPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.applicationsFilterPanel.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(545, 441);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(464, 441);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // propertiesControl1
            // 
            this.propertiesControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesControl1.Location = new System.Drawing.Point(12, 12);
            this.propertiesControl1.Name = "propertiesControl1";
            this.propertiesControl1.PropertyPages.Add(this.propertyPage1);
            this.propertiesControl1.SelectedPropertyPageIndex = 0;
            this.propertiesControl1.Size = new System.Drawing.Size(608, 423);
            this.propertiesControl1.TabIndex = 4;
            // 
            // propertyPage1
            // 
            this.propertyPage1.Controls.Add(this.office2007PropertyPage1);
            this.propertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPage1.Location = new System.Drawing.Point(0, 0);
            this.propertyPage1.Name = "propertyPage1";
            this.propertyPage1.Size = new System.Drawing.Size(454, 423);
            this.propertyPage1.TabIndex = 0;
            this.propertyPage1.Text = "Filters";
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // office2007PropertyPage1.ContentPanel
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.sqlTextFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.databasesFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.applicationsFilterPanel);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(452, 366);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(454, 423);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Supply additional filtering criteria for the Query Monitor.";
            // 
            // sqlTextFilterPanel
            // 
            this.sqlTextFilterPanel.Controls.Add(this.panel6);
            this.sqlTextFilterPanel.Controls.Add(this.panel7);
            this.sqlTextFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sqlTextFilterPanel.Location = new System.Drawing.Point(1, 151);
            this.sqlTextFilterPanel.Name = "sqlTextFilterPanel";
            this.sqlTextFilterPanel.Size = new System.Drawing.Size(450, 75);
            this.sqlTextFilterPanel.TabIndex = 13;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.propertiesHeaderStrip2);
            this.panel6.Controls.Add(this.sqlTextExcludeFilterTextBox);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(450, 55);
            this.panel6.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Exclude:";
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip2.TabIndex = 1;
            this.propertiesHeaderStrip2.Text = "Which SQL text would you like to exclude from the Query Monitor?";
            // 
            // sqlTextExcludeFilterTextBox
            // 
            this.sqlTextExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextExcludeFilterTextBox.Location = new System.Drawing.Point(69, 34);
            this.sqlTextExcludeFilterTextBox.Multiline = true;
            this.sqlTextExcludeFilterTextBox.Name = "sqlTextExcludeFilterTextBox";
            this.sqlTextExcludeFilterTextBox.Size = new System.Drawing.Size(374, 20);
            this.sqlTextExcludeFilterTextBox.TabIndex = 2;
            this.sqlTextExcludeFilterTextBox.TextChanged += new System.EventHandler(this.sqlTextExcludeFilterTextBox_TextChanged);
            this.sqlTextExcludeFilterTextBox.Resize += new System.EventHandler(this.sqlTextExcludeFilterTextBox_Resize);
            this.sqlTextExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label4);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 55);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(450, 20);
            this.panel7.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(178, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(265, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // databasesFilterPanel
            // 
            this.databasesFilterPanel.Controls.Add(this.panel2);
            this.databasesFilterPanel.Controls.Add(this.panel3);
            this.databasesFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.databasesFilterPanel.Location = new System.Drawing.Point(1, 76);
            this.databasesFilterPanel.Name = "databasesFilterPanel";
            this.databasesFilterPanel.Size = new System.Drawing.Size(450, 75);
            this.databasesFilterPanel.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.propertiesHeaderStrip1);
            this.panel2.Controls.Add(this.databasesExcludeFilterTextBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(450, 55);
            this.panel2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Exclude:";
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip1.TabIndex = 1;
            this.propertiesHeaderStrip1.Text = "Which databases would you like to exclude from the Query Monitor?";
            // 
            // databasesExcludeFilterTextBox
            // 
            this.databasesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesExcludeFilterTextBox.Location = new System.Drawing.Point(69, 34);
            this.databasesExcludeFilterTextBox.Multiline = true;
            this.databasesExcludeFilterTextBox.Name = "databasesExcludeFilterTextBox";
            this.databasesExcludeFilterTextBox.Size = new System.Drawing.Size(374, 20);
            this.databasesExcludeFilterTextBox.TabIndex = 2;
            this.databasesExcludeFilterTextBox.TextChanged += new System.EventHandler(this.databasesExcludeFilterTextBox_TextChanged);
            this.databasesExcludeFilterTextBox.Resize += new System.EventHandler(this.databasesExcludeFilterTextBox_Resize);
            this.databasesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(450, 20);
            this.panel3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(178, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(265, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // applicationsFilterPanel
            // 
            this.applicationsFilterPanel.Controls.Add(this.panel5);
            this.applicationsFilterPanel.Controls.Add(this.panel8);
            this.applicationsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.applicationsFilterPanel.Location = new System.Drawing.Point(1, 1);
            this.applicationsFilterPanel.Name = "applicationsFilterPanel";
            this.applicationsFilterPanel.Size = new System.Drawing.Size(450, 75);
            this.applicationsFilterPanel.TabIndex = 11;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.propertiesHeaderStrip9);
            this.panel5.Controls.Add(this.applicationsExcludeFilterTextBox);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(450, 55);
            this.panel5.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Exclude:";
            // 
            // propertiesHeaderStrip9
            // 
            this.propertiesHeaderStrip9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip9.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertiesHeaderStrip9.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip9.Name = "propertiesHeaderStrip9";
            this.propertiesHeaderStrip9.Size = new System.Drawing.Size(436, 25);
            this.propertiesHeaderStrip9.TabIndex = 1;
            this.propertiesHeaderStrip9.Text = "Which applications would you like to exclude from the Query Monitor?";
            // 
            // applicationsExcludeFilterTextBox
            // 
            this.applicationsExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationsExcludeFilterTextBox.Location = new System.Drawing.Point(69, 34);
            this.applicationsExcludeFilterTextBox.Multiline = true;
            this.applicationsExcludeFilterTextBox.Name = "applicationsExcludeFilterTextBox";
            this.applicationsExcludeFilterTextBox.Size = new System.Drawing.Size(374, 20);
            this.applicationsExcludeFilterTextBox.TabIndex = 2;
            this.applicationsExcludeFilterTextBox.TextChanged += new System.EventHandler(this.applicationsExcludeFilterTextBox_TextChanged);
            this.applicationsExcludeFilterTextBox.Resize += new System.EventHandler(this.applicationsExcludeFilterTextBox_Resize);
            this.applicationsExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 55);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(450, 20);
            this.panel8.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(178, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(265, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // AdvancedQueryMonitorConfigurationDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(632, 476);
            this.Controls.Add(this.propertiesControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(580, 400);
            this.Name = "AdvancedQueryMonitorConfigurationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Query Monitor Configuration";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AdvancedQueryMonitorConfigurationDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AdvancedQueryMonitorConfigurationDialog_HelpRequested);
            this.propertyPage1.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.sqlTextFilterPanel.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.databasesFilterPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.applicationsFilterPanel.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesControl propertiesControl1;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage propertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private System.Windows.Forms.Panel sqlTextFilterPanel;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label3;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip2;
        private System.Windows.Forms.TextBox sqlTextExcludeFilterTextBox;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel databasesFilterPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private System.Windows.Forms.TextBox databasesExcludeFilterTextBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel applicationsFilterPanel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label12;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip9;
        private System.Windows.Forms.TextBox applicationsExcludeFilterTextBox;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label9;

    }
}