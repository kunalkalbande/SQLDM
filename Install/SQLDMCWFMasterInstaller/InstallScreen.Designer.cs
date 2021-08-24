namespace Installer_form_application
{
    partial class InstallScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallScreen));
            this.labelHading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.checkBoxLaunchApp = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelHading
            // 
            this.labelHading.AutoSize = true;
            this.labelHading.BackColor = System.Drawing.Color.Transparent;
            this.labelHading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHading.Location = new System.Drawing.Point(187, 26);
            this.labelHading.Name = "labelHading";
            this.labelHading.Size = new System.Drawing.Size(192, 21);
            this.labelHading.TabIndex = 35;
            this.labelHading.Text = "Setup wizard completed ";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(188, 77);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(401, 202);
            this.labelDesc.TabIndex = 36;
            this.labelDesc.Text = "Congratulations!  SQL Diagnostic Manager and the IDERA Dashboard have been succes" +
    "sfully installed.";
            // 
            // buttonFinish
            // 
            this.buttonFinish.BackColor = System.Drawing.Color.LightGray;
            this.buttonFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonFinish.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonFinish.Location = new System.Drawing.Point(509, 420);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(72, 22);
            this.buttonFinish.TabIndex = 2;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = false;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // checkBoxLaunchApp
            // 
            this.checkBoxLaunchApp.AutoSize = true;
            this.checkBoxLaunchApp.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxLaunchApp.Checked = true;
            this.checkBoxLaunchApp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLaunchApp.Location = new System.Drawing.Point(191, 425);
            this.checkBoxLaunchApp.Name = "checkBoxLaunchApp";
            this.checkBoxLaunchApp.Size = new System.Drawing.Size(211, 17);
            this.checkBoxLaunchApp.TabIndex = 1;
            this.checkBoxLaunchApp.Text = "Launch Idera SQL Diagnostic Manager";
            this.checkBoxLaunchApp.UseVisualStyleBackColor = false;
            // 
            // InstallScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.checkBoxLaunchApp);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallScreen_FormClosing);
            this.Load += new System.EventHandler(this.InstallScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.CheckBox checkBoxLaunchApp;
    }
}