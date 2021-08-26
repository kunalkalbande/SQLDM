//------------------------------------------------------------------------------
// <copyright file="Table.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
using System.Runtime.Serialization;

    /// <summary>
    /// Represents a monitored table.
    /// </summary>
    [Serializable]
    public class MonitoredTable : MonitoredObject
    {
        #region fields

        private MonitoredDatabase database;

        private string schema;

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        protected MonitoredTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredTable"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="name">The name.</param>
        public MonitoredTable(MonitoredDatabase database, string name)
            : this(database, "dbo", name)
        {
            //TODO: Is this right for versions prior to 2005?
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Table"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public MonitoredTable(MonitoredDatabase database, string schema, string name)
            : base()
        {
            Name = name;
            Database = database;
            Schema = schema;
        }

        public MonitoredTable(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            database = (MonitoredDatabase)info.GetValue("database", typeof(MonitoredDatabase));
            schema = info.GetString("schema");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public MonitoredDatabase Database
        {
            get { return database; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("database");
                database = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public string Schema
        {
            get { return schema; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("schema");
                schema = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> returned from the <see cref="T:Idera.SQLdm.Objects.ObjectNaespaceUtility"></see> class.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}", Schema, Name);
        }

        /// <summary>
        /// Toes the URI.
        /// </summary>
        /// <returns></returns>
        public override Uri ToUri()
        {
            return new Uri(Database.ToUri(), String.Format("{0}.{1}", Schema, Name));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("database", database);
            info.AddValue("schema", schema);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
