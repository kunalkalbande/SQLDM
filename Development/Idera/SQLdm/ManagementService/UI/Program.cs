//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.UI
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Windows.Forms;
    using System.Text;
    using System.Threading;
    using Idera.SQLdm.Common.UI.Dialogs.Config;
    using BBS.TracerX;

    public class Program
    {
        #region members
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Main");

        // Calling InitLogging() as part of static initialization should guarantee
        // the logging is configured early enough.
        private static bool LogFileOpened = initLogging();
        #endregion

        static void Main(string[] args)
        {
            bool grantedInitialOwnership;

            using (Mutex singleInstanceMutex = new Mutex(true, "SQLdmManagementServiceConsole", out grantedInitialOwnership))
            {
                if (grantedInitialOwnership)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // Startup the main form.
                    Application.Run(new ManagementServiceConfigWizard());
                }
                else
                {
                    // Set focus on the running Import Wizard app.
                    NativeMethods.EnumWindows(new NativeMethods.EnumWinCallBack(EnumWindowCallBack), 0);
                }
            }
        }

        private static bool initLogging() {
            // Don't load the TracerX configuration.  Just use the defaults.
            //bool configRc = BBS.TracerX.XmlConfig.Configure();
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "SQLdmManagementServiceConsole.tx1";
            return Logger.FileLogging.Open();
        } 

        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            // Get the window title.
            IntPtr windowHandle = (IntPtr)hwnd;
            StringBuilder windowText = new StringBuilder(256);
            NativeMethods.GetWindowText((int)windowHandle, windowText, windowText.Capacity);

            // Restore if its import wizard window.
            if (windowText.ToString().StartsWith("IDERA SQLDM Management Service Configuration Wizard"))
            {
                NativeMethods.ShowWindow(windowHandle, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(windowHandle);
                return false;
            }

            return true;
        }
    }
}
