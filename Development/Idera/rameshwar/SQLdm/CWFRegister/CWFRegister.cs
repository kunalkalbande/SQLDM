using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.CWFRegister.Helpers;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.CWFRegister.Objects;

namespace Idera.SQLdm.CWFRegister
{
    public partial class CWFRegister : Form
    {
        private delegate void ShowHelperDialog();

        enum WizardPage
        {
            RepoPage,
            RegisterPage,
            FinishPage
        }

        enum AuthType
        {
            WindowsAuth,
            SQLServerAuth
        }

        static Logger Log = Logger.GetLogger("CWFRegister");
        private CWFDetails cwfDetails = null;
        private WizardPage currentPage;
        private const string BrowseForServersText = "<Browse for more...>";
        private const string BrowseForDatabasesText = "<Browse server...>";

        public CWFRegister()
        {
            InitializeComponent();
            currentPage = WizardPage.RepoPage;
            cbAuthentication.SelectedIndex = (int)AuthType.WindowsAuth;
            SetServerDropDownItems();
            SetDatabaseDropDownItems();
            UpdatePageView();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;

            //read the CWF info from the repo before going to the CWF page.
            if (currentPage == WizardPage.RegisterPage)
            {
                Log.Info("Register Page, setting data of product");
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    if (!RepositoryHelper.IsValidRepository(GetConnnectionInfo()))
                    {
                        MessageBox.Show(String.Format("The selected SQLdm repository database, [{0}], is not a valid repository.  This tool only supports version {1}.", this.cbDatabase.Text, Idera.SQLdm.Common.Constants.ValidRepositorySchemaVersion), 
                                        "Invalid Repository Version", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Error);
                        currentPage = WizardPage.RepoPage;
                        Log.Warn("Invalid Repository Version");
                    }
                    else
                    {
                        cwfDetails = RepositoryHelper.GetTheRegistrationInformation(GetConnnectionInfo());
                        DisplayCWFData();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(String.Format("Unable to read the IDERA Dashboard data from the SQLdm Repository. \n\nError: {0}", exception.Message), "Repository Read Error", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);
                    currentPage = WizardPage.RepoPage;
                    Log.ErrorFormat("Unable to read the Idera Dashboard data from the SQLdm Repository. Error = {0}, {1}", exception.Message, exception);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }

            if (currentPage > WizardPage.FinishPage)
            {
                this.Cursor = Cursors.WaitCursor;

                Log.Info("Finish Page, register product with CWF");

                if (RegisterWithCWF())
                {
                    this.Close();
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    currentPage--;
                }
            }
            else
            {
                registerWizard.Tabs[(int)currentPage].Selected = true;
                Log.Info("Register Page, setting data to connect to CWF");
                UpdatePageView();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            currentPage--;
            registerWizard.Tabs[(int)currentPage].Selected = true;
            UpdatePageView();
        }

        private void UpdatePageView()
        {
            switch (currentPage)
            {
                case WizardPage.RepoPage:
                    {
                        this.btnPrevious.Enabled = false;
                        this.btnNext.Enabled = false;
                        this.btnCancel.Enabled = true;
                        this.lblTitle.Text = "SQL Diagnostic Manager Repository Credentials";
                        this.lblDescription.Text = "Enter your SQL Diagnostic Manager repository credentials to retrieve your IDERA Dashboard information.";
                        this.cbServer.Focus();
                        UpdateRepoControls();
                        break;
                    }
                case WizardPage.RegisterPage:
                    {
                        this.btnPrevious.Enabled = true;
                        this.btnNext.Text = "Next";
                        this.lblTitle.Text = "Register with an Installation of the IDERA Dashboard";
                        this.lblDescription.Text = "Register with an Installation of IDERA Dashboard.  You can register with either a local or remote installation of the IDERA Dashboard.";
                        UpdateRegisterControls();
                        break;
                    }
                case WizardPage.FinishPage:
                    {

                        this.btnPrevious.Enabled = true;
                        this.btnNext.Enabled = true;
                        this.btnNext.Text = "Finish";
                        this.btnCancel.Enabled = true;
                        this.lblTitle.Text = "Register with IDERA Dashboard";
                        this.lblDescription.Text = "Review your IDERA Dashboard settings then click Finish to register with the IDERA Dashboard.  Make sure the IDERA Dashboard is installed on the remote machine before attempting to register.";
                        UpdateFinishControls();
                        break;
                    }
                default:
                    {
                        this.btnPrevious.Enabled = false;
                        this.btnNext.Enabled = false;
                        this.btnCancel.Enabled = false;
                        break;
                    }
            }
        }

        private bool RegisterWithCWF()
        {
            using(Log.InfoCall("RegisterWithCWF"))
            {
                bool error = false;

                if (cwfDetails.ProductID != Constants.NOT_REGISTERED)
                {
                    Log.Info("Connection details to CWF");
                    Log.InfoFormat("Host Name = {0}", cwfDetails.HostName);
                    Log.InfoFormat("Port = {0}", cwfDetails.Port);
                    Log.InfoFormat("Base URL = {0}", cwfDetails.BaseURL);
                }
            
                try
                {
                    if (CWFCommHelper.RegisteredWithCWF(cwfDetails))
                        CWFCommHelper.UnregisterProduct(cwfDetails);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(String.Format("Unable to communicate with the IDERA Dashboard. \n\nError: {0}", exception.Message), "IDERA Dashboard Com Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    error = true;

                    Log.ErrorFormat("Unable to communicate with the Idera Dashboard. Error = {0}, {1}", exception.Message, exception);
                }

                CWFDetails newCWFDetails = CreateNewCWFDetails();
                try
                {
                    if (!error)
                    {
                        Log.Info("New connection details to CWF");
                        Log.InfoFormat("Host Name = {0}", newCWFDetails.HostName);
                        Log.InfoFormat("Port = {0}", newCWFDetails.Port);
                        Log.InfoFormat("Base URL = {0}", newCWFDetails.BaseURL);

                        newCWFDetails.ProductID = CWFCommHelper.RegisterProduct(newCWFDetails, this.cbServer.Text, this.cbDatabase.Text);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Unable to register with the IDERA Dashboard. \n\nError: {0}", e.Message), "IDERA Dashboard Register Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    error = true;

                    Log.ErrorFormat("Unable to register with the Idera Dashboard. Error = {0}, {1}", e.Message, e);
                }

                if (!error)
                {
                    try
                    {
                        RepositoryHelper.UpdateTheRegistrationInformation(GetConnnectionInfo(), newCWFDetails);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(String.Format("Unable to update the IDERA Dashboard data in the SQLDM Repository. \n\nError: {0}", exception.Message), "Repository Update Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        error = true;

                        Log.ErrorFormat("Unable to update the Idera Dashboard data in the SQLdm Repository. Error = {0}, {1}", exception.Message, exception);
                    }
                }
                return !error;  //return false if there was an error.
            }
        }

        private CWFDetails CreateNewCWFDetails()
        {
            CWFDetails details = new CWFDetails();

            details.HostName = this.txtHost.Text;
            details.Port = Convert.ToInt32(this.txtPort.Text);
            details.UserName = this.txtServiceAccount.Text;
            details.Password = this.txtCWFPassword.Text;
            details.DisplayName = this.txtDisplayName.Text.Replace(" ", "");  //There cannot be spaces in the display name (due to a bug in CWF)
            return details;
        }

        private void cbAuthentication_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void UpdateRepoControls()
        {
            if (cbAuthentication.SelectedIndex == (int)AuthType.SQLServerAuth)
            {
                txtUsername.Enabled = true;
                txtRepoPassword.Enabled = true;
            }
            else
            {
                txtUsername.Enabled = false;
                txtRepoPassword.Enabled = false;
            }
            this.btnNext.Enabled = !string.IsNullOrEmpty(cbServer.Text)
                                    && !string.IsNullOrEmpty(cbDatabase.Text)
                                    && (cbAuthentication.SelectedIndex == (int)AuthType.WindowsAuth
                                        || (!string.IsNullOrEmpty(txtUsername.Text)
                                                && !string.IsNullOrEmpty(txtRepoPassword.Text)));

        }

        private void cbAuthentication_TextUpdate(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void txtRepoPassword_TextChanged(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void UpdateFinishControls()
        {
            lblHostBefore.Text = cwfDetails.HostName;
            lblPortBefore.Text = cwfDetails.Port == 0 ? "9292" : cwfDetails.Port.ToString();
            lblAccountBefore.Text = cwfDetails.UserName;
            lblNameBefore.Text = cwfDetails.DisplayName;

            lblHostAfter.Text = txtHost.Text;
            lblPortAfter.Text = txtPort.Text;
            lblAccountAfter.Text = txtServiceAccount.Text;
            lblNameAfter.Text = txtDisplayName.Text.Replace(" ", "");   //There cannot be spaces in the display name (due to a bug in CWF)
        }

        private void txtHost_TextChanged(object sender, EventArgs e)
        {
            UpdateRegisterControls();
        }

        private void txtPort_TextChanged(object sender, EventArgs e)
        {
            UpdateRegisterControls();
        }

        private void txtServiceAccount_TextChanged(object sender, EventArgs e)
        {
            UpdateRegisterControls();
        }

        private void txtCWFPassword_TextChanged(object sender, EventArgs e)
        {
            UpdateRegisterControls();
        }

        private void txtDisplayName_TextChanged(object sender, EventArgs e)
        {
            UpdateRegisterControls();
        }

        private void DisplayCWFData()
        {
            this.txtHost.Text = cwfDetails.HostName;
            this.txtPort.Text = cwfDetails.Port == 0 ? "9292" : cwfDetails.Port.ToString();
            this.txtServiceAccount.Text = cwfDetails.UserName;
            this.txtCWFPassword.Text = cwfDetails.Password;
            this.txtDisplayName.Text = cwfDetails.DisplayName;
        }

        private void UpdateRegisterControls()
        {
            this.btnNext.Enabled = !string.IsNullOrEmpty(txtHost.Text)
                                    && !string.IsNullOrEmpty(txtPort.Text)
                                    && !string.IsNullOrEmpty(txtServiceAccount.Text)
                                    && !string.IsNullOrEmpty(txtCWFPassword.Text)
                                    && !string.IsNullOrEmpty(txtDisplayName.Text);
        }

        private SqlConnectionInfo GetConnnectionInfo()
        {
            using(Log.InfoCall("GetConnnectionInfo"))
            {
                SqlConnectionInfo conInfo = null;

                Log.InfoFormat("Server = {0}", cbServer.Text);
                Log.InfoFormat("Database = {0}", cbDatabase.Text);

                if (cbAuthentication.SelectedIndex == (int)AuthType.WindowsAuth)
                {
                    conInfo = new SqlConnectionInfo(cbServer.Text.Trim(), cbDatabase.Text.Trim());
                    Log.Info("Authentication = Windows Authentication");
                }
                else
                {
                    conInfo = new SqlConnectionInfo(cbServer.Text.Trim(), cbDatabase.Text.Trim(),
                                                    txtUsername.Text.Trim(), txtRepoPassword.Text.Trim());
                    Log.Info("Authentication = SQL Server Authentication");
                }
                return conInfo;
            }
        }

        private SqlConnectionInfo GetServerConnnectionInfo()
        {
            SqlConnectionInfo ci = null;
            if (cbAuthentication.SelectedIndex == (int)AuthType.WindowsAuth)
            {
                ci = new SqlConnectionInfo(cbServer.Text.Trim());
            }
            else
            {
                ci = new SqlConnectionInfo(cbServer.Text.Trim(), txtUsername.Text.Trim(), txtRepoPassword.Text.Trim());
            }
            return ci;
        }

        private void cbServer_TextUpdate(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void cbServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedServer = cbServer.SelectedItem as string;
            if (selectedServer != null)
            {
                if (selectedServer == BrowseForServersText)
                {
                    ShowHelperDialog method = delegate() {
                                SqlServerBrowserDialog ssbd = new SqlServerBrowserDialog();
                                if (ssbd.ShowDialog(this) == DialogResult.OK)
                                {
                                    string instance = ssbd.SelectedInstance;
                                    cbServer.SelectedIndex = cbServer.Items.IndexOf(instance);
                                    cbServer.Text = instance;
                                }
                                else
                                {
                                    SetDefaultItem(cbServer);
                                }
                                UpdateRepoControls();
                            };
                    BeginInvoke(method);
                }
                else
                {
                    cbServer.Text = selectedServer;
                    UpdateRepoControls();
                }
            }
        }

        private void cbDatabase_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedDatabase = cbDatabase.SelectedItem as string;
            if (selectedDatabase != null)
            {
                if (selectedDatabase == BrowseForDatabasesText)
                {
                    ShowHelperDialog method = delegate()
                    {
                        Idera.SQLdm.Common.Configuration.SqlConnectionInfo connectionInfo = GetServerConnnectionInfo();
                        DatabaseBrowserDialog ssbd = new DatabaseBrowserDialog(connectionInfo);
                        if (ssbd.ShowDialog(this) == DialogResult.OK)
                        {
                            string instance = ssbd.SelectedDatabase;
                            cbDatabase.SelectedIndex = cbDatabase.Items.IndexOf(instance);
                            cbDatabase.Text = instance;
                        }
                        else
                        {
                            SetDefaultItem(cbDatabase);
                        }
                        UpdateRepoControls();
                    };
                    BeginInvoke(method);
                }
                else
                {
                    cbDatabase.Text = selectedDatabase;
                    UpdateRepoControls();
                }
            }
        }

        private void cbDatabase_TextUpdate(object sender, EventArgs e)
        {
            UpdateRepoControls();
        }

        private void SetServerDropDownItems()
        {
            cbServer.Items.Clear();
            cbServer.Items.Add(BrowseForServersText);
            SetDefaultItem(cbServer);
        }

        private void SetDatabaseDropDownItems()
        {
            cbDatabase.Items.Clear();
            cbDatabase.Items.Add(BrowseForDatabasesText);
            SetDefaultItem(cbDatabase);
        }

        private void SetDefaultItem(ComboBox cbo)
        {
            if (cbo.Items.Count > 1)
                cbo.SelectedIndex = 0;
            else
            {
                cbo.SelectedIndex = -1;
                cbo.SelectedText = "";
            }
        }
    }
}
