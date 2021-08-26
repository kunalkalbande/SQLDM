//------------------------------------------------------------------------------
// <copyright file="WmiServiceSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiService information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiServiceSnapshot : Snapshot
    {
        private DataTable wmiService = new DataTable("WmiService");

        public WmiServiceSnapshot(string serverName)
            : base(serverName)
        {
            wmiService.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiService
        {
            get { return wmiService; }
            internal set { wmiService = value; }
        }

        private void Initialize()
        {
            WmiService.Columns.Add("Name", typeof(string));
            WmiService.Columns.Add("ProcessId", typeof(uint));
            WmiService.Columns.Add("State", typeof(string));
        }
    }
}
