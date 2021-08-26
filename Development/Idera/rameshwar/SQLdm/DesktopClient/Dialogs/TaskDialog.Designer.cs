namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class TaskDialog
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
            this.detailsTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.subject = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.commentsDetails = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.instance = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.metric = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.value = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.completed = new System.Windows.Forms.Label();
            this.created = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.statusDetailCombo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ownerDetails = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.message = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.severityImage = new System.Windows.Forms.PictureBox();
            this.severity = new System.Windows.Forms.Label();
            this.detailsTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusDetailCombo)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(305, 277);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(386, 277);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // detailsTablePanel
            // 
            this.detailsTablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTablePanel.BackColor = System.Drawing.Color.Transparent;
            this.detailsTablePanel.ColumnCount = 4;
            this.detailsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.detailsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.detailsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.detailsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsTablePanel.Controls.Add(this.label2, 0, 0);
            this.detailsTablePanel.Controls.Add(this.label7, 0, 3);
            this.detailsTablePanel.Controls.Add(this.subject, 1, 3);
            this.detailsTablePanel.Controls.Add(this.label10, 0, 4);
            this.detailsTablePanel.Controls.Add(this.commentsDetails, 1, 6);
            this.detailsTablePanel.Controls.Add(this.label12, 0, 6);
            this.detailsTablePanel.Controls.Add(this.instance, 3, 0);
            this.detailsTablePanel.Controls.Add(this.label3, 2, 0);
            this.detailsTablePanel.Controls.Add(this.label8, 0, 1);
            this.detailsTablePanel.Controls.Add(this.metric, 1, 1);
            this.detailsTablePanel.Controls.Add(this.label9, 0, 2);
            this.detailsTablePanel.Controls.Add(this.value, 1, 2);
            this.detailsTablePanel.Controls.Add(this.label6, 2, 2);
            this.detailsTablePanel.Controls.Add(this.completed, 3, 2);
            this.detailsTablePanel.Controls.Add(this.created, 3, 1);
            this.detailsTablePanel.Controls.Add(this.label1, 2, 1);
            this.detailsTablePanel.Controls.Add(this.label5, 0, 5);
            this.detailsTablePanel.Controls.Add(this.statusDetailCombo, 1, 5);
            this.detailsTablePanel.Controls.Add(this.ownerDetails, 3, 5);
            this.detailsTablePanel.Controls.Add(this.label4, 2, 5);
            this.detailsTablePanel.Controls.Add(this.message, 1, 4);
            this.detailsTablePanel.Controls.Add(this.panel1, 1, 0);
            this.detailsTablePanel.Location = new System.Drawing.Point(8, 8);
            this.detailsTablePanel.MinimumSize = new System.Drawing.Size(150, 187);
            this.detailsTablePanel.Name = "detailsTablePanel";
            this.detailsTablePanel.RowCount = 8;
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsTablePanel.Size = new System.Drawing.Size(457, 259);
            this.detailsTablePanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Severity:";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(3, 63);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Subject:";
            // 
            // subject
            // 
            this.subject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTablePanel.SetColumnSpan(this.subject, 3);
            this.subject.Location = new System.Drawing.Point(68, 63);
            this.subject.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.subject.Name = "subject";
            this.subject.Size = new System.Drawing.Size(386, 17);
            this.subject.TabIndex = 16;
            this.subject.Text = "3sub";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.Location = new System.Drawing.Point(3, 83);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 17);
            this.label10.TabIndex = 21;
            this.label10.Text = "Message:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // commentsDetails
            // 
            this.commentsDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTablePanel.SetColumnSpan(this.commentsDetails, 3);
            this.commentsDetails.Location = new System.Drawing.Point(68, 136);
            this.commentsDetails.MaxLength = 1024;
            this.commentsDetails.MinimumSize = new System.Drawing.Size(4, 20);
            this.commentsDetails.Multiline = true;
            this.commentsDetails.Name = "commentsDetails";
            this.commentsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.commentsDetails.Size = new System.Drawing.Size(386, 120);
            this.commentsDetails.TabIndex = 2;
            this.commentsDetails.Text = "Variable height";
            this.commentsDetails.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.Location = new System.Drawing.Point(3, 136);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 17);
            this.label12.TabIndex = 26;
            this.label12.Text = "Comments:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // instance
            // 
            this.instance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.instance.Location = new System.Drawing.Point(270, 3);
            this.instance.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.instance.Name = "instance";
            this.instance.Size = new System.Drawing.Size(184, 17);
            this.instance.TabIndex = 6;
            this.instance.Text = "inst";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(203, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Instance:";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(3, 23);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 17);
            this.label8.TabIndex = 17;
            this.label8.Text = "Metric:";
            // 
            // metric
            // 
            this.metric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metric.Location = new System.Drawing.Point(68, 23);
            this.metric.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.metric.Name = "metric";
            this.metric.Size = new System.Drawing.Size(129, 17);
            this.metric.TabIndex = 19;
            this.metric.Text = "metric name";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(3, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 17);
            this.label9.TabIndex = 18;
            this.label9.Text = "Value:";
            // 
            // value
            // 
            this.value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.value.Location = new System.Drawing.Point(68, 43);
            this.value.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(129, 17);
            this.value.TabIndex = 20;
            this.value.Text = "metric value";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(203, 43);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Completed:";
            // 
            // completed
            // 
            this.completed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.completed.Location = new System.Drawing.Point(270, 43);
            this.completed.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.completed.Name = "completed";
            this.completed.Size = new System.Drawing.Size(184, 17);
            this.completed.TabIndex = 12;
            this.completed.Text = "date";
            // 
            // created
            // 
            this.created.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.created.Location = new System.Drawing.Point(270, 23);
            this.created.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.created.Name = "created";
            this.created.Size = new System.Drawing.Size(184, 17);
            this.created.TabIndex = 4;
            this.created.Text = "XX/XX/XXXX XX:XX:XX";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(203, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Created:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(3, 112);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Status:";
            // 
            // statusDetailCombo
            // 
            this.statusDetailCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.statusDetailCombo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.statusDetailCombo.Location = new System.Drawing.Point(68, 109);
            this.statusDetailCombo.Name = "statusDetailCombo";
            this.statusDetailCombo.Size = new System.Drawing.Size(129, 21);
            this.statusDetailCombo.TabIndex = 0;
            this.statusDetailCombo.SelectionChangeCommitted += new System.EventHandler(this.statusDetailCombo_SelectionChangeCommitted);
            // 
            // ownerDetails
            // 
            this.ownerDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ownerDetails.Location = new System.Drawing.Point(270, 109);
            this.ownerDetails.MaxLength = 256;
            this.ownerDetails.Name = "ownerDetails";
            this.ownerDetails.Size = new System.Drawing.Size(184, 20);
            this.ownerDetails.TabIndex = 1;
            this.ownerDetails.Text = "owner";
            this.ownerDetails.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(203, 112);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Owner:";
            // 
            // message
            // 
            this.message.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTablePanel.SetColumnSpan(this.message, 3);
            this.message.Location = new System.Drawing.Point(68, 83);
            this.message.MaxLength = 256;
            this.message.Multiline = true;
            this.message.Name = "message";
            this.message.ReadOnly = true;
            this.message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.message.Size = new System.Drawing.Size(386, 20);
            this.message.TabIndex = 28;
            this.message.TabStop = false;
            this.message.Text = "up to three lines";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.severityImage);
            this.panel1.Controls.Add(this.severity);
            this.panel1.Location = new System.Drawing.Point(65, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(135, 20);
            this.panel1.TabIndex = 29;
            // 
            // severityImage
            // 
            this.severityImage.Location = new System.Drawing.Point(2, 2);
            this.severityImage.Name = "severityImage";
            this.severityImage.Size = new System.Drawing.Size(16, 16);
            this.severityImage.TabIndex = 4;
            this.severityImage.TabStop = false;
            // 
            // severity
            // 
            this.severity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.severity.Location = new System.Drawing.Point(22, 3);
            this.severity.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.severity.Name = "severity";
            this.severity.Size = new System.Drawing.Size(108, 17);
            this.severity.TabIndex = 3;
            this.severity.Text = "sev";
            // 
            // TaskDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(473, 312);
            this.Controls.Add(this.detailsTablePanel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "To Do";
            this.detailsTablePanel.ResumeLayout(false);
            this.detailsTablePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusDetailCombo)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.severityImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel detailsTablePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label subject;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox commentsDetails;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label instance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label metric;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label value;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label completed;
        private System.Windows.Forms.Label created;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor statusDetailCombo;
        private System.Windows.Forms.TextBox ownerDetails;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label severity;
        private System.Windows.Forms.PictureBox severityImage;
    }
}