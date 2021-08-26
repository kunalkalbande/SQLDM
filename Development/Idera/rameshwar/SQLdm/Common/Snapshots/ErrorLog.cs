//------------------------------------------------------------------------------
// <copyright file="ErrorLogDetail.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Data;

    /// <summary>
    /// Represents a fully-populated error log
    /// </summary>
    [Serializable]
    public sealed class ErrorLog : Snapshot, ISerializable
    {
        #region fields

        private const int MaxRecordsBeforeCompression = 7500;

        private DataTable                   messages = null;

        private ErrorLogTerminationType?    logTerminationType = null;
        private DateTime?                   earliestDate = null;
        private DateTime?                   latestDate = null;
        private bool                        hasBeenInternallyFiltered = false;
        private List<byte[]>                compressedDataTables = null; 

        #endregion

        #region constructors

        internal ErrorLog(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            messages = new DataTable("SQL Server Error Log");
            messages.RemotingFormat = SerializationFormat.Binary;
            messages.Columns.Add("Log Source", typeof (LogFileType));
            messages.Columns.Add("Log Name", typeof (string));
            messages.Columns.Add("Message Number", typeof (int));
            messages.Columns.Add("Message",typeof(string));
            messages.Columns.Add("MessageType", typeof(MonitoredState));
            messages.Columns.Add("Source",typeof(string));
            messages.Columns.Add("Local Time",typeof(DateTime));
        }

        public ErrorLog(SerializationInfo info, StreamingContext context)
        {
            // called here to initialize Snapshot base class which does not extend ISerializable
            SetObjectData(info, context);

            logTerminationType = (Nullable<ErrorLogTerminationType>)info.GetValue("logTerminationType",typeof(Nullable<ErrorLogTerminationType>));
            earliestDate = (Nullable<DateTime>)info.GetValue("earliestDate", typeof(Nullable<DateTime>));
            latestDate = (Nullable<DateTime>)info.GetValue("latestDate", typeof(Nullable<DateTime>));
            hasBeenInternallyFiltered = info.GetBoolean("hasBeenInternallyFiltered");
            compressedDataTables = (List<byte[]>)info.GetValue("compressedDataTables", typeof (List<byte[]>));
            messages = (DataTable) info.GetValue("messages", typeof (DataTable));
            if (messages == null)
                messages = GetMessagesTable();

            if (compressedDataTables != null && compressedDataTables.Count > 0)
            {
                // messages will contain nothing 
                messages.BeginLoadData();
                foreach (byte[] serialzedTable in compressedDataTables)
                {   
                    // merge in each deserialized chunk
                    DataTable chunk = Serialized<DataTable>.DeserializeCompressed<DataTable>(serialzedTable);
                    messages.Merge(chunk, false);
                    chunk.Clear();
                }
                messages.EndLoadData();

                // clear the list and null its reference so it will get garbage collected
                compressedDataTables.Clear();
                compressedDataTables = null;
            }
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);

            info.AddValue("logTerminationType", logTerminationType);
            info.AddValue("earliestDate", earliestDate);
            info.AddValue("latestDate", latestDate);
            info.AddValue("hasBeenInternallyFiltered", hasBeenInternallyFiltered);

            if (messages.Rows.Count > 0)
            {
                CompressMessages();
            }

            info.AddValue("compressedDataTables", compressedDataTables);
            info.AddValue("messages", null);
        }

        #region properties

        /// <summary>
        /// Enumeration describing the manner in which the log file was closed
        /// </summary>
        public ErrorLogTerminationType? LogTerminationType
        {
            get { return logTerminationType; }
        }

        /// <summary>
        /// Collection of individual informational and error messages from the log
        /// </summary>
        public DataTable Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// Earliest date in the collection
        /// </summary>
        public DateTime? EarliestDate
        {
            get { return earliestDate; }
            internal set { earliestDate = value; }
        }

        /// <summary>
        /// Latest date in the collection
        /// </summary>
        public DateTime? LatestDate
        {
            get { return latestDate; }
            internal set { latestDate = value; }
        }

        /// <summary>
        /// Indicates that filtering occurred based on an internal (non-user-triggered) filter
        /// </summary>
        public bool HasBeenInternallyFiltered
        {
            get { return hasBeenInternallyFiltered; }
            internal set { hasBeenInternallyFiltered = value; }
        }

        #endregion

        #region methods

        public DataRow NewRow()
        {
            DataTable table = GetMessagesTable();
            return table.NewRow();
        }

        public void AddRow(DataRow row)
        {
            DataTable table = GetMessagesTable();
            if (table.Rows.Count >= MaxRecordsBeforeCompression)
            {
                CompressMessages();
            }

            table.Rows.Add(row);
        }

        public void Clear()
        {
            if (messages != null)
                messages.Clear();
            if (compressedDataTables != null)
                compressedDataTables.Clear();
        }

        private void CompressMessages()
        {            
            if (compressedDataTables == null)
                compressedDataTables = new List<byte[]>();

            messages.EndLoadData();
            byte[] bytes = Serialized<DataTable>.SerializeCompressed<DataTable>(messages);
            this.compressedDataTables.Add(bytes);

            messages.Clear();
            messages.BeginLoadData();
        }

        private DataTable GetMessagesTable()
        {
            if (messages == null)
            {
                messages = new DataTable("SQL Server Error Log");
                messages.RemotingFormat = SerializationFormat.Binary;
                messages.Columns.Add("Log Source", typeof(LogFileType));
                messages.Columns.Add("Log Name", typeof(string));
                messages.Columns.Add("Message Number", typeof(int));
                messages.Columns.Add("Message", typeof(string));
                messages.Columns.Add("MessageType", typeof(MonitoredState));
                messages.Columns.Add("Source", typeof(string));
                messages.Columns.Add("Local Time", typeof(DateTime));
                messages.BeginLoadData();
            }
            return messages;
        }

        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public override string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();
        //    dump.Append("ArchiveName: " + ArchiveName); dump.Append("\n");
        //    dump.Append("Server: " + Server); dump.Append("\n");
        //    dump.Append("LogNumber: " + LogNumber.ToString()); dump.Append("\n");
        //    dump.Append("LogTerminationType: " + LogTerminationType.ToString()); dump.Append("\n");
        //    foreach(ErrorLogLine message in Messages)
        //    {
        //        dump.Append(message.Dump());
        //    }
        //    return dump.ToString();
        //}

        ///// <summary>
        ///// Sets the messages in the error log
        ///// </summary>
        //internal void SetMessages(ErrorLogLineCollection messages)
        //{
        //    _messages = messages;
        //}

        ///// <summary>
        ///// Sets the termination type of the error log
        ///// </summary>
        //internal void SetLogTerminationType(ErrorLogTerminationType logTerminationType)
        //{
        //    _logTerminationType = logTerminationType;
        //}


        #endregion




        #region ISerializable Members


        #endregion
    }
}
