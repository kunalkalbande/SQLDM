using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBS.TracerX;
using System.Threading;
using System.IO;
using System.Security.Principal;

namespace Installer_form_application
{
    static class Program
    {
        private const string CONTAINING_FOLDER_FOR_32BIT_INSTALLER = "x86";
        private static void InitLogging()
        {
            bool configRc = Logger.Xml.Configure();
            Logger.FileLogging.Name = "SQLdmMasterInstaller.tx1";
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Main";

            // Do not log the command line args, because it can contain the password for SQL Auth.
            Logger.GetLogger("StandardData").FileTraceLevel = BBS.TracerX.TraceLevel.Info;
            Logger.FileLogging.Open();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string masterInstallerDirName = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).Name.ToString();
            InitLogging();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!IsRunAsAdmin())
                MessageBox.Show("The SQL Diagnostic Manager Installation must be run as an administrator. Please either Right Click on the Installer and select 'Run as Administrator' or run the installer from an account that has adequate administrative privileges", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (Environment.Is64BitOperatingSystem && masterInstallerDirName == CONTAINING_FOLDER_FOR_32BIT_INSTALLER)
                    MessageBox.Show("You are trying to run 32bit application on a 64bit machine, please run the correct version", "Incompitable Version", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    Application.Run(new Introduction());
            }
        }
        private static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principle = new WindowsPrincipal(id);

            return principle.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
