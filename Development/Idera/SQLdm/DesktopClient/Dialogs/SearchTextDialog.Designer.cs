namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SearchTextDialog
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
            this.searchTextLabel = new System.Windows.Forms.Label();
            this.searchTextTextBox = new System.Windows.Forms.TextBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.searchUpCheckBox = new System.Windows.Forms.CheckBox();
            this.findNextButton = new System.Windows.Forms.Button();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.LookInLabel = new System.Windows.Forms.Label();
            this.searchListComboBox = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchTextLabel
            // 
            this.searchTextLabel.AutoSize = true;
            this.searchTextLabel.Location = new System.Drawing.Point(10, 6);
            this.searchTextLabel.Name = "searchTextLabel";
            this.searchTextLabel.Size = new System.Drawing.Size(56, 13);
            this.searchTextLabel.TabIndex = 0;
            this.searchTextLabel.Text = "Fi&nd what:";
            // 
            // searchTextTextBox
            // 
            this.searchTextTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextTextBox.Location = new System.Drawing.Point(12, 22);
            this.searchTextTextBox.Name = "searchTextTextBox";
            this.searchTextTextBox.Size = new System.Drawing.Size(344, 20);
            this.searchTextTextBox.TabIndex = 0;
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(10, 19);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.matchCaseCheckBox.TabIndex = 3;
            this.matchCaseCheckBox.Text = "Match &case";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchUpCheckBox
            // 
            this.searchUpCheckBox.AutoSize = true;
            this.searchUpCheckBox.Location = new System.Drawing.Point(10, 42);
            this.searchUpCheckBox.Name = "searchUpCheckBox";
            this.searchUpCheckBox.Size = new System.Drawing.Size(75, 17);
            this.searchUpCheckBox.TabIndex = 4;
            this.searchUpCheckBox.Text = "Search &up";
            this.searchUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(171, 164);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(85, 23);
            this.findNextButton.TabIndex = 5;
            this.findNextButton.Text = "&Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionsGroupBox.Controls.Add(this.searchUpCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(12, 88);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(344, 68);
            this.optionsGroupBox.TabIndex = 2;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Find options";
            // 
            // LookInLabel
            // 
            this.LookInLabel.AutoSize = true;
            this.LookInLabel.Location = new System.Drawing.Point(10, 46);
            this.LookInLabel.Name = "LookInLabel";
            this.LookInLabel.Size = new System.Drawing.Size(45, 13);
            this.LookInLabel.TabIndex = 6;
            this.LookInLabel.Text = "&Look in:";
            // 
            // searchListComboBox
            // 
            this.searchListComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchListComboBox.Location = new System.Drawing.Point(12, 62);
            this.searchListComboBox.MinimumSize = new System.Drawing.Size(185, 0);
            this.searchListComboBox.Name = "searchListComboBox";
            this.searchListComboBox.Size = new System.Drawing.Size(244, 21);
            this.searchListComboBox.TabIndex = 1;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(271, 164);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(85, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // SearchTextDialog
            // 
            this.AcceptButton = this.findNextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 208);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.searchListComboBox);
            this.Controls.Add(this.LookInLabel);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.searchTextTextBox);
            this.Controls.Add(this.searchTextLabel);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 236);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(217, 236);
            this.Name = "SearchTextDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = " Find";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchTextDialog_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextDialog_KeyDown);
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label searchTextLabel;
        private System.Windows.Forms.TextBox searchTextTextBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.CheckBox searchUpCheckBox;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Label LookInLabel;
        private System.Windows.Forms.ComboBox searchListComboBox;
        private System.Windows.Forms.Button closeButton;
    }
}
