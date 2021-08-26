//------------------------------------------------------------------------------
// <copyright file="WmiEncryptableVolumeSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the WmiEncryptableVolume information of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public class WmiEncryptableVolumeSnapshot : Snapshot
    {
        private DataTable wmiEncryptableVolume = new DataTable("WmiEncryptableVolume");

        public WmiEncryptableVolumeSnapshot(string serverName)
            : base(serverName)
        {
            wmiEncryptableVolume.RemotingFormat = SerializationFormat.Binary;
            Initialize();
        }

        public DataTable WmiEncryptableVolume
        {
            get { return wmiEncryptableVolume; }
            internal set { wmiEncryptableVolume = value; }
        }

        private void Initialize()
        {
            WmiEncryptableVolume.Columns.Add("DriveLetter", typeof(string));
            WmiEncryptableVolume.Columns.Add("ProtectionStatus", typeof(uint));
        }
    }
}
