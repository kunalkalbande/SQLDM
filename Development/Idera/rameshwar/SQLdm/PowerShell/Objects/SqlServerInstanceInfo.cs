//------------------------------------------------------------------------------
// <copyright file="SqlServerInstanceInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Data;

    // Note: not using IsClustered because SMO seems to return true for all non-local servers.

    public class SqlServerInstanceInfo
    {
        public readonly string Name;
        public readonly string Server;
        public readonly string Instance;
        public readonly string Version;
//      public readonly bool IsClustered;
//      public readonly bool IsLocal;

        internal SqlServerInstanceInfo(DataRow row)
        {
            Name = row["Name"] as string;
            Server = row["Server"] as string;
            Instance = row["Instance"] as string;
            Version = row["Version"] as string;
//          IsClustered = (bool)row["IsClustered"];
//          IsLocal = (bool) row["IsLocal"];
        }

        internal SqlServerInstanceInfo(string server, string instance, string version)
        {
            Server = server;
            Instance = instance;
            
            if (Instance.Equals("MSSQLSERVER"))
                Name = Server;
            else
                Name = Server + "\\" + Instance;

            Version = version;
     //       IsLocal = isLocal;
        }

    }
}
