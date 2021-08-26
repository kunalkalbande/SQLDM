namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class PulsePlatformConnectionConfigurationControl
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
            this._pulseServerTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this._testConnectionButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._testConnectionProgressControl = new MRG.Controls.UI.LoadingCircle();
            this._testConnectionWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // _pulseServerTextBox
            // 
            this._pulseServerTextBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._pulseServerTextBox.Location = new System.Drawing.Point(25, 15);
            this._pulseServerTextBox.Name = "_pulseServerTextBox";
            this._pulseServerTextBox.Size = new System.Drawing.Size(200, 22);
            this._pulseServerTextBox.TabIndex = 0;
            // 
            // _testConnectionButton
            // 
            this._testConnectionButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._testConnectionButton.Location = new System.Drawing.Point(25, 43);
            this._testConnectionButton.Name = "_testConnectionButton";
            this._testConnectionButton.Size = new System.Drawing.Size(113, 27);
            this._testConnectionButton.TabIndex = 1;
            this._testConnectionButton.Text = "Test Connection";
            this._testConnectionButton.UseVisualStyleBackColor = true;
            this._testConnectionButton.Click += new System.EventHandler(this._testConnectionButton_Click);
            // 
            // _testConnectionProgressControl
            // 
            this._testConnectionProgressControl.Active = false;
            this._testConnectionProgressControl.Color = System.Drawing.Color.Gray;
            this._testConnectionProgressControl.InnerCircleRadius = 5;
            this._testConnectionProgressControl.Location = new System.Drawing.Point(140, 42);
            this._testConnectionProgressControl.Name = "_testConnectionProgressControl";
            this._testConnectionProgressControl.NumberSpoke = 12;
            this._testConnectionProgressControl.OuterCircleRadius = 11;
            this._testConnectionProgressControl.RotationSpeed = 70;
            this._testConnectionProgressControl.Size = new System.Drawing.Size(39, 31);
            this._testConnectionProgressControl.SpokeThickness = 2;
            this._testConnectionProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this._testConnectionProgressControl.TabIndex = 15;
            this._testConnectionProgressControl.Visible = false;
            // 
            // _testConnectionWorker
            // 
            this._testConnectionWorker.WorkerSupportsCancellation = true;
            this._testConnectionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._testConnectionWorker_DoWork);
            this._testConnectionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._testConnectionWorker_RunWorkerCompleted);
            // 
            // PulsePlatformConnectionConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._testConnectionProgressControl);
            this.Controls.Add(this._testConnectionButton);
            this.Controls.Add(this._pulseServerTextBox);
            this.Name = "PulsePlatformConnectionConfigurationControl";
            this.Size = new System.Drawing.Size(547, 84);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox _pulseServerTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _testConnectionButton;
        private MRG.Controls.UI.LoadingCircle _testConnectionProgressControl;
        private System.ComponentModel.BackgroundWorker _testConnectionWorker;

    }
}
