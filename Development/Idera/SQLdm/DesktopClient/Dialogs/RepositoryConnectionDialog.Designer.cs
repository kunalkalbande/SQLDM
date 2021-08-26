namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class RepositoryConnectionDialog
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
            this.connectButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.dividerLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.serverLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databaseLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.authenticationLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.userNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.passwordLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.rememberPasswordCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.serversDropDownList = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.databaseDropDownList = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.authenticationDropDownList = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.userNameTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.passwordTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.headerImage = new System.Windows.Forms.PictureBox();
            this.browseServersWorker = new System.ComponentModel.BackgroundWorker();
            this.browseDatabasesWorker = new System.ComponentModel.BackgroundWorker();
            this.connectionProgressBar = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(239, 246);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(320, 246);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // dividerLabel
            // 
            this.dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLabel.Location = new System.Drawing.Point(12, 235);
            this.dividerLabel.Name = "dividerLabel";
            this.dividerLabel.Size = new System.Drawing.Size(383, 2);
            this.dividerLabel.TabIndex = 4;
            // 
            // serverLabel
            // 
            this.serverLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(3, 7);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 9;
            this.serverLabel.Text = "Server:";
            // 
            // databaseLabel
            // 
            this.databaseLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.Location = new System.Drawing.Point(3, 34);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(56, 13);
            this.databaseLabel.TabIndex = 5;
            this.databaseLabel.Text = "Database:";
            // 
            // authenticationLabel
            // 
            this.authenticationLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(3, 61);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(78, 13);
            this.authenticationLabel.TabIndex = 6;
            this.authenticationLabel.Text = "Authentication:";
            // 
            // userNameLabel
            // 
            this.userNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(20, 87);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(61, 13);
            this.userNameLabel.TabIndex = 7;
            this.userNameLabel.Text = "User name:";
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(20, 113);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordLabel.TabIndex = 8;
            this.passwordLabel.Text = "Password:";
            // 
            // rememberPasswordCheckbox
            // 
            this.rememberPasswordCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rememberPasswordCheckbox.AutoSize = true;
            this.rememberPasswordCheckbox.Location = new System.Drawing.Point(113, 136);
            this.rememberPasswordCheckbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.rememberPasswordCheckbox.Name = "rememberPasswordCheckbox";
            this.rememberPasswordCheckbox.Size = new System.Drawing.Size(126, 16);
            this.rememberPasswordCheckbox.TabIndex = 5;
            this.rememberPasswordCheckbox.Text = "Remember Password";
            this.rememberPasswordCheckbox.UseVisualStyleBackColor = true;
            // 
            // serversDropDownList
            // 
            this.serversDropDownList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serversDropDownList.FormattingEnabled = true;
            this.serversDropDownList.Location = new System.Drawing.Point(96, 3);
            this.serversDropDownList.Name = "serversDropDownList";
            this.serversDropDownList.Size = new System.Drawing.Size(283, 21);
            this.serversDropDownList.TabIndex = 0;
            this.serversDropDownList.SelectedIndexChanged += new System.EventHandler(this.serversDropDownList_SelectedIndexChanged);
            this.serversDropDownList.TextUpdate += new System.EventHandler(this.serversDropDownList_TextUpdate);
            // 
            // databaseDropDownList
            // 
            this.databaseDropDownList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.databaseDropDownList.FormattingEnabled = true;
            this.databaseDropDownList.Location = new System.Drawing.Point(96, 30);
            this.databaseDropDownList.Name = "databaseDropDownList";
            this.databaseDropDownList.Size = new System.Drawing.Size(283, 21);
            this.databaseDropDownList.TabIndex = 1;
            this.databaseDropDownList.SelectedIndexChanged += new System.EventHandler(this.databaseDropDownList_SelectedIndexChanged);
            this.databaseDropDownList.TextUpdate += new System.EventHandler(this.databaseDropDownList_TextUpdate);
            // 
            // authenticationDropDownList
            // 
            this.authenticationDropDownList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.authenticationDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationDropDownList.FormattingEnabled = true;
            this.authenticationDropDownList.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.authenticationDropDownList.Location = new System.Drawing.Point(96, 57);
            this.authenticationDropDownList.Name = "authenticationDropDownList";
            this.authenticationDropDownList.Size = new System.Drawing.Size(283, 21);
            this.authenticationDropDownList.TabIndex = 2;
            this.authenticationDropDownList.SelectedIndexChanged += new System.EventHandler(this.authenticationDropDownList_SelectedIndexChanged);
            // 
            // userNameTextbox
            // 
            this.userNameTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.userNameTextbox.Location = new System.Drawing.Point(113, 84);
            this.userNameTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.userNameTextbox.Name = "userNameTextbox";
            this.userNameTextbox.Size = new System.Drawing.Size(266, 20);
            this.userNameTextbox.TabIndex = 3;
            // 
            // passwordTextbox
            // 
            this.passwordTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.passwordTextbox.Location = new System.Drawing.Point(113, 110);
            this.passwordTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.passwordTextbox.Name = "passwordTextbox";
            this.passwordTextbox.Size = new System.Drawing.Size(266, 20);
            this.passwordTextbox.TabIndex = 4;
            this.passwordTextbox.UseSystemPasswordChar = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.54308F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.45692F));
            this.tableLayoutPanel1.Controls.Add(this.serversDropDownList, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rememberPasswordCheckbox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.passwordTextbox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.databaseDropDownList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.userNameLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.userNameTextbox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.authenticationLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.authenticationDropDownList, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.databaseLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.serverLabel, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 73);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(383, 155);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // headerImage
            // 
            this.headerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RepositoryConnectionDialogHeader;
            this.headerImage.InitialImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.RepositoryConnectionDialogHeader;
            this.headerImage.Location = new System.Drawing.Point(0, 0);
            this.headerImage.Name = "headerImage";
            this.headerImage.Size = new System.Drawing.Size(407, 65);
            this.headerImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.headerImage.TabIndex = 16;
            this.headerImage.TabStop = false;
            // 
            // browseServersWorker
            // 
            this.browseServersWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.browseServersWorker_RunWorkerCompleted);
            // 
            // browseDatabasesWorker
            // 
            this.browseDatabasesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.browseDatabasesWorker_RunWorkerCompleted);
            // 
            // connectionProgressBar
            // 
            this.connectionProgressBar.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(201)))), ((int)(((byte)(67)))));
            this.connectionProgressBar.Color2 = System.Drawing.Color.White;
            this.connectionProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionProgressBar.Location = new System.Drawing.Point(0, 65);
            this.connectionProgressBar.Name = "connectionProgressBar";
            this.connectionProgressBar.Size = new System.Drawing.Size(407, 3);
            this.connectionProgressBar.Speed = 15;
            this.connectionProgressBar.Step = 5F;
            this.connectionProgressBar.TabIndex = 3;
            // 
            // RepositoryConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 281);
            this.Controls.Add(this.connectionProgressBar);
            this.Controls.Add(this.headerImage);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.dividerLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.connectButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepositoryConnectionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to SQLDM Repository";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RepositoryConnectionDialog_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton connectButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dividerLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serverLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databaseLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel authenticationLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel userNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel passwordLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox rememberPasswordCheckbox;


        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox serversDropDownList;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox databaseDropDownList;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox authenticationDropDownList;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox userNameTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox passwordTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox headerImage;
        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar connectionProgressBar;
        private System.ComponentModel.BackgroundWorker browseServersWorker;
        private System.ComponentModel.BackgroundWorker browseDatabasesWorker;
    }
}