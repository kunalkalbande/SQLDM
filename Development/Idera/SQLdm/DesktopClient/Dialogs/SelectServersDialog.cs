//------------------------------------------------------------------------------
// <copyright file="SelectServersDialog.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SelectServersDialog : BaseDialog
    {
        private readonly ICollection<int> excludedServers;
        private readonly List<int> selectedTags = new List<int>();
        private readonly List<int> selectedServers = new List<int>();
        private bool ignoreCheckedChanged = false;
        private bool showTemplates = false;
        private ArrayList alertListConfiguration = new ArrayList();


        /// <summary>
        /// Template to concatenate the names of the servers.
        /// </summary>
        public SelectServersDialog(string description)
            : this(description, null, null)
        {
            this.DialogHeader = "Select Servers or Tags";
        }

        public SelectServersDialog(string description, ICollection<int> excludedServers,
            ArrayList alertConfiguration)
        {
            this.DialogHeader = "Select Servers or Tags";
            this.excludedServers = excludedServers;
            InitializeComponent();
            descriptionLabel.Text = description;

            if (alertConfiguration != null)
            {
                showTemplates = true;
                this.Text = "Select Modification Targets";
                this.alertListConfiguration.AddRange(alertConfiguration);
            }

            UpdateSelectedTags();
            InitializeAvailableServersList();
            this.AdaptFontSize();
        }

        public SelectServersDialog(string description, ICollection<int> excludedServers, bool hideTags)
            : this(description, excludedServers, null)
        {
            this.DialogHeader = "Select Servers or Tags";
            if (hideTags)
            {
                selectAllServersCheckBox.Enabled = serversListBox.Enabled = true;
                selectServerRadioButton.Checked = true;
                selectServerRadioButton.Visible = 
                    selectTagsRadioButton.Visible = 
                    tagsDropDownButton.Visible = false;

                int controlMoveUp = selectAllServersCheckBox.Top - selectTagsRadioButton.Top;
                int controlMoveLeft = 20;

                selectAllServersCheckBox.Top -= controlMoveUp;
                selectAllServersCheckBox.Left -= controlMoveLeft;
                serversListBox.Top -= controlMoveUp;
                serversListBox.Left -= controlMoveLeft;
                serversListBox.Height += controlMoveUp;
                serversListBox.Width += controlMoveLeft;
            }
        }

        public IList<int> SelectedServers
        {
            get { return selectedServers; }
        }

        /// <summary>
        /// Return all selected servers in a String array.
        /// </summary>
        /// <returns>All selected servers in a String array.</returns>
        public List<String> GetSelectedServerNames()
        {
            List<String> serverNames = new List<string>();

            selectedServers.ForEach(serverId => serverNames.Add(ApplicationModel.Default.ActiveInstances[serverId].InstanceName));

            return serverNames;
        }

        private void InitializeAvailableServersList()
        {
            serversListBox.BeginUpdate();

            foreach (MonitoredSqlServer instance in ApplicationModel.Default.ActiveInstances)
            {
                if (excludedServers == null || !excludedServers.Contains(instance.Id))
                {
                    serversListBox.Items.Add(new CheckboxListItem(instance.InstanceName, instance.Id));
                }
            }

            serversListBox.EndUpdate();

            if (serversListBox.Items.Count > 0)
            {
                serversStatusLabel.Hide();
            }
            else
            {
                serversStatusLabel.ForeColor = SystemColors.GrayText;
                serversStatusLabel.Text = "No SQL Server instances are currently monitored.";
                serversStatusLabel.Show();
            }
        }

        private void tagsDropDownButton_DroppingDown(object sender, CancelEventArgs e)
        {
            ((PopupMenuTool) (toolbarsManager.Tools["tagsPopupMenu"])).Tools.Clear();
            SortedDictionary<string, Tag> sortedTags = new SortedDictionary<string, Tag>();

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                sortedTags.Add(tag.Name, tag);
            }

            foreach (Tag tag in sortedTags.Values)
            {
                StateButtonTool tagTool;
                string tagIdString = tag.Id.ToString();

                if (toolbarsManager.Tools.Exists(tag.Id.ToString()))
                {
                    tagTool = (StateButtonTool)toolbarsManager.Tools[tagIdString];
                }
                else
                {
                    tagTool = new StateButtonTool(tag.Id.ToString());
                    tagTool.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
                    toolbarsManager.Tools.Add(tagTool);
                }

                tagTool.SharedProps.Caption = tag.Name;
                tagTool.Checked = selectedTags.Contains(tag.Id);
                ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Add(tagTool);
            }

            UpdateSelectedTags();
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.OwningMenu == toolbarsManager.Tools["tagsPopupMenu"])
            {
                try
                {
                    int tagId = Convert.ToInt32(e.Tool.Key);

                    if (!selectedTags.Contains(tagId))
                    {
                        selectedTags.Add(tagId);
                    }
                    else
                    {
                        selectedTags.Remove(tagId);
                    }

                    UpdateSelectedTags();
                }
                catch{}
            }
        }

        private void UpdateSelectedTags()
        {
            StringBuilder newText = new StringBuilder();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Checked)
                {
                    if (newText.Length > 0)
                    {
                        newText.Append(", ");
                    }

                    newText.Append(tag.SharedProps.Caption);
                }
            }

            if (newText.Length != 0)
            {
                tagsDropDownButton.Text = newText.ToString();
            }
            else
            {
                tagsDropDownButton.Text = "< Click here to choose tags >";
            }

            UpdateOkButtonState();
        }

        private void selectServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            tagsDropDownButton.Enabled = selectTagsRadioButton.Checked;
            selectAllServersCheckBox.Enabled = 
                serversListBox.Enabled = selectServerRadioButton.Checked;
            UpdateOkButtonState();
        }

        private void BuildSelectedServers()
        {
            selectedServers.Clear();

            if (selectTagsRadioButton.Checked && selectedTags.Count > 0)
            {
                foreach (int tagId in selectedTags)
                {
                    if (ApplicationModel.Default.Tags.Contains(tagId))
                    {
                        Tag tag = ApplicationModel.Default.Tags[tagId];

                        foreach (int instanceId in tag.Instances)
                        {
                            if (!selectedServers.Contains(instanceId) && 
                                !excludedServers.Contains(instanceId))
                            {
                                selectedServers.Add(instanceId);
                            }
                        }
                    }
                }
            }
            else if (selectServerRadioButton.Checked && serversListBox.CheckedItems.Count > 0)
            {
                foreach (CheckboxListItem selectedServer in serversListBox.CheckedItems)
                {
                    selectedServers.Add((int)selectedServer.Tag);
                }
            }

            UpdateSelectedTemplate();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            BuildSelectedServers();
        }

        private void tagsDropDownButton_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButtonState();
        }

        private void serversListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOkButtonState();

            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                selectAllServersCheckBox.Checked = false;
                ignoreCheckedChanged = false;
            }
        }

        /// <summary>
        /// To enable or disable the OK button
        /// </summary>
        private void UpdateOkButtonState()
        {
            okButton.Enabled = (selectTagsRadioButton.Checked
                ? selectedTags.Count > 0
                : serversListBox.CheckedItems.Count > 0) || selectTemplateCheckBox.Checked;
        }

        private void selectAllServersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;

                for (int i = 0; i < serversListBox.Items.Count; i++)
                {
                    serversListBox.SetItemChecked(i, selectAllServersCheckBox.Checked);
                }

                ignoreCheckedChanged = false;
            }

            UpdateOkButtonState();
        }

		/// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// On event load for this windows. Load comboBox with all all Alerts templates list and update drop down button
        /// </summary>
        private void SelectServersDialog_Load(object sender, EventArgs e)
        {
            var alertTemplateList =
                RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            // load comboBox with all all Alerts templates list
            foreach (AlertTemplate template in alertTemplateList)
            {
                templateDropDownButton.Items.Add(template.TemplateID, template.Name);
            }

            templateDropDownButton.SelectedIndex = 0;
            ShowOrHideTemplateDropDownButton();
        }

        /// <summary>
        /// On check/uncheck event.
        /// </summary>
        private void selectTemplateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            templateDropDownButton.Enabled = selectTemplateCheckBox.Checked;
            UpdateOkButtonState();
        }

        /// <summary>
        /// To clon the template alert configuration with other configurations
        /// </summary>
        private void UpdateSelectedTemplate()
        {
            if (selectTemplateCheckBox.Checked)
            {
                int templateID = (int)templateDropDownButton.SelectedItem.DataValue;
                AlertConfiguration templateConfiguration =
                    RepositoryHelper.GetDefaultAlertConfiguration(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo, templateID);

                foreach (var item in alertListConfiguration)
                {
                    MetricThresholdEntry entry = item as MetricThresholdEntry;

                    //need change the state and the TemplateID
                    if (entry != null)
                    {
                        entry.State = ThresholdState.Changed;
                        entry.MonitoredServerID = templateID;
                        templateConfiguration.ChangeItems.Add(entry);
                    }
                }

                var managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                managementService.ChangeAlertTemplateConfiguration(templateConfiguration);
            }
        }

        /// <summary>
        /// To parameterizing the SelectServerDialog window so that we would be hiding controls if we need it
        /// </summary>
        private void ShowOrHideTemplateDropDownButton()
        {
            if (!showTemplates)
            {
                //Select template controls
                this.selectTemplateCheckBox.Enabled = this.selectTemplateCheckBox.Visible = false;
                this.templateDropDownButton.Enabled = this.templateDropDownButton.Visible = false;

                //Select tags controls
                this.selectTagsRadioButton.Location = new System.Drawing.Point(20, 19);
                this.tagsDropDownButton.Location = new System.Drawing.Point(38, 43);
               
                //Select servers controls
                this.selectServerRadioButton.Location = new System.Drawing.Point(20, 78);
                this.selectAllServersCheckBox.Location = new System.Drawing.Point(41, 98);
                this.serversListBox.Location = new System.Drawing.Point(38, 116);
                this.serversListBox.Size = new System.Drawing.Size(383, 329);
            }
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2, 
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution(showTemplates);
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2, 
        private void ScaleControlsAsPerResolution(bool showTemplates)
        {
            if (!showTemplates)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.serversListBox, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.65F, 1.5F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.selectServerRadioButton, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 1.5F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.selectTagsRadioButton, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 1.5F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tagsDropDownButton, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 1.5F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.selectAllServersCheckBox, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 1.5F));
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    this.tagsDropDownButton.Height = this.tagsDropDownButton.Height - 20;
                    this.serversListBox.Location = new Point(this.serversListBox.Location.X - 20, this.serversListBox.Location.Y);
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    this.tagsDropDownButton.Height = this.tagsDropDownButton.Height - 30;
                    this.serversListBox.Location = new Point(this.serversListBox.Location.X - 20, this.serversListBox.Location.Y + 20);
                    this.serversListBox.Width += 60;
                    this.selectAllServersCheckBox.Location = new Point(this.selectAllServersCheckBox.Location.X, this.selectAllServersCheckBox.Location.Y + 10);
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    this.tagsDropDownButton.Height = this.tagsDropDownButton.Height - 30;
                    this.tagsDropDownButton.Width += 40;
                    this.serversListBox.Location = new Point(this.serversListBox.Location.X - 20, this.serversListBox.Location.Y + 20);
                    this.serversListBox.Width += 240;
                    this.selectAllServersCheckBox.Location = new Point(this.selectAllServersCheckBox.Location.X, this.selectAllServersCheckBox.Location.Y + 10);
                }
            }
        }
    }
}
