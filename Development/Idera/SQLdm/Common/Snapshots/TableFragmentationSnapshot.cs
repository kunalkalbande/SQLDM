//------------------------------------------------------------------------------
// <copyright file="TableFragmentationSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;


    [Serializable]
    public class TableFragmentationSnapshot : Snapshot, IContinuousSnapshot
    {
        #region fields

        private Dictionary<String, DatabaseStatistics> dbStatistics = new Dictionary<string, DatabaseStatistics>();

        #endregion

        #region constructors

        internal TableFragmentationSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public Dictionary<String, DatabaseStatistics> DbStatistics
        {
            get { return dbStatistics; }
            internal set { dbStatistics = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void CombineSnapshot(IContinuousSnapshot snapshot)
        {
		    // SQLdm Minimum Privileges - Varun Chopra - Include Server Permissions for Continous Callbacks
            MinimumPermissions = ((Snapshot)snapshot).MinimumPermissions;
            MetadataPermissions = ((Snapshot)snapshot).MetadataPermissions;
            CollectionPermissions = ((Snapshot)snapshot).CollectionPermissions;

            foreach (DatabaseStatistics dbstats in ((TableFragmentationSnapshot)snapshot).DbStatistics.Values)
            {
                if (!DbStatistics.ContainsKey(dbstats.Name))
                {
                    DbStatistics.Add(dbstats.Name, dbstats);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }

}
