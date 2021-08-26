using Idera.SQLdm.DesktopClient.Controls;
namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class DependentObjectDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadWorker = new System.ComponentModel.BackgroundWorker();
            this.gradientPanel1 = new GradientPanel();
            this.dependencyTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.label2 = new System.Windows.Forms.Label();
            this.loadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.gradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dependencyTree)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Object Name:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(90, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(359, 20);
            this.textBox1.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(374, 395);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // loadWorker
            // 
            this.loadWorker.WorkerSupportsCancellation = true;
            this.loadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadWorker_DoWork);
            this.loadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadWorker_RunWorkerCompleted);
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.Color.White;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.White;
            this.gradientPanel1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.gradientPanel1.Controls.Add(this.dependencyTree);
            this.gradientPanel1.Controls.Add(this.label2);
            this.gradientPanel1.Controls.Add(this.loadingCircle);
            this.gradientPanel1.Location = new System.Drawing.Point(12, 63);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.gradientPanel1.Size = new System.Drawing.Size(437, 326);
            this.gradientPanel1.TabIndex = 3;
            // 
            // dependencyTree
            // 
            this.dependencyTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dependencyTree.Location = new System.Drawing.Point(1, 1);
            this.dependencyTree.Name = "dependencyTree";
            this.dependencyTree.Size = new System.Drawing.Size(435, 324);
            this.dependencyTree.TabIndex = 0;
            this.dependencyTree.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = Idera.SQLdm.Common.Constants.LOADING;
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingCircle.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle.InnerCircleRadius = 8;
            this.loadingCircle.Location = new System.Drawing.Point(187, 137);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 24;
            this.loadingCircle.OuterCircleRadius = 9;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.Size = new System.Drawing.Size(51, 42);
            this.loadingCircle.SpokeThickness = 4;
            this.loadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle.TabIndex = 1;
            // 
            // DependentObjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(461, 430);
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(477, 468);
            this.Name = "DependentObjectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Dependencies";
            this.Load += new System.EventHandler(this.DependentObjectDialog_Load);
            this.gradientPanel1.ResumeLayout(false);
            this.gradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dependencyTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button cancelButton;
        private GradientPanel gradientPanel1;
        private MRG.Controls.UI.LoadingCircle loadingCircle;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker loadWorker;
        private Infragistics.Win.UltraWinTree.UltraTree dependencyTree;
    }
}