using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System.Threading;
    using Common;

    public partial class SelectServerTagsDialog : Form
    {
        private ICollection<string> tags;
        private ICollection<string> selectedTags;
        private bool inUpdateButtons = false;
        private bool singleSelect = false;

        private ThreadStart postDelegate;

        public SelectServerTagsDialog()
        {
            InitializeComponent();
            if (postDelegate == null)
                postDelegate = new ThreadStart(UpdateButtons);
            AdaptFontSize();
        }

        public ICollection<string> Tags
        {
            get
            {
                if (tags == null)
                    tags = new List<string>();
                return tags;
            }
            set
            {
                tags = value;
            }
        }

        public ICollection<string> SelectedTags
        {
            get
            {
                if (selectedTags == null)
                    selectedTags = new List<string>();
                return selectedTags;
            }
            set { selectedTags = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ICollection<string> selected = SelectedTags;
            selected.Clear();
            foreach (string name in tagsListBox.CheckedItems)
            {
                selected.Add(name);
            }

        }

        private void SelectServerTagsDialog_Load(object sender, EventArgs e)
        {
            bool check = false;
            bool haveSelection = false;
            ICollection<string> selected = SelectedTags;
            tagsListBox.Items.Clear();
            foreach (string name in Tags)
            {
                if (singleSelect && haveSelection)
                    check = false;
                else
                {
                    check = selected.Contains(name);
                    haveSelection = check;
                }
                tagsListBox.Items.Add(name, check);
            }
            UpdateButtons();

        }

        private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (inUpdateButtons)
                return;

            bool selectAllState = selectAllCheckBox.Checked;
            int itemCount = tagsListBox.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                tagsListBox.SetItemChecked(i, selectAllState);
            }
            tagsListBox.BeginInvoke(postDelegate);

        }

        private void UpdateButtons()
        {
            inUpdateButtons = true;
            btnOK.Enabled = tagsListBox.CheckedItems.Count > 0;
            selectAllCheckBox.Checked = tagsListBox.CheckedItems.Count == tagsListBox.Items.Count;
            inUpdateButtons = false;
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        private void tagsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (singleSelect && e.NewValue == CheckState.Checked)
            {
                foreach (int i in tagsListBox.CheckedIndices)
                {
                    if (i != e.Index)
                        tagsListBox.SetItemChecked(i, false);
                }
            }
            tagsListBox.BeginInvoke(postDelegate);
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