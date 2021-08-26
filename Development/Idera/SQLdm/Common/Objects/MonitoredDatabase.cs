//------------------------------------------------------------------------------
// <copyright file="Database.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a monitored database.
    /// </summary>
    [Serializable]
    public class MonitoredDatabase : MonitoredObject
    {
        #region fields

        private MonitoredSqlServer server;

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        protected MonitoredDatabase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Database"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="name">The name.</param>
        public MonitoredDatabase(MonitoredSqlServer server, string name)
            : base()
        {
            Name = name;
            Server = server;
        }

        public MonitoredDatabase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            server = (MonitoredSqlServer)info.GetValue("server", typeof(MonitoredSqlServer));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public MonitoredSqlServer Server
        {
            get { return server; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("server");
                server = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Toes the URI.
        /// </summary>
        /// <returns></returns>
        public override Uri ToUri()
        {
            Uri serverUri = new Uri(String.Format("sqldm://{0}", Server.ConnectionInfo.InstanceName));
            return new Uri(serverUri, Name);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("server", server);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
