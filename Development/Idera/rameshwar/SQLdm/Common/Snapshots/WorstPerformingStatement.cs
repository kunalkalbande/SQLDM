//------------------------------------------------------------------------------
// <copyright file="WorstPerformingStatement.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// A single worst performing statement
    /// </summary>
    public class WorstPerformingStatement
    {
        #region fields

        private DateTime? _completionTime = null;
        private TimeSpan? _duration = null;
        private CpuTime _cpuTime = null;
        private long? _reads = null;
        private long? _writes = null;
        private string _database = null;
        private string _ntUser = null;
        private string _sqlUser = null;
        private string _client = null;
        private string _appName = null;
        private WorstPerformingStatementType? _type = null;
        
        
        #endregion

        #region constructors

        #endregion

        #region properties

        public DateTime? completionTime
        {
            get { return _completionTime; }
        }

        public TimeSpan? duration
        {
            get { return _duration; }
        }

        public CpuTime cpuTime
        {
            get { return _cpuTime; }
        }

        public long? reads
        {
            get { return _reads; }
        }

        public long? writes
        {
            get { return _writes; }
        }

        public string database
        {
            get { return _database; }
        }

        public string ntUser
        {
            get { return _ntUser; }
        }

        public string sqlUser
        {
            get { return _sqlUser; }
        }

        public string client
        {
            get { return _client; }
        }

        public string appName
        {
            get { return _appName; }
        }

        public WorstPerformingStatementType? type
        {
            get { return _type; }
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
