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
    public class QueryMonitorStatement : TraceStatement, ISerializable
    {
        #region fields

        private string database = null;
        private string ntUser = null;
        private string sqlUser = null;
        private string client = null;
        private string appName = null;
        private WorstPerformingStatementType? statementType = null;
        private string signatureHash = null;
        private string queryPlan = null; //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new field for query Plan  
        private Boolean isActualPlan = true; //SQLdm 10.0 (Tarun Sapra) : Flag to tell if the actual plan is available or not        

        #endregion

        #region constructors

        public QueryMonitorStatement() {}

        protected QueryMonitorStatement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            database = info.GetString("database");
            ntUser = info.GetString("ntUser");
            sqlUser = info.GetString("sqlUser");
            client = info.GetString("client");
            appName = info.GetString("appName");
            statementType = (WorstPerformingStatementType?) info.GetValue("statementType", typeof (WorstPerformingStatementType?));
            signatureHash = info.GetString("signatureHash");
            queryPlan=info.GetString("queryPlan"); //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new field for query Plan    
            isActualPlan = info.GetBoolean("isActualPlan"); //SQLdm 10.0 (Tarun Sapra) : Flag to tell if the actual plan is there in the queryPlan
        }

        #endregion

        #region properties

		//SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new Property for query Plan
        public string QueryPlan
        {
            get { return queryPlan; }
            internal set { queryPlan = value; }
        }
        
        /// <summary>
        /// The database specified by the USE database statement, or the default database if no USE database statement 
        /// is issued for a given connection.  The statement may actually operate on another database.
        /// </summary>
        public string Database
        {
            get { return database; }
            internal set { database = value; }
        }

        //START SQLdm 10.0 (Tarun Sapra) 
        public Boolean IsActualPlan
        {
            get { return isActualPlan; }
            internal set { isActualPlan = value; }
        }
        //END SQLdm 10.0 (Tarun Sapra) 
        
        /// <summary>
        /// Microsoft Windows user name
        /// </summary>
        public string NtUser
        {
            get { return ntUser; }
            internal set { ntUser = value; }
        }

        /// <summary>
        /// The SQL Server user name
        /// </summary>
        public string SqlUser
        {
            get { return sqlUser; }
            internal set { sqlUser = value; }
        }

        /// <summary>
        /// The hostname of the attached client
        /// </summary>
        public string Client
        {
            get { return client; }
            internal set { client = value; }
        }

        /// <summary>
        /// The application name which executed the statement
        /// </summary>
        public string AppName
        {
            get { return appName; }
            internal set { appName = value; }
        }

        /// <summary>
        /// The event type - this is a special roll-up of EventType for Query Monitor only
        /// </summary>
        public WorstPerformingStatementType? StatementType
        {
            get { return statementType; }
            internal set { statementType = value; }
        }


        public string SignatureHash
        {
            get { return signatureHash; }
            internal set { signatureHash = value; }
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
            info.AddValue("database", database);
            info.AddValue("ntUser", ntUser);
            info.AddValue("sqlUser", sqlUser);
            info.AddValue("client", client);
            info.AddValue("appName", appName);
            info.AddValue("statementType", statementType);
            info.AddValue("signatureHash", signatureHash);
            info.AddValue("queryPlan", queryPlan); //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new field for query Plan
            info.AddValue("isActualPlan", isActualPlan); //SQLdm 10.0 (Tarun Sapra) - Flag for displaying actual query plan
        }

        #endregion

        #region nested types

        #endregion

    }
}
