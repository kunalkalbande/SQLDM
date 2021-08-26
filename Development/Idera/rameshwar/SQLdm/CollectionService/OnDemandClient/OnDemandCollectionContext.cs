using System;
using System.Diagnostics;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Messages;
using Idera.SQLdm.Common.Services;
using System.Collections.Generic;
using Idera.SQLdm.CollectionService.Configuration;

namespace Idera.SQLdm.CollectionService.OnDemandClient
{
    /// <summary>
    /// SQLdm 10.0 (Vineet Kumar) -- Added for executing Nunit test cases on collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OnDemandCollectionContext<T> :  ISnapshotSink
    {
        // SQLdm 10.1.3 (Varun Chopra) SQLDM-19247: Timeout When Viewing Large Databases Under Tables & Indexes - Configurable Collection Time
        private static readonly TimeSpan DEFAULT_WAITTIME = TimeSpan.FromSeconds(CollectionServiceConfiguration.GetCollectionServiceElement().CollectionWaitTimeInSeconds);

        private bool cancelled = false;
        private Serialized<T> snap = null;
        private ManualResetEvent readyEvent = new ManualResetEvent(false);
        private Stopwatch stopwatch = new Stopwatch();
        private TimeSpan waitTime = DEFAULT_WAITTIME;
        private List<Serialized<T>> listSnap = null;//SQLdm 10.0 Vineet -- Added to support multiple probes
        
        public OnDemandCollectionContext()
        {
        }

        public OnDemandCollectionContext(TimeSpan waitTime)
        {
            this.waitTime = waitTime;
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
        /// SQLdm 10.0 Vineet -- Added to support multiple probes
        /// </summary>
        /// <returns></returns>
        public List<Serialized<T>> WaitMultiple()
        {
            stopwatch.Start();
            for (TimeSpan timeout = waitTime; waitTime > stopwatch.Elapsed; timeout = waitTime - stopwatch.Elapsed)
            {
                if (readyEvent.WaitOne(timeout, false))
                {
                    stopwatch.Stop();
                    return listSnap;
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
                try
                {
                    this.snap = (Serialized<T>)snapshot;
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
           
            return Guid.NewGuid();
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
