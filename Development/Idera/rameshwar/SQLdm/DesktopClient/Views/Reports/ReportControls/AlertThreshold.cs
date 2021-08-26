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

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
   public partial class AlertThresholds : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
   {
      private static readonly Logger Log = Logger.GetLogger("Alert Thresholds");
      private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

      public AlertThresholds()
      {
          InitializeComponent();
          AdaptFontSize();
      }

      protected override void InitInstanceCombo()
      {
         instanceCombo.Items.Clear();
         ValueListItem[] instances = null;
         ValueListItem instance;
         int instanceCount;

         int i = 0;
         int serverID = -1;

         // if the instance combo is not displayed, don't do any work.
         if (instanceCombo.Visible == false)
         {
            return;
         }

         if (drillThroughArguments != null)
         {
            IList<ReportParameter> paramsList = ((LocalReport)drillThroughArguments.Report).OriginalParametersToDrillthrough;

            try
            {
               serverID = Convert.ToInt32(paramsList[0].Values[0]);
            }
            catch
            {
               serverID = -1;
            }
         }
         instanceSelectOne = new ValueListItem(null, "< Select a Server >");
         instanceCombo.Items.Add(instanceSelectOne);
         currentServer = null;

         instanceCount = ApplicationModel.Default.ActiveInstances.Count;
         instances = new ValueListItem[instanceCount];
         i = 0;
         foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
         {
            instance = new ValueListItem(server, server.InstanceName);

            if ((serverID != -1) && (serverID == server.Id))
            {
               currentServer = server;
            }
            instances[i++] = instance;
         }

         instanceCombo.Items.AddRange(instances);
         //now re-select the one they had selected, if they did have one
         if (instanceCombo.SelectedItem == null && currentServer != null)
         {
            instanceCombo.SelectedIndex = instanceCombo.FindStringExact(currentServer.InstanceName);
         }
         else
         {
            instanceCombo.SelectedIndex = 0;
         }
      }

      public override void InitReport()
      {
         base.InitReport();
         InitializeReportViewer();
         sampleSizeCombo.Visible = false;
         sampleLabel.Visible = false;
         periodLabel.Visible = false;
         periodCombo.Visible = false;
         tagsComboBox.Visible = false;
         tagsLabel.Visible = false;
         rangeLabel.Visible = false;
         rangeInfoLabel.Visible = false;
         ReportType = ReportTypes.AlertThreshold;
         RunReport(true);
      }

      protected override void SetReportParameters()
      {
         base.SetReportParameters();

         reportParameters.Add("SqlServerID", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem.ToString());
      }

      public override bool CanRunReport(out string message)
      {
         message = String.Empty;

         if (instanceCombo.SelectedIndex == 0)
         {
            message = "A SQL Server instance must be selected to generate this report.";
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

         reportParameters.Add("SqlServerID", reportData.instanceID.ToString());
   
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
                passthroughParameters.Add(new ReportParameter("SqlServerID", (string)localReportData.reportParameters["SqlServerID"]));

                ReportDataSource dataSource = new ReportDataSource("AlertThreshold");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetAlertThreshold", localReportData.instanceID);
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
                         "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlertThreshold.rdl"))
                     {
                        reportViewer.LocalReport.LoadReportDefinition(stream);
                     }
                     foreach (ReportDataSource dataSource in reportData.dataSources)
                     {
                        reportViewer.LocalReport.DataSources.Add(dataSource);
                     }
                     reportViewer.LocalReport.SetParameters(passthroughParameters);
                     reportViewer.RefreshReport();
                     reportViewer.LocalReport.DisplayName = "AlertThreshold";
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

