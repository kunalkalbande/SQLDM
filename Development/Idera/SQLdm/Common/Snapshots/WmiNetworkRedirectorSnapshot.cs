//------------------------------------------------------------------------------
// <copyright file="WmiNetworkRedirectorSnapshot.cs" company="Idera, Inc.">
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
    public class WmiNetworkRedirectorSnapshot : Snapshot
    {
        private DataTable wmiNetworkRedirector = new DataTable("WmiNetworkRedirector");

        public WmiNetworkRedirectorSnapshot(string serverName)
            : base(serverName)
        {
            wmiNetworkRedirector.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiNetworkRedirector
        {
            get { return wmiNetworkRedirector; }
            internal set { wmiNetworkRedirector = value; }
        }

        private void Initialize()
        {
            WmiNetworkRedirector.Columns.Add("NetworkErrorsPerSec", typeof(uint));
        }
    }
}
