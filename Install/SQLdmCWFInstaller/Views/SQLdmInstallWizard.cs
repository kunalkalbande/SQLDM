using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using SQLdmCWFInstaller.Helpers;
using Idera.SQLdm.Common.Security.Encryption;
using PluginCommon;
using System.Threading;

namespace SQLdmCWFInstaller
{
    /// <summary>
    /// /// SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created windows application page to install/upgrade CWF and SQLdm both.
    /// </summary>
    public partial class SQLdmInstallWizard : Form
    {
        #region fields
        private static readonly Logger Log = Logger.GetLogger("SQLdmCWFInstaller_SQLdmInstallWizard");

        private WizardPage currentPage;
        private SQLdmInstallInfo installInfo;//= new SQLdmInstallInfo();
        private SQLdmInstallInfo localInstallInfo = new SQLdmInstallInfo { HostName = Environment.GetEnvironmentVariable("COMPUTERNAME") };//initialize the hostname as the local computer name
        private SQLdmInstallInfo remoteInstallInfo;//= new SQLdmInstallInfo();

        private bool proceedWithoutCWF = false;
        private bool isUpgrade = false;
        private bool isAnotherSQLdmRegistered = true;
        private bool isEverythingAlright = true;
        private bool isCWFInstalledLocally = false;
        bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
        private string finishScreenMessage = String.Empty;
        private Version prodVer = Assembly.GetExecutingAssembly().GetName().Version;
        private string productVersion = string.Empty;
        private string currentAssmeblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #endregion

        enum WizardPage
        {
            WelcomePage,
            RegisterPage,
            DisplayNamePage,
            FinishPage
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SQLdmInstallWizard()
        {
            InitializeComponent();
            productVersion = prodVer.Major + "." + prodVer.Minor + ".0.0";
            isUpgrade= CheckIfUpgrade();
            
            //Set the welcome page as the current page 
            currentPage = WizardPage.WelcomePage;
            //select the current page
            installWizard.Tabs[(int)currentPage].Selected = true;
            this.lnkLabel2.Click += new EventHandler(lnkLabel2_Click);//dm 10.0 Vineet --  Updated text and added link for idera.com
            UpdatePageView();
        }

        /// <summary>
        /// dm 10.0 Vineet --  Updated text and added link for idera.com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lnkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.CWFIntroVideoLink);
        }

        #region Events
        /// <summary>
        /// CWF Register Radio Button Click events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radioButtonCWF_Click(object sender, EventArgs e)
        {
            var targetRadioButton = (RadioButton)sender;
            if (targetRadioButton != null)
            {
                if (radioButtonLocal.Checked)
                {
                    ConfigureRegisterPageLables();
                    PersistUserProvidedValue(ref remoteInstallInfo); //SQLdm 9.0 (Ankit Srivastava)- added ref operator
                    FillValueTextBoxes(localInstallInfo);

                }
                else
                {
                    PersistUserProvidedValue(ref localInstallInfo); //SQLdm 9.0 (Ankit Srivastava)- added ref operator
                    FillValueTextBoxes(remoteInstallInfo);

                    this.titleText.Rtf = @"{\rtf1\ansi Register with a remote IDERA Dashboard}";
                    
                }
                EnableNextButtonAccordingly();
            }
        }

        /// <summary>
        /// input text boxes click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void textBoxCWF_Click(object sender, EventArgs e)
        {
            EnableNextButtonAccordingly();

        }

        /// <summary>
        /// previous button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void previousBtn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (currentPage == WizardPage.FinishPage && !isAnotherSQLdmRegistered)
                currentPage--;

            currentPage--;
            installWizard.Tabs[(int)currentPage].Selected = true;
            UpdatePageView();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// next button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void nextBtn_Click(object sender, EventArgs e)
        {
            string currentPageName = currentPage.ToString();
            string nextBtnName = nextBtn.Text; //SQLdm 9.0 (Ankit Srivastava) -- Capturing the nextBtn text for loggin before its changed
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ProcessPage();

                //increment to the next page.
                currentPage++;


                if (currentPage > WizardPage.FinishPage)
                    this.Close();
                else
                {
                    installWizard.Tabs[(int)currentPage].Selected = true;
                    UpdatePageView();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
				//SQLdm 9.0 (Ankit Srivastava) -- Updated the Logging from Info to Error inc ase of exception
                Log.Error(String.Format("This error occured on {1} button click for {0} page:", currentPageName, nextBtnName), ex);
            }
            Log.Info(String.Format("{1} button click ran successfully for {0} page.", currentPageName, nextBtnName));
        }

        /// <summary>
        /// cancel button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method checks if upgrade needed for SQLdm installation
        /// </summary>
        /// <returns></returns>
        private bool CheckIfUpgrade()
        {
            try
            {
                string upgradeCode = is64BitOperatingSystem ? Constants.UpgradeCodeX64 : Constants.UpgradeCodeX86;
                string productCode = is64BitOperatingSystem ? Constants.ProductCodeX64 : Constants.ProductCodeX86;

                string[] productCodes = SQLdmHelper.GetProductCodes(new Guid(upgradeCode), is64BitOperatingSystem);
                if (productCodes !=null && productCodes.Length >0)//if upgrade code matches or product already installed
                {
                    // Convert the code to the format found in the registry 
                    var productCodeSearchString = SQLdmHelper.Reverse(productCode, SQLdmHelper.GuidRegistryFormatPattern);
                    if (!productCodes.Any(code => code.IndexOf(productCodeSearchString, StringComparison.OrdinalIgnoreCase) >= 0))//check if product code differs from the exisiting ones
                        return true;
                    //TODO else 

                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in CheckIfUpgrade: ", ex);
                
            }

            return false;
        }

        /// <summary>
        ///  This method process the user input and proceeds accordingly 
        /// </summary>
        private void ProcessPage()
        {
            try
            {
                switch (currentPage)
                {
                    case WizardPage.WelcomePage:
                        //SQLdm 9.0 (Ankit Srivastava) Fixed Rally defect DE44279 removed ConfigureRegisterPageLables method call from here
                        PersistUserProvidedValue(ref remoteInstallInfo); //SQLdm 9.0 (Ankit Srivastava)- added ref operator
                        FillValueTextBoxes(localInstallInfo);
                        isCWFInstalledLocally = IsCWFInstalledLocally();
                        break;

                    case WizardPage.RegisterPage:
                        ProcessRegisterPage();
                        break;

                    case WizardPage.DisplayNamePage:
                        ProcessDisplayNamePage();
                        break;

                    case WizardPage.FinishPage:
                        if (isEverythingAlright)
                            InstallSQLdm();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in ProcessPage: ", ex);
                
            }
        }

        //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created new method for processing diff scenarios of display name page
        private void ProcessDisplayNamePage()
        {
            //SQLdm 9.1 (vineet kumar) -- removed validation of display name to allow upgradation
            string candidateDisplayName = string.Empty;
            labelDisplayNameMessage.Text = String.Empty;
            installInfo = installInfo ?? new SQLdmInstallInfo();
            candidateDisplayName = textBoxRegisterName.Text.Trim().Replace(" ", "");
            if (!IsDisplayNameValid(candidateDisplayName))
            {
                labelDisplayNameMessage.Text = "Special characters are not \n allowed in display names";
                labelDisplayNameMessage.Font = new System.Drawing.Font("Arial",7, System.Drawing.FontStyle.Bold);
                ConfigureDisplayNamePageLables();
                currentPage--;
            }
            else 
            {
                installInfo.InstanceDisplayName = candidateDisplayName;
            }
        }

        /// <summary>
        /// SQLdm 9.1: Gaurav Karwal - created for validating the display name.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns>true - if valid, else false</returns>
        private bool IsDisplayNameValid(string displayName) 
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(displayName, "[^a-z A-Z 0-9]");
        }
        //[END] SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created new method for processing diff scenarios of display name page

        /// <summary>
        /// This method processes the Register Page details provided by the user
        /// </summary>
        private void ProcessRegisterPage()
        {
            if (radioButtonLocal.Checked)
            {
                PersistUserProvidedValue(ref localInstallInfo); //SQLdm 9.0 (Ankit Srivastava)- added ref operator
                installInfo = localInstallInfo;
                Log.Info("isCWFInstalledLocally=" + isCWFInstalledLocally.ToString());
                if (!isCWFInstalledLocally)
                {
                    EnableDisableInstaller(false);
                    InstallCWF();
                    EnableDisableInstaller(true);
                }

            }
            else
            {
                PersistUserProvidedValue(ref remoteInstallInfo); //SQLdm 9.0 (Ankit Srivastava)- added ref operator
                installInfo = remoteInstallInfo;
            }
            string versionString;//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard
            bool isCWFInstalled = IsCWFInstalled(out versionString);
            if (!isCWFInstalled)
                    Thread.Sleep(2000); // SQLdm 9.0 (Ankit Srivastava) Waiting after the local CWF installation 
            //Check again
            
            if (isCWFInstalled || IsCWFInstalled(out versionString))//check the installation status and proceed accordingly
            {
                //start:SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard
                System.Version installedCWFVersion = new Version(versionString);
                if (installedCWFVersion.Major < Constants.CWFVersionWithoutRevision.Major || (installedCWFVersion.Major == Constants.CWFVersionWithoutRevision.Major && installedCWFVersion.Minor < Constants.CWFVersionWithoutRevision.Minor) || (installedCWFVersion.Major == Constants.CWFVersionWithoutRevision.Major && installedCWFVersion.Minor == Constants.CWFVersionWithoutRevision.Minor && installedCWFVersion.Build < Constants.CWFVersionWithoutRevision.Build))
                {
                    if (radioButtonLocal.Checked)
                    {
                        PersistUserProvidedValue(ref localInstallInfo); 
                        installInfo = localInstallInfo;
                        Log.Info("isCWFInstalledLocally=" + isCWFInstalledLocally.ToString());
                       
                            EnableDisableInstaller(false);
                            InstallCWF();
                            EnableDisableInstaller(true);
                    }
                    else
                    {
                        MessageBox.Show(Constants.RemoteCWFOutdatedMessage);
                        proceedWithoutCWF = false;
                        isCWFInstalledLocally = IsCWFInstalledLocally();//fixing DE44362
                        ConfigureRegisterPageLables();
                        currentPage--; 
                        return;
                    }
                }
                //end:SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard
                proceedWithoutCWF = false;
                isAnotherSQLdmRegistered = true;//SQLdm 9.1(Vineet Kumar) -- Setting it true to always ask instance name.
                if (!isAnotherSQLdmRegistered)
                    currentPage++;
            }
            else // if not installed properly 
            {
                if (MessageBox.Show(this, Constants.CWFDetailsConfirmation, Constants.CWFConfirmationHeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    proceedWithoutCWF = true;
                else
                {
                    proceedWithoutCWF = false;
                    isCWFInstalledLocally = IsCWFInstalledLocally();//fixing DE44362
                    ConfigureRegisterPageLables();
                    currentPage--;
                }
            }
        }

        /// <summary>
        /// This method updates each page's view accordingly
        /// </summary>
        private void UpdatePageView()
        {
            switch (currentPage)
            {
                case WizardPage.WelcomePage:
                    {
                        this.previousBtn.Enabled = false;
                        this.nextBtn.Enabled = true;
                        this.cancelBtn.Enabled = true;
                        ConfigureWelcomePageLables();
                        break;
                    }
                case WizardPage.RegisterPage:
                    {
                        this.previousBtn.Enabled = true;
                        EnableNextButtonAccordingly();
                        this.nextBtn.Text = "Next";
                        isCWFInstalledLocally = IsCWFInstalledLocally();//fixing DE44362
                        ConfigureRegisterPageLables();
                        break;
                    }
                case WizardPage.DisplayNamePage:
                    {
                        this.previousBtn.Enabled = true;
                        this.nextBtn.Enabled = true;
                        this.nextBtn.Text = "Next";
                        ConfigureDisplayNamePageLables();
                        break;
                    }
                case WizardPage.FinishPage:
                    {

                        this.previousBtn.Enabled = true;
                        this.nextBtn.Enabled = true;
                        this.nextBtn.Text = "Finish";
                        this.cancelBtn.Enabled = true;
                        ConfigureFinishPageLables();
                        break;
                    }
                default:
                    {
                        this.previousBtn.Enabled = false;
                        this.nextBtn.Enabled = false;
                        this.cancelBtn.Enabled = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// This method configures Welcome Page Lables
        /// </summary>
        private void ConfigureWelcomePageLables()
        {
            lnkLabel1.Visible = lnkLabel2.Visible = lnkLabel3.Visible = !isUpgrade;
            if (isUpgrade)
            {
                this.titleText.Rtf = @"{\rtf1\ansi Welcome to the SQL Diagnostic Manager Upgrade wizard}";
                this.descriptionText.Rtf = @"{\rtf1\ansi The SQL Diagnostic Manager (SQLDM) Upgrade wizard helps you upgrade to the latest available version. This version includes, our common technology framework, the IDERA Dashboard. The IDERA Dashboard provides a platform of services that allow for an integrated user experience across multiple IDERA products. For help with the upgrade process, see the SQLDM help manual.}";
            }
            else
            {
                this.titleText.Rtf = @"{\rtf1\ansi Welcome to the SQL Diagnostic Manager Setup wizard}";
                this.descriptionText.Rtf = @"{\rtf1\ansi The SQLDM Setup wizard helps you install SQLDM and our common technology framework, the IDERA Dashboard on your machine. The IDERA Dashboard provides a platform of services that allow for an integrated user experience across multiple IDERA products. For help with the installation process, see the SQLDM help manual.}";
                this.descriptionText.Rtf = @"{\rtf1\ansi Welcome to the SQL Diagnostic Manager setup, which will install or upgrade SQL Diagnostic Manager and IDERA Dashboard, a required component for SQL Diagnostic Manager 9.0 and later. The IDERA Dashboard provides a common platform of services and integrated user experience across multiple IDERA products.
\line \line
The installation will include 2 processes
\line 1. Setting up the IDERA Dashboard. This includes a Tomcat web server. (Required)
\line 2. Setting up SQL Diagnostic Manager and registering with IDERA Dashboard. (Required)";
          }
        }

        /// <summary>
        /// This method configures Register Page Lables
        /// </summary>
        private void ConfigureRegisterPageLables()
        {
            //this.descriptionText.Rtf = @"{\rtf1\ansi The Idera Dashboard hosts the SQLdm web console and allows you to have an integrated user experience across multiple Idera products. You can choose to register SQLdm with a local copy of the CWF or through a remote installation of the framework. Please provide \b Host Name\b0 , \b Port\b0 , \b Service Account\b0 , and \b Password\b0  in the fields below for installation(if required) and Registration.}";
            this.descriptionText.Rtf = @"{\rtf1\ansi The IDERA Dashboard hosts the SQLDM web console and allows you to have an integrated user experience across multiple IDERA products. You can choose to install this framework locally or register with previously installed copy on another server.}";
            if (isCWFInstalledLocally)
            {
                this.titleText.Rtf = @"{\rtf1\ansi Select an option to register for the IDERA Dashboard}";
               //this.descriptionText.Rtf = @"{\rtf1\ansi The Idera Dashboard is already installed in this machine. The Idera Dashboard hosts the SQLdm web console and allows you to have an integrated user experience across multiple Idera products. You can choose to register SQLdm with a local copy of the CWF or through a remote installation of the framework.}";

                this.radioButtonLocal.Text = "Register with the local IDERA Dashboard";
            }
            else
            {
                this.titleText.Rtf = @"{\rtf1\ansi Select an option to run the IDERA Dashboard}";
                //this.descriptionText.Rtf = @"{\rtf1\ansi The Idera Dashboard hosts the SQLdm web console and allows you to have an integrated user experience across multiple Idera products. You can choose to install this framework locally or register with a previously installed copy on another server.}";

                this.radioButtonLocal.Text = "Install the IDERA Dashboard on your local machine";
            }

        }

        /// <summary>
        /// This method configures DisplayName Page Lables
        /// </summary>
        private void ConfigureDisplayNamePageLables()
        {
            this.titleText.Rtf = @"{\rtf1\ansi Specify a display name for this registration}";
            this.descriptionText.Rtf = @"{\rtf1\ansi Provide a unique display name for this instance of SQLDM on the IDERA Dashboard. We recommend using display names based on location or function such as " + "\"SQLDM East\" or \"SQLDM Production\".}";
        }

        /// <summary>
        /// This method configures Finish Page Lables
        /// </summary>
        private void ConfigureFinishPageLables()
        {
            this.titleText.Rtf = @"{\rtf1\ansi Complete the SQL Diagnostic Manager Setup wizard}";
            this.descriptionText.Rtf = isEverythingAlright ?
                                                @"{\rtf1\ansi You have successfully configured SQLDM and the IDERA Dashboard. Click \b Finish\b0  to begin the installation process and exit the wizard or click \b Previous\b0  to review your settings.}"
                                                  : @"{\rtf1\ansi " + finishScreenMessage + "}";
        }

        /// <summary>
        /// This method fills the CWF detail text boxes according the current Install info object
        /// </summary>
        /// <param name="currInstallInfo"></param>
        private void FillValueTextBoxes(SQLdmInstallInfo currInstallInfo)
        {
            if (currInstallInfo == null) return;
            textBoxHostName.Text = currInstallInfo.HostName;
            textBoxPort.Text = currInstallInfo.PortName;
            textBoxServiceAccount.Text = currInstallInfo.ServiceAccount;
            textBoxPassword.Text = currInstallInfo.ServiceAccountPass;
        }

        /// <summary>
        /// This methods persists the data into the current install info object with the details entered by user
        /// </summary>
        /// <param name="currInstallInfo"></param>
        private void PersistUserProvidedValue(ref SQLdmInstallInfo currInstallInfo)//SQLdm 9.0 (Ankit Srivastava)- added ref operator
        {
            currInstallInfo = currInstallInfo ?? new SQLdmInstallInfo();
            currInstallInfo.HostName = textBoxHostName.Text.Trim();
            currInstallInfo.PortName = textBoxPort.Text.Trim();
            currInstallInfo.ServiceAccount = textBoxServiceAccount.Text.Trim();
            currInstallInfo.ServiceAccountPass = textBoxPassword.Text.Trim();
        }

        /// <summary>
        /// This method enables and disables the installer controls according to the input
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableDisableInstaller(bool enabled)
        {
            this.previousBtn.Enabled = enabled;
            this.nextBtn.Enabled = enabled;
            this.cancelBtn.Enabled = enabled;
            radioButtonLocal.Enabled = enabled;
            radioButtonRemote.Enabled = enabled;

        }

        /// <summary>
        /// This method enables the next button accordingly
        /// </summary>
        private void EnableNextButtonAccordingly()
        {
            nextBtn.Enabled = !(String.IsNullOrWhiteSpace(textBoxPort.Text) || String.IsNullOrWhiteSpace(textBoxHostName.Text)
                                    || String.IsNullOrWhiteSpace(textBoxServiceAccount.Text) || String.IsNullOrWhiteSpace(textBoxPassword.Text));
            groupBoxRemoteValues.Visible = true;
            textBoxHostName.Enabled = radioButtonRemote.Checked;
            textBoxPort.Enabled = true;
        }

        /// <summary>
        /// Creats the CWF helper object according to the user provided install info
        /// </summary>
        /// <returns></returns>
        private CWFHelper CreateHelper(bool isLocalStatusCheck = false)
        {
            try
            {
                installInfo = installInfo ?? new SQLdmInstallInfo();

                if (installInfo.ServiceAccount != null && installInfo.ServiceAccount.Contains(@".\"))
                    installInfo.ServiceAccount = installInfo.ServiceAccount.Replace(@".", (installInfo.HostName ?? Environment.MachineName));
				//SQLdm 9.0 (Ankit Srivastava) - Calling localhost in case of local machine installation
                string coreServiceURL = "http://" + (radioButtonLocal.Checked ? "localhost":(installInfo.HostName ?? "localhost")) + ":" + (installInfo.PortName ?? "9292");
                if (isLocalStatusCheck)//fixing DE44362
                    return new CWFHelper(string.Empty, string.Empty, coreServiceURL);
                return new CWFHelper(installInfo.ServiceAccount, installInfo.ServiceAccountPass, coreServiceURL);
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in CreateHelper: ", ex);
            }
            return null;
        }

        /// <summary>
        /// gets the current assembly's version
        /// </summary>
        /// <returns></returns>
        private string GetProductAssemblyVersion()
        {

            string fullName;
            try
            {
                fullName = Assembly.GetExecutingAssembly().FullName;
                string[] assemblyDetails = fullName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string versionDetail = assemblyDetails.Where(detail => detail.Contains("Version")).First();
                string[] versionArray = versionDetail.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (versionArray.Length > 1)
                    return versionArray[1];
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in GetProductAssemblyVersion: ", ex);
                
            }
            finally
            {
                fullName = null;
            }
            return null;

        }

        private bool IsCWFInstalledLocally()
        {
            CWFHelper helper = CreateHelper(true);//fixing DE44362 adding true parameter
            
            return helper.GetLocalInstallionStatus();
        }

        /// <summary>
        /// This method checks with CWF API (for given hostName and portName and Credentials) if CWF installed or not
        /// </summary>
        /// <returns></returns>
        private bool IsCWFInstalled(out string version)//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard added version as out parameter
        {
            try
            {
                CWFHelper helper = CreateHelper();
                string strJson = helper.GetStatus();

                if (!String.IsNullOrWhiteSpace(strJson))
                {
                    GetServiceStatusResponse status = new JavaScriptSerializer().Deserialize<GetServiceStatusResponse>(strJson);
                    if (status != null && (bool)status.CanRun)
                    {
                        version = status.Version;//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in CheckIfCWFInstalled: ", ex);
                
            }
            version = string.Empty;
            return false;
        }

        /// <summary>
        /// This method checks with CWF API (for given hostName and portName and Credentials) if any other SQLdm installed
        /// </summary>
        /// <returns></returns>
        private bool IsAnotherSQLdmRegistered()
        {
            try
            {
                if (installInfo != null)
                {
                    CWFHelper helper = CreateHelper();
                    //productVersion = productVersion ?? GetProductAssemblyVersion();
                    var strJson = helper.GetProducts(Constants.ProductName, productVersion);

                    if (!String.IsNullOrWhiteSpace(strJson))
                    {
                        List<Product> products = new JavaScriptSerializer().Deserialize<List<Product>>(strJson);
                        if (products != null && products.Count > 0)
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in IsAnotherSQLdmRegistered: ", ex);
                
            }
            return false;

        }

        /// <summary>
        ///This method checks with CWF API (for given hostName and portName and Credentials) if any other SQLdm installed with the same instance name
        /// </summary>
        /// <returns></returns>
        private bool IsDisplayNameDuplicate()
        {
            try
            {
                if (installInfo != null)
                {
                    CWFHelper helper = CreateHelper();
                    //productVersion = productVersion ?? GetProductAssemblyVersion();
                    return helper.CheckIfDuplicate(Constants.ProductName, productVersion, textBoxRegisterName.Text.Trim());

                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in IsDisplayNameDuplicate: ", ex);
                
            }
            return false;
        }

        /// <summary>
        /// This method installs the Idera Web Framework locally
        /// </summary>
        /// <returns></returns>
        private bool InstallCWF()
        {
            Process cwfInstProcess = new Process();
            try
            {
                cwfInstProcess = Process.Start(currentAssmeblyFolder+@"\IderaDashboard.msi");
                cwfInstProcess.WaitForExit();   //wait until it installs
                //TODO: handle the situation where CWF fails to install
                return true;
            }
            catch (InvalidOperationException ex) // if user cancels the installation
            {
                Log.Error("Exception thrown in InstallCWF: ", ex);
                return false; 
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in InstallCWF: ", ex);

            }
            finally
            {
                cwfInstProcess = null;
            }
            return false;

        }

        /// <summary>
        /// This method installs the SQL Dignostics Manager
        /// </summary>
        private void InstallSQLdm()
        {
            Process installProcess = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            try
            {
                processInfo.Arguments = String.Format(Constants.InstallationCommandArgs, installInfo.PortName.Trim(), installInfo.HostName.Trim(), installInfo.ServiceAccount, EncryptionHelper.QuickEncrypt(installInfo.ServiceAccountPass));
                processInfo.FileName = currentAssmeblyFolder + @"\SQLDiagnosticManager" + (is64BitOperatingSystem ? "-x64" : String.Empty) + ".exe";

                //[START] SQLdm 9.0 (Ankit Srivastava)- added instancename argument and logging of SQLdm installation args and filename
                if (!String.IsNullOrWhiteSpace(installInfo.InstanceDisplayName))
                    processInfo.Arguments += " CWF_INSTANCE_NAME=\""+installInfo.InstanceDisplayName+"\"";
                
				Log.Info("SQLdm install Arguments: "+ processInfo.Arguments);
                Log.Info("SQLdm install FileName: " + processInfo.FileName);
                //[END] SQLdm 9.0 (Ankit Srivastava)- added instancename argument and logging of SQLdm installation args and filename
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in InstallSQLdm: ", ex);
            }
            finally
            {
                installProcess = null;
                processInfo = null; 
            }

        }

        private bool DoSomething()
        {
            return true;
        }
        #endregion
    }
}
