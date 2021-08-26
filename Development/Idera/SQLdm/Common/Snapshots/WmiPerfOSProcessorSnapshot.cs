//------------------------------------------------------------------------------
// <copyright file="WmiPerfOSProcessorSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPerfOSProcessor information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPerfOSProcessorSnapshot : Snapshot
    {
        private DataTable wmiPerfOSProcessor = new DataTable("WmiPerfOSProcessor");

        public WmiPerfOSProcessorSnapshot(string serverName)
            : base(serverName)
        {
            wmiPerfOSProcessor.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPerfOSProcessor
        {
            get { return wmiPerfOSProcessor; }
            internal set { wmiPerfOSProcessor = value; }
        }

        private void Initialize()
        {
            WmiPerfOSProcessor.Columns.Add("Name", typeof(string));
            WmiPerfOSProcessor.Columns.Add("InterruptsPerSec", typeof(uint));
            WmiPerfOSProcessor.Columns.Add("PercentProcessorTime", typeof(ulong));
            WmiPerfOSProcessor.Columns.Add("PercentPrivilegedTime", typeof(ulong));
            WmiPerfOSProcessor.Columns.Add("PercentInterruptTime", typeof(ulong));
            WmiPerfOSProcessor.Columns.Add("Frequency_Sys100NS", typeof(ulong));
            WmiPerfOSProcessor.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
        }
    }
}
