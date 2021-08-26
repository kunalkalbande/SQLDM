namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    partial class ServiceBrowserDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceBrowserDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label_AvailInstancesOn = new System.Windows.Forms.Label();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.serviceListBox = new System.Windows.Forms.ListBox();
            this.serviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.wmiProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.stackLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serviceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(197, 181);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(116, 181);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // label_AvailInstancesOn
            // 
            this.label_AvailInstancesOn.AutoSize = true;
            this.label_AvailInstancesOn.Location = new System.Drawing.Point(9, 9);
            this.label_AvailInstancesOn.Name = "label_AvailInstancesOn";
            this.label_AvailInstancesOn.Size = new System.Drawing.Size(133, 13);
            this.label_AvailInstancesOn.TabIndex = 0;
            this.label_AvailInstancesOn.Text = "&Available instances on {0}:";
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = this.serviceListBox;
            this.stackLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stackLayoutPanel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.stackLayoutPanel1.Controls.Add(this.serviceListBox);
            this.stackLayoutPanel1.Controls.Add(this.wmiProgressControl);
            this.stackLayoutPanel1.Location = new System.Drawing.Point(12, 25);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.stackLayoutPanel1.Size = new System.Drawing.Size(260, 150);
            this.stackLayoutPanel1.TabIndex = 0;
            // 
            // serviceListBox
            // 
            this.serviceListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.serviceListBox.DataSource = this.serviceBindingSource;
            this.serviceListBox.DisplayMember = "DisplayName";
            this.serviceListBox.FormattingEnabled = true;
            this.serviceListBox.IntegralHeight = false;
            this.serviceListBox.Location = new System.Drawing.Point(1, 1);
            this.serviceListBox.Name = "serviceListBox";
            this.serviceListBox.Size = new System.Drawing.Size(258, 148);
            this.serviceListBox.TabIndex = 0;
            this.serviceListBox.DoubleClick += new System.EventHandler(this.serviceListBox_DoubleClick);
            this.serviceListBox.SelectedValueChanged += new System.EventHandler(this.serviceListBox_SelectedValueChanged);
            // 
            // serviceBindingSource
            // 
            this.serviceBindingSource.DataSource = typeof(Idera.SQLdm.Common.UI.Dialogs.Config.ServiceInstance);
            // 
            // wmiProgressControl
            // 
            this.wmiProgressControl.Active = false;
            this.wmiProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiProgressControl.BackColor = System.Drawing.Color.White;
            this.wmiProgressControl.Color = System.Drawing.Color.DarkGray;
            this.wmiProgressControl.InnerCircleRadius = 8;
            this.wmiProgressControl.Location = new System.Drawing.Point(1, 1);
            this.wmiProgressControl.Name = "wmiProgressControl";
            this.wmiProgressControl.NumberSpoke = 10;
            this.wmiProgressControl.OuterCircleRadius = 12;
            this.wmiProgressControl.RotationSpeed = 80;
            this.wmiProgressControl.Size = new System.Drawing.Size(258, 148);
            this.wmiProgressControl.SpokeThickness = 4;
            this.wmiProgressControl.TabIndex = 4;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // ServiceBrowserDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(284, 216);
            this.Controls.Add(this.label_AvailInstancesOn);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.stackLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServiceBrowserDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select {0} Service";
            this.stackLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.serviceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private MRG.Controls.UI.LoadingCircle wmiProgressControl;
        private System.Windows.Forms.Label label_AvailInstancesOn;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ListBox serviceListBox;
        private System.Windows.Forms.BindingSource serviceBindingSource;
    }
}