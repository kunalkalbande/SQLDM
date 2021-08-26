using System.Drawing;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Controls;
using Infragistics.Win.UltraWinToolbars;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows.Forms;
    using Common;
    using Helpers;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class ApplyCustomCounterToServersDialog : Form
    {
        private const string NewTagToolKey = "New tag...";

        private int selectedMetricID;
        private BackgroundWorkerWithProgressDialog progressWorker;
        private readonly List<int> selectedTags = new List<int>();
        private readonly List<int> selectedServers = new List<int>();
        private bool suppressSelectionChanged = false;
        private MetricType metricType;
        private bool isCheckedTag = false;

        /// <summary>
        /// Needed to determine if the final selection implied an addition of a server
        /// </summary>
        private List<string> initialSelected;

        /// <summary>
        /// Needed to determine if the final selection implied a removal of a server
        /// </summary>
        private List<string> initialAvailable;

        /// <summary>
        /// Needed to determine if the final selection implied an addition of a Tag
        /// </summary>
        private List<string> initialTagsSelected;

        /// <summary>
        /// Needed to determine if the final selection implied a removal of a Tag
        /// </summary>
        private List<string> initialTagsAvailable;

        public ApplyCustomCounterToServersDialog(int initialMetric)
        {
            InitializeComponent();
            selectedMetricID = initialMetric;
            this.AdaptFontSize();
        }

        private void initializeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                if (System.Threading.Thread.CurrentThread.Name == null)
                    System.Threading.Thread.CurrentThread.Name = "LoadCounterMetaData";

                MetricDefinitions metrics = new MetricDefinitions(true, false, true);
                metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                             selectedMetricID);

                // get the description and update the dialog
                MetricDescription? description = metrics.GetMetricDescription(selectedMetricID);
                worker.ReportProgress(1, description);
                // get the counter definition and update the dialog
                CustomCounterDefinition counterDefinition = metrics.GetCounterDefinition(selectedMetricID);
                metricType = counterDefinition.MetricType;
                worker.ReportProgress(2, counterDefinition);

                // return the list of instances monitoring the counter
                e.Result =
                    RepositoryHelper.GetCustomCounterTagsAndServers(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo, selectedMetricID);
            }
        }

        private void initializeWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    {
                        MetricDescription? description = (MetricDescription?)e.UserState;
                        if (description.HasValue)
                        {
                            counterNameLabel.Text = description.Value.Name;
                            counterDescriptionLabel.Text = description.Value.Description;
                        }
                    }
                    break;
                case 2:
                    {
                        CustomCounterDefinition counterDefinition = (CustomCounterDefinition)e.UserState;
                        metricType = counterDefinition.MetricType;
                        counterDefinitionLabel.Text = GetCounterDefinition(counterDefinition);
                    }
                    break;
            }
        }

        private void initializeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Error initializing the link custom counters dialog.  Please resolve the error and try again.",
                                                e.Error);
                DialogResult = DialogResult.Abort;
                Close();
            }
            else if (e.Result is Pair<IList<int>, IList<int>>)
            {
                Pair<IList<int>, IList<int>> result = (Pair<IList<int>, IList<int>>)e.Result;
                selectedTags.AddRange(result.First);
                selectedServers.AddRange(result.Second);
                UpdateTags();
            }
            else
            {
                ApplicationMessageBox.ShowError(this, "An unexpected result type was encountered while initializing the custom counter properties.");
            }
        }

        private void ApplyCustomCounterToServersDialog_Load(object sender, EventArgs e)
        {
            initializeWorker.RunWorkerAsync();
        }

        private string GetCounterDefinition(CustomCounterDefinition counterDefinition)
        {
            StringBuilder buffer = new StringBuilder();
            switch (counterDefinition.MetricType)
            {
                case MetricType.WMI:
                    buffer.Append("OS:");
                    buffer.Append(counterDefinition.ObjectName);
                    buffer.Append("/");
                    buffer.Append(counterDefinition.CounterName);
                    if (!String.IsNullOrEmpty(counterDefinition.InstanceName))
                    {
                        buffer.Append("/");
                        buffer.Append(counterDefinition.InstanceName);
                    }
                    break;
                case MetricType.SQLCounter:
                    buffer.Append("SQL:");
                    buffer.Append(counterDefinition.ObjectName);
                    buffer.Append("/");
                    buffer.Append(counterDefinition.CounterName);
                    if (!String.IsNullOrEmpty(counterDefinition.InstanceName))
                    {
                        buffer.Append("/");
                        buffer.Append(counterDefinition.InstanceName);
                    }
                    break;
                case MetricType.SQLStatement:
                    buffer.Append("BATCH: ");
                    buffer.Append(counterDefinition.SqlStatement);
                    break;
            }

            return buffer.ToString();
        }

        private void okButton_Click(object sender, EventArgs args)
        {
            Cursor = Cursors.WaitCursor;

            progressWorker = new BackgroundWorkerWithProgressDialog(this);
            progressWorker.WorkerSupportsCancellation = false;
            progressWorker.WorkerReportsProgress = false;
            progressWorker.Delay = 3000;
            progressWorker.DoWork += progressWorker_DoWork;
            progressWorker.RunWorkerCompleted += progressWorker_RunWorkerCompleted;
            progressWorker.RunWorkerAsync();

            okButton.Enabled = false;
            DialogResult = DialogResult.None;
        }

        private void progressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            progressWorker = null;
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(
                    this,
                    "Error trying to start monitoring this metric on the selected instances.",
                    e.Error);

                DialogResult = DialogResult.None;
                okButton.Enabled = true;
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void progressWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
            worker.SetStatusText("Linking tags and servers...");



            #region Change Log
            //Setting context data for auditing on ManagementService

            var auditableEntity = new AuditableEntity();
            auditableEntity.Name = String.Format(counterNameLabel.Text);

            string addedServers = "";
            string removedServers = "";
            string addedTags = "";
            string removedTags = "";
            
            this.BuildLinkedAndUnlinkedTagsString(ref addedTags, ref removedTags);
            this.BuildSelectedServersString(ref addedServers, ref removedServers);
            auditableEntity.AddMetadataProperty("LinkedTags", addedTags);
            auditableEntity.AddMetadataProperty("UnLinkedTags", removedTags);
            auditableEntity.AddMetadataProperty("LinkedServers", addedServers);
            auditableEntity.AddMetadataProperty("UnLinkedServers", removedServers);

            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("CustomCounterAddedToTag", auditableEntity);

            #endregion Change Log

            ApplicationModel.Default.AddCustomCounterToMonitoredServers(selectedMetricID, selectedTags, selectedServers);
        }

        private void ApplyCustomCounterToServersDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (progressWorker != null)
                    e.Cancel = true;
            }
        }

        private void ApplyCustomCounterToServersDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CustomCountersLink);
        }

        private void ApplyCustomCounterToServersDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CustomCountersLink);
        }
        
        /// <summary>
        /// Fills addedTags and removedTags from processed data from the toolbarsManager.Tools["tagsPopupMenu"]
        /// </summary>
        /// <param name="addedTags"></param>
        /// <param name="removedTags"></param>
        private void BuildLinkedAndUnlinkedTagsString(ref string addedTags, ref string removedTags)
        {
            var addedTagsBuilder = new StringBuilder();
            var removedTagsBuilder = new StringBuilder();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Checked && !initialTagsSelected.Contains(tag.CaptionResolved))
                {
                    if (addedTagsBuilder.Length > 0)
                        addedTagsBuilder.Append(",");

                    addedTagsBuilder.AppendFormat("{0}", tag.CaptionResolved);
                }
                else if (!tag.Checked && !initialTagsAvailable.Contains(tag.CaptionResolved))
                {

                    if (removedTagsBuilder.Length > 0)
                        removedTagsBuilder.Append(",");

                    removedTagsBuilder.AppendFormat("{0}", tag.CaptionResolved);
                }
            }

            addedTags = addedTagsBuilder.ToString();
            removedTags = removedTagsBuilder.ToString();
        }

        /// <summary>
        /// Fills addServers and removedServers with processed data from instanceSelectionList.Selected 
        /// and instanceSelectionList.Available.
        /// </summary>
        /// <param name="addedServers"></param>
        /// <param name="removedServers"></param>
        private void BuildSelectedServersString(ref string addedServers, ref string removedServers)
        {
            var addedServersBuilder = new StringBuilder();
            var removedServersBuilder = new StringBuilder();
            
            foreach (object monitoredSqlServer in instanceSelectionList.Selected.Items)
            {
                string instanceName = string.Empty;

                if (monitoredSqlServer.GetType() == typeof (MonitoredSqlServer))
                    instanceName = ((MonitoredSqlServer) monitoredSqlServer).InstanceName;

                //The following lines refer to servers added via Tag selection:  5 - [b]
                //And may be required later on
                //else if (monitoredSqlServer.GetType() == typeof(ExtendedListItem))
                //    instanceName = ((ExtendedListItem) monitoredSqlServer).Text;

                if (!string.IsNullOrEmpty(instanceName) && !initialSelected.Contains(instanceName))
                {
                    if (addedServersBuilder.Length > 0)
                        addedServersBuilder.Append(",");

                    addedServersBuilder.AppendFormat("{0}", instanceName);
                }
            }
            
            foreach (MonitoredSqlServer monitoredSqlServer in instanceSelectionList.Available.Items)
            {
                if (!initialAvailable.Contains(monitoredSqlServer.InstanceName))
                {
                    if (removedServersBuilder.Length > 0)
                        removedServersBuilder.Append(",");

                    removedServersBuilder.AppendFormat("{0}", monitoredSqlServer.InstanceName);
                }
            }

            addedServers = addedServersBuilder.ToString();
            removedServers = removedServersBuilder.ToString();
        }

        private string BuildSelectedTagsString()
        {
            StringBuilder selectedTagsString = new StringBuilder();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Key != NewTagToolKey && tag.Checked)
                {
                    if (selectedTagsString.Length > 0)
                    {
                        selectedTagsString.Append(", ");
                    }

                    selectedTagsString.Append(tag.SharedProps.Caption);
                }
            }

            return selectedTagsString.ToString();
        }

        private void UpdateSelectedTags()
        {
            string selectedTagsString = BuildSelectedTagsString();
            tagsDropDownButton.Text = selectedTagsString.Length != 0
                                          ? selectedTagsString
                                          : "< Click here to choose tags >";
            UpdateSelectedServers();
        }

        private void UpdateSelectedServers()
        {
            try
            {
                suppressSelectionChanged = true;
                instanceSelectionList.Selected.BeginUpdate();
                instanceSelectionList.Selected.Items.Clear();
                instanceSelectionList.Available.BeginUpdate();
                instanceSelectionList.Available.Items.Clear();
                bool initialServersAlreadyDefined = true;

                if (initialSelected == null)
                {
                    initialSelected = new List<string>();
                    initialAvailable = new List<string>();

                    initialServersAlreadyDefined = false;
                }

                foreach (int tagId in selectedTags)
                {
                    if (ApplicationModel.Default.Tags.Contains(tagId))
                    {
                        Tag tag = ApplicationModel.Default.Tags[tagId];

                        foreach (int instanceId in tag.Instances)
                        {
                            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                            {
                                MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
                                if (metricType == MetricType.VMCounter && !instance.IsVirtualized)
                                {
                                    continue;
                                }
                                // DM 10.3 (Varun Chopra) SQLDM-28744 - CustomCounters for Non Linux Instances
                                if (metricType == MetricType.WMI && instance.CloudProviderId == SQLdm.Common.Constants.LinuxId)
                                {
                                    continue;
                                }
                                StringBuilder itemName = new StringBuilder(instance.InstanceName);
                                itemName.Append(" - [");
                                itemName.Append(tag.Name);
                                itemName.Append("]");

                                ExtendedListItem listItem = new ExtendedListItem(itemName.ToString());
                                listItem.Tag = new Pair<int, int>(instanceId, tagId);
                                listItem.TextColor = SystemColors.GrayText;
                                listItem.CanRemove = false;
                                instanceSelectionList.Selected.Items.Add(listItem);
                            }
                        }
                    }
                }

                foreach (MonitoredSqlServer instance in ApplicationModel.Default.ActiveInstances)
                {
                    MetricDefinitions metrics = new MetricDefinitions(true, false, true);
                    metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                             selectedMetricID);
                    CustomCounterDefinition counterDefinition = metrics.GetCounterDefinition(selectedMetricID);
                    // Allow mapping of Azure Custom counter to azure server
                    if (metricType == MetricType.AzureCounter && instance.CloudProviderId != Common.Constants.MicrosoftAzureId)
                    {
                        continue;
                    }
                    if (metricType == MetricType.VMCounter && !instance.IsVirtualized)
                    {
                        continue;
                    }
                    // DM 10.3 (Varun Chopra) SQLDM-28744 - CustomCounters for Non Linux Instances
                    if (metricType == MetricType.WMI && instance.CloudProviderId == SQLdm.Common.Constants.LinuxId)
                    {
                        continue;
                    }
                    if (selectedServers.Contains(instance.Id))
                    {
                        instanceSelectionList.Selected.Items.Add(instance);

                        if (!initialServersAlreadyDefined && !initialSelected.Contains(instance.InstanceName))
                            initialSelected.Add(instance.InstanceName);
                    }
                    else
                    {
                        if ((counterDefinition.ServerType == null || counterDefinition.ServerType.ToUpper() == "UNKNOWN")
                            || (instance.VirtualizationConfiguration != null && counterDefinition.ServerType.Equals(instance.VirtualizationConfiguration.VCServerType)))
                        {
                            instanceSelectionList.Available.Items.Add(instance);
                            if (!initialServersAlreadyDefined)
                                initialAvailable.Add(instance.InstanceName);
                        }
                        
                    }
                }
            }
            finally
            {
                suppressSelectionChanged = false;
                instanceSelectionList.Selected.EndUpdate();
                instanceSelectionList.Available.EndUpdate();
            }
        }

        private void UpdateTags()
        {
            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Clear();
            SortedDictionary<string, Tag> sortedTags = new SortedDictionary<string, Tag>();

            initialTagsSelected = new List<string>();
            initialTagsAvailable = new List<string>();

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

            ToolBase newTagTool;
            if (!toolbarsManager.Tools.Exists(NewTagToolKey))
            {
                newTagTool = new StateButtonTool(NewTagToolKey);
                newTagTool.SharedProps.Caption = NewTagToolKey;
                toolbarsManager.Tools.Add(newTagTool);
            }
            else
            {
                newTagTool = toolbarsManager.Tools[NewTagToolKey];
            }

            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools.Add(newTagTool);
            ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools[NewTagToolKey].InstanceProps.IsFirstInGroup = true;

            // Save an initial state
            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarsManager.Tools["tagsPopupMenu"])).Tools)
            {
                if(tag.Checked)
                {
                    initialTagsSelected.Add(tag.CaptionResolved);
                }
                else
                {
                    initialTagsAvailable.Add(tag.CaptionResolved);
                }
            }

            UpdateSelectedTags();
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (isCheckedTag)
            {
                isCheckedTag = false;
            }
            else if (e.Tool.OwningMenu == toolbarsManager.Tools["tagsPopupMenu"])
            {
                try
                {
                    if (e.Tool.Key == NewTagToolKey)
                    {
                        UncheckedNewTagButton();

                        AddTagDialog dialog = new AddTagDialog();

                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            selectedTags.Add(dialog.TagId);
                            UpdateTags();
                        }
                    }
                    else
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
                    }

                    UpdateSelectedTags();
                }
                catch { }
            }
        }

        /// <summary>
        /// Verifies that last item with text 'New Tag...' never is selected. It is in Tags' list.
        /// </summary>
        private void UncheckedNewTagButton()
        {
            PopupMenuTool popupMenuTool =
                (toolbarsManager.Tools["tagsPopupMenu"]) as PopupMenuTool;

            if (popupMenuTool != null)
            {
                StateButtonTool tag =
                    popupMenuTool.Tools[popupMenuTool.Tools.Count - 1] as StateButtonTool;

                if (tag != null && tag.Checked && tag.CaptionResolved == NewTagToolKey)
                {
                    isCheckedTag = true;
                    tag.Checked = false;
                }
            }
        }

        private void instanceSelectionList_SelectionChanged(object sender, EventArgs e)
        {
            if (!suppressSelectionChanged)
            {
                selectedServers.Clear();

                foreach (object obj in instanceSelectionList.Selected.Items)
                {
                    if (obj is MonitoredSqlServer)
                    {
                        selectedServers.Add(((MonitoredSqlServer)obj).Id);
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
