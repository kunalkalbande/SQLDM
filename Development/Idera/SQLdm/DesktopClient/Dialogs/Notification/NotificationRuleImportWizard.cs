using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    enum WizardPage
    {
        WelcomePage,
        ImportPage,
        FinishPage
    }
    /// <summary>
    /// SQLdm 10.0 (Swati Gogia) Import/Export Wizard - This class represents the import wizard for notification rules.
    /// </summary>
    public partial class NotificationRuleImportWizard : Form
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NotificationRuleImportWizard");
        private WizardPage currentPage;     
        private List<NotificationRule> notificationRuleUpdated;
        private List<Triple<string, string, int>> addedFiles;
        private int fileId = 1;
        List<NotificationRule> lstNotificationRuleSerialisable;
        Dictionary<string, string> lstErrors;
        private string errorMessage;
        public NotificationRuleImportWizard()
        {
            InitializeComponent();            
            currentPage = WizardPage.WelcomePage;
            addedFiles = new List<Triple<string, string, int>>();
            lstNotificationRuleSerialisable = new List<NotificationRule>();
            lstErrors = new Dictionary<string, string>();
            notificationRuleUpdated = new List<NotificationRule>();
            btnBack.Enabled = false;
            ultraTabControl1.CreationFilter = new HideTabHeaders();
            this.ultraTabControl1.Appearance.BackColor = Color.Transparent;
            this.ultraTabControl1.Appearance.BackColor2 = Color.Transparent;
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
                    ImportFiles();
                    UpdateFinishPage();
                    btnNext.Text = "Finish";
                    break;
                case WizardPage.FinishPage:
                    AddRules();
                    this.Close();
                    break;
            }
            currentPage++;
            if (currentPage > WizardPage.FinishPage)
                this.Close();
            else
            {
                ultraTabControl1.Tabs[(int)currentPage].Selected = true;
                // UpdatePageView();
            }
            EnableDisableImportNextButton();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateFinishPage()
        {
            string error = string.Empty;
            int count = 0;
            errorMessage = string.Empty;
            if (lstErrors != null)
            {
                if (lstErrors.Count > 0)
                {
                    foreach (var err in lstErrors)
                    {
                        if (count < 8)
                            error += err.Key + " : " + err.Value + Environment.NewLine;
                        errorMessage += err.Key + " : " + err.Value + Environment.NewLine;
                    }
                }
            }
            if (string.IsNullOrEmpty(error))
            {
                lblSummaryMessage.Text = "Validation of all Alert Responses was successful. Click Finish to complete the import process and exit the wizard.";
                lblSummaryMessage.ForeColor = Color.Green;
            }
            else
            {
                lblSummaryMessage.Text = "One or more Alert Response(s) failed validation. Click Finish to import the valid Alert Response or click View Validation Errors for more details. You can also click Back to access previous steps of the wizard. ";
                lblSummaryMessage.ForeColor = Color.Red;
            }
            lblErrors.Text = error;
            if (lstErrors.Count < addedFiles.Count)
            {
                btnNext.Enabled = true;
            }
            else
            {
                lblSummaryMessage.Text = "No valid Alert Response found.";
                btnNext.Enabled = false;
            }
        }


        /// <summary>
        /// For importing custom counter files
        /// </summary>
        private void ImportFiles()
        {
            lstNotificationRuleSerialisable.Clear();
            lstErrors.Clear();
            List<string> lstRulenames = new List<string>();
            foreach (var file in addedFiles)
            {
                string filePath = file.First;
                try
                {
                    LOG.Info("File selected for importing Alert Response : " + filePath);
                    NotificationRule ruleSerializable = Idera.SQLdm.DesktopClient.Helpers.NotificationRuleHelper.DeserializeNotificationRule(filePath);
                    string errorMessage = string.Empty;
                    string ruleName = ruleSerializable.Description;
                    ruleName = GetAcceptableRuleName(ruleName, lstRulenames);
                    lstRulenames.Add(ruleName);
                    ruleSerializable.Description = ruleName;
                    NotificationRule rule = new NotificationRule();
                    rule.Description = ruleName;

                    rule.Id = ruleSerializable.Id;
                    rule.Enabled = ruleSerializable.Enabled;
                    rule.MetricIDs = ruleSerializable.MetricIDs;
                    rule.IsMetricsWithAndChecked = ruleSerializable.IsMetricsWithAndChecked;
                    rule.Destinations = ruleSerializable.Destinations;
                    rule.ServerNameComparison = ruleSerializable.ServerNameComparison;
                    rule.ServerTagComparison = ruleSerializable.ServerTagComparison;
                    rule.SnapshotTimeComparison = ruleSerializable.SnapshotTimeComparison;
                    rule.StateComparison = ruleSerializable.StateComparison;
                    rule.StateChangeComparison = ruleSerializable.StateChangeComparison;

                    ruleSerializable = rule;
                    lstNotificationRuleSerialisable.Add(ruleSerializable);
                    
                }
                catch (Exception ex)
                {
                    LOG.Info(filePath + " : " + "Could not parse input xml to a valid Alert Response." + "Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    lstErrors.Add(filePath, "Could not parse input xml to a valid Alert Response.");
                }
            }
        }

        private string GetAcceptableRuleName(string ruleName, List<string> lstRulenames)
        {
            int suffix = 1;
            while (!IsRuleNameAvailable(ruleName, lstRulenames))
            {
                if (suffix == 1)
                    ruleName = ruleName + "_" + suffix.ToString();
                else
                {
                    ruleName = ruleName.Substring(0, ruleName.LastIndexOf("_") + 1) + suffix.ToString();
                }
                suffix++;
            }
            return ruleName;
        }
        
        private bool IsRuleNameAvailable(string ruleName, List<string> lstRulenames)
        {
            bool available = RepositoryHelper.IsNotificationRuleNameAvailable(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, ruleName);
            if (available)
                available = !lstRulenames.Contains(ruleName);
            return available;
            
        }

        private void AddRules()
        {
            bool allSucceded = true;
            foreach (var rule in lstNotificationRuleSerialisable)
            {
                try
                {
                    var x = AddRule(rule);
                    notificationRuleUpdated.Add(x);
                }
                catch (Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException ex)
                {
                    ApplicationMessageBox.ShowError(null,
                                                    "An error occurred while attempting to contact the Management Service.",
                                                    ex as Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException);
                    this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                    LOG.Error("An error occurred while attempting to contact the Management Service.", ex);
                    allSucceded = false;
                    break;
                }
                catch (Exception ex)
                {
                    LOG.Error("One or more alert responses were not imported successfully.", ex);
                    allSucceded = false;
                }
            }
            if (!allSucceded)
            {
                ApplicationMessageBox.ShowError(
                       this,
                       "One or more alert responses were not imported successfully. Please check the logs for details.");
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        /// <summary>
        /// Add a rule to repo
        /// </summary>
        /// <param name="ruleSerializable"></param>
        /// <returns></returns>
        private NotificationRule AddRule(NotificationRule ruleSerializable)
        {
            var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
            
            try
            {
                NotificationRule rule=
                      managementService.AddNotificationRule(ruleSerializable);
                LOG.Info("Alert Response " + ruleSerializable.Description + " imported succesfully");
                return rule;

            }
            catch (Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException ex)
            {
                throw;
            }
            catch (Exception e)
            {
                //LOG.Error("There was an error adding Alert Response + " + ruleSerializable.Description + " . Error Details : " + e.InnerException == null ? e.Message : e.InnerException.Message);

                throw;
            }
        }
        void EnableDisableImportNextButton()
        {
            if (currentPage == WizardPage.ImportPage)
            {
                if (addedFiles.Count > 0)
                {
                    btnNext.Enabled = true;
                }
                else
                {
                    btnNext.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Use this method to open the import custom counters dialogues
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="newRule"></param>
        /// <returns></returns>
        public static DialogResult ImportNewRule(IWin32Window owner, out List<NotificationRule> newRule)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            NotificationRuleImportWizard dialog = null;
            try
            {
                dialog = new NotificationRuleImportWizard();
                dialog.Text = "Import Alert Response";
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
            newRule = dialog.notificationRuleUpdated;
            return dialogResult;
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            string currentPageName = currentPage.ToString();
            string nextBtnName = btnNext.Text;
            Cursor.Current = Cursors.WaitCursor;
            btnBack.Enabled = true;
            btnNext.Enabled = true;
            btnNext.Text = "Next";
            switch (currentPage)
            {
                case WizardPage.WelcomePage:
                    break;
                case WizardPage.ImportPage:
                    btnBack.Enabled = false;
                    break;
                case WizardPage.FinishPage:
                    break;
            }
            currentPage--;
            if (currentPage > WizardPage.FinishPage)
                this.Close();
            else
            {
                ultraTabControl1.Tabs[(int)currentPage].Selected = true;
                // UpdatePageView();
            }
            EnableDisableImportNextButton();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            addedFiles.Clear();
            AddFilesToPanel();

        }

        void AddFilesToPanel()
        {
            if (panelFiles.HasChildren)
            {
                panelFiles.Controls.Clear();
            }
            for (int i = 0; i < addedFiles.Count; i++)
            {
                Label lbl = new Label();
                lbl.Name = "lbl" + i.ToString();
                lbl.Text = addedFiles[i].First;
                lbl.AutoSize = true;
                lbl.Font = new Font("Microsoft Sans Serif", 8.5F);
                this.Controls.Add(lbl); panelFiles.Controls.Add(lbl);
            }
            EnableDisableImportNextButton();
            lblFileCount.Text = string.Format("Number of files selected : {0}", addedFiles.Count.ToString());
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            SqlTextDialog dialog = new SqlTextDialog(errorMessage, "Error Details", false);
            dialog.ShowDialog(ParentForm);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFileDialogue = new OpenFileDialog();
            browseFileDialogue.Multiselect = true;
            browseFileDialogue.Title = "Browse Alert Responses";
            browseFileDialogue.Filter = "XML Files (*.xml)|*.xml";
            DialogResult result = browseFileDialogue.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                addedFiles.Clear();
                for (int i = 0; i < browseFileDialogue.FileNames.Length; i++)
                {
                    int id = fileId++;
                    addedFiles.Add(new Triple<string, string, int>(browseFileDialogue.FileNames[i], browseFileDialogue.SafeFileNames[i], id));
                }
            }
            AddFilesToPanel();
            EnableDisableImportNextButton();
        }



    }
}
