namespace Installer_form_application
{
    partial class Credentials
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Credentials));
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.radioButtonNotRegister = new System.Windows.Forms.RadioButton();
            this.radioButtonRemote = new System.Windows.Forms.RadioButton();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.backgroundWorkerRemote = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerFetchRemoteDetails = new System.ComponentModel.BackgroundWorker();
            this.username = new System.Windows.Forms.Label();
            this.url = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.Label();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.labelRemoteDashbaordVersion = new System.Windows.Forms.Label();
            this.labelForVersionVerdict = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(260, 32);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(503, 47);
            this.labelHeading.TabIndex = 30;
            this.labelHeading.Text = "Registration with IDERA Dashboard";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelDesc.Location = new System.Drawing.Point(261, 79);
            this.labelDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(505, 50);
            this.labelDesc.TabIndex = 31;
            this.labelDesc.Text = "Please register with an IDERA Dashboard installed on a remote server, or choose \"" +
    "Proceed without registration\".";
            // 
            // radioButtonNotRegister
            // 
            this.radioButtonNotRegister.AutoSize = true;
            this.radioButtonNotRegister.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonNotRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.radioButtonNotRegister.Location = new System.Drawing.Point(265, 161);
            this.radioButtonNotRegister.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonNotRegister.Name = "radioButtonNotRegister";
            this.radioButtonNotRegister.Size = new System.Drawing.Size(183, 19);
            this.radioButtonNotRegister.TabIndex = 2;
            this.radioButtonNotRegister.Text = "Proceed without registration.";
            this.radioButtonNotRegister.UseVisualStyleBackColor = false;
            this.radioButtonNotRegister.CheckedChanged += new System.EventHandler(this.radioButtonNotRegister_CheckedChanged);
            // 
            // radioButtonRemote
            // 
            this.radioButtonRemote.AutoSize = true;
            this.radioButtonRemote.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.radioButtonRemote.Location = new System.Drawing.Point(265, 133);
            this.radioButtonRemote.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonRemote.Name = "radioButtonRemote";
            this.radioButtonRemote.Size = new System.Drawing.Size(310, 19);
            this.radioButtonRemote.TabIndex = 1;
            this.radioButtonRemote.Text = "Register with IDERA Dashboard on a remote server.";
            this.radioButtonRemote.UseVisualStyleBackColor = false;
            this.radioButtonRemote.CheckedChanged += new System.EventHandler(this.radioButtonRemote_CheckedChanged);
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(467, 517);
            this.buttonBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(96, 27);
            this.buttonBack.TabIndex = 7;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(675, 517);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 27);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(571, 517);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(96, 27);
            this.buttonNext.TabIndex = 6;
            this.buttonNext.Text = "Next";
            this.buttonNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // backgroundWorkerRemote
            // 
            this.backgroundWorkerRemote.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRemote_DoWork);
            this.backgroundWorkerRemote.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // backgroundWorkerFetchRemoteDetails
            // 
            this.backgroundWorkerFetchRemoteDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerFetchRemoteDetails_DoWork);
            this.backgroundWorkerFetchRemoteDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerFetchRemoteDetails_RunWorkerCompleted);
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.BackColor = System.Drawing.Color.Transparent;
            this.username.Location = new System.Drawing.Point(265, 238);
            this.username.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(204, 17);
            this.username.TabIndex = 32;
            this.username.Text = "Remote Dashboard Username:";
            // 
            // url
            // 
            this.url.AutoSize = true;
            this.url.BackColor = System.Drawing.Color.Transparent;
            this.url.Location = new System.Drawing.Point(265, 206);
            this.url.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(167, 17);
            this.url.TabIndex = 33;
            this.url.Text = "Remote Dashboard URL:";
            // 
            // password
            // 
            this.password.AutoSize = true;
            this.password.BackColor = System.Drawing.Color.Transparent;
            this.password.Location = new System.Drawing.Point(265, 268);
            this.password.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(200, 17);
            this.password.TabIndex = 34;
            this.password.Text = "Remote Dashboard Password:";
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Location = new System.Drawing.Point(467, 203);
            this.textBoxUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(303, 22);
            this.textBoxUrl.TabIndex = 35;
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(467, 235);
            this.textBoxUsername.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(303, 22);
            this.textBoxUsername.TabIndex = 36;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(467, 267);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(303, 22);
            this.textBoxPassword.TabIndex = 37;
            // 
            // buttonTest
            // 
            this.buttonTest.BackColor = System.Drawing.Color.LightGray;
            this.buttonTest.Location = new System.Drawing.Point(571, 299);
            this.buttonTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(200, 28);
            this.buttonTest.TabIndex = 38;
            this.buttonTest.Text = "Test Version Compatibility";
            this.buttonTest.UseVisualStyleBackColor = false;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // labelRemoteDashbaordVersion
            // 
            this.labelRemoteDashbaordVersion.AutoSize = true;
            this.labelRemoteDashbaordVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashbaordVersion.Location = new System.Drawing.Point(265, 348);
            this.labelRemoteDashbaordVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelRemoteDashbaordVersion.Name = "labelRemoteDashbaordVersion";
            this.labelRemoteDashbaordVersion.Size = new System.Drawing.Size(191, 17);
            this.labelRemoteDashbaordVersion.TabIndex = 39;
            this.labelRemoteDashbaordVersion.Text = "Remote Dashboard Version: ";
            this.labelRemoteDashbaordVersion.Visible = false;
            // 
            // labelForVersionVerdict
            // 
            this.labelForVersionVerdict.AutoSize = true;
            this.labelForVersionVerdict.BackColor = System.Drawing.Color.Transparent;
            this.labelForVersionVerdict.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelForVersionVerdict.ForeColor = System.Drawing.Color.Red;
            this.labelForVersionVerdict.Location = new System.Drawing.Point(265, 368);
            this.labelForVersionVerdict.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelForVersionVerdict.Name = "labelForVersionVerdict";
            this.labelForVersionVerdict.Size = new System.Drawing.Size(455, 17);
            this.labelForVersionVerdict.TabIndex = 40;
            this.labelForVersionVerdict.Text = "Compatible version found on remote machine, installation can proceed.";
            this.labelForVersionVerdict.Visible = false;
            // 
            // Credentials
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(791, 559);
            this.Controls.Add(this.labelForVersionVerdict);
            this.Controls.Add(this.labelRemoteDashbaordVersion);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.password);
            this.Controls.Add(this.url);
            this.Controls.Add(this.username);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.radioButtonRemote);
            this.Controls.Add(this.radioButtonNotRegister);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Credentials";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Credentials_FormClosing);
            this.Load += new System.EventHandler(this.Credentials_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNotRegister;
        private System.Windows.Forms.RadioButton radioButtonRemote;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRemote;
        private System.ComponentModel.BackgroundWorker backgroundWorkerFetchRemoteDetails;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.Label url;
        private System.Windows.Forms.Label password;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Label labelRemoteDashbaordVersion;
        private System.Windows.Forms.Label labelForVersionVerdict;
    }
}