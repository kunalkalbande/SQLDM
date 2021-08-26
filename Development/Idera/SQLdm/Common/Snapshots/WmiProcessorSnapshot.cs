//------------------------------------------------------------------------------
// <copyright file="WmiProcessorSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiProcessor information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiProcessorSnapshot : Snapshot
    {
        private DataTable wmiProcessor = new DataTable("WmiProcessor");
        private List<DataTable> listWmiProcessor = new List<DataTable>();//required for interval collections

        public List<DataTable> ListWmiProcessor
        {
            get { return listWmiProcessor; }
            set { listWmiProcessor = value; }
        }
        public WmiProcessorSnapshot(string serverName)
            : base(serverName)
        {
            wmiProcessor.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiProcessor
        {
            get { return wmiProcessor; }
            internal set { wmiProcessor = value; }
        }

        private void Initialize()
        {
           
        }
    }
}
