//------------------------------------------------------------------------------
// <copyright file="SQLModuleOptionsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the SQLModuleOptions info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class SQLModuleOptionsSnapshot : Snapshot
    {
        #region fields

        private DataTable sQLModuleOptions = new DataTable("SQLModuleOptions");

        #endregion

        #region constructors

        internal SQLModuleOptionsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            sQLModuleOptions.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable SQLModuleOptions
        {
            get { return sQLModuleOptions; }
            internal set { sQLModuleOptions = value; }
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
