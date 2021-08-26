//------------------------------------------------------------------------------
// <copyright file="TableFragmentationConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;


    // Supress the warning regarding overrides
#pragma warning disable 0659
    
    [Serializable]
    public class TableFragmentationConfiguration : OnDemandConfiguration, IContinuousConfiguration
    {
        #region fields

        #endregion

        private int collectionTimeSeconds = 600;
        private DateTime startTimeUTC = DateTime.MinValue;
        private TimeSpan? runTime = TimeSpan.FromMinutes(3);
        private TimeSpan waitForPickupTime = TimeSpan.FromSeconds(600);
        private DateTime? fragmentationStatisticsStartTime;

        private DateTime? presentFragmentationStatisticsRunTime;
        private DateTime? lastFragmentationStatisticsRunTime;
        private DateTime? previousFragmentationStatisticsRunTime;
        private Int16? fragmentationStatisticsDays;
        private List<string> tableStatisticsExcludedDatabases;

        private DateTime? timeStampLocal;
        private TableFragmentationCollectorStatus collectorStatus = TableFragmentationCollectorStatus.Stopped;

        private FileSize fragmentationMinimumTableSize = new FileSize(8000);

        private int order = 1;

        private bool forceFinish = false;

        #region constructors

        public TableFragmentationConfiguration(int monitoredServerId)
            : base(monitoredServerId)
        {
        }


        public TableFragmentationConfiguration(int monitoredServerId, TableFragmentationConfiguration oldConfiguration, TableFragmentationConfiguration newConfiguration)
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
                this.FragmentationStatisticsStartTime = newConfiguration.FragmentationStatisticsStartTime;
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

                if (LastFragmentationStatisticsRunTime.HasValue)
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

        public DateTime? TimeStampLocal
        {
            get { return timeStampLocal; }
            set { timeStampLocal = value; }
        }

        public DateTime? PresentFragmentationStatisticsRunTime
        {
            get { return presentFragmentationStatisticsRunTime; }
            set { presentFragmentationStatisticsRunTime = value; }
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public bool ForceFinish
        {
            get { return forceFinish; }
            set { forceFinish = value; }
        }
        /// <summary>
        /// Returns whether the current time is within the collector window
        /// </summary>
        [Auditable("Ready for indefinite collection of query wait statistics")]
        public bool InCollectionWindow
        {
            get
            {
                if (ForceFinish)
                    return false;

                if ((FragmentationStatisticsStartTime.HasValue && TimeStampLocal.HasValue) &&
                    (MonitoredSqlServer.MatchDayOfWeek(TimeStampLocal.Value.DayOfWeek,
                                                     FragmentationStatisticsDays)))
                {
                    if (
                        ((!LastFragmentationStatisticsRunTime.HasValue) //If there is no known last refresh
                         ||
                         (LastFragmentationStatisticsRunTime.Value.DayOfYear < TimeStampLocal.Value.DayOfYear)
                        //Or if the last refresh was yesterday
                         ||
                         (LastFragmentationStatisticsRunTime.Value.Year < TimeStampLocal.Value.Year)
                        //Or if the last refresh fell in the last year
                         ||
                         (LastFragmentationStatisticsRunTime.Value.TimeOfDay < FragmentationStatisticsStartTime.Value.TimeOfDay)
                        //Or the stats time has been moved forward
                         ||
                         (collectorStatus >= TableFragmentationCollectorStatus.Starting) //Or we're in a retry period
                        )
                        &&
                        (TimeStampLocal.Value.TimeOfDay >= FragmentationStatisticsStartTime.Value.TimeOfDay)
                        &&
                        (TimeStampLocal.Value.TimeOfDay <=
                         FragmentationStatisticsStartTime.Value.TimeOfDay + TimeSpan.FromHours(3)))
                    //And the current server time is within the window
                    {
                        return true;
                    }
                }
                
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
                if (LastFragmentationStatisticsRunTime.HasValue && TimeStampLocal.HasValue)
                    return TimeStampLocal.Value - LastFragmentationStatisticsRunTime.Value;
                else
                    return null;
            }
        }

        public TimeSpan WaitForPickupTime
        {
            get { return waitForPickupTime; }
            set { waitForPickupTime = value; }
        }

        public DateTime? FragmentationStatisticsStartTime
        {
            get { return fragmentationStatisticsStartTime; }
            set { fragmentationStatisticsStartTime = value; }
        }

        public DateTime? LastFragmentationStatisticsRunTime
        {
            get { return lastFragmentationStatisticsRunTime; }
            set { lastFragmentationStatisticsRunTime = value; }
        }

        public DateTime? PreviousFragmentationStatisticsRunTime
        {
            get { return previousFragmentationStatisticsRunTime; }
            set { previousFragmentationStatisticsRunTime = value; }
        }

        public short? FragmentationStatisticsDays
        {
            get { return fragmentationStatisticsDays; }
            set { fragmentationStatisticsDays = value; }
        }


        public TableFragmentationCollectorStatus CollectorStatus
        {
            get { return collectorStatus; }
            set { collectorStatus = value; }
        }

        public FileSize FragmentationMinimumTableSize
        {
            get { return fragmentationMinimumTableSize; }
            set { fragmentationMinimumTableSize = value; }
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
            if (obj.GetType() != typeof(TableFragmentationConfiguration)) return false;
            return Equals((TableFragmentationConfiguration)obj);
        }

        public bool Equals(TableFragmentationConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.collectionTimeSeconds == collectionTimeSeconds && other.startTimeUTC.Equals(startTimeUTC) &&
                   other.runTime.Equals(runTime) && other.waitForPickupTime.Equals(waitForPickupTime) &&
                   other.FragmentationStatisticsStartTime.Equals(FragmentationStatisticsStartTime) &&
                   other.lastFragmentationStatisticsRunTime.Equals(lastFragmentationStatisticsRunTime) &&
                   other.previousFragmentationStatisticsRunTime.Equals(previousFragmentationStatisticsRunTime) &&
                   other.FragmentationStatisticsDays.Equals(FragmentationStatisticsDays) &&
                   Equals(other.tableStatisticsExcludedDatabases, tableStatisticsExcludedDatabases)
                   && other.FragmentationMinimumTableSize == fragmentationMinimumTableSize;

            // Do not include these
            //other.timeStampLocal.Equals(timeStampLocal) && other.maxOrMin.Equals(maxOrMin) &&
            //other.collectorStatus.Equals(collectorStatus);
        }

        public bool EqualsLimited(TableFragmentationConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            bool a;
            if (other.fragmentationStatisticsStartTime.HasValue)
            {
                a = other.FragmentationStatisticsStartTime.Equals(FragmentationStatisticsStartTime);
            }
            else
            {
                a = other.FragmentationStatisticsStartTime.HasValue == FragmentationStatisticsStartTime.HasValue;
            }

            if (!a)
                return false;

            bool b;
            if (other.FragmentationStatisticsDays.HasValue)
            {
                b = other.FragmentationStatisticsDays.Equals(FragmentationStatisticsDays);
            }
            else
            {
                b = other.FragmentationStatisticsDays.HasValue == FragmentationStatisticsDays.HasValue;
            }

            if (!b)
                return false;

            bool c;
            if (other.TableStatisticsExcludedDatabases != null && TableStatisticsExcludedDatabases != null)
            {
                c = other.TableStatisticsExcludedDatabases.Count == TableStatisticsExcludedDatabases.Count;
            }
            else
            {
                c = other.TableStatisticsExcludedDatabases == null && TableStatisticsExcludedDatabases == null;
            }

            if (!c)
                return false;

            bool d = other.FragmentationMinimumTableSize == fragmentationMinimumTableSize;

            if (!d)
                return false;

            // Only do this last check if everything else is identical
            // Otherwise if the count of these two arrays is off it will cause a problem
            if (other.TableStatisticsExcludedDatabases != null && TableStatisticsExcludedDatabases != null && other.TableStatisticsExcludedDatabases.Count == TableStatisticsExcludedDatabases.Count)
            {
                for (int i = 0; i < other.TableStatisticsExcludedDatabases.Count; i++)
                {
                    if (other.TableStatisticsExcludedDatabases[i] != TableStatisticsExcludedDatabases[i])
                        c = false;
                }
            }

            return a && b && c && d;
        }


        public IContinuousConfiguration CombineConfiguration(List<IContinuousConfiguration> configurations)
        {
            TableFragmentationConfiguration returnConfig = null;
            foreach (TableFragmentationConfiguration config in configurations)
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
                            foreach (string s in config.TableStatisticsExcludedDatabases)
                            {
                                if (!returnConfig.TableStatisticsExcludedDatabases.Contains(s))
                                    returnConfig.TableStatisticsExcludedDatabases.Add(s);
                            }
                        }
                        if (config.FragmentationStatisticsDays.HasValue)
                        {
                            if (!returnConfig.FragmentationStatisticsDays.HasValue)
                            {
                                returnConfig.FragmentationStatisticsDays = config.FragmentationStatisticsDays;
                            }
                            else
                            {
                                returnConfig.FragmentationStatisticsDays =
                                    (short)
                                    (returnConfig.FragmentationStatisticsDays.Value | config.FragmentationStatisticsDays.Value);
                            }
                        }
                        if (config.FragmentationStatisticsStartTime.HasValue)
                        {
                            if ((!returnConfig.FragmentationStatisticsStartTime.HasValue) || (config.FragmentationStatisticsStartTime < returnConfig.FragmentationStatisticsStartTime))
                            {
                                returnConfig.FragmentationStatisticsStartTime = config.FragmentationStatisticsStartTime;
                            }
                        }

                        if (config.LastFragmentationStatisticsRunTime.HasValue)
                        {
                            if ((!returnConfig.LastFragmentationStatisticsRunTime.HasValue) || (config.LastFragmentationStatisticsRunTime > returnConfig.LastFragmentationStatisticsRunTime))
                            {
                                returnConfig.LastFragmentationStatisticsRunTime = config.LastFragmentationStatisticsRunTime;
                            }
                        }
                        if (config.PreviousFragmentationStatisticsRunTime.HasValue)
                        {
                            if ((!returnConfig.PreviousFragmentationStatisticsRunTime.HasValue) || (config.PreviousFragmentationStatisticsRunTime > returnConfig.PreviousFragmentationStatisticsRunTime))
                            {
                                returnConfig.PreviousFragmentationStatisticsRunTime = config.PreviousFragmentationStatisticsRunTime;
                            }
                        }
                        if (config.TimeStampLocal > returnConfig.TimeStampLocal)
                        {
                            returnConfig.TimeStampLocal = config.TimeStampLocal;
                        }
                       if (config.FragmentationMinimumTableSize.Kilobytes < returnConfig.FragmentationMinimumTableSize.Kilobytes)
                        {
                            returnConfig.FragmentationMinimumTableSize = config.FragmentationMinimumTableSize;
                        }
                    }
                }

                returnConfig.Order = config.Order + 1;
                if (returnConfig.Order > 10)
                {
                    returnConfig.Order = 1;
                }
            }

            

            return returnConfig;
        }


        #endregion

        #region nested types

        #endregion

    }
}
