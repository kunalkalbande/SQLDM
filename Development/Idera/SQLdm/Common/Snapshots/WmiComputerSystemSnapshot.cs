//------------------------------------------------------------------------------
// <copyright file="WmiTCPSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents the WmiComputerSystem information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiComputerSystemSnapshot : Snapshot
    {
        private DataTable wmiComputerSystem = new DataTable("WmiComputerSystem");

        public WmiComputerSystemSnapshot(string serverName)
            : base(serverName)
        {
            wmiComputerSystem.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiComputerSystem
        {
            get { return wmiComputerSystem; }
            internal set { wmiComputerSystem = value; }
        }

        private void Initialize()
        {
            WmiComputerSystem.Columns.Add("DomainRole", typeof(uint));
        }
    }
}
