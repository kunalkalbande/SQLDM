//------------------------------------------------------------------------------
// <copyright file="DBSecuritySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the DBSecurity info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class DBSecuritySnapshot : Snapshot
    {
        #region fields

        private DataTable dBSecurity = new DataTable("DBSecurity");
        private string dbName = string.Empty;

        
        #endregion

        #region constructors

        internal DBSecuritySnapshot(SqlConnectionInfo info, string db)
            : base(info.InstanceName)
        {
            dBSecurity.RemotingFormat = SerializationFormat.Binary;
            //To fix recomm generation problem with SDR-S7
            dbName = db;
        }

        #endregion

        #region properties

        public DataTable DBSecurity
        {
            get { return dBSecurity; }
            internal set { dBSecurity = value; }
        }
        //To fix recomm generation problem with SDR-S7
        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
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
