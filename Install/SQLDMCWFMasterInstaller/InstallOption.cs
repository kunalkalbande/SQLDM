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
    public partial class InstallOption : Form
    {
        private Logger LOG = Logger.GetLogger("InstallOption");
        Form screenObject;   


        public InstallOption(Form screenObj)
        {                                          
            InitializeComponent();
			
#if TRIAL
            this.labelHeading.Location = new Point(191, 28);
            this.labelHeading.Margin = new Padding(0, 0, 0, 0);
            this.labelHeading.Size = new Size(225, 21);
            
            this.labelDesc.Location = new Point(193, 65);
            this.labelDesc.Margin = new Padding(0, 0, 0, 0);
            this.labelDesc.Size = new Size(379, 41);
            this.labelDesc.Text = "Please Select the SQL Diagnostic Manager components that you wish to install.";
            

            this.checkBoxDMConsoleOnly.Location = new Point(checkBoxDMConsoleOnly.Location.X, checkBoxDMConsoleOnly.Location.Y + 20);
            this.LabelForDMConsoleOnly.Location = new Point(LabelForDMConsoleOnly.Location.X, LabelForDMConsoleOnly.Location.Y + 20);

            this.checkBoxDMServicesAndRepository.Location = new Point(checkBoxDMServicesAndRepository.Location.X, checkBoxDMServicesAndRepository.Location.Y + 30);
            this.LabelForDMServicesAndRepository.Location = new Point(LabelForDMServicesAndRepository.Location.X, LabelForDMServicesAndRepository.Location.Y + 30);

            this.checkBoxInstallDashbaord.Visible = false;
            this.checkBoxInstallDashbaord.Checked = false;
            this.labelForDashboardReInstall.Visible = false;
            this.LabelForInstallDashbaord.Visible = false;

            this.labelForSqlDmReInstall.Location = new Point(labelForSqlDmReInstall.Location.X, labelForSqlDmReInstall.Location.Y + 40);
//            this.labelForSqlDmReInstall.Size = new Size(302, 26);
 //           this.labelForSqlDmReInstall.Text = "A previous version of SQLdm is detected and it will be upgraded \r\nusing this installer.";

            this.AutoScaleDimensions = new SizeF(6F, 13F);

            this.ClientSize = new Size(593, 454);

            this.Margin = new Padding(0, 0, 0, 0);
#endif //TRIAL

            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        private void InstallOption_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();        
        }

        private void checkChangedForInstallingComponents(object sender, EventArgs e)
        {
            if (!checkBoxInstallDashbaord.Checked && !checkBoxDMServicesAndRepository.Checked && !checkBoxDMConsoleOnly.Checked) //no option is selected
                buttonNext.Enabled = false;
            else
                buttonNext.Enabled = true;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {           
            if (SQLdmDetails.IfServicesAreInstalledOnLocal && 
                SQLdmDetails.ShouldConsoleOptionBeEnabled && 
                SQLdmDetails.ShouldServicesOptionBeEnabled && 
                checkBoxDMConsoleOnly.Checked && 
                !checkBoxDMServicesAndRepository.Checked)
            {
                LOG.WarnFormat("Latest version of Desktop Client, needs latest Serivces too.");
                MessageBox.Show("Latest version of Desktop Client, needs latest Serivces too. Please choose the correct options","Incompatible Install",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetProperties();
            this.Hide();

            if (properties.installSQLDMServiceAndRepositoryOption && !properties.installDashboardOption) //remote or no registration
            {
                properties.localRegister = false;
				
#if TRIAL
                properties.notRegister = true;
                ServiceAccount nextScreen = new ServiceAccount(this);
                nextScreen.Show();

#else
                Credentials cred = new Credentials(this);
                cred.Show();

#endif
				
            }
            //Fixing: SQLdm-26692
            else if(properties.installSQLDMConsoleOption && !properties.installDashboardOption && !properties.installSQLDMServiceAndRepositoryOption)
            {
                properties.remoteRegister = properties.localRegister = false;
                properties.notRegister = true;
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
            else
            {
                properties.remoteRegister = false;
                if (checkBoxInstallDashbaord.Checked && checkBoxDMServicesAndRepository.Checked)
                {
                    properties.localRegister = true;
                    properties.notRegister = false;
                }
                else
                {
                    properties.localRegister = false;
                    properties.notRegister = true;
                }
                ServiceAccount nextScreen = new ServiceAccount(this);
                nextScreen.Show();
            }            
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            SetProperties();
            this.Hide();
            screenObject.Show();
        }

        private void SetProperties()
        {
            properties.installSQLDMConsoleOption = checkBoxDMConsoleOnly.Checked;
            properties.installSQLDMServiceAndRepositoryOption = checkBoxDMServicesAndRepository.Checked;
            properties.installDashboardOption = checkBoxInstallDashbaord.Checked;            
        }

        private void LoadUI()
        {
            if (IderaDashboardDetails.IsDashboardInstalledOnLocal)
            {
                if (IderaDashboardDetails.IsRestrictedVersionInstalledOnLocal)
                {
                    LOG.InfoFormat("Restricted version of dashboard is installed: {0}", IderaDashboardDetails.InstalledVersion);
                    labelForDashboardReInstall.Text = "The installed version (" + IderaDashboardDetails.InstalledVersion + ") of dashboard on the local machine is \n" +
                        "incompatible, Please uninstall the dashboard and then retry the installation.";
                    labelForDashboardReInstall.ForeColor = System.Drawing.Color.Red;
                }
                //Do not use past SQLDM 10.4.   The InstallationHelper.getCurrentVersion() in CWF needs to be changed to return the correct version!
                else if (IderaDashboardDetails.InstalledVersion.Trim() == properties.ShippingVersionOfDashboard.Trim())   //Latest version already installed
                {
                    LOG.InfoFormat("Latest version of dashboard is installed: {0}", IderaDashboardDetails.InstalledVersion);
                    //FIXING SQLDM-26253
                    labelForDashboardReInstall.Text = "The installed version (" + IderaDashboardDetails.InstalledVersion + ") of dashboard on the local machine is upto date.";
                    labelForDashboardReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
                else
                {
                    LOG.InfoFormat("Old version of dashboard is installed: {0}", IderaDashboardDetails.InstalledVersion);
                    labelForDashboardReInstall.Text = "The installed version (" + IderaDashboardDetails.InstalledVersion + ") of dashboard on the local machine is eligible \n" +
                    "for upgrade. If you continue with local registration, the dashboard will be \n" +
                    "upgraded to " + properties.ShippingVersionOfDashboard + ".";  //Do not use past SQLDM 10.4.   The InstallationHelper.getCurrentVersion() in CWF needs to be changed to return the correct version!
                    labelForDashboardReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            else
            {
                LOG.InfoFormat("Dashboard is not installed");
            }

            if (SQLdmDetails.IsDmInstalledOnLocal)
            {
                if (SQLdmDetails.IsLatestVersionInstalled)
                {
                    LOG.InfoFormat("Latest version of SQLdm is installed: {0}", SQLdmDetails.InstalledVersion);
                    labelForSqlDmReInstall.Text = "Latest version of SQLdm is already installed on your system. For repair mode,\r\n" +
    "please use the standalone installer";
                    labelForSqlDmReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
                else if (SQLdmDetails.ShouldConsoleOptionBeEnabled && !SQLdmDetails.ShouldServicesOptionBeEnabled)
                {
                    LOG.InfoFormat("Only old version of SQLdm desktop client is installed: {0}", SQLdmDetails.InstalledVersion);
                    labelForSqlDmReInstall.Text = "Previous version (" + SQLdmDetails.InstalledVersion.ToString() + ") of SQLdm Desktop Client is detected and only\n" +
                        "that can be upgraded using this installer.";
                    labelForSqlDmReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
                else if (!SQLdmDetails.ShouldConsoleOptionBeEnabled && SQLdmDetails.ShouldServicesOptionBeEnabled)
                {
                    LOG.InfoFormat("Only old version of SQLdm services is installed: {0}", SQLdmDetails.InstalledVersion);
                    labelForSqlDmReInstall.Text = "Previous version (" + SQLdmDetails.InstalledVersion.ToString() + ") of SQLdm Services and Repository is detected \n"  +
                        "and only that can be upgraded using this installer.";
                    labelForSqlDmReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
                else
                {
                    LOG.InfoFormat("Old version of SQLdm is installed: {0}", SQLdmDetails.InstalledVersion);
                    labelForSqlDmReInstall.Text = "Previous version (" + SQLdmDetails.InstalledVersion.ToString() + ") of SQLdm is detected and it will be upgraded\n" +
                        "using this installer.";
                    labelForSqlDmReInstall.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            else
            {
                LOG.InfoFormat("SQLdm is not installed");
            }
        }     

        private void ConfigureUI()
        {
            checkBoxDMConsoleOnly.Checked = checkBoxDMConsoleOnly.Enabled = SQLdmDetails.ShouldConsoleOptionBeEnabled;
            checkBoxDMServicesAndRepository.Checked = checkBoxDMServicesAndRepository.Enabled = SQLdmDetails.ShouldServicesOptionBeEnabled;
            checkBoxInstallDashbaord.Checked = IderaDashboardDetails.IsDashboardInstalledOnLocal ? IderaDashboardDetails.ShouldDashboardOptionBeEnabled : false;
            checkBoxInstallDashbaord.Enabled = IderaDashboardDetails.ShouldDashboardOptionBeEnabled;

            LOG.InfoFormat("ShouldConsoleOptionBeEnabled:{0}, ShouldServicesOptionBeEnabled:{1}, ShouldDashboardOptionBeEnabled:{2}", 
                SQLdmDetails.ShouldConsoleOptionBeEnabled.ToString(), 
                SQLdmDetails.ShouldServicesOptionBeEnabled.ToString(), 
                IderaDashboardDetails.ShouldDashboardOptionBeEnabled.ToString());
#if TRIAL

#else 
           
            labelForDashboardReInstall.Visible = IderaDashboardDetails.IsDashboardInstalledOnLocal;

#endif //TRIAL
            labelForSqlDmReInstall.Visible = SQLdmDetails.IsDmInstalledOnLocal;
        }
        
        private void InstallOption_Load(object sender, EventArgs e)
        {
            this.Enabled = false;
            backgroundWorkerForPopulatingData.RunWorkerAsync();                               
        }

        private void backgroundWorkerForPopulatingData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            ConfigureUI();
            LoadUI();
        }

        private void backgroundWorkerForPopulatingData_DoWork(object sender, DoWorkEventArgs e)
        {            
            SQLdmDetails.PopulateDetails();
            IderaDashboardDetails.PopulateDetails();
        }
    }
}
