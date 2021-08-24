using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class InstallReady : Form
    {
        Form backScreen;
        public InstallReady(Form backScreenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            backScreen = backScreenObj;
        }

        private void ConfigureUIForBothDmAndId()
        {                                                
            this.labeSPSAccount.Visible = true;
            if (properties.remoteRegister)
            {
                this.labelIDSAccount.Visible = false;
                this.labelDashboardInstructions.Visible = false;
            }
            else
            {
                this.labelIDSAccount.Visible = true;
                this.labelDashboardInstructions.Visible = true;
            }          
        }

        private void ConfigureUIForDashboard()
        {
            this.labeSPSAccount.Visible = false;
            this.labelIDSAccount.Visible = true;
            this.labelDashboardInstructions.Visible = true;
        }

        private void ConfigureUIForDmServices()
        {                        
            this.labeSPSAccount.Visible = true;
            this.labelIDSAccount.Visible = false;
            this.labelDashboardInstructions.Visible = false;        
        }

        private void ConfigureUIForDmConsole()
        {
            this.labeSPSAccount.Visible = false;
            this.labelIDSAccount.Visible = false;
            this.labelDashboardInstructions.Visible = false;
        }
        private void LoadUIForBothDmAndId()
        {
            this.labelDesc.Text = "Ready to install IDERA Dashboard and SQL Diagnostic Manager.";
            this.labelSetUpInfo.Text = "Setup will grant these accounts access to IDERA Dashboard and SQL Diagnostic Manager :";

            if (properties.remoteRegister)
            {
                this.labelDesc.Text = "Ready to install IDERA SQL Diagnostic Manager and register with Remote Dashboard.";
                this.labelSetUpInfo.Text = "Setup will grant this account access to IDERA SQL Diagnostic Manager :";
                labelIDSAccount.Text = string.Format("IDERA Dashboard Service Account: {0}", properties.remoteDashboardServiceUsername);
            }
            else
                labelIDSAccount.Text = string.Format("IDERA Dashboard Service Account: {0}", properties.IDSUsername);

            labeSPSAccount.Text = string.Format("SQL DM Service Account: {0}", properties.SPSUsername);
        }

        private void LoadUIForDashboard()
        {
            this.labelDesc.Text = "Ready to install IDERA Dashboard.";
            this.labelSetUpInfo.Text = "Setup will grant this account access to IDERA Dashboard:";

            labelIDSAccount.Text = string.Format("IDERA Dashboard Service Account: {0}", properties.IDSUsername);
        }

        private void LoadUIForDmServices()
        {
            this.labelDesc.Text = "Ready to install IDERA SQL Diagnostic Manager.";
            this.labelSetUpInfo.Text = "Setup will grant this account access to IDERA SQL Diagnostic Manager:";

            labeSPSAccount.Text = string.Format("SQL DM Service Account: {0}", properties.SPSUsername);
        }

        private void LoadUIForDmConsole()
        {
            this.labelDesc.Text = "Ready to install IDERA SQL Diagnostic Manager.";
            this.labelSetUpInfo.Text = "IDERA SQL Diagnostic Manager is getting installed at the following path: \n   " + properties.dmInstallationPath;
        }

        private void InstallReady_Load(object sender, EventArgs e)
        {
#if TRIAL
            if (properties.installSQLDMServiceAndRepositoryOption)
            {
                ConfigureUIForDmServices();
                LoadUIForDmServices();
            }
            else
            {
                ConfigureUIForDmConsole();
                LoadUIForDmConsole();
            }
            labelSetUpInfo.Size = new Size(395, 24);
            labeSPSAccount.Location = new Point(labeSPSAccount.Location.X, labeSPSAccount.Location.Y - 75);
            labelInstallOrReview.Location = new Point(labelInstallOrReview.Location.X, labelInstallOrReview.Location.Y - 60);

#else 
            if (properties.notRegister)
            {
                if (properties.installDashboardOption)
                {
                    ConfigureUIForDashboard();
                    LoadUIForDashboard();
                }
                else
                {
                    if (properties.installSQLDMServiceAndRepositoryOption)
                    {
                        ConfigureUIForDmServices();
                        LoadUIForDmServices();
                    }
                    else
                    {
                        ConfigureUIForDmConsole();
                        LoadUIForDmConsole();
                    }
                }
            }
            else
            {
                ConfigureUIForBothDmAndId();
                LoadUIForBothDmAndId();
            }

#endif //TRIAL
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            // Store to retry failed upgrade scenarios
            properties.IsDmInstalledOnLocal = SQLdmDetails.IsDmInstalledOnLocal;

            this.Cursor = Cursors.WaitCursor;
            RemoteProgress nextScreen = new RemoteProgress(this);
            this.Hide();
            nextScreen.Show();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreen.Show();
        }

        private void InstallReady_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}
