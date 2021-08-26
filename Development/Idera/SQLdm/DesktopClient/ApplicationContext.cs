using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.ApplicationBlocks.Data;
using Application = System.Windows.Application;
using ApplicationMessageBox = Idera.SQLdm.Common.UI.Dialogs.ApplicationMessageBox;
using SplashScreen = Idera.SQLdm.DesktopClient.Dialogs.SplashScreen;
using Timer = System.Windows.Forms.Timer;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient
{
    internal class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        private const int SplashDuration = 4000;
        private static readonly Logger Log = Logger.GetLogger("ApplicationContext");
        private static readonly Logger StartUpTimeLog = Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private SplashScreen splashScreen;
        private GettingStartedWizard gettingStartedWizard;
        private Timer splashScreenTimer;
        private BackgroundWorker validateConnectionWorker;
        private Exception connectionError;

        internal InitialView DefaultView { get; set; }
        public bool IsConnected { get; set; }

        
        /// <summary>
        /// General startup steps:
        ///     (1) Show splash screen
        ///     (2) If connection does not exist or SQL Server password must be specified
        ///         (2.1) Show repository connection dialog
        ///     (3) Validate connection
        ///         (3.1) If valid
        ///             (3.1.1) If no servers being monitored, show getting started wizard
        ///                 (3.1.1.1) On close wizard, show main form
        ///             (3.1.2) Else, show main form
        ///         (3.2) Else
        ///             (3.2.1) Show repository connection dialog
        ///                 (3.2.1.1) Goto to step 3
        /// </summary>
        internal ApplicationContext(System.Windows.Application wpfapp)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Initialize();


            IsConnected = true;
            splashScreen = new SplashScreen();
            // splashScreen.BackColor = System.Drawing.Color.FromArgb(236, 235, 234); // TODO:  Expedient.  Possibly change property settings where appropriate.
            
            MainForm = splashScreen;
            
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ||
                    (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity == false &&
                     Settings.Default.ActiveRepositoryConnection.SavePassword))
                {
                    splashScreen.SetStatus(SplashScreenStatus.ConnectingToRepository);
                    validateConnectionWorker.RunWorkerAsync();
                }
            }
            else
            {
                //SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27168--Closing splash screen and showing repository connection dialog
                //when default repository connection is null
                Program.CloseSplash();
                ShowRepositoryConnectionDialog();
            }

            splashScreenTimer.Enabled = true;
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationContext : {0}", stopWatch.ElapsedMilliseconds);
        }

        

        private void Initialize()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            SqlConnectionInfo.DefaultConnectionTimeout = Settings.Default.RepositoryConnectionTimeoutInSeconds;
            SqlHelper.CommandTimeout = Settings.Default.RepositoryCommandTimeoutInSeconds;
            
            splashScreenTimer = new Timer();
            splashScreenTimer.Tick += new EventHandler(splashScreenTimer_Tick);
            splashScreenTimer.Interval = SplashDuration;

            validateConnectionWorker = new BackgroundWorker();
            validateConnectionWorker.DoWork += new DoWorkEventHandler(validateConnectionWorker_DoWork);
            validateConnectionWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(validateConnectionWorker_RunWorkerCompleted);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationContext.Initialize: {0}", stopWatch.ElapsedMilliseconds);
        }

        private void splashScreenTimer_Tick(object sender, EventArgs e)
        {
            // SqlDm 10.2 -- Tushar --Improving Start up time for desktop client.
            // Current implementation of this method checks whether backgroundworker initiated in the constructor of
            // of application context is completed of not. If it is completed then close the splash screen, otherwise 
            // splash screen will be closed by the RunWorkerCompleted method of the background worker.
            // Now, the splash screen will be closed as soon as the timer of 4 seconds is elapsed and the main tread 
            // will continue execution.

            //splashScreenTimer.Enabled = false;

            //if (!validateConnectionWorker.IsBusy)
            //{
            if (splashScreen != null && !splashScreen.IsDisposed)
            {
                splashScreen.SetStatus(SplashScreenStatus.None);
                splashScreen.Close();
            }
            //}
        }

        private static void validateConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ValidateConnectionWorker";

            if (Settings.Default.ActiveRepositoryConnection == null)
            {
                throw new ArgumentException("The repository connection cannot be null.");
            }
            else
            {

                Settings.Default.ActiveRepositoryConnection.RefreshRepositoryInfo();

                if (Settings.Default.ActiveRepositoryConnection.RepositoryInfo.IsValidVersion)
                {
                    ApplicationModel.Default.Initialize();
                }
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by validateConnectionWorker_DoWork : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void validateConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            connectionError = e.Error;
            //SqlDM 10.2 (Tushar)--Calling RepositoryConnectionDialog if there is a error with saved connection.
            if (connectionError != null)
                ShowRepositoryConnectionDialog();
            if (!splashScreenTimer.Enabled)
            {
                if (splashScreen != null && !splashScreen.IsDisposed)
                {
                    splashScreen.SetStatus(SplashScreenStatus.None);
                    splashScreen.Close();
                    splashScreen = null;
                }
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("validateConnectionWorker_RunWorkerCompleted time taken : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void ShowRepositoryConnectionDialog()
        {
            RepositoryConnectionDialog repositoryConnectionDialog = new RepositoryConnectionDialog();
            repositoryConnectionDialog.StartPosition = FormStartPosition.CenterScreen;
            repositoryConnectionDialog.ShowInTaskbar = true;
            repositoryConnectionDialog.Closed += RepositoryConnectionDialogClosed;
            //SqlDM 10.2 - Tushar - using ShowDialog to freeze the background main window and on cancel click by user closing the whole application.
            //Start-SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27157
            //Showing the getting started wizard when the new repository connection provided by user have zero monitored servers. 
            DialogResult dialogResult = repositoryConnectionDialog.ShowDialog();
            switch (dialogResult)
            {
                case DialogResult.OK:
                    if (ApplicationModel.Default.UserToken != null && ApplicationModel.Default.UserToken.IsSQLdmAdministrator &&
                        Settings.Default.ActiveRepositoryConnection != null && Settings.Default.ActiveRepositoryConnection.RepositoryInfo != null &&
                        Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                    {
                        Dialogs.GettingStartedWizard gettingStartedWizard = new Dialogs.GettingStartedWizard();
                        gettingStartedWizard.TopMost = true;
                        gettingStartedWizard.ShowDialog();
                    }
                    break;
                case DialogResult.Cancel:
                    Application.Current.Shutdown();
                    break;
            }
            //End-SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27157
        }

        private void RepositoryConnectionDialogClosed(object sender, EventArgs e)
        {
            IsConnected = Settings.Default.ActiveRepositoryConnection != null;
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            Log.Debug("OnMainFormClosed called with sender of type ", sender.GetType());

            if (sender is SplashScreen)
            {
                if (connectionError != null)
                {
                    ApplicationMessageBox.ShowError(null,
                                                    "An error occurred while establishing a connection to the SQL Diagnostic Manager Repository. Please resolve the following error in order to continue.",
                                                    connectionError);

                    ShowRepositoryConnectionDialog();
                }
                else
                {
                    if (Settings.Default.ActiveRepositoryConnection == null ||
                        (Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity == false &&
                         Settings.Default.ActiveRepositoryConnection.SavePassword == false))
                    {
                        ShowRepositoryConnectionDialog();
                    }
                    else if (Settings.Default.ActiveRepositoryConnection.RepositoryInfo != null)
                    {
                        if (!Settings.Default.ActiveRepositoryConnection.RepositoryInfo.IsValidVersion)
                        {
                            ApplicationMessageBox.ShowError(null,
                                                            string.Format(Resources.InvalidRepositoryVersionMessageWithMetadata, Settings.Default.ActiveRepositoryConnection.RepositoryInfo.VersionString, Common.Constants.ValidRepositorySchemaVersion),
                                                            null);

                            ShowRepositoryConnectionDialog();
                        }
                        else
                        {
                            // Got a valid repository.  Check the license.
                            if (LicenseChecker.WillShowMessage())
                            {
                                // We need a dummy MainForm to own the messages that will be displayed
                                // by the LicenseChecker since the ApplicationMessageBox cannot be a
                                // MainForm because it is not a Form.
                                MainForm = new LicenseChecker();
                                MainForm.Show(); // See LicenseChecker.OnActivated()
                                return;
                            }
                            else
                            {
                                bool close = PassedLicenseCheck();
                                if (close)
                                {
                                    base.OnMainFormClosed(sender, e);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(null, Resources.InvalidRepositoryVersionMessage, null);
                        ShowRepositoryConnectionDialog();
                    }
                }
            }
            else if (sender is LicenseChecker)
            {
                LicenseChecker licenseChecker = (LicenseChecker)sender;
                if (licenseChecker.Passed)
                {
                    PassedLicenseCheck();
                }
                else
                {
                    ShowRepositoryConnectionDialog();
                }
            }
            else if (sender is RepositoryConnectionDialog)
            {
                RepositoryConnectionDialog repositoryConnectionDialog = sender as RepositoryConnectionDialog;

                if (repositoryConnectionDialog.DialogResult == DialogResult.Cancel)
                {
                    base.OnMainFormClosed(sender, e);
                }
                else if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator &&
                         Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                {
                    ShowGettingStartedWizard();
                }
                else
                {
                    DefaultView = InitialView.Default;
                }
            }
            else if (sender is GettingStartedWizard)
            {
                DefaultView = InitialView.Default;
                if (((GettingStartedWizard)sender).ShowPulseView)
                {
                    DefaultView = InitialView.Pulse;
                }
                gettingStartedWizard = null;
            }
            else if (sender is PulseGettingStartedDialog)
            {
                DefaultView = InitialView.Default;
                if (MainForm.DialogResult == DialogResult.OK)
                {
                    DefaultView = InitialView.Pulse;
                }
            }
            else
            {
                base.OnMainFormClosed(sender, e);
                return;
            }

            if (DefaultView != InitialView.None)
            {
                base.OnMainFormClosed(sender, e);
            }
        }

        private bool PassedLicenseCheck()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator &&
                Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
            {
                ShowGettingStartedWizard();
                return false;
            }
            else
            {
                DefaultView = InitialView.Default;
                return true;
            }
        }

        private void ShowGettingStartedWizard()
        {
            gettingStartedWizard = new GettingStartedWizard();
            gettingStartedWizard.StartPosition = FormStartPosition.CenterScreen;
            MainForm = gettingStartedWizard;
            MainForm.Show();
        }

    }

    public enum InitialView
    {
        None,
        Default,
        Pulse
    }

}
