//------------------------------------------------------------------------------
// <copyright file="WmiPhysicalMemorySnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPhysicalMemory information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPhysicalMemorySnapshot : Snapshot
    {
        private DataTable wmiPhysicalMemory = new DataTable("WmiPhysicalMemory");

        public WmiPhysicalMemorySnapshot(string serverName)
            : base(serverName)
        {
            wmiPhysicalMemory.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPhysicalMemory
        {
            get { return wmiPhysicalMemory; }
            internal set { wmiPhysicalMemory = value; }
        }

        private void Initialize()
        {
            WmiPhysicalMemory.Columns.Add("Caption", typeof(string));
            WmiPhysicalMemory.Columns.Add("Capacity", typeof(ulong));
        }
    }
}
