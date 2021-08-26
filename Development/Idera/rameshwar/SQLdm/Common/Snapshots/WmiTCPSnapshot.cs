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
    /// Represents the WmiTCP information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiTCPSnapshot : Snapshot
    {
        private DataTable wmiTCP = new DataTable("WmiTCP");

        public WmiTCPSnapshot(string serverName)
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
