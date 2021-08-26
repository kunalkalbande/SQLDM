//------------------------------------------------------------------------------
// <copyright file="IndexDetail.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents detailed information on an index
    /// </summary>
    [Serializable]
    public sealed class IndexStatistics : Snapshot
    {
        #region fields

        private double? averageKeyLength = null;
        private List<IndexDataDistribution>dataDistribution = new List<IndexDataDistribution>();
        private int? distributionSteps = null;
        private string filegroupName = null;
        private List<IndexColumn> indexColumns = new List<IndexColumn>();
        private long? rowsSampled = null;

        #endregion

        #region constructors

        public IndexStatistics(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets average key length for index
        /// </summary>
        public double? AverageKeyLength
        {
            get { return averageKeyLength; }
            internal set { averageKeyLength = value; }
        }

        /// <summary>
        /// Gets collection of data distribution objects
        /// </summary>
        public List<IndexDataDistribution> DataDistribution
        {
            get { return dataDistribution; }
            internal set { dataDistribution = value; }
        }

        /// <summary>
        /// Gets distribution steps
        /// </summary>
        public int? DistributionSteps
        {
            get { return distributionSteps; }
            internal set { distributionSteps = value; }
        }

        /// <summary>
        /// Gets filegroup name
        /// </summary>
        public string FilegroupName
        {
            get { return filegroupName; }
            internal set { filegroupName = value; }
        }

        /// <summary>
        /// Gets collection of index columns
        /// </summary>
        public List<IndexColumn> IndexColumns
        {
            get { return indexColumns; }
            internal set { indexColumns = value; }
        }

        /// <summary>
        /// Gets number of rows sampled
        /// </summary>
        public long? RowsSampled
        {
            get { return rowsSampled; }
            internal set { rowsSampled = value; }
        }

        #endregion

        #region methods


        #endregion

    }
}
