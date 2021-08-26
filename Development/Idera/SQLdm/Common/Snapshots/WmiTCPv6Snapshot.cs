//------------------------------------------------------------------------------
// <copyright file="WmiTCPv6Snapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiTCPv6 information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiTCPv6Snapshot : Snapshot
    {
        private DataTable wmiTCP = new DataTable("WmiTCP");

        public WmiTCPv6Snapshot(string serverName)
            : base(serverName)
        {
            wmiTCP.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiTCP
        {
            get { return wmiTCP; }
            internal set { wmiTCP = value; }
        }

        private void Initialize()
        {
            WmiTCP.Columns.Add("SegmentsPersec", typeof(uint));
            WmiTCP.Columns.Add("SegmentsRetransmittedPerSec", typeof(uint));
        }
    }
}
