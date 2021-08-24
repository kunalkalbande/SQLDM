namespace Installer_form_application
{
    partial class PortForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PortForm));
            this.textBoxCoreServicesPort = new System.Windows.Forms.TextBox();
            this.labelCoreServicePort = new System.Windows.Forms.Label();
            this.labelWebAppServicePort = new System.Windows.Forms.Label();
            this.textBoxWebAppServicePort = new System.Windows.Forms.TextBox();
            this.labelWebAppMonitorPort = new System.Windows.Forms.Label();
            this.textBoxWebAppMonitorPort = new System.Windows.Forms.TextBox();
            this.textBoxWebAppSSLPort = new System.Windows.Forms.TextBox();
            this.labelWebAppSSLPort = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // textBoxCoreServicesPort
            // 
            this.textBoxCoreServicesPort.Location = new System.Drawing.Point(514, 91);
            this.textBoxCoreServicesPort.Name = "textBoxCoreServicesPort";
            this.textBoxCoreServicesPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxCoreServicesPort.TabIndex = 1;
            this.textBoxCoreServicesPort.Text = "9292";
            this.textBoxCoreServicesPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCoreServicePort
            // 
            this.labelCoreServicePort.AutoSize = true;
            this.labelCoreServicePort.BackColor = System.Drawing.Color.Transparent;
            this.labelCoreServicePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelCoreServicePort.Location = new System.Drawing.Point(189, 94);
            this.labelCoreServicePort.Name = "labelCoreServicePort";
            this.labelCoreServicePort.Size = new System.Drawing.Size(186, 13);
            this.labelCoreServicePort.TabIndex = 1;
            this.labelCoreServicePort.Text = "IDERA Dashboard Core Services Port";
            // 
            // labelWebAppServicePort
            // 
            this.labelWebAppServicePort.AutoSize = true;
            this.labelWebAppServicePort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppServicePort.Location = new System.Drawing.Point(189, 135);
            this.labelWebAppServicePort.Name = "labelWebAppServicePort";
            this.labelWebAppServicePort.Size = new System.Drawing.Size(237, 13);
            this.labelWebAppServicePort.TabIndex = 2;
            this.labelWebAppServicePort.Text = "IDERA Dashboard Web Application Service Port";
            // 
            // textBoxWebAppServicePort
            // 
            this.textBoxWebAppServicePort.Location = new System.Drawing.Point(514, 132);
            this.textBoxWebAppServicePort.Name = "textBoxWebAppServicePort";
            this.textBoxWebAppServicePort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppServicePort.TabIndex = 2;
            this.textBoxWebAppServicePort.Text = "9290";
            this.textBoxWebAppServicePort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelWebAppMonitorPort
            // 
            this.labelWebAppMonitorPort.AutoSize = true;
            this.labelWebAppMonitorPort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppMonitorPort.Location = new System.Drawing.Point(189, 176);
            this.labelWebAppMonitorPort.Name = "labelWebAppMonitorPort";
            this.labelWebAppMonitorPort.Size = new System.Drawing.Size(236, 13);
            this.labelWebAppMonitorPort.TabIndex = 4;
            this.labelWebAppMonitorPort.Text = "IDERA Dashboard Web Application Monitor Port";
            // 
            // textBoxWebAppMonitorPort
            // 
            this.textBoxWebAppMonitorPort.Location = new System.Drawing.Point(514, 173);
            this.textBoxWebAppMonitorPort.Name = "textBoxWebAppMonitorPort";
            this.textBoxWebAppMonitorPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppMonitorPort.TabIndex = 3;
            this.textBoxWebAppMonitorPort.Text = "9094";
            this.textBoxWebAppMonitorPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxWebAppSSLPort
            // 
            this.textBoxWebAppSSLPort.Location = new System.Drawing.Point(514, 214);
            this.textBoxWebAppSSLPort.Name = "textBoxWebAppSSLPort";
            this.textBoxWebAppSSLPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppSSLPort.TabIndex = 4;
            this.textBoxWebAppSSLPort.Text = "9291";
            this.textBoxWebAppSSLPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelWebAppSSLPort
            // 
            this.labelWebAppSSLPort.AutoSize = true;
            this.labelWebAppSSLPort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppSSLPort.Location = new System.Drawing.Point(189, 217);
            this.labelWebAppSSLPort.Name = "labelWebAppSSLPort";
            this.labelWebAppSSLPort.Size = new System.Drawing.Size(221, 13);
            this.labelWebAppSSLPort.TabIndex = 6;
            this.labelWebAppSSLPort.Text = "IDERA Dashboard Web Application SSL Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(188, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Service Ports\r\n";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(350, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 6;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(506, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(428, 420);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 5;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(188, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "Specify the ports to be used by the IDERA Dashboard services.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(189, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(344, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "If Firewall is enabled make sure it allows TCP traffic through these ports.";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // PortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxWebAppSSLPort);
            this.Controls.Add(this.labelWebAppSSLPort);
            this.Controls.Add(this.textBoxWebAppMonitorPort);
            this.Controls.Add(this.labelWebAppMonitorPort);
            this.Controls.Add(this.textBoxWebAppServicePort);
            this.Controls.Add(this.labelWebAppServicePort);
            this.Controls.Add(this.labelCoreServicePort);
            this.Controls.Add(this.textBoxCoreServicesPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PortForm_FormClosing);
            this.Load += new System.EventHandler(this.PortForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCoreServicesPort;
        private System.Windows.Forms.Label labelCoreServicePort;
        private System.Windows.Forms.Label labelWebAppServicePort;
        private System.Windows.Forms.TextBox textBoxWebAppServicePort;
        private System.Windows.Forms.Label labelWebAppMonitorPort;
        private System.Windows.Forms.TextBox textBoxWebAppMonitorPort;
        private System.Windows.Forms.TextBox textBoxWebAppSSLPort;
        private System.Windows.Forms.Label labelWebAppSSLPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}