using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

using Microsoft.SqlServer.MessageBox;

using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.ImportWizard.Objects;
using Idera.SQLdm.ImportWizard.Properties;


namespace Idera.SQLdm.ImportWizard
{
    static class Program
    {
        #region members
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Main");

        // Calling InitLogging() as part of static initialization should guarantee
        // the logging is configured early enough.
        private static bool LogFileOpened = initLogging();
        #endregion

        #region Main
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool grantedInitialOwnership;

            using (Mutex singleInstanceMutex = new Mutex(true, "SQLdmImportWizard", out grantedInitialOwnership))
            {
                if (grantedInitialOwnership)
                {
                    // Set unhandled exception handlers.
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    // Initialize UI stuff.
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // Check if DM 4.x is installed.
                    string msg = string.Empty;
                    if (SQLdm4x.Is4xInstalled(out msg))
                    {
                        // Parse command line args.
                        string host = string.Empty, db = string.Empty, user = string.Empty, pwd = string.Empty;
                        if (parseCommandLineArgs(args, out host, out db, out user, out pwd, out msg))
                        {
                            // Startup the main form.
                            ApplicationContext context = new ApplicationContext(host,db,user,pwd);
                            Application.Run(context);
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(null, msg, null);
                        }
                    }
                    else
                    {
                        ApplicationMessageBox.ShowWarning(null, msg);
                    }
                }
                else
                {
                    // Set focus on the running Import Wizard app.
                    Helpers.NativeMethods.EnumWindows(new Helpers.NativeMethods.EnumWinCallBack(EnumWindowCallBack), 0);
                }
            }
        }
        #endregion

        #region methods
        private static bool initLogging()
        {
            // Configure logging for this assembly using the App.config file.
            // If this doesn't work, the default properties of the TracerX.Configuration object are
            // probably OK.
            bool configRc = Logger.Xml.Configure();
            Logger.FileLogging.Name = "SQLdmImportWizard.tx1";
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";

            // Do not log the command line args, because it can contain the password for SQL Auth.
            Logger.GetLogger("StandardData").FileTraceLevel = TraceLevel.Info;
            bool ret = Logger.FileLogging.Open(); 

            return ret;
        }

        private static bool parseCommandLineArgs(
                string[] args,
                out string hostOut,
                out string dbOut,
                out string userOut,
                out string pwdOut,
                out string msgOut
            )
        {
            const string PARAM_REPOSITORY_HOST = "RepositoryHost";
            const string PARAM_REPOSITORY_DB = "RepositoryDB";
            const string PARAM_REPOSITORY_USER = "RepositoryUser";
            const string PARAM_REPOSITORY_PASS = "RepositoryPassword";
            const string VALID_PARAMS = "-" + PARAM_REPOSITORY_HOST
                                        + ", -" + PARAM_REPOSITORY_DB
                                        + ", -" + PARAM_REPOSITORY_USER
                                        + ", and -" + PARAM_REPOSITORY_PASS + ".";

            // Init returns.
            hostOut = dbOut = userOut = pwdOut = msgOut = string.Empty;

            // If no command line params have been specified, that is okay
            // we will prompt the user for connection information.  Else
            // parse the command line params.
            bool isOk = true;
            int argCount = args.Length;
            if (argCount != 0)
            {
                string tempHost = string.Empty;
                string tempDb = string.Empty;
                string tempUser = string.Empty;
                string tempPwd = string.Empty;
                CaseInsensitiveComparer comparer = CaseInsensitiveComparer.DefaultInvariant;
                for (int i = 0; i < args.Length && isOk; i++)
                {
                    string arg = args[i];
                    if (!(arg.StartsWith("-") || arg.StartsWith("/")))
                    {
                        isOk = false;
                        continue;
                    }

                    arg = arg.Substring(1).Trim();

                    if (comparer.Compare(arg, PARAM_REPOSITORY_HOST) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            tempHost = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, PARAM_REPOSITORY_DB) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            tempDb = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, PARAM_REPOSITORY_USER) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            tempUser = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, PARAM_REPOSITORY_PASS) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            tempPwd = args[++i];
                        }
                        continue;
                    }
                    else
                    {
                        isOk = false;
                    }
                }
                if (!isOk) // Setup invalid args error message.
                {
                    Log.Debug("Invalid command line args have been specified.");
                    msgOut = Resources.InvalidCommandLineParamsError + " " + VALID_PARAMS;
                }

                // Get the repository connection information object.
                if (isOk)
                {
                    // Update host & database.
                    if (!string.IsNullOrEmpty(tempHost) && !string.IsNullOrEmpty(tempDb))
                    {
                        hostOut = tempHost;
                        dbOut = tempDb;

                        // Update user/password.
                        if (!string.IsNullOrEmpty(tempUser) && !string.IsNullOrEmpty(tempPwd))
                        {
                            userOut = tempUser;
                            pwdOut = tempPwd;
                        }
                        else
                        {
                            Log.Debug("User credentials have not been specified.");
                        }
                    }
                    else
                    {
                        Log.Debug("SQLdm Repository instance or database have not been specified.");
                    }
                }
            }
            else
            {
                Log.Debug("Command line parameters have not been specified");
            }

            return isOk;
        }

        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            // Get the window title.
            IntPtr windowHandle = (IntPtr)hwnd;
            StringBuilder windowText = new StringBuilder(256);
            Helpers.NativeMethods.GetWindowText((int)windowHandle, windowText, windowText.Capacity);

            // Restore if its import wizard window.
            if (windowText.ToString().StartsWith("Idera SQL diagnostic manager Import Wizard"))
            {
                Helpers.NativeMethods.ShowWindow(windowHandle, Helpers.NativeMethods.SW_RESTORE);
                Helpers.NativeMethods.SetForegroundWindow(windowHandle);
                return false;
            }

            return true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnhandledExceptionMessage(e.ExceptionObject);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowUnhandledExceptionMessage(e.Exception);
        }

        private static void ShowUnhandledExceptionMessage(object exceptionObject)
        {
            if (exceptionObject == null)
            {
                ApplicationMessageBox.ShowError(null, "Unhandled exception object is null.", null);
                Log.Fatal(Resources.ExceptionUnhandled);
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
        #endregion
    }
}