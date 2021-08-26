using System;
using System.ComponentModel;
using Idera.SQLdm.Common.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.Misc;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AddJobFilter : BaseDialog
    {

        private const string WILDCARD = "%";
        private const string EQUAL_OPER_VALUE_MISMATCH = "If the Operator is 'Equals' the Value field cannot contain a wilcard character.";
        private const string LIKE_OPER_VALUE_MISMATCH = "If the Operator is 'Like' the Value field must contain a wilcard (%) character.";

        private string[] opCodes = { OpCodes.Like.ToString(), OpCodes.Equals.ToString() };
        int instanceId;

        public AddJobFilter(int instanceID, bool editStep)
        {
            this.DialogHeader = "Add Job Filter";
            InitializeComponent();

            this.instanceId = instanceID;

            UpdateControls();

            this.btnBrowse.Visible = (instanceID > 0);
            this.label5.Visible = editStep;
            this.stepOpCode.Visible = editStep;
            this.stepValue.Visible = editStep;
            // Set new location for label6 if 'editStep' is false.
            SetLocationForWildcardsInfoLabel(editStep);

            this.catOpCode.Items.AddRange(opCodes);
            this.catOpCode.SelectedIndex = (int)OpCodes.Like;
            this.jobOpCode.Items.AddRange(opCodes);
            this.jobOpCode.SelectedIndex = (int)OpCodes.Like;
            this.stepOpCode.Items.AddRange(opCodes);
            this.stepOpCode.SelectedIndex = (int)OpCodes.Like;

            StepName = (editStep) ? StepName : WILDCARD;
            AdaptFontSize();
        }

        /// <summary>
        /// Set the position for the label 6 (wild card information label). Increase the vertical position
        /// if 'editStep' is disabled.
        /// </summary>
        /// <param name="editStep">Indicates if the 'edit step' is enabled</param>
        private void SetLocationForWildcardsInfoLabel(bool editStep)
        {
            if (!editStep)
            {
                Point oldPosition = label6.Location;
                label6.Location = new Point(oldPosition.X, oldPosition.Y - label5.Height - 13);
            }
        }

        public string CatOpCode
        {
            get { return this.catOpCode.Text.Trim(); }
            set { this.catOpCode.SelectedItem = value; }
        }

        public string Category
        {
            get { return this.catValue.Text.Trim(); }
            set 
            { 
                this.catValue.Text = value;
                if (this.catValue.Text.Contains(WILDCARD))
                    CatOpCode = OpCodes.Like.ToString();
            }
        }

        public string JobOpCode
        {
            get { return this.jobOpCode.Text.Trim(); }
            set { this.jobOpCode.SelectedItem = value; }
        }

        public string JobName
        {
            get { return this.jobValue.Text.Trim(); }
            set 
            { 
                this.jobValue.Text = value;
                if (this.jobValue.Text.Contains(WILDCARD))
                    JobOpCode = OpCodes.Like.ToString();
            }
        }

        public string StepOpCode
        {
            get { return this.stepOpCode.Text.Trim(); }
            set { this.stepOpCode.SelectedItem = value; }
        }

        public string StepName
        {
            get { return this.stepValue.Text.Trim(); }
            set 
            { 
                this.stepValue.Text = value;
                if (this.stepValue.Text.Contains(WILDCARD))
                    StepOpCode = OpCodes.Like.ToString();
            }
        }

        public bool FilterExists
        {
            set { this.filterExists.Visible = value; }
        }

        public string FilterExistsText
        {
            set { this.filterExists.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Category.Contains(WILDCARD))
                CatOpCode = OpCodes.Like.ToString();
            if (JobName.Contains(WILDCARD))
                JobOpCode = OpCodes.Like.ToString();
            if (StepName.Contains(WILDCARD))
                StepOpCode = OpCodes.Like.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            BrowseJobStepsDialog dialog = new BrowseJobStepsDialog(instanceId);
            if (DialogResult.OK == dialog.ShowDialog(this))
            {
                CatOpCode = OpCodes.Equals.ToString();
                Category = dialog.Category;
                JobOpCode = OpCodes.Equals.ToString();
                JobName = dialog.JobName;
                StepOpCode = OpCodes.Equals.ToString();
                StepName = dialog.StepName;
            }
            dialog.Dispose();
        }

        private void FilterExists_VisibleChanged(object sender, EventArgs e)
        {
            label6.Visible = !filterExists.Visible;
        }

        private void AddJobFilter_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.AddJobStepFilter);
        }

        private void AddJobFilter_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.AddJobStepFilter);
        }

        private void UpdateControls()
        {
            bool catOK, jobOK, stepOK;
            Validation check;

            this.btnOK.Enabled = false;

            check = validator.Validate("checkCategory", true, false);
            if (check == null)
                catOK = false;
            else 
                catOK = check.IsValid;

            check = validator.Validate("checkName", true, false);
            if (check == null)
                jobOK = false;
            else 
                jobOK = check.IsValid;

            check = validator.Validate("checkStep", true, false);
            if (check == null)
                stepOK = false;
            else 
                stepOK = check.IsValid;


            this.btnOK.Enabled = (catOK && jobOK && stepOK);

        }

        private void OnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void validator_Validating(object sender, ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            string value = e.Value == null ? null : e.Value.ToString().Trim();

            // Assume everything is Okie Dokie unless proven otherwise...
            e.IsValid = true;

            if (String.IsNullOrEmpty(value))
            {
                e.IsValid = false;
            }
            else
            {
                if (e.ValidationSettings.ValidationGroup.Key == "checkCategory")
                {
                    if (e.Value.ToString().Contains(WILDCARD))
                    {
                        if (catOpCode.SelectedIndex == (int)OpCodes.Equals)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = EQUAL_OPER_VALUE_MISMATCH;
                        }
                    }
                    else
                    {
                        if (catOpCode.SelectedIndex == (int)OpCodes.Like)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = LIKE_OPER_VALUE_MISMATCH;
                        }
                    }
                }
                else if (e.ValidationSettings.ValidationGroup.Key == "checkName")
                {
                    if (e.Value.ToString().Contains(WILDCARD))
                    {
                        if (jobOpCode.SelectedIndex == (int)OpCodes.Equals)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = EQUAL_OPER_VALUE_MISMATCH;
                        }
                    }
                    else
                    {
                        if (jobOpCode.SelectedIndex == (int)OpCodes.Like)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = LIKE_OPER_VALUE_MISMATCH;
                        }
                    }
                }
                else if (e.ValidationSettings.ValidationGroup.Key == "checkStep")
                {
                    if (e.Value.ToString().Contains(WILDCARD))
                    {
                        if (stepOpCode.SelectedIndex == (int)OpCodes.Equals)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = EQUAL_OPER_VALUE_MISMATCH;
                        }
                    }
                    else
                    {
                        if (stepOpCode.SelectedIndex == (int)OpCodes.Like)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = LIKE_OPER_VALUE_MISMATCH;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}