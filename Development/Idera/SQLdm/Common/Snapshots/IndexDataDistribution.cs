//------------------------------------------------------------------------------
// <copyright file="IndexDataDistribution.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a row of index data distribution data
    /// </summary>
    [Serializable]
    public sealed class IndexDataDistribution
    {
        #region fields

        private string _dataValue = null;
        private double? _rows = null;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor for SQL 2000
        /// </summary>
        internal IndexDataDistribution(string dataValue, double rows)
        {
            _dataValue = dataValue;
            _rows = rows;
        }


        #endregion

        #region properties

        /// <summary>
        /// Returns data relating to the highst order column
        /// </summary>
        public string DataValue
        {
            get { return _dataValue; }
        }

        /// <summary>
        /// Returns number of rows with the same key for SQL Server 2000
        /// </summary>
        public double? Rows
        {
            get { return _rows; }
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
        //    dump.Append("DataValue: " + DataValue); dump.Append("\n");
        //    dump.Append("Rows: " + (Rows > -1 ? Rows.ToString() : "not set")); dump.Append("\n");

        //    return dump.ToString();
        //}
        #endregion

    }
}
