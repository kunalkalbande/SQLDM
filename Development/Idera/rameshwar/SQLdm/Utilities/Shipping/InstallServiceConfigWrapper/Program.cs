using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InstallServiceConfigWrapper
{
    class Program
    {
        enum ServiceType { CS, MS, Unknown }
        const int ServiceIndex = 0;
        const int ErrorRc = 3;
        const int OkRc = 0;
        const string MSConfigOption = "MS";
        const string CSConfigOption = "CS";
        const string MSConfigExe = "SQLdmManagementService.exe";
        const string CSConfigExe = "SQLdmCollectionService.exe";
        const string MSConfigInstallLog = "SQLdmManagementService.InstallLog";
        const string CSConfigInstallLog = "SQLdmCollectionService.InstallLog";

        static int Main(string[] args)
        {
            // If no arguments we have a problem bail out.
            if (args == null || args.Length == 0)
            {
                return ErrorRc;
            }

            // Figure out which service we are configuring.
            ServiceType svcType = parseServiceType(args[0]);
            if (svcType == ServiceType.Unknown)
            {
                return ErrorRc;
            }

            // Combine remaining args into a single argument string.
            string configArg = combineArgs(args);

            // Determine the exe to run based on service type.
            string exeName = svcType == ServiceType.MS ? MSConfigExe : CSConfigExe;
            string logName = svcType == ServiceType.MS ? MSConfigInstallLog : CSConfigInstallLog;

            // Configure the service.
            return runConfig(exeName, logName, configArg);
        }

        private static string combineArgs(
                string[] args
            )
        {
            string ret = string.Empty;
            for (int i = ServiceIndex + 1; i < args.Length; ++i)
            {
                // If its a flag switch copy as is.
                if (args[i].StartsWith("-") || args[i].StartsWith("/"))
                {
                    ret += args[i];
                }
                // Else put in double quotes.
                else
                {
                    ret += "\"" + args[i] + "\"";
                }

                // If not the last then put a space.
                if (i != args.Length - 1) { ret += " "; }
            }
            return ret;
        }

        private static ServiceType parseServiceType(
                string arg0
            )
        {
            ServiceType ret = ServiceType.Unknown;

            if (arg0.StartsWith("-") || arg0.StartsWith("/"))
            {
                string svcTypeStr = arg0.Substring(1).Trim();
                CaseInsensitiveComparer comparer = CaseInsensitiveComparer.DefaultInvariant;
                if (comparer.Compare(svcTypeStr, MSConfigOption) == 0)
                {
                    ret = ServiceType.MS;
                }
                else if (comparer.Compare(svcTypeStr, CSConfigOption) == 0)
                {
                    ret = ServiceType.CS;
                }
            }

            return ret;
        }

        private static int runConfig(
                string exeName,
                string logName,
                string args
            )
        {
            // Create the process start info object and update it.
            ProcessStartInfo psi = new ProcessStartInfo();
            string dir = Path.GetDirectoryName(Application.ExecutablePath);
            psi.WorkingDirectory = dir;
            psi.Arguments = args;
            psi.FileName = Path.Combine(dir, exeName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            // Start the process and wait ~5 minutes for it to be done.
            int rc = OkRc;
            try
            {
                // Start the process.
                Process p = Process.Start(psi);

                // Wait for about 10 minutes to be done.
                int cntr = 0;
                while (!p.HasExited && cntr < 1200)
                {
                    Thread.Sleep(500);
                    ++cntr;
                }

                // If process has exited, then return exit code and delete the install log file.
                if (p.HasExited)
                {
                    rc = p.ExitCode;
                    FileInfo logFile = new FileInfo(Path.Combine(dir, logName));
                    logFile.Delete();  // There is no need to check if it exists, the API can handle it.
                }
                else
                {
                    rc = ErrorRc;
                }
            }
            catch
            {
                rc = ErrorRc;
            }

            return rc;
        }
    }
}
