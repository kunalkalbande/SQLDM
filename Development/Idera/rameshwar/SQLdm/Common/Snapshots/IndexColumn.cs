//------------------------------------------------------------------------------
// <copyright file="IndexColumn.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents an index column
    /// </summary>
    [Serializable]
    public sealed class IndexColumn
    {
        #region fields

        private double? _averageLength = null;
        private double? _averageRowHits = null;
        private string _columns = null;
        private double? _rowHitsPercent = null;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor for SQL 2000
        /// </summary>
        internal IndexColumn(double? averageLength, double? rowHitsPercent, double? averageRowHits, string columns)
        {
            if (columns == null || columns.Length == 0) throw new ArgumentNullException("columns");
            _averageLength = averageLength;
            _averageRowHits = averageRowHits;
            _columns = columns;
            _rowHitsPercent = rowHitsPercent * 100;
        }

        #endregion

        #region properties

        /// <summary>
        /// Returns average row length for an index column for SQL Server 2000
        /// </summary>
        public double? AverageLength
        {
            get { return _averageLength; }
        }

        /// <summary>
        /// Returns average row hits for an index column
        /// </summary>
        public double? AverageRowHits
        {
            get { return _averageRowHits; }
        }

        /// <summary>
        /// Returns the indexed column or columns
        /// </summary>
        public string Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Returns the Row Hits Percent
        /// </summary>
        public double? RowHitsPercent
        {
            get { return _rowHitsPercent; }

        }

        #endregion

        #region methods

        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();
        //    dump.Append("Columns: " + Columns); dump.Append("\n");
        //    dump.Append("AverageLength: " + (AverageLength > -1 ? AverageLength.ToString() : "not set")); dump.Append("\n");
        //    dump.Append("AverageRowHits: " + (AverageLength > -1 ? AverageRowHits.ToString() : "not set")); dump.Append("\n");

        //    return dump.ToString();
        //}

        #endregion

    }
}
