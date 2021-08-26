//------------------------------------------------------------------------------
// <copyright file="Database.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the most basic status and naming information on a database
    /// </summary>
    [Serializable]
    public class Database : ICloneable
    {
        private const string DatabaseIsAccessibleText = "The database '{0}' is accessible.";
        private const string DatabaseIsInaccessibleText = "The database '{0}' is inaccessible.";

        #region constants

        #endregion

        #region fields

        private static readonly Logger Logger = Logger.GetLogger("Database");

        private string name = null;
        private DatabaseStatus status = DatabaseStatus.Undetermined;
        private string serverName = null;

        #endregion

        #region constructors

        internal Database(string serverName, string dbName)
        {
            this.name = dbName;
            this.serverName = serverName;
        }

        #endregion

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #region properties

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string Name
        {   
            get { return name; }
            internal set { name = value; }
        }


        public string ServerName
        {
            get { return serverName; }
            internal set { serverName = value; }
        }

        public DatabaseStatus Status
        {
            get { return status; }
            internal set { status = value; }
        }
        
        /// <summary>
        /// Note that this will NOT return Single User as an inaccessible type even if we could not, in fact, access the database
        /// Such a check should be done another way
        /// </summary>
        public bool IsAccessibleStatus
        {
            get {
                bool result =
                    ((status & DatabaseStatus.RestoringMirror) != DatabaseStatus.RestoringMirror)
                    && ((status & DatabaseStatus.Loading) != DatabaseStatus.Loading)
                    && ((status & DatabaseStatus.PreRecovery) != DatabaseStatus.PreRecovery)
                    && ((status & DatabaseStatus.Recovering) != DatabaseStatus.Recovering)
                    && ((status & DatabaseStatus.Suspect) != DatabaseStatus.Suspect)
                    && ((status & DatabaseStatus.Offline) != DatabaseStatus.Offline)
                    && ((status & DatabaseStatus.Inaccessible) != DatabaseStatus.Inaccessible)
                    && ((status & DatabaseStatus.Undetermined) != DatabaseStatus.Undetermined);
                   

                String logText = result ? DatabaseIsAccessibleText : DatabaseIsInaccessibleText;
                Logger.Debug(String.Format(logText, Name));

                return result;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods


        public static bool MatchStatus(DatabaseStatus status, DatabaseStatus matchStatus)
        {
            return (matchStatus & status) == matchStatus;
        }

        #endregion

        #region interface implementations
        
        #endregion

        #region nested types

        #endregion

    }

    public class DatabaseStatusDisplay : IComparable
    {
        private DatabaseStatus databaseStatus = DatabaseStatus.Undetermined;

        public DatabaseStatusDisplay(DatabaseStatus databaseStatus)
        {
            this.databaseStatus = databaseStatus;
        }

        public DatabaseStatus Status
        {
            get { return databaseStatus; }
            set { databaseStatus = value; }
        }

        public override string ToString()
        {
            List<DatabaseStatus> list = new List<DatabaseStatus>();

            // this just separates the combined value into a list individual values for 
            // easier string processing 
            foreach (int val in Enum.GetValues(typeof(DatabaseStatus)))
            {
                if (((DatabaseStatus)val & databaseStatus) == (DatabaseStatus)val)
                {
                    list.Add((DatabaseStatus)val);
                }
            }
            return (GetStringFromDBStatus(list));
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            DatabaseStatusDisplay other = rhs as DatabaseStatusDisplay;
            if (other == null) return 1;

            if (other.Status > databaseStatus) return -1;
            else if (other.Status < databaseStatus) return 1;
            else return 0;
        }

        private string GetStringFromDBStatus(List<DatabaseStatus> statusList)
        {
            StringBuilder statusDisplay = new StringBuilder();

            if (statusList.Count > 0)
            {
                // 0 is status Normal and is only a valid status when it is the only value in the status.
                if (statusList.Count == 1)
                {
                    statusDisplay = statusDisplay.Append(GetEnumDescription(statusList[0]));
                }
                else
                {
                    for (int i = 1; i < statusList.Count; i++)
                    {
                        if (i > 1)
                        {
                            statusDisplay.Append(", ");
                        }
                        statusDisplay = statusDisplay.Append(GetEnumDescription(statusList[i]));
                    }
                }
            }
            return (statusDisplay.ToString());
        }

        private string GetEnumDescription(object o)
        {
            System.Type otype = o.GetType();
            if (otype.IsEnum)
            {
                FieldInfo field = otype.GetField(Enum.GetName(otype, o));
                if (field != null)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return o.ToString();
        }
    }
}
