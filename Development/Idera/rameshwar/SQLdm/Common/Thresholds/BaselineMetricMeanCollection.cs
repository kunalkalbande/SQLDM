//------------------------------------------------------------------------------
// <copyright file="BaselineMetricMeanCollection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//SQLdm 10.1 (Srishti Purohit)
//4.1.12 Revise Multiple Baseline for Independent Scheduling
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Thresholds
{
    using BBS.TracerX;
    using Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Class contains collection of Metric Mean respect to baseline tamplate for specific server.
    /// </summary>
    [Serializable]
    public class BaselineMetricMeanCollection
    {
        #region fields

        private const int defaultIndex = -1;
        private static readonly Logger LOG = Logger.GetLogger(typeof(BaselineMetricMean));
        private int serverId;
        private List<BaselineMetricMean> baselineMeanServerMeanList = new List<BaselineMetricMean>();

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaselineMetricMeanCollection"/> class.
        /// </summary>
        public BaselineMetricMeanCollection()
        {
            //If BaselineMetricMeanCollection is initialized using this constructor, serverId and connectionString needs to be set separately for the object
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaselineMetricMeanCollection"/> class.
        /// </summary>
        public BaselineMetricMeanCollection(int serverId)
        {
            this.serverId = serverId;
        }

        #endregion

        #region properties

        public int ServerId
        {
            get
            {
                return serverId;
            }

            set
            {
                serverId = value;
            }
        }

        public List<BaselineMetricMean> BaselineMeanServerMeanList
        {
            get
            {
                return baselineMeanServerMeanList;
            }

            set
            {
                baselineMeanServerMeanList = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods
        //public IDictionary<int, double?> GetBaselineMeanScheduled(int templateId)
        //{
        //    BaselineMetricMean meanMetric = new BaselineMetricMean();
        //    baselineMeanForMetric.TryGetValue(templateId, out meanMetric);
        //    return meanMetric.BaselineMetricMeanDic;
        //}

        public bool GetBaselineMeanScheduled(Snapshots.AlertableSnapshot refresh, int metricId, out double? baselineValueScheduled)
        {
            baselineValueScheduled = null;
            try
            {
                LOG.Verbose("Calling GetBaselineMeanScheduled.");
                if (refresh != null)
                {
                    DateTime? collectionTime = refresh.TimeStamp;
                    if (refresh is ScheduledRefresh)
                    {
                        LOG.Verbose("Processing ScheduledRefresh");
                    }
                    else
                    {
                        LOG.Verbose("snapshots.AlertableSnapshot is not convertable in ScheduledRefresh.");
                    }
                    baselineValueScheduled = getBaselineValueUsingCustomBaseline(collectionTime, metricId);
                    return true;
                }
                else
                {
                    LOG.Error("Alertable refresh object is null, so not able to Get BaselineMeanScheduled.");
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error while getting baseline using GetBaselineMeanScheduled. ", ex);
            }
            return false;
        }

        private double? getBaselineValueUsingCustomBaseline(DateTime? collectionTime, int metricId)
        {
            double? scheduledMean = null;
            BaselineMetricMean selectedBaselineMetricMean;
            try
            {
                if (collectionTime != null)
                {
                    DateTime collTimeNotNull = collectionTime ?? DateTime.Now;
                    LOG.VerboseFormat("For baseline alert metric {0}, server {1} Searching baseline corresponding to active scheduled custom template.", metricId, serverId);
                    selectedBaselineMetricMean = (from meanMetric in baselineMeanServerMeanList
                                                  where meanMetric.BaselineConfig.TemplateID != defaultIndex && meanMetric.BaselineConfig.Active == true && metricId == meanMetric.Metric && isTemplateScheduledMatch(meanMetric, metricId, collTimeNotNull) == true
                                                  select meanMetric).FirstOrDefault();
                    scheduledMean = selectedBaselineMetricMean != null ? selectedBaselineMetricMean.Mean : null;
                    
                    //Commenting this code As per discussion with Idera
                    //SQLdm10.1 (SQLdm) srishti purohit -- if no active custom basesline available then Default baseline will be checked
                    //if (scheduledMean == null)
                    //{
                    //    selectedBaselineMetricMean = (from meanMetric in baselineMeanServerMeanList
                    //                                  orderby meanMetric.UTCCalculationTime descending
                    //                                  where meanMetric.BaselineConfig.TemplateID != defaultIndex && meanMetric.BaselineConfig.Active == false && metricId == meanMetric.Metric && isTemplateScheduledMatch(meanMetric, metricId, collTimeNotNull) == true
                    //                                  select meanMetric).FirstOrDefault();
                    //    scheduledMean = selectedBaselineMetricMean != null ? selectedBaselineMetricMean.Mean : null;
                    //}
                    //LOG.InfoFormat("For baseline alert metric {0}, server {1} key not found in BaselineMetricMean for any custom template. Searching baseline corresponding to active default template.", metricId, serverId, collTimeNotNull);
                    if (scheduledMean == null)
                    {
                        LOG.VerboseFormat("For baseline alert metric {0}, server {1} key not found in BaselineMetricMean for any active custom template. Searching baseline corresponding to active default template.", metricId, serverId);
                        selectedBaselineMetricMean = (from meanMetric in baselineMeanServerMeanList
                                                      where meanMetric.BaselineConfig.TemplateID == defaultIndex && meanMetric.BaselineConfig.Active == true && metricId == meanMetric.Metric
                                                      select meanMetric).FirstOrDefault();
                        scheduledMean = selectedBaselineMetricMean != null ? selectedBaselineMetricMean.Mean : null;
                    }
                    
                    if (scheduledMean == null)
                    {
                        LOG.VerboseFormat("For baseline alert metric {0}, server {1} key not found in BaselineMetricMean for any active default template. Searching baseline corresponding to inactive default template.", metricId, serverId);
                        selectedBaselineMetricMean = (from meanMetric in baselineMeanServerMeanList
                                                      orderby meanMetric.UTCCalculationTime descending
                                                      where meanMetric.BaselineConfig.TemplateID == defaultIndex && meanMetric.BaselineConfig.Active == false && metricId == meanMetric.Metric
                                                      select meanMetric).FirstOrDefault();
                        scheduledMean = selectedBaselineMetricMean != null ? selectedBaselineMetricMean.Mean : null;
                    }
                    if (scheduledMean == null)
                    {
                        LOG.VerboseFormat("No baseline found in baselineMeanForMetric object for server : {0} and metric : {1}. Scheduled mean set to null.", serverId, metricId);
                    }
                    //findScheduledMean(sortedBaselines, metricId, collTimeNotNull, out scheduledMean);
                    //if (scheduledMean == null)
                    //{
                    //    LOG.WarnFormat("For baseline alert metric {0} key not found in BaselineMetricMean for any custom template. Searching baseline corresponding to default template.", metricId);
                    //    findDefaultMean(baselineMeanServerMeanList.Where(o => o.BaselineConfig.TemplateID == defaultIndex), metricId, out scheduledMean);
                    //}
                }
                else
                {
                    LOG.Error(" Refresh collection time is null.");
                    throw new Exception(" Refresh collection time is null.");
                }

            }
            catch (Exception ex)
            {
                LOG.Error("Error in selectTemplate method.", ex);
            }
            return scheduledMean;
        }
        //private void findScheduledMean(IEnumerable<BaselineMetricMean> baselineMeanDic, int metricId, DateTime collTimeNotNull, out double? scheduledmean)
        //{
        //    scheduledmean = null;
        //    foreach (BaselineMetricMean metricMeanObject in baselineMeanDic)
        //    {
        //        if (metricMeanObject.Metric == metricId)
        //        {
        //            DateTime startTimeUTC = metricMeanObject.BaselineConfig.Template.ScheduledStartDate.ToUniversalTime();
        //            DateTime endTimeUTC = metricMeanObject.BaselineConfig.Template.ScheduledEndDate.ToUniversalTime();
        //            bool isRangeCrossingMidnight = startTimeUTC.TimeOfDay > endTimeUTC.TimeOfDay;
        //            if (Data.BaselineHelpers.CheckTimeInRange(collTimeNotNull, startTimeUTC, endTimeUTC, isRangeCrossingMidnight))
        //            {
        //                if (checkDayOfWeek(collTimeNotNull, metricMeanObject.BaselineConfig.Template, isRangeCrossingMidnight))
        //                {
        //                    LOG.InfoFormat(metricMeanObject.BaselineConfig.TemplateID + " template is picked up as scheduled baseline to apply on server : {0} and metric : {1}.", serverId, metricId);

        //                    scheduledmean = metricMeanObject.Mean;
        //                    return;
        //                }
        //            }
        //        }
        //    }

        //    LOG.WarnFormat("No baseline found in baselineMeanForMetric object for server : {0} and metric : {1}.", serverId, metricId);

        //}

        
        /// <summary>
        /// Function checks if template is matching collection time as scheduled
        /// </summary>
        /// <param name="metricMeanObject"></param>
        /// <returns></returns>
        private bool isTemplateScheduledMatch(BaselineMetricMean metricMeanObject, int metricId, DateTime collTimeNotNull)
        {
            try
            {
                if (metricMeanObject.BaselineConfig.IsScheduledNotFound)
                {
                    LOG.VerboseFormat("Baseline Custom : No Schedule found for server : {0} and template id : {1}.", serverId, metricMeanObject.BaselineConfig.TemplateID);
                }
                else
                {
                    DateTime startTimeUTC = metricMeanObject.BaselineConfig.Template.ScheduledStartDate.Value.ToUniversalTime();
                    DateTime endTimeUTC = metricMeanObject.BaselineConfig.Template.ScheduledEndDate.Value.ToUniversalTime();
                    bool isRangeCrossingMidnight = startTimeUTC.TimeOfDay >= endTimeUTC.TimeOfDay;

                    if (Data.BaselineHelpers.CheckTimeInRange(collTimeNotNull, startTimeUTC, endTimeUTC, isRangeCrossingMidnight))
                    {
                        if (checkDayOfWeek(collTimeNotNull, metricMeanObject.BaselineConfig.GetDaysOfWeekAfterShiftScheduledBaseline(TimeSpan.Zero), isRangeCrossingMidnight))
                        {
                            LOG.VerboseFormat(metricMeanObject.BaselineConfig.TemplateID + " template is picked up as scheduled baseline to apply on server : {0} and metric : {1}.", serverId, metricId);

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error in selectTemplate method.", ex);
            }
            return false;
        }
        //private void findDefaultMean(IEnumerable<BaselineMetricMean> defaultServerMeanDic, int metricId, out double? scheduledMean)
        //{
        //    scheduledMean = null;
        //    //If not getting any schedule template then only use default template
        //    foreach (BaselineMetricMean meanObject in defaultServerMeanDic)
        //    {
        //        if (meanObject.Metric == metricId)
        //        {
        //            scheduledMean = meanObject.Mean;
        //            return;
        //        }
        //        else
        //        {
        //            LOG.WarnFormat("For baseline alert metric {0} key not found in BaselineMetricMean.", metricId);
        //        }
        //    }

        //    LOG.WarnFormat("No default baseline found in baselineMeanForMetric object for server : {0} and metric : {1}.", serverId, metricId);

        //}
        private bool checkDayOfWeek(DateTime collTimeNotNull, short dayShiftUTC, bool isRangeCrossingMidnight)
        {
            DayOfWeek dayOfWeek;
            // it gives you day of week of collection day
            if (isRangeCrossingMidnight)
                dayOfWeek = collTimeNotNull.DayOfWeek == DayOfWeek.Sunday ? DayOfWeek.Saturday : collTimeNotNull.DayOfWeek - 1;
            else
                dayOfWeek = collTimeNotNull.DayOfWeek;
            if (Objects.MonitoredSqlServer.MatchDayOfWeek(dayOfWeek, dayShiftUTC))
            {
                return true;
            }
            return false;
        }

        //private void sortBaselineMetricMeanList()
        //{
        //    baselineMeanServerMeanList = baselineMeanServerMeanList.OrderByDescending(x => x.BaselineConfig.Active).ThenByDescending(x => x.UTCCalculationTime).ToList();
        //}
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion       

    }

}
