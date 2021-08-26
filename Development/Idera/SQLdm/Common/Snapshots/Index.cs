//------------------------------------------------------------------------------
// <copyright file="Index.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents basic information on an index
    /// </summary>
    [Serializable]
    public class Index 
    {
        #region fields

//        private Table ownerTable = null;
        private string name = null;
        private bool? clustered = null;
        private int? levels = null;
        private int? rowsModifiedSinceStatistics = null;
        private long? tableRows = null;
        private bool? unique = null;
        private DateTime? lastUpdate = null;
        private FileSize size = new FileSize();
        private string filegroupName = null;
        private int? fillFactor = null;
        

        #endregion

        #region constructors

        public Index()
        {
            
        }

        #endregion

        #region properties


        /// <summary>
        /// Returns whether the index is clustered
        /// </summary>
        public bool? Clustered
        {
            get { return clustered; }
            internal set { clustered = value; }
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
        /// Gets number of rows modified since statistics were last updated
        /// </summary>
        public int? RowsModifiedSinceStatistics
        {
            get { return rowsModifiedSinceStatistics; }
            internal set { rowsModifiedSinceStatistics = value; }
        }

        /// <summary>
        /// Gets number of rows in the owning table
        /// </summary>
        public long? Tablerows
        {
            get { return tableRows; }
            internal set { tableRows = value; }
        }


        /// <summary>
        /// Gets whether this is a unique index
        /// </summary>
        public bool? Unique
        {
            get { return unique; }
            internal set { unique = value; }
        }

        /// <summary>
        /// Gets name of index
        /// </summary>
        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }
        
        public DateTime? LastUpdate
        {
            get { return lastUpdate; }
            internal set { lastUpdate = value; }
        }

        public FileSize Size
        {
            get { return size; }
            internal set { size = value; }
        }


        public int? Levels
        {
            get { return levels; }
            set { levels = value; }
        }

        public int? FillFactor
        {
            get { return fillFactor; }
            set { fillFactor = value; }
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
