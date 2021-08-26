//------------------------------------------------------------------------------
// <copyright file="WmiVolumeSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiVolume information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiVolumeSnapshot : Snapshot
    {
        private DataTable wmiVolume = new DataTable("WmiVolume");

        public WmiVolumeSnapshot(string serverName)
            : base(serverName)
        {
            wmiVolume.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiVolume
        {
            get { return wmiVolume; }
            internal set { wmiVolume = value; }
        }

        private void Initialize()
        {
            WmiVolume.Columns.Add("Name", typeof(string));
            WmiVolume.Columns.Add("FileSystem", typeof(string));
            WmiVolume.Columns.Add("BlockSize", typeof(ulong));
        }
    }
}
