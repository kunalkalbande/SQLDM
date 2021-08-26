//------------------------------------------------------------------------------
// <copyright file="AdhocQuerySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    using System.Data;
    using Idera.SQLdm.Common.Configuration.ServerActions;

    /// <summary>
    /// Returns data from an adhoc query
    /// </summary>
    [Serializable]
    public class AdhocQuerySnapshot : ServerActionSnapshot
    {
        #region fields

        private AdhocQueryConfiguration config;
        private TimeSpan? duration;
        private int? rowsAffected;
        private int? rowSets;
        private DataSet dataset;

        #endregion

        #region constructors

        internal AdhocQuerySnapshot(string serverName)
            : base(serverName)
        {

        }

        #endregion

        #region properties

        public AdhocQueryConfiguration Configuration
        {
            get { return config; }
            internal set { config = value; }
        }

        /// <summary>
        /// Table name of reindexed object
        /// </summary>
        public TimeSpan? Duration
        {
            get { return duration; }
            internal set { duration = value; }
        }

        public int? RowsAffected
        {
            get { return rowsAffected; }
            internal set { rowsAffected = value; }
        }

        public int RowSetCount
        {
            get
            {
                if (rowSets.HasValue)
                    return rowSets.Value;

                if (dataset != null)
                    return dataset.Tables.Count;

                return 0;
            }
            set
            {
                rowSets = value;
            }
        }

        public DataSet DataSet
        {
            get { return dataset; }
            internal set { dataset = value; }
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
