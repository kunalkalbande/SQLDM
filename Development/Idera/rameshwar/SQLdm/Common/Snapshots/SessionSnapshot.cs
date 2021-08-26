//------------------------------------------------------------------------------
// <copyright file="SessionSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using Data;

    /// <summary>
    /// Represents the sessions on a given server at a given point in time
    /// </summary>
    [Serializable]
    public sealed class SessionSnapshot : Snapshot, ISerializable
    {
        #region fields

        private Dictionary<Pair<int?, DateTime?>, Session> sessionList;
        private ServerSystemProcesses systemProcesses;
        private DataSet blockingTreeBySession;

        #endregion

        #region constructors

        internal SessionSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            sessionList = new Dictionary<Pair<int?, DateTime?>, Session>();
            systemProcesses = new ServerSystemProcesses();
            blockingTreeBySession = null;
        }

        public SessionSnapshot(SerializationInfo info, StreamingContext context)
        {
            base.SetObjectData(info, context);
            sessionList = (Dictionary<Pair<int?, DateTime?>, Session>)info.GetValue("sessionList", typeof(Dictionary<Pair<int?, DateTime?>, Session>));
            systemProcesses = (ServerSystemProcesses)info.GetValue("systemProcesses", typeof(ServerSystemProcesses));
            blockingTreeBySession = null;
        }

        public SessionSnapshot(SqlDataReader reader)
        {
            if (reader.FieldCount != 8)
                throw new ArgumentOutOfRangeException("reader", "reader contains invalid number of columns");

            DateTime collectionDateTime = reader.GetDateTime(1).ToLocalTime();

            base.TimeStamp = reader.GetDateTime(1);
            base.TimeStampLocal = base.TimeStamp.Value.ToLocalTime();

            if (reader.IsDBNull(3))
                systemProcesses = new ServerSystemProcesses();
            else
            {
                systemProcesses = Serialized<ServerSystemProcesses>.DeserializeCompressed<ServerSystemProcesses>(reader.GetSqlBytes(3).Value);
            }
            if (reader.IsDBNull(4))
                sessionList = new Dictionary<Pair<int?, DateTime?>, Session>();
            else
            {
                sessionList = Serialized<object>.DeserializeCompressed<Dictionary<Pair<int?,DateTime?>,Session>>(reader.GetSqlBytes(4).Value);
            }
        }

        public SessionSnapshot(DateTime utcCollectionDateTime, Dictionary<Pair<int?, DateTime?>,Session> sessionList)
        {
            TimeStamp = utcCollectionDateTime;
            this.sessionList = sessionList;
        }

        #endregion

        #region properties

        /// <summary>
        /// Returns the current Session List as a blocking tree
        /// </summary>
        public DataSet BlockingTreeBySession
        {
            get
            {
                if (blockingTreeBySession == null)
                    blockingTreeBySession = GetBlockingTreeBySession(sessionList);
                return blockingTreeBySession;
            }
        }

        /// <summary>
        /// List of sessions matching the request
        /// </summary>
        public Dictionary<Pair<int?, DateTime?>,Session> SessionList
        {
            get { return sessionList; }
            internal set { sessionList = value; }
        }

        /// <summary>
        /// Summary of sessions on the server
        /// </summary>
        public ServerSystemProcesses SystemProcesses
        {
            get { return systemProcesses; }
            internal set { systemProcesses = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a dataset representing SessionDetails as a tree
        /// Returns an empty dataset if there are no blocking chains
        /// </summary>
        public static DataSet GetBlockingTreeBySession(Dictionary<Pair<int?, DateTime?>, Session> InputSessionList)
        {
            DataSet blockingDataSet = new DataSet("Blocking DataSet");
            blockingDataSet.EnforceConstraints = false;

            DataTable leadBlockersTable = blockingDataSet.Tables.Add("Lead Blockers");

            leadBlockersTable.Columns.Add("Session ID", typeof(string));
            leadBlockersTable.Columns.Add("System Process", typeof(bool));
            leadBlockersTable.Columns.Add("Type", typeof(string));
            leadBlockersTable.Columns.Add("Status", typeof(string));
            leadBlockersTable.Columns.Add("Logged In", typeof(DateTime));
            leadBlockersTable.Columns.Add("Last Activity", typeof(DateTime));
            leadBlockersTable.Columns.Add("Database", typeof(string));
            leadBlockersTable.Columns.Add("Open Transactions", typeof(Int64));
            leadBlockersTable.Columns.Add("Blocking Count", typeof(int));
            leadBlockersTable.Columns.Add("Wait Time", typeof(Int64));
            leadBlockersTable.Columns.Add("Wait Type", typeof(string));
            leadBlockersTable.Columns.Add("Wait Resource", typeof(string));
            leadBlockersTable.Columns.Add("Cpu", typeof(Int64));
            leadBlockersTable.Columns.Add("Physical I/O", typeof(Int64));
            leadBlockersTable.Columns.Add("Memory", typeof(Int64));
            leadBlockersTable.Columns.Add("Command", typeof(string));
            leadBlockersTable.Columns.Add("Execution Context", typeof(string));
            leadBlockersTable.Columns.Add("Application", typeof(string));
            leadBlockersTable.Columns.Add("User", typeof(string));
            leadBlockersTable.Columns.Add("Host", typeof(string));
            leadBlockersTable.Columns.Add("RecordID", typeof(int));

            leadBlockersTable.PrimaryKey = new DataColumn[] { leadBlockersTable.Columns["RecordID"] };


            DataTable blockedTable = blockingDataSet.Tables.Add("Locks");

            blockedTable.Columns.Add("Session ID", typeof(string));
            blockedTable.Columns.Add("System Process", typeof(bool));
            blockedTable.Columns.Add("Type", typeof(string));
            blockedTable.Columns.Add("Status", typeof(string));
            blockedTable.Columns.Add("Logged In", typeof(DateTime));
            blockedTable.Columns.Add("Last Activity", typeof(DateTime));
            blockedTable.Columns.Add("Database", typeof(string));
            blockedTable.Columns.Add("Open Transactions", typeof(Int64));
            blockedTable.Columns.Add("Blocking Count", typeof(int));
            blockedTable.Columns.Add("Wait Time", typeof(Int64));
            blockedTable.Columns.Add("Wait Type", typeof(string));
            blockedTable.Columns.Add("Wait Resource", typeof(string));
            blockedTable.Columns.Add("Cpu", typeof(Int64));
            blockedTable.Columns.Add("Physical I/O", typeof(Int64));
            blockedTable.Columns.Add("Memory", typeof(Int64));
            blockedTable.Columns.Add("Command", typeof(string));
            blockedTable.Columns.Add("Execution Context", typeof(string));
            blockedTable.Columns.Add("Application", typeof(string));
            blockedTable.Columns.Add("User", typeof(string));
            blockedTable.Columns.Add("Host", typeof(string));
            blockedTable.Columns.Add("RecordID", typeof(int));
            blockedTable.Columns.Add("ParentID", typeof(int));

            blockedTable.PrimaryKey = new DataColumn[] { blockedTable.Columns["RecordID"] };

            leadBlockersTable.BeginLoadData();
            blockedTable.BeginLoadData();

            foreach (Session sessionItem in InputSessionList.Values)
            {
                DataRow dr;
                bool leadBlocker;

                if (sessionItem.Blocking || sessionItem.BlockedBy.HasValue)
                {
                    if (!sessionItem.BlockedBy.HasValue)
                    {
                        dr = leadBlockersTable.NewRow();
                        leadBlocker = true;
                    }
                    else
                    {
                        dr = blockedTable.NewRow();
                        dr["ParentID"] = sessionItem.BlockedBy.HasValue ? (int)sessionItem.BlockedBy.Value : 0;
                        leadBlocker = false;
                    }

                    if (sessionItem.Spid.HasValue)
                        dr["Session ID"] = sessionItem.Spid;
                    dr["System Process"] = sessionItem.IsSystemProcess;
                    dr["Type"] = sessionItem.IsSystemProcess ? "System" : "User";
                    dr["Status"] = sessionItem.Status.ToString();
                    if (sessionItem.LoggedInSince.HasValue)
                        dr["Logged In"] = sessionItem.LoggedInSince.Value.ToLocalTime();
                    if (sessionItem.LastActivity.HasValue)
                        dr["Last Activity"] = sessionItem.LastActivity.Value.ToLocalTime();
                    dr["Database"] = sessionItem.Database;
                    if (sessionItem.OpenTransactions.HasValue)
                        dr["Open Transactions"] = sessionItem.OpenTransactions.Value;
                    dr["Blocking Count"] = sessionItem.BlockingCount;
                    dr["Wait Time"] = sessionItem.WaitTime.TotalMilliseconds;
                    dr["Wait Type"] = sessionItem.WaitType;
                    dr["Wait Resource"] = sessionItem.WaitResource;
                    dr["Cpu"] = sessionItem.Cpu.TotalMilliseconds;
                    if (sessionItem.PhysicalIo.HasValue)
                        dr["Physical I/O"] = sessionItem.PhysicalIo.Value;
                    dr["Memory"] = sessionItem.Memory.Kilobytes;
                    dr["Command"] = sessionItem.Command;
                    if (sessionItem.ExecutionContext.HasValue)
                        dr["Execution Context"] = sessionItem.ExecutionContext.Value.ToString();
                    dr["Blocking Count"] = sessionItem.BlockingCount;
                    dr["Application"] = sessionItem.Application;
                    dr["User"] = sessionItem.UserName;
                    dr["Host"] = sessionItem.Workstation;
                    dr["RecordID"] = sessionItem.Spid;

                    if (leadBlocker)
                    {
                        leadBlockersTable.Rows.Add(dr);
                    }
                    else
                    {
                        blockedTable.Rows.Add(dr);
                    }
                }
            }

            blockingDataSet.Relations.Add("relation1", blockingDataSet.Tables["Lead Blockers"].Columns["RecordID"],
                                              blockingDataSet.Tables["Locks"].Columns["ParentID"], false);
            blockingDataSet.Relations.Add("relation2", blockingDataSet.Tables["Locks"].Columns["RecordID"],
                                              blockingDataSet.Tables["Locks"].Columns["ParentID"], false);

            leadBlockersTable.EndLoadData();
            blockedTable.EndLoadData();

            return blockingDataSet;
        }

        #endregion

        #region interface implementations

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
            info.AddValue("sessionList", sessionList);
            info.AddValue("systemProcesses", systemProcesses);
            // do not serialize the dataset - it can be rebuilt
        }

        #endregion

        #region nested types

        #endregion

    }
}
