using CWFInstallerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Security.Encryption;

namespace Installer_form_application
{
    public partial class ServiceAccount : Form
    {
        Form backScreenObj;
        private Logger LOG = Logger.GetLogger("Description");
        public ServiceAccount(Form backScreen)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObj = backScreen;

#if TRIAL
            checkBoxSameCreds.Visible = false;
#endif 
        }

        private void checkBoxSameCreds_CheckedChanged(object sender, EventArgs e)
        {
            properties.useSameCreds = checkBoxSameCreds.Checked;
            ConfigureUIForBothAccounts();
            LoadUIForBothAccounts(); 
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (properties.useSameCreds)
            {
                properties.IDSUsername = properties.SPSUsername = textBoxSPSUserName.Text;
                properties.IDSPassword = properties.SPSPassword = textBoxSPSPassword.Text;
            }
            else
            {
                properties.IDSUsername = textBoxIDSUsername.Text;
                properties.IDSPassword = textBoxIDSPassword.Text;
                properties.SPSUsername = textBoxSPSUserName.Text;
                properties.SPSPassword = textBoxSPSPassword.Text;
            }
            this.Hide();
            backScreenObj.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (properties.useSameCreds && properties.installSQLDMServiceAndRepositoryOption && properties.installDashboardOption)
            {
                properties.IDSUsername = properties.SPSUsername = textBoxSPSUserName.Text;
                properties.IDSPassword = properties.SPSPassword = textBoxSPSPassword.Text;
            }
            else
            {
                properties.IDSUsername = textBoxIDSUsername.Text;
                properties.IDSPassword = textBoxIDSPassword.Text;
                properties.SPSUsername = textBoxSPSUserName.Text;
                properties.SPSPassword = textBoxSPSPassword.Text;
            }
            this.buttonNext.Enabled = false;
            this.buttonBack.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.buttonNext.Enabled = true;
            this.buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;

            if (e.Result != null && e.Result.ToString() != string.Empty)
            {
                LOG.ErrorFormat("ServiceAccount.backgroundWorker_RunWorkerCompleted- Exception: {0}",e.Result.ToString());
                MessageBox.Show(e.Result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide();

#if TRIAL

            RepositoryDetailsDM nextScreen = new RepositoryDetailsDM(this);
            nextScreen.Show();

#else 
            if (properties.installDashboardOption)
            {
                if (IderaDashboardDetails.IsDashboardInstalledOnLocal)
                {
                    RepositoryDetails nextScreen = new RepositoryDetails(this);
                    nextScreen.Show();
                }
                else
                {
                    PortForm nextScreen = new PortForm(this);
                    nextScreen.Show();
                }
            }
            else
            {
                RepositoryDetailsDM nextScreen = new RepositoryDetailsDM(this);
                nextScreen.Show();
            }

#endif

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (properties.installDashboardOption)  //local ID validation
                    Validator.ValidateServiceCredentials(properties.IDSUsername, properties.IDSPassword);                
            }
            catch (CWFBaseException ex)
            {
                e.Result = ex.ErrorMessage; 
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }

            try
            {
                if (properties.installSQLDMServiceAndRepositoryOption)   //Dm validation                
                    Validator.ValidateServiceCredentials(properties.SPSUsername, properties.SPSPassword);
                Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("ServiceAccount", EncryptionHelper.QuickEncrypt(properties.SPSUsername));
                Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("ServicePassword", EncryptionHelper.QuickEncrypt(properties.SPSPassword));

            }
            catch (CWFBaseException ex)
            {
                if (ex.ErrorCode == "INV_AcctCred")
                    e.Result = "The IDERA SQL Diagnostic Manager credentials you provided are invalid. Please enter valid credentials for the IDERA SQL Diagnostic Manager.";
                else
                    e.Result = ex.ErrorMessage;
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void ServiceAccount_FormClosing(object sender, FormClosingEventArgs e)
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

        private void ConfigureUIForBothAccounts()
        {

#if TRIAL

#else
            checkBoxSameCreds.Visible = true;
#endif

            if (properties.useSameCreds)
            {
                labelSQLDMCredential.Visible = false;
                labelDashboardCredentials.Visible = false;
                labelIDSUsername.Visible = false;
                textBoxIDSUsername.Visible = false;
                labelIDSPassword.Visible = false;
                textBoxIDSPassword.Visible = false;                
            }
            else
            {
                labelSQLDMCredential.Visible = true;
                labelDashboardCredentials.Visible = true;
                labelIDSUsername.Visible = true;
                textBoxIDSUsername.Visible = true;
                labelIDSPassword.Visible = true;
                textBoxIDSPassword.Visible = true;
            }
            labelSQLdmPassword.Visible = true;
            labelSQLdmUsername.Visible = true;
            textBoxSPSUserName.Visible = true;
            textBoxSPSPassword.Visible = true;
        }

        private void LoadUIForBothAccounts()
        {
            if(textBoxIDSUsername.Text == string.Empty)
                textBoxIDSUsername.Text = properties.IDSUsername;

            if (textBoxIDSPassword.Text == string.Empty)
                textBoxIDSPassword.Text = properties.IDSPassword;

            if (textBoxSPSUserName.Text == string.Empty)
                textBoxSPSUserName.Text = properties.SPSUsername;

            if (textBoxSPSPassword.Text == string.Empty)
                textBoxSPSPassword.Text = properties.SPSPassword;
            
            labelDesc.Text = "Specify the service account(s) for IDERA Dashboard and SQL Diagnostic Manager." +
                " The service accounts will be used to collect information and to log into the applications." +
                " You can log into the IDERA Dashboard to create additional users.";
        }

        private void ConfigureUIForSQLdmAccount()
        {
            checkBoxSameCreds.Visible = false;                        
            labelDashboardCredentials.Visible = false;
            labelIDSUsername.Visible = false;
            textBoxIDSUsername.Visible = false;
            labelIDSPassword.Visible = false;
            textBoxIDSPassword.Visible = false;

            labelSQLDMCredential.Visible = true;
            labelSQLdmPassword.Visible = true;
            labelSQLdmUsername.Visible = true;
            textBoxSPSUserName.Visible = true;
            textBoxSPSPassword.Visible = true;
        }

        private void LoadUIForSQLdmAccount()
        {
            textBoxSPSUserName.Text = properties.SPSUsername;
            textBoxSPSPassword.Text = properties.SPSPassword;
            labelDesc.Text = "Specify the service account for SQL Diagnostic Manager." +
                " The service accounts will be used to collect information and to log into the applications.";
        }

        private void ConfigureUIDashboardAccount()
        {
            checkBoxSameCreds.Visible = false;
            labelSQLDMCredential.Visible = false;
            labelSQLdmPassword.Visible = false;
            labelSQLdmUsername.Visible = false;
            textBoxSPSUserName.Visible = false;
            textBoxSPSPassword.Visible = false;

            labelDashboardCredentials.Visible = true;
            labelIDSUsername.Visible = true;
            textBoxIDSUsername.Visible = true;
            labelIDSPassword.Visible = true;
            textBoxIDSPassword.Visible = true;            
        }

        private void LoadUIForDashboardAccount()
        {
            textBoxIDSUsername.Text = properties.IDSUsername;
            textBoxIDSPassword.Text = properties.IDSPassword;
            labelDesc.Text = "Specify the service account for IDERA Dashboard." +
                " The service accounts will be used to collect information and to log into the applications." +
                " You can log into the IDERA Dashboard to create additional users.";
        }

        private void ServiceAccount_Load(object sender, EventArgs e)
        {               
            if (properties.localRegister)
            {
                ConfigureUIForBothAccounts();
                LoadUIForBothAccounts();
            }
            else if (properties.remoteRegister)
            {
                ConfigureUIForSQLdmAccount();
                LoadUIForSQLdmAccount();
            }
            else
            {
                if (properties.installDashboardOption && properties.installSQLDMServiceAndRepositoryOption)
                {
                    ConfigureUIForBothAccounts();
                    LoadUIForBothAccounts();
                }
                else if (properties.installSQLDMServiceAndRepositoryOption)
                {
                    ConfigureUIForSQLdmAccount();
                    LoadUIForSQLdmAccount();
                }
                else
                {
                    ConfigureUIDashboardAccount();
                    LoadUIForDashboardAccount();
                }
            }
        }        
    }
}
