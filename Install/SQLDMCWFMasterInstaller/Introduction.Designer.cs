namespace Installer_form_application
{
    partial class Introduction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Introduction));
            this.button1 = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.LabelHere = new System.Windows.Forms.LinkLabel();
            this.linkLabelGuide = new System.Windows.Forms.LinkLabel();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelHeading, "labelHeading");
            this.labelHeading.Name = "labelHeading";
            // 
            // LabelHere
            // 
            this.LabelHere.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LabelHere, "LabelHere");
            this.LabelHere.Name = "LabelHere";
            this.LabelHere.UseCompatibleTextRendering = true;
            // 
            // linkLabelGuide
            // 
            this.linkLabelGuide.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelGuide, "linkLabelGuide");
            this.linkLabelGuide.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelGuide.Name = "linkLabelGuide";
            this.linkLabelGuide.TabStop = true;
            this.linkLabelGuide.UseCompatibleTextRendering = true;
            this.linkLabelGuide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGuide_LinkClicked);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Introduction
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.linkLabelGuide);
            this.Controls.Add(this.LabelHere);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Introduction";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Introduction_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.LinkLabel linkLabelGuide;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.LinkLabel LabelHere;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
    }
}

