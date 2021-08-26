using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Collections;
    using System.Threading;
    using Common;
    using Helpers;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Configuration;

    public partial class TestInstanceCustomCountersDialog : Form
    {
        private MonitoredSqlServer instance;
        private MetricDefinitions metricDefinitions;
        private int[] metricList;
        private bool suppressSelectAllCheckChanged = false;

        private Dictionary<int, UltraDataRow> rowMap;

        public TestInstanceCustomCountersDialog(MonitoredSqlServer instance, MetricDefinitions metricDefinitions, int[] metricList)
        {
            this.metricDefinitions = metricDefinitions;
            this.metricList = metricList;
            this.instance = instance;

            InitializeComponent();

            ultraGrid1.DrawFilter = new HideFocusRectangleDrawFilter();
            AdaptFontSize();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            Triple<UltraDataRow,object,object> rowStatus = new Triple<UltraDataRow,object,object>();

            BackgroundWorker worker = (BackgroundWorker)sender;
            List<UltraDataRow> selectedRows = (List<UltraDataRow>)args.Argument;

            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);

            int sqlServerID = instance.Id;
            worker.ReportProgress(0, selectedRows);

            Dictionary<int, UltraDataRow> rowMap = new Dictionary<int, UltraDataRow>();

            List<CustomCounterConfiguration> configurationList = new List<CustomCounterConfiguration>();
            foreach (UltraDataRow row in selectedRows)
            {
                if (worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }

                int metricID = (int) row["MetricID"];
                // keep track of the row
                rowMap.Add(metricID, row);

                CustomCounterDefinition counterDefinition = metricDefinitions.GetCounterDefinition(metricID);
                if (counterDefinition != null && counterDefinition.CalculationType == CalculationType.Delta)
                    worker.ReportProgress(2, row);
                else
                    worker.ReportProgress(1, row);

                CustomCounterConfiguration configuration = new CustomCounterConfiguration(sqlServerID, metricID);
                configurationList.Add(configuration);            
            }

            try
            {
                int pass = 0;
                DateTime lastPass = DateTime.MinValue;
                while (configurationList.Count > 0 && pass < 2)
                {
                    if (worker.CancellationPending)
                    {
                        args.Cancel = true;
                        break;
                    }

                    TimeSpan timeSinceLastPass = DateTime.Now - lastPass;
                    if (timeSinceLastPass < TimeSpan.FromSeconds(2))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(2) - timeSinceLastPass);
                    }

                    CustomCounterCollectionSnapshot snapshot = managementService.GetCustomCounter(configurationList);
                    lastPass = DateTime.Now;

                    pass++;
                    configurationList.Clear();

                    if (snapshot.CollectionFailed)
                    {
                        rowStatus.Second = snapshot.Error;
                        rowStatus.Third = 0;
                        foreach (UltraDataRow row in selectedRows)
                        {
                            rowStatus.First = row;
                            worker.ReportProgress(10, rowStatus);
                        }
                        worker.ReportProgress(99, snapshot.Error);
                    }
                    else
                    {
                        foreach (CustomCounterSnapshot counterSnapshot in snapshot.CustomCounterList.Values)
                        {
                            int metricID = counterSnapshot.Definition.MetricID;
                            UltraDataRow row = rowMap[metricID];

                            object testResult = DBNull.Value;
                            object scaledTestResult = DBNull.Value;
                            try
                            {
                                if (counterSnapshot.CollectionFailed)
                                {
                                    testResult = counterSnapshot.Error;
                                }
                                else if (pass == 1 && counterSnapshot.Definition.CalculationType == CalculationType.Delta)
                                {
                                    // delta counters require 2 passes to be able to calculate a value
                                    configurationList.Add(
                                        new CustomCounterConfiguration(sqlServerID, counterSnapshot));
                                    worker.ReportProgress(3, row);
                                    worker.ReportProgress(11,
                                                          new Pair<UltraDataRow, TimeSpan?>(row,
                                                                                            counterSnapshot.RunTime));
                                    continue;
                                }
                                else if (counterSnapshot.DisplayValue.HasValue)
                                {
                                    testResult = counterSnapshot.DisplayValue.Value;
                                    try
                                    {
                                        scaledTestResult = ((decimal) testResult)*
                                                           Convert.ToDecimal(counterSnapshot.Definition.Scale);
                                    } catch (OverflowException oe)
                                    {
                                        scaledTestResult = new OverflowException("The value multiplied by the scaling factor resulted in a value that is larger than supported.", oe);
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
                            worker.ReportProgress(11, new Pair<UltraDataRow, TimeSpan?>(row, counterSnapshot.RunTime));
                        }
                    }
                }
            } catch (Exception e)
            {
                // exception affects all counters being collected in the list
                foreach (CustomCounterConfiguration counterConfig in configurationList)
                {
                    UltraDataRow row = rowMap[counterConfig.MetricID];
                    rowStatus.First = row;
                    rowStatus.Second = e;
                    worker.ReportProgress(10, rowStatus);
                    worker.ReportProgress(11, new Pair<UltraDataRow, TimeSpan?>(row, null));
                }
                worker.ReportProgress(99, e);
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
                    foreach (UltraDataRow row in counterGridDataSource.Rows)
                    {
                        row["Value"] = DBNull.Value;
                        row["ScaledValue"] = DBNull.Value;
                        row["Result"] = "";
                        row["Exception"] = DBNull.Value;
                        row["Duration"] = DBNull.Value;
                    }
                    foreach (UltraDataRow row in (ICollection)e.UserState)
                    {
                        row["Result"] = "Waiting";
                    }
                    break;
                case 1:
                    {
                        UltraDataRow udr = (UltraDataRow)e.UserState;
                        udr["Result"] = "Running (First Collection)";
                        break;
                    }
                case 2:
                    {
                        UltraDataRow udr = (UltraDataRow)e.UserState;
                        udr["Result"] = "Running (First Collection)";
                        break;
                    }
                case 3:
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
                        Pair<UltraDataRow,TimeSpan?> rowStatus = (Pair<UltraDataRow,TimeSpan?>)e.UserState;
                        if (rowStatus.Second.HasValue)
                            rowStatus.First["Duration"] = new DateTime(rowStatus.Second.Value.Ticks);
                        else
                            rowStatus.First["Duration"] = DBNull.Value;
                        break;
                    }
                case 99:
                    ApplicationMessageBox.ShowError(this,
                                                    "There was an error while trying to retrieve the custom counter values.  Resolve the error and try again.",
                                                    (Exception)e.UserState);
                    break;
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

        private void testButton_Click(object sender, EventArgs e)
        {
            testButton.Enabled = false;
            testButton.Visible = false;
            List<UltraDataRow> selectedInstances = GetSelectedRows();
            backgroundWorker.RunWorkerAsync(selectedInstances);
        }

        private List<UltraDataRow> GetSelectedRows()
        {
            List<UltraDataRow> selectedInstances = new List<UltraDataRow>();
            // build a list of counters to test 
            foreach (UltraDataRow row in counterGridDataSource.Rows)
            {
                bool enabled = (bool)row["Test"];
                if (enabled)
                {
                    selectedInstances.Add(row);
                }
            }
            return selectedInstances;
        }

        private void TestInstanceCustomCountersDialog_Load(object sender, EventArgs e)
        {
            Text = Text + " - " + instance.InstanceName;

            rowMap = new Dictionary<int, UltraDataRow>();
            foreach (int metricID in metricList)
            {
                MetricDescription? description = metricDefinitions.GetMetricDescription(metricID);
                UltraDataRow newRow = counterGridDataSource.Rows.Add(
                    new object[]
                        {
                            true,
                            metricID,
                            description.Value.Name,
                            DBNull.Value,
                            DBNull.Value
                        });

                rowMap.Add(metricID, newRow);
            }
            UpdateTestButton();
        }

        private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!suppressSelectAllCheckChanged)
            {
                bool selected = selectAllCheckBox.Checked;
                foreach (UltraDataRow row in counterGridDataSource.Rows)
                {
                    row["Test"] = selected;
                }
                UpdateTestButton();
            }
        }

        private void ultraGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement != null && selectedElement is CheckIndicatorUIElement && e.Button == MouseButtons.Left)
            {
                UltraGridRow gridRow = selectedElement.SelectableItem as UltraGridRow;
                if (gridRow != null)
                {
                    UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                    if (dataRow != null)
                    {
                        bool current = (bool)dataRow["Test"];
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
            if (backgroundWorker.IsBusy)
                return;

            bool testButtonEnabled = false;
            foreach (UltraDataRow row in counterGridDataSource.Rows)
            {
                if (true == (bool)row["Test"])
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

        private void TestInstanceCustomCountersDialog_FormClosing(object sender, FormClosingEventArgs e)
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
                foreach (UltraDataRow row in counterGridDataSource.Rows)
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

        private void TestInstanceCustomCountersDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CustomCountersTest);
        }

        private void TestInstanceCustomCountersDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
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