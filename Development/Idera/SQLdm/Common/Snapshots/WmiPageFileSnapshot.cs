//------------------------------------------------------------------------------
// <copyright file="WmiPageFileSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiPageFile information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiPageFileSnapshot : Snapshot
    {
        private DataTable wmiPageFile = new DataTable("WmiPageFile");

        public WmiPageFileSnapshot(string serverName)
            : base(serverName)
        {
            wmiPageFile.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiPageFile
        {
            get { return wmiPageFile; }
            internal set { wmiPageFile = value; }
        }

        private void Initialize()
        {
            WmiPageFile.Columns.Add("Name", typeof(string));
        }
    }
}
