//------------------------------------------------------------------------------
// <copyright file="QueryAnalyzerSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

    /// <summary>
    /// Represents the QueryAnalyzer info on a monitored server //SQLdm 10.0 (Srishti Purohit) Q46,Q47,Q48,Q49,Q50 -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class QueryAnalyzerSnapshot : Snapshot
    {
        #region fields

        private string dbname;

        private int actualState;

        private AffectedBatches affectedBatchesQ46;
        private AffectedBatches affectedBatchesQ47;
        private AffectedBatches affectedBatchesQ48;
        private AffectedBatches affectedBatchesQ49;
        private AffectedBatches affectedBatchesQ50;

        //private string batchQ46;

        //private string batchQ47;

        //private string batchQ48;

        //private string batchQ49;

        //private string batchQ50;
        
        #endregion

        #region constructors

        internal QueryAnalyzerSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties
        
        public string Dbname
        {
            get { return dbname; }
            set { dbname = value; }
        }
        
        public int ActualState
        {
            get { return actualState; }
            set { actualState = value; }
        }
        public AffectedBatches AffectedBatchesQ46
        {
            get { return affectedBatchesQ46; }
            set { affectedBatchesQ46 = value; }
        }

        public AffectedBatches AffectedBatchesQ47
        {
            get { return affectedBatchesQ47; }
            set { affectedBatchesQ47 = value; }
        }

        public AffectedBatches AffectedBatchesQ48
        {
            get { return affectedBatchesQ48; }
            set { affectedBatchesQ48 = value; }
        }

        public AffectedBatches AffectedBatchesQ49
        {
            get { return affectedBatchesQ49; }
            set { affectedBatchesQ49 = value; }
        }

        public AffectedBatches AffectedBatchesQ50
        {
            get { return affectedBatchesQ50; }
            set { affectedBatchesQ50 = value; }
        }
        //public string BatchQ46
        //{
        //    get { return batchQ46; }
        //    set { batchQ46 = value; }
        //}
        //public string BatchQ47
        //{
        //    get { return batchQ47; }
        //    set { batchQ47 = value; }
        //}
        //public string BatchQ48
        //{
        //    get { return batchQ48; }
        //    set { batchQ48 = value; }
        //}
        //public string BatchQ49
        //{
        //    get { return batchQ49; }
        //    set { batchQ49 = value; }
        //}
        //public string BatchQ50
        //{
        //    get { return batchQ50; }
        //    set { batchQ50 = value; }
        //}

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
