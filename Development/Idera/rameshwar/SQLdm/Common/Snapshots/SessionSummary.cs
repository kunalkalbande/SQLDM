//------------------------------------------------------------------------------
// <copyright file="SessionSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;
    using System.Data.SqlClient;
    using Data;

    /// <summary>
    /// Represents data for the Session Summary on demand view
    /// </summary>
    [Serializable]
    public sealed class SessionSummary : Snapshot, ISerializable
    {
        #region fields

        private TimeSpan? responseTime;
        private ServerSystemProcesses systemProcesses;
        private LockStatistics lockCounters;

        #endregion

        #region constructors

        public SessionSummary(SqlConnectionInfo connectionInfo)
            : base(connectionInfo.InstanceName)
        {
            responseTime = null;
            systemProcesses = new ServerSystemProcesses();
            lockCounters = new LockStatistics();
        }

        public SessionSummary(SerializationInfo info, StreamingContext context)
        {
            SetObjectData(info, context);
            responseTime = (TimeSpan?) info.GetValue("responseTime", typeof (TimeSpan?));
            systemProcesses = (ServerSystemProcesses)info.GetValue("systemProcesses", typeof(ServerSystemProcesses));
            lockCounters = (LockStatistics)info.GetValue("lockCounters", typeof(LockStatistics));
        }

        public SessionSummary(SqlDataReader reader)
        {
            if (reader.FieldCount != 8)
                throw new ArgumentOutOfRangeException("reader", "reader contains invalid number of columns");

            base.TimeStamp = reader.GetDateTime(1);
            base.TimeStampLocal = base.TimeStamp.Value.ToLocalTime();

            if (reader.IsDBNull(3))
                systemProcesses = new ServerSystemProcesses();
            else
            {
                systemProcesses = Serialized<ServerSystemProcesses>.DeserializeCompressed<ServerSystemProcesses>(reader.GetSqlBytes(3).Value);
            }
            if (reader.IsDBNull(5))
                lockCounters = new LockStatistics();
            else
            {
                lockCounters = Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>(reader.GetSqlBytes(5).Value);
            }
            if (reader.IsDBNull(7))
                responseTime = null;
            else
            {
                responseTime = TimeSpan.FromMilliseconds(reader.GetInt32(7));
            }
        }

        #endregion

        #region properties

        public TimeSpan? ResponseTime
        {
            get { return responseTime; }
            internal set { responseTime = value; }
        }

        public ServerSystemProcesses SystemProcesses
        {
            get { return systemProcesses; }
            internal set { systemProcesses = value; }
        }

        public LockStatistics LockCounters
        {
            get { return lockCounters; }
            internal set { lockCounters = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
            info.AddValue("responseTime", responseTime);
            info.AddValue("systemProcesses", systemProcesses);
            info.AddValue("lockCounters", lockCounters);
            // do not serialize the blocking session dataset - it can be rebuilt
        }

        #endregion

        #region nested types

        #endregion

    }
}
