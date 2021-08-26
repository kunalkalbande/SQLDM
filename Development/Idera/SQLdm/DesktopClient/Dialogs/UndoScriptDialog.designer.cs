namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class UndoScriptDialog
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
            ActiproSoftware.SyntaxEditor.Document document1 = new ActiproSoftware.SyntaxEditor.Document();
            ActiproSoftware.SyntaxEditor.VisualStudio2005SyntaxEditorRenderer visualStudio2005SyntaxEditorRenderer1 = new ActiproSoftware.SyntaxEditor.VisualStudio2005SyntaxEditorRenderer();
            this.btnCancel = new System.Windows.Forms.Button();
            this.syntaxEditor1 = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            //this.txtActiSyntax = new System.Windows.Forms.TextBox();
            this.btnOptimizeNow = new System.Windows.Forms.Button();
            this.workerLoad = new System.ComponentModel.BackgroundWorker();
            this._loading = new MRG.Controls.UI.LoadingCircle();
            this.lblLoading = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(492, 450);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // syntaxEditor1
            // 
            this.syntaxEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            document1.ReadOnly = true;
            document1.Text = "Select * from Table_1";
            this.syntaxEditor1.Document = document1;
            this.syntaxEditor1.Location = new System.Drawing.Point(8, 8);
            this.syntaxEditor1.Name = "syntaxEditor1";
            visualStudio2005SyntaxEditorRenderer1.ResetAllPropertiesOnSystemColorChange = false;
            this.syntaxEditor1.Renderer = visualStudio2005SyntaxEditorRenderer1;
            this.syntaxEditor1.Size = new System.Drawing.Size(559, 436);
            this.syntaxEditor1.TabIndex = 0;
            ////
            ////txtActiSyntax
            //// 
            //this.txtActiSyntax.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //            | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.txtActiSyntax.BackColor = System.Drawing.Color.White;
            //this.txtActiSyntax.Location = new System.Drawing.Point(8, 8);
            //this.txtActiSyntax.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            //this.txtActiSyntax.MinimumSize = new System.Drawing.Size(0, 20);
            //this.txtActiSyntax.Multiline = true;
            //this.txtActiSyntax.Name = "actiSyntaxTextBox";
            //this.txtActiSyntax.ReadOnly = true;
            //this.txtActiSyntax.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            //this.txtActiSyntax.Size = new System.Drawing.Size(559, 436);
            //this.txtActiSyntax.TabIndex = 19;
            //this.txtActiSyntax.Text = "< Undo Command >";
            // btnOptimizeNow
            // 
            this.btnOptimizeNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptimizeNow.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOptimizeNow.Location = new System.Drawing.Point(411, 450);
            this.btnOptimizeNow.Name = "btnOptimizeNow";
            this.btnOptimizeNow.Size = new System.Drawing.Size(75, 23);
            this.btnOptimizeNow.TabIndex = 1;
            this.btnOptimizeNow.Text = "Run";
            this.btnOptimizeNow.UseVisualStyleBackColor = true;
            this.btnOptimizeNow.Click += new System.EventHandler(this.btnOptimizeNow_Click);
            // 
            // workerLoad
            // 
            this.workerLoad.WorkerReportsProgress = true;
            this.workerLoad.WorkerSupportsCancellation = true;
            this.workerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerLoad_DoWork);
            this.workerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerLoad_RunWorkerCompleted);
            this.workerLoad.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerLoad_ProgressChanged);
            // 
            // _loading
            // 
            this._loading.Active = false;
            this._loading.BackColor = System.Drawing.Color.White;
            this._loading.Color = System.Drawing.Color.DarkGray;
            this._loading.InnerCircleRadius = 5;
            this._loading.Location = new System.Drawing.Point(33, 21);
            this._loading.Name = "_loading";
            this._loading.NumberSpoke = 12;
            this._loading.OuterCircleRadius = 11;
            this._loading.RotationSpeed = 100;
            this._loading.Size = new System.Drawing.Size(30, 24);
            this._loading.SpokeThickness = 2;
            this._loading.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this._loading.TabIndex = 33;
            this._loading.Text = "loadingCircle1";
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.BackColor = System.Drawing.Color.White;
            this.lblLoading.Location = new System.Drawing.Point(69, 27);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(54, 13);
            this.lblLoading.TabIndex = 34;
            this.lblLoading.Text = Idera.SQLdm.Common.Constants.LOADING;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(8, 450);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 35;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // UndoScriptDialog
            // 
            this.AcceptButton = this.btnOptimizeNow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(575, 481);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this._loading);
            this.Controls.Add(this.btnOptimizeNow);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.syntaxEditor1);
            //this.Controls.Add(this.txtActiSyntax);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(591, 519);
            this.Name = "UndoScriptDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Undo Optimization";
            this.Load += new System.EventHandler(this.UndoScriptDialog_Load);
            this.Shown += new System.EventHandler(this.UndoScriptDialog_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UndoScriptDialog_FormClosing);
            //this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.UndoScriptDialog_HelpRequested);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ActiproSoftware.SyntaxEditor.SyntaxEditor syntaxEditor1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOptimizeNow;
        private System.ComponentModel.BackgroundWorker workerLoad;
        private MRG.Controls.UI.LoadingCircle _loading;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Button btnCopy;
        //Temp TextBox
        //private System.Windows.Forms.TextBox txtActiSyntax;
    }
}