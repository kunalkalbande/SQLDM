using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Events;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Dialogs;
using System.Threading;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.IO;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    /// <summary>
    /// SQLdm 10.0 (Swati Gogia) Import/Export Wizard screen. This represent the import export wizard screen
    /// </summary>
    internal partial class ImportExportView : View
    {
        public IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
        public ImportExportView()
        {
            InitializeComponent();
            ScaleControlsAsPerResolution();
        }

        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                this.splitContainer5.Size = new System.Drawing.Size(1600, 503);
                this.splitContainer5.SplitterDistance = this.splitContainer5.Size.Width/2;

                this.splitContainer4.Size = new System.Drawing.Size(1600, 303);
                this.splitContainer4.SplitterDistance = this.splitContainer1.Size.Width / 4;

                //this.splitContainer1.Size = new System.Drawing.Size(1600, 538);
                //this.splitContainer1.SplitterDistance = this.splitContainer1.Size.Width / 2;

                //this.splitContainer2.Size = new System.Drawing.Size(1600, 402);
                //this.splitContainer2.SplitterDistance = this.splitContainer1.Size.Width / 2;

                //this.splitContainer3.Size = new System.Drawing.Size(1600, 303);
                this.splitContainer3.SplitterDistance = 1300;


            }
        }


        private void btnCustomCounterImport_Click(object sender, EventArgs e)
        {
            List<Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>> newCounters = null;
            try
            {   /// Checking if any new counter got added or not -- SQLdm 10.0.0 (Ankit Nagpal)
                if (CustomCounterImportWizard.ImportNewCounter(this, out newCounters) == DialogResult.OK && newCounters != null && newCounters.Count!=0)                
                {
                    //foreach (var newCounter in newCounters)
                    //{
                    //    int metricID = newCounter.First.MetricID;

                    //    CustomCounter wrapper =
                    //        new CustomCounter(newCounter.First, newCounter.Second, newCounter.Third);

                    //    string question = "Would you like to link your counter to monitored SQL Server instances now?";

                    //    if (ApplicationMessageBox.ShowQuestion(this, question, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    //    {
                    //        ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object arg) { ApplicationModel.Default.RefreshMetricMetaData(); });
                    //        using (ApplyCustomCounterToServersDialog acctsd = new ApplyCustomCounterToServersDialog(metricID))
                    //        {
                    //            acctsd.ShowDialog(this);
                    //        }
                    //    }
                    //}
                    ApplicationMessageBox.ShowInfo(this, "Custom counters imported successfully.");

                }
            }
            catch (Exception ex)
            {
                Log.Error("Error occurred while importing custom counters : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Error occurred while importing Custom Counters.");
            }


        }

        private void btnCustomCounterExport_Click(object sender, EventArgs e)
        {
            MetricDefinitions metrics = new MetricDefinitions(true, false, true);
            metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            MetricDescription? description;
            List<CustomCounter> listCustomCounters = new List<CustomCounter>();
            string selectedDirectory = null;
            try
            {
                foreach (int metricID in metrics.GetCounterDefinitionKeys())
                {
                    description = metrics.GetMetricDescription(metricID);
                    if (description.HasValue)
                    {
                        CustomCounter customCounter = new CustomCounter(metrics.GetMetricDefinition(metricID), description.Value,
                                                    metrics.GetCounterDefinition(metricID));
                        listCustomCounters.Add(customCounter);
                    }
                }
                using (var sfd = new FolderBrowserDialog())
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        selectedDirectory = sfd.SelectedPath;
                        foreach (CustomCounter counter in listCustomCounters)
                        {
                            //CustomCounter counter = counterGrid.Selected.Rows[i].ListObject as CustomCounter;
                            string xml = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.SerializeCustomCounter(counter);
                            string fileName = counter.GetMetricDescription().Name + ".xml";
                            string fullFileName = Path.Combine(selectedDirectory, fileName);
                            File.WriteAllText(fullFileName, xml);
                        }
                        ApplicationMessageBox.ShowInfo(this, "Custom counters exported to selected directory");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error occurred while exporting custom counters : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Error occurred while exporting Custom Counters.");
            }
        }


        private void btnAlertTemplateImport_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = AlertTemplateImportWizard.ImportNewAlertTemplate(this);
                //Checking if any new template got created --SQLdm 10.0.0 (Ankit Nagpal) 
                if(dialogResult==DialogResult.OK)
                ApplicationMessageBox.ShowInfo(this, "Alert templates Imported to selected directory");
            }
            catch (Exception ex)
            {
                Log.Error("Error occurred while Importing alert templates : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Error occurred while importing alert templates.");
            }

        }

        private void btnAlertTemplateExport_Click(object sender, EventArgs e)
        {
            string selectedDirectory = null;
            List<AlertTemplate> alertTemplates = RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            using (var sfd = new FolderBrowserDialog())
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    selectedDirectory = sfd.SelectedPath;
                    foreach (AlertTemplate template in alertTemplates)
                    {
                        Idera.SQLdm.Common.Configuration.AlertConfiguration configuration = RepositoryHelper.GetDefaultAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, template.TemplateID);
                        AlertTemplateSerializable temp = AlertTemplateHelper.GetAlertTemplateSerializable(template.Name, template.Description, configuration);
                        string xml = AlertTemplateHelper.CreateXML(temp);
                        string fileName = template.Name + ".xml";
                        string fullFileName = Path.Combine(selectedDirectory, fileName);
                        try
                        {
                            File.WriteAllText(fullFileName, xml);

                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("Error occurred while exporting {} alert template. Error details : {1}", template.Name, ex.Message);
                            ApplicationMessageBox.ShowError(this, "Error occurred while exporting alert templates.");
                        }
                    }                     
                    ApplicationMessageBox.ShowInfo(this, "Alert templates exported successfully.");
                }
                
            }
            
        }

        private void btnAlertResponseImport_Click(object sender, EventArgs e)
        {
            List<NotificationRule> newRules = null;
            try
            {
                if (NotificationRuleImportWizard.ImportNewRule(this, out newRules) == DialogResult.OK && newRules != null)
                {
                    ApplicationMessageBox.ShowInfo(this, "Selected alert response imported successfully.");
                    
                }
                

            }
            catch (Exception ex)
            {
                Log.Error("Error occurred while importing Alert Responses : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                 ApplicationMessageBox.ShowError(this, "Import operation failed : ", ex);
            }
           

        }

        private void btnAlertResponseExport_Click(object sender, EventArgs e)
        {
            string selectedDirectory = null;
            try
            {
                using (var sfd = new FolderBrowserDialog())
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        selectedDirectory = sfd.SelectedPath;
                        foreach (NotificationRule rule in managementService.GetNotificationRules())
                        {
                            string xml = Idera.SQLdm.DesktopClient.Helpers.NotificationRuleHelper.SerializeNotificationRule(rule);
                            string fileName = rule.Description + ".xml";
                            string fullFileName = Path.Combine(selectedDirectory, fileName);
                            File.WriteAllText(fullFileName, xml);
                        }
                        ApplicationMessageBox.ShowInfo(this, "Alert Response exported to selected directory");

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Export Alert Responses operation failed : Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Export operation failed : ", ex);
            }
        }

        private void btnCustomReportImport_Click(object sender, EventArgs e)
        {
            try
            {
                Idera.SQLdm.Common.Objects.CustomReport report;
                DialogResult dialogResult = CustomReportImportWizard.ImportNewReport(this, out report);
                //Checking if report imported is not null  -- SQLdm 10.0.0(Ankit Nagpal)
                if(report!=null)
                ApplicationMessageBox.ShowInfo(this, "Custom report imported successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("Import Custom Report operation failed : Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Import Custom Report operation failed : ", ex);
            }
        }

        private void btnCustomReportExport_Click(object sender, EventArgs e)
        {
            List<CustomReport> customReports = RepositoryHelper.GetCustomReportsList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo).ToList();
            string selectedDirectory = null;
            string xml = null;
            try
            {
                using (var sfd = new FolderBrowserDialog())
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        selectedDirectory = sfd.SelectedPath;
                        foreach (CustomReport _CurrentCustomReport in customReports)
                        {
                            //includes aggregation that may have been set up previously
                            var _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                            //populate the counters that have already been selected
                            if (_CurrentCustomReport.Metrics == null) _CurrentCustomReport.Metrics = new SortedDictionary<int, CustomReportMetric>();

                            _CurrentCustomReport.Metrics.Clear();

                            foreach (DataRow row in _selectedCountersDataTable.Rows)
                            {
                                string metricName = row["CounterName"].ToString();
                                string metricDescription = row["CounterShortDescription"].ToString();

                                //_selectedCounters.Add(metricName, metricDescription);

                                Idera.SQLdm.Common.Objects.CustomReport.CounterType type = (Idera.SQLdm.Common.Objects.CustomReport.CounterType)int.Parse(row["CounterType"].ToString());
                                Idera.SQLdm.Common.Objects.CustomReport.Aggregation aggregation = (Idera.SQLdm.Common.Objects.CustomReport.Aggregation)int.Parse(row["Aggregation"].ToString());

                                _CurrentCustomReport.Metrics.Add(int.Parse(row["GraphNumber"].ToString()),
                                            new CustomReportMetric(metricName, metricDescription, type, aggregation));

                            }

                            xml = Idera.SQLdm.DesktopClient.Helpers.CustomReportHelper.SerializeCustomReport(_CurrentCustomReport);
                            string fileName = _CurrentCustomReport.Name + ".xml";
                            string fullFileName = Path.Combine(selectedDirectory, fileName);
                            File.WriteAllText(fullFileName, xml);

                        }
                    }
                    //Checking if valid custom report xml is getting exported or not --SQLdm 10.0.0(Ankit Nagpal)
                    if(xml!=null && xml!="")
                    ApplicationMessageBox.ShowInfo(this, "Custom reports exported successfully to selected directory.");
                }

            }
            catch (Exception ex)
            {
                Log.Error("Export Custom Reports operation failed : Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Export operation failed for Custom report: ", ex);
            }
        }


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.ImportExportHelp);
        }
       
    }
}
