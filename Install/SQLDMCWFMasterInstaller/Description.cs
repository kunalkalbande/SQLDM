using CWFInstallerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BBS.TracerX;

namespace Installer_form_application
{
    
    public partial class Description : Form
    {
        private Logger LOG = Logger.GetLogger("Description");
        bool askForNewPath = false;
        Form screenObject;
        public Description(Form screenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void ConfigureUIForLocalRegistration()
        {
            labelPathSP.Visible = true;
            textBoxSPPath.Visible = true;
            labelPathID.Visible = true;
            textBoxIDPath.Visible = true;
            labelInstanceDesc.Visible = true;
            labelInstanceExpl.Visible = true;
            labelDisplayName.Visible = true;
            textBoxDisplayName.Visible = true;
            checkAvailabilityButton.Visible = true;
            availabilityLabel.Visible = properties.isDisplayNameAvailable;

            textBoxSPPath.Enabled = !SQLdmDetails.IsDmInstalledOnLocal;
            textBoxIDPath.Enabled = !IderaDashboardDetails.IsDashboardInstalledOnLocal;
            textBoxDisplayName.Enabled = true;

            buttonNext.Enabled = properties.isDisplayNameAvailable;
        }

        private void LoadUIForLocalRegistration()
        {
            if (SQLdmDetails.PathForInstalledVersion != string.Empty)
                properties.dmInstallationPath = SQLdmDetails.PathForInstalledVersion;
                        
            if (IderaDashboardDetails.PathForInstalledVersion != string.Empty)
                properties.idInstallationPath = IderaDashboardDetails.PathForInstalledVersion;

            textBoxSPPath.Text = properties.dmInstallationPath;
            textBoxIDPath.Text = properties.idInstallationPath;            
            textBoxDisplayName.Text = properties.DisplayName;
            // SQLDM-28056 (Varun Chopra) Update Radio Buttons
            LoadApplicationUsersProperties();
        }

        /// <summary>
        /// Load Application Users Properties and update Radio Buttons
        /// </summary>
        /// <remarks>
        /// SQLDM-28056 (Varun Chopra) Creating Shortcuts for all users
        /// </remarks>
        private void LoadApplicationUsersProperties()
        {
            radioButtonAllUsers.Checked = properties.allUsers;
            radioButtonCurrentUser.Checked = !properties.allUsers;
        }

        private void ConfigureUIForRemoteRegistration()
        {
            labelPathSP.Visible = true;
            textBoxSPPath.Visible = true;
            labelPathID.Visible = false;
            textBoxIDPath.Visible = false;
            labelInstanceDesc.Visible = true;
            labelInstanceExpl.Visible = true;
            labelDisplayName.Visible = true;
            textBoxDisplayName.Visible = true;
            checkAvailabilityButton.Visible = true;
            availabilityLabel.Visible = properties.isDisplayNameAvailable;
            
            textBoxSPPath.Enabled = !SQLdmDetails.IsDmInstalledOnLocal;
            textBoxIDPath.Enabled = false;
            textBoxDisplayName.Enabled = true;

            buttonNext.Enabled = properties.isDisplayNameAvailable;
        }

        private void LoadUIForRemoteRegistration()
        {
            if (SQLdmDetails.PathForInstalledVersion != string.Empty)
                properties.dmInstallationPath = SQLdmDetails.PathForInstalledVersion;

            properties.idInstallationPath = string.Empty;

            textBoxIDPath.Text = properties.idInstallationPath;
            textBoxSPPath.Text = properties.dmInstallationPath;
            textBoxDisplayName.Text = properties.DisplayName;
            // SQLDM-28056 (Varun Chopra) Update Radio Buttons
            LoadApplicationUsersProperties();
        }

        private void ConfigureUIForConsoleOnlyInstallation()
        {
            textBoxDisplayName.Visible = false;
            labelInstanceExpl.Visible = false;
            labelDisplayName.Visible = false;
            labelInstanceDesc.Visible = false;
            textBoxIDPath.Visible = false;
            labelPathID.Visible = false;
            checkAvailabilityButton.Visible = false;
            availabilityLabel.Visible = false;

            textBoxSPPath.Enabled = !SQLdmDetails.IsDmInstalledOnLocal;
        }

        private void LoadUIForConsoleOnlyInstallation()
        {
            if (SQLdmDetails.PathForInstalledVersion != string.Empty)
                properties.dmInstallationPath = SQLdmDetails.PathForInstalledVersion;

            textBoxSPPath.Text = properties.dmInstallationPath;
            textBoxDisplayName.Text = properties.DisplayName;
            // SQLDM-28056 (Varun Chopra) Update Radio Buttons
            LoadApplicationUsersProperties();
            if (textBoxSPPath.Text == string.Empty)
                buttonNext.Enabled = false;
        }

        private void ConfigureUIForNoRegisteration()
        {
            if (properties.installSQLDMServiceAndRepositoryOption || properties.installSQLDMConsoleOption)
            {
                labelPathSP.Visible = true;
                textBoxSPPath.Visible = true;
            }
            else
            {
                labelPathSP.Visible = false;
                textBoxSPPath.Visible = false;
            }

            if (properties.installDashboardOption)
            {
                labelPathID.Visible = true;
                textBoxIDPath.Visible = true;
            }
            else
            {
                labelPathID.Visible = false;
                textBoxIDPath.Visible = false;
            }

            textBoxSPPath.Enabled = !SQLdmDetails.IsDmInstalledOnLocal;
            textBoxIDPath.Enabled = !IderaDashboardDetails.IsDashboardInstalledOnLocal;

            labelInstanceDesc.Visible = false;
            labelInstanceExpl.Visible = false;
            labelDisplayName.Visible = false;
            textBoxDisplayName.Visible = false;
            checkAvailabilityButton.Visible = false;
            availabilityLabel.Visible = false;
        }

        private void LoadUIForNoRegistration()
        {
            if (SQLdmDetails.PathForInstalledVersion != string.Empty)
                properties.dmInstallationPath = SQLdmDetails.PathForInstalledVersion;

            if (IderaDashboardDetails.PathForInstalledVersion != string.Empty)
                properties.idInstallationPath = IderaDashboardDetails.PathForInstalledVersion;

            textBoxSPPath.Text = properties.dmInstallationPath;
            textBoxIDPath.Text = properties.idInstallationPath;
            // SQLDM-28056 (Varun Chopra) Update Radio Buttons
            LoadApplicationUsersProperties();
        }

        private void Description_Load(object sender, EventArgs e)
        {

#if TRIAL

            LOG.InfoFormat("Description_Load- No Registeration is happenning");
            ConfigureUIForNoRegisteration();
            LoadUIForNoRegistration();
            buttonNext.Enabled = true;

#else

            if (properties.localRegister)
            {
                LOG.InfoFormat("Description_Load- Local Registeration is happenning");
                ConfigureUIForLocalRegistration();
                LoadUIForLocalRegistration();
            }
            else if (properties.remoteRegister)
            {
                LOG.InfoFormat("Description_Load- Remote Registeration is happenning");
                ConfigureUIForRemoteRegistration();
                LoadUIForRemoteRegistration();
            }
            else
            {
                LOG.InfoFormat("Description_Load- No Registeration is happenning");
                ConfigureUIForNoRegisteration();
                LoadUIForNoRegistration();
            }

            this.textBoxSPPath.SelectionStart = 0;
            textBoxSPPath.Focus();
            //adding it here, bcz we dont want this event to remain enabled during page load.
            this.textBoxDisplayName.TextChanged += new System.EventHandler(this.displayNameTextBox_TextChanged);

#endif //TRIAL

        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.idInstallationPath = textBoxIDPath.Text;
            properties.dmInstallationPath = textBoxSPPath.Text;
            properties.DisplayName = textBoxDisplayName.Text;
            // SQLDM-28056 (Varun Chopra) Update All Users Properties
            SaveApplicationUsersProperties();
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.DisplayName = textBoxDisplayName.Text.Trim();
            properties.idInstallationPath = textBoxIDPath.Text;
            properties.dmInstallationPath = textBoxSPPath.Text;
            // SQLDM-28056 (Varun Chopra) Update All Users Properties
            SaveApplicationUsersProperties();
            this.Cursor = Cursors.WaitCursor;
            this.buttonNext.Enabled = false;
            this.buttonBack.Enabled = false;           
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Save Application Users Properties with Radio Buttons
        /// </summary>
        /// <remarks>
        /// SQLDM-28056 (Varun Chopra) Creating Shortcuts for all users
        /// </remarks>
        private void SaveApplicationUsersProperties()
        {
            properties.allUsers = radioButtonAllUsers.Checked;
        }

        private void Description_FormClosing(object sender, FormClosingEventArgs e)
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

        private bool checkIfTextboxesAreEmpty()
        {
            return (textBoxSPPath.Visible && textBoxSPPath.Text == string.Empty)
                || (textBoxIDPath.Visible && textBoxIDPath.Text == string.Empty)
                || (textBoxDisplayName.Visible && textBoxDisplayName.Text == string.Empty);
        }

        //as soon as user edit the display name, again check for it's availability
        private void displayNameTextBox_TextChanged(object sender, EventArgs e)
        {
            buttonNext.Enabled = false;
            properties.isDisplayNameAvailable = false;
            availabilityLabel.Visible = false;
        }
        
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (properties.notRegister)
            {
                buttonNext.Enabled = !checkIfTextboxesAreEmpty();
            }
            else
            {
                buttonNext.Enabled = !checkIfTextboxesAreEmpty() && properties.isDisplayNameAvailable;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.buttonNext.Enabled = true;
            this.buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;

            if (askForNewPath)
            {
                if (DialogResult.No == MessageBox.Show("The entered path(s) doesn't exist, shall we continue creating the required folders?", "Path Not Exist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    return;
            }

            if (e.Result != null && e.Result.ToString() != string.Empty)
            {
                LOG.ErrorFormat("Description.backgroundWorker1_RunWorkerCompleted- Exception: {0}",e.Result.ToString());
                MessageBox.Show(e.Result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide();
            InstallReady nextScreen = new InstallReady(this);
            nextScreen.Show();
		}

        private void ValidatePath(string dirPath)
        {
            if (!Path.IsPathRooted(dirPath))
            {
                throw new InvalidPathException();
            }
            askForNewPath |= !Directory.Exists(dirPath);  //If the path does not exist, than create a new one
        }        

        private void ValidateDirectoryPaths()
        {
            if (properties.installDashboardOption && !IderaDashboardDetails.IsDashboardInstalledOnLocal) //Fresh installation of ID in local 
            {
                ValidatePath(properties.idInstallationPath);
            }

            if ((properties.installSQLDMConsoleOption || properties.installSQLDMServiceAndRepositoryOption) && !SQLdmDetails.IsDmInstalledOnLocal)
            {
                ValidatePath(properties.dmInstallationPath);
            }
        }

        private void ValidateDiskSpace()
        {
            string driveNameForDm = properties.dmInstallationPath.Split('\\')[0];
            string driveNameForId = properties.idInstallationPath.Split('\\')[0];
            
            if(properties.installDashboardOption)
                Validator.ValidateDiskSpaceForDashboard(driveNameForId);

            if (properties.installSQLDMConsoleOption || properties.installSQLDMServiceAndRepositoryOption)
            {
                SQLdmHelper.ValidateDiskSpaceForSelectedComponents(
                    driveNameForDm,
                    properties.installDashboardOption && driveNameForDm==driveNameForId,
                    properties.installSQLDMConsoleOption && !SQLdmDetails.IfConsoleIsInstalledOnLocal,  //dont check for the component which is already installed
                    properties.installSQLDMServiceAndRepositoryOption && !SQLdmDetails.IfServicesAreInstalledOnLocal
                    );
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ValidateDirectoryPaths();
                
                properties.idInstallationPath = properties.idInstallationPath.TrimEnd('\\');
                properties.dmInstallationPath = properties.dmInstallationPath.TrimEnd('\\');                
                
                ValidateDiskSpace();
            }
            catch (CWFBaseException ex)
            {
                e.Result = ex.ErrorCode + " - " + ex.ErrorMessage;                                     
                return;
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
                return;
            }
        }

        private void UpdateUIAsPerAvailability(bool available)
        {
            this.Cursor = Cursors.Default;
            if (available)
            {
                properties.isDisplayNameAvailable = true;
                availabilityLabel.Visible = true;
                buttonNext.Enabled = true;
            }
            else
            {
                //fixing SQLDM-26647
                if (DialogResult.Yes == MessageBox.Show("An instance of SQLdm is already registered with this display name, please confirm if the target dashboard is the same one with which existing version of SQLdm is already registered.", "Display Name Already Exist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    UpdateUIAsPerAvailability(true);
                else
                {
                    properties.isDisplayNameAvailable = false;
                    availabilityLabel.Visible = false;
                    buttonNext.Enabled = false;
                }
            }
        }

        private void checkAvailabilityButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                Validator.validateInstanceName(textBoxDisplayName.Text.Trim());
            }
            catch (CWFBaseException ex)
            {
                this.Cursor = Cursors.Default;
                LOG.InfoFormat("Invalid Display Name: {0}",ex.Message);
                //fixing SQLDM-26643
                MessageBox.Show("The Display Name you provided is invalid, it needs to be alphanumeric(a-z,A-Z,0-9) only. Please enter a valid Display Name.", "Invalid Display Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (properties.localRegister && IderaDashboardDetails.IsDashboardInstalledOnLocal) //upgradation while local registeration
            {
                UpdateUIAsPerAvailability(!IderaDashboardHelper.DoesDisplayNameAlreadyExist("localhost", IderaDashboardDetails.ServicePortForInstalledVersion, textBoxDisplayName.Text.Trim(), properties.IDSUsername, properties.IDSPassword));
            }
            else if (properties.remoteRegister)
            {
                UpdateUIAsPerAvailability(!IderaDashboardHelper.DoesDisplayNameAlreadyExist(properties.remoteHostname, properties.remoteCoreServicePortNumber, textBoxDisplayName.Text.Trim(), properties.remoteDashboardServiceUsername, properties.remoteDashboardServicePassword));
            }
            else
                UpdateUIAsPerAvailability(true);
        }
    }
}
