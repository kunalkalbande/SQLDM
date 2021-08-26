using System.Linq;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using Common;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinToolTip;
    using Objects;
    using Wintellect.PowerCollections;

    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class NotificationRuleDialog : Form, IAuditable
    {
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationRuleDialog");
        private bool loading;
        private NotificationRule rule;

        private MetricDefinitions metricDefinitions;
        private List<NotificationProviderInfo> providerTypes;
        private IManagementService managementService;
        private Dictionary<int, Pair<string, int>> tagMap;
        private const String RegExToRemoveHtmlCode = @"(<style\b[^>]*>)[^<>]*(<\/style>)|<[^>]*>";
        private const String RegExToRemoveTdCode = @"<td[^\>]*\>";

        public NotificationRuleDialog(IManagementService managementService, MetricDefinitions metricDefinitions)
        {
            InitializeComponent();
            this.managementService = managementService;
            this.metricDefinitions = metricDefinitions;

            bool haveSNMPProvider = false;
            bool haveSMTPProvider = false;

            foreach (NotificationProviderInfo npi in managementService.GetNotificationProviders())
            {
                if (npi is SnmpNotificationProviderInfo)
                    haveSNMPProvider = true;
                if (npi is SmtpNotificationProviderInfo)
                    haveSMTPProvider = true;
                if (npi is TaskNotificationProviderInfo && !ApplicationModel.Default.IsTasksViewEnabled)
                    continue;
                NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                ndi.ProviderID = npi.Id;
                ndi.Enabled = false;
                NotificationRule.Destinations.Add(ndi);
            }
            if (!haveSNMPProvider)
            {
                SnmpNotificationProviderInfo npi = new SnmpNotificationProviderInfo(true);
                npi.Name = "Network Management (SNMP) Trap Message Provider";
                npi = (SnmpNotificationProviderInfo)managementService.AddNotificationProvider(npi);
                NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                ndi.ProviderID = npi.Id;
                ndi.Enabled = false;
                NotificationRule.Destinations.Add(ndi);
            }
            if (!haveSMTPProvider)
            {
                SmtpNotificationProviderInfo npi = new SmtpNotificationProviderInfo(true);
                npi.Name = "Email (SMTP) Provider";
                npi = (SmtpNotificationProviderInfo)managementService.AddNotificationProvider(npi);
                NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                ndi.ProviderID = npi.Id;
                ndi.Enabled = false;
                NotificationRule.Destinations.Add(ndi);
            }

            // Autoscale fontsize.
            AdaptFontSize();
        }

        public NotificationRuleDialog(IManagementService managementService, MetricDefinitions metricDefinitions, List<NotificationProviderInfo> providers, bool forceDestination)
        {
            InitializeComponent();
            this.managementService = managementService;
            this.metricDefinitions = metricDefinitions;
            providerTypes = providers;
            if (forceDestination)
            {
                addActionButton.Enabled = false;

                foreach (NotificationProviderInfo npi in ProviderTypes)
                {
                    NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                    ndi.ProviderID = npi.Id;
                    NotificationRule.Destinations.Add(ndi);
                }
            }

            // Autoscale fontsize.
            AdaptFontSize();
        }

        public NotificationRule NotificationRule
        {
            get
            {
                if (rule == null)
                    rule = new NotificationRule();
                return rule;
            }
            set
            {
                rule = value;

                FixupDestinationProvider();
            }
        }

        private void FixupDestinationProvider()
        {
            NotificationRuleLabelGenerator.FixupDestinationProvider(managementService, rule);
        }

        private List<NotificationProviderInfo> ProviderTypes
        {
            get
            {
                if (providerTypes == null)
                    providerTypes = new List<NotificationProviderInfo>();
                return providerTypes;
            }
        }

        private void ProviderSelectionChanged(NotificationProviderInfo providerType, bool selected)
        {
            ProviderSelectionChanged(providerType, selected, false);
        }

        private void ProviderSelectionChanged(NotificationProviderInfo providerType, bool selected, bool createDestinations)
        {
            if (selected && !ProviderTypes.Contains(providerType))
            {
                ProviderTypes.Add(providerType);

                if (createDestinations)
                {
                    // if destination is event log - add a destination and configure it
                    if (providerType.DestinationType == typeof(EventLogDestination))
                    {
                        NotificationProviderInfo provider = GetFirstAvailableProvider(providerType.DestinationType);
                        if (provider != null)
                        {
                            EventLogDestination eld = new EventLogDestination();
                            eld.Provider = provider;
                            rule.Destinations.Add(eld);
                        }
                    }
                    // if destination is task - add a destination and configure it
                    if (providerType.DestinationType == typeof(TaskDestination))
                    {
                        NotificationProviderInfo provider = GetFirstAvailableProvider(providerType.DestinationType);
                        if (provider != null)
                        {
                            TaskDestination td = new TaskDestination();
                            td.Provider = provider;
                            td.Subject = "$(AlertSummary)";
                            td.Body = "$(AlertText)\r\n\r\n$(Metric): $(Description)";

                            rule.Destinations.Add(td);
                        }
                    }
                }
            }
            if (!selected && ProviderTypes.Contains(providerType))
            {
                ProviderTypes.Remove(providerType);
                foreach (NotificationDestinationInfo destination in Collections.ToArray(rule.Destinations))
                {
                    if (destination.Provider.ProviderType == providerType.ProviderType)
                    {
                        rule.Destinations.Remove(destination);
                    }
                }
            }
        }

        private void NotificationRuleDialog_Load(object sender, EventArgs args)
        {
            //            try
            //            {
            //                Helpers.NativeMethods.SetZoneMapping(
            //                    Idera.SQLdm.DesktopClient.Helpers.NativeMethods.URLZONE.URLZONE_TRUSTED,
            //                    "about:security_SQLdmDesktopClient.exe",
            //                    Idera.SQLdm.DesktopClient.Helpers.NativeMethods.SZM_CREATE);
            //            } catch (Exception)
            //            {
            //                /* depending on why it failed - the checkboxes/buttons in webbrowser controls will not function unless the above url is added to the trusted zone. */
            //            }
            //
            //            providersListBrowser.ObjectForScripting = this;

            try
            {
                loading = true;
                LoadTags();
                AddConditions();
            }
            finally
            {
                loading = false;
            }
            txtDescription.Text = rule.Description;
            RebuildLinks();
        }

        private void LoadTags()
        {
            tagMap = new Dictionary<int, Pair<string, int>>();
            /// SQLdm 10.1(Srishti Purohit)
            /// removing global tags from list
            foreach (Tag tag in ApplicationModel.Default.LocalTags)
            {
                tagMap.Add(tag.Id, new Pair<string, int>(tag.Name, tag.Id));
            }
        }

        private void AddConditions()
        {
            conditionListBox.Items.Clear();

            ListViewItem item = AddListItem(conditionListBox, "ServerName", "Where the SQL Server Instance is in <a href=\"ServerNameDialog\">specified list</a>");
            item.Checked = (rule.ServerNameComparison != null && rule.ServerNameComparison.Enabled);
            item = AddListItem(conditionListBox, "ServerTag", "Where SQL Server Instance has a tag in <a href=\"ServerTagDialog\">specified list</a>");
            item.Checked = (rule.ServerTagComparison != null && rule.ServerTagComparison.Enabled);
            // Checking the and/or checkbox based on IsMetricsWithAndChecked value
            item = AddListItem(conditionListBox, "Metric", "Where metric is in <a href=\"MetricSelectionDialog\">specified list</a>");
            // For the new notification rule this check box will be unchecked by default and for the exisiting rules it will check the IsMetricsWithAndChecked
            if (rule.Id == Guid.Empty)
            {
                item.Checked = false;
            }
            else
            {
                item.Checked = (rule.IsMetricsWithAndChecked == false);
            }
            // Adding new checkbox wtih And condition here
            item = AddListItem(conditionListBox, "MetricWithAnd", "Where all metrics are in <a href=\"MetricSelectionDialog\">specified list</a>");
            // For the new notification rule this check box will be unchecked by default and for the exisiting rules it will check the IsMetricsWithAndChecked
            if (rule.Id == Guid.Empty)
            {
                item.Checked = false;
            }
            else
            {
                item.Checked = (rule.IsMetricsWithAndChecked == true);
            }
            //            item = AddListItem(conditionListBox, "StateValue", "where severity is <a href=\"SelectSeverityDialog\">a specific value</a>");
            //            item.Checked = (rule.StateComparison != null && rule.StateComparison.Enabled);
            item = AddListItem(conditionListBox, "StateChange", "Where metric severity has changed");
            item.Checked = (rule.StateChangeComparison != null && rule.StateChangeComparison.Enabled && !string.IsNullOrWhiteSpace(rule.MetricSeverityValue));
            item = AddListItem(conditionListBox, "SnapshotTime", "Where refresh occurred during a <a href=\"DateRangeDialog\">specific time frame</a>");
            item.Checked = (rule.SnapshotTimeComparison != null && rule.SnapshotTimeComparison.Enabled);

            item = AddListItem(conditionListBox, "AlertRankValue", "Where Alert Rank <a href=\"AlertRankValuesDialog\">value</a>");
            if (rule.IsRankValueChecked == true)
            {
                item.Checked = true;
            }
            else
            {
                item.Checked = false;
            }

            item = AddListItem(conditionListBox, "MetricSeverityValue", "Where metric severity is unchanged for a specific <a href=\"MetricSeverityDialog\">time period</a>");
            if (rule.IsMetricSeverityChecked == true)
            {
                item.Checked = true;
            }
            else
            {
                item.Checked = false;
            }

        }

        private ListViewItem AddListItem(ListView view, string elementName, string text)
        {
            NodeType ntype = new NodeType();
            ntype.ElementName = elementName;
            ntype.Text = text;

            ListViewItem item = new ListViewItem(text);
            item.Tag = ntype;
            item.Name = ntype.ElementName;

            view.Items.Add(item);

            return item;
        }



        private void RebuildLinks()
        {
            providersListBrowser.DocumentText = NotificationRuleLabelGenerator.GetDestinationCheckList(managementService, rule);

            rulePreview.DocumentText =
                NotificationRuleLabelGenerator.RebuildLinks(managementService, rule, ProviderTypes, metricDefinitions, false, tagMap);
            UpdateControls();
        }

        private List<NotificationDestinationInfo> GetDestinationsForProvider(NotificationProviderInfo provider)
        {
            List<NotificationDestinationInfo> result = new List<NotificationDestinationInfo>();
            foreach (NotificationDestinationInfo destination in rule.Destinations)
            {

                if (destination.Provider.GetType() == provider.GetType())
                    result.Add(destination);
            }

            return result;
        }

#pragma warning disable 0649
        class NodeType
        {
            internal object Tag;
            internal string Text;
            internal string ElementName;

            public string GetRuleText()
            {
                return Text;
            }

            public override string ToString()
            {
                return GetRuleText();
            }
        }
#pragma warning restore 0649

        private void conditionListBox_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (loading)
                return;

            ListViewItem item = e.Item;
            NodeType ntype = item.Tag as NodeType;
            if (ntype == null)
                return;

            bool check = e.Item.Checked;

            if (ntype.ElementName == "ServerName")
            {
                rule.ServerNameComparison.Enabled = check;
            }
            if (ntype.ElementName == "ServerTag")
            {
                rule.ServerTagComparison.Enabled = check;
            }
            if (ntype.ElementName == "Metric")
            {
                // if the Or checkbox is checked then set the IsMetricsWithAndChecked to false and uncheck the And check box
                //SQLdm 10.0.2 Barkha Khatri this line should be outside if 
                if (check)
                {
                    rule.IsMetricsWithAndChecked = false;
                    foreach (ListViewItem itm in conditionListBox.Items)
                    {
                        if (itm.Name == "MetricWithAnd")
                        {
                            itm.Checked = false;
                            //SQLDM-26122 - Code change for rule description when "Where all metrics.."is selected
                            rule.Metrics = check ? new List<Metric>() : null;
                        }
                    }
                }
                rule.Metrics = check ? new List<Metric>() : null;
            }
            if (ntype.ElementName == "AlertRankValue")
            {
                if (check)
                {
                    rule.IsRankValueChecked = true;
                }
                else
                {
                    rule.IsRankValueChecked = false;
                }
            }

            if (ntype.ElementName == "MetricSeverityValue") // metric sheverity check conditon
            {
                rule.IsMetricSeverityChecked = check;
                rule.MetricSeverityValue = string.Empty;
                if (check)
                {
                    foreach (ListViewItem itm in conditionListBox.Items)
                    {
                        if (itm.Name == "SnapshotTime" || itm.Name == "StateChange")
                        {
                            itm.Checked = false;
                            // rule.Metrics = check ? new List<Metric>() : null;
                        }
                    }

                    // rule.SnapshotTimeComparison.Enabled = true;
                    rule.StateChangeComparison.Enabled = true;
                    rule.StateComparison.Enabled = true;
                    rule.MetricSeverityValue = string.IsNullOrWhiteSpace(rule.MetricSeverityValue) ? "4" : rule.MetricSeverityValue;
                    NotificationRuleLabelGenerator.Rvalue = 1;
                }
                else
                {
                    rule.MetricSeverityValue = string.Empty;
                }
            }
            // Added this code to handle the new checkbox with And condition here
            if (ntype.ElementName == "MetricWithAnd")
            {
                // if the And checkbox is checked then set the IsMetricsWithAndChecked to true and uncheck the Or check box
                //SQLdm 10.0.2 Barkha Khatri this line should be outside if 
                if (check)
                {
                    rule.IsMetricsWithAndChecked = true;
                    //rule.Metrics = check ? new List<Metric>() : null;
                    foreach (ListViewItem itm in conditionListBox.Items)
                    {
                        if (itm.Name == "Metric")
                        {
                            itm.Checked = false;
                            //SQLDM-26122 - Code change for rule description when "Where all metrics.."is selected
                            rule.Metrics = check ? new List<Metric>() : null;
                        }
                    }
                }
                rule.Metrics = check ? new List<Metric>() : null;
            }
            if (ntype.ElementName == "StateValue")
            {
                rule.StateComparison.Enabled = check;
            }
            if (ntype.ElementName == "StateChange") //where metric severity has changed
            {
                rule.StateChangeComparison.Enabled = check;
                foreach (ListViewItem itm in conditionListBox.Items)
                {
                    if (rule.StateChangeComparison.Enabled == true && itm.Name == "MetricSeverityValue")
                    {
                        rule.IsMetricSeverityChecked = false;
                        itm.Checked = false;
                        //rule.Metrics = check ? new List<Metric>() : null;
                    }
                }
            }
            if (ntype.ElementName == "SnapshotTime")
            {
                rule.SnapshotTimeComparison.Enabled = check;
                foreach (ListViewItem itm in conditionListBox.Items)
                {
                    if (rule.SnapshotTimeComparison.Enabled == true /*&& rule.StateChangeComparison.Enabled == true*/ && itm.Name == "MetricSeverityValue")
                    {
                        rule.IsMetricSeverityChecked = false;
                        itm.Checked = false;
                        //  rule.Metrics = check ? new List<Metric>() : null;
                    }
                }
            }

            int checkedCount = conditionListBox.Items.Cast<ListViewItem>()
                .Where(clbItem => clbItem.Name == "StateChange" || clbItem.Name == "MetricSeverityValue")
                .Where(clbItem => clbItem.Checked).Count();

            rule.IsMetricSeverityAndUnchecked = checkedCount != 0;

            if (checkedCount == 0)
            {
                rule.MetricSeverityValue = string.IsNullOrWhiteSpace(rule.MetricSeverityValue) ? "4" : rule.MetricSeverityValue;
                rule.StateChangeComparison.Enabled = false;
            }

            RebuildLinks();
        }

        private void providersListBox_ItemChecked(object sender, ItemCheckedEventArgs args)
        {
            if (loading)
                return;

            ListViewItem item = args.Item;
            NodeType ntype = item.Tag as NodeType;
            if (ntype == null)
                return;

            bool check = args.Item.Checked;

            NotificationProviderInfo npi = ntype.Tag as NotificationProviderInfo;

            if (npi.ProviderTypeName.EndsWith(".SmtpNotificationProvider"))
            {
                // see if an smtp provider exists
                if (check && GetFirstAvailableProvider(typeof(SmtpDestination)) == null)
                {
                    DialogResult dr = ApplicationMessageBox.ShowMessage(this,
                            "In order to add an e-mail destination an SMTP Notification Provider is needed.  Would you like to create an SMTP Notification Provider?",
                            null,
                            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Question,
                            Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OKCancel, false);

                    if (dr == DialogResult.OK)
                    {
                        NotificationRulesDialog owner = Owner as NotificationRulesDialog;
                    }
                    if (dr != DialogResult.OK)
                    {
                        loading = true;
                        args.Item.Checked = false;
                        loading = false;
                        return;
                    }
                }

                ProviderSelectionChanged(npi, check);
            }
            else
                if (npi.ProviderTypeName.EndsWith(".EventLogNotificationProvider"))
            {
                ProviderSelectionChanged(npi, check, true);
            }
            else
                    if (npi.ProviderTypeName.EndsWith(".TaskNotificationProvider"))
            {
                ProviderSelectionChanged(npi, check, true);
            }

            RebuildLinks();
        }

        private IList<string> GetMonitoredServers()
        {
            List<string> list = new List<string>();

            foreach (MonitoredSqlServerWrapper server in ApplicationModel.Default.ActiveInstances)
            {
                list.Add(server.InstanceName);
            }

            return list;
        }

        internal class DestinationFinder
        {
            internal Type DestinationType;

            internal bool DestinationPredicate(NotificationDestinationInfo destination)
            {
                return destination.GetType() == DestinationType;
            }
        }

        private T[] GetDestinationsForType<T>() where T : class
        {
            DestinationFinder finder = new DestinationFinder();
            finder.DestinationType = typeof(T);
            List<NotificationDestinationInfo> selected = rule.Destinations.FindAll(finder.DestinationPredicate);

            T[] result = new T[selected.Count];
            int i = 0;
            foreach (NotificationDestinationInfo dest in selected)
            {
                result[i] = dest as T;
            }

            return result;
        }

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditableEntity = new AuditableEntity();

            auditableEntity.Name = txtDescription.Text;
            String description = Regex.Replace(rulePreview.DocumentText, RegExToRemoveTdCode, " ");
            description = Regex.Replace(description, RegExToRemoveHtmlCode, String.Empty);
            auditableEntity.AddMetadataProperty("Description", description);

            return auditableEntity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return GetAuditableEntity();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NotificationRule notificationRule = NotificationRule;
            notificationRule.Description = txtDescription.Text;
            AuditableEntity entity = GetAuditableEntity();
            try
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                if (notificationRule.Id == Guid.Empty)
                {
                    AuditingEngine.SetAuxiliarData("AddActionResponse", entity);
                    NotificationRule = managementService.AddNotificationRule(notificationRule);
                }
                else
                {
                    entity.AddMetadataProperty("EditType", "EditAlertResponse");
                    AuditingEngine.SetAuxiliarData("EditActionResponse", entity);
                    managementService.UpdateNotificationRule(notificationRule);
                }
                rule.IsAlertResponseDailogClick = true;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to save the Alert Response.", ex);
                // focus the first field on the form
                txtDescription.Focus();
                DialogResult = DialogResult.None;
            }
        }

        private void rulePreview_Navigating(object sender, WebBrowserNavigatingEventArgs args)
        {
            Uri uri = args.Url;
            string scheme = uri.Scheme;
            string senderName = ((WebBrowser)sender).Name;
            string path = uri.AbsolutePath;
            string query = uri.Query;
            if (!String.IsNullOrEmpty(query))
                query = query.Substring(1);

            if (scheme != "about")
            {
                args.Cancel = true;

                if (scheme.Equals("internal"))
                {
                    switch (path)
                    {
                        case "Servers":
                            {
                                ServerSelectionDialog ssd = new ServerSelectionDialog();
                                ssd.Servers = GetMonitoredServers();
                                List<string> selected = new List<string>();
                                foreach (string name in rule.ServerNameComparison.ServerNames)
                                    selected.Add(name);
                                ssd.SelectedServers = selected;
                                if (ssd.ShowDialog(this) == DialogResult.OK)
                                {
                                    rule.ServerNameComparison.ServerNames.Clear();
                                    rule.ServerNameComparison.ServerNames.AddRange(selected);
                                }
                            }
                            break;
                        case "ServerTags":
                            {
                                using (SelectServerTagsDialog ssd = new SelectServerTagsDialog())
                                {
                                    // set list of available tags
                                    ICollection<string> tags = ssd.Tags;
                                    foreach (Pair<string, int> tagPair in tagMap.Values)
                                        tags.Add(tagPair.First);

                                    // set list of selected tags
                                    List<string> selected = new List<string>();
                                    foreach (int tagId in rule.ServerTagComparison.ServerTagIds)
                                    {
                                        if (tagMap.ContainsKey(tagId))
                                            selected.Add(tagMap[tagId].First);
                                    }
                                    ssd.SelectedTags = selected;

                                    if (ssd.ShowDialog(this) == DialogResult.OK)
                                    {
                                        ArrayList ruleTags = rule.ServerTagComparison.ServerTagIds;
                                        ruleTags.Clear();

                                        ICollection<string> selectedTags = ssd.SelectedTags;
                                        foreach (Pair<string, int> tagPair in tagMap.Values)
                                        {
                                            if (selectedTags.Contains(tagPair.First))
                                                ruleTags.Add(tagPair.Second);
                                        }
                                    }
                                }
                            }
                            break;
                        case "Metrics":
                            {
                                MetricSelectionDialog msd = new MetricSelectionDialog(metricDefinitions);
                                msd.SelectedMetrics = rule.MetricIDs;
                                if (msd.ShowDialog(this) == DialogResult.OK)
                                {
                                    rule.MetricIDs = msd.SelectedMetrics;
                                }
                            }
                            break;
                        case "RankValue":
                            {
                                AlertRankValuesDialog arv = new AlertRankValuesDialog();
                                string[] rankValueWithIndex = new string[2];
                                if (!string.IsNullOrEmpty(rule.RankValue))
                                {
                                    rankValueWithIndex = rule.RankValue.Trim().Split();

                                    if (rankValueWithIndex.Length >= 2)
                                        NotificationRuleLabelGenerator.RankSelectedValue = rankValueWithIndex[1];
                                    else
                                        NotificationRuleLabelGenerator.RankSelectedValue = rankValueWithIndex[0];
                                }
                                if (arv.ShowDialog(this) == DialogResult.OK)
                                {
                                    rule.RankValue = arv.rankValue.ToString().Trim();
                                    rule.CmbRankValue = arv.cmbRankValue.Trim();
                                }
                            }
                            break;
                        case "MetricSeverityValue":
                            {
                                MetricSeverityDialog ms = new MetricSeverityDialog();
                                NotificationRuleLabelGenerator.SelectedSeverityValue = Convert.ToInt32(rule.MetricSeverityValue);
                                if (ms.ShowDialog(this) == DialogResult.OK)
                                {
                                    rule.MetricSeverityValue = ms.metricSheverityValue.ToString().Trim();
                                    rule.IsMetricSheverityDialogOk = true;
                                }
                            }
                            break;
                        case "MetricStateDeviation":
                            {
                                StateSelectionDialog scd = new StateSelectionDialog();
                                scd.Rule = rule;
                                scd.ShowDialog(this);
                            }
                            break;
                        case "MetricStateChanged":
                            {
                                StateChangeDialog scd = new StateChangeDialog();
                                scd.Rule = rule.StateChangeComparison;
                                scd.ShowDialog(this);
                            }
                            break;
                        case "SnapshotTime":
                            {
                                SnapshotTimeDialog std = new SnapshotTimeDialog();
                                std.SnapshotTimeRule = rule.SnapshotTimeComparison;
                                if (std.ShowDialog(this) == DialogResult.OK)
                                {
                                }
                            }
                            break;
                        case "TaskDestination":
                        case "JobDestination":
                        case "ProgramDestination":
                        case "QMDestination":
                        case "SqlDestination":
                        case "PulseDestination":
                        case "SmtpDestination":
                        case "PADestination":
                        case "QWaitsDestination":
                        case "PowerShellDestination":
                        case "SCOMAlertDestination":
                        case "SCOMEventDestination":
                            {
                                int x = Convert.ToInt32(query);

                                NotificationDestinationInfo destination = null;

                                if (senderName.Equals("providersListBrowser"))
                                    destination = GetDestination(x);
                                if (senderName.Equals("rulePreview"))
                                    destination = GetEnabledDestinations(rule)[x];

                                ConfigureDestination(destination);
                            }
                            break;
                        case "DestinationCheckChanged":
                            {
                                OnDestinationCheckChanged(query);
                                rule.IsMetricSheverityDialogOk = true;
                            }
                            break;
                        case "RemoveDestination":
                            {
                                OnDestinationRemoveClicked(query);
                            }
                            break;
                    }
                }
                RebuildLinks();
            }
        }

        private DialogResult ConfigureDestination(NotificationDestinationInfo destination)
        {
            DialogResult result = DialogResult.OK;

            if (destination == null)
                return result; ;

            if (destination is SmtpDestination)
            {
                NotificationProviderInfo npi = destination.Provider;
                SmtpNotificationProviderInfo providerInfo = (SmtpNotificationProviderInfo)npi;
                bool smtpProviderIsConfigured = ((providerInfo != null) && !string.IsNullOrEmpty(providerInfo.SmtpServer));

                if (!smtpProviderIsConfigured && (providerInfo != null))
                {
                    if (MessageBox.Show(string.Format("This SMTP provider \"{0}\" is not configured{1}{1}Do you want to configure it?", providerInfo.Name, Environment.NewLine),
                        @"Alert Response", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SmtpProviderConfigDialog configDialog = new SmtpProviderConfigDialog(managementService);
                        configDialog.NotificationProvider = providerInfo;
                        smtpProviderIsConfigured = configDialog.ShowDialog(this) == DialogResult.OK;
                    }
                }

                if (smtpProviderIsConfigured)
                {
                    using (SmtpDestinationsDialog sdd = new SmtpDestinationsDialog())
                    {
                        IList<NotificationProviderInfo> smtpProviders = GetAvailableProviders();
                        if (!smtpProviders.Contains(destination.Provider))
                        {
                            smtpProviders.Insert(0, destination.Provider);
                        }
                        else
                        {
                            int index = smtpProviders.IndexOf(destination.Provider);
                            smtpProviders[index] = destination.Provider;
                        }
                        sdd.Destination = (SmtpDestination)destination;
                        sdd.SetNotificationProviders(smtpProviders);

                        result = sdd.ShowDialog(this);
                    }
                }
            }
            else if (destination is TaskDestination)
            {
                using (TaskDestinationsDialog tdd = new TaskDestinationsDialog())
                {
                    tdd.Destination = (TaskDestination)destination;
                    result = tdd.ShowDialog(this);
                }
            }
            else if (destination is PulseDestination)
            {
                using (PulseDestinationDialog pdd = new PulseDestinationDialog())
                {
                    pdd.Destination = (PulseDestination)destination;
                    result = pdd.ShowDialog(this);
                }
            }
            else if (destination is ProgramDestination)
            {
                using (ProgramDestinationDialog pdd = new ProgramDestinationDialog())
                {
                    pdd.Destination = (ProgramDestination)destination;
                    result = pdd.ShowDialog(this);
                }
            }
            else if (destination is JobDestination)
            {
                using (JobDestinationDialog tdd = new JobDestinationDialog())
                {
                    tdd.Destination = (JobDestination)destination;
                    result = tdd.ShowDialog(this);
                }
            }
            else if (destination is SqlDestination)
            {
                // TODO
                using (SqlDestinationDialog tdd = new SqlDestinationDialog())
                {
                    tdd.Destination = (SqlDestination)destination;
                    result = tdd.ShowDialog(this);
                }
            }
            else if (destination is EnableQMDestination)
            {



                using (QueryMonitorDestinationDialog qmdd = new QueryMonitorDestinationDialog())
                {
                    EnableQMDestination qmd = (EnableQMDestination)destination;

                    if (qmd.IsPropertySet(EnableQMDestination.PROPERTY_DURATION))
                    {
                        qmdd.QMDurationInMinutes = qmd.DurationInMinutes;
                    }

                    result = qmdd.ShowDialog(this);

                    if (result == DialogResult.OK)
                    {
                        qmd.DurationInMinutes = qmdd.QMDurationInMinutes;
                    }
                }

            }
            else if (destination is EnablePADestination)
            {
                using (PrescriptiveAnalysisDestinationDialog paDialog = new PrescriptiveAnalysisDestinationDialog())
                {
                    EnablePADestination pad = (EnablePADestination)destination;

                    if (pad.IsPropertySet(EnablePADestination.PROPERTY_CATEGORY))
                    {
                        //if (pad.BlockedCategoriesList.Count == 1 && pad.BlockedCategoriesList[0] == -1)
                        //    paDialog.BlockedCategoryID = new List<int>();
                        paDialog.blockedCategoriesList = pad.BlockedCategoriesListObject;
                        paDialog.isConfigured = true;
                    }
                    else
                    {
                        paDialog.isConfigured = false;
                    }

                    result = paDialog.ShowDialog(this);

                    if (result == DialogResult.OK)
                    {
                        pad.BlockedCategoriesListObject = paDialog.blockedCategoriesList;
                    }
                }

            }
            //Query Waits Alert Action Response
            //10.1 Srishti Purohit SQLdm
            else if (destination is EnableQWaitsDestination)
            {
                using (QueryWaitsDestinationDialog qwdd = new QueryWaitsDestinationDialog())
                {
                    EnableQWaitsDestination qwd = (EnableQWaitsDestination)destination;

                    if (qwd.IsPropertySet(EnableQWaitsDestination.PROPERTY_DURATION))
                    {
                        qwdd.QWaitsDurationInMinutes = qwd.DurationInMinutes;
                    }

                    result = qwdd.ShowDialog(this);

                    if (result == DialogResult.OK)
                    {
                        qwd.DurationInMinutes = qwdd.QWaitsDurationInMinutes;
                    }
                }
            }
            //Powershell Alert Response Action
            //SQLdm 10.1 Srishti Purohit
            else if (destination is PowerShellDestination)
            {
                // TODO
                using (PowerShellDialog tdd = new PowerShellDialog())
                {
                    tdd.Destination = (PowerShellDestination)destination;
                    result = tdd.ShowDialog(this);
                }
            }

            return result;

        }

        private List<NotificationDestinationInfo> GetEnabledDestinations(NotificationRule rule)
        {
            List<NotificationDestinationInfo> results = new List<NotificationDestinationInfo>();

            foreach (NotificationDestinationInfo ndi in rule.Destinations)
            {
                if (ndi.Enabled)
                    results.Add(ndi);
            }

            return results;
        }

        public NotificationDestinationInfo GetDestination(int x)
        {
            if (x >= 0 && x < rule.Destinations.Count)
                return rule.Destinations[x];
            return null;
        }

        public IList<NotificationProviderInfo> GetAvailableProviders()
        {
            if (Owner is NotificationRulesDialog)
                return ((NotificationRulesDialog)Owner).GetNotificationProviders();
            else
                return ProviderTypes;
        }

        public NotificationProviderInfo GetFirstAvailableProvider(Type destinationType)
        {
            foreach (NotificationProviderInfo npi in GetAvailableProviders())
            {
                if (npi.DestinationType == destinationType)
                    return npi;
            }
            return null;
        }

        private void UpdateControls()
        {
            NotificationRule rule = NotificationRule;

            string descError = String.Empty;
            string actionError = String.Empty;
            string ruleError = String.Empty;

            bool okEnabled = true;

            try
            {
                if (String.IsNullOrEmpty(txtDescription.Text.Trim()))
                {
                    nameErrorPictureBox.Visible = true;
                    UltraToolTipInfo tti = new UltraToolTipInfo();
                    tti.ToolTipTitle = "Name Required";
                    tti.ToolTipText = "Please enter a name for your rule.";
                    tti.ToolTipImage = ToolTipImage.Info;
                    tooltipManager.SetUltraToolTip(nameErrorPictureBox, tti);
                    okEnabled = false;
                }
                else if (rule.IsMetricSeverityChecked && string.IsNullOrEmpty(rule.MetricSeverityValue))
                {
                    UltraToolTipInfo tti = new UltraToolTipInfo();
                    ruleErrorPictureBox.Visible = true;
                    tti.ToolTipTitle = "Specific Time Frame is Required";
                    tti.ToolTipText = "Specific Time Frame is Required.";
                    tti.ToolTipImage = ToolTipImage.Info;
                    tooltipManager.SetUltraToolTip(ruleErrorPictureBox, tti);
                    okEnabled = false;
                }
                else if (rule.IsRankValueChecked && rule.RankValue != null && rule.RankValue.Trim() == "")
                {

                    UltraToolTipInfo tti = new UltraToolTipInfo();
                    ruleErrorPictureBox.Visible = true;
                    tti.ToolTipTitle = "Alert Rank is Required";
                    tti.ToolTipText = "Alert Rank is Required.";
                    tti.ToolTipImage = ToolTipImage.Info;
                    tooltipManager.SetUltraToolTip(ruleErrorPictureBox, tti);
                    okEnabled = false;
                }
                else
                {
                    nameErrorPictureBox.Visible = false;
                    rule.Validate(false);
                    actionErrorPictureBox.Visible = ruleErrorPictureBox.Visible = false;
                }
            }
            catch (ActionRuleValidationException arve)
            {
                okEnabled = false;
                actionErrorPictureBox.Visible = arve.ActionMessage;
                ruleErrorPictureBox.Visible = arve.RuleMessage;
                UltraToolTipInfo tti = new UltraToolTipInfo();
                tti.ToolTipTitle = arve.Message;
                tti.ToolTipText = arve.Message2;
                tti.ToolTipImage = ToolTipImage.Info;

                if (arve.ActionMessage)
                    tooltipManager.SetUltraToolTip(actionErrorPictureBox, tti);
                if (arve.RuleMessage)
                    tooltipManager.SetUltraToolTip(ruleErrorPictureBox, tti);
            }
            catch (Exception)
            {
                /* can't imagine why we could get here */
            }
            okButton.Enabled = okEnabled;
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
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

        private void rulePreview_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Help || e.KeyCode == Keys.F1)
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
            }
        }

        private void addActionButton_Click(object sender, EventArgs e)
        {
            //bool includeEventLog = true;
            //bool includeEnableQM = true;

            IList<NotificationProviderInfo> availableProviders = GetAvailableProviders();
            List<NotificationProviderInfo> providers = new List<NotificationProviderInfo>(availableProviders);

            if (rule.Destinations.Count > 0)
            {
                foreach (NotificationDestinationInfo dest in rule.Destinations)
                {
                    if (dest is EventLogDestination
                        || dest is EnableQMDestination
                        || dest is SnmpDestination
                        || dest is PulseDestination
                        || dest is EnablePADestination
                        || dest is EnableQWaitsDestination
                        || dest is SCOMAlertDestination
                        || dest is SCOMEventDestination)
                    {
                        if (providers.Contains(dest.Provider))
                            providers.Remove(dest.Provider);
                    }
                }
            }

            using (AddActionDialog dlg = new AddActionDialog(providers, availableProviders != providerTypes))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (NotificationProviderInfo npi in dlg.SelectedProviders)
                    {
                        NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                        ndi.Provider = (NotificationProviderInfo)npi.Clone();
                        if (DialogResult.OK == ConfigureDestination(ndi))
                        {
                            ndi.Enabled = true;
                            rule.Destinations.Add(ndi);
                        }
                        else
                        {
                            ndi.Enabled = false;
                        }
                    }
                }
            }
            RebuildLinks();
        }


        // TODO: Add logic to prompt for more info if necessary.
        public void OnDestinationCheckChanged(string value)
        {
            using (LOG.InfoCall("OnDestinationCheckChanged"))
            {
                int x = Convert.ToInt32(value);
                if (x < 0 || x > rule.Destinations.Count)
                    return;

                NotificationDestinationInfo destination = rule.Destinations[x];

                destination.Enabled = !destination.Enabled;

                if (destination.Enabled)
                {
                    if (destination is SmtpDestination ||
                        destination is SnmpDestination)
                    {
                        NotificationProviderInfo npi = destination.Provider;

                        DialogResult cfgResult = DialogResult.OK;

                        if (npi is SmtpNotificationProviderInfo)
                        {
                            if (string.IsNullOrEmpty(((SmtpNotificationProviderInfo)npi).SmtpServer))
                            {
                                using (SmtpProviderConfigDialog spcd = new SmtpProviderConfigDialog(managementService))
                                {
                                    spcd.NotificationProvider = destination.Provider;
                                    cfgResult = spcd.ShowDialog(this);
                                }
                            }
                        }
                        else if (npi is SnmpNotificationProviderInfo)
                        {
                            if (string.IsNullOrEmpty(((SnmpNotificationProviderInfo)npi).Address))
                            {
                                using (SnmpProviderConfigDialog spcd = new SnmpProviderConfigDialog(managementService))
                                {
                                    spcd.NotificationProvider = destination.Provider;
                                    cfgResult = spcd.ShowDialog(this);
                                }
                            }
                        }

                        if ((cfgResult == DialogResult.OK) &&
                            (Owner is NotificationRulesDialog))
                            ((NotificationRulesDialog)Owner).ReloadProviders = true;
                        else
                            destination.Enabled = false;
                    }

                    if (destination.Enabled && String.IsNullOrEmpty(destination.ToString()))
                    {
                        if (destination is SmtpDestination ||
                            destination is PulseDestination ||
                            destination is SqlDestination ||
                            destination is EnableQMDestination ||
                            destination is EnableQWaitsDestination ||
                            destination is ProgramDestination ||
                            destination is JobDestination ||
                            destination is EnablePADestination ||
                            destination is TaskDestination ||
                            destination is PowerShellDestination ||
                            destination is SCOMAlertDestination ||
                            destination is SCOMEventDestination)
                        {
                            if (DialogResult.OK != ConfigureDestination(destination))
                            {
                                destination.Enabled = false;
                            }
                        }
                    }
                }

                RebuildLinks();
            }
        }

        public void OnDestinationRemoveClicked(string value)
        {
            using (LOG.InfoCall("OnDestinationRemoveClicked"))
            {
                int x = Convert.ToInt32(value);
                if (x >= 0 && x < rule.Destinations.Count)
                {
                    NotificationDestinationInfo destination = rule.Destinations[x];
                    rule.Destinations.RemoveAt(x);
                }

                RebuildLinks();
            }
        }

        private void providersListBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Help || e.KeyCode == Keys.F1)
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
            }
        }

        private void providersListBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs args)
        {
            rulePreview_Navigating(sender, args);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            NotificationRuleLabelGenerator.Rvalue = 15;
        }
    }

}
