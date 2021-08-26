//------------------------------------------------------------------------------
// <copyright file="FullTextCatalog.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents all full text catalogs on a server
    /// </summary>
    [Serializable]
    public sealed class FullTextCatalogs : Snapshot
    {
        #region fields

        private DataTable catalogs = new DataTable("FullTextCatalogs");
        private ServiceState fullTextServiceStatus = ServiceState.UnableToMonitor;
        private int? processPriority = null;
        
        #endregion

        #region constructors

        internal FullTextCatalogs(SqlConnectionInfo info)
            : base(info.InstanceName)
         {
            DataColumn[] keys = new DataColumn[2];


            DataColumn column;
            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName= "Database";

            keys[0] = column;
            catalogs.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "Catalog";

            keys[1] = column;
            catalogs.Columns.Add(column);

            catalogs.PrimaryKey = keys;
            catalogs.RemotingFormat = SerializationFormat.Binary;
            catalogs.Columns.Add("Path", typeof(string));
            catalogs.Columns.Add("Accent Sensitivity", typeof(bool));
            catalogs.Columns.Add("Size", typeof(FileSize));
            catalogs.Columns.Add("ItemCount", typeof(long));
            catalogs.Columns.Add("LastPopulation", typeof(DateTime));
            catalogs.Columns.Add("PopulationStatus", typeof(FullTextSearchPopulationStatus));
            catalogs.Columns.Add("KeyCount", typeof(long));
            catalogs.Columns.Add("TableCount", typeof (long));
            catalogs.Columns.Add("Owner", typeof(string));
            catalogs.Columns.Add("IsDefault", typeof(bool));
            catalogs.Columns.Add("Filegroup", typeof(string));
         }

        #endregion

        #region properties

        public DataTable Catalogs
        {
            get { return catalogs; }
            internal set { catalogs = value; }
        }


        public ServiceState FullTextServiceStatus
        {
            get { return fullTextServiceStatus; }
            set { fullTextServiceStatus = value; }
        }

        public int? ProcessPriority
        {
            get { return processPriority; }
            set { processPriority = value; }
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
