using CWFInstallerService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;

namespace Installer_form_application
{
    public partial class RepositoryDetailsDM : Form
    {
        Form backScreenObject;
        string hostnameDM, username, password;
        bool sqlAuth = false, is2FA = false;
        bool dbExists = false;
        bool doNotMoveForward = false;
        private Logger LOG = Logger.GetLogger("RepositoryDetailsDM");

        public RepositoryDetailsDM(Form backScreenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObject = backScreenObj;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.DMInstance = textBoxDMInstance.Text;
            properties.DMDBName = textBoxDMDBName.Text;

            properties.needSqlAuthForConnectingToDmRepo = checkBox_UseSQLAuth_Repo.Checked;
            properties.needSqlAuthForConnectingToDmService = checkBox_UseSQLAuth_Service.Checked;

            if (textBoxDMInstance.Text == "(local)")
            {
                hostnameDM = Environment.MachineName;
            }
            else
            {
                hostnameDM = textBoxDMInstance.Text;
            }

            if (checkBox_UseSQLAuth_Repo.Checked)
            {                
                username = properties.SQLUsername_DM_Repo;
                password = properties.SQLPassword_DM_Repo;
            }
            else
            {
                username = properties.SPSUsername;
                password = properties.SPSPassword;
            }

            buttonNext.Enabled = false;
            buttonBack.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            backgroundWorker1.RunWorkerAsync();
        }
        
        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.DMInstance = textBoxDMInstance.Text;
            properties.DMDBName = textBoxDMDBName.Text;
            properties.needSqlAuthForConnectingToDmRepo = checkBox_UseSQLAuth_Repo.Checked;
            properties.needSqlAuthForConnectingToDmService = checkBox_UseSQLAuth_Service.Checked;            
            this.Hide();
            backScreenObject.Show();
        }

        private void RepositoryDetailsDM_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        public void disableCheckBox(bool isSQLAuthForManagementService)
        {
            if (isSQLAuthForManagementService)
            {
                checkBox_UseSQLAuth_Service.Checked = false;
            }
            else
            {
                checkBox_UseSQLAuth_Repo.Checked = false;
            }
        }

        public void StoreSQLAuth(string username, string password, bool isSQLAuthForManagementService)
        {
            if (isSQLAuthForManagementService)
            {
                properties.SQLUsername_DM_Service = username;
                properties.SQLPassword_DM_Service = password;
            }
            else
            {
                properties.SQLUsername_DM_Repo = username;
                properties.SQLPassword_DM_Repo = password;
            }
        }

        private void buttonChange_Repo_Click(object sender, EventArgs e)
        {
            SQLAUTHDM newScreen = new SQLAUTHDM(this, false);
            newScreen.ShowDialog();
        }

        private void checkBox_UseSQLAuth_Repo_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseSQLAuth_Repo.Checked)
            {
                if (!properties.needSqlAuthForConnectingToDmRepo && properties.SQLUsername_DM_Repo == string.Empty && properties.SQLPassword_DM_Repo == string.Empty)  //If user has already entered sql creds, don't pop up this dialog on check change
                {
                    SQLAUTHDM newScreen = new SQLAUTHDM(this, false);
                    DialogResult dr = newScreen.ShowDialog();
                    if (DialogResult.OK != dr)
                    {
                        checkBox_UseSQLAuth_Repo.Checked = false;
                        return;
                    }
                }
                buttonChange_Repo.Enabled = true;
                properties.needSqlAuthForConnectingToDmRepo = true;
            }
            else
            {
                properties.needSqlAuthForConnectingToDmRepo = false;
                buttonChange_Repo.Enabled = false;
            }
        }

        private void ConfigureUIForUpgradeScenario()
        {
            textBoxDMInstance.Enabled = false;
            textBoxDMDBName.Enabled = false;
            if (properties.needSqlAuthForConnectingToDmRepo)
                checkBox_UseSQLAuth_Repo.Checked = true;
            if (properties.needSqlAuthForConnectingToDmService)
                checkBox_UseSQLAuth_Service.Checked = true;
        }

        private void ConfigureUIForFreshInstallScenario()
        {
            textBoxDMInstance.Enabled = true;
            textBoxDMDBName.Enabled = true;
            if (properties.needSqlAuthForConnectingToDmRepo)
                checkBox_UseSQLAuth_Repo.Checked = true;
            if (properties.needSqlAuthForConnectingToDmService)
                checkBox_UseSQLAuth_Service.Checked = true;
        }

        private void LoadUIForUpgradeScenario()
        {
            textBoxDMInstance.Text = SQLdmDetails.SqlInstanceName;
            textBoxDMDBName.Text = SQLdmDetails.DbName;
        }

        private void LoadUIForFreshScenario()
        {
            textBoxDMInstance.Text = properties.DMInstance;
            textBoxDMDBName.Text = properties.DMDBName;
        }

        private void RepositoryDetailsDM_Load(object sender, EventArgs e)
        {
            if (SQLdmDetails.IfServicesAreInstalledOnLocal)
            {
                ConfigureUIForUpgradeScenario();
                LoadUIForUpgradeScenario();

                backgroundWorkerForCheckingCorruptedConfig.RunWorkerAsync();
            }
            else
            {
                ConfigureUIForFreshInstallScenario();
                LoadUIForFreshScenario();
            }
        }

        private void backgroundWorkerForCheckingCorruptedConfig_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SQLdmDetails.SqlInstanceName == string.Empty || SQLdmDetails.DbName == string.Empty)
            {                
                e.Result = false; //corrupted file found
            }
        }

        private void backgroundWorkerForCheckingCorruptedConfig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null && e.Result is bool && (bool)e.Result == false)
            {
                LOG.ErrorFormat("RepositoryDetailsDM.backgroundWorkerForCheckingCorruptedConfig_RunWorkerCompleted- Incomplete details from Management Service Config, IntanceName: {0}, DatabaseName: {1}", SQLdmDetails.SqlInstanceName, SQLdmDetails.DbName);
                buttonNext.Enabled = false;
                textBoxDMDBName.Text = textBoxDMInstance.Text = "Could not be retrieved...";
                MessageBox.Show("There is some problem in reading the configuration file for installed version of SQLdm, we are not able to fetch repositoy details for it. Please uninstall the old version and than re-run this installer.", "Corrupted Management Service Configuration File", MessageBoxButtons.OK, MessageBoxIcon.Error);               
            }
        }

        private void buttonChange_Service_Click(object sender, EventArgs e)
        {
            SQLAUTHDM newScreen = new SQLAUTHDM(this, true);
            newScreen.ShowDialog();
        }

        private void checkBox_UseSQLAuth_Service_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseSQLAuth_Service.Checked)
            {
                if (!properties.needSqlAuthForConnectingToDmService && properties.SQLUsername_DM_Service == string.Empty && properties.SQLPassword_DM_Service == string.Empty)  //If user has already entered sql creds, don't pop up this dialog on check change
                {
                    SQLAUTHDM newScreen = new SQLAUTHDM(this, true);
                    DialogResult dr = newScreen.ShowDialog();
                    if (DialogResult.OK != dr)
                    {
                        checkBox_UseSQLAuth_Service.Checked = false;
                        return;
                    }
                }
                buttonChange_Service.Enabled = true;
                properties.needSqlAuthForConnectingToDmService = true;
            }
            else
            {
                properties.needSqlAuthForConnectingToDmService = false;
                buttonChange_Service.Enabled = false;
            }            
        }

        //Start
        //SQLDM 10.1 (Barkha Khatri) -- getting  existing license information
        private void backgroundWorkerForLicenseInfo_DoWork(object sender, DoWorkEventArgs e)
        {

            LicenseSummary licenseSummary = null;
            string instance = properties.DMInstance;
            string repository = properties.DMDBName;
            //SQLDM-28112 - Vamshi Krishna - The authentication should use the needSqlAuthForConnectingToDmRepo instead of needSqlAuthForConnectingToId
            bool useSqlAuth = properties.needSqlAuthForConnectingToDmRepo; // properties.needSqlAuthForConnectingToId;

            string sqlUser = properties.SQLUsername_DM_Repo;
            string sqlPassword = properties.SQLPassword_DM_Repo;
            licenseSummary = LicenseHelper.GetLicenseSummary(useSqlAuth, sqlUser, sqlPassword, instance, repository);
            e.Result= licenseSummary;
        }
        //SQLDM 10.1 (Barkha Khatri) -- checking whether new key is required or not
        private void backgroundWorkerForLicenseInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.buttonNext.Enabled = true;
            this.buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;

            bool isNewKeyRequired = true;
            LicenseSummary licenseSummary =(LicenseSummary) e.Result;
            if (licenseSummary.IsTrial)
            {
                if (licenseSummary.Status == LicenseStatus.Expired)
                {
                    isNewKeyRequired = true;
                }
                else
                {
                    isNewKeyRequired = false;
                }
            }
            else // for production license
            {
                if (licenseSummary.ProductVersion.Major < 9)
                {
                    isNewKeyRequired = true;
                }
                else
                {
                    if (licenseSummary.Status == LicenseStatus.CountExceeded || licenseSummary.Status == LicenseStatus.Expired || licenseSummary.Status == LicenseStatus.NoValidKeys)
                    {
                        isNewKeyRequired = true;
                    }
                    else
                    {
                        isNewKeyRequired = false;
                    }
                }
            }
            if (isNewKeyRequired)
            {
                LicenseScreen licenseScreen = new LicenseScreen(this, licenseSummary);
                licenseScreen.Show();
            }
            else
            {
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
        }
        
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SQLdmHelper.ValidateRepositoryConnection(hostnameDM, properties.DMDBName, properties.needSqlAuthForConnectingToDmRepo, username, password, is2FA, ref dbExists, "SQL Diagnostic Manager");
                if (!SQLdmHelper.ValidateCurrentUserSysAdminPermissions(hostnameDM, properties.needSqlAuthForConnectingToDmRepo, username, password))
                {
                    if (DialogResult.No == MessageBox.Show("The SQL Diagnostic Manager Repository Installation requires sysadmin privileges. Please use credentials that have sysadmin privileges on the SQL Server Instance that has been entered or choose a new SQL Server Instance that has sysadmin credentials.",
                         "Warning: Non-SysAdmin User", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))

                    {
                        doNotMoveForward = true;
                    }
                    else
                        doNotMoveForward = false;
                }

                //if sqldm repo/services already exist
                if (SQLdmDetails.IfServicesAreInstalledOnLocal)
                {
                    if(properties.needSqlAuthForConnectingToDmRepo)//do not use integrated login for sql server
                        properties.DisplayName = IderaDashboardHelper.GetExistingDisplayName(SQLdmDetails.SqlInstanceName, SQLdmDetails.DbName, false, properties.SQLUsername_DM_Repo, properties.SQLPassword_DM_Repo);
                    else
                        properties.DisplayName = IderaDashboardHelper.GetExistingDisplayName(SQLdmDetails.SqlInstanceName, SQLdmDetails.DbName, true, properties.SPSUsername, properties.SPSPassword);
                }
            }
            catch (CWFBaseException ex)
            {
                e.Result = ex.ErrorCode + ex.ErrorMessage;
                return;
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
                return;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonNext.Enabled = true;
            buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;

            //non sysadmin rights pop up resulted in 'no' as input from user
            if(doNotMoveForward)
            {                
                return;
            }

            if (e.Result != null && e.Result.ToString() != string.Empty)
            {
                LOG.ErrorFormat("RepositoryDetailsDM.backgroundWorker1_RunWorkerCompleted()- Exception: {0}",e.Result.ToString());
                MessageBox.Show(e.Result.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!SQLdmDetails.IfServicesAreInstalledOnLocal && dbExists)
            {
                if (DialogResult.No == MessageBox.Show("The specified repository already exists. It will be updated to the latest version if necessary. Do you want to continue?", "Repository Database Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    return;
            }

            if (SQLdmDetails.IfServicesAreInstalledOnLocal)
            {
                LOG.Info("Checking License Information");
                buttonNext.Enabled = false;
                buttonBack.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                backgroundWorkerForLicenseInfo.RunWorkerAsync();
            }
            else
            {
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
            this.Hide();
        }
    }
}
