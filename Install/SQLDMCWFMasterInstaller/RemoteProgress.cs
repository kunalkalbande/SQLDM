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
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.Security.Encryption;
using System.Text.RegularExpressions;

namespace Installer_form_application
{
    public partial class RemoteProgress : Form
    {
        Form backScreen;
        const string SQLDM_INSTALLER_NAME_64 = @"SQLDiagnosticManager-x64.exe";
        const string SQLDM_INSTALLER_NAME = @"SQLDiagnosticManager.exe";
        const string PRODUCT_NAME_IDERA_DASHBOARD = "Idera Dashboard";//make sure case does not change
        const string PRODUCT_NAME_SQLDM_64 = "IDERA SQL Diagnostic Manager (x64)";//make sure case does not change
        const string PRODUCT_NAME_SQLDM = "IDERA SQL diagnostic manager";//make sure case does not change
        const string DASHBOARD_INSTALLER_NAME = @"IderaDashboard.msi";
        const string SMO_INSTALLER_NAME = @"SharedManagementObjects.msi";   // SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
        const string SQL_CLR_TYPES_INSTALLER_NAME = @"SQLSysCLrTypes.msi";
        const int PERC_VALUE_FOR_VALIDATING_PRODUCTS = 101;
        readonly string INSTALLER_BASE_DIR = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
        readonly string DASHBOARD_INSTALLER_PATH;
        readonly string SQLDM_INSTALLER_PATH;
        readonly string SMO_INSTALLER_PATH;
        readonly string SQL_CLR_TYPE_INSTALLER_PATH;

        string SQLdmProductName = PRODUCT_NAME_SQLDM_64;
        string SQLdmInstallerName = SQLDM_INSTALLER_NAME_64;

        const string MSG_DASHBOARD_UNSUCCESSFULL = "\nIDERA Dashboard installation was unsuccessfull.";
        const string MSG_SQLDM_UNSUCCESSFULL = "\nIDERA SQLdm Installation was unsuccessfull.";
        const string MSG_DASHBOARD_SUCCESSFULL = "\nIDERA Dashboard has been successfully installed.";
        const string MSG_SQLDM_SUCCESSFULL = "\nIDERA SQLdm has been successfully installed.";

        const string InstallMsgSqlDm = "Installing IDERA SQL Diagnostic Manager...";
        const string InstallMsgDashboard = "Installing IDERA Dashboard...";
        const string ALL_USERS = "AllUsers";
        const string ONLY_CURRENT_USER = "OnlyCurrentUser";
        private bool installationTriedForSQLdm = false;
        private bool installationTriedForDashboard = false;
        private bool installationTriedForSMO = false;
        private bool installationTriedForSqlClrTypes = false;

        System.Diagnostics.Process processForDashboard = new System.Diagnostics.Process();
        System.Diagnostics.Process processForSQLdm = new System.Diagnostics.Process();        

        string error = string.Empty;
        private Logger LOG = Logger.GetLogger("RemoteProgress");
        public RemoteProgress(Form backScreenObj)
        {
            InitializeComponent();

            AcceptButton = buttonNext;
            CancelButton = buttonCancel;

            backScreen = backScreenObj;
            if (Environment.Is64BitOperatingSystem)
            {
                SQLdmInstallerName = SQLDM_INSTALLER_NAME_64;
                SQLdmProductName = PRODUCT_NAME_SQLDM_64;
            }
            else
            {
                SQLdmInstallerName = SQLDM_INSTALLER_NAME;
                SQLdmProductName = PRODUCT_NAME_SQLDM;
            }

            DASHBOARD_INSTALLER_PATH = Path.Combine(INSTALLER_BASE_DIR, DASHBOARD_INSTALLER_NAME);
            SQLDM_INSTALLER_PATH = Path.Combine(INSTALLER_BASE_DIR, SQLdmInstallerName);            
            SMO_INSTALLER_PATH = Path.Combine(INSTALLER_BASE_DIR, SMO_INSTALLER_NAME); // SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
            SQL_CLR_TYPE_INSTALLER_PATH = Path.Combine(INSTALLER_BASE_DIR, SQL_CLR_TYPES_INSTALLER_NAME);
        }

        private void CancelClosingEvent(Object sender, CancelEventArgs e) //This event would handle cancellation of windows form
        {
            MessageBox.Show("Installation is in progress, closing it in between may put the software in unstable state.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;                
        }

        private void RePopulatingDashbaordAndDmDetails()
        {
            SQLdmDetails.PopulateDetails();
            IderaDashboardDetails.PopulateDetails();
        }

        private void RemoteProgress_Shown(Object sender, EventArgs e)
        {
            if (properties.remoteRegister)   //remote registration
                backgroundWorkerRemote.RunWorkerAsync(); 
            else
                backgroundWorkerLocal.RunWorkerAsync();
        }

        public void setProgressStatus(string status)
        {
            if (labelProgress.InvokeRequired)
            {
                labelProgress.Invoke(new Action<String>(setProgressStatus), new object[] { status });
            }
            else
                labelProgress.Text = status;
        }

        private void installProduct()
        {         
            if (properties.installDashboardOption) //Dashboard Installation
            {
                if (System.IO.File.Exists(DASHBOARD_INSTALLER_PATH))
                {
                    setProgressStatus(InstallMsgDashboard);
                    LOG.Info("\nDashBoard Installation Started: " + DateTime.Now.ToString());
                    installDashboard();
                    LOG.Info("\nDashBoard Installation Ended: " + DateTime.Now.ToString());
                    installationTriedForDashboard = true;
                    backgroundWorkerLocal.ReportProgress(GetProgressPercentage());
                }
                else
                {
                    LOG.ErrorFormat("RemoteProgress.installProduct()- Missing Dashboard File: {0}",DASHBOARD_INSTALLER_PATH);
                    MessageBox.Show("Installer files are missing for IDERA Dashboard, please download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            if ((properties.installSQLDMConsoleOption || properties.installSQLDMServiceAndRepositoryOption)) //SQLdm Installation
            {
                if (System.IO.File.Exists(SQLDM_INSTALLER_PATH))
                {
                    setProgressStatus(InstallMsgSqlDm);
                    LOG.Info("\nDM Installation Started: " + DateTime.Now.ToString());
                    installSQLDM();
                    LOG.Info("\nDM Installation Ended: " + DateTime.Now.ToString());
                    installationTriedForSQLdm = true;
                    backgroundWorkerLocal.ReportProgress(GetProgressPercentage());
                }
                else
                {
                    LOG.ErrorFormat("RemoteProgress.installProduct()- Missing SQLdm File: {0}", SQLDM_INSTALLER_PATH);
                    MessageBox.Show("Installer files are missing for IDERA SQL Diagnostic Manager, please download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }            
            TrySqlClrTypesInstall(); // SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
            TrySMOInstall();
        }

        /// <summary>
        /// /SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
        /// Try to Install SqlClrTypes msi.
        /// </summary>
        private void TrySqlClrTypesInstall()
        {
            if (System.IO.File.Exists(SQL_CLR_TYPE_INSTALLER_PATH))
            {
                LOG.Info("\nSqlClrTypes Installation Started: " + DateTime.Now.ToString());
                InstallGenericMSI(SQL_CLR_TYPE_INSTALLER_PATH, "InstallSqlServerTypes");
                LOG.Info("\nSqlClrTypes Installation Ended: " + DateTime.Now.ToString());
                installationTriedForSqlClrTypes = true;
                backgroundWorkerLocal.ReportProgress(GetProgressPercentage());
            }
            else
            {
                LOG.ErrorFormat("RemoteProgress.TrySqlClrTypesInstall() - Missing SqlClrTypes File: {0}", SQL_CLR_TYPE_INSTALLER_PATH);
                MessageBox.Show("Installer files are missing for SQLSysCLrTypes, please download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
        /// Try to Install SMO msi.
        /// </summary>
        private void TrySMOInstall()
        {
            if (System.IO.File.Exists(SMO_INSTALLER_PATH))
            {
                LOG.Info("\nSMO Installation Started: " + DateTime.Now.ToString());
                InstallGenericMSI(SMO_INSTALLER_PATH, "InstallSMO");
                LOG.Info("\nSMO Installation Ended: " + DateTime.Now.ToString());
                installationTriedForSMO = true;
                backgroundWorkerLocal.ReportProgress(GetProgressPercentage());
            }
            else
            {
                LOG.ErrorFormat("RemoteProgress.TrySMOInstall() - Missing SMO File: {0}", SMO_INSTALLER_PATH);
                MessageBox.Show("Installer files are missing for Shared Management Objects (SMO), please download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void installDashboard()
        {            
            RegistryKey cwfkey = null;
            string logString;
            RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            try
            {
                cwfkey = localMachineX64View.OpenSubKey(@"SOFTWARE\Idera\CWF");
            }
            catch (System.Security.SecurityException secex)
            {
                MessageBox.Show("Please run this installer as Admin user" + " Security Exception: " + secex.ToString());
                Application.Exit();
            }

            string productArgs = " /l*V InstallDashboard.log /quiet AGREETOLICENSE=yes SERVICE_ACCOUNT=" + properties.IDSUsername;
            productArgs += " SERVICE_PASSWORD=" + HandleDoubleQuotes(properties.IDSPassword);
            productArgs += " INSTALLDIR=" + HandleDoubleQuotes(properties.idInstallationPath);


            if (!IderaDashboardDetails.IsDashboardInstalledOnLocal) //fresh installation
            {
                productArgs += " REPOSITORY_CORE_DATABASE=" + HandleDoubleQuotes(properties.IDDBName) + " REPOSITORY_INSTANCE=" + HandleDoubleQuotes(properties.IDInstance);
                productArgs += " WEBAPP_PORT=" + HandleDoubleQuotes(properties.WebAppServicePort) + " WEBAPP_MONITOR_PORT=" + HandleDoubleQuotes(properties.WebAppMonitorPort);
                productArgs += " WEBAPP_SSL_PORT=" + HandleDoubleQuotes(properties.WebAppSSLPort) + " COLLECTION_PORT=" + HandleDoubleQuotes(properties.CoreServicesPort);                
            }

            if (properties.needSqlAuthForConnectingToId)
            {
                productArgs += " REPOSITORY_SQLAUTH=1 REPOSITORY_SQLPASSWORD=" + HandleDoubleQuotes(properties.SQLPasswordID)+" REPOSITORY_SQLUSERNAME=" + HandleDoubleQuotes(properties.SQLUsernameID);
            }
            // SQLDM-28056 (Varun Chopra) Creating Shortcuts for All users / Current Users
            productArgs = SetApplicationUsers(productArgs);

            productArgs = " /i " + HandleDoubleQuotes(DASHBOARD_INSTALLER_PATH) + "" + productArgs;
            //SQLdm 10.1(srishti purohit)
            //Idera defect fix "password of the service account visible in logs"
            //Fixing: SQLDM-26688 - Masking Passwords
            logString = productArgs.ToString();
            logString = logString.Replace(" SERVICE_PASSWORD=" + HandleDoubleQuotes(properties.IDSPassword), " SERVICE_PASSWORD=" + "*****");
            logString = logString.Replace(" REPOSITORY_SQLPASSWORD=" + HandleDoubleQuotes(properties.SQLPasswordID), " REPOSITORY_SQLPASSWORD=" + "*****");                  
            LOG.InfoFormat("RemoteProgress.installDashboard()- ProductArgs: {0}", logString);
            installNewMSI(productArgs);                     
        }

        /// <summary>
        /// Updates product args to install for all users / current user based on value selected in <see cref="Description"/> page
        /// </summary>
        /// <param name="productArgs"> Add Product Args for Installation</param>
        /// <returns>
        /// Updated Product Arguments
        /// </returns>
        /// <remarks>
        /// SQLDM-28056 (Varun Chopra) Creating Shortcuts for All users / Current Users
        /// </remarks>
        private static string SetApplicationUsers(string productArgs)
        {
            if (properties.allUsers)
            {
                productArgs += " ApplicationUsers=" + ALL_USERS + " ALLUSERS=1";
            }
            else
            {
                productArgs += " ApplicationUsers=" + ONLY_CURRENT_USER;
            }
            return productArgs;
        }

        private void installSQLDM()
        {
            string logString;

            string productArgs = "/l*V InstallSQLDM.log";
            //SQLDM 10.1 (Barkha Khatri)
            //SQLDM-26419 fix -- adding INSTALLDIR in product args
            productArgs += " INSTALLDIR=\"\"" + HandleDoubleQuotes(properties.dmInstallationPath) + "\"\"";
            if (properties.installSQLDMServiceAndRepositoryOption)
            {
                productArgs += " INSTANCE=" + HandleDoubleQuotes(properties.DMInstance);
                productArgs += " REPOSITORY=" + HandleDoubleQuotes(properties.DMDBName);
                productArgs += " SERVICEUSERNAME=" + HandleBackSlashInRemoveEscapeChars(HandleDoubleQuotes(properties.SPSUsername));
                productArgs += " SERVICEPASSWORD=" + RemoveEscapeChars(HandleDoubleQuotes( properties.SPSPassword));
            }

            if (properties.installSQLDMConsoleOption && properties.installSQLDMServiceAndRepositoryOption)
            {
                productArgs += " SETUPTYPE=Typical";
            }
            else if (properties.installSQLDMConsoleOption)
            {
                productArgs += " SETUPTYPE=Console";
            }
            else
            {
                productArgs += " SETUPTYPE=Services";
            }            

            if (properties.needSqlAuthForConnectingToDmRepo)
            {
                productArgs += " SQLSERVER_AUTHENTICATION=1";
                productArgs += " SQLSERVER_PASSWORD=" + HandleDoubleQuotes(properties.SQLPassword_DM_Repo);
                productArgs += " SQLSERVER_USERNAME=" + HandleDoubleQuotes(properties.SQLUsername_DM_Repo);                
            }

            if (properties.needSqlAuthForConnectingToDmService)
            {
                productArgs += " SERVICE_SQLAUTHENTICATION=1";
                productArgs += " SERVICE_SQL_PASSWORD=" + HandleDoubleQuotes(properties.SQLPassword_DM_Service);
                productArgs += " SERVICE_SQL_USERNAME=" + HandleDoubleQuotes(properties.SQLUsername_DM_Service);                 
            }

            if (properties.localRegister) //register with local dashboard
            {
                if (!string.IsNullOrEmpty(properties.DisplayName))
                    productArgs += " CWF_INSTANCE_NAME=" + HandleDoubleQuotes(properties.DisplayName);                

                if (!string.IsNullOrEmpty(properties.CoreServicesPort))
                    productArgs += " CWF_PORT=" + HandleDoubleQuotes(properties.CoreServicesPort);

                productArgs += " CWF_HOSTNAME=" + HandleDoubleQuotes(System.Environment.MachineName);

                if (!string.IsNullOrEmpty(properties.IDSUsername))
                    productArgs += " CWF_SERVICE_USER_NAME=" + HandleDoubleQuotes(properties.IDSUsername);               

                if (!string.IsNullOrEmpty(properties.IDSPassword))
                    productArgs += " CWF_SERVICE_PASSWORD=" + HandleDoubleQuotes(EncryptionHelper.QuickEncrypt(properties.IDSPassword));                
            }

            if (properties.remoteRegister)
            {
                if (!string.IsNullOrEmpty(properties.DisplayName))
                    productArgs += " CWF_INSTANCE_NAME=" + HandleDoubleQuotes(properties.DisplayName);

                if (!string.IsNullOrEmpty(properties.CoreServicesPort))
                    productArgs += " CWF_PORT=" + HandleDoubleQuotes(properties.CoreServicesPort);

                if (!string.IsNullOrEmpty(properties.remoteHostname))
                    productArgs += " CWF_HOSTNAME=" + HandleDoubleQuotes(properties.remoteHostname);                

                if (!string.IsNullOrEmpty(properties.remoteDashboardServiceUsername))                
                    productArgs += " CWF_SERVICE_USER_NAME=" + HandleDoubleQuotes(properties.remoteDashboardServiceUsername);                

                if (!string.IsNullOrEmpty(properties.remoteDashboardServicePassword))
                    productArgs += " CWF_SERVICE_PASSWORD=" + HandleDoubleQuotes(EncryptionHelper.QuickEncrypt(properties.remoteDashboardServicePassword));                
            }

            if (properties.ValidLicenseKeys.Length > 0)
            {
                productArgs += " NEW_LICENSE_KEYS=" + HandleDoubleQuotes(properties.ValidLicenseKeys);
            }
            // SQLDM-28056 (Varun Chopra) Creating Shortcuts for All users / Current Users
            productArgs = SetApplicationUsers(productArgs);

            productArgs += " /quiet";
            productArgs = " /v" + "\"" + productArgs + "\"";    
        
            //removing passwords from logString
            //Fixing: SQLDM-26688 - Masking Passwords
            logString = productArgs.ToString();
            logString = logString.Replace(" SERVICEPASSWORD=" + HandleDoubleQuotes(properties.SPSPassword), " SERVICEPASSWORD="+"*****");
            logString = logString.Replace(" SQLSERVER_PASSWORD=" + HandleDoubleQuotes(properties.SQLPassword_DM_Repo), " SQLSERVER_PASSWORD=" + "*****");
            logString = logString.Replace(" SERVICE_SQL_PASSWORD=" + RemoveEscapeChars(HandleDoubleQuotes(properties.SQLPassword_DM_Service)), " SERVICE_SQL_PASSWORD=" + "*****");            

            LOG.InfoFormat("RemoteProgress.installSQLDM()- ProductArgs: {0}",logString);
            executeExe(productArgs, SQLDM_INSTALLER_PATH);
        }

        /// <summary>
        ///  SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
        ///  Install any MSI in System Directory.
        /// </summary>
        private void InstallGenericMSI(string installerPath, string logName)
        {
            string windir = Environment.SystemDirectory; 
            string windrive = Path.GetPathRoot(Environment.SystemDirectory); 

            string productArgs = " /l*V " + logName + ".log /quiet /norestart AGREETOLICENSE=yes ROOTDRIVE=" + windrive + " ";
            productArgs = " /i " + HandleDoubleQuotes(installerPath) + "" + productArgs;
            LOG.InfoFormat("RemoteProgress.InstallGenericMSI() - ProductArgs: {0}", productArgs);
            installNewMSI(productArgs);
        }

        /// <summary>
        /// handles double quotes within the data and wraps the whole string in double quotes
        /// </summary>
        /// <param name="toBeDoubleQuoted"></param>
        /// <returns></returns>
        private static string HandleDoubleQuotes(string toBeDoubleQuoted)
        {
            return toBeDoubleQuoted != string.Empty ? (toBeDoubleQuoted.StartsWith("\"") && toBeDoubleQuoted.EndsWith("\"")) ? toBeDoubleQuoted : "\"" + toBeDoubleQuoted.Replace("\"", "\\" + "\"") + "\"" : string.Empty;
        }

        private static string HandleBackSlashInRemoveEscapeChars(string username)
        {
            String pattern = @"\\";
            Match m = Regex.Match(username, pattern);
            string initialUserName = username.Substring(0, m.Index) + username.Substring(m.Index + 1);
            String convertedUsername = RemoveEscapeChars(initialUserName);
            convertedUsername = convertedUsername.Substring(0, m.Index) + "\\" + convertedUsername.Substring(m.Index);
            return convertedUsername;
        }

        //SQLDM-29408. Handle Special Characters in Password string
        private static string RemoveEscapeChars(string password)
        {
            return Regex.Unescape(password);
        }
        
        private void installNewMSI(string productArgs)
        {
            processForDashboard.StartInfo.FileName = @"msiexec";
            processForDashboard.StartInfo.Arguments = productArgs;
            processForDashboard.Start();
            processForDashboard.WaitForExit();
            processForDashboard.Close();
        }
        
        private void executeExe(string productArgs, string filename)
        {
            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            processForSQLdm.StartInfo.FileName = filename;
            processForSQLdm.StartInfo.Arguments = productArgs;
            processForSQLdm.Start();
            processForSQLdm.WaitForExit();
            int cec = processForSQLdm.ExitCode;
            LOG.Info("executeExe:  Exit code:  " + cec.ToString());
            processForSQLdm.Close();
            /* Commenting the code
            if(cec != 0)
            {
                MessageBox.Show("Running a second time.  Click OK to proceed.....");
                LOG.Info("cec not zero.  Let's run the installer again.");
                System.Diagnostics.Process p2 = new System.Diagnostics.Process();
                p2.StartInfo.FileName = filename;
                p2.StartInfo.Arguments = productArgs;
                p2.Start();
                p2.WaitForExit();
                int cec2 = p2.ExitCode;
                LOG.Info("executeExe(Second Pass):  Exit code:  " + cec2.ToString());
                p2.Close();

            }*/
        }

        private void backgroundWorkerRemote_DoWork(object sender, DoWorkEventArgs e)
        {
            LOG.Info("REMOTE INSTALL STARTED :" + DateTime.Now.ToString());
            installProduct();
            LOG.Info("REMOTE INSTALL COMPLETED :" + DateTime.Now.ToString());

            RePopulatingDashbaordAndDmDetails();
            backgroundWorkerLocal.ReportProgress(PERC_VALUE_FOR_VALIDATING_PRODUCTS);
            System.Threading.Thread.Sleep(2000); //self intended 2 sec delay, for enhancing the user expereince            

            bool checkDmSuccess = (Environment.Is64BitOperatingSystem ? InstallationHelper.checkIfProductInstalledSuccessfully(PRODUCT_NAME_SQLDM_64) : InstallationHelper.checkIfProductInstalledSuccessfully(PRODUCT_NAME_SQLDM))
                &&
                SQLdmDetails.IsLatestVersionInstalled
                &&
                properties.installSQLDMConsoleOption? SQLdmDetails.IfConsoleIsInstalledOnLocal:true
                &&
                properties.installSQLDMServiceAndRepositoryOption? SQLdmDetails.IfServicesAreInstalledOnLocal:true;

            // update dm success for retry attempts on failed upgrades
            properties.CheckDmSuccess = checkDmSuccess;
            properties.RetryCountSQLdmUpgrade++;

            if (!checkDmSuccess)
            {
                error = MSG_SQLDM_UNSUCCESSFULL;
                e.Result = false;
            }
            else
                e.Result = true;

            if (error != String.Empty)
            {
                LOG.ErrorFormat("RemoteProgress.backgroundWorkerRemote_DoWork()- Installation Failed... CheckDmSuccess: {0}, Error:{1}", checkDmSuccess.ToString(), error);
            }
            else
                LOG.InfoFormat("RemoteProgress.backgroundWorkerRemote_DoWork()- Installation is successfull.");
        }

        private void backgroundWorkerRemote_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == PERC_VALUE_FOR_VALIDATING_PRODUCTS)
            {
                setProgressStatus("Validating Installed Products...");                
                return;
            }

            if (e.ProgressPercentage == 0)
            {
                progressBar.Value = 0;
                labelForPerc.Text = string.Empty;
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = e.ProgressPercentage;
                labelForPerc.Text = e.ProgressPercentage.ToString() + "%";
            }
        }

        private void backgroundWorkerLocal_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = false;
            Cursor.Current = Cursors.WaitCursor;
            LOG.Info("LOCAL INSTALL STARTED :" + DateTime.Now.ToString());
            installProduct();
            LOG.Info("LOCAL INSTALL COMPLETED :" + DateTime.Now.ToString());

            RePopulatingDashbaordAndDmDetails();
            backgroundWorkerLocal.ReportProgress(PERC_VALUE_FOR_VALIDATING_PRODUCTS);
            System.Threading.Thread.Sleep(2000); //self intended 2 sec delay, for enhancing the user expereince            

            bool checkDmSuccess = true, checkIdSuccess = true;

            if (properties.installDashboardOption)
            {
                checkIdSuccess = InstallationHelper.checkIfProductInstalledSuccessfully(PRODUCT_NAME_IDERA_DASHBOARD)
                    && !IderaDashboardDetails.IsUpgradationRequiredOnLocal;
            }

            if (properties.installSQLDMServiceAndRepositoryOption || properties.installSQLDMConsoleOption)
            {
                checkDmSuccess = (Environment.Is64BitOperatingSystem ? InstallationHelper.checkIfProductInstalledSuccessfully(PRODUCT_NAME_SQLDM_64) : InstallationHelper.checkIfProductInstalledSuccessfully(PRODUCT_NAME_SQLDM))
                    &&
                    SQLdmDetails.IsLatestVersionInstalled
                    &&
                    (properties.installSQLDMConsoleOption ? SQLdmDetails.IfConsoleIsInstalledOnLocal : true)
                    &&
                    (properties.installSQLDMServiceAndRepositoryOption ? SQLdmDetails.IfServicesAreInstalledOnLocal : true);

            // update dm success for retry attempts on failed upgrades
            properties.CheckDmSuccess = checkDmSuccess;
            properties.RetryCountSQLdmUpgrade++;

            }

            if(checkIdSuccess && checkDmSuccess)
                e.Result = true;

            if (properties.installDashboardOption)
            {
                if (checkIdSuccess)
                    error += MSG_DASHBOARD_SUCCESSFULL;
                else
                    error += MSG_DASHBOARD_UNSUCCESSFULL;
            }

            if (properties.installSQLDMServiceAndRepositoryOption || properties.installSQLDMConsoleOption)
            {
                if (checkDmSuccess)
                    error += MSG_SQLDM_SUCCESSFULL;
                else
                    error += MSG_SQLDM_UNSUCCESSFULL;
            }

            if (error != String.Empty)
            {
                LOG.ErrorFormat("RemoteProgress.backgroundWorkerLocal_DoWork()- Installation Failed... CheckIdSuccess: {0}, CheckDmSuccess: {1}, Error:{2}", checkIdSuccess.ToString(), checkDmSuccess.ToString(), error);
            }
            else
                LOG.InfoFormat("RemoteProgress.backgroundWorkerLocal_DoWork()- Installation is successfull.");            
        }

        private int GetProgressPercentage()
        {
            double total = 0, installedProduct = 0;

            total =
                (properties.installDashboardOption ? Properties.Settings.Default.MemoryRqdForDashboardInMB : 0)
                +
                (properties.installSQLDMConsoleOption ? Properties.Settings.Default.MemoryRqdForSQLdmConsoleInMB : 0)
                +
                (properties.installSQLDMServiceAndRepositoryOption ? Properties.Settings.Default.MemoryRqdForSQLdmServicesInMB : 0)
                  + // SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
                Properties.Settings.Default.MemoryRqdForSqlClrTypesInMB + Properties.Settings.Default.MemoryRqdForSMOInMB; 

            if (properties.installDashboardOption && installationTriedForDashboard)
                installedProduct += Properties.Settings.Default.MemoryRqdForDashboardInMB;

            if (properties.installSQLDMConsoleOption && installationTriedForSQLdm)
                installedProduct += Properties.Settings.Default.MemoryRqdForSQLdmConsoleInMB;

            if (properties.installSQLDMServiceAndRepositoryOption && installationTriedForSQLdm)
                installedProduct += Properties.Settings.Default.MemoryRqdForSQLdmServicesInMB;

            if (installationTriedForSqlClrTypes)    // SQLdm 10.2 (Anshul Aggarwal) - Query Montior Extended Events using XEReader API
                installedProduct += Properties.Settings.Default.MemoryRqdForSqlClrTypesInMB;

            if (installationTriedForSMO)
                installedProduct += Properties.Settings.Default.MemoryRqdForSMOInMB;

            if (installedProduct == 0)
                return 0;

            return (int)(installedProduct / total * 100);
        }

        private void backgroundWorkerLocal_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == PERC_VALUE_FOR_VALIDATING_PRODUCTS)
            {
                setProgressStatus("Validating Installed Products...");               
                return;
            }

            if (e.ProgressPercentage == 0)
            {
                progressBar.Value = 0;
                labelForPerc.Text = string.Empty;
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = e.ProgressPercentage;
                labelForPerc.Text = e.ProgressPercentage.ToString()+"%";
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            this.Cursor = Cursors.Default;
            if (e.Result != null && (bool)e.Result)
            {                
                InstallScreen nextScreen = new InstallScreen(this);                
                nextScreen.Show();
            }
            else
            {
                // Logging for additional information
                LOG.Info(string.Format("\nbackgroundWorker_RunWorkerCompleted on {0} SQLdmDetails.IsDmInstalledOnLocal {1} properties.CheckDmSuccess {2} properties.RetryCountSQLdmUpgrade {3} properties.MAXRETRYCOUNTSQLDMUPGRADE {4}",
                    DateTime.Now.ToString(),
                    properties.IsDmInstalledOnLocal,
                    properties.CheckDmSuccess,
                    properties.RetryCountSQLdmUpgrade,
                    properties.MAXRETRYCOUNTSQLDMUPGRADE));

                // To retry for failed upgrade scenarios of SQLdm
                if (properties.IsDmInstalledOnLocal && !properties.CheckDmSuccess && properties.RetryCountSQLdmUpgrade > 0 && properties.RetryCountSQLdmUpgrade <= properties.MAXRETRYCOUNTSQLDMUPGRADE)
                {
                    this.Cursor = Cursors.WaitCursor;
                    RemoteProgress nextScreen = new RemoteProgress(backScreen);
                    this.Hide();
                    nextScreen.Show();
                }
                else
                {
                    InstallationFailure nextScreen = new InstallationFailure(error);
                    nextScreen.Show();
                }
            }
        }
    }
}
