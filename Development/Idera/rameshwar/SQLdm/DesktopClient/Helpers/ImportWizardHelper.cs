//------------------------------------------------------------------------------
// <copyright file="ImportWizardHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public static class ImportWizardHelper
    {
        #region constants

        // Version checking.
        private const string DM_ROOT_REGISTRY_KEY = @"Software\Idera\DiagnosticManager";
        private const string DM_SERVERS_REGISTRY_KEY = DM_ROOT_REGISTRY_KEY + @"\Servers";
        private const string APPLICATION_PATH_REGISTRY_VALUE = "Application Path";
        private const string DM_EXE_NAME_PATH = @"\DiagnosticManager.exe";
        private const string DM_VER_PREFIX = "4.";

        // Import wizard.
        const string SQLDM_IMPORT_WIZARD = "SQLdmImportWizard.exe";

        // Cmd line args.
        const string PARAM_REPOSITORY_HOST = "RepositoryHost";
        const string PARAM_REPOSITORY_DB = "RepositoryDB";
        const string PARAM_REPOSITORY_USER = "RepositoryUser";
        const string PARAM_REPOSITORY_PASS = "RepositoryPassword";


        #endregion

        #region members

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(Program));

        #endregion

        #region methods

        public static bool IsDM4xInstalled()
        {
            bool isInstalled = true;
            string rootPath = string.Empty;
            string version = string.Empty;

            // Open the registry root of SQLdm 4.x, and get the SQLdm root path.
            using (RegistryKey rkSQLdm = Registry.LocalMachine.OpenSubKey(DM_ROOT_REGISTRY_KEY))
            {
                // Check if SQLdm key was opened.
                if (rkSQLdm == null)
                {
                    Log.Info("Unable to open SQLDM 4.x registry root, a previous version of SQLDM is not installed.");
                    isInstalled = false;
                }

                // Read the ApplicationPath value, this is the root path of SQLdm.
                if (isInstalled)
                {
                    try
                    {
                        // Make sure that the application path registry value type is a string.
                        if (rkSQLdm.GetValueKind(APPLICATION_PATH_REGISTRY_VALUE) != RegistryValueKind.String)
                        {
                            Log.Error("Application Path data type is not a string, SQLDM is not installed.");
                            isInstalled = false;
                        }

                        // Get the application path value and make sure its not null/empty and 
                        // root folder exists.
                        if (isInstalled)
                        {
                            rootPath = rkSQLdm.GetValue(APPLICATION_PATH_REGISTRY_VALUE).ToString();
                            if (!string.IsNullOrEmpty(rootPath))
                            {
                                if (!Directory.Exists(rootPath))
                                {
                                    Log.Error("SQLDM root folder does not exist (" + rootPath + "), SQLDM is not installed.");
                                    isInstalled = false;
                                }
                            }
                            else
                            {
                                Log.Error("Application Path registry value is null/empty, SQLDM is not installed.");
                                isInstalled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception was encountered when reading the Application Path value.", ex);
                        isInstalled = false;
                    }
                }
            }

            // Get the SQLdm version from the file version info of the exe, and
            // check if its DM 4.x.
            if (isInstalled)
            {
                // Check if file exists.
                string dmExePath = rootPath + DM_EXE_NAME_PATH;
                if (!File.Exists(dmExePath))
                {
                    Log.Error("SQLDM binary DiagnosticManager.exe does not exist (" + dmExePath + "), SQLDM is not installed.");
                    isInstalled = false;
                }

                // Get the product version from file version info.
                if (isInstalled)
                {
                    version = getProductVersionFromFileVersionInfo(dmExePath);
                    if (string.IsNullOrEmpty(version))
                    {
                        Log.Error("Version information was not retrieved (" + dmExePath + "), SQLDM is not installed.");
                        isInstalled = false;
                    }
                }

                // Check if DM 4.x is installed.
                if (isInstalled)
                {
                    if (!version.StartsWith(DM_VER_PREFIX))
                    {
                        isInstalled = false;
                    }
                }
            }

            return isInstalled;
        }

        public static void StartImportWizard(SqlConnectionInfo connectionInfo)
        {
            // Validate input.
            if (connectionInfo == null)
            {
                Log.Error("Connection info not specified for starting the Import Wizard");
                return;
            }

            // Start the process.
            string exe = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + SQLDM_IMPORT_WIZARD;
            string args = cmdLineArgs(connectionInfo);
            Process.Start(exe, args);
        }

        private static unsafe string getProductVersionFromFileVersionInfo(string filePath)
        {
            string version = string.Empty;
            try
            {
                // Figure out how much version info there is:
                int handle = 0;
                int size = NativeMethods.GetFileVersionInfoSize(filePath, out handle);

                // Allocate buffer for reading version information.
                if (size != 0)
                {
                    byte[] buffer = new byte[size];
                    if (buffer != null)
                    {
                        // Read file version information.
                        if (NativeMethods.GetFileVersionInfo(filePath, handle, size, buffer))
                        {
                            // Get the locale info from the version info, the subBlock memory is deallocated
                            // when the buffer block memory goes out of scope.
                            short* subBlock = null;
                            uint len = 0;
                            if (NativeMethods.VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out len))
                            {
                                // Get the ProductVersion value
                                string spv = @"\StringFileInfo\" + subBlock[0].ToString("X4") + subBlock[1].ToString("X4") + @"\ProductVersion";
                                if (!NativeMethods.VerQueryValue(buffer, spv, out version, out len))
                                {
                                    Log.Error("Failed to read version information string");
                                    version = string.Empty;
                                }
                            }
                            else
                            {
                                Log.Error("Failed to read locale info");
                            }
                        }
                        else
                        {
                            Log.Error("Failed to read the file version info into the allocated buffer");
                        }
                    }
                    else
                    {
                        Log.Error("Read buffer allocation failed for size " + size);
                    }
                }
                else
                {
                    Log.Error("Version information block size is 0");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception raised when retrieving version information ", ex);
                version = string.Empty;
            }

            return version;
        }

        private static string cmdLineArgs(SqlConnectionInfo connectionInfo)
        {
            Debug.Assert(connectionInfo != null);

            string cla = string.Empty;
            cla += "-" + PARAM_REPOSITORY_HOST + " \"" + connectionInfo.InstanceName + "\"";
            cla += " -" + PARAM_REPOSITORY_DB + " \"" + connectionInfo.DatabaseName + "\"";
            if (!connectionInfo.UseIntegratedSecurity)
            {
                cla += " -" + PARAM_REPOSITORY_USER + " \"" + connectionInfo.UserName + "\"";
                cla += " -" + PARAM_REPOSITORY_PASS + " \"" + connectionInfo.Password + "\"";
            }
            return cla;
        }

        #endregion
    }
}
