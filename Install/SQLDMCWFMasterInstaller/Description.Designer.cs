using System.Windows.Forms;
namespace Installer_form_application
{
    partial class Description
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Description));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelPathDesc = new System.Windows.Forms.Label();
            this.textBoxSPPath = new System.Windows.Forms.TextBox();
            this.textBoxIDPath = new System.Windows.Forms.TextBox();
            this.labelPathSP = new System.Windows.Forms.Label();
            this.labelPathID = new System.Windows.Forms.Label();
            this.labelInstanceDesc = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelInstanceExpl = new System.Windows.Forms.Label();
            this.textBoxDisplayName = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.checkAvailabilityButton = new System.Windows.Forms.Button();
            this.availabilityLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonAllUsers = new System.Windows.Forms.RadioButton();
            this.radioButtonCurrentUser = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(350, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 5;
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
            this.buttonCancel.TabIndex = 6;
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
            this.buttonNext.TabIndex = 4;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(185, 27);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(212, 21);
            this.labelHeading.TabIndex = 46;
            this.labelHeading.Text = "Choose a destination folder";
            // 
            // labelPathDesc
            // 
            this.labelPathDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelPathDesc.Location = new System.Drawing.Point(186, 78);
            this.labelPathDesc.Name = "labelPathDesc";
            this.labelPathDesc.Size = new System.Drawing.Size(406, 23);
            this.labelPathDesc.TabIndex = 49;
            this.labelPathDesc.Text = "Please provide us the destination folder where you want to install the component(" +
    "s):";
            // 
            // textBoxSPPath
            // 
            this.textBoxSPPath.Location = new System.Drawing.Point(290, 142);
            this.textBoxSPPath.Name = "textBoxSPPath";
            this.textBoxSPPath.Size = new System.Drawing.Size(246, 20);
            this.textBoxSPPath.TabIndex = 1;
            this.textBoxSPPath.Text = "C:\\Program Files\\Idera\\SQL Diagnostic Manager\\";
            this.textBoxSPPath.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxIDPath
            // 
            this.textBoxIDPath.Location = new System.Drawing.Point(290, 112);
            this.textBoxIDPath.Name = "textBoxIDPath";
            this.textBoxIDPath.Size = new System.Drawing.Size(246, 20);
            this.textBoxIDPath.TabIndex = 2;
            this.textBoxIDPath.Text = "C:\\Program Files\\Idera\\Dashboard\\";
            this.textBoxIDPath.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelPathSP
            // 
            this.labelPathSP.AutoSize = true;
            this.labelPathSP.BackColor = System.Drawing.Color.Transparent;
            this.labelPathSP.Location = new System.Drawing.Point(187, 144);
            this.labelPathSP.Name = "labelPathSP";
            this.labelPathSP.Size = new System.Drawing.Size(51, 13);
            this.labelPathSP.TabIndex = 52;
            this.labelPathSP.Text = "SQL DM:";
            // 
            // labelPathID
            // 
            this.labelPathID.AutoSize = true;
            this.labelPathID.BackColor = System.Drawing.Color.Transparent;
            this.labelPathID.Location = new System.Drawing.Point(186, 112);
            this.labelPathID.Name = "labelPathID";
            this.labelPathID.Size = new System.Drawing.Size(98, 13);
            this.labelPathID.TabIndex = 53;
            this.labelPathID.Text = "IDERA Dashboard:";
            // 
            // labelInstanceDesc
            // 
            this.labelInstanceDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelInstanceDesc.Location = new System.Drawing.Point(187, 276);
            this.labelInstanceDesc.Name = "labelInstanceDesc";
            this.labelInstanceDesc.Size = new System.Drawing.Size(394, 22);
            this.labelInstanceDesc.TabIndex = 55;
            this.labelInstanceDesc.Text = "Provide a unique name for the SQL DM instance";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.BackColor = System.Drawing.Color.Transparent;
            this.labelDisplayName.Location = new System.Drawing.Point(186, 353);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 56;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelInstanceExpl
            // 
            this.labelInstanceExpl.BackColor = System.Drawing.Color.Transparent;
            this.labelInstanceExpl.Location = new System.Drawing.Point(187, 298);
            this.labelInstanceExpl.Name = "labelInstanceExpl";
            this.labelInstanceExpl.Size = new System.Drawing.Size(367, 46);
            this.labelInstanceExpl.TabIndex = 57;
            this.labelInstanceExpl.Text = resources.GetString("labelInstanceExpl.Text");
            // 
            // textBoxDisplayName
            // 
            this.textBoxDisplayName.Location = new System.Drawing.Point(291, 350);
            this.textBoxDisplayName.Name = "textBoxDisplayName";
            this.textBoxDisplayName.Size = new System.Drawing.Size(246, 20);
            this.textBoxDisplayName.TabIndex = 3;
            this.textBoxDisplayName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // checkAvailabilityButton
            // 
            this.checkAvailabilityButton.BackColor = System.Drawing.Color.LightGray;
            this.checkAvailabilityButton.Location = new System.Drawing.Point(428, 377);
            this.checkAvailabilityButton.Name = "checkAvailabilityButton";
            this.checkAvailabilityButton.Size = new System.Drawing.Size(107, 23);
            this.checkAvailabilityButton.TabIndex = 58;
            this.checkAvailabilityButton.Text = "Check Availability";
            this.checkAvailabilityButton.UseVisualStyleBackColor = false;
            this.checkAvailabilityButton.Click += new System.EventHandler(this.checkAvailabilityButton_Click);
            // 
            // availabilityLabel
            // 
            this.availabilityLabel.AutoSize = true;
            this.availabilityLabel.ForeColor = System.Drawing.Color.ForestGreen;
            this.availabilityLabel.Location = new System.Drawing.Point(187, 382);
            this.availabilityLabel.Name = "availabilityLabel";
            this.availabilityLabel.Size = new System.Drawing.Size(154, 13);
            this.availabilityLabel.TabIndex = 59;
            this.availabilityLabel.Text = "Note: Display name is available";
            this.availabilityLabel.Visible = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(187, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 22);
            this.label1.TabIndex = 60;
            this.label1.Text = "Install this application for:";
            // 
            // radioButtonAllUsers
            // 
            this.radioButtonAllUsers.AutoSize = true;
            this.radioButtonAllUsers.Checked = true;
            this.radioButtonAllUsers.Location = new System.Drawing.Point(190, 211);
            this.radioButtonAllUsers.Name = "radioButtonAllUsers";
            this.radioButtonAllUsers.Size = new System.Drawing.Size(222, 17);
            this.radioButtonAllUsers.TabIndex = 61;
            this.radioButtonAllUsers.TabStop = true;
            this.radioButtonAllUsers.Text = "Anyone who uses this computer (all users)";
            this.radioButtonAllUsers.UseVisualStyleBackColor = true;
            // 
            // radioButtonCurrentUser
            // 
            this.radioButtonCurrentUser.AutoSize = true;
            this.radioButtonCurrentUser.Location = new System.Drawing.Point(190, 234);
            this.radioButtonCurrentUser.Name = "radioButtonCurrentUser";
            this.radioButtonCurrentUser.Size = new System.Drawing.Size(146, 17);
            this.radioButtonCurrentUser.TabIndex = 62;
            this.radioButtonCurrentUser.Text = "Only for me (Current User)";
            this.radioButtonCurrentUser.UseVisualStyleBackColor = true;
            // 
            // Description
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.radioButtonCurrentUser);
            this.Controls.Add(this.radioButtonAllUsers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.availabilityLabel);
            this.Controls.Add(this.checkAvailabilityButton);
            this.Controls.Add(this.textBoxDisplayName);
            this.Controls.Add(this.labelInstanceExpl);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.labelInstanceDesc);
            this.Controls.Add(this.labelPathID);
            this.Controls.Add(this.labelPathSP);
            this.Controls.Add(this.textBoxIDPath);
            this.Controls.Add(this.textBoxSPPath);
            this.Controls.Add(this.labelPathDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Description";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Description_FormClosing);
            this.Load += new System.EventHandler(this.Description_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelPathDesc;
        private System.Windows.Forms.TextBox textBoxSPPath;
        private System.Windows.Forms.TextBox textBoxIDPath;
        private System.Windows.Forms.Label labelPathSP;
        private System.Windows.Forms.Label labelPathID;
        private System.Windows.Forms.Label labelInstanceDesc;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelInstanceExpl;
        private System.Windows.Forms.TextBox textBoxDisplayName;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button checkAvailabilityButton;
        private Label availabilityLabel;
        private Label label1;
        private RadioButton radioButtonAllUsers;
        private RadioButton radioButtonCurrentUser;
    }
}