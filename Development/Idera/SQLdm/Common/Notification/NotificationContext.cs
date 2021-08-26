//------------------------------------------------------------------------------
// <copyright file="NotificationContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public class NotificationContext
    {
        private readonly IEvent Event;
        private readonly NotificationRule rule;
        private readonly AlertableSnapshot refresh;
        private NotificationDestinationInfo destination;

        private int sendAttempts;
        public DateTime lastSendAttempt;
        public Exception lastSendException;

        public NotificationContext(IEvent ievent, NotificationDestinationInfo destination, NotificationRule rule, AlertableSnapshot refresh) 
        {
            this.Event = ievent;
            this.Destination = destination;
            this.rule = rule;
            this.refresh = refresh;

            this.LastSendAttempt = DateTime.MinValue;
        }

        public int SendAttempts
        {
            get { return sendAttempts; }
            set { sendAttempts = value; }
        }

        public DateTime LastSendAttempt
        {
            get { return lastSendAttempt; }
            internal set { lastSendAttempt = value; }
        }

        public Exception LastSendException
        {
            get { return lastSendException; }
            set { lastSendException = value; }
        }

        public NotificationDestinationInfo Destination
        {
            get { return destination; }
            internal set { destination = value; }
        }

        public AlertableSnapshot Refresh
        {
            get { return refresh; }
        }

        public NotificationRule Rule
        {
            get { return rule; }
        }

        public IEvent SourceEvent
        {
            get { return Event; }
        }
    }
}
