using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.UI.Dialogs;
using System.IO;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents the import wizard for custom reports.
    /// </summary>
    public partial class CustomReportImportWizard : Form
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CustomCounterImportWizard");
        private WizardPage currentPage;
        private SqlConnectionInfo connectionInfo;
        private OpenFileDialog opeFileDialog;
        Idera.SQLdm.Common.Objects.CustomReport newCustomReport;
        private IManagementService managementService;
        bool isDuplicateFilename;
        bool isValidfileName;

        enum WizardPage
        {
            WelcomePage,
            ImportPage
        }
        public CustomReportImportWizard()
        {
            InitializeComponent();
            opeFileDialog = new OpenFileDialog();
            currentPage = WizardPage.WelcomePage;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.HelpButton = true;
            this.HelpButtonClicked += new CancelEventHandler(CustomReportImportWizard_HelpButtonClicked);
            chkOverwrite.Enabled = isDuplicateFilename;
            lblSelectedFile.Text = "Selected File : ";
            ultraTabControl1.CreationFilter = new HideTabHeaders();
            this.ultraTabControl1.Appearance.BackColor = Color.Transparent;
            this.ultraTabControl1.Appearance.BackColor2 = Color.Transparent;
            EnableDisableFinishAndBackButton();
        }

        void CustomReportImportWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.CustomReportsWizardImport);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            string currentPageName = currentPage.ToString();
            string nextBtnName = btnNext.Text;
            Cursor.Current = Cursors.WaitCursor;
            btnNext.Text = "Next";
            btnBack.Enabled = true;

            currentPage--;

            if (currentPage > WizardPage.ImportPage)
                this.Close();
            else
            {
                ultraTabControl1.Tabs[(int)currentPage].Selected = true;
                // UpdatePageView();
            }
            EnableDisableFinishAndBackButton();
            Cursor.Current = Cursors.Default;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            string currentPageName = currentPage.ToString();
            string nextBtnName = btnNext.Text;
            Cursor.Current = Cursors.WaitCursor;
            btnBack.Enabled = true;
            bool changePage = true;
            switch (currentPage)
            {
                case WizardPage.WelcomePage:
                    break;
                case WizardPage.ImportPage:
                    if (newCustomReport.ShowTopServers)
                        changePage = FinalizeCustomReportTopServer();
                    else
                        changePage = FinalizeCustomReport();
                    btnNext.Text = "Finish";
                    break;
            }
            if (changePage)
                currentPage++;
            if (currentPage > WizardPage.ImportPage)
                this.Close();
            else
            {
                ultraTabControl1.Tabs[(int)currentPage].Selected = true;
                // UpdatePageView();
            }
            EnableDisableFinishAndBackButton();
            Cursor.Current = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            newCustomReport = null;
            this.Close();
        }

        /// <summary>
        /// se this method to open the import custom counters dialogues
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static DialogResult ImportNewReport(IWin32Window owner, out Idera.SQLdm.Common.Objects.CustomReport report)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            CustomReportImportWizard dialog = null;
            try
            {
                dialog = new CustomReportImportWizard();
                dialog.Text = "Import Custom Report";
                dialogResult = dialog.ShowDialog(owner);

            }
            catch (Exception e)
            {
                if (dialog != null)
                {
                    dialog.Dispose();
                    dialog = null;
                }
                ApplicationMessageBox.ShowError(owner,
                                                "Error attempting to import custom counter.  Please resolve the error and try again.",
                                                e);
            }
            finally
            {
                if (dialog != null)
                    dialog.Dispose();
            }
            report = dialog.newCustomReport;
            return dialogResult;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            opeFileDialog = new OpenFileDialog();
            opeFileDialog.Filter = "xml files (*.xml)|*.xml";
            if (opeFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (opeFileDialog.CheckFileExists)
                {
                    try
                    {
                        CustomReportSerializable customReportSerializable = CustomReportHelper.DeserializeCustomReport(opeFileDialog.FileName);
                        newCustomReport = CustomReportHelper.GetCustomReport(customReportSerializable);
                        if (AddReport())
                            SetMessage("Imported custom report is valid. Click Finish to complete the import process and exit the Custom Report Import Wizard.", true);
                    }
                    catch
                    {
                        SetMessage("Could not parse the selected xml file into a valid custom report. Please select a valid custom report xml.", false);
                        isValidfileName = false;
                    }

                    lblSelectedFile.Text = "Selected File : " + opeFileDialog.FileName;
                    EnableDisableFinishAndBackButton();
                }
            }
        }

        private bool FinalizeCustomReport()
        {
            //int? previousServerID = null;

            //build the dataset block of the report
            string dataset = newCustomReport.GetCounterSummaryDataSet();

            string tableRDL = newCustomReport.GetCustomReportsTable();
            string chartsRDL = newCustomReport.GetCustomReportCharts();
            string showTableRDL = newCustomReport.GetShowTablesParameter(newCustomReport.ShowTable);

            string reportText = newCustomReport.ReportRDL;
            StringBuilder sb = new StringBuilder(reportText);

            //insert dataset block
            int startReplace = reportText.IndexOf("<DataSet Name=\"CounterSummary\">");
            if (startReplace <= 0) return true;
            int endReplace = reportText.IndexOf("</DataSet>", startReplace) + 10;
            if (endReplace <= 0) return true;
            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, dataset);
            reportText = sb.ToString();

            //Insert table block in the body of the report
            startReplace = reportText.IndexOf("<Table Name=\"table1\">");
            if (startReplace <= 0) return true;

            endReplace = reportText.IndexOf("</Table>", startReplace) + 8;
            if (endReplace <= 0) return true;

            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, tableRDL);
            reportText = sb.ToString();

            //Insert chart block
            startReplace = reportText.IndexOf("<Table Name=\"table2\">");
            if (startReplace <= 0) return true;

            endReplace = reportText.IndexOf("</Table>", startReplace) + 8;
            if (endReplace <= 0) return true;

            startReplace = endReplace + 1;
            endReplace = reportText.IndexOf("<Table Name=\"table1\">", startReplace) - 1;
            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, chartsRDL);
            reportText = sb.ToString();

            //Insert new DisplayTables parameter
            startReplace = reportText.IndexOf("<ReportParameter Name=\"DisplayTables\">");
            if (startReplace <= 0) return true;

            endReplace = reportText.IndexOf("</ReportParameter>", startReplace) + 18;
            if (endReplace <= 0) return true;

            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, showTableRDL);
            reportText = sb.ToString();

            Serialized<string> serialReportText = new Serialized<string>(reportText, true);

#if(DEBUG)
            UTF8Encoding encoding = new UTF8Encoding();

            FileStream fs = new FileStream(string.Format("c:\\LastCustomReportGenerated{0}.rdl", DateTime.Now.ToString("hhmmss")), FileMode.Create);
            fs.Write(encoding.GetBytes(reportText), 0, encoding.GetBytes(reportText).Length);
            fs.Flush();
            fs.Close();
#endif
            newCustomReport.ReportRDL = serialReportText;

            ApplicationModel.Default.AddOrUpdateCustomReport(newCustomReport);
            // get repository and management service interface
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
            //delete all counters for this report
            managementService.DeleteCustomReportCounters(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                newCustomReport.Name);

            bool isUpdateSucceessfull = UpdateExistingReport(newCustomReport);
            if (isUpdateSucceessfull)
                ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, newCustomReport.Name, null);
            return isUpdateSucceessfull;

        }
        //START: SQLdm 10.0 (Tarun Sapra)- Dynamically changing the rdl as per the selected counters
        /// <summary>
        /// Take the selected counters and add them to the top servers custom report object
        /// </summary>
        private bool FinalizeCustomReportTopServer()
        {
            string tablesRDL = newCustomReport.GetTopSeversCustomReportTables();
            string reportText = GetTopServersReportXML();
            reportText = System.Text.RegularExpressions.Regex.Replace(reportText, "{Custom Report Title}", newCustomReport.Name, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
            reportText = System.Text.RegularExpressions.Regex.Replace(reportText, "{Tables For Custom Counters}", tablesRDL, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
            Serialized<string> serialReportText = new Serialized<string>(reportText, true);

#if(DEBUG)
            UTF8Encoding encoding = new UTF8Encoding();

            FileStream fs = new FileStream(string.Format("c:\\LastTopServersCustomReportGenerated{0}.rdl", DateTime.Now.ToString("hhmmss")), FileMode.Create);
            fs.Write(encoding.GetBytes(reportText), 0, encoding.GetBytes(reportText).Length);
            fs.Flush();
            fs.Close();
#endif
            newCustomReport.ReportRDL = serialReportText;

            ApplicationModel.Default.AddOrUpdateCustomReport(newCustomReport);
            // get repository and management service interface
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);

            //delete all counters for this report
            managementService.DeleteCustomReportCounters(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                newCustomReport.Name);

            bool isUpdateSucceessfull = UpdateExistingReport(newCustomReport);
            if (isUpdateSucceessfull)
                ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, newCustomReport.Name, null);
            return isUpdateSucceessfull;
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Dynamically changing the rdl as per the selected counters
        
        private bool UpdateExistingReport(CustomReport report)
        {
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
            try
            {
                int rowNumber = 0;
                foreach (CustomReportMetric metric in report.Metrics.Values)
                {
                    managementService.InsertCounterToGraph(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                        report.Name,
                        rowNumber,
                        metric.MetricName,
                        metric.MetricDescription,
                        (int)metric.Aggregation,
                        (int)metric.Source);
                    rowNumber++;
                }


                return true;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(Parent, ex.Message);
            }
            return false;
        }

        private bool AddReport()
        {
            string reportName = newCustomReport.Name;
            isDuplicateFilename = false;
            isValidfileName = true;
            if (reportName.Length == 0)
            {
                SetMessage("Report name is blank.", false);
                isValidfileName = false;
                return false;
            }

            if (!CustomReportHelper.ReportNameIsValid(reportName))
            {
                SetMessage("Report name is invalid.", false);
                isValidfileName = false;
                return false;
            }
            string topServerReportXMLText = GetTopServersReportXML();
            if (ApplicationModel.Default.CustomReports.ContainsKey(reportName))
            {
                SetMessage("Report name is already in use. Please select the Replace existing report check box and click Finish to complete importing the new custom report.", false);
                isDuplicateFilename = true;
                isValidfileName = true;
                if (newCustomReport.ShowTopServers)
                {
                    newCustomReport.ReportRDL = System.Text.RegularExpressions.Regex.Replace(topServerReportXMLText, "{Custom Report Title}", reportName, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                }
                else
                {
                    newCustomReport.ReportRDL = GetCustomReportXML().Replace("{Custom Report Title}", reportName);
                }
                return false;
            }
            if (newCustomReport.ShowTopServers)
            {
                newCustomReport.ReportRDL = System.Text.RegularExpressions.Regex.Replace(topServerReportXMLText, "{Custom Report Title}", reportName, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
            }
            else
            {
                newCustomReport.ReportRDL = GetCustomReportXML().Replace("{Custom Report Title}", reportName);
            }

            return true;
        }

        private void SetMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            if (isSuccess)
                lblMessage.ForeColor = Color.Green;
            else
                lblMessage.ForeColor = Color.Red;
        }

        private string GetTopServersReportXML()
        {
            String rdlString = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CustomReportTopServers.rdl";

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(rdlString))
            {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string GetCustomReportXML()
        {
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                           "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CustomReport.rdl"))
            {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void chkOverwrite_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableFinishAndBackButton();
        }

        private void EnableDisableFinishAndBackButton()
        {
            btnNext.Enabled = true;
            btnBack.Enabled = true;
            btnNext.Text = "Next";
            if (currentPage == WizardPage.ImportPage)
            {
                btnNext.Text = "Finish";
                if (!isValidfileName)
                    btnNext.Enabled = false;
                if (isValidfileName && isDuplicateFilename && !chkOverwrite.Checked)
                    btnNext.Enabled = false;
                chkOverwrite.Enabled = isDuplicateFilename;
            }
            if (currentPage == WizardPage.WelcomePage)
                btnBack.Enabled = false;

        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            lblSelectedFile.Text = "Selected File : ";
            lblMessage.Text = string.Empty;
            newCustomReport = null;
            btnNext.Enabled = false;
            chkOverwrite.Enabled = chkOverwrite.Checked = false;
            isValidfileName = isDuplicateFilename = false;
        }
    }
}
