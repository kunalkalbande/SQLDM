//------------------------------------------------------------------------------
// <copyright file="OpenTransaction.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a single open transaction
    /// </summary>
    [Serializable]
    public sealed class OpenTransaction : Session
    {
        #region fields

        private TimeSpan? runTime = null;
        private DateTime? startTime = null;
        //10.0 SQLdm Srishti Purohit -- baseline mean comparision change
        private double? transMinutesBaseline;
        private double? transMinutesPercentage;

        #endregion

        #region constructors

        public OpenTransaction() : base() { }

        public OpenTransaction(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            runTime = (TimeSpan?)info.GetValue("runTime", typeof(TimeSpan?));
            startTime = (DateTime?)info.GetValue("startTime", typeof(DateTime?));
        }

        //internal OldestOpenTransaction
        //    (string command,
        //    string database,
        //    string host,
        //    string program,
        //    int runTime,
        //    string server,
        //    int spid,
        //    DateTime startTime,
        //    string user)
        //{
        //    if (server == null || server.Length == 0) throw new ArgumentNullException("server");

        //    _command = command;
        //    _database = database;
        //    _host = host;
        //    _program = program;
        //    _runTime = runTime;
        //    _server = server;
        //    _spid = spid;
        //    _startTime = startTime;
        //    _user = user;
        //}

        ///// <summary>
        ///// Initializes a new instance of the OldestOpenTransaction class representing a failed
        ///// collection and providing trace information regarding why the failure occurred.
        ///// </summary>
        //internal OldestOpenTransaction(
        //    string server,
        //    string traceInfo)
        //{
        //    if (server == null || server.Length == 0) throw new ArgumentNullException("server");
        //    _server = server;
        //    SetCollectionFailed(traceInfo);
        //}

        #endregion

        #region properties

        /// <summary>
        /// How long the transaction has been running
        /// </summary>
        public TimeSpan? RunTime
        {
            get { return runTime; }
            internal set { runTime = value; }
        }

        public double? TransMinutesBaselineMean
        {
            get { return transMinutesBaseline; }
            internal set { transMinutesBaseline = value; }
        }

        public double? OpenTransMinutesPercDeviationBaseine
        {
            get { return transMinutesPercentage; }
            set { transMinutesPercentage = value; }
        }
        /// <summary>
        /// When this transaction started
        /// </summary>
        public DateTime? StartTime
        {
            get { return startTime; }
            internal set { startTime = value; }
        }

        #endregion

        #region methods


        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public override string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();
        //    dump.Append("Command: " + Command); dump.Append("\n");
        //    dump.Append("Database: " + Database); dump.Append("\n");
        //    dump.Append("Host: " + Host); dump.Append("\n");
        //    dump.Append("Program: " + Program); dump.Append("\n");
        //    dump.Append("RunTime: " + RunTime.ToString()); dump.Append("\n");
        //    dump.Append("Server: " + Server); dump.Append("\n");
        //    dump.Append("Spid: " + Spid.ToString()); dump.Append("\n");
        //    dump.Append("StartTime: " + StartTime.ToString()); dump.Append("\n");
        //    dump.Append("User: " + User); dump.Append("\n");
        //    return dump.ToString();
        //}

        ///// <summary>
        ///// Sets command for the transaction
        ///// </summary>
        //internal void SetCommand(string command)
        //{
        //    _command = command;
        //}
        ///// <summary>
        ///// Sets database for the transaction
        ///// </summary>
        //internal void SetDatabase(string database)
        //{
        //    _database = database;
        //}
        ///// <summary>
        ///// Sets host for the transaction
        ///// </summary>
        //internal void SetHost(string host)
        //{
        //    _host = host;
        //}
        ///// <summary>
        ///// Sets program for the transaction
        ///// </summary>
        //internal void SetProgram(string program)
        //{
        //    _program = program;
        //}
        ///// <summary>
        ///// Sets runTime for the transaction
        ///// </summary>
        //internal void SetRunTime(int runTime)
        //{
        //    _runTime = runTime;
        //}
        ///// <summary>
        ///// Sets spid for the transaction
        ///// </summary>
        //internal void SetSpid(int spid)
        //{
        //    _spid = spid;
        //}
        ///// <summary>
        ///// Sets startTime for the transaction
        ///// </summary>
        //internal void SetStartTime(DateTime startTime)
        //{
        //    _startTime = startTime;
        //}
        ///// <summary>
        ///// Sets user for the transaction
        ///// </summary>
        //internal void SetUser(string user)
        //{
        //    _user = user;
        //}

        #endregion

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("runTime", runTime);
            info.AddValue("startTime", startTime);
        }

    }
}