//------------------------------------------------------------------------------
// <copyright file="WmiPerfOSMemorySnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPerfOSMemory information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPerfOSMemorySnapshot : Snapshot
    {
        private DataTable wmiPerfOSMemory = new DataTable("WmiPerfOSMemory");

        public WmiPerfOSMemorySnapshot(string serverName)
            : base(serverName)
        {
            wmiPerfOSMemory.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPerfOSMemory
        {
            get { return wmiPerfOSMemory; }
            internal set { wmiPerfOSMemory = value; }
        }

        private void Initialize()
        {
            WmiPerfOSMemory.Columns.Add("PagesPersec", typeof(uint));
            WmiPerfOSMemory.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
        }
    }
}
