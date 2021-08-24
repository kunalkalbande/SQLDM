namespace Installer_form_application
{
    partial class InstallationFailure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationFailure));
            this.labelHeadDescription = new System.Windows.Forms.Label();
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelHeadDescription
            // 
            this.labelHeadDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelHeadDescription.Location = new System.Drawing.Point(190, 66);
            this.labelHeadDescription.Name = "labelHeadDescription";
            this.labelHeadDescription.Size = new System.Drawing.Size(371, 53);
            this.labelHeadDescription.TabIndex = 43;
            this.labelHeadDescription.Text = "IDERA SQL Diagnostic Manager Wizard ended prematurely because of an error. Your s" +
    "ystem has not been modified. To install this program at a later time, run Setup " +
    "again.";
            // 
            // labelHeading
            // 
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelHeading.Location = new System.Drawing.Point(189, 26);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(191, 23);
            this.labelHeading.TabIndex = 42;
            this.labelHeading.Text = "Installation Failed";
            // 
            // buttonFinish
            // 
            this.buttonFinish.BackColor = System.Drawing.Color.LightGray;
            this.buttonFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFinish.Location = new System.Drawing.Point(509, 420);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(72, 22);
            this.buttonFinish.TabIndex = 1;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = false;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(190, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(371, 51);
            this.label1.TabIndex = 44;
            // 
            // InstallationFailure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelHeadDescription);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonFinish);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallationFailure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallFailure_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelHeadDescription;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Label label1;
    }
}