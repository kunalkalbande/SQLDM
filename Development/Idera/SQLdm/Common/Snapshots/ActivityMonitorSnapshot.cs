//------------------------------------------------------------------------------
// <copyright file="ActivityProfilerSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a collection of active waits from a server
    /// </summary>
    [Serializable]
    public class ActivityMonitorSnapshot : Snapshot, IContinuousSnapshot
    {
        #region fields

        DataTable _blockingReports = new DataTable("BlockingReports");

        #endregion

        #region constructors

        internal ActivityMonitorSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            _blockingReports.Columns.Add(new DataColumn("StatementUTCStartTime", typeof(DateTime)));
            _blockingReports.Columns.Add(new DataColumn("WaitDuration", typeof(long)));
            _blockingReports.Columns.Add(new DataColumn("SessionID", typeof(int)));
            _blockingReports.Columns.Add(new DataColumn("WaitType", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("HostName", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("ProgramName", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("LoginName", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("DatabaseName", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("StatementText", typeof(string)));
            _blockingReports.Columns.Add(new DataColumn("MSTicks", typeof(long)));
            _blockingReports.Columns.Add(new DataColumn("UTCCollectionDateTime", typeof(DateTime)));
        }

        #endregion

        #region properties

        public DataTable BlockingReports
        {
            get { return _blockingReports; }
            internal set { _blockingReports = value; }
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
            _blockingReports.Merge(((ActivityMonitorSnapshot)snapshot).BlockingReports);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }

}
