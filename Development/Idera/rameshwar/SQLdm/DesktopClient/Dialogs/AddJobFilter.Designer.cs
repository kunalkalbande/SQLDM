using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddJobFilter
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
            Infragistics.Win.Misc.ValidationGroup validationGroup10 = new Infragistics.Win.Misc.ValidationGroup("checkCategory");
            Infragistics.Win.Misc.ValidationGroup validationGroup11 = new Infragistics.Win.Misc.ValidationGroup("checkName");
            Infragistics.Win.Misc.ValidationGroup validationGroup12 = new Infragistics.Win.Misc.ValidationGroup("checkStep");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.filterExists = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.jobValue = new System.Windows.Forms.TextBox();
            this.stepValue = new System.Windows.Forms.TextBox();
            this.catValue = new System.Windows.Forms.TextBox();
            this.stepOpCode = new System.Windows.Forms.ComboBox();
            this.jobOpCode = new System.Windows.Forms.ComboBox();
            this.catOpCode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.filterExists);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.jobValue);
            this.groupBox1.Controls.Add(this.stepValue);
            this.groupBox1.Controls.Add(this.catValue);
            this.groupBox1.Controls.Add(this.stepOpCode);
            this.groupBox1.Controls.Add(this.jobOpCode);
            this.groupBox1.Controls.Add(this.catOpCode);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 146);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // filterExists
            // 
            this.filterExists.AutoSize = true;
            this.filterExists.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filterExists.ForeColor = System.Drawing.Color.Red;
            this.filterExists.Location = new System.Drawing.Point(109, 117);
            this.filterExists.Name = "filterExists";
            this.filterExists.Size = new System.Drawing.Size(141, 13);
            this.filterExists.TabIndex = 12;
            this.filterExists.Text = "This filter already exists";
            this.filterExists.Visible = false;
            this.filterExists.VisibleChanged += new System.EventHandler(this.FilterExists_VisibleChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(251, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Use % for wildcards";
            // 
            // jobValue
            // 
            this.jobValue.Location = new System.Drawing.Point(175, 60);
            this.jobValue.Name = "jobValue";
            this.jobValue.Size = new System.Drawing.Size(175, 20);
            this.jobValue.TabIndex = 9;
            this.validator.GetValidationSettings(this.jobValue).NotificationSettings.Text = "You must specify a job name or wildcard value.";
            this.validator.GetValidationSettings(this.jobValue).RetainFocusOnError = true;
            this.validator.GetValidationSettings(this.jobValue).ValidationGroupKey = "checkName";
            this.validator.GetValidationSettings(this.jobValue).ValidationPropertyName = "Text";
            this.jobValue.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // stepValue
            // 
            this.stepValue.Location = new System.Drawing.Point(175, 93);
            this.stepValue.Name = "stepValue";
            this.stepValue.Size = new System.Drawing.Size(175, 20);
            this.stepValue.TabIndex = 10;
            this.validator.GetValidationSettings(this.stepValue).NotificationSettings.Text = "You must specify a step name of wildcard value.";
            this.validator.GetValidationSettings(this.stepValue).RetainFocusOnError = true;
            this.validator.GetValidationSettings(this.stepValue).ValidationGroupKey = "checkStep";
            this.validator.GetValidationSettings(this.stepValue).ValidationPropertyName = "Text";
            this.stepValue.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // catValue
            // 
            this.catValue.Location = new System.Drawing.Point(175, 29);
            this.catValue.Name = "catValue";
            this.catValue.Size = new System.Drawing.Size(175, 20);
            this.catValue.TabIndex = 8;
            this.validator.GetValidationSettings(this.catValue).NotificationSettings.Text = "You must specify a job category name or a wildcard value.";
            this.validator.GetValidationSettings(this.catValue).RetainFocusOnError = true;
            this.validator.GetValidationSettings(this.catValue).ValidationGroupKey = "checkCategory";
            this.validator.GetValidationSettings(this.catValue).ValidationPropertyName = "Text";
            this.catValue.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // stepOpCode
            // 
            this.stepOpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stepOpCode.FormattingEnabled = true;
            this.stepOpCode.Location = new System.Drawing.Point(87, 93);
            this.stepOpCode.MaxDropDownItems = 2;
            this.stepOpCode.Name = "stepOpCode";
            this.stepOpCode.Size = new System.Drawing.Size(82, 21);
            this.stepOpCode.TabIndex = 7;
            this.stepOpCode.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
            // 
            // jobOpCode
            // 
            this.jobOpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.jobOpCode.FormattingEnabled = true;
            this.jobOpCode.Location = new System.Drawing.Point(87, 60);
            this.jobOpCode.MaxDropDownItems = 2;
            this.jobOpCode.Name = "jobOpCode";
            this.jobOpCode.Size = new System.Drawing.Size(82, 21);
            this.jobOpCode.TabIndex = 6;
            this.jobOpCode.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
            // 
            // catOpCode
            // 
            this.catOpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.catOpCode.FormattingEnabled = true;
            this.catOpCode.Location = new System.Drawing.Point(87, 29);
            this.catOpCode.MaxDropDownItems = 2;
            this.catOpCode.Name = "catOpCode";
            this.catOpCode.Size = new System.Drawing.Size(82, 21);
            this.catOpCode.TabIndex = 5;
            this.catOpCode.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Step Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Job Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Job Category";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(175, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(87, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operator";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(300, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(219, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(13, 158);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // validator
            // 
            this.validator.MessageBoxIcon = System.Windows.Forms.MessageBoxIcon.None;
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Caption = "Input required";
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup10.Key = "checkCategory";
            validationGroup11.Key = "checkName";
            validationGroup12.Key = "checkStep";
            this.validator.ValidationGroups.Add(validationGroup10);
            this.validator.ValidationGroups.Add(validationGroup11);
            this.validator.ValidationGroups.Add(validationGroup12);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // AddJobFilter
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle=FormBorderStyle.FixedDialog;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(394, 191);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddJobFilter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Job Filter";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AddJobFilter_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddJobFilter_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox jobValue;
        private System.Windows.Forms.TextBox stepValue;
        private System.Windows.Forms.TextBox catValue;
        private System.Windows.Forms.ComboBox stepOpCode;
        private System.Windows.Forms.ComboBox jobOpCode;
        private System.Windows.Forms.ComboBox catOpCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label filterExists;
        private Infragistics.Win.Misc.UltraValidator validator;
    }
}