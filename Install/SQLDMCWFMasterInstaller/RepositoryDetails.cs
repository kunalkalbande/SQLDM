using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;
using BBS.TracerX;

namespace Installer_form_application
{
    public partial class RepositoryDetails : Form
    {
        Form screenObject;
        private Logger LOG = Logger.GetLogger("RepositoryDetails");
        string hostname, username, password;
        bool dbExists = false, dbExistsScreenAlreadyShown = false;

        public RepositoryDetails(Form screenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }


        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.IDInstance = textBoxIDInstance.Text;
            properties.IDDBName = textBoxIDDBName.Text;

            properties.needSqlAuthForConnectingToId = checkBoxUseAuthID.Checked;

            if (textBoxIDInstance.Text == "(local)")            
                hostname = Environment.MachineName;            
            else            
                hostname = textBoxIDInstance.Text;   
                     
            if (checkBoxUseAuthID.Checked)
            {
                username = properties.SQLUsernameID;
                password = properties.SQLPasswordID;
            }
            else
            {
                username = properties.IDSUsername;
                password = properties.IDSPassword;
            }

            buttonNext.Enabled = false;
            buttonBack.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            backgroundWorker1.RunWorkerAsync();
        }

        private void RepositoryDetails_FormClosing(object sender, FormClosingEventArgs e)
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

        private void buttonChange_Click(object sender, EventArgs e)
        {
            SQLAuthID newScreen = new SQLAuthID(this);
            newScreen.Show();
        }

        private void checkBoxUseAuthID_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseAuthID.Checked)
            {                                
                if (!properties.needSqlAuthForConnectingToId && properties.SQLUsernameID == string.Empty && properties.SQLPasswordID == string.Empty)  //If user has already entered sql creds, don't pop up this dialog on check change
                {
                    SQLAuthID newScreen = new SQLAuthID(this);
                    DialogResult dr = newScreen.ShowDialog();
                    if (DialogResult.OK != dr)
                    {
                        checkBoxUseAuthID.Checked = false;
                        return;
                    }                                      
                }                                 
                buttonChangeID.Enabled = true;
                properties.needSqlAuthForConnectingToId = true;
            }
            else
            {
                properties.needSqlAuthForConnectingToId = false;
                buttonChangeID.Enabled = false;
            }
        }

        public void disableCheckBox()
        {
            checkBoxUseAuthID.Checked = false;
        }

        public void StoreSQLAuth(string username, string password)
        {
            properties.SQLUsernameID = username;
            properties.SQLPasswordID = password;
        }

        private void ConfigureUIForUpgrade()
        {
            textBoxIDInstance.Enabled = false;
            textBoxIDDBName.Enabled = false;
            checkBoxUseAuthID.Checked = properties.needSqlAuthForConnectingToId;                  
        }

        private void ConfigureUIForFresh()
        {
            textBoxIDInstance.Enabled = true;
            textBoxIDDBName.Enabled = true;
            checkBoxUseAuthID.Checked = properties.needSqlAuthForConnectingToId;           
        }

        private void LoadUIForUpgrade()
        {
            textBoxIDInstance.Text = IderaDashboardDetails.SqlInstanceNameForInstalledVersion;
            textBoxIDDBName.Text = IderaDashboardDetails.DBNameForInstalledVersion;
        }

        private void LoadUIForFresh()
        {
            textBoxIDInstance.Text = properties.IDInstance;
            textBoxIDDBName.Text = properties.IDDBName;
        }
        private void RepositoryDetails_Load(object sender, EventArgs e) //flow would not come here in case remote reg
        {
            if (IderaDashboardDetails.IsDashboardInstalledOnLocal)   //Upgrade
            {
                ConfigureUIForUpgrade();
                LoadUIForUpgrade();
            }
            else
            {
                ConfigureUIForFresh();
                LoadUIForFresh();
            }            
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.IDInstance = textBoxIDInstance.Text;
            properties.IDDBName = textBoxIDDBName.Text;
            this.Hide();
            screenObject.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Validator.ValidateRepositoryConnection(hostname, properties.IDDBName, properties.needSqlAuthForConnectingToId, username, password, false, ref dbExists, "Idera Dashboard");
            }
            catch (UnauthorizedAccessException ex)
            {
                //fixing SQLDM-26641
                e.Result = string.Format("The current user does not have permissions to create databases or logins on the SQL Server Instance {0}.",hostname);
            }
            catch (CWFBaseException ex)
            {
                e.Result = ex.ErrorCode + ex.ErrorMessage;                
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;                
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonNext.Enabled = true;
            buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;
            if (e.Result != null && e.Result.ToString() != string.Empty)
            {
                LOG.ErrorFormat("RepostioryDetails.backgroundWorker1_RunWorkerCompleted- NeedSQLAuth: {0}, Exception: {1}",properties.needSqlAuthForConnectingToId.ToString(),e.Result.ToString());
                MessageBox.Show(e.Result.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IderaDashboardDetails.IsDashboardInstalledOnLocal && dbExists)
            {
                if (DialogResult.No == MessageBox.Show("The specified repository already exists. It will be updated to the latest version if necessary. Do you want to continue?", "Repository Database Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    return;
            }

            this.Hide();            

            if (properties.installSQLDMServiceAndRepositoryOption)
            {
                RepositoryDetailsDM nextScreen = new RepositoryDetailsDM(this);
                nextScreen.Show();
            }
            else
            {
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
        }
    }
}
