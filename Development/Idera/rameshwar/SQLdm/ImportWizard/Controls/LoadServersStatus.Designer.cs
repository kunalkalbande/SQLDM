namespace Idera.SQLdm.ImportWizard.Controls
{
    partial class LoadServersStatus
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
            this._spinner = new MRG.Controls.UI.LoadingCircle();
            this._lnklbl_Status = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // _spinner
            // 
            this._spinner.Active = false;
            this._spinner.Color = System.Drawing.Color.DarkGray;
            this._spinner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._spinner.InnerCircleRadius = 8;
            this._spinner.Location = new System.Drawing.Point(0, 0);
            this._spinner.Name = "_spinner";
            this._spinner.NumberSpoke = 10;
            this._spinner.OuterCircleRadius = 10;
            this._spinner.RotationSpeed = 100;
            this._spinner.Size = new System.Drawing.Size(150, 150);
            this._spinner.SpokeThickness = 4;
            this._spinner.TabIndex = 0;
            // 
            // _lnklbl_Status
            // 
            this._lnklbl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lnklbl_Status.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this._lnklbl_Status.Location = new System.Drawing.Point(0, 0);
            this._lnklbl_Status.Name = "_lnklbl_Status";
            this._lnklbl_Status.Size = new System.Drawing.Size(150, 150);
            this._lnklbl_Status.TabIndex = 1;
            this._lnklbl_Status.Text = "<Link text>";
            this._lnklbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lnklbl_Status.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnklbl_Status_LinkClicked);
            // 
            // LoadServersStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lnklbl_Status);
            this.Controls.Add(this._spinner);
            this.Name = "LoadServersStatus";
            this.ResumeLayout(false);

        }

        #endregion

        private MRG.Controls.UI.LoadingCircle _spinner;
        private System.Windows.Forms.LinkLabel _lnklbl_Status;
    }
}
