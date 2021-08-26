namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddEditAlertTemplate
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("checkName");
            this.label1 = new System.Windows.Forms.Label();
            this.templateName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.templateDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.cboBaseTemplate = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkMakeDefault = new System.Windows.Forms.CheckBox();
            this.btnEditConfig = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBaseTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // templateName
            // 
            this.templateName.Location = new System.Drawing.Point(57, 13);
            this.templateName.Name = "templateName";
            this.templateName.Size = new System.Drawing.Size(384, 20);
            this.templateName.TabIndex = 1;
            this.validator.GetValidationSettings(this.templateName).IsRequired = true;
            this.validator.GetValidationSettings(this.templateName).NotificationSettings.Text = "You must specicfy a unique name for the Alert Template";
            this.validator.GetValidationSettings(this.templateName).RetainFocusOnError = true;
            this.validator.GetValidationSettings(this.templateName).ValidationGroupKey = "checkName";
            this.validator.GetValidationSettings(this.templateName).ValidationPropertyName = "Text";
            this.templateName.TextChanged += new System.EventHandler(this.templateName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // templateDescription
            // 
            this.templateDescription.Location = new System.Drawing.Point(16, 67);
            this.templateDescription.Multiline = true;
            this.templateDescription.Name = "templateDescription";
            this.templateDescription.Size = new System.Drawing.Size(425, 84);
            this.templateDescription.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Copy from:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(381, 206);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(300, 206);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // validator
            // 
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "checkName";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // cboBaseTemplate
            // 
            this.cboBaseTemplate.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboBaseTemplate.Location = new System.Drawing.Point(19, 175);
            this.cboBaseTemplate.Name = "cboBaseTemplate";
            this.cboBaseTemplate.Size = new System.Drawing.Size(260, 21);
            this.cboBaseTemplate.TabIndex = 9;
            this.cboBaseTemplate.ValueChanged += new System.EventHandler(this.cboBaseTemplate_ValueChanged);
            // 
            // chkMakeDefault
            // 
            this.chkMakeDefault.AutoSize = true;
            this.chkMakeDefault.Location = new System.Drawing.Point(300, 175);
            this.chkMakeDefault.Name = "chkMakeDefault";
            this.chkMakeDefault.Size = new System.Drawing.Size(107, 17);
            this.chkMakeDefault.TabIndex = 10;
            this.chkMakeDefault.Text = "Default Template";
            this.chkMakeDefault.UseVisualStyleBackColor = true;
            this.chkMakeDefault.CheckedChanged += new System.EventHandler(this.makeDefault_CheckedChanged);
            // 
            // btnEditConfig
            // 
            this.btnEditConfig.Location = new System.Drawing.Point(19, 206);
            this.btnEditConfig.Name = "btnEditConfig";
            this.btnEditConfig.Size = new System.Drawing.Size(99, 23);
            this.btnEditConfig.TabIndex = 11;
            this.btnEditConfig.Text = "Edit Configuration";
            this.btnEditConfig.UseVisualStyleBackColor = true;
            this.btnEditConfig.Click += new System.EventHandler(this.btnEditConfig_Click);
            // 
            // AddEditAlertTemplate
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(468, 241);
            this.Controls.Add(this.btnEditConfig);
            this.Controls.Add(this.chkMakeDefault);
            this.Controls.Add(this.cboBaseTemplate);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.templateDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.templateName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditAlertTemplate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "{0} Alert Template";
            this.Load += new System.EventHandler(this.AddEditAlertTemplate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBaseTemplate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox templateName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox templateDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private Infragistics.Win.Misc.UltraValidator validator;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboBaseTemplate;
        private System.Windows.Forms.CheckBox chkMakeDefault;
        private System.Windows.Forms.Button btnEditConfig;

    }
}
