using System.Linq;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using BBS.TracerX;
    using Common;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinGrid;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class TestCustomCounterDialog : Form
    {
        private static readonly Logger Log = Logger.GetLogger("TestCustomCounterDialog");
        private CustomCounterDefinition counterDefinition;
        private MetricDescription metricDescription;
        private bool suppressSelectAllCheckChanged = false;
        private bool useCachedDefinition = true;
        private List<IAzureResource> _azureResources;
        private IAzureProfile _profile;
        private const string ResourceLabelText = "Resource:";

        public TestCustomCounterDialog(CustomCounterDefinition counterDefinition, MetricDescription description, bool useCachedDefinition)
        {
            this.counterDefinition = counterDefinition;
            this.metricDescription = description;
            this.useCachedDefinition = useCachedDefinition;   

            InitializeComponent();

            ultraGrid1.DrawFilter = new HideFocusRectangleDrawFilter();
            this.AdaptFontSize();
        }

        public TestCustomCounterDialog(CustomCounterDefinition counterDefinition, MetricDescription description)
            : this(counterDefinition, description, true)
        {
        }

        private void ultraGrid1_InitializeLayout(object sender,
                                                 Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            Triple<UltraDataRow, object, object> rowStatus = new Triple<UltraDataRow, object, object>();

            BackgroundWorker worker = (BackgroundWorker) sender;
            List<UltraDataRow> selectedRows = (List<UltraDataRow>) args.Argument;

            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);

            worker.ReportProgress(0, selectedRows);
            foreach (UltraDataRow row in selectedRows)
            {
                if (worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }

                worker.ReportProgress(1, row);

                object testResult = DBNull.Value;
                object scaledTestResult = DBNull.Value;
                CustomCounterSnapshot snapshot = null;
                try
                {
                    var sqlServerId = counterDefinition.MetricType == MetricType.AzureCounter
                        ? _profile.SqlServerId
                        : ((MonitoredSqlServerWrapper) row["InstanceObject"]).Id;
                    var testConfiguration = new CustomCounterConfiguration(sqlServerId, counterDefinition);

                    snapshot = managementService.GetCustomCounter(testConfiguration);
                    if (counterDefinition.CalculationType == CalculationType.Delta && snapshot != null &&
                        !snapshot.CollectionFailed)
                    {
                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            break;
                        }

                        // delta type counters require another trip
                        worker.ReportProgress(2, row);
                        worker.ReportProgress(11, new Pair<UltraDataRow, TimeSpan?>(row, snapshot.RunTime));
                        testConfiguration = new CustomCounterConfiguration(sqlServerId, snapshot);
                        snapshot = managementService.GetCustomCounter(testConfiguration);
                    }

                    if (snapshot != null)
                    {
                        if (snapshot.CollectionFailed)
                        {
                            testResult = snapshot.Error;
                        }
                        else if (snapshot.DisplayValue.HasValue)
                        {
                            testResult = snapshot.DisplayValue.Value;
                            try
                            {
                                scaledTestResult =
                                    ((decimal) testResult) * Convert.ToDecimal(counterDefinition.Scale);
                            }
                            catch (OverflowException oe)
                            {
                                scaledTestResult = new OverflowException(
                                    "The value multiplied by the scaling factor resulted in a value that is larger than supported.",
                                    oe);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    testResult = e;
                }

                rowStatus.First = row;
                rowStatus.Second = testResult;
                rowStatus.Third = scaledTestResult;

                worker.ReportProgress(10, rowStatus);
                worker.ReportProgress(11, new Pair<UltraDataRow, TimeSpan?>(row, snapshot != null ? snapshot.RunTime : null));
            }
            worker.ReportProgress(100);

            if (worker.CancellationPending)
                args.Cancel = true;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null && worker.CancellationPending)
                return;

            switch (e.ProgressPercentage)
            {
                case 0:
                    // set initial status and result for all rows
                    foreach (UltraDataRow row in serverGridDataSource.Rows)
                    {
                        row["Value"] = DBNull.Value;
                        row["ScaledValue"] = DBNull.Value;
                        row["Result"] = "";
                        row["Exception"] = DBNull.Value;
                        row["Duration"] = DBNull.Value;
                    }
                    foreach (UltraDataRow row in (ICollection) e.UserState)
                    {
                        row["Result"] = "Waiting";
                    }
                    break;
                case 1:
                    {
                        UltraDataRow udr = (UltraDataRow) e.UserState;
                        if (counterDefinition.CalculationType == CalculationType.Value)
                            udr["Result"] = "Running";
                        else
                            udr["Result"] = "Running (First Collection)";
                        break;
                    }
                case 2:
                    {
                        UltraDataRow udr = (UltraDataRow)e.UserState;
                        udr["Result"] = "Running (Second Collection)";
                        break;
                    }
                case 10:
                    {
                        Triple<UltraDataRow, object, object> rowStatus =
                            (Triple<UltraDataRow, object, object>) e.UserState;
                        if (rowStatus.Second is Exception)
                        {
                            rowStatus.First["Result"] = ((Exception)rowStatus.Second).Message;
                            rowStatus.First["Exception"] = (Exception)rowStatus.Second;
                        }
                        else
                        {
                            rowStatus.First["Value"] = rowStatus.Second;
                            if (rowStatus.Third is Exception)
                            {
                                rowStatus.First["Result"] = ((Exception) rowStatus.Third).Message;
                                rowStatus.First["Exception"] = (Exception) rowStatus.Third;
                            }
                            else
                            {
                                rowStatus.First["ScaledValue"] = rowStatus.Third;
                                rowStatus.First["Result"] = "Success";
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        Pair<UltraDataRow, TimeSpan?> rowStatus = (Pair<UltraDataRow,TimeSpan?>)e.UserState;
                        if (rowStatus.Second.HasValue)
                            rowStatus.First["Duration"] = new DateTime(rowStatus.Second.Value.Ticks);
                        else
                            rowStatus.First["Duration"] = DBNull.Value;
                        break;
                    }
                case 100:
                    break;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            testButton.Visible = true;
            UpdateTestButton();
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
                return;
            }
        }

        private List<UltraDataRow> GetSelectedRows()
        {
            List<UltraDataRow> selectedInstances = new List<UltraDataRow>();
            // build a list of servers to test 
            foreach (UltraDataRow row in serverGridDataSource.Rows)
            {
                bool enabled = (bool) row["Test"];
                if (enabled)
                {
                    selectedInstances.Add(row);
                }
            }
            return selectedInstances;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            testButton.Enabled = false;
            testButton.Visible = false;
            List<UltraDataRow> selectedInstances = GetSelectedRows();
            backgroundWorker.RunWorkerAsync(selectedInstances);
        }

        private void TestCustomCounterDialog_Load(object sender, EventArgs e)
        {
            counterNameContentPanel.Visible = !String.IsNullOrEmpty(metricDescription.Name);
            metricNameLabel.Text = metricDescription.Name;

            switch (counterDefinition.MetricType)
            {
                case MetricType.SQLStatement:
                    contentStackLayoutPanel.ActiveControl = sqlBatchContentPanel;
                    sqlBatchLabel.Text = counterDefinition.SqlStatement;
                    break;
                case MetricType.AzureCounter:
                {
                    LoadAzureCustomCounter();
                    return;
                }
                default:
                {
                    contentStackLayoutPanel.ActiveControl = counterDefinitionContentPanel;
                    objectNameLabel.Text = counterDefinition.ObjectName;
                    counterNameLabel.Text = counterDefinition.CounterName;
                    if (counterDefinition.InstanceName.Trim().Length > 0)
                        instanceNameLabel.Text = counterDefinition.InstanceName;
                    break;
                }
            }

            Set<int> selectedServers = null;
                    
            if (counterDefinition.MetricID >= 0)
            {
                try
                {
                    selectedServers = RepositoryHelper.GetInstancesMonitoringCustomCounter(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counterDefinition.MetricID);
                } catch
                {
                    /* */
                }
            } 
            if (selectedServers == null)
                selectedServers = new Set<int>();

            bool allSelected = selectedServers.Count > 0;
            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                // DM 10.3 (Varun Chopra) SQLDM-28744 - CustomCounters for Non Linux Instances
                if (counterDefinition.MetricType == MetricType.WMI && instance.Instance.CloudProviderId == SQLdm.Common.Constants.LinuxId)
                {
                    continue;
                }
                bool selectItem = selectedServers.Contains(instance.Id);
                if (!selectItem && allSelected)
                    allSelected = false;

                UltraDataRow newRow = serverGridDataSource.Rows.Add(
                    new object[]
                        {
                            instance,
                            selectItem,
                            instance.InstanceName,
                            DBNull.Value,
                            DBNull.Value
                        });
            }
            selectAllCheckBox.Checked = allSelected;

            if (!useCachedDefinition)
                counterDefinition.MetricID = -1;

            UpdateTestButton();
        }

        private void LoadAzureCustomCounter()
        {
            descriptionLabel.Text =
                "Test the custom counter against one or more Azure resource provided by the Application Profile.";
            groupBox2.Text = "Select resource(s) to test the custom counter against:";
            ultraGrid1.DisplayLayout.Bands["Band 0"].Columns["Instance"].Header.Caption = "Resource";
            contentStackLayoutPanel.ActiveControl = counterDefinitionContentPanel;
            // Set the Resource label
            objNameLabel.Text = ResourceLabelText;
            objectNameLabel.Text = counterDefinition.ObjectName;
            // Set the counter label
            counterNameLabel.Text = counterDefinition.CounterName;
            if (counterDefinition.InstanceName.Trim().Length > 0)
            {
                instanceNameLabel.Text = counterDefinition.InstanceName;
            }

            // Collect Custom Counter Data using Metric name and resource URI
            var metricName = counterDefinition.SqlStatement;
            var resourceUri = counterDefinition.ServerType;
            // use the profile id
            long profileId = counterDefinition.ProfileId;

            // To get from the counter definition
            try
            {
                _profile = RepositoryHelper.GetAzureProfile(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, profileId,
                    resourceUri);
                if (_profile == null)
                {
                    return;
                }

                var monitorManagementConfiguration = new MonitorManagementConfiguration
                {
                    Profile = _profile
                };
                var managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection
                        .ConnectionInfo);
                _azureResources = managementService.GetAzureApplicationResources(monitorManagementConfiguration);
                if (_azureResources == null || _azureResources.Count == 0)
                {
                    return;
                }

                var metricResource = _profile.ApplicationProfile.Resources.FirstOrDefault();
                if (metricResource == null)
                {
                    return;
                }
                
                var allSelect = true;
                foreach (var resource in _azureResources.Where(r => r.Type == metricResource.Type))
                {
                    var isSelected = resource.Uri == metricResource.Uri;
                    if (allSelect && !isSelected)
                    {
                        allSelect = false;
                    }

                    serverGridDataSource.Rows.Add(
                        new object[]
                        {
                            resource,
                            isSelected,
                            resource.Name,
                            DBNull.Value,
                            DBNull.Value
                        });
                }
                selectAllCheckBox.Checked = allSelect;
                if (!useCachedDefinition)
                    counterDefinition.MetricID = -1;

                UpdateTestButton();
            }
            catch (Exception exception)
            {
                Log.Error("Problem in fetching the Custom Counter resource, returned the following error: {0}", exception);
            }
        }

        private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!suppressSelectAllCheckChanged)
            {
                foreach (UltraDataRow row in serverGridDataSource.Rows)
                {
                    row["Test"] = selectAllCheckBox.Checked;
                }
            }
            UpdateTestButton();
        }

        private void ultraGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement =
                ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement != null && selectedElement is CheckIndicatorUIElement && e.Button == MouseButtons.Left)
            {
                UltraGridRow gridRow = selectedElement.SelectableItem as UltraGridRow;
                if (gridRow != null)
                {
                    UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                    if (dataRow != null)
                    {
                        bool current = (bool) dataRow["Test"];
                        dataRow["Test"] = !current;
                    }
                }
                suppressSelectAllCheckChanged = true;
                selectAllCheckBox.Checked = false;
                suppressSelectAllCheckChanged = false;
                UpdateTestButton();
            }
        }

        private void UpdateTestButton()
        {
            // don't enable the test button if the background worker is running
            if (backgroundWorker != null && backgroundWorker.IsBusy)
                return;

            bool testButtonEnabled = false;
            foreach (UltraDataRow row in serverGridDataSource.Rows)
            {
                if (true == (bool) row["Test"])
                {
                    testButtonEnabled = true;
                    break;
                }
            }
            testButton.Enabled = testButtonEnabled;
        }

        private void ultraGrid1_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (e.Cell.Column.Key.Equals("Result"))
            {
                UltraDataRow dataRow = e.Cell.Row.ListObject as UltraDataRow;
                if (dataRow != null)
                {
                    Exception exception = dataRow["Exception"] as Exception;
                    if (exception != null)
                    {
                        ApplicationMessageBox.ShowError(this, "Error processing custom counter", exception);
                    }
                }
            }
        }

        private void TestCustomCounterDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker.IsBusy)
                {
                    backgroundWorker.CancelAsync();
                    // create a new background worker
                    backgroundWorker = new BackgroundWorker();
                    backgroundWorker.WorkerReportsProgress = true;
                    backgroundWorker.WorkerSupportsCancellation = true;
                    backgroundWorker.DoWork += this.backgroundWorker_DoWork;
                    backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
                    backgroundWorker.ProgressChanged += this.backgroundWorker_ProgressChanged;
                }
                foreach (UltraDataRow row in serverGridDataSource.Rows)
                {
                    string value = row["Result"] as string;
                    if (value == null || value.Equals("Waiting") || value.StartsWith("Running"))
                        row["Result"] = "Cancelled";
                }
            }
            finally
            {
                testButton.Visible = true;
                UpdateTestButton();
            }
        }

        private void TestCustomCounterDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CustomCountersTest);
        }

        private void TestCustomCounterDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CustomCountersTest);
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