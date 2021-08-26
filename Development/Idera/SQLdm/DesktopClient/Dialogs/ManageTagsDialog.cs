using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ManageTagsDialog : BaseDialog
    {
        private readonly Dictionary<int, ListViewItem> tagItemLookupTable = new Dictionary<int, ListViewItem>();

        public ManageTagsDialog()
        {
            this.DialogHeader = "Manage Tags";
            InitializeComponent();
            getTagsWorker.RunWorkerAsync();
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            SetPropertiesTheme();
            Infragistics.Windows.Themes.ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, System.EventArgs e)
        {
            SetPropertiesTheme();
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(office2007PropertyPage1);
        }

        private void getTagsWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ApplicationModel.Default.RefreshTags();
            e.Cancel = getTagsWorker.CancellationPending;
        }

        private void getTagsWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                statusLabel.Hide();

                if (e.Error == null)
                {
                    if (ApplicationModel.Default.LocalTags.Count > 0)
                    {
                        tagsListView.BeginUpdate();

                        //SQLDM 10.1 (Srishti Purohit)
                        //removing global tags as they can not be edited from SQLdm console
                        foreach (Tag tag in ApplicationModel.Default.LocalTags)
                        {
                            AddTag(tag);
                        }

                        tagsListView.EndUpdate();
                    }
                    else
                    {
                        statusLabel.ForeColor = SystemColors.GrayText;
                        statusLabel.Text = "Click the Add button to create a tag.";
                        statusLabel.Show();
                    }
                    ApplicationModel.Default.LocalTags.Changed += Tags_Changed;  
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while loading tags.", e.Error);
                }
            }
        }

        private void AddTag(Tag tag)
        {
            if (tag != null)
            {
                ListViewItem tagItem = new ListViewItem(tag.Name);
                tagItem.SubItems.Add(tag.Instances.Count.ToString());
                tagItem.SubItems.Add(tag.CustomCounters.Count.ToString());
                tagItem.SubItems.Add(tag.Permissions.Count.ToString());
                tagItem.Tag = tag;
                tagsListView.Items.Add(tagItem);
                tagItemLookupTable.Add(tag.Id, tagItem);
            }
        }

        private void RemoveTag(Tag tag)
        {
            if (tag != null)
            {
                ListViewItem existingItem;

                if (tagItemLookupTable.TryGetValue(tag.Id, out existingItem))
                {
                    existingItem.Remove();
                    tagItemLookupTable.Remove(tag.Id);
                }
            }
        }

        private void UpdateTag(Tag tag)
        {
            ListViewItem existingItem;

            if (tag != null && tagItemLookupTable.TryGetValue(tag.Id, out existingItem))
            {
                existingItem.SubItems[0].Text = tag.Name;
                existingItem.SubItems[1].Text = tag.Instances.Count.ToString();
                existingItem.SubItems[2].Text = tag.CustomCounters.Count.ToString();
                existingItem.SubItems[3].Text = tag.Permissions.Count.ToString();
                existingItem.Tag = tag;
            }
        }

        private void Tags_Changed(object sender, TagCollectionChangedEventArgs e)
        {
            tagsListView.BeginUpdate();

            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        AddTag(tag);
                    }
                    tagsListView.Sort();
                    break;
                case KeyedCollectionChangeType.Removed:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        RemoveTag(tag);
                    }
                    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        UpdateTag(tag);
                    }
                    tagsListView.Sort();
                    break;
                case KeyedCollectionChangeType.Cleared:
                    tagsListView.Items.Clear();
                    tagItemLookupTable.Clear();
                    break;
            }

            tagsListView.EndUpdate();

            if (tagsListView.Items.Count == 0)
            {
                statusLabel.ForeColor = SystemColors.GrayText;
                statusLabel.Text = "Click the Add button to create a tag.";
                statusLabel.Show();
            }
            else
            {
                statusLabel.Hide();
            }
        }

        private void ManageTagsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            getTagsWorker.CancelAsync();
            removeTagsWorker.CancelAsync();
        }

        private void tagsListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            editButton.Enabled = tagsListView.SelectedItems.Count == 1;
            removeButton.Enabled = tagsListView.SelectedItems.Count > 0;
        }

        private void addButton_Click(object sender, System.EventArgs e)
        {
            AddTag();
        }

        private void editButton_Click(object sender, System.EventArgs e)
        {
            EditTag();
        }

        private void removeButton_Click(object sender, System.EventArgs e)
        {
            RemoveSelectedTags();
        }

        private void tagsListView_DoubleClick(object sender, System.EventArgs e)
        {
            EditTag();
        }

        private void AddTag()
        {
            TagPropertiesDialog dialog = new TagPropertiesDialog();
            dialog.ShowDialog(this);
        }

        private void EditTag()
        {
            if (tagsListView.SelectedItems.Count == 1)
            {
                TagPropertiesDialog dialog = new TagPropertiesDialog(tagsListView.SelectedItems[0].Tag as Tag);
                dialog.ShowDialog(this);
            }
        }

        private void RemoveSelectedTags()
        {
            if (tagsListView.SelectedItems.Count > 0)
            {
                bool doTagLinksExist = false;

                List<Tag> tags = new List<Tag>();

                foreach (ListViewItem tagItem in tagsListView.SelectedItems)
                {
                    Tag tag = tagItem.Tag as Tag;

                    if (tag != null)
                    {
                        if (tag.Instances.Count > 0 || tag.CustomCounters.Count > 0 || tag.Permissions.Count > 0)
                        {
                            doTagLinksExist = true;
                        }

                        tags.Add(tag);
                    }
                }

                if (doTagLinksExist)
                {
                    string message = tags.Count == 1
                                         ? "The selected tag is associated with one or more SQLDM objects. Are you sure you want to delete this tag?"
                                         : "One or more of the selected tags are associated with SQLDM objects. Are you sure you want to delete these tags?";

                    if (ApplicationMessageBox.ShowWarning(this, message, ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }

                removeTagsWorker.RunWorkerAsync(tags);
                Cursor = Cursors.WaitCursor;
            }
        }

        private void removeTagsWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ApplicationModel.Default.RemoveTags(e.Argument as IList<Tag>);
            e.Cancel = removeTagsWorker.CancellationPending;
        }

        private void removeTagsWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                Cursor = Cursors.Default;

                if (e.Error == null)
                {
                    editButton.Enabled =
                        removeButton.Enabled = false;
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while removing tags.", e.Error);
                }
            }
        }

        private void ManageTagsDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ShowHelp();
        }

        private void ManageTagsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ShowHelp();
        }

        private static void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.TagsManageTags);
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.manageTagsMainContainer, AutoScaleSizeHelper.ControlType.Form, new SizeF(1.2F, 1.2F));
                this.statusLabel.Size = new Size(this.statusLabel.Width - 50, this.statusLabel.Height + 20);
                this.statusLabel.Location = new Point(this.statusLabel.Location.X + 5, this.statusLabel.Location.Y);
                this.label4.Scale(new SizeF(1.2F, 1.2F));
                this.columnHeader1.Width = 308;
                this.columnHeader2.Width = 154;
                this.columnHeader3.Width = 154;
                this.columnHeader4.Width = 154;
                this.addButton.Height -= 10;
                this.addButton.Location = new Point(this.addButton.Location.X, this.addButton.Location.Y - 10);
                this.removeButton.Height -= 10;
                this.removeButton.Location = new Point(this.removeButton.Location.X, this.removeButton.Location.Y - 10);
                this.editButton.Height -= 10;
                this.editButton.Location = new Point(this.editButton.Location.X, this.editButton.Location.Y - 10);
                manageTagsButtonsContainer.Height += 10;
                return;
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.manageTagsMainContainer, AutoScaleSizeHelper.ControlType.Form, new SizeF(1.2F, 1.2F));
                this.statusLabel.Size = new Size(this.statusLabel.Width - 50, this.statusLabel.Height + 20);
                this.statusLabel.Location = new Point(this.statusLabel.Location.X + 5, this.statusLabel.Location.Y);
                this.label4.Scale(new SizeF(1.2F, 1.2F));
                this.columnHeader1.Width = 350;
                this.columnHeader2.Width = 200;
                this.columnHeader3.Width = 200;
                this.columnHeader4.Width = 200;
                this.addButton.Height -= 10;
                this.addButton.Location = new Point(this.addButton.Location.X, this.addButton.Location.Y - 10);
                this.removeButton.Height -= 10;
                this.removeButton.Location = new Point(this.removeButton.Location.X, this.removeButton.Location.Y - 10);
                this.editButton.Height -= 10;
                this.editButton.Location = new Point(this.editButton.Location.X, this.editButton.Location.Y - 10);
                manageTagsButtonsContainer.Height += 10;
                return;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.manageTagsMainContainer, AutoScaleSizeHelper.ControlType.Form, new SizeF(1.2F, 1.2F));
                this.statusLabel.Size = new Size(this.statusLabel.Width - 50, this.statusLabel.Height + 20);
                this.statusLabel.Location = new Point(this.statusLabel.Location.X + 5, this.statusLabel.Location.Y);
                this.label4.Scale(new SizeF(1.2F, 1.2F));
                this.columnHeader1.Width = 416;
                this.columnHeader2.Width = 208;
                this.columnHeader3.Width = 208;
                this.columnHeader4.Width = 208;
                this.addButton.Height -= 10;
                this.addButton.Location = new Point(this.addButton.Location.X, this.addButton.Location.Y - 10);
                this.removeButton.Height -= 10;
                this.removeButton.Location = new Point(this.removeButton.Location.X, this.removeButton.Location.Y - 10);
                this.editButton.Height -= 10;
                this.editButton.Location = new Point(this.editButton.Location.X, this.editButton.Location.Y - 10);
                manageTagsButtonsContainer.Height += 10;
                return;
            }
        }
    }
}