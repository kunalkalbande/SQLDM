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
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.Common.Thresholds;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents the import wizard for Alert Template.
    /// </summary>
    public partial class AlertTemplateImportWizard : Form
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AlertTemplateImportWizard");
        private WizardPage currentPage;
        private OpenFileDialog opeFileDialog;
        private AlertTemplate template;
        bool isDuplicateFilename;
        bool isValidfileName;
        private AlertTemplateSerializable alertTemplateSerializable;
        private List<string> existingTemplateNames = new List<string>();
        public bool templateCreated;
     
        enum WizardPage
        {
            WelcomePage,
            ImportPage
        }
        public AlertTemplateImportWizard()
        {
            InitializeComponent();
            opeFileDialog = new OpenFileDialog();
            currentPage = WizardPage.WelcomePage;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.HelpButton = true;
            this.HelpButtonClicked += new CancelEventHandler(AlertTemplateImportWizard_HelpButtonClicked);
            chkOverwrite.Enabled = isDuplicateFilename;
            lblSelectedFile.Text = "Selected File : ";
            ultraTabControl1.CreationFilter = new HideTabHeaders();
            this.ultraTabControl1.Appearance.BackColor = Color.Transparent;
            this.ultraTabControl1.Appearance.BackColor2 = Color.Transparent;
            EnableDisableFinishAndBackButton();
            foreach (AlertTemplate template in RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo))
            {
                existingTemplateNames.Add(template.Name);
            }
        }

        void AlertTemplateImportWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.AlertTemplatesWizardImport);
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
            btnNext.Text = "Next";
            btnBack.Enabled = true;
            switch (currentPage)
            {
                case WizardPage.WelcomePage:
                    break;
                case WizardPage.ImportPage:
                    CreateNewTemplate();
                    btnNext.Text = "Finish";
                    break;
            }
            currentPage++;
            if (currentPage > WizardPage.ImportPage)
                this.Close();
            else
            {
                ultraTabControl1.Tabs[(int)currentPage].Selected = true;
            }
            EnableDisableFinishAndBackButton();
            Cursor.Current = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// se this method to open the import alert template dialogues
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static DialogResult ImportNewAlertTemplate(IWin32Window owner)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            AlertTemplateImportWizard dialog = null;
            try
            {
                dialog = new AlertTemplateImportWizard();
                dialog.Text = "Import Alert Template";
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
                                                "Error attempting to import alert template.  Please resolve the error and try again.",
                                                e);
            }
            finally
            {
                if (dialog != null)
                    dialog.Dispose();
            }
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
                        alertTemplateSerializable = AlertTemplateHelper.DeserializeAlertTemplate(opeFileDialog.FileName);
                        if (IsTemplateNameAvailable(alertTemplateSerializable.Name))
                            SetMessage("Imported alert template is valid. Click Finish to complete the import process and exit the Alert Template Import Wizard.", true);
                        else
                            SetMessage("Alert template name is already in use. Please select the Replace existing alert template check box and click Finish to complete importing the new alert template.", false);
                    }
                    catch
                    {
                        SetMessage("Could not parse the selected XML file into a valid alert template. Please select a valid alert template XML.", false);
                        isValidfileName = false;
                    }
                  
                    lblSelectedFile.Text = "Selected File : " + opeFileDialog.FileName;
                    EnableDisableFinishAndBackButton();
                }
            }
        }

        private bool IsTemplateNameAvailable(string templateName)
        {
            isValidfileName = true;
            isDuplicateFilename = existingTemplateNames.Contains(templateName.Trim());
            return !isDuplicateFilename;
        }

        private void CreateNewTemplate()
        {
            try
            {
                if (!isDuplicateFilename)
                {
                    template = new AlertTemplate();
                    template.Name = alertTemplateSerializable.Name;
                    template.Description = alertTemplateSerializable.Description;
                    template.DefaultIndicator = false;
                    List<Wintellect.PowerCollections.Triple<string, int, string>> sources = RepositoryHelper.GetAlertThresholdOptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    int sourceInstance = sources[0].Second;
                    template.TemplateID =
                                     RepositoryHelper.AddAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                       0,
                                                                       sourceInstance,
                                                                       template.Name,
                                                                       template.Description);
                    templateCreated = true;
                }
                else
                {
                    template = RepositoryHelper.GetAlertTemplateList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo).Where(p => p.Name == alertTemplateSerializable.Name).FirstOrDefault();
                }
                LoadAlerts(template.TemplateID);
                var managementService =
                     ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                IList<MetricThresholdEntry> thresholdList = new List<MetricThresholdEntry>();

                Configuration.PrepareChangedItems();

                managementService.ChangeAlertTemplateConfiguration(Configuration);
            }
            catch (Exception ex)
            {
                if (templateCreated && template !=null)
                {
                    RepositoryHelper.DeleteAlertTemplate(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, template.TemplateID);
                }
                ApplicationMessageBox.ShowError(this,
                                              "Error attempting to import alert template.  Please resolve the error and try again.",
                                              ex);
            }
        }

        private void LoadAlerts(int instanceId)
        {
            try
            {
                Configuration = RepositoryHelper.GetDefaultAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);
                LoadAlertThresholdList();

                foreach (var configItemSerializable in alertTemplateSerializable.AlertConfugurationItems)
                {
                    var threshold = AlertThresholds.Where(p => p.MetricID == configItemSerializable.MetricId && p.MetricID < 1000).FirstOrDefault();
                    if (threshold != null)
                    {
                        threshold.ConfiguredAlertValue = configItemSerializable.ConfiguredAlertValue;
                        threshold.Enabled = configItemSerializable.Enabled;
                        threshold.ThresholdEntry.CriticalThreshold = Threshold.Deserialize(configItemSerializable.CriticalThreshold);
                        threshold.ThresholdEntry.InfoThreshold = Threshold.Deserialize(configItemSerializable.InfoThreshold);
                        threshold.ThresholdEntry.WarningThreshold = Threshold.Deserialize(configItemSerializable.WarningThreshold);

                        if (!string.IsNullOrEmpty(configItemSerializable.Data))
                        {
                            object data = Threshold.DeserializeData(configItemSerializable.Data);
                            if (!(data is AdvancedAlertConfigurationSettings))
                                data = new AdvancedAlertConfigurationSettings(Idera.SQLdm.Common.Events.MetricDefinition.GetMetric(configItemSerializable.MetricId), data);
                            threshold.SetData(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occurred while importing alert template {0}. Error Details : {1}", alertTemplateSerializable != null ? alertTemplateSerializable.Name : string.Empty, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        private void LoadAlertThresholdList()
        {
            AlertThresholds = new BindingList<AlertConfigurationItem>();
            if (AlertThresholds.Count > 0)
                AlertThresholds.Clear();

            foreach (AlertConfigurationItem item in Configuration.ItemList)
            {
                AlertThresholds.Add(item);
            }
        }
     
        private void SetMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            if (isSuccess)
                lblMessage.ForeColor = Color.Green;
            else
                lblMessage.ForeColor = Color.Red;
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
            alertTemplateSerializable = null;
            template = null;
            btnNext.Enabled = false;
            chkOverwrite.Enabled = chkOverwrite.Checked = false;
            isValidfileName = isDuplicateFilename = false;
        }

        public AlertConfiguration Configuration { get; set; }
        public BindingList<AlertConfigurationItem> AlertThresholds
        { get; set; }
    }
}
