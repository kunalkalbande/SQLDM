//------------------------------------------------------------------------------
// <copyright file="BlockingSession.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a blocking session for the blocking check
    /// </summary>
    [Serializable]
    public sealed class BlockingSession : IFilterableSession
    {
        #region fields

        private int? spid = null;
        private string application = null;
        private string host = null;
        private string login = null;
        private string databasename = null;
        private int? objectId = null;
        private string requestmode = null;
        private TimeSpan blockingTime = new TimeSpan();
        private string inputBuffer = null;
        private DateTime? blockingStartTimeUTC = null;
        private DateTime? blockingLastBatch = null;
        private Guid _BlockID = Guid.Empty;
        private long? _xActID;
        private bool isInterRefresh = false;
        private string resource = "";
        #endregion

        #region constructors

        #endregion

        #region properties

        public string WaitResource
        {
            get { return resource.TrimEnd(); }
            set { resource = value; }
        }

        public bool IsInterRefresh
        {
            get { return isInterRefresh; }
            set { isInterRefresh = value; }
        }

        public long? xActID
        {
            get { return _xActID; }
            set { _xActID = value; }
        }

        public Guid BlockID
        {
            get { return _BlockID; }
            set { _BlockID = value; }
        }

        public int? Spid
        {
            get { return spid; }
            internal set { spid = value; }
        }

        public string Host
        {
            get { return host != null ? host.TrimEnd() : host; }
            internal set { host = value; }
        }

        public string Login
        {
            get { return login != null ? login.TrimEnd() : login; }
            internal set { login = value; }
        }

        public string Application
        {
            get
            {
                return application != null ? application.TrimEnd() : application;
            }
            internal set { application = value; }
        }

        public string Databasename
        {
            get { return databasename; }
            internal set { databasename = value; }
        }

        public int? ObjectId
        {
            get { return objectId; }
            internal set { objectId = value; }
        }

        public string Requestmode
        {
            get { return requestmode; }
            internal set { requestmode = value; }
        }

        public TimeSpan BlockingTime
        {
            get { return blockingTime; }
            internal set { blockingTime = value; }
        }

        public string InputBuffer
        {
            get { return inputBuffer; }
            internal set { inputBuffer = value; }
        }


        public DateTime? BlockingStartTimeUTC
        {
            get { return blockingStartTimeUTC; }
            internal set { blockingStartTimeUTC = value; }
        }

        public DateTime? BlockingLastBatch
        {
            get { return blockingLastBatch; }
            internal set { blockingLastBatch = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
