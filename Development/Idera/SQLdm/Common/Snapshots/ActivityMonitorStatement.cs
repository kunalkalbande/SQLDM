//------------------------------------------------------------------------------
// <copyright file="QueryMonitorStatement.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Idera.SQLdm.Common.Snapshots;


namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// A single query monitor statement
    /// </summary>
    [Serializable]
    public class ActivityMonitorStatement : TraceStatement, ISerializable
    {
        #region fields
        private readonly QueryMonitorStatement statement = new QueryMonitorStatement();

        private int objectID;


        #endregion

        #region constructors

        public ActivityMonitorStatement() { }

        protected ActivityMonitorStatement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            statement.Database = info.GetString("database");
            statement.NtUser = info.GetString("ntUser");
            statement.SqlUser = info.GetString("sqlUser");
            statement.Client = info.GetString("client");
            statement.AppName = info.GetString("appName");
            objectID = info.GetInt32("objectID");
            statement.StatementType = (WorstPerformingStatementType?)info.GetValue("statementType", typeof(WorstPerformingStatementType?));
            statement.SignatureHash = info.GetString("signatureHash");
        }

        #endregion

        #region properties

        public QueryMonitorStatement QueryMonitorStatement
        {
            get { return statement; }
        }

        /// <summary>
        /// The database specified by the USE database statement, or the default database if no USE database statement 
        /// is issued for a given connection.  The statement may actually operate on another database.
        /// </summary>
        public string Database
        {
            get { return statement.Database; }
            internal set { statement.Database = value; }
        }
        
        /// <summary>
        /// object if of the object on whose activity the statement relates
        /// </summary>
        public int ObjectID
        {
            get { return objectID; }
            set { objectID = value; }
        }

        /// <summary>
        /// Microsoft Windows user name
        /// </summary>
        public string NtUser
        {
            get { return statement.NtUser; }
            internal set { statement.NtUser = value; }
        }

        /// <summary>
        /// The SQL Server user name
        /// </summary>
        public string SqlUser
        {
            get { return statement.SqlUser; }
            internal set { statement.SqlUser = value; }
        }

        /// <summary>
        /// The hostname of the attached client
        /// </summary>
        public string Client
        {
            get { return statement.Client; }
            internal set { statement.Client = value; }
        }

        /// <summary>
        /// The application name which executed the statement
        /// </summary>
        public string AppName
        {
            get { return statement.AppName; }
            internal set { statement.AppName = value; }
        }

        /// <summary>
        /// The event type - this is a special roll-up of EventType for Query Monitor only
        /// </summary>
        public WorstPerformingStatementType? StatementType
        {
            get { return statement.StatementType; }
            internal set { statement.StatementType = value; }
        }


        public string SignatureHash
        {
            get { return statement.SignatureHash; }
            internal set { statement.SignatureHash = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods




        #endregion

        #region interface implementations

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("database", statement.Database);
            info.AddValue("ntUser", statement.NtUser);
            info.AddValue("sqlUser", statement.SqlUser);
            info.AddValue("client", statement.Client);
            info.AddValue("appName", statement.AppName);
            info.AddValue("objectID", objectID);
            info.AddValue("statementType", statement.StatementType);
            info.AddValue("signatureHash", statement.SignatureHash);
        }

        #endregion

        #region nested types

        #endregion

    }
}
