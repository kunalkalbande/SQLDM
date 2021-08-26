//------------------------------------------------------------------------------
// <copyright file="WmiPerfOSSystemSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPerfOSSystem information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPerfOSSystemSnapshot : Snapshot
    {
        private DataTable wmiPerfOSSystem = new DataTable("WmiPerfOSSystem");
        private List<DataTable> listWmiPerfOSSystem = new List<DataTable>();//this is required for multiple collections

        public WmiPerfOSSystemSnapshot(string serverName)
            : base(serverName)
        {
            wmiPerfOSSystem.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPerfOSSystem
        {
            get { return wmiPerfOSSystem; }
            internal set { wmiPerfOSSystem = value; }
        }

        public List<DataTable> ListWmiPerfOSSystem
        {
            get { return listWmiPerfOSSystem; }
            internal set { listWmiPerfOSSystem = value; }
        }


        private void Initialize()
        {
           
        }
    }
}
