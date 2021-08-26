//------------------------------------------------------------------------------
// <copyright file="TableGrowthSnapshot.cs" company="Idera, Inc.">
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
    public class TableGrowthSnapshot : Snapshot, IContinuousSnapshot
    {
        #region fields

        private Dictionary<String, DatabaseStatistics> dbStatistics = new Dictionary<string, DatabaseStatistics>();

        #endregion

        #region constructors

        internal TableGrowthSnapshot(SqlConnectionInfo info)
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
            foreach (DatabaseStatistics dbstats in ((TableGrowthSnapshot)snapshot).DbStatistics.Values)
            {
                if (!DbStatistics.ContainsKey(dbstats.Name))
                {
                    DbStatistics.Add(dbstats.Name,dbstats);
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
