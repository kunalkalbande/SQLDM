//------------------------------------------------------------------------------
// <copyright file="TableGrowthConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

  
    // Supress the warning regarding overrides
#pragma warning disable 0659
    /// <summary>
    /// Configuration object for Active Waits probe
    /// </summary>
    [Serializable]
    public class TableGrowthConfiguration : OnDemandConfiguration, IContinuousConfiguration
    {
        #region fields

        #endregion

        private int collectionTimeSeconds = 600;
        private DateTime startTimeUTC = DateTime.MinValue;
        private TimeSpan? runTime = TimeSpan.FromMinutes(3);
        private TimeSpan waitForPickupTime = TimeSpan.FromSeconds(600);
        private DateTime? growthStatisticsStartTime;

        private DateTime? lastGrowthStatisticsRunTime;
        private DateTime? previousGrowthStatisticsRunTime;
        private Int16? growthStatisticsDays;
        private List<string> tableStatisticsExcludedDatabases;
        private List<int> alreadyCollectedDatabases = new List<int>();

        private DateTime? timeStampLocal;
        private bool maxOrMin;
        private bool inRetry = false;

        #region constructors

        public TableGrowthConfiguration(int monitoredServerId)
            : base(monitoredServerId)
        {
        }


        public TableGrowthConfiguration(int monitoredServerId, TableGrowthConfiguration oldConfiguration, TableGrowthConfiguration newConfiguration)
            : base(monitoredServerId)
        {
            if (oldConfiguration != null)
            {
                ClientSessionId = oldConfiguration.ClientSessionId;
            }

            if (newConfiguration != null)
            {
                this.collectionTimeSeconds = newConfiguration.CollectionTimeSeconds;
                this.startTimeUTC = newConfiguration.StartTimeUTC;
                this.runTime = newConfiguration.RunTime;
                this.waitForPickupTime = newConfiguration.WaitForPickupTime;
                this.growthStatisticsStartTime = newConfiguration.GrowthStatisticsStartTime;
                this.timeStampLocal = newConfiguration.TimeStampLocal;
                this.TableStatisticsExcludedDatabases = newConfiguration.TableStatisticsExcludedDatabases;
            }
        }

      


        #endregion

        #region properties

        /// <summary>
        /// Indicates that this is the configuration object that should take preference in all combines
        /// </summary>
        public bool IsMaster
        {
            get;
            set;
        }

        public List<string> TableStatisticsExcludedDatabases
        {
            get
            {
                if (tableStatisticsExcludedDatabases == null)
                    tableStatisticsExcludedDatabases = new List<string>();

                return tableStatisticsExcludedDatabases;
            }
            set
            {
                if (value == null)
                    tableStatisticsExcludedDatabases = null;
                else
                    tableStatisticsExcludedDatabases = new List<string>(value);
            }
        }

        /// <summary>
        /// The length of the collector before it returns data to the collection service
        /// </summary>
        public int CollectionTimeSeconds
        {
            get { return collectionTimeSeconds; }
            set { collectionTimeSeconds = value; }
        }

        [Auditable(false)]
        new public bool ReadyForCollection
        {
            get
            {
                return (InCollectionWindow);
            }
        }

        [Auditable(false)]
        public bool InPickupWindow
        {
            get
            {
                if (ReadyForCollection)
                    return true;

                if (LastGrowthStatisticsRunTime.HasValue)
                {
                    if (TimeElapsedSinceShutdown.HasValue && TimeElapsedSinceShutdown <= waitForPickupTime)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// The time to start the collector
        /// </summary>
        public DateTime StartTimeUTC
        {
            get { return startTimeUTC; }
            set { startTimeUTC = value; }
        }

       /// <summary>
        /// The total length of time to run the collector
        /// Leave null for unlimited
        /// </summary>
        public TimeSpan? RunTime
        {
            get { return runTime; }
            set { runTime = value; }
        }

        //public DateTime? StopTimeUTC
        //{
        //    get
        //    {
        //        if (RunTime.HasValue)
        //        {
        //            return StartTimeUTC + RunTime;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public DateTime? TimeStampLocal
        {
            get { return timeStampLocal; }
            set { timeStampLocal = value; }
        }

        /// <summary>
        /// Returns whether the current time is within the collector window
        /// </summary>
        [Auditable("Ready for indefinite collection of query wait statistics")]
        public bool InCollectionWindow
        {
            get
            {
                if ((GrowthStatisticsStartTime.HasValue && TimeStampLocal.HasValue) &&
                    (MonitoredSqlServer.MatchDayOfWeek(TimeStampLocal.Value.DayOfWeek,
                                                     GrowthStatisticsDays)))
                {
                    if (
                        ((!LastGrowthStatisticsRunTime.HasValue) //If there is no known last refresh
                         ||
                         (LastGrowthStatisticsRunTime.Value.DayOfYear < TimeStampLocal.Value.DayOfYear)
                         //Or if the last refresh was yesterday
                         ||
                         (LastGrowthStatisticsRunTime.Value.Year < TimeStampLocal.Value.Year)
                         //Or if the last refresh fell in the last year
                         ||
                         (LastGrowthStatisticsRunTime.Value.TimeOfDay < GrowthStatisticsStartTime.Value.TimeOfDay)
                         //Or the stats time has been moved forward
                         ||
                         (inRetry) //Or we're in a retry period
                        )
                        &&
                        (TimeStampLocal.Value.TimeOfDay >= GrowthStatisticsStartTime.Value.TimeOfDay)
                        &&
                        (TimeStampLocal.Value.TimeOfDay <=
                         GrowthStatisticsStartTime.Value.TimeOfDay + TimeSpan.FromHours(3)))
                        //And the current server time is within the window
                    {
                        return true;
                    }
                }
                inRetry = false;
                alreadyCollectedDatabases.Clear();
                return false;
            }


        }

        /// <summary>
        /// Time remaining until scheduled collector shutdown
        /// </summary>
        public TimeSpan? TimeElapsedSinceShutdown
        {
            get
            {
                if (LastGrowthStatisticsRunTime.HasValue && TimeStampLocal.HasValue)
                    return TimeStampLocal.Value - LastGrowthStatisticsRunTime.Value;
                else
                    return null;
            }
        }

        public TimeSpan WaitForPickupTime
        {
            get { return waitForPickupTime; }
            set { waitForPickupTime = value; }
        }

        public DateTime? GrowthStatisticsStartTime
        {
            get { return growthStatisticsStartTime; }
            set { growthStatisticsStartTime = value; }
        }

        public DateTime? LastGrowthStatisticsRunTime
        {
            get { return lastGrowthStatisticsRunTime; }
            set { lastGrowthStatisticsRunTime = value; }
        }

        public DateTime? PreviousGrowthStatisticsRunTime
        {
            get { return previousGrowthStatisticsRunTime; }
            set { previousGrowthStatisticsRunTime = value; }
        }

        public short? GrowthStatisticsDays
        {
            get { return growthStatisticsDays; }
            set { growthStatisticsDays = value; }
        }

        public List<int> AlreadyCollectedDatabases
        {
            get { return alreadyCollectedDatabases; }
            set { alreadyCollectedDatabases = value; }
        }

        public bool MaxOrMin
        {
            get { return maxOrMin; }
            set { maxOrMin = value; }
        }

        public bool InRetry
        {
            get { return inRetry; }
            set { inRetry = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

     
        #endregion

        #region interface implementations

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TableGrowthConfiguration)) return false;
            return Equals((TableGrowthConfiguration) obj);
        }

        public bool Equals(TableGrowthConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.collectionTimeSeconds == collectionTimeSeconds && other.startTimeUTC.Equals(startTimeUTC) && other.runTime.Equals(runTime) && other.waitForPickupTime.Equals(waitForPickupTime) && other.growthStatisticsStartTime.Equals(growthStatisticsStartTime) && other.lastGrowthStatisticsRunTime.Equals(lastGrowthStatisticsRunTime) && other.previousGrowthStatisticsRunTime.Equals(previousGrowthStatisticsRunTime) && other.growthStatisticsDays.Equals(growthStatisticsDays) && Equals(other.tableStatisticsExcludedDatabases, tableStatisticsExcludedDatabases) && Equals(other.alreadyCollectedDatabases, alreadyCollectedDatabases) && other.timeStampLocal.Equals(timeStampLocal) && other.maxOrMin.Equals(maxOrMin) && other.inRetry.Equals(inRetry);
        }

        public IContinuousConfiguration CombineConfiguration(List<IContinuousConfiguration> configurations)
        {
            TableGrowthConfiguration returnConfig = null;
            foreach (TableGrowthConfiguration config in configurations)
            {
                if (returnConfig == null)
                {
                    returnConfig = config;
                }
                else
                {
                    if (config.ReadyForCollection)
                    {
                        if (config.CollectionTimeSeconds <
                            returnConfig.CollectionTimeSeconds)
                        {
                            returnConfig.CollectionTimeSeconds =
                                config.CollectionTimeSeconds;
                        }
                        if (returnConfig.RunTime.HasValue && (!config.RunTime.HasValue || config.RunTime > returnConfig.RunTime))
                        {
                            returnConfig.RunTime = config.RunTime;
                        }
                        if (config.TableStatisticsExcludedDatabases != null)
                        {
                            foreach(string s in config.TableStatisticsExcludedDatabases)
                            {
                                if (!returnConfig.TableStatisticsExcludedDatabases.Contains(s))
                                    returnConfig.TableStatisticsExcludedDatabases.Add(s);
                            }
                        }
                        if (config.GrowthStatisticsDays.HasValue)
                        {
                            if (!returnConfig.GrowthStatisticsDays.HasValue)
                            {
                                returnConfig.GrowthStatisticsDays = config.GrowthStatisticsDays;
                            }
                            else
                            {
                                returnConfig.GrowthStatisticsDays =
                                    (short)
                                    (returnConfig.GrowthStatisticsDays.Value | config.GrowthStatisticsDays.Value);
                            }
                        }
                        if (config.GrowthStatisticsStartTime.HasValue)
                        {
                            if ((!returnConfig.GrowthStatisticsStartTime.HasValue) ||  (config.GrowthStatisticsStartTime < returnConfig.GrowthStatisticsStartTime))
                            {
                                returnConfig.GrowthStatisticsStartTime = config.GrowthStatisticsStartTime;
                            }
                        }
                        
                         if (config.LastGrowthStatisticsRunTime.HasValue)
                        {
                            if ((!returnConfig.LastGrowthStatisticsRunTime.HasValue) ||  (config.LastGrowthStatisticsRunTime > returnConfig.LastGrowthStatisticsRunTime))
                            {
                                returnConfig.LastGrowthStatisticsRunTime = config.LastGrowthStatisticsRunTime;
                            }
                        }
                         if (config.PreviousGrowthStatisticsRunTime.HasValue)
                        {
                            if ((!returnConfig.PreviousGrowthStatisticsRunTime.HasValue) ||  (config.PreviousGrowthStatisticsRunTime > returnConfig.PreviousGrowthStatisticsRunTime))
                            {
                                returnConfig.PreviousGrowthStatisticsRunTime = config.PreviousGrowthStatisticsRunTime;
                            }
                        }
                        if (config.TimeStampLocal > returnConfig.TimeStampLocal)
                        {
                            returnConfig.TimeStampLocal = config.TimeStampLocal;
                        }
                        if (config.AlreadyCollectedDatabases.Count > 0)
                        {
                            foreach (int i in config.AlreadyCollectedDatabases)
                            {
                                if (!returnConfig.AlreadyCollectedDatabases.Contains(i))
                                    returnConfig.AlreadyCollectedDatabases.Add(i);
                            }
                        }
                    }
                }
            }
            return returnConfig;
        }


        #endregion

        #region nested types

        #endregion

    }
}
