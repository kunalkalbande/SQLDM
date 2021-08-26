//------------------------------------------------------------------------------
// <copyright file="ActiveWaitsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;


    public interface IContinuousSnapshot 
    {
        void CombineSnapshot(IContinuousSnapshot snapshot);
        
    }

    /// <summary>
    /// Represents a collection of active waits from a server
    /// </summary>
    [Serializable]
    public class ActiveWaitsSnapshot : Snapshot, IContinuousSnapshot
    {
        #region fields

        DataTable activeWaits = new DataTable("ActiveWaits");

        #endregion

        #region constructors

        public ActiveWaitsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            activeWaits.Columns.Add(new DataColumn("StatementUTCStartTime", typeof (DateTime)));
            activeWaits.Columns.Add(new DataColumn("WaitDuration", typeof(decimal)));//SQLdm 10.0 (Gaurav Karwal): changed from long to decimal
            activeWaits.Columns.Add(new DataColumn("SessionID", typeof(int)));
            activeWaits.Columns.Add(new DataColumn("WaitType", typeof(string)));
            activeWaits.Columns.Add(new DataColumn("HostName", typeof(string)));
            activeWaits.Columns.Add(new DataColumn("ProgramName", typeof(string))); 
            activeWaits.Columns.Add(new DataColumn("LoginName", typeof(string)));
            activeWaits.Columns.Add(new DataColumn("DatabaseName", typeof(string)));
            activeWaits.Columns.Add(new DataColumn("StatementText", typeof(string)));
            activeWaits.Columns.Add(new DataColumn("MSTicks", typeof(long)));
            activeWaits.Columns.Add(new DataColumn("UTCCollectionDateTime", typeof(DateTime)));
        }

        #endregion

        #region properties

        public DataTable ActiveWaits
        {
            get { return activeWaits; }
            internal set { activeWaits = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void CombineSnapshot(IContinuousSnapshot snapshot)
        {
			// SQLdm Minimum Privileges - Varun Chopra - Set permissions read with Continous Collectors- Minimum, Metadata and Collection
            MinimumPermissions = ((Snapshot)snapshot).MinimumPermissions;
            MetadataPermissions = ((Snapshot)snapshot).MetadataPermissions;
            CollectionPermissions = ((Snapshot)snapshot).CollectionPermissions;
            activeWaits.Merge(((ActiveWaitsSnapshot)snapshot).ActiveWaits);   
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }

}
