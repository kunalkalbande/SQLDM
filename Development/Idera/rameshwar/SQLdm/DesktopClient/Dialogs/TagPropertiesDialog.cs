using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class TagPropertiesDialog : Form
    {
        private const string TagNameInstructionText = "< Type a tag name >";

        private readonly Tag existingTag;
        private readonly Dictionary<int, string> serverLookupTable = new Dictionary<int, string>();
        private readonly Dictionary<int, string> customCounterLookupTable = new Dictionary<int, string>();
        private readonly Dictionary<int, PermissionDefinition> permissionLookupTable = new Dictionary<int, PermissionDefinition>();
        private bool isApplicationSecurityEnabled = false;
        private bool configurationChanged;
        private bool ignoreCheckedChanged = false;
        private int tagId = -1;
        private List<int> currentCounterChecked = new List<int>();
        private static Dictionary<int, string> serversChecked = new Dictionary<int, string>();

        public TagPropertiesDialog()
            : this(null)
        {
        }

        public TagPropertiesDialog(Tag tag)
        {
            InitializeComponent();

            existingTag = tag;

            if (existingTag == null)
            {
                Text = "Add Tag";
                tagNameTextBox.ForeColor = SystemColors.GrayText;
                tagNameTextBox.Text = TagNameInstructionText;
            }
            else
            {
                tagNameTextBox.Text = existingTag.Name;
                tagId = existingTag.Id;
            }

            initializeWorker.RunWorkerAsync();
            AdaptFontSize();
        }

        public int TagId
        {
            get { return tagId; }
        }

        public int ServerCount
        {
            get { return availableServersListBox.CheckedItems.Count; }
        }

        public int CustomCounterCount
        {
            get { return customCountersListBox.CheckedItems.Count; }
        }

        public int PermissionCount
        {
            get { return permissionsListBox.CheckedItems.Count; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string newTagName = tagNameTextBox.Text.Trim();

            if (!configurationChanged)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                DialogResult = DialogResult.None;
                bool isUniqueName = true;

                foreach (Tag tag in ApplicationModel.Default.Tags)
                {
                    if (existingTag != null && tag.Id == existingTag.Id)
                    {
                        continue;
                    }
                    else
                    {
                        if (newTagName.ToLower() == tag.Name.ToLower())
                        {
                            isUniqueName = false;
                            break;
                        }
                    }
                }

                if (!isUniqueName)
                {
                    ApplicationMessageBox.ShowInfo(this, "The specified tag name already exists. Please provide a unique tag name.");
                }
                else
                {
                    tagNameTextBox.Enabled =
                        availableServersListBox.Enabled =
                        okButton.Enabled =
                        cancelButton.Enabled = false;
                    saveChangesWorker.RunWorkerAsync(newTagName);
                }
            }
        }

        private void tagNameTextBox_Enter(object sender, EventArgs e)
        {
            if (string.CompareOrdinal(TagNameInstructionText, tagNameTextBox.Text) == 0)
            {
                tagNameTextBox.Clear();
            }

            tagNameTextBox.ForeColor = SystemColors.WindowText;
        }

        private void tagNameTextBox_TextChanged(object sender, EventArgs e)
        {
            configurationChanged = true;
            okButton.Enabled = tagNameTextBox.Text.Trim().Length > 0 &&
                               string.CompareOrdinal(TagNameInstructionText, tagNameTextBox.Text.Trim()) != 0;
        }

        private void tagNameTextBox_Leave(object sender, EventArgs e)
        {
            if (tagNameTextBox.Text.Trim().Length == 0)
            {
                tagNameTextBox.ForeColor = SystemColors.GrayText;
                tagNameTextBox.Text = TagNameInstructionText;
            }
        }

        private IList<int> GetCheckedServersIds(string tagname)
        {
            List<int> ids = new List<int>();
            bool newServerAdded = false;

            foreach (CheckboxListItem listItem in availableServersListBox.CheckedItems)
            {
                ids.Add((int)listItem.Tag);
                if (!currentCounterChecked.Contains((int)listItem.Tag))
                {
                    newServerAdded = true;
                }
            }

            if (newServerAdded)
                AuditingEngine.SetAuxiliarData("ServersData", new AuditAuxiliar<bool>(true));

            return ids;
        }

        private IList<int> GetCheckedCustomCounterIds(string tagname)
        {
            var ids = new List<int>();
            var customCounters = new List<KeyValuePair<string, string>>();
            var removedCustomCounters = new List<KeyValuePair<string, string>>();
            var allCustomCounters = GetCustomCounters();

            foreach (CheckboxListItem listItem in customCountersListBox.CheckedItems)
            {
                ids.Add((int)listItem.Tag);

                if (!currentCounterChecked.Contains((int)listItem.Tag))
                {
                    customCounters.Add(new KeyValuePair<string, string>(tagname, listItem.Text));
                }
            }

            foreach (int tagId in currentCounterChecked)
            {
                if (!ids.Contains(tagId))
                {
                    removedCustomCounters.Add(new KeyValuePair<string, string>(tagname, allCustomCounters[tagId]));
                }
            }

            var auditAuxiliar = new AuditAuxiliar<List<KeyValuePair<string, string>>>(customCounters);
            var auditAuxiliarRemovedCounters = new AuditAuxiliar<List<KeyValuePair<string, string>>>(removedCustomCounters);
            AuditingEngine.SetAuxiliarData("CustomCountersData", auditAuxiliar);
            AuditingEngine.SetAuxiliarData("CustomCountersDataRemovedTags", auditAuxiliarRemovedCounters);

            return ids;
        }

        private Dictionary<int, string> GetCustomCounters()
        {
            var customCounters = new Dictionary<int, string>();
            foreach (CheckboxListItem item in customCountersListBox.Items)
            {
                customCounters.Add((int)item.Tag, item.Text);
            }

            return customCounters;
        }

        private IList<int> GetCheckedPermissionIds()
        {
            List<int> ids = new List<int>();

            foreach (CheckboxListItem listItem in permissionsListBox.CheckedItems)
            {
                ids.Add((int)listItem.Tag);
            }

            return ids;
        }

        private void AddServersChecked(CheckedListBox.CheckedItemCollection checkedItemCollection)
        {
            serversChecked.Clear();
            foreach (CheckboxListItem item in checkedItemCollection)
            {
                int indice;
                int.TryParse(item.Tag.ToString(), out indice);

                if (serversChecked.ContainsKey(indice))
                {
                    serversChecked.Remove(indice);
                }
                serversChecked.Add(indice, item.ToString());
            }

        }

        private void saveChangesWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string tagName = e.Argument as string;
            IList<int> instances = GetCheckedServersIds(tagName);
            IList<int> customCounters = GetCheckedCustomCounterIds(tagName);
            IList<int> permissions = GetCheckedPermissionIds();
            Tag tag = new Tag(tagId, tagName, instances, customCounters, permissions);
            tagId = ApplicationModel.Default.AddOrUpdateTag(tag);
            RepositoryHelper.UpdateCustomCounters(instances as List<int>);
            e.Cancel = saveChangesWorker.CancellationPending;
        }

        private void saveChangesWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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
                    if (existingTag == null)
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while adding the tag.", e.Error);
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while updating the tag.", e.Error);
                    }

                    DialogResult = DialogResult.None;
                    tagNameTextBox.Enabled =
                        availableServersListBox.Enabled =
                        okButton.Enabled =
                        cancelButton.Enabled = true;
                }
            }
        }

        private void initializeWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //
            // Fetch instances
            //

            serverLookupTable.Clear();
            IList<MonitoredSqlServer> monitoredServers =
                    RepositoryHelper.GetMonitoredSqlServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, true);

            foreach (MonitoredSqlServer instance in monitoredServers)
            {
                serverLookupTable.Add(instance.Id, instance.InstanceName);
            }

            //
            // Fetch custom counters
            //

            customCounterLookupTable.Clear();
            MetricDefinitions customCounters = new MetricDefinitions(true, false, true, true);
            customCounters.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, true);
            int[] customCounterIds = customCounters.GetMetricDescriptionKeys();

            foreach (int customCounterId in customCounterIds)
            {
                MetricDescription? definition = customCounters.GetMetricDescription(customCounterId);

                if (definition.HasValue)
                {
                    customCounterLookupTable.Add(customCounterId, definition.Value.Name);
                }
            }

            //
            // Fetch application security configuration
            //

            permissionLookupTable.Clear();
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            Configuration configuration = managementService.GetSecurityConfiguration();
            isApplicationSecurityEnabled = configuration.IsSecurityEnabled;

            if (isApplicationSecurityEnabled)
            {
                foreach (KeyValuePair<int, PermissionDefinition> permission in configuration.Permissions)
                {
                    if (permission.Value != null && !permission.Value.System)
                    {
                        permissionLookupTable.Add(permission.Key, permission.Value);
                    }
                }
            }

            e.Cancel = initializeWorker.CancellationPending;
        }

        private void initializeWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                serversStatusLabel.Hide();
                customCountersStatusLabel.Hide();
                permissionsStatusLabel.Hide();

                if (e.Error == null)
                {
                    //
                    // Initialize instances
                    //

                    if (serverLookupTable.Count != 0)
                    {
                        availableServersListBox.BeginUpdate();

                        foreach (KeyValuePair<int, string> instance in serverLookupTable)
                        {
                            availableServersListBox.Items.Add(new CheckboxListItem(instance.Value, instance.Key));
                        }

                        if (existingTag != null)
                        {
                            foreach (int instanceId in existingTag.Instances)
                            {
                                int index = CheckboxListItemIndexOf(instanceId, availableServersListBox);

                                if (index != -1)
                                {
                                    availableServersListBox.SetItemChecked(index, true);
                                }
                            }
                            AddServersChecked(availableServersListBox.CheckedItems);
                        }

                        availableServersListBox.EndUpdate();
                    }
                    else
                    {
                        serversStatusLabel.ForeColor = SystemColors.GrayText;
                        serversStatusLabel.Text = "No SQL Server instances are currently monitored.";
                        serversStatusLabel.Show();
                    }

                    //
                    // Initialize custom counters
                    //

                    if (customCounterLookupTable.Count != 0)
                    {
                        customCountersListBox.BeginUpdate();

                        foreach (KeyValuePair<int, string> customCounter in customCounterLookupTable)
                        {
                            customCountersListBox.Items.Add(new CheckboxListItem(customCounter.Value, customCounter.Key));
                        }

                        if (existingTag != null)
                        {
                            foreach (int customCounterId in existingTag.CustomCounters)
                            {
                                int index = CheckboxListItemIndexOf(customCounterId, customCountersListBox);

                                if (index != -1)
                                {
                                    customCountersListBox.SetItemChecked(index, true);
                                    currentCounterChecked.Add(customCounterId);
                                }
                            }
                        }

                        customCountersListBox.EndUpdate();
                    }
                    else
                    {
                        customCountersStatusLabel.ForeColor = SystemColors.GrayText;
                        customCountersStatusLabel.Text = "No custom counters have been defined.";
                        customCountersStatusLabel.Show();
                    }

                    //
                    // Initialize permissions
                    //

                    if (isApplicationSecurityEnabled)
                    {
                        if (permissionLookupTable.Count != 0)
                        {
                            foreach (KeyValuePair<int, PermissionDefinition> permission in permissionLookupTable)
                            {
                                StringBuilder listItemName = new StringBuilder(permission.Value.Login);
                                listItemName.Append(" (");
                                listItemName.Append(permission.Value.PermissionType);
                                listItemName.Append(")");
                                if (!permission.Value.Enabled)
                                {
                                    listItemName.Append(" (Disabled)");
                                }
                                permissionsListBox.Items.Add(new CheckboxListItem(listItemName.ToString(), permission.Key));
                            }

                            if (existingTag != null)
                            {
                                foreach (int permissionId in existingTag.Permissions)
                                {
                                    int index = CheckboxListItemIndexOf(permissionId, permissionsListBox);

                                    if (index != -1)
                                    {
                                        permissionsListBox.SetItemChecked(index, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            permissionsStatusLabel.ForeColor = SystemColors.GrayText;
                            permissionsStatusLabel.Text = "No user or group permissions have been defined.";
                            permissionsStatusLabel.Show();
                        }
                    }
                    else
                    {
                        permissionsStatusLabel.ForeColor = SystemColors.GrayText;
                        permissionsStatusLabel.Text = "Application Security is not enabled.";
                        permissionsStatusLabel.Show();
                    }

                    configurationChanged = false;
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while loading form data.", e.Error);
                }
            }
        }

        private static int CheckboxListItemIndexOf(int tag, CheckedListBox listBox)
        {
            int index = -1;

            if (listBox != null)
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    CheckboxListItem item = listBox.Items[i] as CheckboxListItem;

                    if (item != null)
                    {
                        if (item.Tag is int)
                        {
                            if ((int)item.Tag == tag)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
            }

            return index;
        }

        private void TagPropertiesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            initializeWorker.CancelAsync();
        }

        private void availableServersListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            configurationChanged = true;

            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                selectAllServersCheckBox.Checked = false;
                ignoreCheckedChanged = false;
            }
        }

        private void customCountersListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            configurationChanged = true;

            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                selectAllCustomCountersCheckBox.Checked = false;
                ignoreCheckedChanged = false;
            }
        }

        private void permissionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            configurationChanged = true;

            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                selectAllPermissionsCheckBox.Checked = false;
                ignoreCheckedChanged = false;
            }
        }

        private void selectAllServersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;

                for (int i = 0; i < availableServersListBox.Items.Count; i++)
                {
                    availableServersListBox.SetItemChecked(i, selectAllServersCheckBox.Checked);
                }

                ignoreCheckedChanged = false;
            }
        }

        private void selectAllCustomCountersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;

                for (int i = 0; i < customCountersListBox.Items.Count; i++)
                {
                    customCountersListBox.SetItemChecked(i, selectAllCustomCountersCheckBox.Checked);
                }

                ignoreCheckedChanged = false;
            }
        }

        private void selectAllPermissionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;

                for (int i = 0; i < permissionsListBox.Items.Count; i++)
                {
                    permissionsListBox.SetItemChecked(i, selectAllPermissionsCheckBox.Checked);
                }

                ignoreCheckedChanged = false;
            }
        }

        private void TagPropertiesDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ShowHelp();
        }

        private void TagPropertiesDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ShowHelp();
        }

        private void ShowHelp()
        {
            if (existingTag == null)
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.TagsAddTag);
            }
            else
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.TagsTagProperties);
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

    internal class CheckboxListItem
    {
        public string Text;
        public object Tag;

        public CheckboxListItem(string text)
        {
            Text = text;
        }

        public CheckboxListItem(string text, object tag)
            : this(text)
        {
            Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
