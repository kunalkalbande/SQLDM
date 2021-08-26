//------------------------------------------------------------------------------
// <copyright file="WmiProcessSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiProcess information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiProcessSnapshot : Snapshot
    {
        private DataTable wmiProcess = new DataTable("WmiProcess");

        public WmiProcessSnapshot(string serverName)
            : base(serverName)
        {
            wmiProcess.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiProcess
        {
            get { return wmiProcess; }
            internal set { wmiProcess = value; }
        }

        private void Initialize()
        {
            WmiProcess.Columns.Add("Name", typeof(string));
            WmiProcess.Columns.Add("CommandLine", typeof(string));
            WmiProcess.Columns.Add("Priority", typeof(uint));
            WmiProcess.Columns.Add("ThreadCount", typeof(uint));
            WmiProcess.Columns.Add("ProcessId", typeof(uint));
            WmiProcess.Columns.Add("WorkingSetSize", typeof(ulong));
        }
    }
}
