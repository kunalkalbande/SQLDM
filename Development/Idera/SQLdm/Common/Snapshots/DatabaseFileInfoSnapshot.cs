//------------------------------------------------------------------------------
// <copyright file="DatabaseFileInfoSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the DatabaseFileInfoSnapshot info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class GetMasterFilesSnapshot : Snapshot
    {
        #region fields

        private DataTable databaseFileInfo = new DataTable("DatabaseFileInfo");

        #endregion

        #region constructors

        internal GetMasterFilesSnapshot()
        {
        }

        internal GetMasterFilesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            databaseFileInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable DatabaseFileInfo
        {
            get { return databaseFileInfo; }
            internal set { databaseFileInfo = value; }
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
