//------------------------------------------------------------------------------
// <copyright file="WmiPerfDiskPhysicalDiskSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPerfDiskPhysicalDisk information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPerfDiskPhysicalDiskSnapshot : Snapshot
    {
        private DataTable wmiPerfDiskPhysicalDisk = new DataTable("WmiPerfDiskPhysicalDisk");

        public WmiPerfDiskPhysicalDiskSnapshot(string serverName)
            : base(serverName)
        {
            wmiPerfDiskPhysicalDisk.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPerfDiskPhysicalDisk
        {
            get { return wmiPerfDiskPhysicalDisk; }
            internal set { wmiPerfDiskPhysicalDisk = value; }
        }

        private void Initialize()
        {
            WmiPerfDiskPhysicalDisk.Columns.Add("Name", typeof(string));
            WmiPerfDiskPhysicalDisk.Columns.Add("DiskTransfersPerSec", typeof(uint));
            WmiPerfDiskPhysicalDisk.Columns.Add("Frequency_Sys100NS", typeof(ulong));
            WmiPerfDiskPhysicalDisk.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
        }
    }
}
