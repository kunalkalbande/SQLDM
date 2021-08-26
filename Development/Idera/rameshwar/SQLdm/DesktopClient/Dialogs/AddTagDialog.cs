using System;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AddTagDialog : Form
    {
        private int tagId = -1;

        public AddTagDialog()
        {
            InitializeComponent();
        }

        public int TagId
        {
            get { return tagId; }
        }

        public string TagName
        {
            get { return tagTextBox.Text.Trim(); }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string newTagName = tagTextBox.Text.Trim();
            bool isUniqueName = true;

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (newTagName.ToLower() == tag.Name.ToLower())
                {
                    isUniqueName = false;
                    break;
                }
            }

            if (!isUniqueName)
            {
                ApplicationMessageBox.ShowInfo(this, "The specified tag name already exists. Please provide a unique tag name.");
            }
            else
            {
                tagTextBox.Enabled =
                    okButton.Enabled =
                    cancelButton.Enabled = false;
                Cursor = Cursors.WaitCursor;
                backgroundWorker.RunWorkerAsync(newTagName);
            }
        }
        
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string newTagName = e.Argument as string;
            tagId = ApplicationModel.Default.AddOrUpdateTag(new Tag(-1, newTagName));
            e.Cancel = backgroundWorker.CancellationPending;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    tagTextBox.Enabled =
                        okButton.Enabled =
                        cancelButton.Enabled = true;
                    Cursor = Cursors.Default;
                    ApplicationMessageBox.ShowError(this, "An error occurred while adding the tag.", e.Error);
                }
            }
        }

        private void tagTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = tagTextBox.Text.Trim().Length > 0;
        }
    }
}