namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class JobDestinationDialog
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
            this.testButton = new System.Windows.Forms.Button();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.jobStepTextBox = new System.Windows.Forms.TextBox();
            this.jobNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.browseJobButton = new System.Windows.Forms.Button();
            this.browseJobStepButton = new System.Windows.Forms.Button();
            this.serverComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.serverComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testButton.Enabled = false;
            this.testButton.Location = new System.Drawing.Point(64, 157);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 5;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox1.Location = new System.Drawing.Point(12, 12);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(378, 56);
            this.informationBox1.TabIndex = 15;
            this.informationBox1.Text = "Enter the server, job name and optionally the job step to start.  You may use $(I" +
                "nstance) for the server to start the job on the server that caused the alert.";
            // 
            // jobStepTextBox
            // 
            this.jobStepTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jobStepTextBox.Enabled = false;
            this.jobStepTextBox.Location = new System.Drawing.Point(64, 121);
            this.jobStepTextBox.Name = "jobStepTextBox";
            this.jobStepTextBox.Size = new System.Drawing.Size(254, 20);
            this.jobStepTextBox.TabIndex = 3;
            // 
            // jobNameTextBox
            // 
            this.jobNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jobNameTextBox.Enabled = false;
            this.jobNameTextBox.Location = new System.Drawing.Point(64, 94);
            this.jobNameTextBox.Name = "jobNameTextBox";
            this.jobNameTextBox.Size = new System.Drawing.Size(254, 20);
            this.jobNameTextBox.TabIndex = 1;
            this.jobNameTextBox.TextChanged += new System.EventHandler(this.jobNameTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Job Step:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Job Name:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(243, 157);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(324, 157);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Server:";
            // 
            // browseJobButton
            // 
            this.browseJobButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseJobButton.Enabled = false;
            this.browseJobButton.Location = new System.Drawing.Point(324, 92);
            this.browseJobButton.Name = "browseJobButton";
            this.browseJobButton.Size = new System.Drawing.Size(75, 23);
            this.browseJobButton.TabIndex = 2;
            this.browseJobButton.Text = "Browse...";
            this.browseJobButton.UseVisualStyleBackColor = true;
            this.browseJobButton.Click += new System.EventHandler(this.browseJobButton_Click);
            // 
            // browseJobStepButton
            // 
            this.browseJobStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseJobStepButton.Enabled = false;
            this.browseJobStepButton.Location = new System.Drawing.Point(324, 119);
            this.browseJobStepButton.Name = "browseJobStepButton";
            this.browseJobStepButton.Size = new System.Drawing.Size(75, 23);
            this.browseJobStepButton.TabIndex = 4;
            this.browseJobStepButton.Text = "Browse...";
            this.browseJobStepButton.UseVisualStyleBackColor = true;
            this.browseJobStepButton.Click += new System.EventHandler(this.browseJobStepButton_Click);
            // 
            // serverComboBox
            // 
            this.serverComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverComboBox.DataSource = this.bindingSource;
            this.serverComboBox.DisplayMember = "InstanceName";
            this.serverComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.serverComboBox.Location = new System.Drawing.Point(64, 67);
            this.serverComboBox.Name = "serverComboBox";
            this.serverComboBox.NullText = "< Select an instance >";
            this.serverComboBox.Size = new System.Drawing.Size(254, 21);
            this.serverComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.serverComboBox.TabIndex = 0;
            this.serverComboBox.ValueMember = "InstanceName";
            this.serverComboBox.SelectionChangeCommitted += new System.EventHandler(this.serverComboBox_SelectionChangeCommitted);
            this.serverComboBox.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.serverComboBox_BeforeDropDown);
            // 
            // bindingSource
            // 
            this.bindingSource.AllowNew = false;
            this.bindingSource.DataSource = typeof(Idera.SQLdm.Common.Objects.MonitoredSqlServer);
            // 
            // JobDestinationDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(405, 187);
            this.Controls.Add(this.serverComboBox);
            this.Controls.Add(this.browseJobStepButton);
            this.Controls.Add(this.browseJobButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.informationBox1);
            this.Controls.Add(this.jobStepTextBox);
            this.Controls.Add(this.jobNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobDestinationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQL Agent Job Action";
            this.Load += new System.EventHandler(this.JobDestinationDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.serverComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button testButton;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.TextBox jobStepTextBox;
        private System.Windows.Forms.TextBox jobNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button browseJobButton;
        private System.Windows.Forms.Button browseJobStepButton;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor serverComboBox;
        private System.Windows.Forms.BindingSource bindingSource;
    }
}