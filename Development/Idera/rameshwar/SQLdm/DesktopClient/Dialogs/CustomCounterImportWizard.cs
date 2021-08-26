using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Events;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using System.Text.RegularExpressions;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    
    enum WizardPage
    {
        WelcomePage,
        ImportPage,
        FinishPage
    }

    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - This class represents the import wizard for custom counters.
    /// </summary>
    public partial class CustomCounterImportWizard : Form
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CustomCounterImportWizard");
        private WizardPage currentPage;
        private const string INVALIDCOUNTERCHARS = @"/?:&\*<>|#%,;@""'$+=~!^(){}[]-";
        private static readonly string[] RESERVEDNAMES ={"CON","AUX","PRN","COM1","LPT2",".","..","nul","com2",
        "com3","com4","com5","com6","com7","com8","com9","lpt1","lpt3","lpt4","lpt5","lpt6",
        "lpt7","lpt8","lpt9","clock$"};
        private List<Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>> counterUpdated;
        private List<Triple<string, string, int>> addedFiles;
        private int fileId = 1;
        List<Idera.SQLdm.DesktopClient.Helpers.CustomCounterSerializable> lstCounterSerialisable;
        Dictionary<string, string> lstErrors;
        private string errorMessage;
        public CustomCounterImportWizard()
        {
            InitializeComponent();
            this.HelpButton = true;
            this.HelpButtonClicked += new CancelEventHandler(CustomCounterImportWizard_HelpButtonClicked);
            currentPage = WizardPage.WelcomePage;
            addedFiles = new List<Triple<string, string, int>>();
            lstCounterSerialisable = new List<Helpers.CustomCounterSerializable>();
            lstErrors = new Dictionary<string, string>();
            counterUpdated = new List<Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>>();
            btnBack.Enabled = false;
            ultraTabControl1.CreationFilter = new HideTabHeaders();
            this.ultraTabControl1.Appearance.BackColor = Color.Transparent;
            this.ultraTabControl1.Appearance.BackColor2 = Color.Transparent;
        }

        void CustomCounterImportWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.CustomCountersWizardImport);
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
                    AddCounters();
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
                lblSummaryMessage.Text = "Validation of all custom counters was successful. Click Finish to complete the import process and exit the wizard.";
                lblSummaryMessage.ForeColor = Color.Green;
            }
            else
            {
                lblSummaryMessage.Text = "One or more custom counters failed validation. Click Finish to import the valid custom counters or click View Validation Errors for more details. You can also click Back to access previous steps of the wizard. ";
                lblSummaryMessage.ForeColor = Color.Red;
            }
            lblErrors.Text = error;
            if (lstErrors.Count < addedFiles.Count)
            {
                btnNext.Enabled = true;
            }
            else
            {
                lblSummaryMessage.Text = "No valid custom counters found.";
                btnNext.Enabled = false;
            }
        }

        /// <summary>
        /// For importing custom counter files
        /// </summary>
        private void ImportFiles()
        {
            lstCounterSerialisable.Clear();
            lstErrors.Clear();
            List<string> lstCounternames = new List<string>();
            foreach (var file in addedFiles)
            {
                string filePath = file.First;
                try
                {
                    LOG.Info("File selected for importing custom counter : " + filePath);
                    Idera.SQLdm.DesktopClient.Helpers.CustomCounterSerializable counterSerializable = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.DeserializeCustomCounter(filePath);
                    string errorMessage = string.Empty;
                    string counterName = counterSerializable.MetricDefinition.Name ?? counterSerializable.MetricDescription.Name;
                    counterName = GetAcceptableCounterName(counterName, lstCounternames);
                    lstCounternames.Add(counterName);
                    counterSerializable.MetricDefinition.Name = counterName;
                    MetricDescriptionSerializable metricDesc = new MetricDescriptionSerializable();
                    metricDesc.Name = counterName;
                    metricDesc.Category = counterSerializable.MetricDescription.Category;
                    metricDesc.Comments = counterSerializable.MetricDescription.Comments;
                    metricDesc.Description = counterSerializable.MetricDescription.Description;
                    counterSerializable.MetricDescription = metricDesc;
                    lstCounterSerialisable.Add(counterSerializable);
                    //if (!IsCounterNameValid(counterName, out errorMessage))
                    //{
                    //    LOG.Info(filePath + " : " + errorMessage);
                    //    lstErrors.Add(filePath, errorMessage);
                    //}
                    //else
                    //{
                            //if (lstCounternames.Contains(counterName))
                            //{
                            //    errorMessage = "There is already an xml file added for the same counter name. Hence this counter will not be imported.";
                            //    LOG.Info(filePath + " : " + errorMessage);
                            //    lstErrors.Add(filePath, errorMessage);
                            //}
                            //else
                            //{
                            //    lstCounternames.Add(counterName);
                            //    lstCounterSerialisable.Add(counterSerializable);
                            //}
                    //}

                }
                catch (Exception ex)
                {
                    LOG.Info(filePath + " : " + "Could not parse input xml to a valid custom counter." + "Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    lstErrors.Add(filePath, "Could not parse input xml to a valid custom counter.");
                }
            }
        }

        private string GetAcceptableCounterName(string counterName,List<string> lstCounternames)
        {
            int suffix = 1;
            while (!IsCounterNameAvailable(counterName, lstCounternames))
            {
                if (suffix == 1)
                    counterName = counterName + "_" + suffix.ToString();
                else
                {
                    counterName = counterName.Substring(0, counterName.LastIndexOf("_") + 1) + suffix.ToString();
                }
                suffix++;
            }
            return counterName;
        }

        private bool IsCounterNameAvailable(string counterName,List<string> lstCounternames)
        {
            bool available = RepositoryHelper.IsCustomCounterNameAvailable(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counterName);
            if (available)
                available = !lstCounternames.Contains(counterName);
            return available;
        }

        private void AddCounters()
        {
            bool allSucceded = true;
            foreach (var counter in lstCounterSerialisable)
            {
                try
                {
                    var x = AddCounter(counter);
                    counterUpdated.Add(x);
                }
                catch
                {
                    allSucceded = false;
                }
            }
            if (!allSucceded)
                ApplicationMessageBox.ShowError(
                       this,
                       "One or more custom counters were not imported successfully. Please check the logs for details.");
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// Validates custom counter name for cls compliance
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool IsCounterNameCLSCompliant(string counterName, string message)
        {
            if (string.IsNullOrEmpty(counterName))
            {
                message = "The custom counter name cannot be empty strings";
                return false;
            }

            foreach (char c in counterName)
            {
                if (char.IsControl(c))
                {
                    message = "The custom counter name cannot contain Unicode control characters";
                    return false;
                }
                if (char.IsSurrogate(c))
                {
                    message = "The custom counter name cannot contain surrogate characters";
                    return false;
                }
                if (INVALIDCOUNTERCHARS.Contains(c.ToString()))
                {
                    message = "The custom counter name cannot contain any of the following characters: /?:&&\\*\"<>|#%,;@$+=\'~!^(){}[]-\r\n";
                    return false;
                }
            }
            foreach (string reservedName in RESERVEDNAMES)
            {
                if (counterName.ToLower().Equals(reservedName.ToLower()))
                {
                    message = "The custom counter name cannot be system reserved names including 'CON', 'AUX', 'PRN', 'COM1' or 'LPT2'";
                    return false;
                }
            }
            if (!Regex.IsMatch(counterName, "^[A-Za-z]+[A-Za-z0-9_ ]+$"))
            {
                {
                    message = "The custom counter name cannot be '.' or '..'";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates counter name for duplicacy and other validations
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool IsCounterNameValid(string counterName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!IsCounterNameCLSCompliant(counterName, errorMessage))
            {
                return false;
            }

            bool available = "all".Equals(counterName, StringComparison.InvariantCultureIgnoreCase) == false;
            if (available)
                available = RepositoryHelper.IsCustomCounterNameAvailable(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counterName);

            if (!available)
            {
                errorMessage = String.Format(
                        "The counter {0} is already in use.  Please enter a different name. ",
                        counterName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Use this method to open the import custom counters dialogues
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="newCounter"></param>
        /// <returns></returns>
        public static DialogResult ImportNewCounter(IWin32Window owner, out List<Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>> newCounter)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            CustomCounterImportWizard dialog = null;
            try
            {
                dialog = new CustomCounterImportWizard();
                dialog.Text = "Import Custom Counter";
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
            newCounter = dialog.counterUpdated;
            return dialogResult;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFileDialogue = new OpenFileDialog();
            browseFileDialogue.Multiselect = true;
            browseFileDialogue.Title = "Browse Custom Counters";
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
        /// Add a counter to repo
        /// </summary>
        /// <param name="counterSerializable"></param>
        /// <returns></returns>
        private Triple<MetricDefinition, MetricDescription, CustomCounterDefinition> AddCounter(Idera.SQLdm.DesktopClient.Helpers.CustomCounterSerializable counterSerializable)
        {
            var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            IManagementService managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
            CustomCounterDefinition counterDefinition = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.GetCustomCounterDefinition(counterSerializable.CounterDefinition);

            // Force Imported counters to be disabled
            // The user can use them by setting the Azure Profiles
            if(counterDefinition.MetricType == MetricType.AzureCounter)
            {
                counterDefinition.IsEnabled = false;
            }

            MetricDescription metricDescription = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.GetMetricDescription(counterSerializable.MetricDescription);

            MetricDefinition metricDefinition = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.GetMetricDefinition(counterSerializable.MetricDefinition);

            metricDefinition.DefaultMessageID = 0;
            metricDefinition.ProcessNotifications = true;
            metricDefinition.Rank = 50;
            metricDefinition.Options = ThresholdOptions.OKDisabled | ThresholdOptions.NumericValue |
                                       ThresholdOptions.CalculateMaxValue | ThresholdOptions.AdditionalData;
            metricDefinition.ValueType = typeof(object);
            try
            {
                int newMetricID =
                      managementService.AddCustomCounter(metricDefinition, metricDescription, counterDefinition,
                                                         true);
                LOG.Info("Custom Counter " + metricDefinition.Name + " imported succesfully");
                return new Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>(metricDefinition,
                                                                                            metricDescription,
                                                                                            counterDefinition);

            }
            catch (Exception e)
            {
                LOG.Error("There was an error adding cutom counter + " + metricDefinition.Name + " . Error Details : " + e.InnerException == null ? e.Message : e.InnerException.Message);

                throw;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            addedFiles.Clear();
            AddFilesToPanel();

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

        private void btnDetails_Click(object sender, EventArgs e)
        {
            SqlTextDialog dialog = new SqlTextDialog(errorMessage, "Error Details", false);
            dialog.ShowDialog(ParentForm);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
