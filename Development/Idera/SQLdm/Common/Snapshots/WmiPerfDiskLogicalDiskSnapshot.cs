//------------------------------------------------------------------------------
// <copyright file="WmiPerfDiskLogicalDiskSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPerfDiskLogicalDisk information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPerfDiskLogicalDiskSnapshot : Snapshot
    {
        private DataTable wmiPerfDiskLogicalDisk = new DataTable("WmiPerfDiskLogicalDisk");

        public WmiPerfDiskLogicalDiskSnapshot(string serverName)
            : base(serverName)
        {
            wmiPerfDiskLogicalDisk.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPerfDiskLogicalDisk
        {
            get { return wmiPerfDiskLogicalDisk; }
            internal set { wmiPerfDiskLogicalDisk = value; }
        }

        private void Initialize()
        {
            WmiPerfDiskLogicalDisk.Columns.Add("Name", typeof(string));
            WmiPerfDiskLogicalDisk.Columns.Add("AvgDiskQueueLength", typeof(ulong));
            WmiPerfDiskLogicalDisk.Columns.Add("CurrentDiskQueueLength", typeof(uint));
            WmiPerfDiskLogicalDisk.Columns.Add("SplitIOPerSec", typeof(uint));
            WmiPerfDiskLogicalDisk.Columns.Add("PercentDiskTime", typeof(ulong));
            WmiPerfDiskLogicalDisk.Columns.Add("PercentDiskTime_Base", typeof(ulong));
            WmiPerfDiskLogicalDisk.Columns.Add("AvgDisksecPerTransfer", typeof(uint));
            WmiPerfDiskLogicalDisk.Columns.Add("AvgDisksecPerTransfer_Base", typeof(uint));
            WmiPerfDiskLogicalDisk.Columns.Add("DiskTransfersPerSec", typeof(uint));
            WmiPerfDiskLogicalDisk.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
        }
    }
}
