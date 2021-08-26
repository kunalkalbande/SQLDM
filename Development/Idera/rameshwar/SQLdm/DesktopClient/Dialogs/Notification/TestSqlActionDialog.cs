using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System.Collections;
    using BBS.TracerX;
    using Helpers;
    using Common.Configuration;
    using Common.Configuration.ServerActions;
    using Common.Notification.Providers;
    using Common.Services;
    using Common.Snapshots;
    using Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinGrid;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class TestSqlActionDialog : Form
    {
        private static readonly Logger Log = Logger.GetLogger("TestSqlActionDialog");
        private NotificationDestinationInfo destination;
        private bool suppressSelectAllCheckChanged = false;

        public TestSqlActionDialog()
        {
            InitializeComponent();

            ultraGrid1.DrawFilter = new HideFocusRectangleDrawFilter();

            // Auto scale font size.
            AdaptFontSize();
        }

        internal NotificationDestinationInfo Destination
        {
            get { return destination;  }
            set { destination = value; }
        }

        private void TestSqlActionDialog_Load(object sender, EventArgs e)
        {
            actionTypeLabel.Text = String.Empty;
            if (destination is JobDestination)
            {
                actionTypeLabel.Text = "SQL Agent Job";
                contentStackLayoutPanel.ActiveControl = counterDefinitionContentPanel;
                objectNameLabel.Text = ((JobDestination) destination).Job;
                counterNameLabel.Text = ((JobDestination) destination).Step;
                ultraGrid1.DisplayLayout.Bands[0].Columns["Duration"].Hidden = true;
            }
            else
            if (destination is SqlDestination)
            {
                actionTypeLabel.Text = "SQL Script";
                contentStackLayoutPanel.ActiveControl = sqlBatchContentPanel;
                descriptionLabel.Text = ((SqlDestination) destination).Description;
                sqlBatchLabel.Text = ((SqlDestination)destination).Sql;
            }

            string server = String.Empty;
            if (destination is JobDestination)
                server = ((JobDestination)destination).Server;
            else
            if (destination is SqlDestination)
                server = ((SqlDestination) destination).Server;

            bool allSelected = ApplicationModel.Default.ActiveInstances.Count > 0;
            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                bool selectItem = instance.InstanceName.Equals(server, StringComparison.CurrentCultureIgnoreCase);
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

            UpdateTestButton();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            testButton.Enabled = false;
            testButton.Visible = false;
            List<UltraDataRow> selectedInstances = GetSelectedRows();
            backgroundWorker.RunWorkerAsync(selectedInstances);
        }

        private void UpdateTestButton()
        {
            // don't enable the test button if the background worker is running
            if (backgroundWorker != null && backgroundWorker.IsBusy)
                return;

            bool testButtonEnabled = false;
            foreach (UltraDataRow row in serverGridDataSource.Rows)
            {
                if ((bool)row["Test"])
                {
                    testButtonEnabled = true;
                    break;
                }
            }
            testButton.Enabled = testButtonEnabled;
        }

        private void UpdateControls()
        {
            MonitoredSqlServer targetServer = null;

            testButton.Enabled = targetServer != null && !backgroundWorker.IsBusy;
        }

        private List<UltraDataRow> GetSelectedRows()
        {
            List<UltraDataRow> selectedInstances = new List<UltraDataRow>();
            // build a list of servers to test 
            foreach (UltraDataRow row in serverGridDataSource.Rows)
            {
                bool enabled = (bool)row["Test"];
                if (enabled)
                {
                    selectedInstances.Add(row);
                }
            }
            return selectedInstances;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            List<UltraDataRow> selectedRows = (List<UltraDataRow>)args.Argument;
            Pair<UltraDataRow, object> rowStatus = new Pair<UltraDataRow, object>();
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


                Snapshot snapshot = null;
                MonitoredSqlServerWrapper instance = (MonitoredSqlServerWrapper)row["InstanceObject"];
                object testResult = DBNull.Value;
                try
                {
                    if (destination is JobDestination)
                    {
                        JobControlConfiguration jcc = 
                            new JobControlConfiguration(instance.Id, ((JobDestination)destination).Job, ((JobDestination)destination).Step, JobControlAction.Start);

                        AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                        snapshot = managementService.SendJobControl(jcc);
                    }
                    else
                        if (destination is SqlDestination)
                        {
                            AdhocQueryConfiguration aqc =
                                new AdhocQueryConfiguration(instance.Id, ((SqlDestination)destination).Sql, false);
                            snapshot = managementService.SendAdhocQuery(aqc);
                        }
                        else
                            throw new ApplicationException(String.Format("Unknown action destination '{0}'",
                                                           destination.GetType().Name));

                    if (snapshot.Error != null)
                        testResult = snapshot.Error;
                    else
                        testResult = snapshot;
                }
                catch (Exception e)
                {
                    testResult = e;
                }
                rowStatus.First = row;
                rowStatus.Second = testResult;

                worker.ReportProgress(10, rowStatus);
                if (destination is SqlDestination && snapshot != null)
                    worker.ReportProgress(11, new Pair<UltraDataRow, TimeSpan?>(row, ((AdhocQuerySnapshot)snapshot).Duration));
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
                        udr["Result"] = "Running";                        
                        break;
                    }
                case 10:
                    {
                        Pair<UltraDataRow, object> rowStatus =
                            (Pair<UltraDataRow, object>)e.UserState;
                        if (rowStatus.Second is Exception)
                        {
                            rowStatus.First["Result"] = ((Exception)rowStatus.Second).Message;
                            rowStatus.First["Exception"] = (Exception)rowStatus.Second;
                        }
                        else
                        {
                            if (destination is SqlDestination)
                            {
                                rowStatus.First["Result"] = "Success";
                                AdhocQuerySnapshot aqs = rowStatus.Second as AdhocQuerySnapshot;
                                if (aqs != null)
                                {
                                    if (aqs.RowsAffected == null || aqs.RowsAffected.Value < 0)
                                        rowStatus.First["Result"] = String.Format("Success! {0} rowsets selected.", aqs.RowSetCount);
                                    else
                                        rowStatus.First["Result"] = String.Format("Success! {0} rows affected.", aqs.RowsAffected.Value);
                                }
                            }
                            else
                            if (destination is JobDestination)
                                rowStatus.First["Result"] = "Job successfully started";
                            else
                                rowStatus.First["Result"] = String.Format("{0}", rowStatus.Second);
                        }
                        break;
                    }
                case 11:
                    {
                        Pair<UltraDataRow, TimeSpan?> rowStatus = (Pair<UltraDataRow, TimeSpan?>)e.UserState;
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
                ApplicationMessageBox.ShowError(this, "Test failed", e.Error);
            }
        }

        private void TestSqlActionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
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

        private void doneButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}