namespace Installer_form_application
{
    partial class RemoteProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteProgress));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelProgress = new System.Windows.Forms.Label();
            this.backgroundWorkerRemote = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerLocal = new System.ComponentModel.BackgroundWorker();
            this.labelForPerc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Gainsboro;
            this.progressBar.Location = new System.Drawing.Point(191, 178);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(392, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(187, 28);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(188, 21);
            this.labelHeading.TabIndex = 47;
            this.labelHeading.Text = "Installation in progress...";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Enabled = false;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(350, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 50;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(506, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 49;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            this.buttonNext.Enabled = false;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(428, 420);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 48;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = false;
            // 
            // labelProgress
            // 
            this.labelProgress.BackColor = System.Drawing.Color.Transparent;
            this.labelProgress.Location = new System.Drawing.Point(188, 162);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(310, 13);
            this.labelProgress.TabIndex = 52;
            // 
            // backgroundWorkerRemote
            // 
            this.backgroundWorkerRemote.WorkerReportsProgress = true;
            this.backgroundWorkerRemote.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRemote_DoWork);
            this.backgroundWorkerRemote.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerRemote_ProgressChanged);
            this.backgroundWorkerRemote.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // backgroundWorkerLocal
            // 
            this.backgroundWorkerLocal.WorkerReportsProgress = true;
            this.backgroundWorkerLocal.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLocal_DoWork);
            this.backgroundWorkerLocal.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerLocal_ProgressChanged);
            this.backgroundWorkerLocal.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // labelForPerc
            // 
            this.labelForPerc.AutoSize = true;
            this.labelForPerc.BackColor = System.Drawing.Color.Transparent;
            this.labelForPerc.Location = new System.Drawing.Point(550, 162);
            this.labelForPerc.Name = "labelForPerc";
            this.labelForPerc.Size = new System.Drawing.Size(0, 13);
            this.labelForPerc.TabIndex = 53;
            // 
            // RemoteProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.labelForPerc);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CancelClosingEvent);
            this.Shown += new System.EventHandler(this.RemoteProgress_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelProgress;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRemote;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLocal;
        private System.Windows.Forms.Label labelForPerc;
    }
}