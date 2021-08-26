//------------------------------------------------------------------------------
// <copyright file="FullTextTablesConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for full text tables 
    /// </summary>
    [Serializable]
    public class FullTextTablesConfiguration : OnDemandConfiguration
    {
        #region fields

        string databaseName = null;
        string catalogName = null;

        #endregion

        #region constructors

        public FullTextTablesConfiguration(int monitoredServerId, string databaseName, string catalogName) : base(monitoredServerId)
        {
            if (databaseName == null) throw new ArgumentNullException("databaseName");
            if (catalogName == null) throw new ArgumentNullException("catalogName");

            this.databaseName = databaseName;
            this.catalogName = catalogName;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (DatabaseName != null && CatalogName != null); }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string CatalogName
        {
            get { return catalogName; }
            set { catalogName = value; }
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
