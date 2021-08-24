using System;

namespace Installer_form_application
{
    partial class EULA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EULA));
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.richTextBoxEULA = new System.Windows.Forms.RichTextBox();
            this.checkBoxAccept = new System.Windows.Forms.CheckBox();
            this.labelDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(196, 22);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(221, 21);
            this.labelHeading.TabIndex = 13;
            this.labelHeading.Text = "End-User License Agreement";
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(353, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 3;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(509, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            // 
            // buttonNext
            // 
            this.buttonNext.Enabled = false;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(431, 420);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            // 
            // richTextBoxEULA
            // 
            this.richTextBoxEULA.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxEULA.Location = new System.Drawing.Point(200, 82);
            this.richTextBoxEULA.Name = "richTextBoxEULA";
            this.richTextBoxEULA.ReadOnly = true;
            this.richTextBoxEULA.Size = new System.Drawing.Size(380, 235);
            this.richTextBoxEULA.TabIndex = 49;
            this.richTextBoxEULA.Text = "";
            this.richTextBoxEULA.TextChanged += new System.EventHandler(this.richTextBoxEULA_TextChanged);
            // 
            // checkBoxAccept
            // 
            this.checkBoxAccept.AutoSize = true;
            this.checkBoxAccept.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxAccept.Location = new System.Drawing.Point(200, 336);
            this.checkBoxAccept.Name = "checkBoxAccept";
            this.checkBoxAccept.Size = new System.Drawing.Size(308, 17);
            this.checkBoxAccept.TabIndex = 1;
            this.checkBoxAccept.Text = "I accept the terms and conditions of this License Agreement";
            this.checkBoxAccept.UseVisualStyleBackColor = false;
            this.checkBoxAccept.CheckedChanged += new System.EventHandler(this.checkBoxAccept_CheckedChanged);
            // 
            // labelDesc
            // 
            this.labelDesc.AutoSize = true;
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDesc.Location = new System.Drawing.Point(197, 56);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(314, 13);
            this.labelDesc.TabIndex = 51;
            this.labelDesc.Text = "Please read carefully the following License Agreement";
            // 
            // EULA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.checkBoxAccept);
            this.Controls.Add(this.richTextBoxEULA);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EULA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EULA_FormClosing);
            this.Load += new System.EventHandler(this.EULA_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void richTextBoxEULA_TextChanged(object sender, EventArgs e)
        {
            
        }

        #endregion

        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.RichTextBox richTextBoxEULA;
        private System.Windows.Forms.CheckBox checkBoxAccept;
        private System.Windows.Forms.Label labelDesc;
    }
}