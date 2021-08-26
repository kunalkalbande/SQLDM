//------------------------------------------------------------------------------
// <copyright file="OnDemandCollectionContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting.Lifetime;
    using System.Threading;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Services;
    using System.Collections.Generic;
    using Idera.SQLdm.ManagementService.Configuration;

    /// <summary>
    /// This class is used for on-demand collection.  When a client requests on-demand data from the
    /// management service, it creates a new instance of this class and passes it to the collection service.
    /// As data is returned to the management service, it is forwarded back to the client.
    /// </summary>
    public class OnDemandCollectionContext<T> : MarshalByRefObject, ISnapshotSink, IDisposable, ISponsor
    {
        private static readonly TimeSpan DEFAULT_WAITTIME = TimeSpan.FromSeconds(ManagementServiceConfiguration.GetManagementServiceElement().CollectionWaitTimeInSeconds);

        private bool disposed = false;
        private bool cancelled = false;
        private Serialized<T> snap = null;
        private ManualResetEvent readyEvent = new ManualResetEvent(false);
        private Stopwatch stopwatch = new Stopwatch();
        private TimeSpan waitTime = DEFAULT_WAITTIME;
        private List<Serialized<T>> listSnap = null;//SQLdm 10.0 Vineet -- Added to support multiple probes

        private Guid sessionId;

        public OnDemandCollectionContext()
        {
        }

        public OnDemandCollectionContext(TimeSpan waitTime)
        {
            this.waitTime = waitTime;
        }

        /// <summary>
        /// Sets the session id of this context in order to support cancelling the request.
        /// </summary>
        /// <param name="sessionId"></param>
        public void SetSessionId(Guid sessionId)
        {
            this.sessionId = sessionId;
            Dictionary<Guid, ISnapshotSink> sinks = Management.SnapshotSinks;
            ISnapshotSink sink = null;
            lock (sinks)
            {
                // cancel an existing request with the same id
                if (sinks.TryGetValue(sessionId, out sink))
                {
                    sink.Cancelled = true;
                    sinks.Remove(sessionId);
                }
                // keep track of this session 
                sinks.Add(sessionId, this);   
            }
        }

        /// <summary>
        /// Initialize the lifetime of this object when used remotely.
        /// A reference to this object is passed to the collection service and is initially 
        /// configured to live for the UI wait time plus 15 seconds.  Once the collection
        /// service has delivered the snapshot the object lease is expired so the object 
        /// will get garbage collected.  
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease) base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = waitTime + TimeSpan.FromSeconds(15);
                lease.RenewOnCallTime = TimeSpan.Zero;
            }

            lease.Register(this);

            return lease;
        }

        TimeSpan ISponsor.Renewal(ILease lease)
        {
            lock(readyEvent)
            {
                if (disposed)
                    return TimeSpan.Zero;
                
                TimeSpan elapsed = stopwatch.Elapsed;
                TimeSpan ttl = waitTime + TimeSpan.FromSeconds(15);
                return elapsed > ttl ? TimeSpan.Zero : ttl - elapsed;
            }
        }

        public void Dispose()
        {
            lock (readyEvent)
            {
                if (!disposed)
                {
                    disposed = true;
                    snap = null;

                    // make sure the event handle gets closed
                    readyEvent.Close();
                   
                    // unregister will cause the lease to expire and cause the 
                    // object to get garbage collected.
                    ILease lease = (ILease) GetLifetimeService();
                    lease.Unregister(this);

                    if (sessionId != default(Guid))
                    {
                        Dictionary<Guid, ISnapshotSink> sinks = Management.SnapshotSinks;
                        ISnapshotSink sink = null;
                        lock (sinks)
                        {
                            if (sinks.TryGetValue(sessionId, out sink))
                            {
                                // only remove ourselves from the map
                                if (sink == this)
                                    sinks.Remove(sessionId);
                            }
                        }
                    }
                }
            }
        }

        public static ISnapshotSink GetSink(Guid sessionId)
        {
            ISnapshotSink sink = null;
            if (sessionId != default(Guid))
            {
                Dictionary<Guid, ISnapshotSink> sinks = Management.SnapshotSinks;
                lock (sinks)
                {
                    sinks.TryGetValue(sessionId, out sink);
                }
            }
            return sink;
        }

        /// <summary>
        /// SQLdm 10.0 Vineet -- Added to support multiple probes
        /// </summary>
        /// <returns></returns>
        public Serialized<List<T>> WaitMultiple()
        {
            //stopwatch.Start();
            //for (TimeSpan timeout = waitTime; waitTime > stopwatch.Elapsed; timeout = waitTime - stopwatch.Elapsed)
            //{
            //    if (readyEvent.WaitOne(timeout, false))
            //    {
            //        stopwatch.Stop();
            //        return listSnap;
            //    }
            //}
            throw new ServiceException(Status.ErrorRequestTimeout, "");
        }

        public Serialized<T> Wait()
        {
            stopwatch.Start();

            for (TimeSpan timeout = waitTime; waitTime > stopwatch.Elapsed; timeout = waitTime - stopwatch.Elapsed)
            {
                if (readyEvent.WaitOne(timeout, false))
                {
                    stopwatch.Stop();
                    return snap;
                }
            }

            throw new ServiceException(Status.ErrorRequestTimeout, "");
        }
        
        /// <summary>
        /// Callback from the Collection Service with the results of a client on-demand request.
        /// </summary>
        /// <param name="snapshot">The collected snapshot</param>
        /// <param name="state">Object passed to the collection service for correlation</param>
        public void Process(ISerialized snapshot, object state)
        {
            if (!disposed)
            {
                try
                {
                    this.snap = (Serialized<T>) snapshot;
                }
                finally
                {
                    // make sure to signal that the request is complete
                    lock (readyEvent)
                    {
                        if (!readyEvent.SafeWaitHandle.IsClosed)
                            readyEvent.Set();
                    }
                }
            }
        }

        #region ISnapshotSink Members

        public TimeSpan ManagementServiceWaitTime
        {
            get
            {
                return waitTime;     
            }
            set
            {
                waitTime = value;
            }
        }

        public bool Cancelled
        {
            get
            {
                return cancelled;
            }
            set
            {
                cancelled = value;
            }
        }

        /// <summary>
        /// Registers the sink so that it can be retrieved again using its id.
        /// </summary>
        /// <returns></returns>
        public Guid RegisterSink()
        {
            if (sessionId == Guid.Empty)
            {
                sessionId = Guid.NewGuid();
                SetSessionId(sessionId);
            }

            return sessionId;
        }

        #endregion

        /// <summary>
        /// SQLdm 10.0 Vineet -- Added to support multiple probes
        /// </summary>
        /// <param name="listSnapshot"></param>
        /// <param name="state"></param>
        public void ProcessMultiple(List<ISerialized> snapshot, object state)
        {
            try
            {
                this.listSnap = new List<Serialized<T>>();
                foreach (ISerialized xy in snapshot)
                {
                    var snp = (Serialized<T>)xy;
                    listSnap.Add(snp);
                }

            }
            finally
            {
                // make sure to signal that the request is complete
                lock (readyEvent)
                {
                    if (!readyEvent.SafeWaitHandle.IsClosed)
                        readyEvent.Set();
                }
            }
        }
    }
}
