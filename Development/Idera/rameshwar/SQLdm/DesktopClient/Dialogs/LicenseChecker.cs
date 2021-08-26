using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using BBS.TracerX;
using Microsoft.Win32;

namespace Idera.SQLdm.DesktopClient.Dialogs {

    /// <summary>
    /// This class was explicitly created to support the ApplicationContext class.
    /// </summary>
    public partial class LicenseChecker : Form {
        private bool _passed;
        private bool _activated;

        private static readonly TimeSpan _warningPeriod = TimeSpan.FromDays(10);
        private static readonly Logger Log = Logger.GetLogger("LicenseChecker");
        
        public LicenseChecker() {
            InitializeComponent();

            Opacity = 0.0;
            Text = Application.ProductName;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = Resources.AppIcon;
            ShowIcon = true;
            ShowInTaskbar = true;
            AdaptFontSize();
        }

        public bool Passed {
            get { return _passed; }
        }

        public static bool WillShowMessage() {
            return WillShowMessage(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
        }

        /// <summary>
        /// Returns true if CheckLicense would display a message.
        /// The repository to check is specified by connectionInfo.
        /// </summary>
        public static bool WillShowMessage(SqlConnectionInfo connectionInfo) {
                LicenseSummary licenseInfo = RepositoryHelper.GetLicenseKeys(connectionInfo);
            if (licenseInfo.Status == LicenseStatus.OK) {
                if (licenseInfo.Expiration - DateTime.Now < _warningPeriod) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return true;
            }
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);

            if (!_activated) {
                _activated = true;
                _passed = CheckLicense(this, true);
                this.Close();
            }
        }

        public static bool CheckLicense(IWin32Window owner, bool maybeWarn) {
            return CheckLicense(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, owner, maybeWarn);
        }

        // This checks the license on the repository specified by the connectionInfo parameter.
        // This returns true if the license is OK or the user user "fixes" the license
        // by providing a new key or by removing servers while in this method.
        // This returns false if the license is in violation and the user does not fix it.
        // This displays a warning if the license is OK but within 10 days of expiration.
        // This static method doesn't need to be in this class, but it's got to go somewhere.
        public static bool CheckLicense(SqlConnectionInfo connectionInfo, IWin32Window owner, bool maybeWarn, LicenseSummary licenseSummary=null) {
            using (Log.InfoCall()) {
                Log.Debug("maybeWarn = ", maybeWarn);
                bool goodLicense = false;
                ApplicationMessageBox msgBox;
                //sqldm-30136 start
                LicenseSummary licenseInfo=null;
                // This filters out the bad keys and adds up the good ones.
                if (licenseSummary == null)
                    licenseInfo = RepositoryHelper.GetLicenseKeys(connectionInfo);
                else
                    licenseInfo = licenseSummary;
                //sqldm-30136 end

                switch (licenseInfo.Status) {
                    case LicenseStatus.Expired:
                        // Message about expired license.
                        msgBox = new ApplicationMessageBox();
                        msgBox.Text = string.Format("The license for repository {0} has expired.  You must enter a valid license key to continue.", licenseInfo.Repository);
                        msgBox.SetCustomButtons("Buy Now","Manage Licenses", "Cancel");
                        msgBox.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                        msgBox.Show(owner);

                        switch (msgBox.CustomDialogResult)
                        {
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button1:
                                // Launch the product purchase url
                                ApplicationController.Default.LaunchProductPurchaseUrl();
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2:
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                        }
                        break;
                    case LicenseStatus.CountExceeded:
                        // Message about too many monitored servers.
                        msgBox = new ApplicationMessageBox();
                        msgBox.Text = string.Format("Repository {0} is monitoring {1} SQL Server instances.  It is only licensed to monitor {2}.  You must apply a new license or remove some monitored servers to continue.", licenseInfo.Repository, licenseInfo.MonitoredServers, licenseInfo.LicensedServers);
                        msgBox.SetCustomButtons("Buy Now","Manage Licenses", "Manage Servers", "Cancel");
                        msgBox.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                        msgBox.Show(owner);

                        switch (msgBox.CustomDialogResult)
                        {
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button1:
                                // Launch the product purchase url
                                ApplicationController.Default.LaunchProductPurchaseUrl();
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2:
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button3:
                                // Display the manage servers dialog, then check the license again.
                                ManageServersDialog manageServersDialog = new ManageServersDialog();
                                manageServersDialog.ShowDialog(owner);
                                goodLicense = CheckLicense(connectionInfo, owner, true);
                                break;

                        }
                        break;
                    case LicenseStatus.NoValidKeys:
                        // Message about no keys.
                        msgBox = new ApplicationMessageBox();
                        msgBox.Text = string.Format("No valid license key exists for repository {0}. A valid license key must be entered to continue.  If this is a new install, verify that the SQLDM Management Service is running and that the SQL Server credentials specified in the SQLdm Management Service Configuration Wizard can communicate with and write to the SQLdm Repository so a trial license can be generated.", licenseInfo.Repository);
                        msgBox.SetCustomButtons("Buy Now","Manage Licenses", "Cancel");
                        msgBox.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                        msgBox.Show(owner);
                        switch (msgBox.CustomDialogResult)
                        {
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button1:
                                // Launch the product purchase url
                                ApplicationController.Default.LaunchProductPurchaseUrl();
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                            case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2:
                                // Display the license dialog, then check the license again to confirm
                                // that the user fixed the problem.
                                LicenseDialog.Display(owner, connectionInfo);
                                goodLicense = CheckLicense(connectionInfo, owner, false);
                                break;
                        }
                        break;
                    case LicenseStatus.OK:
                        goodLicense = true;

                        // See if we're within 10 days of expiring.
                        if (maybeWarn) 
                        {
                            TimeSpan remaining = licenseInfo.Expiration - DateTime.Now;
                            Log.Debug("Remaining time = ", remaining);
                            if (remaining < TimeSpan.FromDays(10)) 
                            {
                                // Warning about impending expiration.
                                msgBox = new ApplicationMessageBox();
                                switch (remaining.Days) 
                                {
                                    case 0:
                                        msgBox.Text = string.Format("The license for repository '{0}' expires today!", licenseInfo.Repository);
                                        break;
                                    case 1:
                                        msgBox.Text = string.Format("The license for repository '{0}' expires tomorrow!", licenseInfo.Repository);
                                        break;
                                    default:
                                        msgBox.Text = string.Format("The license for repository '{0}' expires in {1} days.", licenseInfo.Repository, remaining.Days);
                                        break;
                                }
                                msgBox.SetCustomButtons("Buy Now","Manage Licenses", "Continue");
                                msgBox.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Warning;
                                msgBox.Show(owner);

                                switch (msgBox.CustomDialogResult)
                                {
                                    case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button1:
                                        ApplicationController.Default.LaunchProductPurchaseUrl();
                                        break;
                                    case Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2:
                                        LicenseDialog.Display(owner, connectionInfo);
                                        goodLicense = CheckLicense(connectionInfo, owner, false);
                                        break;
                                    
                                }
     
                            }
                        }

                        if (!licenseInfo.IsTrial)
                        {
                            //We need to mark the desktop client machine as well as the management service machine
                            RegistryKey rk = null;
                            RegistryKey rks = null;

                            rk = Registry.LocalMachine;
                            rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                            rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                            if (rks != null)
                                rks.Close();
                            rks = null;

                            if (rk != null)
                                rk.Close();
                            rk = null;
                        }
                        break;
                    default:
                        throw new ApplicationException("Unexpected license state: " + licenseInfo.Status);
                }

                Log.Debug("Returning ", goodLicense);
                return goodLicense;
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}