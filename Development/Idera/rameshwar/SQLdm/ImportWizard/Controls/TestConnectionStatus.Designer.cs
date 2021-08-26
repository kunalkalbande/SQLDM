namespace Idera.SQLdm.ImportWizard.Controls
{
    partial class TestConnectionStatus
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
            this._pnl_Image = new System.Windows.Forms.Panel();
            this._statusImage = new System.Windows.Forms.PictureBox();
            this._spinner = new MRG.Controls.UI.LoadingCircle();
            this._lnklbl_Status = new System.Windows.Forms.LinkLabel();
            this._pnl_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._statusImage)).BeginInit();
            this.SuspendLayout();
            // 
            // _pnl_Image
            // 
            this._pnl_Image.Controls.Add(this._statusImage);
            this._pnl_Image.Controls.Add(this._spinner);
            this._pnl_Image.Dock = System.Windows.Forms.DockStyle.Left;
            this._pnl_Image.Location = new System.Drawing.Point(0, 0);
            this._pnl_Image.Name = "_pnl_Image";
            this._pnl_Image.Size = new System.Drawing.Size(24, 48);
            this._pnl_Image.TabIndex = 0;
            // 
            // _statusImage
            // 
            this._statusImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statusImage.Location = new System.Drawing.Point(0, 0);
            this._statusImage.Name = "_statusImage";
            this._statusImage.Size = new System.Drawing.Size(24, 48);
            this._statusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._statusImage.TabIndex = 3;
            this._statusImage.TabStop = false;
            // 
            // _spinner
            // 
            this._spinner.Active = false;
            this._spinner.Color = System.Drawing.Color.DarkGray;
            this._spinner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._spinner.InnerCircleRadius = 5;
            this._spinner.Location = new System.Drawing.Point(0, 0);
            this._spinner.Name = "_spinner";
            this._spinner.NumberSpoke = 10;
            this._spinner.OuterCircleRadius = 7;
            this._spinner.RotationSpeed = 100;
            this._spinner.Size = new System.Drawing.Size(24, 48);
            this._spinner.SpokeThickness = 2;
            this._spinner.TabIndex = 2;
            // 
            // _lnklbl_Status
            // 
            this._lnklbl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lnklbl_Status.Location = new System.Drawing.Point(24, 0);
            this._lnklbl_Status.Name = "_lnklbl_Status";
            this._lnklbl_Status.Size = new System.Drawing.Size(367, 48);
            this._lnklbl_Status.TabIndex = 1;
            this._lnklbl_Status.TabStop = true;
            this._lnklbl_Status.Text = "linkLabel1";
            this._lnklbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._lnklbl_Status.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnklbl_Status_LinkClicked);
            // 
            // TestConnectionStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lnklbl_Status);
            this.Controls.Add(this._pnl_Image);
            this.Name = "TestConnectionStatus";
            this.Size = new System.Drawing.Size(391, 48);
            this._pnl_Image.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._statusImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pnl_Image;
        private System.Windows.Forms.LinkLabel _lnklbl_Status;
        private System.Windows.Forms.PictureBox _statusImage;
        private MRG.Controls.UI.LoadingCircle _spinner;
    }
}
