//------------------------------------------------------------------------------
// <copyright file="TaskScheduler.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using BBS.TracerX;
    using Wintellect.PowerCollections;

    public interface IScheduledTask 
    {
        DateTime RunTime { get; set; }
        void Run(Object arg);
    }

    [Serializable]
    public class TaskScheduler : ISerializable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("TaskScheduler");

        private ReaderWriterLock sync;
        private OrderedBag<IScheduledTask> taskList;
        private TimerCallback timerPopped;
        private Timer timer;

        private DateTime nextPop;

        public TaskScheduler()
        {
            sync = new ReaderWriterLock();

            // create list of tasks to run ordered by the number
            // of millis until its next runtime.
            taskList = new OrderedBag<IScheduledTask>(CompareScheduledTask);

            // set method to execute when the timer pops
            timerPopped = TimerMethod;
            // create a disabled timer
            timer = new Timer(timerPopped, this, -1, -1);

            nextPop = DateTime.MaxValue;

            LOG.Debug("TaskScheduler created");
        }

        protected TaskScheduler(SerializationInfo info, StreamingContext context)
        {
            sync = new ReaderWriterLock();
            taskList = info.GetValue("taskList", typeof (OrderedBag<IScheduledTask>)) as OrderedBag<IScheduledTask>;
            timerPopped = TimerMethod;
            nextPop = DateTime.MaxValue;

            LOG.Debug("TaskScheduler deserialized");

            if (taskList.Count > 0)
                updateTimer();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("taskList", taskList);
        }

        // delegate used for sorting items in the task list
        private static int CompareScheduledTask(IScheduledTask x, IScheduledTask y)
        {
            return y.RunTime.CompareTo(x.RunTime);
        }

        public void Stop()
        {
            throw new NotImplementedException("Stopping the task scheduler not implemented yet.");
        }

        public void addTask(IScheduledTask workItem)
        {
            sync.AcquireWriterLock(-1);
            try
            {
                internalAdd(workItem);
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
        }

        public IEnumerable<IScheduledTask> FindAllTasks(Predicate<IScheduledTask> predicate)
        {
            IEnumerable<IScheduledTask> result = null;
            sync.AcquireReaderLock(-1);
            try
            {
                result = taskList.FindAll(predicate);
            }
            finally
            {
                sync.ReleaseReaderLock();
            }

            return result;
        }

        public IScheduledTask FindTask(Predicate<IScheduledTask> predicate)
        {
            IScheduledTask result = null;
            sync.AcquireReaderLock(-1);
            try
            {
                foreach (IScheduledTask task in taskList)
                {
                    if (predicate.Invoke(task))
                    {
                        result = task;
                        break;
                    }
                }
            }
            finally
            {
                sync.ReleaseReaderLock();
            }

            return result;
        }

        /// <summary>
        /// Add the item to the task list and update the timer
        /// </summary>
        /// <param name="workItem"></param>
        protected void internalAdd(IScheduledTask workItem)
        {
            LOG.Debug("TaskScheduler.internalAdd enter");

            // when should this item run 
            DateTime popTime = workItem.RunTime;

            LOG.DebugFormat("popTime={0}", popTime);

            // add the item to the task list
            taskList.Add(workItem);

            // if this item should run first then reset the timer
            if (nextPop == DateTime.MaxValue || nextPop.CompareTo(popTime) > 0)
            {
                updateTimer();
            }
            LOG.Debug("TaskScheduler.internalAdd exit");
        }

        private void updateTimer()
        {
            DateTime popTime = taskList.GetFirst().RunTime;
            long millis = (popTime.Ticks - DateTime.Now.Ticks) / 10000;
            if (millis < 1)
                millis = 1;

            LOG.DebugFormat("Setting timer to pop in {0} millis", millis);

            timer.Change(millis, 0);

            nextPop = popTime;
        }

        protected void handleTimerPopped()
        {
            sync.AcquireWriterLock(-1);
            try
            {
                while (taskList.Count > 0)
                {
                    IScheduledTask task = taskList.GetFirst();

                    if (task.RunTime > DateTime.Now)
                        break;

                    scheduleTask(taskList.RemoveFirst());
                }
                // force timer update
                updateTimer();
            }
            finally
            {
                sync.ReleaseWriterLock();
            }
        }

        private void scheduleTask(IScheduledTask task)
        {

            // send to the cloned workitem to the threadpool for execution
            ThreadPool.QueueUserWorkItem(new WaitCallback(task.Run));

        }


        public static void TimerMethod(Object state)
        {
            LOG.Debug("Timer Popped");
            TaskScheduler task = state as TaskScheduler;
            if (task != null)
                task.handleTimerPopped();
        }

    }
}
