using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using BBS.TracerX;
using Application = System.Windows.Forms.Application;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using System.Web;
using Idera.SQLdm.Common.Launch;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient
{
    static class Program
    {
        private static int MAX_INSTANCES = Settings.Default.MultipleDesktopClients;

        // Command line arguments
        private const string CliSilentExit = "silent";
        private const string CliInstanceName = "instance";
        private const string CliDatabaseName = "database";
        private const string CliUserName = "user";
        private const string CliPassword = "password";

        // Calling InitLogging() as part of static initialization should guarantee
        // the logging is configured early enough.
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Program");
        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        private static bool LogFileOpened = InitLogging();
        public static ApplicationContext ApplicationContext;
        public static Window MainWindow;
        private static Idera.SQLdm.DesktopClient.Dialogs.SplashScreen splashScreen;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Log.InfoFormat("Current FileTraceLevel = {0}", Log.FileTraceLevel);
            using (Log.InfoCall())
            {
                Stopwatch stopWatchMain = new Stopwatch();
                stopWatchMain.Start();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                bool grantedInitialOwnership;
                Mutex reservation = AcquireMutex();
                stopwatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by Acquire mutex :{0}",stopwatch.ElapsedMilliseconds);
                //System.Windows.MessageBox.Show(args[0]);
                string launchParameters = args.Length > 0 ? args[0] : string.Empty, monitoredInstanceNameToLaunch = string.Empty ;
                int monitoredInstanceIdToLaunch = 0;
                long alertIdToLaunchWith = 0;
                const string ALERT_MONIKER = "alert",INSTANCE_MONIKER = "instance", ANALYSIS_MONIKER = "analysis";
                LaunchParameters launchParams = null;

                
                
                if (reservation == null)
                {
                    string message = string.Format("There are {0} instances of the SQL Diagnostic Manager Desktop Client already running.  Please switch to one of the running instances or wait and try again at a later time.", MAX_INSTANCES);
                    System.Windows.MessageBox.Show(
                        message,
                        "Hang On Now!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);
                    return;
                }

                using (reservation)
                {
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

                    AppDomain.CurrentDomain.UnhandledException +=
                        new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    if (!ProcessCommandLineArguments(args))
                    {
                        return;
                    }
                    else 
                    {
                        //[START] SQLdm 9.1 (Gaurav Karwal): for launching dm in the right context
                        //checking if the args are coming from Web UI
                        if (System.Text.RegularExpressions.Regex.IsMatch(launchParameters, "idera://.*", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) //the url comes always in small case, even you send capitals while launching
                        {
                            //alert url in format Idera://alert?instancename={name}&alertid={alertId}&instanceid={instanceid}
                            //instance url in format Idera://instance?instanceid={instanceid}

                            Uri parsedLaunchParam = new Uri(launchParameters);
                            System.Collections.Specialized.NameValueCollection queryParamsAtLaunch = HttpUtility.ParseQueryString(parsedLaunchParam.Query);

                            monitoredInstanceIdToLaunch = queryParamsAtLaunch.Count > 0 && queryParamsAtLaunch["instanceid"] != null ? Convert.ToInt32(queryParamsAtLaunch["instanceid"]) : 0;
                            monitoredInstanceNameToLaunch = queryParamsAtLaunch.Count > 0 && queryParamsAtLaunch["instancename"] != null ? queryParamsAtLaunch["instancename"] : string.Empty;
                            alertIdToLaunchWith = queryParamsAtLaunch.Count > 0 && queryParamsAtLaunch["alertid"] != null ? Convert.ToInt64(queryParamsAtLaunch["alertid"]) : 0;

                            if (string.Compare(parsedLaunchParam.Host, ALERT_MONIKER, true) == 0) launchParams = new AlertLaunchParameters(monitoredInstanceNameToLaunch, monitoredInstanceIdToLaunch, alertIdToLaunchWith);
                            else if (string.Compare(parsedLaunchParam.Host, INSTANCE_MONIKER, true) == 0) launchParams = new InstanceLaunchParameters(monitoredInstanceIdToLaunch);
                            //SQLdm10.2 (srishti purohit )defect fix
                            //SQLDM-27637('Prescriptive Analysis Summary' component click not working)    
                            else if (string.Compare(parsedLaunchParam.Host, ANALYSIS_MONIKER, true) == 0) launchParams = new AnalysisLaunchParameters(monitoredInstanceIdToLaunch);
                        }
                        //[END] SQLdm 9.1 (Gaurav Karwal): for launching dm in the right context
                    }

                    if (Settings.Default.UpgradeSettings)
                    {
                        // upgrade infragistic settings store in the user config
                        using (System.ComponentModel.Component component = new System.ComponentModel.Component())
                        {
                            string[] productVersion = Application.ProductVersion.Split('.');

                            // Skip upgrade of "NavigationPaneControl.explorerBar" for versions earlier than 5.5 
                            // because the Administration has been added in 5.5 and needs to show up.
                            // NOTE: This logic may need to be modified for future versions.
                            int majorVersion = Convert.ToInt32(productVersion[0]);
                            int minorVersion = Convert.ToInt32(productVersion[1]);
                            if (majorVersion > 5 || (majorVersion == 5 && minorVersion >= 5))
                            {
                                Log.Info("Skipping upgrade of navigation pane settings for version 5.5 and higher.");
                            }
                            else
                            {
                                try
                                {
                                    new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar.
                                        UltraExplorerBarApplicationSettings(
                                        component,
                                        "NavigationPaneControl.explorerBar")
                                        .Upgrade();
                                }
                                catch (Exception e)
                                {
                                    Log.Error("Failed to upgrade navigation pane user settings: ", e);
                                }
                            }

                            try
                            {
                                new Infragistics.Win.UltraWinTree.UltraTree.UltraTreeApplicationSettings(
                                    component,
                                    "SessionsBlockingView.blockingTree")
                                    .Upgrade();
                            }
                            catch (Exception e)
                            {
                                Log.Error("Failed to upgrade blocking tree user settings: ", e);
                            }
                        }
                        try
                        {
                            // upgrade idera settings in the user config
                            Settings.Default.Upgrade();
                            Settings.Default.UpgradeSettings = false;
                            Settings.Default.Save();
                        }
                        catch (Exception e)
                        {
                            Log.Error("Failed to upgrade prior version settings: ", e);
                        }
                    }

                    using (Log.InfoCall("CheckForPulse"))
                    {
                        if (args.Length == 1 && Newsfeed.Shared.Helpers.LinkHelper.IsPulseUrl(args[0]))
                        {
                            Log.InfoFormat("Newsfeed command request sent: {0}", args[0]);
                            Newsfeed.Plugins.UI.PulseController.SendClipboardMessage(args[0]);
                        }
                    }

                    stopwatch.Reset();
                    stopwatch.Start();
					//SqlDM 10.2 (Tushar)--Starting Splash screen.
                    Thread splashThread = new Thread(new ThreadStart(StartSplash));
                    splashThread.Start();
                    // WPF Application
                    var application = new System.Windows.Application();

                    // dummy WPF window to use until mainform created
                    MainWindow = new Window();

                    stopwatch.Stop();
                    StartUpTimeLog.DebugFormat("Create application and set dummy window: {0}", stopwatch.Elapsed);

                    var initialview = InitialView.None;

                    stopwatch.Restart();
                    SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
                    using (ApplicationContext = new ApplicationContext(application))
                    {
						//SqlDM 10.2 (Tushar)--Commented this since splash scrren is now running in seperate thread.
                        //Application.Run(ApplicationContext);
                        initialview = InitialView.Default;//ApplicationContext.DefaultView;
                    }

                    stopwatch.Stop();
                    Log.DebugFormat("ApplicationContext complete: {0}", stopwatch.Elapsed);

                    if (!ApplicationContext.IsConnected)
                    {
                        return;
                    }

                    Log.Debug("ApplicationContext thread started. Creating MainWindow instance.");
                    stopwatch.Restart();

                    var stopWatch1 = new Stopwatch();
                    stopWatch1.Start();
                    // load up the toolbar item style dictionary
                    var resourceUri = new Uri("Style/ToolBar.Style.xaml", UriKind.Relative);
                    var resources = System.Windows.Application.LoadComponent(resourceUri) as ResourceDictionary;
                    if (resources != null)
                        application.Resources.MergedDictionaries.Add(resources);
                    stopWatch1.Stop();
                    StartUpTimeLog.DebugFormat("Time taken for loading Toolbar.Style : {0}",stopWatch1.ElapsedMilliseconds);
                    stopWatch1.Reset();
                    stopWatch1.Start();
                    // load up the menu style dictionary
                    resourceUri = new Uri("Style/Menu.Style.xaml", UriKind.Relative);
                    resources = System.Windows.Application.LoadComponent(resourceUri) as ResourceDictionary;
                    if (resources != null)
                        application.Resources.MergedDictionaries.Add(resources);
                    stopWatch1.Stop();
                    StartUpTimeLog.DebugFormat("Time taken for loading Menu.Style : {0}",stopWatch1.ElapsedMilliseconds);
                    stopWatch1.Reset();
                    stopWatch1.Start();
                    //load up the svg icons
                    resourceUri = new Uri("Style/SVGIcons.xaml", UriKind.Relative);
                    resources = System.Windows.Application.LoadComponent(resourceUri) as ResourceDictionary;
                    if (resources != null)
                        application.Resources.MergedDictionaries.Add(resources);
                    stopWatch1.Stop();
                    StartUpTimeLog.DebugFormat("Time taken for loading SVGIcons : {0}", stopWatch1.ElapsedMilliseconds);

                    stopWatch1.Reset();
                    stopWatch1.Start();
                    //load up the svg icons
                    resourceUri = new Uri("Style/Light.xaml", UriKind.Relative);
                    resources = System.Windows.Application.LoadComponent(resourceUri) as ResourceDictionary;
                    if (resources != null)
                        application.Resources.MergedDictionaries.Add(resources);
                    stopWatch1.Stop();
                    StartUpTimeLog.DebugFormat("Time taken for loading Light : {0}", stopWatch1.ElapsedMilliseconds);

                    ChangeSplashStatus(Dialogs.SplashScreenStatus.LoadingUI);//SqlDm 10.2 (Tushar)--Changing status of splash screen.
                    // create the main window while the startup stuff if running
                    var main = launchParams!=null? new MainWindow(launchParams): new MainWindow();

                    stopwatch.Stop();
                    Log.DebugFormat("MainWindow created: {0}", stopwatch.Elapsed);

                    // quit if user cancelled out of the startup stuff)
                    if (initialview == InitialView.None)
                        return;

                    // tap into the wpf unhandled exception handler
                    application.DispatcherUnhandledException += Program.Application_DispatcherUnhandledException;

                    // init application controller
                    ApplicationController.Default.HookEvents();

                    // force the active repository connection events to fire

                    // SqlDM 10.2  - Tushar
                    // No need to call this at all. There are three methods inside this: 
                    // 1.OnActiveRepositoryConnectionChanging
                    // 2.ApplicationModel.Default.Initialize();
                    // 3.OnActiveRepositoryConnectionChanged
                    // ApplicationModel.Default.Initialize() is already called after user changes repository connection, so need to call it again.
                    // OnActiveRepositoryConnectionChanging and OnActiveRepositoryConnectionChanged are the event handlers which reinitializes the
                    // forms, But when repository is changed during splash screen, forms are not even loaded so there is no point of updating and them.
                    //Settings.Default.RefreshRepositoryConnection();

                    if (initialview == InitialView.Pulse)
                        ApplicationController.Default.ShowPulseView();
                    CloseSplash();//SqlDM 10.2 (Tushar)-- Closing the splash screen
                    //Start-SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27157
                    //If repository connection is established and number of monitored sql servers is zero in that repository, then showing the GettingStartedWizard
                    //before showing the main UI.
                    if (ApplicationModel.Default.UserToken!=null && ApplicationModel.Default.UserToken.IsSQLdmAdministrator && 
                        Settings.Default.ActiveRepositoryConnection!=null && Settings.Default.ActiveRepositoryConnection.RepositoryInfo!=null &&
                        Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                    {
                        Dialogs.GettingStartedWizard gettingStartedWizard = new Dialogs.GettingStartedWizard();
                        gettingStartedWizard.TopMost = true;
                        //Start-Sql Dm 10.2 (Tushar)--Fix for defect SQLDM-27246--Adding logs for GettingStartedWizard.
                        StartUpTimeLog.DebugFormat("Getting started wizard shown..");
                        stopwatch.Reset();
                        stopwatch.Start();
                        gettingStartedWizard.ShowDialog();
                        stopwatch.Stop();
                        StartUpTimeLog.DebugFormat("Getting started wizard closed.\nTime taken by GettingStartedWizard : {0}",stopwatch.ElapsedMilliseconds);
                        //End-Sql Dm 10.2 (Tushar)--Fix for defect SQLDM-27246--Adding logs for GettingStartedWizard.
                    }
                    //End-SqlDm 10.2 (Tushar)--Fix for defect SQLDM-27157
                    stopWatchMain.Stop();
                    StartUpTimeLog.DebugFormat("Time taken by complete UI creation : {0}",stopWatchMain.ElapsedMilliseconds);
                    // show the main window
					application.Run(MainWindow = main);

                    // SQLdm 10.1.2 (Varun Chopra): SQLDM-21118 Using return in the main causes an application to terminate only after all foreground threads have terminated.
                    return;
                }
            }
        }

        //SqlDM 10.2 (Tushar)--Added this method to start the splash screen
        private static void StartSplash()
        {
            splashScreen = new Idera.SQLdm.DesktopClient.Dialogs.SplashScreen();
            splashScreen.SetStatus(Dialogs.SplashScreenStatus.ConnectingToRepository);
            Application.Run(splashScreen);
        }

        //SqlDM 10.2 (Tushar)--Added this method to close the splash screen.
        //SqlDM 10.2 (Tushar)--Making close splash screen method public becasue we might need to close splash screen from other threads also.
        public static void CloseSplash()
        {
            if (splashScreen == null)
                return;

            // Shut down the splash screen
            splashScreen.Invoke(new EventHandler(splashScreen.StopSplash));
            splashScreen.Dispose();
            splashScreen = null;
        }

        //SqlDM 10.2 (Tushar)--Added this method to change the status of splash screen.
        //SqlDM 10.2 (Tushar)--Making change splash screen method public because we might need to close splash screen from other threads also.
        public static void ChangeSplashStatus(Dialogs.SplashScreenStatus status)
        {
            if (splashScreen == null)
                return;
            splashScreen.Invoke(new MethodInvoker( delegate { splashScreen.ChangeStatus(status); }));
        }

        private static Mutex AcquireMutex()
        {
            for (int i = 0; i < MAX_INSTANCES; i++)
            {
                bool grantedInitialOwnership;
                var mutex = new Mutex(true, String.Format("{0}_{1}", Application.ProductName, i), out grantedInitialOwnership);
                if (grantedInitialOwnership) return mutex;
                mutex.Dispose();
            }

            return null;
        }

        /// <summary>
        /// Processes command line arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A boolean value indicating if the application should continue to run after the command line arguments are processed.</returns>
        private static bool ProcessCommandLineArguments(string[] args)
        {
            using (Log.InfoCall("ProcessCommandLineArguments"))
            {
                if (args != null && args.Length > 0)
                {
                    //sQLdM 10.2 (Tushar)--Adding logs for logging arguments passed.
                    StartUpTimeLog.InfoFormat("Arguments passed are : {0}", string.Join(",", args));
                    Log.Info("Processing command line arguments...");
                    bool launchDesktopClient = true;
                    string instanceName = String.Empty;
                    string databaseName = String.Empty;
                    string userName = String.Empty;
                    string password = String.Empty;

                    for (int i = 0; i < args.Length; i++)
                    {
                        string arg = args[i];

                        if (!(arg.StartsWith("-") || arg.StartsWith("/")))
                        {
                            continue;
                        }

                        arg = arg.Substring(1).Trim();

                        if (string.Compare(arg, CliSilentExit, true) == 0)
                        {
                            launchDesktopClient = false;
                            continue;
                        }
                        else if (string.Compare(arg, CliInstanceName, true) == 0)
                        {
                            if (i + 1 < args.Length && args[i + 1].Length > 0)
                            {
                                instanceName = args[++i];
                            }

                            Log.Debug(string.Format("Instance Name = {0}", instanceName));
                            continue;
                        }
                        else if (string.Compare(arg, CliDatabaseName, true) == 0)
                        {
                            if (i + 1 < args.Length && args[i + 1].Length > 0)
                            {
                                databaseName = args[++i];
                            }

                            Log.Debug(string.Format("Database Name = {0}", databaseName));
                            continue;
                        }
                        else if (string.Compare(arg, CliUserName, true) == 0)
                        {
                            if (i + 1 < args.Length && args[i + 1].Length > 0)
                            {
                                userName = args[++i];
                            }

                            Log.Debug(string.Format("User Name = {0}", userName));
                            continue;
                        }
                        else if (string.Compare(arg, CliPassword, true) == 0)
                        {
                            if (i + 1 < args.Length && args[i + 1].Length > 0)
                            {
                                password = args[++i];
                            }

                            Log.Debug("A password was specified.");
                            continue;
                        }
                        else
                        {
                            Log.Debug(string.Format("Command line argument '{0}' is invalid.", arg));
                        }
                    }

                    if (instanceName != String.Empty && databaseName != String.Empty)
                    {
                        RepositoryConnection connection =
                            Settings.Default.FindRepositoryConnection(instanceName, databaseName);

                        if (connection == null)
                        {
                            connection = new RepositoryConnection();
                            connection.ConnectionInfo.InstanceName = instanceName;
                            connection.ConnectionInfo.DatabaseName = databaseName;
                        }

                        if (userName == String.Empty)
                        {
                            connection.ConnectionInfo.UseIntegratedSecurity = true;
                        }
                        else
                        {
                            connection.ConnectionInfo.UseIntegratedSecurity = false;
                            connection.ConnectionInfo.UserName = userName;
                            connection.ConnectionInfo.Password = password;
                            connection.SavePassword = true;
                        }

                        try
                        {
                            Settings.Default.SetActiveRepositoryConnection(connection, true);
                        }
                        catch (Exception e)
                        {
                            Log.Error("An error occurred while saving the command line connection information.", e);
                        }
                    }
                    else
                    {
                        Log.Debug("A valid instance and database must be specified in order to connect.");
                    }

                    Log.Debug(string.Format("Silent Exit = {0}", !launchDesktopClient));

                    

                    return launchDesktopClient;
                }
                else
                {
                    // If there are no command line arguments, just launch the desktop client.
                    Log.Info("No command line arguments were specified.");
                    return true;
                }
            }
        }

        private static bool InitLogging()
        {
            // Configure logging for this assembly using the App.config file.
            // This includes setting the output directory.
            // If this doesn't work, the default properties of the 
            // TracerX.Configuration object are probably OK.
            Logger.Xml.Configure();

            // The name of the log file is set in code here (the directory 
            // was loaded from the .config file).
            Logger.FileLogging.Name = "SQLdmDesktopClient.tx1";

            // Threads should be named, but an exception occurs if 
            // you try to change the name.
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";

            // Now open the log file using the configuration 
            // settings that were previously loaded or set.
            bool isLogOpened = Logger.FileLogging.Open();
            PulseController.InitializeLogging(Logger.FileLogging.Directory);
            return isLogOpened;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnhandledExceptionMessage(e.ExceptionObject);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowUnhandledExceptionMessage(e.Exception);
        }

        internal static void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ShowUnhandledExceptionMessage(e.Exception);
            e.Handled = true;
        }

        private static void ShowUnhandledExceptionMessage(object exceptionObject)
        {
            if (exceptionObject == null)
            {
                ApplicationMessageBox.ShowError(null, "Unhandled exception object is null.", null);
                Log.Fatal(Resources.ExceptionUnhandled);
            }
            else if (exceptionObject is Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException)
            {
                ApplicationMessageBox.ShowError(null,
                                                "An error occurred while attempting to contact the Management Service.",
                                                exceptionObject as Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException);
            }
            else if (exceptionObject is Exception)
            {
                ApplicationMessageBox.ShowError(null,
                                                Resources.ExceptionUnhandled,
                                                exceptionObject as Exception);
                Log.Fatal(Resources.ExceptionUnhandled, exceptionObject as Exception);
            }
            else
            {
                ApplicationException exception = new ApplicationException(exceptionObject.ToString());
                ApplicationMessageBox.ShowError(null, Resources.ExceptionUnhandled, exception);
                Log.Fatal(Resources.ExceptionUnhandled, exception);
            }
        }

        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            using (Log.InfoCall())
            {
                IntPtr windowHandle = (IntPtr)hwnd;

                StringBuilder windowText = new StringBuilder(256);
                Helpers.NativeMethods.GetWindowText((int)windowHandle, windowText, windowText.Capacity);
                Log.Info("windowText = ", windowText);

                if (windowText.ToString().StartsWith(Application.ProductName))
                {
                    Helpers.NativeMethods.ShowWindow(windowHandle, Helpers.NativeMethods.SW_RESTORE);
                    Helpers.NativeMethods.SetForegroundWindow(windowHandle);
                    Log.Debug("Returning false.");
                    return false;
                }

                Log.Debug("Returning true.");
                return true;
            }
        }
    }
}
