//------------------------------------------------------------------------------
// <copyright file="SessionDetailSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Snapshot representing the Session Detail view
    /// </summary>
    [Serializable]
    public sealed class SessionDetailSnapshot : Snapshot, ISerializable
    {
        #region fields

        private SessionDetail details = new SessionDetail();
        private List<TraceStatement> traceItems = new List<TraceStatement>();

        #endregion

        #region constructors

        public SessionDetailSnapshot(SqlConnectionInfo info) : base(info.InstanceName)
        {
        }

        public SessionDetailSnapshot(SerializationInfo info, StreamingContext context)
        {
            SetObjectData(info, context);
            details = (SessionDetail)info.GetValue("details", typeof(SessionDetail));
            traceItems = (List<TraceStatement>)info.GetValue("traceItems", typeof(List<TraceStatement>));
        }

        #endregion

        #region properties

        /// <summary>
        /// The detailed information on the selected session
        /// </summary>
        public SessionDetail Details
        {
            get { return details; }
            internal set { details = value; }
        }

        /// <summary>
        /// The list of items from the session trace
        /// </summary>
        public List<TraceStatement> TraceItems
        {
            get { return traceItems; }
            internal set { traceItems = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
            info.AddValue("details", details);
            info.AddValue("traceItems", traceItems);
        }

        #endregion

        #region nested types

        #endregion

    }
}
