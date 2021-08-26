//------------------------------------------------------------------------------
// <copyright file="WorkQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Idera.SQLdm.ManagementService.Health;
    using Wintellect.PowerCollections;

    public interface IWorkQueueThreadAllocationStrategy
    {
        bool TerminateWorker(int waitingWorkers, int queueLength);
        bool AllowNewWorker(int waitingWorkers, int activeWorkers, int queueLength);
    }
    
    public delegate void WorkQueueDelegate();
    
    public class DelegateWorkQueue : WorkQueue<WorkQueueDelegate>
    {
        public DelegateWorkQueue(IWorkQueueThreadAllocationStrategy strategy) : base(strategy)
        {
        }
        
        public override void run(WorkQueueDelegate item)
        {
            item.DynamicInvoke();
        }
    }
    
    public abstract class WorkQueue<T>
    {
        private static BBS.TracerX.Logger LOG;

        internal InstrumentedQueue<T> queue;
        private InstrumentedCollection<Thread> waitingWorkers;
        private InstrumentedCollection<Thread> activeWorkers;
        // strategy used to manage the workers
        private IWorkQueueThreadAllocationStrategy strategy;
                
        private bool running;

        static WorkQueue()
        {
            string implName = MethodInfo.GetCurrentMethod().DeclaringType.Name;
            LOG = BBS.TracerX.Logger.GetLogger(implName);
        }

        public WorkQueue(IWorkQueueThreadAllocationStrategy strategy)
        {
            this.strategy = strategy;
            
            queue = new InstrumentedQueue<T>(new Queue<T>(), Statistics.TaskQueueChanged);
            
            waitingWorkers = new InstrumentedCollection<Thread>(new Set<Thread>(),Statistics.SetWaitingWorkers);
            activeWorkers =  new InstrumentedCollection<Thread>(new Set<Thread>(),Statistics.SetActiveWorkers);
        }

        public void Initialize()
        {
            lock (queue)
            {
                while (strategy.AllowNewWorker(waitingWorkers.Count, activeWorkers.Count, queue.Count))
                {
                    CreateWorker();
                }
            }
        }

        protected void CreateWorker()
        {
            Thread thread = new Thread((new ThreadStart(this.run)));
            waitingWorkers.Add(thread);
            thread.Start();
        }
        
        public virtual void Start()
        {
            Initialize();
        }

        public virtual void Stop(bool force)
        {
            running = false;
            lock (queue)
            {
                foreach (Thread thread in Algorithms.ToArray(waitingWorkers))
                {
                    try { thread.Interrupt(); }  catch { /* */ }
                }
            }
        }

        public T Peek()
        {
            lock (queue)
            {
                return queue.Peek();
            }
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
                while (strategy.AllowNewWorker(waitingWorkers.Count, activeWorkers.Count, queue.Count))
                {
                    CreateWorker();
                }                
                Monitor.Pulse(queue);
            }
        }

        public virtual bool IsEmpty()
        {
            lock (queue)
            {
                return queue.Count == 0;
            }
        }

        private void run()
        {
            T item;

            for (running = true; running; )
            {
                lock (queue)
                {
                    while (IsEmpty() && running)
                    {
                        try
                        {
                            Monitor.Wait(queue);
                        }
                        catch (ThreadInterruptedException)
                        {
                            /* ignore */
                        }
                    }
                    if (!running)
                        break;

                    // dequeue a work item from the queue
                    item = queue.Dequeue();
                    
                    ActivateWorker();
                }
                
                Execute(item);
                
                lock (queue)
                {
                    bool exit = DeactivateWorker();
                    
                    if (running && waitingWorkers.Count == 0 && queue.Count > 0)
                    {
                        while (strategy.AllowNewWorker(waitingWorkers.Count, activeWorkers.Count, queue.Count))
                        {
                            CreateWorker();
                        }
                        if (waitingWorkers.Count == 0)
                            LOG.WarnFormat("Work queue backlog is {0}", queue.Count);
                    }
                    
                    if (exit)
                        return;
                }
            }
        }

        void Execute(T item)
        {
            long startTime = 0;
            long endTime = 0;
            
            Statistics.QueryPerformanceCounter(ref startTime);
            try
            {
                run(item);
            }
            catch (Exception e)
            {
                LOG.Error(e.ToString());
            }
            finally
            {
                Statistics.QueryPerformanceCounter(ref endTime);
                Statistics.TaskCompleted(startTime, endTime);
            }
            
        }
                
        protected void ActivateWorker()
        {
            // remove from waiting and add to active
            waitingWorkers.Remove(Thread.CurrentThread);
            activeWorkers.Add(Thread.CurrentThread);
        }
        
        /// <summary>
        /// Remove the thread from the active list and if still needed
        /// add it back to the waiting list. 
        /// </summary>
        /// <returns>true if the worker should be terminated</returns>
        protected bool DeactivateWorker()
        {
            // remove current thread from active list
            activeWorkers.Remove(Thread.CurrentThread);
            // if we no longer need the worker let it end
            if (running && strategy.TerminateWorker(waitingWorkers.Count, queue.Count) == false)
            {
                // still want the worker so add to waiting
                waitingWorkers.Add(Thread.CurrentThread);
                return false;
            }
            return true;
        }
        
        public void Join()
        {
            using (LOG.DebugCall("Join"))
            {
                for (int waitTime = 500; waitTime <= 1000; waitTime += 500)
                {
                    // have to wait for all active threads to terminate
                    foreach (Thread thread in Algorithms.ToArray(activeWorkers))
                    {
                        try
                        {
                            switch (thread.ThreadState)
                            {
                                case ThreadState.Running:
                                case ThreadState.WaitSleepJoin:
                                    if (thread.Join(waitTime))
                                    {
                                        LOG.Debug("Timeout waiting for thread {0} to end. {1} ", thread.Name, waitTime);
                                    }
                                    break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                foreach (Thread thread in Algorithms.ToArray(activeWorkers))
                {
                    LOG.Error("Thread {0} failed to end in a timely manner.", thread.Name);
                }
            }
        }

        public abstract void run(T item);
    }
    
    /// <summary>
    /// Strategy that allows for a single worker thread.
    /// </summary>
    public class SingleWorkerStrategy : IWorkQueueThreadAllocationStrategy
    {
        private bool lazy;
        private bool terminateWorker;
        
        /// <summary>
        /// Strategy that allows for a single worker thread.
        /// </summary>
        /// <param name="lazyCreation">allocates the thread only when needed</param>
        /// <param name="singleUse">terminates the thread after each use</param>
        public SingleWorkerStrategy(bool lazyCreation, bool singleUse)
        {
            this.lazy = lazyCreation;
            this.terminateWorker = singleUse;
        }
        
        public bool TerminateWorker(int waitingWorkers, int queueLength)
        {
            if (!terminateWorker && queueLength > 0)
                return false;

            return lazy || terminateWorker;
        }

        public bool AllowNewWorker(int waitingWorkers, int activeWorkers, int queueLength)
        {
            if (lazy && queueLength == 0)
                return false;
            
            return waitingWorkers == 0 && activeWorkers == 0;
        }
    }

    /// <summary>
    /// Strategy that allows for a single worker thread.
    /// </summary>
    public class MultipleWorkerStrategy : IWorkQueueThreadAllocationStrategy
    {
        private bool lazy;
        private bool singleUse;
        private int  minWaiting;
        private int  maxAlloc;

        /// <summary>
        /// Strategy that allows for a single worker thread.
        /// </summary>
        public MultipleWorkerStrategy(int minimum, int maximum)
        {
            lazy = true;
            singleUse = false;

            minWaiting = minimum;
            maxAlloc = maximum;
        }

        public bool LazyCreation
        {
            get { return lazy; }
            set { lazy = value; }
        }

        public bool SingleUse
        {
            get { return singleUse; }
            set { singleUse = value; }
        }
        
        public bool TerminateWorker(int waitingWorkers, int queueLength)
        {
            return singleUse || waitingWorkers >= minWaiting;
        }

        public bool AllowNewWorker(int waitingWorkers, int activeWorkers, int queueLength)
        {
            if (lazy && queueLength == 0)
                return false;

            if ((waitingWorkers + activeWorkers) < maxAlloc)
            {
                return lazy ? waitingWorkers == 0 : true;
            }
            return false;
        }
    }    
}
