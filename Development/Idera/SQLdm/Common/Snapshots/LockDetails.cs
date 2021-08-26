//------------------------------------------------------------------------------
// <copyright file="LockDetails.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using Data;

    /// <summary>
    /// Represents the snapshot for the lock list view
    /// </summary>
    [Serializable]
    public sealed class LockDetails : Snapshot, ISerializable
    {
        #region fields

        private Dictionary<Guid, Lock> lockList;
        private LockStatistics lockCounters;
        private DataSet blockingTreeByLock;
        private DataSet blockingTreeBySession;
        private bool hasBeenRowLimited;

        #endregion

        #region constructors

        internal LockDetails(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            lockList = new Dictionary<Guid, Lock>();
            lockCounters = new LockStatistics();
            blockingTreeByLock = null;
            blockingTreeBySession = null;
            hasBeenRowLimited = false;
        }

        public LockDetails(SerializationInfo info, StreamingContext context)
        {
            SetObjectData(info, context);
            lockList = (Dictionary<Guid, Lock>)info.GetValue("lockList", typeof(Dictionary<Guid, Lock>));
            lockCounters = (LockStatistics)info.GetValue("lockCounters", typeof(LockStatistics));
            blockingTreeByLock = null;
            blockingTreeBySession = null;
            try
            {
                hasBeenRowLimited = (bool)info.GetValue("hasBeenRowLimited", typeof (bool));
            }
            catch
            {
                hasBeenRowLimited = false;
            }
        }

        public LockDetails(SqlDataReader reader)
        {
            if (reader.FieldCount != 8)
                throw new ArgumentOutOfRangeException("reader", "reader contains invalid number of columns");

            base.TimeStamp = reader.GetDateTime(1);
            base.TimeStampLocal = base.TimeStamp.Value.ToLocalTime();

            if (reader.IsDBNull(5))
                lockCounters = new LockStatistics();
            else
            {
                lockCounters = Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>(reader.GetSqlBytes(5).Value);
            }
            if (reader.IsDBNull(6))
                lockList = new Dictionary<Guid, Lock>();
            else
            {
                lockList = Serialized<object>.DeserializeCompressed<Dictionary<Guid, Lock>>(reader.GetSqlBytes(6).Value);
            }
        }

        public LockDetails(DateTime utcCollectionDateTime, Dictionary<Guid, Lock> lockList)
        {
            TimeStamp = utcCollectionDateTime;
            this.lockList = lockList;
        }

        #endregion

        #region properties

        /// <summary>
        /// Returns the current Lock List as a blocking tree
        /// </summary>
        public DataSet BlockingTreeByLock
        {
            get
            {
                if (blockingTreeByLock == null)
                    blockingTreeByLock = GetBlockingTreeByLock(LockList);
                return blockingTreeByLock;
            }
        }

        /// <summary>
        /// Returns the current Lock List as a blocking tree
        /// </summary>
        public DataSet BlockingTreeBySession
        {
            get
            {
                if (blockingTreeBySession == null)
                    blockingTreeBySession = GetBlockingTreeBySession(LockList);
                return blockingTreeBySession;
            }
        }

        /// <summary>
        /// List of all locks on the system
        /// </summary>
        public Dictionary<Guid, Lock> LockList
        {
            get { return lockList; }
            internal set { lockList = value; }
        }

        /// <summary>
        /// Collection of lock counter statistics on the system
        /// </summary>
        public LockStatistics LockCounters
        {
            get { return lockCounters; }
            internal set { lockCounters = value; }
        }

        /// <summary>
        /// True when the batch has been cancelled due to row limiting
        /// </summary>
        public bool HasBeenRowLimited
        {
            get { return hasBeenRowLimited; }
            internal set { hasBeenRowLimited = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        
        /// <summary>
        /// Returns a dataset document representing LockDetails as a tree
        /// Returns a blank document if there is no blocking chain
        /// </summary>
        public static DataSet GetBlockingTreeByLock(Dictionary<Guid, Lock> InputLockList)
        {
            DataSet blockingDataSet = new DataSet("Blocking DataSet");
            blockingDataSet.EnforceConstraints = false; 

            DataTable leadBlockersTable = blockingDataSet.Tables.Add("Lead Blockers");
            leadBlockersTable.Columns.Add("Session ID", typeof(string));
            leadBlockersTable.Columns.Add("System Process", typeof(bool));
            leadBlockersTable.Columns.Add("Object", typeof(string));
            leadBlockersTable.Columns.Add("Database", typeof(string));
            leadBlockersTable.Columns.Add("Object Name", typeof(string));
            leadBlockersTable.Columns.Add("Object Schema", typeof(string));
            leadBlockersTable.Columns.Add("Type", typeof(string));
            leadBlockersTable.Columns.Add("Status", typeof(string));
            leadBlockersTable.Columns.Add("Blocking", typeof(bool));
            leadBlockersTable.Columns.Add("Blocked By", typeof(int));
            leadBlockersTable.Columns.Add("Wait Time", typeof(long));
            leadBlockersTable.Columns.Add("Mode", typeof(string));
            leadBlockersTable.Columns.Add("Count", typeof(decimal));
            leadBlockersTable.Columns.Add("Application", typeof(string));
            leadBlockersTable.Columns.Add("User", typeof(string));
            leadBlockersTable.Columns.Add("Host", typeof(string));
            leadBlockersTable.Columns.Add("RecordID", typeof(Pair<int, string>));

            leadBlockersTable.PrimaryKey = new DataColumn[] { leadBlockersTable.Columns["RecordID"] };


            DataTable blockedTable = blockingDataSet.Tables.Add("Locks");

            blockedTable.Columns.Add("Session ID", typeof(string));
            blockedTable.Columns.Add("System Process", typeof(bool));
            blockedTable.Columns.Add("Object", typeof(string));
            blockedTable.Columns.Add("Database", typeof(string));
            blockedTable.Columns.Add("Object Name", typeof(string));
            blockedTable.Columns.Add("Object Schema", typeof(string));
            blockedTable.Columns.Add("Type", typeof(string));
            blockedTable.Columns.Add("Status", typeof(string));
            blockedTable.Columns.Add("Blocking", typeof(bool));
            blockedTable.Columns.Add("Blocked By", typeof(int));
            blockedTable.Columns.Add("Wait Time", typeof(long));
            blockedTable.Columns.Add("Mode", typeof(string));
            blockedTable.Columns.Add("Count", typeof(decimal));
            blockedTable.Columns.Add("Application", typeof(string));
            blockedTable.Columns.Add("User", typeof(string));
            blockedTable.Columns.Add("Host", typeof(string));
            blockedTable.Columns.Add("RecordID", typeof(Pair<int, string>));
            blockedTable.Columns.Add("ParentID", typeof(Pair<int, string>));

            blockedTable.PrimaryKey = new DataColumn[] { blockedTable.Columns["RecordID"] };

            leadBlockersTable.BeginLoadData();
            blockedTable.BeginLoadData();

            foreach (Lock lockItem in InputLockList.Values)
            {
                DataRow dr;
                bool leadBlocker;

                if ((lockItem.Blocking.HasValue && lockItem.Blocking.Value)|| lockItem.Blocked)
                {
                    if ((!lockItem.Blocked) || (lockItem.Blocked && lockItem.Status == LockStatus.Granted))
                    {
                        dr = leadBlockersTable.NewRow();
                        leadBlocker = true;
                    }
                    else
                    {
                        dr = blockedTable.NewRow();
                        dr["ParentID"] =
                            new Pair<int, string>(lockItem.Blocked_by.HasValue ? lockItem.Blocked_by.Value : 0,
                                                  lockItem.Database + '.' + lockItem.ObjectSchema + '.' +
                                                  lockItem.ObjectName + ':' + lockItem.TypeString);
                        leadBlocker = false;
                    }

                    if (lockItem.Spid.HasValue)
                        dr["Session ID"] = lockItem.Spid;
                    dr["System Process"] = lockItem.IsSystemProcess;
                    dr["Object"] = lockItem.Database + '.' + lockItem.ObjectSchema + '.' +
                                                  lockItem.ObjectName + ':' + lockItem.TypeString;
                    dr["Database"] = lockItem.Database;
                    dr["Object Name"] = lockItem.ObjectName;
                    dr["Object Schema"] = lockItem.ObjectSchema;
                    dr["Type"] = lockItem.TypeString;
                    dr["Status"] = lockItem.StatusString;
                    dr["Blocking"] = lockItem.Blocking;
                    if (lockItem.Blocked_by.HasValue)
                        dr["Blocked By"] = lockItem.Blocked_by.Value;
                    dr["Wait Time"] = lockItem.WaitTime.TotalMilliseconds;
                    dr["Mode"] = lockItem.ModeLongString;
                    if (lockItem.InstanceCount.HasValue)
                        dr["Count"] = lockItem.InstanceCount.Value;
                    dr["Application"] = lockItem.Application;
                    dr["User"] = lockItem.User;
                    dr["Host"] = lockItem.Host;
                    dr["RecordID"] = lockItem.InternalLockIdentifier;

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

        /// <summary>
        /// Returns a dataset document representing LockDetails as a tree
        /// Returns a blank document if there is no blocking chain
        /// </summary>
        public static DataSet GetBlockingTreeBySession(Dictionary<Guid, Lock> InputLockList)
        {
            DataSet blockingDataSet = new DataSet("Blocking DataSet");
            blockingDataSet.EnforceConstraints = false;

            DataTable sessionsTable = blockingDataSet.Tables.Add("Sessions");

            sessionsTable.Columns.Add("Session ID", typeof(string));
            sessionsTable.Columns.Add("System Process", typeof(bool));
            sessionsTable.Columns.Add("Object", typeof(string));
            sessionsTable.Columns.Add("Database", typeof(string));
            sessionsTable.Columns.Add("Object Name", typeof(string));
            sessionsTable.Columns.Add("Object Schema", typeof(string));
            sessionsTable.Columns.Add("Type", typeof(string));
            sessionsTable.Columns.Add("Status", typeof(string));
            sessionsTable.Columns.Add("Blocking", typeof(bool));
            sessionsTable.Columns.Add("Blocked By", typeof(int));
            sessionsTable.Columns.Add("Mode", typeof(string));
            sessionsTable.Columns.Add("Count", typeof(decimal));
            sessionsTable.Columns.Add("Application", typeof(string));
            sessionsTable.Columns.Add("User", typeof(string));
            sessionsTable.Columns.Add("Host", typeof(string));

            sessionsTable.PrimaryKey = new DataColumn[] { sessionsTable.Columns["Session ID"] };

            DataTable leadBlockersTable = blockingDataSet.Tables.Add("Lead Blockers");

            leadBlockersTable.Columns.Add("Session ID", typeof(string));
            leadBlockersTable.Columns.Add("System Process", typeof(bool));
            leadBlockersTable.Columns.Add("Object", typeof(string));
            leadBlockersTable.Columns.Add("Database", typeof(string));
            leadBlockersTable.Columns.Add("Object Name", typeof(string));
            leadBlockersTable.Columns.Add("Object Schema", typeof(string));
            leadBlockersTable.Columns.Add("Type", typeof(string));
            leadBlockersTable.Columns.Add("Status", typeof(string));
            leadBlockersTable.Columns.Add("Blocking", typeof(bool));
            leadBlockersTable.Columns.Add("Blocked By", typeof(int));
            leadBlockersTable.Columns.Add("Mode", typeof(string));
            leadBlockersTable.Columns.Add("Count", typeof(decimal));
            leadBlockersTable.Columns.Add("Application", typeof(string));
            leadBlockersTable.Columns.Add("User", typeof(string));
            leadBlockersTable.Columns.Add("Host", typeof(string));
            leadBlockersTable.Columns.Add("RecordID", typeof(int));

            leadBlockersTable.PrimaryKey = new DataColumn[] { leadBlockersTable.Columns["RecordID"] };


            DataTable blockedTable = blockingDataSet.Tables.Add("Locks");

            blockedTable.Columns.Add("Session ID", typeof(string));
            blockedTable.Columns.Add("System Process", typeof(bool));
            blockedTable.Columns.Add("Object", typeof(string));
            blockedTable.Columns.Add("Database", typeof(string));
            blockedTable.Columns.Add("Object Name", typeof(string));
            blockedTable.Columns.Add("Object Schema", typeof(string));
            blockedTable.Columns.Add("Type", typeof(string));
            blockedTable.Columns.Add("Status", typeof(string));
            blockedTable.Columns.Add("Blocking", typeof(bool));
            blockedTable.Columns.Add("Blocked By", typeof(int));
            blockedTable.Columns.Add("Mode", typeof(string));
            blockedTable.Columns.Add("Count", typeof(decimal));
            blockedTable.Columns.Add("Application", typeof(string));
            blockedTable.Columns.Add("User", typeof(string));
            blockedTable.Columns.Add("Host", typeof(string));
            blockedTable.Columns.Add("RecordID", typeof(int));
            blockedTable.Columns.Add("ParentID", typeof(int));

            blockedTable.PrimaryKey = new DataColumn[] { blockedTable.Columns["RecordID"] };

            sessionsTable.BeginLoadData();
            leadBlockersTable.BeginLoadData();
            blockedTable.BeginLoadData();

            foreach (Lock lockItem in InputLockList.Values)
            {
                DataRow dr;
                bool leadBlocker;

                if ((lockItem.Blocking.HasValue && lockItem.Blocking.Value) || lockItem.Blocked)
                {
                    if (!lockItem.Blocked)
                    {
                        // Add the session to the root sessions table
                        if (!sessionsTable.Rows.Contains(lockItem.Spid))
                        {
                            dr = sessionsTable.NewRow();
                            if (lockItem.Spid.HasValue)
                            {
                                dr["Session ID"] = lockItem.Spid;
                                dr["System Process"] = lockItem.IsSystemProcess;
                                dr["Application"] = lockItem.Application;
                                dr["User"] = lockItem.User;
                                dr["Host"] = lockItem.Host;
                                sessionsTable.Rows.Add(dr);
                            }
                        }
                        dr = leadBlockersTable.NewRow();
                        leadBlocker = true;
                    }
                    else
                    {
                        dr = blockedTable.NewRow();
                        dr["ParentID"] = lockItem.Blocked_by.HasValue ? lockItem.Blocked_by.Value : 0;
                        leadBlocker = false;
                    }

                    if (lockItem.Spid.HasValue)
                        dr["Session ID"] = lockItem.Spid;
                    dr["System Process"] = lockItem.IsSystemProcess;
                    dr["Object"] = lockItem.Database + '.' + lockItem.ObjectSchema + '.' +
                                                  lockItem.ObjectName + ':' + lockItem.TypeString;
                    dr["Database"] = lockItem.Database;
                    dr["Object Name"] = lockItem.ObjectName;
                    dr["Object Schema"] = lockItem.ObjectSchema;
                    dr["Type"] = lockItem.TypeString;
                    dr["Status"] = lockItem.StatusString;
                    dr["Blocking"] = lockItem.Blocking;
                    if (lockItem.Blocked_by.HasValue)
                        dr["Blocked By"] = lockItem.Blocked_by.Value;
                    if (lockItem.InstanceCount.HasValue)
                        dr["Count"] = lockItem.InstanceCount.Value;
                    dr["Mode"] = lockItem.ModeShortString;
                    dr["Application"] = lockItem.Application;
                    dr["User"] = lockItem.User;
                    dr["Host"] = lockItem.Host;
                    dr["RecordID"] = lockItem.Spid;

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

            blockingDataSet.Relations.Add("relation1", blockingDataSet.Tables["Sessions"].Columns["Session ID"],
                                              blockingDataSet.Tables["Lead Blockers"].Columns["Session ID"], false);
            blockingDataSet.Relations.Add("relation2", blockingDataSet.Tables["Lead Blockers"].Columns["RecordID"],
                                              blockingDataSet.Tables["Locks"].Columns["ParentID"], false);
            blockingDataSet.Relations.Add("relation3", blockingDataSet.Tables["Locks"].Columns["RecordID"],
                                              blockingDataSet.Tables["Locks"].Columns["ParentID"], false);

            sessionsTable.EndLoadData();
            leadBlockersTable.EndLoadData();
            blockedTable.EndLoadData();

            return blockingDataSet;
        }

        #endregion

        #region interface implementations
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
            info.AddValue("lockList", lockList);
            info.AddValue("lockCounters", lockCounters);
            info.AddValue("hasBeenRowLimited", hasBeenRowLimited);
            // do not serialize the datasets - they can be rebuilt
        }

        #endregion

        #region nested types

        #endregion

    }
}
