//------------------------------------------------------------------------------
// <copyright file="TraceStatement.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Data;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public class TraceStatement : ISerializable
    {
        #region fields

        private DateTime? completionTime = null;
        private TimeSpan? duration = null;
        private TimeSpan? cpuTime = null;
        private long? reads = null;
        private long? writes = null;
        private byte[] compressedSqlText = null;
        private TraceEventType? eventType = null;
        private int sequenceNumber = 0;
        private int spid = 0;

        #endregion

        #region constructors

        public TraceStatement() { }

        protected TraceStatement(SerializationInfo info, StreamingContext context)
        {
            completionTime = (DateTime?)info.GetValue("completionTime", typeof(DateTime?));
            duration = (TimeSpan?)info.GetValue("duration", typeof(TimeSpan?));
            cpuTime = (TimeSpan?)info.GetValue("cpuTime", typeof(TimeSpan?));
            reads = (long?)info.GetValue("reads", typeof(long?));
            writes = (long?)info.GetValue("writes", typeof(long?));
            compressedSqlText =(byte[]) info.GetValue("compressedSqlText",typeof(byte[]));
            eventType = (TraceEventType?)info.GetValue("eventType", typeof(TraceEventType?));
            sequenceNumber = (int)info.GetValue("sequenceNumber", typeof(int));
            spid = (int) info.GetValue("spid", typeof (int));
        }

        #endregion

        #region properties

        /// <summary>
        /// Completion time for the statement or batch
        /// </summary>
        public DateTime? CompletionTime
        {
            get { return completionTime; }
            internal set { completionTime = value; }
        }

        /// <summary>
        /// Duration for the statement
        /// </summary>
        public TimeSpan? Duration
        {
            get { return duration; }
            internal set { duration = value; }
        }

        /// <summary>
        /// Cpu time utilized by the statement
        /// </summary>
        public TimeSpan? CpuTime
        {
            get { return cpuTime; }
            internal set { cpuTime = value; }
        }

        /// <summary>
        /// Number of logical disk reads performed by the server on behalf of the event. 
        /// </summary>
        public long? Reads
        {
            get { return reads; }
            internal set { reads = value; }
        }

        /// <summary>
        /// Number of physical disk writes performed by the server on behalf of the event.
        /// </summary>
        public long? Writes
        {
            get { return writes; }
            internal set { writes = value; }
        }


        /// <summary>
        /// The text of the SQL statement
        /// Compressed when set so use sparingly
        /// </summary>
        public string SqlText
        {
            get { return Serialized<string>.DeserializeCompressed<string>(compressedSqlText);}
            internal set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    compressedSqlText = Serialized<string>.SerializeCompressed<string>((value.Trim()).TrimStart(new char[] { '\n', '\r' }));
                }
            }
        }


        /// <summary>
        /// The trace event type
        /// </summary>
        public TraceEventType? EventType
        {
            get { return eventType; }
            internal set { eventType = value; }
        }

        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }

        /// <summary>
        /// The trace event spid
        /// </summary>
        public int Spid
        {
            get { return spid; }
            internal set { spid = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("completionTime", completionTime);
            info.AddValue("duration", duration);
            info.AddValue("cpuTime", cpuTime);
            info.AddValue("reads", reads);
            info.AddValue("writes", writes);
            info.AddValue("compressedSqlText", compressedSqlText);
            info.AddValue("eventType", eventType);
            info.AddValue("sequenceNumber", sequenceNumber);
            info.AddValue("spid", spid);
        }

        #endregion

        #region nested types

        #endregion
    }
}
