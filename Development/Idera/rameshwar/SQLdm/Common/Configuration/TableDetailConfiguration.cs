//------------------------------------------------------------------------------
// <copyright file="TableDetailConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration info for table details on-demand probe
    /// </summary>
    [Serializable]
    public sealed class TableDetailConfiguration: OnDemandConfiguration
    {
        #region fields

        private int? tableID = null;
        private string database = null;

        #endregion

        #region constructors

        public TableDetailConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }


        public TableDetailConfiguration(int monitoredServerId, int? tableID, string database) : base(monitoredServerId)
        {
            this.tableID = tableID;
            this.database = database;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get
            {
                return (TableID.HasValue && Database != null);
            }
        }

        public int? TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        public string Database
        {
            get { return database; }
            set { database = value; }
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
