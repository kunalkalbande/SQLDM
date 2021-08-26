using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerMonitorView : View
    {
        private readonly int instanceId;
        private static readonly object updateLock = new object();

        public ServerMonitorView(int instanceId)
        {
            this.instanceId = instanceId;
            InitializeComponent();
        }

        public override void RefreshView()
        {
            base.RefreshView();

            if (!refreshBackgroundWorker.IsBusy)
            {
                refreshBackgroundWorker.RunWorkerAsync();
            }
        }

        public override void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic("Servers_Monitor_Tab.html");
        }

        private void refreshBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ServerMonitorWorker";

            e.Result =
                RepositoryHelper.GetServerOverview(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                   instanceId);
        }

        private void refreshBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }
            else if (e.Error != null)
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, e.Error));
            }
            else
            {
                UpdateData(e.Result as DataTable);
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateData(DataTable dataTable)
        {
            lock (updateLock)
            {
                if (dataTable != null && dataTable.Rows.Count == 1)
                {
                    if (dataTable.Rows[0]["ServerVersion"] != DBNull.Value)
                    {
                        ServerVersion serverVersion = new ServerVersion(dataTable.Rows[0]["ServerVersion"] as string);
                        versionInformationLabel.Text = serverVersion + " (Build " + serverVersion.Version + ")";
                    }
                    else
                    {
                        versionInformationLabel.Text = "< Version Unknown >";
                    }

                    if (dataTable.Rows[0]["ResponseTimeInMilliseconds"] != DBNull.Value)
                    {
                        responseTimeButton.Text =
                            (Convert.ToInt32(dataTable.Rows[0]["ResponseTimeInMilliseconds"])).ToString();
                    }
                    else
                    {
                        responseTimeButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["ClientComputers"] != DBNull.Value)
                    {
                        clientComputersButton.Text = Convert.ToInt64(dataTable.Rows[0]["ClientComputers"]).ToString();
                    }
                    else
                    {
                        clientComputersButton.Text = "?";
                    }

                    int userProcesses = 0;
                    if (dataTable.Rows[0]["UserProcesses"] != DBNull.Value)
                    {
                        userProcesses = Convert.ToInt32(dataTable.Rows[0]["UserProcesses"]);
                        userProcessesButton.Text = userProcesses.ToString();
                    }
                    else
                    {
                        userProcessesButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["UserProcessesConsumingCPU"] != DBNull.Value)
                    {
                        activeUserProcessesButton.Text =
                            Convert.ToInt32(dataTable.Rows[0]["UserProcessesConsumingCPU"]).ToString();
                    }
                    else
                    {
                        activeUserProcessesButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["SystemProcesses"] != DBNull.Value)
                    {
                        int totalProcesses = (Convert.ToInt32(dataTable.Rows[0]["SystemProcesses"])) + userProcesses;
                        sqlProcessesButton.Text = totalProcesses.ToString();
                    }
                    else
                    {
                        sqlProcessesButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["BlockedProcesses"] != DBNull.Value)
                    {
                        blockedProcessesButton.Text =
                            (Convert.ToInt32(dataTable.Rows[0]["BlockedProcesses"])).ToString();
                    }
                    else
                    {
                        blockedProcessesButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["DatabaseCount"] != DBNull.Value)
                    {
                        databasesButton.Text = (Convert.ToInt32(dataTable.Rows[0]["DatabaseCount"])).ToString();
                    }
                    else
                    {
                        databasesButton.Text = "?";
                    }

                    if (dataTable.Rows[0]["PacketsReceivedPerSecond"] != DBNull.Value)
                    {
                        double packetsReceivedPerSecond =
                            Math.Round((double)dataTable.Rows[0]["PacketsReceivedPerSecond"], 2);
                        packetsReceivedPerSecondLabel.Text = string.Format("{0:F2} Pkt/s", packetsReceivedPerSecond);
                        packetsReceivedPerSecondFlowControl.Value = packetsReceivedPerSecond;
                    }
                    else
                    {
                        packetsReceivedPerSecondFlowControl.Value = 0;
                        packetsReceivedPerSecondLabel.Text = "? Pkt/s";
                    }

                    if (dataTable.Rows[0]["PacketsSentPerSecond"] != DBNull.Value)
                    {
                        double packetsSentPerSecond =
                            Math.Round((double)dataTable.Rows[0]["PacketsReceivedPerSecond"], 2);
                        packetsSentPerSecondLabel.Text =
                            string.Format("{0:F2} Pkt/s", packetsSentPerSecond);
                        packetsSentPerSecondFlowControl.Value = packetsSentPerSecond;
                    }
                    else
                    {
                        packetsSentPerSecondFlowControl.Value = 0;
                        packetsReceivedPerSecondLabel.Text = "? Pkt/s";
                    }

                    if (dataTable.Rows[0]["TransactionsPerSecond"] != DBNull.Value)
                    {
                        double transactionsPerSecond =
                            Math.Round((double)dataTable.Rows[0]["TransactionsPerSecond"], 2);
                        transactionsPerSecondLabel.Text =
                            string.Format("{0:F2} Tran/s", transactionsPerSecond);
                        transactionsPerSecondFlowControl.Value = transactionsPerSecond;
                    }
                    else
                    {
                        transactionsPerSecondFlowControl.Value = 0;
                        transactionsPerSecondFlowControl.Text = "? Tran/s";
                    }

                    if (dataTable.Rows[0]["ProcessorQueueLength"] != DBNull.Value)
                    {
                        double processorQueueLength = (double) dataTable.Rows[0]["ProcessorQueueLength"];
                        processorQueueLengthStatusBar.Value = processorQueueLength < 5
                                                                  ? Convert.ToInt32(processorQueueLength)
                                                                  : 5;
                    }
                    else
                    {
                        processorQueueLengthStatusBar.Value = 0;
                    }

                    if (dataTable.Rows[0]["CPUActivityPercentage"] != DBNull.Value)
                    {
                        double processorUsagePercentage = (double) dataTable.Rows[0]["CPUActivityPercentage"];
                        processorUsageValueLabel.Text = string.Format("{0:F0}%", processorUsagePercentage);
                        processorUsageFlowControl.Value = processorUsagePercentage;
                    }
                    else
                    {
                        processorUsageValueLabel.Text = "?";
                    }

                    if (dataTable.Rows[0]["PageWritesPerSecond"] != DBNull.Value)
                    {
                        double pageWritesPerSecond = Math.Round((double)dataTable.Rows[0]["PageWritesPerSecond"], 2);

                        pageWritesPerSecondFlowControl.Value = pageWritesPerSecond;
                        pageWritesPerSecondLabel.Text = string.Format("{0:F2} W/s", pageWritesPerSecond);

                        pageWritesPerSecondFlowControl2.Value = pageWritesPerSecond;
                        pageWritesPerSecondLabel2.Text = string.Format("{0:F2} W/s", pageWritesPerSecond);
                    }
                    else
                    {
                        pageWritesPerSecondFlowControl.Value = 0;
                        pageWritesPerSecondLabel.Text = "? W/s";

                        pageWritesPerSecondFlowControl2.Value = 0;
                        pageWritesPerSecondLabel2.Text = "? W/s";
                    }

                    if (dataTable.Rows[0]["PageReadsPerSecond"] != DBNull.Value)
                    {
                        double pageReadsPerSecond = Math.Round((double)dataTable.Rows[0]["PageReadsPerSecond"], 2);

                        pageReadsPerSecondFlowControl.Value = pageReadsPerSecond;
                        pageReadsPerSecondLabel.Text = string.Format("{0:F2} R/s", pageReadsPerSecond);

                        pageReadsPerSecondFlowControl2.Value = pageReadsPerSecond;
                        pageReadsPerSecondLabel2.Text = string.Format("{0:F2} R/s", pageReadsPerSecond);
                    }
                    else
                    {
                        pageReadsPerSecondFlowControl.Value = 0;
                        pageReadsPerSecondLabel.Text = "? R/s";

                        pageReadsPerSecondFlowControl2.Value = 0;
                        pageReadsPerSecondLabel2.Text = "? R/s";
                    }

                    if (dataTable.Rows[0]["SqlCompilationsPerSecond"] != DBNull.Value)
                    {
                        double sqlCompilationsPerSecond =
                            Math.Round((double)dataTable.Rows[0]["SqlCompilationsPerSecond"], 2);

                        sqlCompilationsPerSecondFlowControl.Value = sqlCompilationsPerSecond;
                        sqlCompilationsPerSecondLabel.Text = string.Format("{0:F2} Cmp/s", sqlCompilationsPerSecond);
                    }
                    else
                    {
                        sqlCompilationsPerSecondFlowControl.Value = 0;
                        sqlCompilationsPerSecondLabel.Text = "? Cmp/s";
                    }

                    if (dataTable.Rows[0]["LogFlushesPerSecond"] != DBNull.Value)
                    {
                        double logFlushesPerSecond = Math.Round((double)dataTable.Rows[0]["LogFlushesPerSecond"], 2);
                        logFlushesPerSecondFlowControl.Value = logFlushesPerSecond;
                        logFlushesPerSecondLabel.Text = string.Format("{0:F2} Flush/s", logFlushesPerSecond);
                    }
                    else
                    {
                        logFlushesPerSecondFlowControl.Value = 0;
                        logFlushesPerSecondLabel.Text = "? Flush/s";
                    }

                    if (dataTable.Rows[0]["SqlMemoryAllocatedInKilobytes"] != DBNull.Value &&
                        dataTable.Rows[0]["SqlMemoryUsedInKilobytes"] != DBNull.Value)
                    {
                        double memoryAllocatedMB =
                            ((double) Convert.ToInt64(dataTable.Rows[0]["SqlMemoryAllocatedInKilobytes"]))/1024;
                        double memoryUsedMB = ((double) Convert.ToInt64(dataTable.Rows[0]["SqlMemoryUsedInKilobytes"]))/
                                              1024;
                        sqlMemoryUsageStatusBar.Maximum = Convert.ToInt32(memoryAllocatedMB);
                        sqlMemoryUsageStatusBar.Value = memoryUsedMB < memoryAllocatedMB
                                                            ? Convert.ToInt32(memoryUsedMB)
                                                            : Convert.ToInt32(memoryAllocatedMB);
                        sqlMemoryAllocatedLabel.Text = string.Format("{0:F2} MB", memoryAllocatedMB);
                    }

                    if (dataTable.Rows[dataTable.Rows.Count - 1]["BufferCacheSizeInKilobytes"] != DBNull.Value)
                    {
                        double bufferCacheSizeMB =
                            ((double) Convert.ToInt64(dataTable.Rows[0]["BufferCacheSizeInKilobytes"]))/1024;
                        bufferCacheSizeLabel.Text = string.Format("Size: {0:F1} MB", bufferCacheSizeMB);
                    }
                    else
                    {
                        bufferCacheSizeLabel.Text = string.Format("Size: ?");
                    }

                    if (dataTable.Rows[dataTable.Rows.Count - 1]["BufferCacheHitRatioPercentage"] != DBNull.Value)
                    {
                        int bufferCacheHitRate =
                            Convert.ToInt32(Math.Truncate((double) dataTable.Rows[0]["BufferCacheHitRatioPercentage"]));

                        bufferCacheHitRateStatusBar.Value = bufferCacheHitRate < 100 ? bufferCacheHitRate : 100;
                    }
                    else
                    {
                        bufferCacheHitRateStatusBar.Value = 0;
                    }

                    if (dataTable.Rows[dataTable.Rows.Count - 1]["ProcedureCacheSizeInKilobytes"] != DBNull.Value)
                    {
                        double procedureCacheSizeMB =
                            ((double) Convert.ToInt64(dataTable.Rows[0]["ProcedureCacheSizeInKilobytes"]))/1024;
                        procedureCacheSizeLabel.Text = string.Format("Size: {0:F1} MB", procedureCacheSizeMB);
                    }
                    else
                    {
                        procedureCacheSizeLabel.Text = string.Format("Size: ?");
                    }

                    if (dataTable.Rows[dataTable.Rows.Count - 1]["ProcedureCacheHitRatioPercentage"] != DBNull.Value)
                    {
                        int procedureCacheHitRate = Convert.ToInt32(
                                Math.Truncate((double)dataTable.Rows[0]["ProcedureCacheHitRatioPercentage"]));

                        procedureCacheHitRateStatusBar.Value = procedureCacheHitRate < 100 ? procedureCacheHitRate : 100;
                    }
                    else
                    {
                        procedureCacheHitRateStatusBar.Value = 0;
                    }

                    if (dataTable.Rows[0]["PagesPerSecond"] != DBNull.Value)
                    {
                        double pagesPerSecond = (double) dataTable.Rows[0]["PagesPerSecond"];
                        pagingValueLabel.Text = string.Format("{0:F0}/s", pagesPerSecond);
                        pagingFlowControl.Value = pagesPerSecond;
                    }
                    else
                    {
                        pagingValueLabel.Text = "?";
                    }

                    if (dataTable.Rows[0]["DataFileCount"] != DBNull.Value)
                    {
                        dataFilesCountValueLabel.Text = ((int) dataTable.Rows[0]["DataFileCount"]).ToString();
                    }
                    else
                    {
                        dataFilesCountValueLabel.Text = "?";
                    }

                    if (dataTable.Rows[0]["DataFileSpaceAllocatedInKilobytes"] != DBNull.Value &&
                        dataTable.Rows[0]["DataFileSpaceUsedInKilobytes"] != DBNull.Value)
                    {
                        decimal dataFileSpaceAllocatedInMB =
                            ((decimal) dataTable.Rows[0]["DataFileSpaceAllocatedInKilobytes"])/1024;
                        decimal dataFileSpaceUsedInMB = ((decimal) dataTable.Rows[0]["DataFileSpaceUsedInKilobytes"])/
                                                        1024;

                        dataFilesSizeValueLabel.Text = string.Format("{0:F0} MB", dataFileSpaceUsedInMB);
                        dataFilesPercentFullStatusBar.Maximum = Convert.ToInt32(dataFileSpaceAllocatedInMB);
                        dataFilesPercentFullStatusBar.Value = dataFileSpaceUsedInMB < dataFileSpaceAllocatedInMB
                                                                  ? Convert.ToInt32(dataFileSpaceUsedInMB)
                                                                  : Convert.ToInt32(dataFileSpaceAllocatedInMB);
                    }
                    else
                    {
                        dataFilesSizeValueLabel.Text = "?";
                        dataFilesPercentFullStatusBar.Value = 0;
                    }

                    if (dataTable.Rows[0]["LogFileCount"] != DBNull.Value)
                    {
                        logFilesCountValueLabel.Text = ((int) dataTable.Rows[0]["LogFileCount"]).ToString();
                    }
                    else
                    {
                        logFilesCountValueLabel.Text = "?";
                    }

                    if (dataTable.Rows[0]["LogFileSpaceAllocatedInKilobytes"] != DBNull.Value &&
                        dataTable.Rows[0]["LogFileSpaceUsedInKilobytes"] != DBNull.Value)
                    {
                        decimal logFileSpaceAllocatedInMB =
                            ((decimal) dataTable.Rows[0]["LogFileSpaceAllocatedInKilobytes"])/1024;
                        decimal logFileSpaceUsedInMB = ((decimal) dataTable.Rows[0]["LogFileSpaceUsedInKilobytes"])/1024;

                        logFilesSizeValueLabel.Text = string.Format("{0:F0} MB", logFileSpaceUsedInMB);
                        logFilesPercentFullStatusBar.Maximum = Convert.ToInt32(logFileSpaceAllocatedInMB);
                        logFilesPercentFullStatusBar.Value = logFileSpaceUsedInMB < logFileSpaceAllocatedInMB
                                                                 ? Convert.ToInt32(logFileSpaceUsedInMB)
                                                                 : Convert.ToInt32(logFileSpaceAllocatedInMB);
                    }
                    else
                    {
                        logFilesSizeValueLabel.Text = "?";
                        logFilesPercentFullStatusBar.Value = 0;
                    }

                    if (dataTable.Rows[0]["DiskQueueLength"] != DBNull.Value)
                    {
                        double diskQueueLength = (double) dataTable.Rows[0]["DiskQueueLength"];
                        diskQueueLengthStatusBar.Value = diskQueueLength < 5
                                                             ? Convert.ToInt32(diskQueueLength)
                                                             : 5;
                    }
                    else
                    {
                        diskQueueLengthStatusBar.Value = 0;
                    }
                }
            }
        }
    }
}