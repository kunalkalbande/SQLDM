using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class MirroringOverview : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Mirroring Overview");

        public MirroringOverview()
        {
            InitializeComponent();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            periodCombo.Visible = false;
            periodLabel.Visible = false;
            instanceCombo.Visible = false;
            instanceLabel.Visible = false;
            sampleSizeCombo.Visible = false;
            sampleLabel.Visible = false;
            tagsLabel.Hide();
            tagsComboBox.Hide();
            //chkShowProblemsOnly.Left = tagsLabel.Left;

            clearFilterButton.Visible = false;
            ReportType = ReportTypes.MirroringOverview;

            if (EnableRunReport())
            {
                RunReport();
            }
        }
        protected override void Initialize_bgWorker()
        {
            using (Log.DebugCall())
            {
                // Set the report filter data
                State = UIState.GettingData;
                reportData = new WorkerData();
                reportData.reportViewer = reportViewer;
                reportData.reportType = ReportTypes.MirroringOverview;

                if ((instanceCombo.Visible) && (instanceCombo.SelectedItem != null) && ((ValueListItem)instanceCombo.SelectedItem).DataValue != null)
                {
                    reportData.instanceID = ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id;
                }
                if (periodCombo.SelectedItem == null)
                {
                    periodCombo.SelectedIndex = 0;
                }
                //set the Tag server IDs to null
                reportData.serverIDs = null;

                // if there is a tag selected, get the server IDs.
                if ((tagsComboBox.Visible) && (tagsComboBox.SelectedItem != null))
                {
                    if (((ValueListItem)tagsComboBox.SelectedItem).DataValue != null)
                    {
                        reportData.serverIDs = RepositoryHelper.GetServersWithTag(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                                  ((int)((ValueListItem)tagsComboBox.SelectedItem).DataValue));
                    }
                }
                reportData.periodType = (PeriodType)((ValueListItem)periodCombo.SelectedItem).DataValue;
                reportData.sampleSize = (SampleSize)((ValueListItem)sampleSizeCombo.SelectedItem).DataValue;

                //reportData.Parameters = chkShowProblemsOnly.Checked;

                SetDateRanges(reportData);

                //Create the background thread and start it.
                reportData.bgWorker = new BackgroundWorker();
                reportData.bgWorker.WorkerSupportsCancellation = true;
                reportData.bgWorker.DoWork += bgWorker_DoWork;
                reportData.bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
                reportData.bgWorker.RunWorkerAsync(reportData);
            }            
        }
        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                bool blnProblemsOnly = (bool) localReportData.Parameters;

                ReportDataSource dataSource;// = new ReportDataSource("mirroringCurrentStatus");;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[2];

                dataSource = new ReportDataSource("mirroringCurrentStatus");
                if (localReportData.serverIDs != null)
                {
                    dataSource.Value =
                        RepositoryHelper.GetReportData("p_GetMirroringCurrentStatus",
                                                       GetServerIdXml(localReportData.serverIDs), blnProblemsOnly);
                }
                else
                {
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetMirroringCurrentStatus", blnProblemsOnly);
                }

                localReportData.dataSources[0] = dataSource;
                
                dataSource = new ReportDataSource("mirroringServerConfiguration");
                if (localReportData.serverIDs != null)
                {
                    dataSource.Value =
                        RepositoryHelper.GetReportData("p_GetMirroringServerConfiguration",
                                                       GetServerIdXml(localReportData.serverIDs), blnProblemsOnly);
                }
                else
                {
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetMirroringServerConfiguration", blnProblemsOnly);
                }

                localReportData.dataSources[1] = dataSource;

                if (localReportData.cancelled)
                {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    e.Result = localReportData;
                }
            }
        }

        override protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (Log.DebugCall())
            {
                // Make sure this call is for the most recently requested report.
                Log.Debug("(reportData.bgWorker == sender) = ", reportData.bgWorker == sender);
                if (reportData.bgWorker == sender)
                {
                    // This event handler was called by the currently active report
                    if (reportData.cancelled)
                    {
                        Log.Debug("reportData.cancelled = true.");
                        return;
                    }
                    else if (e.Error != null)
                    {
                        if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                            {
                                Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(this, "An error occurred while retrieving data for the report.  ",
                                            e.Error);
                        }

                        State = UIState.NoDataAcquired;
                    }
                    else
                    {
                        try
                        {
                            reportViewer.Reset();
                            reportViewer.LocalReport.EnableHyperlinks = true;

                            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MirroringOverview.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.RefreshReport();
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(ParentForm, "An error occurred while refreshing the report.", exception);
                            State = UIState.ParmsNeeded;
                        }
                    }
                }
            }
        }
    }
}

