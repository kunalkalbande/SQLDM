using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Helpers.WebServices;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class DeployReportsWizard : Form
    {
        private bool ignoreCheckedChanged = false;
        private readonly IDictionary<string, object> reportParameters;
        private string _selectedCustom;

        public DeployReportsWizard()
            : this(null)
        {

        }

        public DeployReportsWizard(IEnumerable<Common.Objects.CustomReport> customReports)
        {
            InitializeComponent();
            InitializeFormData(customReports);
            scheduledEmailDataSourceInfoPanel.Visible = false;

            AdaptFontSize();
        }

        public DeployReportsWizard(ReportTypes selectedReport, string selectedCustom)
            : this(selectedReport, null, selectedCustom, null)
        {
        }

        public DeployReportsWizard(ReportTypes selectedReport, IDictionary<string, object> reportParameters, string selectedCustom, IEnumerable<Common.Objects.CustomReport> customReports)
            : this(customReports)
        {
            for (int i = 0; i < reportsCheckedListBox.Items.Count; i++)
            {
                CheckboxListItem report = (CheckboxListItem)reportsCheckedListBox.Items[i];

                if ((ReportTypes)report.Tag == selectedReport && (ReportTypes)report.Tag != ReportTypes.Custom)
                {
                    reportsCheckedListBox.SetItemChecked(i, true);
                    break;
                }
                if ((ReportTypes)report.Tag == ReportTypes.Custom && selectedCustom == report.Text)
                {
                    reportsCheckedListBox.SetItemChecked(i, true);
                    _selectedCustom = selectedCustom;
                    break;
                }
            }

            this.reportParameters = reportParameters;
            wizard.SelectedPage = this.reportParameters == null ? reportServerSettingsPage : emailSubscriptionPage;
            scheduledEmailDataSourceInfoPanel.Visible = this.reportParameters != null;

            if (this.reportParameters != null)
            {
                useExistingDataSourceRadioButton.Enabled =
                    useWindowsAuthenticationRadioButton.Enabled = false;
                createNewDataSourceRadioButton.Checked =
                    useSqlServerAuthenticationRadioButton.Checked = true;
            }
        }

        private void InitializeFormData(IEnumerable<Common.Objects.CustomReport> customReports)
        {
            // Initialize reports

            reportsCheckedListBox.BeginUpdate();

            Type reportTypesType = typeof(ReportTypes);
            Array reportTypesValues = Enum.GetValues(reportTypesType);

            foreach (int value in reportTypesValues)
            {
                ReportTypes reportType = (ReportTypes)value;
                FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
                TitleAttribute[] descriptionAttributes =
                    (TitleAttribute[])fieldInfo.GetCustomAttributes(typeof(TitleAttribute), false);
                DeployableAttribute[] deployableAttributes =
                    (DeployableAttribute[])fieldInfo.GetCustomAttributes(typeof(DeployableAttribute), false);

                if (descriptionAttributes.Length > 0 &&
                    deployableAttributes.Length > 0 &&
                    deployableAttributes[0].Deployable &&
                    reportType != ReportTypes.Custom &&
                    reportType != ReportTypes.AlwaysOnDatabaseStatistics
                    //SQLDM-29917 start -Preventing DetailedSessionReport from getting deployed
                    && reportType != ReportTypes.DetailedSessionReport
                    //SQLDM-2997 end
                    ) //TODO: It is possible to add a new Attribute into the ReportsView class in order to avoid the sub-reports in this view
                {
                    reportsCheckedListBox.Items.Add(new CheckboxListItem(descriptionAttributes[0].Title, reportType));
                }
            }
            if (customReports != null)
            {
                foreach (CustomReport report in customReports)
                {
                    reportsCheckedListBox.Items.Add(new CheckboxListItem(report.Name, ReportTypes.Custom));
                }
            }
            reportsCheckedListBox.EndUpdate();

            // Initialize report server and data source

            if (Settings.Default.PreviouslyUsedReportServers != null)
            {
                foreach (string previousReportServer in Settings.Default.PreviouslyUsedReportServers)
                {
                    reportServerComboBox.Items.Add(previousReportServer);
                }
                reportServerComboBox.SelectedIndex = 0;
                browseReportFoldersButton.Enabled = true;
            }
            else
            {
                browseReportFoldersButton.Enabled = false;
            }
            if (Settings.Default.PreviouslyUsedReportFolder != null &&
                Settings.Default.PreviouslyUsedReportFolder.Length > 0)
            {
                reportFolderTextbox.Text = Settings.Default.PreviouslyUsedReportFolder;
                useExistingDataSourceRadioButton.Enabled = true;
                useExistingDataSourceRadioButton.Checked = true;
            }
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;

            if (connectionInfo != null)
            {
                dataSourceServerTextBox.Text = connectionInfo.InstanceName;
                dataSourceDatabaseTextBox.Text = connectionInfo.DatabaseName;

                if (!connectionInfo.UseIntegratedSecurity)
                {
                    useSqlServerAuthenticationRadioButton.Checked = true;
                    loginNameTextbox.Text = connectionInfo.UserName;
                    passwordTextbox.Text = connectionInfo.Password;
                }
            }

            // Initialize email delivery
            emailPriorityComboBox.SelectedItem = "Normal";
        }

        private void reportsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                selectAllReportsCheckBox.Checked = false;
                ignoreCheckedChanged = false;
            }
        }

        private void selectAllReportsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;

                for (int i = 0; i < reportsCheckedListBox.Items.Count; i++)
                {
                    reportsCheckedListBox.SetItemChecked(i, selectAllReportsCheckBox.Checked);
                }
                ignoreCheckedChanged = false;
            }
        }

        private void selectReportsPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            if (reportsCheckedListBox.CheckedItems.Count == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "At least one report must be selected for deployment.");
                e.Cancel = true;
            }
            else if (reportsCheckedListBox.CheckedItems.Count > 1 || reportParameters == null)
            {
                wizard.SelectedPage = reportServerSettingsPage;
            }
        }

        private void createNewDataSourceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            newDataSourcePanel.Enabled = createNewDataSourceRadioButton.Checked;
        }

        private void useSqlServerAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            loginNameTextbox.Enabled =
                passwordTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
        }

        private void reportServerSettings_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            if (reportServerComboBox.Text.Trim().Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "A Microsoft Reporting Services Server must be specified in order to continue.");
                e.Cancel = true;
            }
            else if (createNewDataSourceRadioButton.Checked && useSqlServerAuthenticationRadioButton.Checked &&
                     loginNameTextbox.Text.Trim().Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "A login name must be specified when using SQL Server Authentication for the data source.");
                e.Cancel = true;
            }
        }

        private void wizard_Finish(object sender, EventArgs e)
        {
            ReportServerData serverData = new ReportServerData();

            //make sure the report folder path is properly formatted.
            serverData.reportFolder = reportFolderTextbox.Text.Trim();

            if (serverData.reportFolder.StartsWith("/") == false)
            {
                serverData.reportFolder = String.Format("/{0}", serverData.reportFolder);
            }

            if (serverData.reportFolder.Length > 1)
            {
                serverData.reportFolder = serverData.reportFolder.TrimEnd('/');
            }

            serverData.reports = reportsCheckedListBox.CheckedItems;
            serverData.dataSourceInfo = SetDataSourceInfo();
            serverData.reportServerURL = GetReportServerURL();
            CreateScheduleAndEmail(ref serverData);
            backgroundWorker.RunWorkerAsync(serverData);
            finishPage.AllowMoveNext = false;
            finishPage.AllowMovePrevious = false;
            Cursor = Cursors.WaitCursor;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReportingService2005 webService = new ReportingService2005();
            ReportServerData serverData = (ReportServerData)e.Argument;

            webService.UseDefaultCredentials = true;
            webService.Url = serverData.reportServerURL;

            //Create the report folder
            CreateFolder(webService, serverData.reportFolder);

            // if the user is choosing to use an existing datasource, dataSourceInfo will be null
            if (serverData.dataSourceInfo != null)
            {
                //Create the report data source
                CreateDataSource(webService, serverData.reportFolder, serverData.dataSourceInfo);
            }
            // Make sure there is a data source
            else if (!DoesDatasourceExist(serverData))
            {
                throw new DesktopClientException(
                    "A valid SQLDM report data source was not found. Click the Back button and configure a new data source.");
            }

            // Upload all the specified reports
            foreach (CheckboxListItem report in serverData.reports)
            {
                ReportTypes reportType = (ReportTypes)report.Tag;
                if (reportType == ReportTypes.Custom)
                {
                    DeployReport(webService, serverData.reportFolder, report.Text);
                    if (serverData.extnSettings != null)
                    {
                        string reportName = report.Text;
                        webService.CreateSubscription(
                            String.Format("{0}/{1}", serverData.reportFolder == "/" ? String.Empty : serverData.reportFolder,
                                          reportName), serverData.extnSettings,
                            String.Format("SQLDM Report Subscription: {0}", reportName), "TimedSubscription",
                            serverData.scheduleXML, serverData.reportParameters);
                    }
                }
                else
                {
                    DeployReport(webService, serverData.reportFolder, reportType, true);
                    // add the subscription if one is specified
                    if (serverData.extnSettings != null)
                    {
                        string reportName = ReportsHelper.GetReportTitle(reportType);
                        webService.CreateSubscription(
                            String.Format("{0}/{1}", serverData.reportFolder == "/" ? String.Empty : serverData.reportFolder,
                                          reportName), serverData.extnSettings,
                            String.Format("SQLDM Report Subscription: {0}", reportName), "TimedSubscription",
                            serverData.scheduleXML, serverData.reportParameters);
                    }
                }



                if (backgroundWorker.CancellationPending)
                {
                    break;
                }
            }

            e.Cancel = backgroundWorker.CancellationPending;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            finishPage.AllowMoveNext = true;
            finishPage.AllowMovePrevious = true;

            if (backgroundWorker == sender)
            {
                if (!e.Cancelled)
                {
                    if (e.Error == null)
                    {
                        // Save report server to previously used collection
                        string reportServer = reportServerComboBox.Text.Trim();

                        if (Settings.Default.PreviouslyUsedReportServers == null)
                        {
                            Settings.Default.PreviouslyUsedReportServers = new StringCollection();
                        }
                        else
                        {
                            Settings.Default.PreviouslyUsedReportServers.Remove(reportServer);
                        }

                        Settings.Default.PreviouslyUsedReportServers.Insert(0, reportServer);

                        // Save report folder to previously used
                        string reportFolder = reportFolderTextbox.Text.Trim();
                        Settings.Default.PreviouslyUsedReportFolder = reportFolder;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred during the deployment operation.", e.Error);
                        DialogResult = DialogResult.None;
                    }
                }
            }
        }

        private void wizard_Cancel(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            DeployReportsWizard.ActiveForm.Close();
        }

        private void DeployReportsWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void finishPage_BeforeDisplay(object sender, EventArgs e)
        {
            string reportFolder = reportFolderTextbox.Text.Trim();
            reportServerSummaryLabel.Text = reportServerComboBox.Text.Trim();
            reportFolderServerLabel.Text = reportFolder.Length == 0 ? "< Root Folder >" : reportFolder;
            reportDataSourceSummaryLabel.Text = useExistingDataSourceRadioButton.Checked
                                                    ? "Use existing data source"
                                                    : "Custom";

            if (reportsCheckedListBox.CheckedItems.Count == 1 && reportParameters != null)
            {
                StringCollection daysSelected = new StringCollection();
                if (emailSundayCheckbox.Checked) daysSelected.Add("Sun");
                if (emailMondayCheckbox.Checked) daysSelected.Add("Mon");
                if (emailTuesdayCheckbox.Checked) daysSelected.Add("Tue");
                if (emailWednesdayCheckbox.Checked) daysSelected.Add("Wed");
                if (emailThursdayCheckbox.Checked) daysSelected.Add("Thu");
                if (emailFridayCheckbox.Checked) daysSelected.Add("Fri");
                if (emailSaturdayCheckbox.Checked) daysSelected.Add("Sat");

                StringBuilder emailDeliverySummary = new StringBuilder("At ");
                emailDeliverySummary.Append(emailStartTimePicker.Text);
                emailDeliverySummary.Append(" every ");
                for (int i = 0; i < daysSelected.Count; i++)
                {
                    emailDeliverySummary.Append(daysSelected[i]);

                    if (i < daysSelected.Count - 1)
                    {
                        emailDeliverySummary.Append(", ");
                    }
                }
                emailDeliverySummaryLabel.Text = emailDeliverySummary.ToString();
            }
            else
            {
                emailDeliverySummaryLabel.Text = "N/A";
            }

            selectedReportsSummaryListBox.Items.Clear();
            foreach (CheckboxListItem selectedReport in reportsCheckedListBox.CheckedItems)
            {
                selectedReportsSummaryListBox.Items.Add(selectedReport);
            }
        }

        private void emailSubscriptionPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            if (emailToTextBox.Text.Trim().Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "An email recipient must be specified in order to continue.");
                e.Cancel = true;
            }
            else if (emailSubjectTextBox.Text.Trim().Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "An email subject must be specified in order to continue.");
                e.Cancel = true;
            }
            else if (
                !emailSundayCheckbox.Checked &&
                !emailMondayCheckbox.Checked &&
                !emailTuesdayCheckbox.Checked &&
                !emailWednesdayCheckbox.Checked &&
                !emailThursdayCheckbox.Checked &&
                !emailFridayCheckbox.Checked &&
                !emailSaturdayCheckbox.Checked)
            {
                ApplicationMessageBox.ShowInfo(this, "At least one day must be selected on which to run the report in order to continue.");
                e.Cancel = true;
            }
        }

        private void reportServerSettingsPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            if (reportsCheckedListBox.CheckedItems.Count > 1 || reportParameters == null)
            {
                wizard.SelectedPage = selectReportsPage;
            }
        }

        private void reportServerSettingsPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private string CreateFolder(ReportingService2005 webService, string folderName)
        {
            string[] folders = folderName.Split(new char[] { '/', '\\' });

            string currentLocation = "/";
            foreach (string fld in folders)
            {
                string folder = fld.Trim();

                if (folder.Length == 0)
                {
                    continue;
                }

                string tmpLocation;

                if (currentLocation.Length == 1)
                {
                    tmpLocation = currentLocation + folder;
                }
                else
                {
                    tmpLocation = currentLocation + "/" + folder;
                }

                if (!ItemExists(webService, tmpLocation, ItemTypeEnum.Folder))
                {
                    webService.CreateFolder(folder, currentLocation, null);
                }

                if (currentLocation.Length == 1)
                {
                    currentLocation += folder;
                }
                else
                {
                    currentLocation += "/" + folder;
                }
            }
            return currentLocation;
        }

        private bool ItemExists(ReportingService2005 webService, string item, ItemTypeEnum itemType)
        {
            ItemTypeEnum retType = webService.GetItemType(item);
            return (itemType == retType);
        }

        private DataSourceDefinition SetDataSourceInfo()
        {
            DataSourceDefinition definition = null;

            if (createNewDataSourceRadioButton.Checked)
            {
                // Define the data source definition.
                definition = new DataSourceDefinition();
                definition.ConnectString = "data source=" + dataSourceServerTextBox.Text.Trim() + ";initial catalog=" +
                                           dataSourceDatabaseTextBox.Text.Trim();
                definition.Enabled = true;
                definition.EnabledSpecified = true;
                definition.Extension = "SQL";

                if (useWindowsAuthenticationRadioButton.Checked)
                {
                    definition.CredentialRetrieval = CredentialRetrievalEnum.Integrated;
                }
                else
                {
                    definition.CredentialRetrieval = CredentialRetrievalEnum.Store;
                    definition.UserName = loginNameTextbox.Text.Trim();
                    definition.Password = passwordTextbox.Text.Trim();
                }
                definition.Prompt = null;
                definition.WindowsCredentials = false;
            }
            return definition;
        }

        private void CreateDataSource(ReportingService2005 webService, string location, DataSourceDefinition definition)
        {
            Property prop = new Property();
            prop.Name = "Description";
            prop.Value = "Data source for SQL Diagnostic Manager Reports.";

            webService.CreateDataSource("SQL Diagnostic Manager Data Source", location, true, definition, new Property[] { prop });
        }

        private void DeployReport(ReportingService2005 webService, string targetLocation, string reportName)
        {
            CustomReport customReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, reportName);

            byte[] reportDef = Encoding.UTF8.GetBytes(customReport.ReportRDL);

            if (reportDef.Length > 0)
            {
                Property prop = new Property();
                prop.Name = "Description";
                prop.Value = customReport.ShortDescription;
                webService.CreateReport(customReport.Name, targetLocation, true, reportDef, new Property[] { prop });
            }
            else
            {
                ApplicationMessageBox.ShowError(Parent, String.Format("Unable to locate report {0}", reportName));
            }
        }

        /// <summary>
        /// Deploys report on Reporting Services
        /// </summary>
        /// <param name="webService"></param>
        /// <param name="targetLocation"></param>
        /// <param name="report"></param>
        /// <param name="showReport">Defines whether the report will be hidden or visible on the Report Manager</param>
        private void DeployReport(ReportingService2005 webService, string targetLocation, ReportTypes report, bool showReport)
        {
            byte[] reportDef = GetRdl(report);

            if (reportDef != null)
            {
                var deployProperties = new Property[2];

                var descriptionProperty = new Property();
                descriptionProperty.Name = "Description";
                descriptionProperty.Value = ReportsHelper.GetReportShortDescription(report);

                // This property is used for sub reports that need to be deployed but cannot run alone
                // So we need to hide them but still deploy them
                var hideReportProperty = new Property();
                hideReportProperty.Name = "Hidden";
                hideReportProperty.Value = showReport ? "false" : "true";

                deployProperties[0] = descriptionProperty;
                deployProperties[1] = hideReportProperty;

                webService.CreateReport(ReportsHelper.GetReportTitle(report), targetLocation, true,
                                        reportDef, deployProperties);

                // Look for subreports
                // If any report needs a sub report to be deployed we shall do it here
                // If we need that report to be hidden then send the last parameter: showReport
                // with the value: false
                switch (report)
                {
                    case ReportTypes.AlwaysOnStatistics:
                        DeployReport(webService, targetLocation,
                                     ReportTypes.AlwaysOnDatabaseStatistics, false);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(Parent,
                                                String.Format("Unable to locate report {0}", report));
            }
        }

        private byte[] GetRdl(ReportTypes reportType)
        {
            byte[] rdlFile;
            string rdlFileName;
            Stream stream;

            switch (reportType)
            {
                //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
                case ReportTypes.DiskSpaceUsage:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskSpaceUsage.rdl";
                        break;
                    }
                case ReportTypes.DiskSpaceHistory:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskSpaceHistory.rdl";
                        break;
                    }
                //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
                //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
                case ReportTypes.QueryWaitStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.QueryWaitStatistics.rdl";
                        break;
                    }
                //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
                case ReportTypes.ActiveAlerts:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ActiveAlerts.rdl";
                        break;
                    }
                case ReportTypes.EnterpriseSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.EnterpriseSummary.rdl";
                        break;
                    }
                case ReportTypes.AlertHistory:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlertHistory.rdl";
                        break;
                    }
                case ReportTypes.DatabaseStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DatabaseStatistics.rdl";
                        break;
                    }
                case ReportTypes.QueryOverview:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.QueryOverview.rdl";
                        break;
                    }
                case ReportTypes.QueryStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.QueryStatistics.rdl";
                        break;
                    }
                case ReportTypes.ServerInventory:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerInventory.rdl";
                        break;
                    }
                case ReportTypes.ServerStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerStatistics.rdl";
                        break;
                    }
                case ReportTypes.ServerSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerSummary.rdl";
                        break;
                    }
                //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                case ReportTypes.DeadlockReport:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DeadlockReport.rdl";
                        break;
                    }
                //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                case ReportTypes.TopDatabaseApps:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopDatabaseApplications.rdl";
                        break;
                    }
                case ReportTypes.TopDatabases:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopDatabases.rdl";
                        break;
                    }
                case ReportTypes.TopQueries:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopQueries.rdl";
                        break;
                    }
                case ReportTypes.TopServers:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.Top Servers.rdl";
                        break;
                    }
               
                //START: SQLdm-29252 10.4.1 Feature Request for Uptime report
                case ReportTypes.ServerUptime:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.UptimeReport.rdl";
                        break;
                    }
                //END: SQLdm-29252 10.4.1 Feature Request for Uptime report
                case ReportTypes.TopTableFrag:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopTablesFragmentation.rdl";
                        break;
                    }
                case ReportTypes.TopTablesGrowth:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopTablesGrowth.rdl";
                        break;
                    }
                case ReportTypes.MirroringSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MirroringOverview.rdl";
                        break;
                    }
                case ReportTypes.MirroringHistory:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MirroringHistory.rdl";
                        break;
                    }
                case ReportTypes.MemorySummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MemorySummary.rdl";
                        break;
                    }
                case ReportTypes.DiskSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskSummary.rdl";
                        break;
                    }
                case ReportTypes.CPUSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CPUSummary.rdl";
                        break;
                    }
                //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                case ReportTypes.DetailedSessionReport:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DetailedSessionReport.rdl";
                        break;
                    }
                //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                case ReportTypes.SessionsSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.SessionsSummary.rdl";
                        break;
                    }
                case ReportTypes.ReplicationSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ReplicationSummary.rdl";
                        break;
                    }
                case ReportTypes.MetricThresholds:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MetricThresholds.rdl";
                        break;
                    }
                case ReportTypes.AlertTemplateReport:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlertTemplate.rdl";
                        break;
                    }
                case ReportTypes.DiskDetails:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskDetails.rdl";
                        break;
                    }
                case ReportTypes.TempdbStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TempdbStatistics.rdl";
                        break;
                    }
                case ReportTypes.VirtualizationSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.VirtualizationSummary.rdl";
                        break;
                    }

                case ReportTypes.VirtualizationStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.VirtualizationStatistics.rdl";
                        break;
                    }
                case ReportTypes.TransactionLogStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TransactionLogStatistics.rdl";
                        break;
                    }
                case ReportTypes.BaselineStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.BaselineStatistics.rdl";
                        break;
                    }
                case ReportTypes.ChangeLogSummary:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ChangeLogSummary.rdl";
                        break;
                    }
                case ReportTypes.AlwaysOnStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnStatistics.rdl";
                        break;
                    }
                case ReportTypes.AlwaysOnTopology:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnTopology.rdl";
                        break;
                    }
                case ReportTypes.AlwaysOnDatabaseStatistics:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnDatabaseStatistics.rdl";
                        break;
                    }
                //case ReportTypes.TemplateComparison:
                //    {
                //        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlertTemplateRpt.rdl";
                //        break;
                //    }
                case ReportTypes.AlertThreshold:
                    {
                        rdlFileName = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlertThreshold.rdl";
                        break;
                    }
                default:
                    {
                        return (null);
                    }
            }
            stream = GetType().Assembly.GetManifestResourceStream(rdlFileName);
            rdlFile = new Byte[stream.Length];
            stream.Read(rdlFile, 0, (int)stream.Length);
            return rdlFile;
        }

        private void CreateScheduleAndEmail(ref ReportServerData serverData)
        {
            ParameterValueOrFieldReference[] paramValues;
            List<ParameterValueOrFieldReference> parameters = new List<ParameterValueOrFieldReference>(4);
            //string[] emailAddresses;
            ParameterValue paramValue;

            // only create a schedule if the data has been set.
            if (String.IsNullOrEmpty(emailToTextBox.Text.Trim()) == false)
            {
                serverData.extnSettings = new ExtensionSettings();
                serverData.extnSettings.Extension = "Report Server Email";

                //To addresses
                //emailAddresses = emailToTextBox.Text.Trim().Split(';');
                //foreach (string emailAddress in emailAddresses)
                //{
                //    if (String.IsNullOrEmpty(emailAddress))
                //    {
                //        continue;
                //    }
                //    paramValue = new ParameterValue();
                //    paramValue.Name = "To";
                //    paramValue.Value = emailAddress.Trim();
                //    parameters.Add(paramValue);
                //}
                string strTo = emailToTextBox.Text.Trim();
                paramValue = new ParameterValue();
                paramValue.Name = "TO";
                paramValue.Value = strTo;
                parameters.Add(paramValue);

                //Subject
                paramValue = new ParameterValue();
                paramValue.Name = "Subject";
                paramValue.Value = emailSubjectTextBox.Text.Trim();
                parameters.Add(paramValue);

                //Priority
                paramValue = new ParameterValue();
                paramValue.Name = "Priority";
                paramValue.Value = emailPriorityComboBox.SelectedItem.ToString();
                parameters.Add(paramValue);

                //Render Format, this is not an option in the wizard but must be specified.
                paramValue = new ParameterValue();
                paramValue.Name = "RenderFormat";
                paramValue.Value = "MHTML";
                parameters.Add(paramValue);

                // Include a link to the report in the email.
                paramValue = new ParameterValue();
                paramValue.Name = "IncludeReport";
                paramValue.Value = "true";
                parameters.Add(paramValue);

                //CC addresses
                if (String.IsNullOrEmpty(emailCcTextBox.Text.Trim()) == false)
                {
                    //emailAddresses = emailCcTextBox.Text.Trim().Split(';');

                    //foreach (string emailAddress in emailAddresses)
                    //{
                    //    if (String.IsNullOrEmpty(emailAddress))
                    //    {
                    //        continue;
                    //    }
                    //    paramValue = new ParameterValue();
                    //    paramValue.Name = "CC";
                    //    paramValue.Value = emailAddress.Trim();
                    //    parameters.Add(paramValue);
                    //}
                    strTo = emailCcTextBox.Text.Trim();
                    paramValue = new ParameterValue();
                    paramValue.Name = "CC";
                    paramValue.Value = strTo;
                    parameters.Add(paramValue);
                }

                //Reply To
                if (String.IsNullOrEmpty(emailReplyToTextBox.Text.Trim()) == false)
                {
                    paramValue = new ParameterValue();
                    paramValue.Name = "ReplyTo";
                    paramValue.Value = emailReplyToTextBox.Text.Trim();
                    parameters.Add(paramValue);
                }

                //Comment
                if (String.IsNullOrEmpty(emailCommentTextBox.Text) == false)
                {
                    paramValue = new ParameterValue();
                    paramValue.Name = "Comment";
                    paramValue.Value = emailCommentTextBox.Text;
                    parameters.Add(paramValue);
                }

                //Add all of the parameters
                int numParams = parameters.Count;
                paramValues = new ParameterValueOrFieldReference[numParams];

                for (int index = 0; index < parameters.Count; index++)
                {
                    paramValues[index] = parameters[index];
                }
                serverData.extnSettings.ParameterValues = paramValues;
                SetReportParameters(ref serverData);
                serverData.scheduleXML = CreateScheduleXml();
            }
        }

        private string CreateScheduleXml()
        {
            DateTime startTime = new DateTime(DateTime.Now.Year,
                                      DateTime.Now.Month,
                                      DateTime.Now.Day,
                                      emailStartTimePicker.Value.Hour,
                                      emailStartTimePicker.Value.Minute,
                                      emailStartTimePicker.Value.Second);

            ScheduleDefinition schedule = new ScheduleDefinition();
            schedule.StartDateTime = startTime.ToUniversalTime();
            schedule.EndDateSpecified = false;
            schedule.Item = GetPattern();
            XmlDocument xmlDoc = GetScheduleAsXml(schedule);
            return xmlDoc.OuterXml;

        }

        private RecurrencePattern GetPattern()
        {
            WeeklyRecurrence pattern = new WeeklyRecurrence();
            DaysOfWeekSelector days = new DaysOfWeekSelector();
            pattern.WeeksIntervalSpecified = true;
            pattern.WeeksInterval = 1;

            days.Sunday = emailSundayCheckbox.Checked;
            days.Monday = emailMondayCheckbox.Checked;
            days.Tuesday = emailTuesdayCheckbox.Checked;
            days.Wednesday = emailWednesdayCheckbox.Checked;
            days.Thursday = emailThursdayCheckbox.Checked;
            days.Friday = emailFridayCheckbox.Checked;
            days.Saturday = emailSaturdayCheckbox.Checked;
            pattern.DaysOfWeek = days;
            return pattern;
        }

        private XmlDocument GetScheduleAsXml(ScheduleDefinition schedule)
        {
            MemoryStream buffer = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScheduleDefinition));
            xmlSerializer.Serialize(buffer, schedule);
            buffer.Seek(0, SeekOrigin.Begin);

            XmlDocument document = new XmlDocument();
            document.Load(buffer);

            XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
            ns.AddNamespace("rs", "http://schemas.microsoft.com/sqlserver/2003/12/reporting/reportingservices");
            return document;
        }

        /// <summary>
        /// SetReportParameters. Sets the parameters that will be send to the report for the scheduled email.
        /// </summary>
        /// <param name="serverData"></param>
        private void SetReportParameters(ref ReportServerData serverData)
        {
            ParameterValue[] parameters = new ParameterValue[reportParameters.Count];
            int index = 0;

            foreach (KeyValuePair<string, object> reportParameter in reportParameters)
            {
                ParameterValue parameter = new ParameterValue();
                parameter.Name = reportParameter.Key;
                parameter.Value = reportParameter.Value != null ? string.Format("{0}", reportParameter.Value) : null;
                parameters[index++] = parameter;
            }
            serverData.reportParameters = parameters;
        }

        private void browseReportFolderButton_Click(object sender, EventArgs e)
        {
            ReportServerData serverData = new ReportServerData();
            serverData.reportServerURL = GetReportServerURL();

            BrowseReportServerFoldersDialog dialog = new BrowseReportServerFoldersDialog(serverData);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                reportFolderTextbox.Text = dialog.GetSelectedFolder();
                CheckforExistingDatasource(reportFolderTextbox.Text);
            }
        }

        private string GetReportServerURL()
        {
            string URL;

            //format the report server URL
            URL = reportServerComboBox.Text.Trim().ToLower();
            URL = URL.TrimEnd('/');

            if (URL.StartsWith("http"))
            {
                URL = String.Format("{0}/ReportService2005.asmx", URL);
            }
            else
            {
                URL = String.Format("http://{0}/ReportService2005.asmx", URL);
            }
            return URL;
        }

        private void reportSeverCombo_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(reportServerComboBox.Text))
            {
                browseReportFoldersButton.Enabled = false;
            }
            else
            {
                browseReportFoldersButton.Enabled = true;
            }
        }

        private void CheckforExistingDatasource(string reportFolder)
        {
            BackgroundWorker bgDSCheckWorker;
            ReportServerData serverData = new ReportServerData();
            serverData.reportServerURL = GetReportServerURL();
            serverData.reportFolder = reportFolder;

            bgDSCheckWorker = new BackgroundWorker();
            bgDSCheckWorker.DoWork += bgDSCheck_DoWork;
            bgDSCheckWorker.RunWorkerCompleted += bgDSCheck_RunWorkerCompleted;
            bgDSCheckWorker.RunWorkerAsync(serverData);
        }

        private bool DoesDatasourceExist(ReportServerData serverData)
        {
            if (serverData != null && reportParameters == null)
            {
                ReportingService2005 webService = new ReportingService2005();
                SearchCondition[] searchConditions = new SearchCondition[1];
                CatalogItem[] items;

                webService.UseDefaultCredentials = true;
                webService.Url = serverData.reportServerURL;
                SearchCondition searchCondition = new SearchCondition();
                searchCondition.Condition = ConditionEnum.Contains;
                searchCondition.ConditionSpecified = true;
                searchCondition.Name = "Name";
                searchCondition.Value = "SQL Diagnostic Manager Data Source";
                searchConditions[0] = searchCondition;
                string searchFolder = serverData.reportFolder;
                if (!searchFolder.StartsWith("/"))
                {
                    searchFolder = "/" + searchFolder;
                }
                items = webService.FindItems(searchFolder, BooleanOperatorEnum.And, searchConditions);

                foreach (CatalogItem item in items)
                {
                    if ((item.Type == ItemTypeEnum.DataSource) && (item.Name == "SQL Diagnostic Manager Data Source"))
                    {
                        // We found what we are looking for, just quit.  The background worker finish will set the use
                        // existing datasource radio to enabled if this one was not cancelled.
                        return true;
                    }
                }
            }

            return false;
        }

        private void bgDSCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Cancel = !DoesDatasourceExist(e.Argument as ReportServerData);
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void bgDSCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    useExistingDataSourceRadioButton.Enabled = true;

                    if (
                        ApplicationMessageBox.ShowQuestion(this,
                                                           "An existing data source was found in the specified folder. Would you like to use the existing data source?",
                                                           ExceptionMessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        useExistingDataSourceRadioButton.Checked = true;
                    }
                    else
                    {
                        createNewDataSourceRadioButton.Checked = true;
                    }
                }
            }
            else
            {
                useExistingDataSourceRadioButton.Enabled = false;
                createNewDataSourceRadioButton.Checked = true;
            }
        }

        private void reportFolderTextbox_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(reportFolderTextbox.Text))
            {
                useExistingDataSourceRadioButton.Checked = false;
                useExistingDataSourceRadioButton.Enabled = false;
                createNewDataSourceRadioButton.Checked = true;
            }
        }

        private void reportFolderTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            useExistingDataSourceRadioButton.Enabled = false;
            createNewDataSourceRadioButton.Checked = true;
        }

        private void reportServerComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            useExistingDataSourceRadioButton.Enabled = false;
            createNewDataSourceRadioButton.Checked = true;
        }

        private void DeployReportsWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = e != null;
            ShowHelp();
        }

        private void DeployReportsWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        private void ShowHelp()
        {
            string heplTopic = HelpTopics.ReportsDeploymentIntroduction;
            if (wizard.SelectedPage == selectReportsPage) heplTopic = HelpTopics.ReportsDeploymentSelectReports;
            if (wizard.SelectedPage == emailSubscriptionPage) heplTopic = HelpTopics.ReportsDeploymentScheduleEmail;
            if (wizard.SelectedPage == reportServerSettingsPage) heplTopic = HelpTopics.ReportsDeploymentSelectReportServer;
            if (wizard.SelectedPage == finishPage) heplTopic = HelpTopics.ReportsDeploymentSummary;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(heplTopic);
        }

        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void selectReportsPage_BeforeDisplay(object sender, EventArgs e)
        {

        }
    }

    public class ReportServerData
    {
        public string reportFolder;
        public CheckedListBox.CheckedItemCollection reports;
        public DataSourceDefinition dataSourceInfo;
        public string reportServerURL;
        public ExtensionSettings extnSettings;
        public string scheduleXML;
        public ParameterValue[] reportParameters;
        public CatalogItem[] reportFolders;

        public ReportServerData()
        {
            reportFolder = "";
            reports = null;
            dataSourceInfo = null;
            extnSettings = null;
            scheduleXML = null;
            reportFolders = null;
        }
    }
}