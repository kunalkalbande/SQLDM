//------------------------------------------------------------------------------
// <copyright file="Server.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a monitored server.
    /// </summary>
    [Serializable]
    public class MonitoredServer : MonitoredObject
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        protected MonitoredServer()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Server"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MonitoredServer(int id, string name, bool enabled)
            : base(id, name, enabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServer"/> class.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        public MonitoredServer(MonitoredServer monitoredServer)
            : base(monitoredServer)
        {
        }

        public MonitoredServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

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
            return new Uri(String.Format("sqldm://{0}", Name));
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
