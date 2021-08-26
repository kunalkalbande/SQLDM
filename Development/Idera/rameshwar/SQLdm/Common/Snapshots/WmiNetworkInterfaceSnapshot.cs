//------------------------------------------------------------------------------
// <copyright file="WmiNetworkInterfaceSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiNetworkInterface information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiNetworkInterfaceSnapshot : Snapshot
    {
        private DataTable wmiNetworkInterface = new DataTable("WmiNetworkInterface");

        public WmiNetworkInterfaceSnapshot(string serverName)
            : base(serverName)
        {
            wmiNetworkInterface.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiNetworkInterface
        {
            get { return wmiNetworkInterface; }
            internal set { wmiNetworkInterface = value; }
        }

        private void Initialize()
        {
            WmiNetworkInterface.Columns.Add("Name", typeof(string));
            WmiNetworkInterface.Columns.Add("PacketsPerSec", typeof(uint));
            WmiNetworkInterface.Columns.Add("OutputQueueLength", typeof(uint));
            WmiNetworkInterface.Columns.Add("PacketsReceivedErrors", typeof(uint));
            WmiNetworkInterface.Columns.Add("PacketsOutboundErrors", typeof(uint));
            WmiNetworkInterface.Columns.Add("CurrentBandwidth", typeof(ulong));
            WmiNetworkInterface.Columns.Add("BytesTotalPerSec", typeof(ulong));
            WmiNetworkInterface.Columns.Add("Frequency_Sys100NS", typeof(ulong));
            WmiNetworkInterface.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
        }
    }
}
