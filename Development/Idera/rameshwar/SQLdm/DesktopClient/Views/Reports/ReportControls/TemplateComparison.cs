using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using System.Data;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TemplateComparison : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Template Comparison");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

        public TemplateComparison()
        {
            InitializeComponent();
            AdaptFontSize();
            InitTargetTemplate();
            InitSourceTemplate();
        }

        protected override void InitInstanceCombo()
        {
            InitSourceTemplate();
            InitTargetTemplate();
        }


        public override void InitReport()
        {
          //  string id;
          //  int serverID = -1;
          
            base.InitReport();
            tagsComboBox.Visible = false;
            InitializeReportViewer();

            sourceLabel.Visible = true;
            targetLabel.Visible = true;
            sourceCombo.Visible = true;
            targetCombo.Visible = true;
            sampleSizeCombo.Visible = false;
            sampleLabel.Visible = false;
            periodLabel.Visible = false;
            periodCombo.Visible = false;
            rangeLabel.Visible = false;
            rangeInfoLabel.Visible = false;
            instanceLabel.Visible = false;
            instanceCombo.Visible = false;
            ReportType = ReportTypes.TemplateComparison;
            RunReport(true);
        }


        protected override void SetReportParameters()
        {
            base.SetReportParameters();
           reportParameters.Add("SourceName", sourceCombo.SelectedItem == null ? "" : this.sourceCombo.GetItemText(this.sourceCombo.SelectedItem));
           reportParameters.Add("TargetName", targetCombo.SelectedItem == null ? "" : this.targetCombo.GetItemText(this.targetCombo.SelectedItem));

            if (sourceCombo.SelectedItem == null)
                reportParameters.Add("SourceTemplate", "");
            else
                reportParameters.Add("SourceTemplate", sourceCombo.SelectedValue == null ? "" : sourceCombo.SelectedValue.ToString());

            if (targetCombo.SelectedItem == null)
                reportParameters.Add("TargetTemplate", "");
            else
                reportParameters.Add("TargetTemplate", targetCombo.SelectedValue == null ? "" : targetCombo.SelectedValue.ToString());
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (sourceCombo.SelectedIndex == 0 && targetCombo.SelectedIndex == 0)
            {
                message = "A Source and Target template must be selected to generate this report.";
                return false;
            }
            else
            if (targetCombo.SelectedIndex == sourceCombo.SelectedIndex)
            {
                message = "A Source and Target template must be different to generate this report.";
                return false;
            }
            if (sourceCombo.SelectedIndex == 0)
            {
                message = "A Source template must be selected to generate this report.";
                return false;
            }
            if (targetCombo.SelectedIndex == 0)
            {
                message = "A Target template must be selected to generate this report.";
                return false;
            }

            return true;
        }
        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            reportParameters.Clear();

            reportParameters.Add("SourceTemplate", reportData.SrcTemplateId.ToString());
            reportParameters.Add("SourceName", reportData.SrcTemplateId.ToString());
            reportParameters.Add("TargetName", reportData.TrgTemplateId.ToString());
            reportParameters.Add("TargetTemplate", reportData.TrgTemplateId.ToString());
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem ??
                                             new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex > 0)
                reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());

            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                passthroughParameters.Clear();

                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("SourceName", (string)localReportData.reportParameters["SourceName"]));
                passthroughParameters.Add(new ReportParameter("TargetName", (string)localReportData.reportParameters["TargetName"]));
                ReportDataSource dataSource = new ReportDataSource("TemplateComparison");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTemplateComparison",localReportData.SrcTemplateId, localReportData.TrgTemplateId);
                localReportData.dataSources[0] = dataSource;

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TemplateComparison.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.LocalReport.SetParameters(passthroughParameters);
                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TemplateComparison";
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

