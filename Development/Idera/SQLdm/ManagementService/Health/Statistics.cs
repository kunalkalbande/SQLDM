//------------------------------------------------------------------------------
// <copyright file="Statistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.ManagementService.Configuration;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Idera.SQLdm.ManagementService.Health
{
    public static class Statistics
    {
        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);

        private static readonly BBS.TracerX.Logger LOG;

        public static string PERFCOUNTER_CATEGORY;
        public static PerformanceCounterCategory category;

        public static PerformanceCounter activeWorkers;

        public static PerformanceCounter waitingWorkers;

        public static PerformanceCounter taskQueueLength;
        public static PerformanceCounter tasksQueuedPerSecond;

        public static PerformanceCounter avgTaskTime;
        public static PerformanceCounter avgTaskTimeBase;

        //Performance Counters for SaveServerStatistics
        public static PerformanceCounter serverStatsPerSecond;
        public static PerformanceCounter serverStatSecPerInsert;
        public static PerformanceCounter serverStatSecPerInsertBase;

        //Performance Counters for SaveDatabaseStatistics
        public static PerformanceCounter databaseStatsPerSecond;
        public static PerformanceCounter databaseStatSecPerInsert;
        public static PerformanceCounter databaseStatSecPerInsertBase;

        //Performance Counters for TempDBFileStatistics for SaveDatabaseStatistics function
        public static PerformanceCounter tempDBFileStatsPerSecond;
        public static PerformanceCounter tempDBFileStatSecPerInsert;
        public static PerformanceCounter tempDBFileStatSecPerInsertBase;

        //Performance Counters for SaveDiskDrives
        public static PerformanceCounter diskStatsPerSecond;
        public static PerformanceCounter diskStatSecPerInsert;
        public static PerformanceCounter diskStatSecPerInsertBase;

        //Performance Counters for SaveOSStatistics
        public static PerformanceCounter osStatsPerSecond;
        public static PerformanceCounter osStatSecPerInsert;
        public static PerformanceCounter osStatSecPerInsertBase;

        //Performance Counters for SaveBlocking
        public static PerformanceCounter blockingStatsPerSecond;
        public static PerformanceCounter blockingStatSecPerInsert;
        public static PerformanceCounter blockingStatSecPerInsertBase;

        //Performance Counters for SaveSessionsAndLocks
        public static PerformanceCounter sessionAndLocksStatsPerSecond;
        public static PerformanceCounter sessionAndLocksStatSecPerInsert;
        public static PerformanceCounter sessionAndLocksStatSecPerInsertBase;

        //Performance Counters for 
        public static PerformanceCounter lockStatsPerSecond;
        public static PerformanceCounter lockStatSecPerInsert;
        public static PerformanceCounter lockStatSecPerInsertBase;

        //Performance Counters for SaveCustomCounters
        public static PerformanceCounter customStatsPerSecond;
        public static PerformanceCounter customStatSecPerInsert;
        public static PerformanceCounter customStatSecPerInsertBase;

        //Performance Counters for WaitTypes in SaveWaitStatistics
        public static PerformanceCounter waitTypeStatsPerSecond;
        public static PerformanceCounter waitTypeStatSecPerInsert;
        public static PerformanceCounter waitTypeStatSecPerInsertBase;

        //Performance Counters for WaitStats in SaveWaitStatistics
        public static PerformanceCounter waitStStatsPerSecond;
        public static PerformanceCounter waitStStatSecPerInsert;
        public static PerformanceCounter waitStStatSecPerInsertBase;

        //Performance Counters for WaitStatDetail in SaveWaitStatistics
        public static PerformanceCounter waitStDtlStatsPerSecond;
        public static PerformanceCounter waitStDtlStatSecPerInsert;
        public static PerformanceCounter waitStDtlStatSecPerInsertBase;

        //Performance Counters for SaveActiveWaits
        public static PerformanceCounter queryWaitStatsPerSecond;
        public static PerformanceCounter queryWaitStatSecPerInsert;
        public static PerformanceCounter queryWaitStatSecPerInsertBase;

        //Performance Counters for SaveQueryMonitorStatements
        public static PerformanceCounter queryMonitorStatsPerSecond;
        public static PerformanceCounter queryMonitorStatSecPerInsert;
        public static PerformanceCounter queryMonitorStatSecPerInsertBase;

        //Performance Counters for VMConfigDataStatistics in SaveVMStatistics
        public static PerformanceCounter vmConfigStatsPerSecond;
        public static PerformanceCounter vmConfigStatSecPerInsert;
        public static PerformanceCounter vmConfigStatSecPerInsertBase;

        //Performance Counters for ESXConfigDataStatistics in SaveVMStatistics
        public static PerformanceCounter esxConfigStatsPerSecond;
        public static PerformanceCounter esxConfigStatSecPerInsert;
        public static PerformanceCounter esxConfigStatSecPerInsertBase;

        //Performance Counters for VMStatistics in SaveVMStatistics
        public static PerformanceCounter vmStatsPerSecond;
        public static PerformanceCounter vmStatSecPerInsert;
        public static PerformanceCounter vmStatSecPerInsertBase;

        //Performance Counters for ESXStatistics in SaveVMStatistics
        public static PerformanceCounter esxStatsPerSecond;
        public static PerformanceCounter esxStatSecPerInsert;
        public static PerformanceCounter esxStatSecPerInsertBase;

        //Performance Counters for SaveDeadlocks
        public static PerformanceCounter deadlockPerSecond;
        public static PerformanceCounter deadlockSecPerInsert;
        public static PerformanceCounter deadlockStatSecPerInsertBase;

        //Performance Counters for SaveMirrorStatistics
        public static PerformanceCounter mirrorStatsPerSecond;
        public static PerformanceCounter mirrorStatSecPerInsert;
        public static PerformanceCounter mirrorStatSecPerInsertBase;

        //Performance Counters for GroomMirroringParticipants
        public static PerformanceCounter groomMirrorStatsPerSecond;
        public static PerformanceCounter groomMirrorSecPerInsert;
        public static PerformanceCounter groomMirrorSecPerInsertBase;

        //Performance Counters for GroomMirroringPreferredConfig
        public static PerformanceCounter mirrorPrefCfgStatsPerSecond;
        public static PerformanceCounter mirrorPrefCfgSecPerInsert;
        public static PerformanceCounter mirrorPrefCfgSecPerInsertBase;

        //Performance Counters for GroomReplicationTopology
        public static PerformanceCounter mirrorReplTopologyStatsPerSecond;
        public static PerformanceCounter mirrorReplTopologySecPerInsert;
        public static PerformanceCounter mirrorReplTopologySecPerInsertBase;

        //Performance Counters for SaveReplicationTopology
        public static PerformanceCounter replicationPerSecond;
        public static PerformanceCounter replicationSecPerInsert;
        public static PerformanceCounter replicationSecPerInsertBase;

        //Performance Counters for SaveReplicationTopologySubscriber in SaveReplicationTopology function
        public static PerformanceCounter replicationSubscriberPerSecond;
        public static PerformanceCounter replicationSubscriberSecPerInsert;
        public static PerformanceCounter replicationSubscriberSecPerInsertBase;

        //Performance Counters for SaveReplicationTopologyDistributor in SaveReplicationTopology function
        public static PerformanceCounter replicationDistributorPerSecond;
        public static PerformanceCounter replicationDistributorSecPerInsert;
        public static PerformanceCounter replicationDistributorSecPerInsertBase;


        static Statistics()
        {
            LOG = BBS.TracerX.Logger.GetLogger("Statistics");

            var publishPerformanceCounters = ManagementServiceConfiguration.GetManagementServiceElement().PublishPerformanceCounters;

            PERFCOUNTER_CATEGORY = Program.SERVICE_NAME_BASE +
                                   "$" +
                                   ManagementServiceConfiguration.InstanceName;

            if (!publishPerformanceCounters)
            {
                if (PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY))
                {
                    PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY);
                }
                // SQLDM-28034: 'return' to avoid creating and publishing Performance Counters and Categories for ManagementService
                return;
            }

            try
            {
                bool categoryExists = PerformanceCounterCategory.Exists(PERFCOUNTER_CATEGORY);
                if (categoryExists)
                {
                    //if the Last entry in the CounterCollectionData exists, then we need not delete the Counters as all the counters are
                    //registered already. If the Last counter is not present then we need to delete the existing counters and create again.
                    if (!PerformanceCounterCategory.CounterExists("Wait Statistics Details inserts/sec", PERFCOUNTER_CATEGORY))
                    {
                        PerformanceCounterCategory.Delete(PERFCOUNTER_CATEGORY);
                        categoryExists = false;
                    }
                }

                if (!categoryExists)
                {
                    var counterData = new CounterCreationDataCollection();
                    counterData.AddRange(new[] {
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

                                                // New Perf Counters for 7.5.5

                                                 new CounterCreationData("Server Statistics inserts/sec",
                                                                         "Average number of server statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Server Stats sec/insert",
                                                                         "Average time to insert a server statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Server Stats sec/insert Base",
                                                                         "Base counter for Server Stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Database Statistics inserts/sec",
                                                                         "Average number of database statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Database Stats sec/insert",
                                                                         "Average time to insert a database statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Database Stats sec/insert Base",
                                                                         "Base counter for database Stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Disk Statistics inserts/sec",
                                                                         "Average number of disk statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Disk Statistics sec/insert",
                                                                         "Average time to insert a disk statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Disk Statistics sec/insert Base",
                                                                         "Base counter for disk stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("OS Statistics inserts/sec",
                                                                         "Average number of OS statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("OS Statistics sec/insert",
                                                                         "Average time to insert an OS statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("OS Statistics sec/insert Base",
                                                                         "Base counter for OS stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Blocking Statistics inserts/sec",
                                                                         "Average number of blocking statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Blocking Statistics sec/insert",
                                                                         "Average time to insert a blocking statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Blocking Statistics sec/insert Base",
                                                                         "Base counter for blocking stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Session And Locks Statistics inserts/sec",
                                                                         "Average number of session and lock statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Session And Locks Statistics sec/insert",
                                                                         "Average time to insert a session and lock statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Session And Locks Statistics sec/insert Base",
                                                                         "Base counter for session and lock stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Lock Statistics inserts/sec",
                                                                         "Average number of lock statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Lock Statistics sec/insert",
                                                                         "Average time to insert a lock statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Lock Statistics sec/insert Base",
                                                                         "Base counter for lock stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Custom Statistics inserts/sec",
                                                                         "Average number of custom counter statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Custom Statistics sec/insert",
                                                                         "Average time to insert a custom counter statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Custom Statistics sec/insert Base",
                                                                         "Base counter for custom counter stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Wait Type Statistics inserts/sec",
                                                                         "Average number of wait type statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Wait Type Statistics written/sec",
                                                                         "Average time to insert a wait type statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Wait Type Stats sec/insert Base",
                                                                         "Base counter for wait type stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Query Wait Statistics inserts/sec",
                                                                         "Average number of query wait statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Query Wait Statistics sec/insert",
                                                                         "Average time to insert a query wait statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Query Wait Statistics sec/insert Base",
                                                                         "Base counter for query wait stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Query Monitor Statistics inserts/sec",
                                                                         "Average number of query monitor statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Query Monitor Statistics sec/insert",
                                                                         "Average time to insert a query monitor statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Query Monitor Statistics sec/insert Base",
                                                                         "Base counter for query monitor stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("VM Statistics inserts/sec",
                                                                         "Average number of VM statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("VM Statistics sec/insert",
                                                                         "Average time to insert a VM statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("VM Statistics sec/insert Base",
                                                                         "Base counter for VM stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Deadlock inserts/sec",
                                                                         "Average number of deadlock statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Deadlock sec/insert",
                                                                         "Average time to insert a deadlock statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Deadlock sec/insert Base",
                                                                         "Base counter for deadlock stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Mirroring Statistics inserts/sec",
                                                                         "Average number of mirroring statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Mirroring Statistics sec/insert",
                                                                         "Average time to insert a mirroring statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Mirroring Statistics sec/insert Base",
                                                                         "Base counter for mirroring stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Mirror Participants grooming ops/sec",
                                                                         "Average number of mirror participants statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Mirror Participants grooming sec/op",
                                                                         "Average time to insert a mirror participants statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Mirror Participants grooming sec/op Base",
                                                                         "Base counter for mirror participants stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Replication topology inserts/sec",
                                                                         "Average number of replication topology statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Replication topology sec/insert",
                                                                         "Average time to insert a replication topology statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Replication topology sec/insert Base",
                                                                         "Base counter for replication topology stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Mirror Preferred Config ops/sec",
                                                                         "Average number of mirror preferred config statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Mirror Preferred Config sec/op",
                                                                         "Average time to insert a mirror preferred config statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Mirror Preferred Config sec/op Base",
                                                                         "Base counter for mirror preferred config stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Mirror Replication Topology ops/sec",
                                                                         "Average number of mirror replication topology statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Mirror Replication Topology sec/op",
                                                                         "Average time to insert a mirror replication topology statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Mirror Replication Topology sec/op Base",
                                                                         "Base counter for mirror replication topology stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Temp Database File inserts/sec",
                                                                         "Average number of temp database file statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Temp Database File sec/insert",
                                                                         "Average time to insert a temp database file statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Temp Database File sec/insert Base",
                                                                         "Base counter for temp database file stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Replication topology subscriber inserts/sec",
                                                                         "Average number of replication topology subscriber statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Replication topology subscriber sec/insert",
                                                                         "Average time to insert a replication topology subscriber statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Replication topology subscriber sec/insert Base",
                                                                         "Base counter for replication topology subscriber stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Replication topology distributor inserts/sec",
                                                                         "Average number of replication topology distributor statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Replication topology distributor sec/insert",
                                                                         "Average time to insert a replication topology distributor statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Replication topology distributor sec/insert Base",
                                                                         "Base counter for replication topology distributor stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("ESX Config Data Statistics inserts/sec",
                                                                         "Average number of ESX config data statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("ESX Config Data Statistics sec/insert",
                                                                         "Average time to insert a ESX config data statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("ESX Config Data Statistics sec/insert Base",
                                                                         "Base counter for ESX config data stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("VM Config Data Statistics inserts/sec",
                                                                         "Average number of VM config data statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("VM Config Data Statistics sec/insert",
                                                                         "Average time to insert a VM config data statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("VM Config Data Statistics sec/insert Base",
                                                                         "Base counter for VM config data stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("ESX Statistics inserts/sec",
                                                                         "Average number of ESX statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("ESX Statistics sec/insert",
                                                                         "Average time to insert a ESX statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("ESX Statistics sec/insert Base",
                                                                         "Base counter for ESX stats sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Wait Statistics inserts/sec",
                                                                         "Average number of wait statistics written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Wait Statistics written/sec",
                                                                         "Average time to insert a wait statistics row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Wait Stats sec/insert Base",
                                                                         "Base counter for wait statistics sec/insert",
                                                                         PerformanceCounterType.AverageBase),

                                                 new CounterCreationData("Wait Statistics Details inserts/sec",
                                                                         "Average number of wait type statistics details written per second",
                                                                         PerformanceCounterType.RateOfCountsPerSecond32),
                                                 new CounterCreationData("Wait Statistics Details written/sec",
                                                                         "Average time to insert a wait statistics details row",
                                                                         PerformanceCounterType.AverageTimer32),
                                                 new CounterCreationData("Wait Statistics Details sec/insert Base",
                                                                         "Base counter for wait statistics details sec/insert",
                                                                         PerformanceCounterType.AverageBase)

                    });

                    category = PerformanceCounterCategory.Create(PERFCOUNTER_CATEGORY, "",
                                                                 PerformanceCounterCategoryType.SingleInstance,
                                                                 counterData);
                }
                else
                    category = new PerformanceCounterCategory(PERFCOUNTER_CATEGORY);

                activeWorkers = InitializeCounter(category, "ActiveWorkers");
                waitingWorkers = InitializeCounter(category, "WaitingWorkers");
                taskQueueLength = InitializeCounter(category, "Task Queue Length");
                tasksQueuedPerSecond = InitializeCounter(category, "Tasks Queued/sec");

                avgTaskTime = InitializeCounter(category, "Avg. Task Time");
                avgTaskTimeBase = InitializeCounter(category, "Avg. Task Time Base");

                serverStatsPerSecond = InitializeCounter(category, "Server Statistics inserts/sec");
                serverStatSecPerInsert = InitializeCounter(category, "Server Stats sec/insert");
                serverStatSecPerInsertBase = InitializeCounter(category, "Server Stats sec/insert Base");

                databaseStatsPerSecond = InitializeCounter(category, "Database Statistics inserts/sec");
                databaseStatSecPerInsert = InitializeCounter(category, "Database Stats sec/insert");
                databaseStatSecPerInsertBase = InitializeCounter(category, "Database Stats sec/insert Base");

                diskStatsPerSecond = InitializeCounter(category, "Disk Statistics inserts/sec");
                diskStatSecPerInsert = InitializeCounter(category, "Disk Statistics sec/insert");
                diskStatSecPerInsertBase = InitializeCounter(category, "Disk Statistics sec/insert Base");

                osStatsPerSecond = InitializeCounter(category, "OS Statistics inserts/sec");
                osStatSecPerInsert = InitializeCounter(category, "OS Statistics sec/insert");
                osStatSecPerInsertBase = InitializeCounter(category, "OS Statistics sec/insert Base");

                blockingStatsPerSecond = InitializeCounter(category, "Blocking Statistics inserts/sec");
                blockingStatSecPerInsert = InitializeCounter(category, "Blocking Statistics sec/insert");
                blockingStatSecPerInsertBase = InitializeCounter(category, "Blocking Statistics sec/insert Base");

                sessionAndLocksStatsPerSecond = InitializeCounter(category, "Session And Locks Statistics inserts/sec");
                sessionAndLocksStatSecPerInsert = InitializeCounter(category, "Session And Locks Statistics sec/insert");
                sessionAndLocksStatSecPerInsertBase = InitializeCounter(category, "Session And Locks Statistics sec/insert Base");
                
                //These Categories are not added in collection
                lockStatsPerSecond = InitializeCounter(category, "Lock Statistics inserts/sec");
                lockStatSecPerInsert = InitializeCounter(category, "Lock Statistics sec/insert");
                lockStatSecPerInsertBase = InitializeCounter(category, "Lock Statistics sec/insert Base");

                customStatsPerSecond = InitializeCounter(category, "Custom Statistics inserts/sec");
                customStatSecPerInsert = InitializeCounter(category, "Custom Statistics sec/insert");
                customStatSecPerInsertBase = InitializeCounter(category, "Custom Statistics sec/insert Base");

                waitTypeStatsPerSecond = InitializeCounter(category, "Wait Type Statistics inserts/sec");
                waitTypeStatSecPerInsert = InitializeCounter(category, "Wait Type Statistics written/sec");
                waitTypeStatSecPerInsertBase = InitializeCounter(category, "Wait Type Stats sec/insert Base");

                queryWaitStatsPerSecond = InitializeCounter(category, "Query Wait Statistics inserts/sec");
                queryWaitStatSecPerInsert = InitializeCounter(category, "Query Wait Statistics sec/insert");
                queryWaitStatSecPerInsertBase = InitializeCounter(category, "Query Wait Statistics sec/insert Base");

                queryMonitorStatsPerSecond = InitializeCounter(category, "Query Monitor Statistics inserts/sec");
                queryMonitorStatSecPerInsert = InitializeCounter(category, "Query Monitor Statistics sec/insert");
                queryMonitorStatSecPerInsertBase = InitializeCounter(category, "Query Monitor Statistics sec/insert Base");

                vmStatsPerSecond = InitializeCounter(category, "VM Statistics inserts/sec");
                vmStatSecPerInsert = InitializeCounter(category, "VM Statistics sec/insert");
                vmStatSecPerInsertBase = InitializeCounter(category, "VM Statistics sec/insert Base");

                deadlockPerSecond = InitializeCounter(category, "Deadlock inserts/sec");
                deadlockSecPerInsert = InitializeCounter(category, "Deadlock sec/insert");
                deadlockStatSecPerInsertBase = InitializeCounter(category, "Deadlock sec/insert Base");

                mirrorStatsPerSecond = InitializeCounter(category, "Mirroring Statistics inserts/sec");
                mirrorStatSecPerInsert = InitializeCounter(category, "Mirroring Statistics sec/insert");
                mirrorStatSecPerInsertBase = InitializeCounter(category, "Mirroring Statistics sec/insert Base");

                groomMirrorStatsPerSecond = InitializeCounter(category, "Mirror Participants grooming ops/sec");
                groomMirrorSecPerInsert = InitializeCounter(category, "Mirror Participants grooming sec/op");
                groomMirrorSecPerInsertBase = InitializeCounter(category, "Mirror Participants grooming sec/op Base");

                replicationPerSecond = InitializeCounter(category, "Replication topology inserts/sec");
                replicationSecPerInsert = InitializeCounter(category, "Replication topology sec/insert");
                replicationSecPerInsertBase = InitializeCounter(category, "Replication topology sec/insert Base");

                mirrorPrefCfgStatsPerSecond = InitializeCounter(category, "Mirror Preferred Config ops/sec");
                mirrorPrefCfgSecPerInsert = InitializeCounter(category, "Mirror Preferred Config sec/op");
                mirrorPrefCfgSecPerInsertBase = InitializeCounter(category, "Mirror Preferred Config sec/op Base");

                mirrorReplTopologyStatsPerSecond = InitializeCounter(category, "Mirror Replication Topology ops/sec");
                mirrorReplTopologySecPerInsert = InitializeCounter(category, "Mirror Replication Topology sec/op");
                mirrorReplTopologySecPerInsertBase = InitializeCounter(category, "Mirror Replication Topology sec/op Base");

                tempDBFileStatsPerSecond = InitializeCounter(category, "Temp Database File inserts/sec");
                tempDBFileStatSecPerInsert = InitializeCounter(category, "Temp Database File sec/insert");
                tempDBFileStatSecPerInsertBase = InitializeCounter(category, "Temp Database File sec/insert Base");

                replicationSubscriberPerSecond = InitializeCounter(category, "Replication topology subscriber inserts/sec");
                replicationSubscriberSecPerInsert = InitializeCounter(category, "Replication topology subscriber sec/insert");
                replicationSubscriberSecPerInsertBase = InitializeCounter(category, "Replication topology subscriber sec/insert Base");

                replicationDistributorPerSecond = InitializeCounter(category, "Replication topology distributor inserts/sec");
                replicationDistributorSecPerInsert = InitializeCounter(category, "Replication topology distributor sec/insert");
                replicationDistributorSecPerInsertBase = InitializeCounter(category, "Replication topology distributor sec/insert Base");

                esxConfigStatsPerSecond = InitializeCounter(category, "ESX Config Data Statistics inserts/sec");
                esxConfigStatSecPerInsert = InitializeCounter(category, "ESX Config Data Statistics sec/insert");
                esxConfigStatSecPerInsertBase = InitializeCounter(category, "ESX Config Data Statistics sec/insert Base");

                vmConfigStatsPerSecond = InitializeCounter(category, "VM Config Data Statistics inserts/sec");
                vmConfigStatSecPerInsert = InitializeCounter(category, "VM Config Data Statistics sec/insert");
                vmConfigStatSecPerInsertBase = InitializeCounter(category, "VM Config Data Statistics sec/insert Base");

                esxStatsPerSecond = InitializeCounter(category, "ESX Statistics inserts/sec");
                esxStatSecPerInsert = InitializeCounter(category, "ESX Statistics sec/insert");
                esxStatSecPerInsertBase = InitializeCounter(category, "ESX Statistics sec/insert Base");

                waitStStatsPerSecond = InitializeCounter(category, "Wait Statistics inserts/sec");
                waitStStatSecPerInsert = InitializeCounter(category, "Wait Statistics written/sec");
                waitStStatSecPerInsertBase = InitializeCounter(category, "Wait Stats sec/insert Base");

                waitStDtlStatsPerSecond = InitializeCounter(category, "Wait Statistics Details inserts/sec");
                waitStDtlStatSecPerInsert = InitializeCounter(category, "Wait Statistics Details written/sec");
                waitStDtlStatSecPerInsertBase = InitializeCounter(category, "Wait Statistics Details sec/insert Base");

            }
            catch (Exception e)
            {
                LOG.Error("Error creating and registering performance counters: ", e);
            }
        }

        private static PerformanceCounter InitializeCounter(PerformanceCounterCategory perfcategory, string counter)
        {
            var pc = new PerformanceCounter();
            pc.CategoryName = perfcategory.CategoryName;
            pc.CounterName = counter;
            pc.MachineName = ".";
            pc.ReadOnly = false;
            pc.RawValue = 0;
            return pc;
        }

        public static void SetActiveWorkers(int count)
        {
            try
            {
                if (activeWorkers != null)
                    activeWorkers.RawValue = count;
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Set Active Workers counter: {0}", e);
                if (activeWorkers != null)
                {
                    activeWorkers.Close();
                    activeWorkers.Dispose();
                }
            }
        }

        public static void SetWaitingWorkers(int count)
        {
            try
            {
                if (waitingWorkers != null)
                    waitingWorkers.RawValue = count;
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Set Waiting Workers counter: {0}", e);
                if (waitingWorkers != null)
                {
                    waitingWorkers.Close();
                    waitingWorkers.Dispose();
                }
            }
        }

        public static void TaskQueueChanged(int itemsAdded, int totalCount)
        {
            try
            {
                if (itemsAdded > 0 && tasksQueuedPerSecond != null)
                    tasksQueuedPerSecond.Increment();

                if (taskQueueLength != null)
                    taskQueueLength.RawValue = totalCount;
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Task Queue Changed counter: {0}", e);
                if (avgTaskTime != null)
                {
                    avgTaskTime.Close();
                    avgTaskTime.Dispose();
                }
                if (avgTaskTimeBase != null)
                {
                    avgTaskTimeBase.Close();
                    avgTaskTimeBase.Dispose();
                }
            }

        }

        public static void TaskCompleted(long start, long end)
        {
            try
            {
                if (avgTaskTimeBase != null && avgTaskTime != null)
                {
                    avgTaskTime.IncrementBy(end - start);
                    avgTaskTimeBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Task Completed counter: {0}", e);
                if (avgTaskTime != null)
                {
                    avgTaskTime.Close();
                    avgTaskTime.Dispose();
                }
                if (avgTaskTimeBase != null)
                {
                    avgTaskTimeBase.Close();
                    avgTaskTimeBase.Dispose();
                }
            }

        }

        public static void ServerStatisticWritten(long start, long end)
        {
            try
            {
                if (serverStatsPerSecond != null)
                    serverStatsPerSecond.Increment();

                if (serverStatSecPerInsert != null && serverStatSecPerInsertBase != null)
                {
                    serverStatSecPerInsert.IncrementBy(end - start);
                    serverStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Server Statistic counter: {0}", e);
                if (serverStatsPerSecond != null)
                {
                    serverStatsPerSecond.Close();
                    serverStatsPerSecond.Dispose();
                }
                if (serverStatSecPerInsert != null)
                {
                    serverStatSecPerInsert.Close();
                    serverStatSecPerInsert.Dispose();
                }
                if (serverStatSecPerInsertBase != null)
                {
                    serverStatSecPerInsertBase.Close();
                    serverStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void DatabaseStatisticWritten(long start, long end)
        {
            try
            {
                if (databaseStatsPerSecond != null)
                    databaseStatsPerSecond.Increment();

                if (databaseStatSecPerInsert != null && databaseStatSecPerInsertBase != null)
                {
                    databaseStatSecPerInsert.IncrementBy(end - start);
                    databaseStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Database Statistic counter: {0}", e);
                if (databaseStatsPerSecond != null)
                {
                    databaseStatsPerSecond.Close();
                    databaseStatsPerSecond.Dispose();
                }
                if (databaseStatSecPerInsert != null)
                {
                    databaseStatSecPerInsert.Close();
                    databaseStatSecPerInsert.Dispose();
                }
                if (databaseStatSecPerInsertBase != null)
                {
                    databaseStatSecPerInsertBase.Close();
                    databaseStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void DatabaseStatisticWrittenBulk(long start, long end, long incrementBy)
        {
            try
            {
                if (databaseStatsPerSecond != null)
                    databaseStatsPerSecond.Increment();

                if (databaseStatSecPerInsert != null && databaseStatSecPerInsertBase != null)
                {
                    databaseStatSecPerInsert.IncrementBy(end - start);
                    databaseStatSecPerInsertBase.IncrementBy(incrementBy);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Database Statistic counter: {0}", e);
                if (databaseStatsPerSecond != null)
                {
                    databaseStatsPerSecond.Close();
                    databaseStatsPerSecond.Dispose();
                }
                if (databaseStatSecPerInsert != null)
                {
                    databaseStatSecPerInsert.Close();
                    databaseStatSecPerInsert.Dispose();
                }
                if (databaseStatSecPerInsertBase != null)
                {
                    databaseStatSecPerInsertBase.Close();
                    databaseStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void DiskStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (diskStatsPerSecond != null)
                    diskStatsPerSecond.IncrementBy(count);

                if (diskStatSecPerInsert != null && diskStatSecPerInsertBase != null)
                {
                    diskStatSecPerInsert.IncrementBy(end - start);
                    diskStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Disk Statistic counter: {0}", e);
                if (diskStatsPerSecond != null)
                {
                    diskStatsPerSecond.Close();
                    diskStatsPerSecond.Dispose();
                }
                if (diskStatSecPerInsert != null)
                {
                    diskStatSecPerInsert.Close();
                    diskStatSecPerInsert.Dispose();
                }
                if (diskStatSecPerInsertBase != null)
                {
                    diskStatSecPerInsertBase.Close();
                    diskStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void OSStatisticWritten(long start, long end)
        {
            try
            {
                if (osStatsPerSecond != null)
                    osStatsPerSecond.Increment();

                if (osStatSecPerInsert != null && osStatSecPerInsertBase != null)
                {
                    osStatSecPerInsert.IncrementBy(end - start);
                    osStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating OS Statistic counter: {0}", e);
                if (osStatsPerSecond != null)
                {
                    osStatsPerSecond.Close();
                    osStatsPerSecond.Dispose();
                }
                if (osStatSecPerInsert != null)
                {
                    osStatSecPerInsert.Close();
                    osStatSecPerInsert.Dispose();
                }
                if (osStatSecPerInsertBase != null)
                {
                    osStatSecPerInsertBase.Close();
                    osStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void BlockingStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (blockingStatsPerSecond != null)
                    blockingStatsPerSecond.IncrementBy(count);

                if (blockingStatSecPerInsert != null && blockingStatSecPerInsertBase != null)
                {
                    blockingStatSecPerInsert.IncrementBy(end - start);
                    blockingStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Blocking Statistic counter: {0}", e);
                if (blockingStatsPerSecond != null)
                {
                    blockingStatsPerSecond.Close();
                    blockingStatsPerSecond.Dispose();
                }
                if (blockingStatSecPerInsert != null)
                {
                    blockingStatSecPerInsert.Close();
                    blockingStatSecPerInsert.Dispose();
                }
                if (blockingStatSecPerInsertBase != null)
                {
                    blockingStatSecPerInsertBase.Close();
                    blockingStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void SessionAndLocksStatisticWritten(long start, long end)
        {
            try
            {
                if (sessionAndLocksStatsPerSecond != null)
                    sessionAndLocksStatsPerSecond.Increment();

                if (sessionAndLocksStatSecPerInsert != null && sessionAndLocksStatSecPerInsertBase != null)
                {
                    sessionAndLocksStatSecPerInsert.IncrementBy(end - start);
                    sessionAndLocksStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Session And Locks Statistic counter: {0}", e);
                if (sessionAndLocksStatsPerSecond != null)
                {
                    sessionAndLocksStatsPerSecond.Close();
                    sessionAndLocksStatsPerSecond.Dispose();
                }
                if (sessionAndLocksStatSecPerInsert != null)
                {
                    sessionAndLocksStatSecPerInsert.Close();
                    sessionAndLocksStatSecPerInsert.Dispose();
                }
                if (sessionAndLocksStatSecPerInsertBase != null)
                {
                    sessionAndLocksStatSecPerInsertBase.Close();
                    sessionAndLocksStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void LockStatisticWritten(long start, long end)
        {
            try
            {
                if (lockStatsPerSecond != null)
                    lockStatsPerSecond.Increment();

                if (lockStatSecPerInsert != null && lockStatSecPerInsertBase != null)
                {
                    lockStatSecPerInsert.IncrementBy(end - start);
                    lockStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Lock Statistic counter: {0}", e);
                if (lockStatsPerSecond != null)
                {
                    lockStatsPerSecond.Close();
                    lockStatsPerSecond.Dispose();
                }
                if (lockStatSecPerInsert != null)
                {
                    lockStatSecPerInsert.Close();
                    lockStatSecPerInsert.Dispose();
                }
                if (lockStatSecPerInsertBase != null)
                {
                    lockStatSecPerInsertBase.Close();
                    lockStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void CustomStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (customStatsPerSecond != null)
                    customStatsPerSecond.IncrementBy(count);

                if (customStatSecPerInsert != null && customStatSecPerInsertBase != null)
                {
                    customStatSecPerInsert.IncrementBy(end - start);
                    customStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Custom Statistic counter: {0}", e);
                if (customStatsPerSecond != null)
                {
                    customStatsPerSecond.Close();
                    customStatsPerSecond.Dispose();
                }
                if (customStatSecPerInsert != null)
                {
                    customStatSecPerInsert.Close();
                    customStatSecPerInsert.Dispose();
                }
                if (customStatSecPerInsertBase != null)
                {
                    customStatSecPerInsertBase.Close();
                    customStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void WaitStStatisticWritten(long start, long end)
        {
            try
            {
                if (waitStStatsPerSecond != null)
                    waitStStatsPerSecond.Increment();

                if (waitStStatSecPerInsert != null && waitStStatSecPerInsertBase != null)
                {
                    waitStStatSecPerInsert.IncrementBy(end - start);
                    waitStStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Wait St Statistic counter: {0}", e);
                if (waitStStatsPerSecond != null)
                {
                    waitStStatsPerSecond.Close();
                    waitStStatsPerSecond.Dispose();
                }
                if (waitStStatSecPerInsert != null)
                {
                    waitStStatSecPerInsert.Close();
                    waitStStatSecPerInsert.Dispose();
                }
                if (waitStStatSecPerInsertBase != null)
                {
                    waitStStatSecPerInsertBase.Close();
                    waitStStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void WaitTypeStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (waitTypeStatsPerSecond != null)
                    waitTypeStatsPerSecond.IncrementBy(count);

                if (waitTypeStatSecPerInsert != null && waitTypeStatSecPerInsertBase != null)
                {
                    waitTypeStatSecPerInsert.IncrementBy(end - start);
                    waitTypeStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Wait Type Statistic counter: {0}", e);
                if (waitTypeStatsPerSecond != null)
                {
                    waitTypeStatsPerSecond.Close();
                    waitTypeStatsPerSecond.Dispose();
                }
                if (waitTypeStatSecPerInsert != null)
                {
                    waitTypeStatSecPerInsert.Close();
                    waitTypeStatSecPerInsert.Dispose();
                }
                if (waitTypeStatSecPerInsertBase != null)
                {
                    waitTypeStatSecPerInsertBase.Close();
                    waitTypeStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void WaitStDtlStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (waitStDtlStatsPerSecond != null)
                    waitStDtlStatsPerSecond.IncrementBy(count);

                if (waitStDtlStatSecPerInsert != null && waitStDtlStatSecPerInsertBase != null)
                {
                    waitStDtlStatSecPerInsert.IncrementBy(end - start);
                    waitStDtlStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Wait StDtl Statistic counter: {0}", e);
                if (waitStDtlStatsPerSecond != null)
                {
                    waitStDtlStatsPerSecond.Close();
                    waitStDtlStatsPerSecond.Dispose();
                }
                if (waitStDtlStatSecPerInsert != null)
                {
                    waitStDtlStatSecPerInsert.Close();
                    waitStDtlStatSecPerInsert.Dispose();
                }
                if (waitStDtlStatSecPerInsertBase != null)
                {
                    waitStDtlStatSecPerInsertBase.Close();
                    waitStDtlStatSecPerInsertBase.Dispose();
                }
            }
        }

        public static void QueryWaitStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (queryWaitStatsPerSecond != null)
                    queryWaitStatsPerSecond.IncrementBy(count);

                if (queryWaitStatSecPerInsert != null && queryWaitStatSecPerInsertBase != null)
                {
                    queryWaitStatSecPerInsert.IncrementBy(end - start);
                    queryWaitStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Query Wait Statistic counter: {0}", e);
                if (queryWaitStatsPerSecond != null)
                {
                    queryWaitStatsPerSecond.Close();
                    queryWaitStatsPerSecond.Dispose();
                }
                if (queryWaitStatSecPerInsert != null)
                {
                    queryWaitStatSecPerInsert.Close();
                    queryWaitStatSecPerInsert.Dispose();
                }
                if (queryWaitStatSecPerInsertBase != null)
                {
                    queryWaitStatSecPerInsertBase.Close();
                    queryWaitStatSecPerInsertBase.Dispose();
                }
            }
        }

        public static void VMStatisticWritten(long start, long end)
        {
            try
            {
                if (vmStatsPerSecond != null)
                    vmStatsPerSecond.Increment();

                if (vmStatSecPerInsert != null && vmStatSecPerInsertBase != null)
                {
                    vmStatSecPerInsert.IncrementBy(end - start);
                    vmStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating VM Statistic counter: {0}", e);
                if (vmStatsPerSecond != null)
                {
                    vmStatsPerSecond.Close();
                    vmStatsPerSecond.Dispose();
                }
                if (vmStatSecPerInsert != null)
                {
                    vmStatSecPerInsert.Close();
                    vmStatSecPerInsert.Dispose();
                }
                if (vmStatSecPerInsertBase != null)
                {
                    vmStatSecPerInsertBase.Close();
                    vmStatSecPerInsertBase.Dispose();
                }
            }
        }

        public static void ESXStatisticWritten(long start, long end)
        {
            try
            {
                if (esxStatsPerSecond != null)
                    esxStatsPerSecond.Increment();

                if (esxStatSecPerInsert != null && esxStatSecPerInsertBase != null)
                {
                    esxStatSecPerInsert.IncrementBy(end - start);
                    esxStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating ESX Statistic counter: {0}", e);
                if (esxStatsPerSecond != null)
                {
                    esxStatsPerSecond.Close();
                    esxStatsPerSecond.Dispose();
                }
                if (esxStatSecPerInsert != null)
                {
                    esxStatSecPerInsert.Close();
                    esxStatSecPerInsert.Dispose();
                }
                if (esxStatSecPerInsertBase != null)
                {
                    esxStatSecPerInsertBase.Close();
                    esxStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void VMConfigStatisticWritten(long start, long end)
        {
            try
            {
                if (vmConfigStatsPerSecond != null)
                    vmConfigStatsPerSecond.Increment();

                if (vmConfigStatSecPerInsert != null && vmConfigStatSecPerInsertBase != null)
                {
                    vmConfigStatSecPerInsert.IncrementBy(end - start);
                    vmConfigStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating VMConfig Statistic counter: {0}", e);
                if (vmConfigStatsPerSecond != null)
                {
                    vmConfigStatsPerSecond.Close();
                    vmConfigStatsPerSecond.Dispose();
                }
                if (vmConfigStatSecPerInsert != null)
                {
                    vmConfigStatSecPerInsert.Close();
                    vmConfigStatSecPerInsert.Dispose();
                }
                if (vmConfigStatSecPerInsertBase != null)
                {
                    vmConfigStatSecPerInsertBase.Close();
                    vmConfigStatSecPerInsertBase.Dispose();
                }
            }
        }

        public static void ESXConfigStatisticWritten(long start, long end)
        {
            try
            {
                if (esxConfigStatsPerSecond != null)
                    esxConfigStatsPerSecond.Increment();

                if (esxConfigStatSecPerInsert != null && esxConfigStatSecPerInsertBase != null)
                {
                    esxConfigStatSecPerInsert.IncrementBy(end - start);
                    esxConfigStatSecPerInsertBase.Increment();
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating ESXConfig Statistic counter: {0}", e);
                if (esxConfigStatsPerSecond != null)
                {
                    esxConfigStatsPerSecond.Close();
                    esxConfigStatsPerSecond.Dispose();
                }
                if (esxConfigStatSecPerInsert != null)
                {
                    esxConfigStatSecPerInsert.Close();
                    esxConfigStatSecPerInsert.Dispose();
                }
                if (esxConfigStatSecPerInsertBase != null)
                {
                    esxConfigStatSecPerInsertBase.Close();
                    esxConfigStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void MirrorStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (mirrorStatsPerSecond != null)
                    mirrorStatsPerSecond.IncrementBy(count);

                if (mirrorStatSecPerInsert != null && mirrorStatSecPerInsertBase != null)
                {
                    mirrorStatSecPerInsert.IncrementBy(end - start);
                    mirrorStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Mirror Statistic counter: {0}", e);
                if (mirrorStatsPerSecond != null)
                {
                    mirrorStatsPerSecond.Close();
                    mirrorStatsPerSecond.Dispose();
                }
                if (mirrorStatSecPerInsert != null)
                {
                    mirrorStatSecPerInsert.Close();
                    mirrorStatSecPerInsert.Dispose();
                }
                if (mirrorStatSecPerInsertBase != null)
                {
                    mirrorStatSecPerInsertBase.Close();
                    mirrorStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void DeadlocksStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (deadlockPerSecond != null)
                    deadlockPerSecond.IncrementBy(count);

                if (deadlockSecPerInsert != null && deadlockStatSecPerInsertBase != null)
                {
                    deadlockSecPerInsert.IncrementBy(end - start);
                    deadlockStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Deadlocks Statistic counter: {0}", e);
                if (deadlockPerSecond != null)
                {
                    deadlockPerSecond.Close();
                    deadlockPerSecond.Dispose();
                }
                if (deadlockSecPerInsert != null)
                {
                    deadlockSecPerInsert.Close();
                    deadlockSecPerInsert.Dispose();
                }
                if (deadlockStatSecPerInsertBase != null)
                {
                    deadlockStatSecPerInsertBase.Close();
                    deadlockStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void QMStatementsStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (queryMonitorStatsPerSecond != null)
                    queryMonitorStatsPerSecond.IncrementBy(count);

                if (queryMonitorStatSecPerInsert != null && queryMonitorStatSecPerInsertBase != null)
                {
                    queryMonitorStatSecPerInsert.IncrementBy(end - start);
                    queryMonitorStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating QM Statements Statistic counter: {0}", e);
                if (queryMonitorStatsPerSecond != null)
                {
                    queryMonitorStatsPerSecond.Close();
                    queryMonitorStatsPerSecond.Dispose();
                }
                if (queryMonitorStatSecPerInsert != null)
                {
                    queryMonitorStatSecPerInsert.Close();
                    queryMonitorStatSecPerInsert.Dispose();
                }
                if (queryMonitorStatSecPerInsertBase != null)
                {
                    queryMonitorStatSecPerInsertBase.Close();
                    queryMonitorStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void GroomMirrorPaticipantsStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (groomMirrorStatsPerSecond != null)
                    groomMirrorStatsPerSecond.IncrementBy(count);

                if (groomMirrorSecPerInsert != null && groomMirrorSecPerInsertBase != null)
                {
                    groomMirrorSecPerInsert.IncrementBy(end - start);
                    groomMirrorSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Groom Mirror Paticipants Statistic counter: {0}", e);
                if (groomMirrorStatsPerSecond != null)
                {
                    groomMirrorStatsPerSecond.Close();
                    groomMirrorStatsPerSecond.Dispose();
                }
                if (groomMirrorSecPerInsert != null)
                {
                    groomMirrorSecPerInsert.Close();
                    groomMirrorSecPerInsert.Dispose();
                }
                if (groomMirrorSecPerInsertBase != null)
                {
                    groomMirrorSecPerInsertBase.Close();
                    groomMirrorSecPerInsertBase.Dispose();
                }
            }

        }

        public static void GroomMirrorPrefCfgStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (mirrorPrefCfgStatsPerSecond != null)
                    mirrorPrefCfgStatsPerSecond.IncrementBy(count);

                if (mirrorPrefCfgSecPerInsert != null && mirrorPrefCfgSecPerInsertBase != null)
                {
                    mirrorPrefCfgSecPerInsert.IncrementBy(end - start);
                    mirrorPrefCfgSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Groom Mirror Repl Topology Statistic counter: {0}", e);
                if (mirrorPrefCfgStatsPerSecond != null)
                {
                    mirrorPrefCfgStatsPerSecond.Close();
                    mirrorPrefCfgStatsPerSecond.Dispose();
                }
                if (mirrorPrefCfgSecPerInsert != null)
                {
                    mirrorPrefCfgSecPerInsert.Close();
                    mirrorPrefCfgSecPerInsert.Dispose();
                }
                if (mirrorPrefCfgSecPerInsertBase != null)
                {
                    mirrorPrefCfgSecPerInsertBase.Close();
                    mirrorPrefCfgSecPerInsertBase.Dispose();
                }
            }

        }

        public static void GroomMirrorReplTopologyStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (mirrorReplTopologyStatsPerSecond != null)
                    mirrorReplTopologyStatsPerSecond.IncrementBy(count);

                if (mirrorReplTopologySecPerInsert != null && mirrorReplTopologySecPerInsertBase != null)
                {
                    mirrorReplTopologySecPerInsert.IncrementBy(end - start);
                    mirrorReplTopologySecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Groom Mirror Repl Topology Statistic counter: {0}", e);
                if (mirrorReplTopologyStatsPerSecond != null)
                {
                    mirrorReplTopologyStatsPerSecond.Close();
                    mirrorReplTopologyStatsPerSecond.Dispose();
                }
                if (mirrorReplTopologySecPerInsert != null)
                {
                    mirrorReplTopologySecPerInsert.Close();
                    mirrorReplTopologySecPerInsert.Dispose();
                }
                if (mirrorReplTopologySecPerInsertBase != null)
                {
                    mirrorReplTopologySecPerInsertBase.Close();
                    mirrorReplTopologySecPerInsertBase.Dispose();
                }
            }
        }

        public static void TempDBFileStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (tempDBFileStatsPerSecond != null)
                    tempDBFileStatsPerSecond.IncrementBy(count);

                if (tempDBFileStatSecPerInsert != null && tempDBFileStatSecPerInsertBase != null)
                {
                    tempDBFileStatSecPerInsert.IncrementBy(end - start);
                    tempDBFileStatSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating TempDB File Statistic counter: {0}", e);
                if (tempDBFileStatsPerSecond != null)
                {
                    tempDBFileStatsPerSecond.Close();
                    tempDBFileStatsPerSecond.Dispose();
                }
                if (tempDBFileStatSecPerInsert != null)
                {
                    tempDBFileStatSecPerInsert.Close();
                    tempDBFileStatSecPerInsert.Dispose();
                }
                if (tempDBFileStatSecPerInsertBase != null)
                {
                    tempDBFileStatSecPerInsertBase.Close();
                    tempDBFileStatSecPerInsertBase.Dispose();
                }
            }

        }

        public static void ReplicationStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (replicationPerSecond != null)
                    replicationPerSecond.IncrementBy(count);

                if (replicationSecPerInsert != null && replicationSecPerInsertBase != null)
                {
                    replicationSecPerInsert.IncrementBy(end - start);
                    replicationSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Replication Statistic counter: {0}", e);
                if (replicationPerSecond != null)
                {
                    replicationPerSecond.Close();
                    replicationPerSecond.Dispose();
                }
                if (replicationSecPerInsert != null)
                {
                    replicationSecPerInsert.Close();
                    replicationSecPerInsert.Dispose();
                }
                if (replicationSecPerInsertBase != null)
                {
                    replicationSecPerInsertBase.Close();
                    replicationSecPerInsertBase.Dispose();
                }
            }
        }

        public static void ReplicationSubscriberStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (replicationSubscriberPerSecond != null)
                    replicationSubscriberPerSecond.IncrementBy(count);

                if (replicationSubscriberSecPerInsert != null && replicationSubscriberSecPerInsertBase != null)
                {
                    replicationSubscriberSecPerInsert.IncrementBy(end - start);
                    replicationSubscriberSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Replication Subscriber Statistic counter: {0}", e);
                if (replicationSubscriberPerSecond != null)
                {
                    replicationSubscriberPerSecond.Close();
                    replicationSubscriberPerSecond.Dispose();
                }
                if (replicationSubscriberSecPerInsert != null)
                {
                    replicationSubscriberSecPerInsert.Close();
                    replicationSubscriberSecPerInsert.Dispose();
                }
                if (replicationSubscriberSecPerInsertBase != null)
                {
                    replicationSubscriberSecPerInsertBase.Close();
                    replicationSubscriberSecPerInsertBase.Dispose();
                }
            }

        }

        public static void ReplicationDistributorStatisticWritten(long start, long end, int count)
        {
            try
            {
                if (count == 0) return;

                if (replicationDistributorPerSecond != null)
                    replicationDistributorPerSecond.IncrementBy(count);

                if (replicationDistributorSecPerInsert != null && replicationDistributorSecPerInsertBase != null)
                {
                    replicationDistributorSecPerInsert.IncrementBy(end - start);
                    replicationDistributorSecPerInsertBase.IncrementBy(count);
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error updating Replication Distributor Statistic counter: {0}", e);
                if (replicationDistributorPerSecond != null)
                {
                    replicationDistributorPerSecond.Close();
                    replicationDistributorPerSecond.Dispose();
                }
                if (replicationDistributorSecPerInsert != null)
                {
                    replicationDistributorSecPerInsert.Close();
                    replicationDistributorSecPerInsert.Dispose();
                }
                if (replicationDistributorSecPerInsertBase != null)
                {
                    replicationDistributorSecPerInsertBase.Close();
                    replicationDistributorSecPerInsertBase.Dispose();
                }
            }
        }


        private static void DisposePerformanceCounterCategory(PerformanceCounterCategory category, String categoryName)
        {
            using (LOG.DebugCall("DisposePerformanceCounterCategory"))
            {
                if (category != null && PerformanceCounterCategory.Exists(categoryName))
                {                
                    PerformanceCounter[] performanceCounters = category.GetCounters();

                    foreach (var performanceCounter in performanceCounters)
                    {
                        performanceCounter.Close();
                        performanceCounter.Dispose();
                    }
                    LOG.DebugFormat("Deleting performance counter category: {0}", categoryName);
                    PerformanceCounterCategory.Delete(categoryName);
                }
            }
        }

        public static void Dispose()
        {
            using (LOG.DebugCall("Dispose"))
            {
                DisposePerformanceCounterCategory(category, PERFCOUNTER_CATEGORY);
            }
        }
    }
}
