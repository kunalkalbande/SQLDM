//------------------------------------------------------------------------------
// <copyright file="ODBCHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Win32;

    public static class ODBCHelper
    {

        public static List<string> GetODBCDriverNames()
        {
            List<string> driverNames = new List<string>();

            RegistryKey odbcKey = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("ODBC");
            if (odbcKey != null)
            {
                RegistryKey odbcInstKey = odbcKey.OpenSubKey("ODBCINST.INI");   
                if (odbcInstKey != null)
                {
                    RegistryKey driversKey = odbcInstKey.OpenSubKey("ODBC Drivers");
                    if (driversKey != null)
                    {
                        driverNames.AddRange(driversKey.GetValueNames());
                    }
                }
            }
            return driverNames;
        }

        public static List<string> GetSystemDSNNames(out string fileDSNPath)
        {
            List<string> dsnNames = new List<string>();
            fileDSNPath = "";

            RegistryKey odbcKey = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("ODBC");
            if (odbcKey != null)
            {
                RegistryKey odbcInstKey = odbcKey.OpenSubKey("ODBC.INI");
                if (odbcInstKey != null)
                {
                    RegistryKey dsnKey = odbcInstKey.OpenSubKey("ODBC Data Sources");
                    if (dsnKey != null)
                    {
                        dsnNames.AddRange(dsnKey.GetValueNames());
                    }
                    RegistryKey fdsnKey = odbcInstKey.OpenSubKey("ODBC File DSN");
                    if (fdsnKey != null) 
                        fileDSNPath = fdsnKey.GetValue("DefaultDSNDir", "").ToString();
                }
            }
            return dsnNames;
        }

        public static List<string> GetFileDSNList(string path)
        {
            List<string> dsnNames = new List<string>();

            dsnNames.AddRange(Directory.GetFiles(path, "*.dsn"));

            return dsnNames;
        }

    }
}
