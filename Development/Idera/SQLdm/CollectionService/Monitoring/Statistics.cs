//------------------------------------------------------------------------------
// <copyright file="Statistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Configuration;

    public static class Statistics
    {
        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);

        private static readonly BBS.TracerX.Logger LOG;

        public static string PERFCOUNTER_CATEGORY;

        private const string PERFORMANCE_COUNTER_TOTAL_MONIKER = "_Total";

        public static PerformanceCounterCategory category;

        public static PerformanceCounter activeWorkers;

        public static PerformanceCounter waitingWorkers;

        public static PerformanceCounter taskQueueLength;
        public static PerformanceCounter tasksQueuedPerSecond;

        public static PerformanceCounter avgTaskTime;
        public static PerformanceCounter avgTaskTimeBase;

        //START SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
        public static string PERFCOUNTER_CATEGORY_PERINSTANCE;
        public static PerformanceCounterCategory categoryPerInstance;

        public static PerformanceCounter monitoredServers;
        public static PerformanceCounter maintainenceModeServers;
        public static PerformanceCounter queuedScheduledRefreshes;
        public static PerformanceCounter droppedScheduledRefreshes;
        public static PerformanceCounter alertsRaised;
        public static PerformanceCounter failedCollections;
        public static PerformanceCounter databases;
        public static PerformanceCounter queryMonitorStatements;
        public static PerformanceCounter waitStatisticStatements;
        public static PerformanceCounter collectionRunTime;
        //public static long collectionRunTimeInSeconds = 0;

        public static PerformanceCounter InitializeCounter(PerformanceCounterCategory perfCategory, string counterName, string counterInstanceName)
        {
            var pc = new PerformanceCounter(perfCategory.CategoryName, counterName, counterInstanceName, false);
            pc.MachineName = ".";
            pc.RawValue = 0;
            return pc;
        }
        //END SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters

        static Statistics()
        {
            LOG = BBS.TracerX.Logger.GetLogger("Statistics");

            var publishPerformanceCounters = CollectionServiceConfiguration.GetCollectionServiceElement().PublishPerformanceCounters;

            PERFCOUNTER_CATEGORY = CollectionServiceInstaller.BASE_SERVICE_NAME +
                                    "$" +
                                    CollectionServiceConfiguration.InstanceName;

            PERFCOUNTER_CATEGORY_PERINSTANCE = CollectionServiceInstaller.BASE_SERVICE_NAME +
                                        "$" +
                                        CollectionServiceConfiguration.InstanceName +
                                        "$" + "PERINSTANCE";

            if (!publishPerformanceCounters)
            {
                //SQLDM-29997
                try
                {
                    if (PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY))
                    {
                        PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY);
                    }
                    if (PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY_PERINSTANCE))
                    {
                        PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY_PERINSTANCE);
                    }
                }
                catch(Exception e)
                {
                    LOG.Error("The performance counters on the machine might be corrupted :", e);
                }
                // SQLDM-28034: 'return' to avoid creating and publishing Performance Counters and Categories for Collection Service                    
                return;
            }

            try
            {
                //Performance Counter Category for Counters having only one instance '_Total'
                if (PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY))
                    PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY);

                    CounterCreationDataCollection counterData = new CounterCreationDataCollection();

                    counterData.AddRange(new System.Diagnostics.CounterCreationData[]
                                             {
                                                 new CounterCreationData("ActiveWorkers",
                                                                         "Number of threads processing queued work",
                                                                         PerformanceCounterType.NumberOfItems32),
                                                 new CounterCreationData("WaitingWorkers",
                                                                         "Number of threads waiting for work to be queued",
                                                                         PerformanceCounterType.NumberOfItems32),
                                                 new CounterCreationData("Task Queue Length",
                                                                         "Number of tasks waiting for execution",
                                                                         PerformanceCounterType.NumberOfItems32),
                                                 new CounterCreationData("Tasks Queued/sec",
                                                                         "Average number of tasks queued per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Avg. Task Time",
                                                                         "Average time to process a queued task",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Avg. Task Time Base",
                                                                         "Base counter for Avg. Task Time",
                                                                         PerformanceCounterType.AverageBase),
                                                 
                                                 //START SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
                                                 new CounterCreationData("# Monitored Servers", "Number of monitored servers", 
                                                                          PerformanceCounterType.NumberOfItems64),
                                                 new CounterCreationData("# Servers in Maintenance Mode", "Number of servers in maintenance mode", 
                                                                         PerformanceCounterType.NumberOfItems64),
                                             });

                    category = PerformanceCounterCategory.Create(PERFCOUNTER_CATEGORY, "",
                                                                 PerformanceCounterCategoryType.SingleInstance,
                                                                 counterData);
                //}
                //else
                //    category = new PerformanceCounterCategory(PERFCOUNTER_CATEGORY);


                //Performance Counter Category for Counters having multiple instances, one per monitored server
                try
                {
                    if (PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY_PERINSTANCE))
                        PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY_PERINSTANCE);
                }
                catch(Exception e)
                {
                    LOG.Error("The performance counters on the machine might be corrupted :", e);
                }

                CounterCreationDataCollection counterCollection = new CounterCreationDataCollection();

                    counterCollection.AddRange(new System.Diagnostics.CounterCreationData[]
                                            {                                             
                                                new CounterCreationData("# Queued Scheduled Refreshes",
                                                                         "Number of queued scheduled refreshes",
                                                                         PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("# Dropped Scheduled Refreshes",
                                                                         "Number of scheduled refreshes dropped without pickup",
                                                                         PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("# Alerts Raised", "Number of alerts raised", 
                                                                          PerformanceCounterType.NumberOfItems32),
                                                
                                                 new CounterCreationData("# Failed Collections", "Number of failed collections", 
                                                                          PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("# Databases", "Number of databases", 
                                                                          PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("# Query Monitor Statements", "Number of query monitor statements", 
                                                                          PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("# Wait Statistic Statements", "Number of wait statistic statements", 
                                                                          PerformanceCounterType.NumberOfItems32),

                                                 new CounterCreationData("Collection Run Time", "Collection run time in seconds", 
                                                                          PerformanceCounterType.ElapsedTime)      
                                                //END SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
                                        });

                    categoryPerInstance = PerformanceCounterCategory.Create(PERFCOUNTER_CATEGORY_PERINSTANCE, "",
                                                                 PerformanceCounterCategoryType.MultiInstance,
                                                                 counterCollection);
                //}
                //else
                //    newCategory = new PerformanceCounterCategory(PERFCOUNTER_CATEGORY_NEW);

                activeWorkers = new PerformanceCounter();
                activeWorkers.CategoryName = category.CategoryName;
                activeWorkers.CounterName = "ActiveWorkers";
                activeWorkers.MachineName = ".";
                activeWorkers.ReadOnly = false;
                activeWorkers.RawValue = 0;

                waitingWorkers = new PerformanceCounter();
                waitingWorkers.CategoryName = category.CategoryName;
                waitingWorkers.CounterName = "WaitingWorkers";
                waitingWorkers.MachineName = ".";
                waitingWorkers.ReadOnly = false;
                waitingWorkers.RawValue = 0;

                taskQueueLength = new PerformanceCounter();
                taskQueueLength.CategoryName = category.CategoryName;
                taskQueueLength.CounterName = "Task Queue Length";
                taskQueueLength.MachineName = ".";
                taskQueueLength.ReadOnly = false;
                taskQueueLength.RawValue = 0;

                tasksQueuedPerSecond = new PerformanceCounter();
                tasksQueuedPerSecond.CategoryName = category.CategoryName;
                tasksQueuedPerSecond.CounterName = "Tasks Queued/sec";
                tasksQueuedPerSecond.MachineName = ".";
                tasksQueuedPerSecond.ReadOnly = false;
                tasksQueuedPerSecond.RawValue = 0;

                avgTaskTime = new PerformanceCounter();
                avgTaskTime.CategoryName = category.CategoryName;
                avgTaskTime.CounterName = "Avg. Task Time";
                avgTaskTime.MachineName = ".";
                avgTaskTime.ReadOnly = false;
                avgTaskTime.RawValue = 0;

                avgTaskTimeBase = new PerformanceCounter();
                avgTaskTimeBase.CategoryName = category.CategoryName;
                avgTaskTimeBase.CounterName = "Avg. Task Time Base";
                avgTaskTimeBase.MachineName = ".";
                avgTaskTimeBase.ReadOnly = false;
                avgTaskTimeBase.RawValue = 0;

                //START SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
                monitoredServers = new PerformanceCounter(category.CategoryName, "# Monitored Servers", false);
                monitoredServers.RawValue = 0;
                monitoredServers.MachineName = ".";

                maintainenceModeServers = new PerformanceCounter(category.CategoryName, "# Servers in Maintainence Mode", false);
                maintainenceModeServers.RawValue = 0;
                maintainenceModeServers.MachineName = ".";

                queuedScheduledRefreshes = InitializeCounter(categoryPerInstance, "# Queued Scheduled Refreshes", "_Total");
                droppedScheduledRefreshes = InitializeCounter(categoryPerInstance, "# Dropped Scheduled Refreshes", "_Total");
                alertsRaised = InitializeCounter(categoryPerInstance, "# Alerts Raised", "_Total");
                failedCollections = InitializeCounter(categoryPerInstance, "# Failed Collections", "_Total");
                databases = InitializeCounter(categoryPerInstance, "# Databases", "_Total");
                queryMonitorStatements = InitializeCounter(categoryPerInstance, "# Query Monitor Statements", "_Total");
                waitStatisticStatements = InitializeCounter(categoryPerInstance, "# Wait Statistic Statements", "_Total");
                collectionRunTime = InitializeCounter(categoryPerInstance, "Collection Run Time", "_Total");
                //END SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters

            }
            catch (Exception e)
            {
                LOG.Error("Error creating and registering performance counters: ", e);
            }
        }

        public static void SetActiveWorkers(int count)
        {
            if (activeWorkers != null)
            {
                try
                {
                    activeWorkers.RawValue = count;

                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("SetActiveWorkers Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
					activeWorkers.Close();
					activeWorkers.Dispose();				
                }
				catch (Exception e)
				{
					LOG.ErrorFormat("Error(Exception) updating Active Workers counter: {0}", e);
					activeWorkers.Close();
					activeWorkers.Dispose();				
					
				}
            }
        }

        public static void SetWaitingWorkers(int count)
        {
            if (waitingWorkers != null)
            {
                try
                {
                    waitingWorkers.RawValue = count;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("SetWaitingWorkers Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                    waitingWorkers.Close();
                    waitingWorkers.Dispose();

                }
                catch (Exception e)
                {
                    LOG.Error("SetWaitingWorkers Error (Exception) updating performance counter: ", e);
                    waitingWorkers.Close();
                    waitingWorkers.Dispose();
					
                }
            }
        }

        public static void TaskQueueChanged(int itemsAdded, int totalCount)
        {
            if (itemsAdded > 0 && tasksQueuedPerSecond != null)
            {
                try
                {
                    tasksQueuedPerSecond.Increment();
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("TaskQueueChanged Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                }
                catch (Exception e)
                {
                    LOG.Error("TaskQueueChanged Error (Exception) updating performance counter: ", e);
                }
				
            }

            if (taskQueueLength != null)
            {
                try
                {
                    taskQueueLength.RawValue = totalCount;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("TaskQueueChanged Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                    taskQueueLength.Close();
                    taskQueueLength.Dispose();					
                }
                catch (Exception e)
                {
                    LOG.Error("TaskQueueChanged Error (Exception) updating performance counter: ", e);
                    taskQueueLength.Close();
                    taskQueueLength.Dispose();					
                }
				
            }
        }

        public static void TaskCompleted(long start, long end)
        {
            if (avgTaskTimeBase != null && avgTaskTime != null)
            {
                try
                {
                    avgTaskTime.IncrementBy(end - start);
                    avgTaskTimeBase.Increment();
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("TaskCompleted Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                    avgTaskTimeBase.Close();
                    avgTaskTimeBase.Dispose();					
                }
                catch (Exception e)
                {
                    LOG.Error("TaskCompleted Error (Exception) updating performance counter: ", e);
                    avgTaskTimeBase.Close();
                    avgTaskTimeBase.Dispose();					
                }
				
            }
        }

        //START SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
        public static void SetMonitoredServers(int count)
        {
            if (monitoredServers != null)
            {
                try
                {
                    monitoredServers.RawValue = count;

                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("SetMonitoredServers Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                    monitoredServers.Close();
                    monitoredServers.Dispose();					
                }
                catch (Exception e)
                {
                    LOG.Error("SetMonitoredServers Error (Exception) updating performance counter: ", e);
                    monitoredServers.Close();
                    monitoredServers.Dispose();					
                }				
            }
        }

        public static void SetMaintainenceModeServers(int count)
        {
            if (maintainenceModeServers != null) 
            {
                try
                {
                    maintainenceModeServers.RawValue = count;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    LOG.Error("SetMaintainenceModeServers Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                    maintainenceModeServers.Close();
                    maintainenceModeServers.Dispose();					
                }
                catch (Exception e)
                {
                    LOG.Error("SetMaintainenceModeServers Error (Exception) updating performance counter: ", e);
                    maintainenceModeServers.Close();
                    maintainenceModeServers.Dispose();					
                }
				
            }
            
        }

        public static void SetQueuedScheduledRefreshes(int count, string instanceName)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Queued Scheduled Refreshes") && queuedScheduledRefreshes != null)
                {
                    queuedScheduledRefreshes.InstanceName = instanceName;
                    queuedScheduledRefreshes.RawValue = count;
                    //Updating _Total Instance
                    queuedScheduledRefreshes.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    queuedScheduledRefreshes.IncrementBy(count);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetQueuedScheduledRefreshes Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                if(queuedScheduledRefreshes != null)
                {
                    queuedScheduledRefreshes.Close();
                    queuedScheduledRefreshes.Dispose();
                }				
            }
            catch (Exception e)
            {
                LOG.Error("SetQueuedScheduledRefreshes Error (Exception) updating performance counter: ", e);
                if(queuedScheduledRefreshes != null)
                {
                    queuedScheduledRefreshes.Close();
                    queuedScheduledRefreshes.Dispose();
                }				
            }
			
        }

        public static void SetDroppedScheduledRefreshes(int count, string instanceName)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Dropped Scheduled Refreshes") && droppedScheduledRefreshes != null)
                {
                    droppedScheduledRefreshes.InstanceName = instanceName;
                    droppedScheduledRefreshes.RawValue = count;
                    //Updating _Total Instance
                    droppedScheduledRefreshes.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    droppedScheduledRefreshes.IncrementBy(count);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetDroppedScheduledRefreshes Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                if (droppedScheduledRefreshes != null)
                {
                    droppedScheduledRefreshes.Close();
                    droppedScheduledRefreshes.Dispose();
                }				
            }
            catch (Exception e)
            {
                LOG.Error("SetDroppedScheduledRefreshes Error (Exception) updating performance counter: ", e);
                if (droppedScheduledRefreshes != null)
                {
                    droppedScheduledRefreshes.Close();
                    droppedScheduledRefreshes.Dispose();
                }				
            }			
        }

        public static void SetAlertsRaised(int count, string instanceName, bool clearCount)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Alerts Raised") && alertsRaised != null)
                {
                    alertsRaised.InstanceName = instanceName;
                    if (clearCount == false) alertsRaised.IncrementBy(count);
                    else alertsRaised.RawValue = count;
                    //Updating _Total Instance
                    alertsRaised.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    alertsRaised.IncrementBy(count);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetAlertsRaised Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
				if (alertsRaised !=null)
                {
                    alertsRaised.Close();
                    alertsRaised.Dispose();
                }
            }
            catch (Exception e)
            {
                LOG.Error("SetAlertsRaised Error (Exception) updating performance counter: ", e);
				if (alertsRaised !=null)
                {
                    alertsRaised.Close();
                    alertsRaised.Dispose();
                }
            }
		}

        public static void SetFailedCollections(int count, string instanceName)
        {
            try
            {
                if (categoryPerInstance.CounterExists("# Failed Collections") && failedCollections != null)
                {
                    failedCollections.InstanceName = instanceName;
                    failedCollections.RawValue = count;
                    //Updating _Total Instance
                    failedCollections.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    failedCollections.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Failed Collections counter: {0}", e);
                if(failedCollections != null)
                {
                    failedCollections.Close();
                    failedCollections.Dispose();
                }
            }
        }

        public static void SetDatabases(int? count, string instanceName)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Databases") && databases != null)
                {
                    databases.InstanceName = instanceName;
                    if (count != null)
                    {
                        databases.RawValue = (long)count;
                        //Updating _Total Instance
                        databases.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                        databases.IncrementBy((long)count);
                    }
                    else
                    {
                        databases.RawValue = 0;
                    }
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetDatabases Error (InvalidOperationException) updating performance counter: ", invalidOperationException);

                if(databases != null)
                {
                    databases.Close();
                    databases.Dispose();
                }
            }
            catch (Exception e)
            {
				LOG.Error("SetDatabases Error (Exception) updating performance counter: {0}", e);

                if(databases != null)
                {
                    databases.Close();
                    databases.Dispose();
                }
            }			
        }

        public static void SetQueryMonitorStatements(int count, string instanceName)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Query Monitor Statements") && queryMonitorStatements != null)
                {
                    queryMonitorStatements.InstanceName = instanceName;
                    queryMonitorStatements.RawValue = count;

                    //Updating _Total Instance
                    queryMonitorStatements.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    queryMonitorStatements.IncrementBy(count);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetQueryMonitorStatements Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                if(queryMonitorStatements != null)
                {
                    queryMonitorStatements.Close();
                    queryMonitorStatements.Dispose();
                }
			}
            catch (Exception e)
            {
                LOG.Error("SetQueryMonitorStatements Error (Exception) updating performance counter: ", e);
                if(queryMonitorStatements != null)
                {
                    queryMonitorStatements.Close();
                    queryMonitorStatements.Dispose();
                }
            }			
        }

        public static void SetWaitStatisticStatements(int count, string instanceName)
        {
            try
            {
                if (categoryPerInstance != null && categoryPerInstance.CounterExists("# Wait Statistic Statements") && waitStatisticStatements != null)
                {
                    waitStatisticStatements.InstanceName = instanceName;
                    waitStatisticStatements.RawValue = count;

                    //Updating _Total Instance
                    waitStatisticStatements.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                    waitStatisticStatements.IncrementBy(count);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LOG.Error("SetWaitStatisticStatements Error (InvalidOperationException) updating performance counter: ", invalidOperationException);
                if(waitStatisticStatements != null)
                {
                    waitStatisticStatements.Close();
                    waitStatisticStatements.Dispose();
                } 
			}
            catch (Exception e)
            {
                LOG.Error("SetWaitStatisticStatements Error (Exception) updating performance counter: ", e);
                if(waitStatisticStatements != null)
                {
                    waitStatisticStatements.Close();
                    waitStatisticStatements.Dispose();
                }
            }			
        }

        public static void SetCollectionRunTime(int count, string instanceName)
        {
 			
            try
            {
                if (categoryPerInstance.CounterExists("Collection Run Time") && collectionRunTime != null)
                {
                    collectionRunTime.InstanceName = instanceName;
                    collectionRunTime.RawValue = count;
                }

                //Updating _Total Instance
                collectionRunTime.InstanceName = PERFORMANCE_COUNTER_TOTAL_MONIKER;
                collectionRunTime.IncrementBy(count);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Collection Run Time counter: {0}", e);
                if (collectionRunTime != null)
                {
                    collectionRunTime.Close();
                    collectionRunTime.Dispose();
                }
            }
        }

        private static void DisposePerformanceCounterCategory(PerformanceCounterCategory category, String categoryName, String instanceName = "")
        {
            using (LOG.DebugCall("DisposePerformanceCounterCategory"))
            {
                try
                {
                    if (category != null && PerformanceCounterCategory.Exists(categoryName))
                    {
                        PerformanceCounter[] performanceCounters = new PerformanceCounter[0];
                        if (!String.IsNullOrEmpty(instanceName))
                        {
                            performanceCounters = category.GetCounters(instanceName);
                        }
                        else
                        {
                            performanceCounters = category.GetCounters();
                        }

                        foreach (var performanceCounter in performanceCounters)
                        {
                            performanceCounter.Close();
                            performanceCounter.Dispose();
                        }
                        LOG.DebugFormat("Deleting performance counter category: {0}", categoryName);
                        PerformanceCounterCategory.Delete(categoryName);
                    }
                }
                catch(Exception e)
                {
                    LOG.Error("The performance counters on the machine might be corrupted :", e);
                }
            }
        }

        public static void Dispose()
        {
            using (LOG.DebugCall("Dispose"))
            {
                DisposePerformanceCounterCategory(category, PERFCOUNTER_CATEGORY);

                DisposePerformanceCounterCategory(categoryPerInstance, PERFCOUNTER_CATEGORY_PERINSTANCE, "_Total");
            }
        }
        //END SQLdm 10.0 (Sanjali Makkar): Small Features : Adding Collection Service Counters
    }
}
