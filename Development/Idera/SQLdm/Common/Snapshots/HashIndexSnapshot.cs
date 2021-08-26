//------------------------------------------------------------------------------
// <copyright file="OutOfDateStatsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the hashInex info on a monitored server //SQLdm 10.0 (srishti purohit) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class HashIndexSnapshot : Snapshot
    {
        #region fields

        private string edition = string.Empty;
        private DataTable hashIndexInfo = new DataTable("hashInexInfo");
        private DataTable scannedHashIndexInfo = new DataTable("scannedHashIndexInfo");
        
        #endregion

        #region constructors

        internal HashIndexSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            hashIndexInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string Edition
        {
            get { return edition; }
            set { edition = value; }
        }

        public DataTable HashIndexInfo
        {
            get { return hashIndexInfo; }
            internal set { hashIndexInfo = value; }
        }
        public DataTable ScannedHashIndexInfo
        {
            get { return scannedHashIndexInfo; }
            set { scannedHashIndexInfo = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
