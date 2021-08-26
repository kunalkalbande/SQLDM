//------------------------------------------------------------------------------
// <copyright file="BaselineHelpers.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Data
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Xml;
    using BBS.TracerX;
    using Configuration;
    using Events;
    using Microsoft.ApplicationBlocks.Data;
    using Thresholds;
    using Wintellect.PowerCollections;

    public static class BaselineHelpers
    {
        private static readonly Logger LOG = Logger.GetLogger(typeof(BaselineHelpers));
        private const string ITEMID_START_TAG = "ItemID_";

        private const string GetBaselineTemplatesByIdStoredProcedure = "p_GetBaselineTemplatesById";

        public static void GetEarliestAvailableData(SqlConnection connection, int sqlServerID, out DateTime? earliestDataAvailable)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "p_GetEarliestDataAvailable";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@SqlServerId", sqlServerID);

                SqlParameter parmEarliestData = command.Parameters.Add("@EarliestStatisticsAvailable", SqlDbType.DateTime);
                parmEarliestData.Direction = ParameterDirection.InputOutput;
                parmEarliestData.Value = DBNull.Value;

                command.ExecuteNonQuery();

                SqlDateTime sqlDateTime = (SqlDateTime)parmEarliestData.SqlValue;

                if (sqlDateTime.IsNull)
                    earliestDataAvailable = null;
                else
                    earliestDataAvailable = sqlDateTime.Value;
            }
        }

        /// <summary>
        /// Retrieves baseline configuration settings.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlServerID"></param>
        /// <param name="useDefaults"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="selectedDays"></param>
        /// <param name="earliestDataAvailable"></param>
        /// <returns>true if the baseline has been set or false if it has not.</returns>
        public static bool GetBaselineParameters(SqlConnection connection, int sqlServerID, out bool useDefaults, out DateTime startDate, out DateTime endDate, out short selectedDays, out DateTime? earliestDataAvailable)
        {
            bool baselineSet = true;
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "p_GetBaselineParameters";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SQLServerID", sqlServerID);
                SqlParameter parmUseDefaults = command.Parameters.Add("@UseDefaults", SqlDbType.Bit);
                parmUseDefaults.Direction = ParameterDirection.InputOutput;
                parmUseDefaults.Value = DBNull.Value;
                SqlParameter parmStartDate = command.Parameters.Add("@StartDate", SqlDbType.DateTime);
                parmStartDate.Direction = ParameterDirection.InputOutput;
                parmStartDate.Value = DBNull.Value;
                SqlParameter parmEndDate = command.Parameters.Add("@EndDate", SqlDbType.DateTime);
                parmEndDate.Direction = ParameterDirection.InputOutput;
                parmEndDate.Value = DBNull.Value;
                SqlParameter parmDays = command.Parameters.Add("@Days", SqlDbType.TinyInt);
                parmDays.Direction = ParameterDirection.InputOutput;
                parmDays.Value = DBNull.Value;
                SqlParameter parmEarliestData = command.Parameters.Add("@EarliestStatisticsAvailable", SqlDbType.DateTime);
                parmEarliestData.Direction = ParameterDirection.InputOutput;
                parmEarliestData.Value = DBNull.Value;

                command.ExecuteNonQuery();

                SqlBoolean sqlBoolean = (SqlBoolean)parmUseDefaults.SqlValue;
                useDefaults = (sqlBoolean.IsNull) ? true : sqlBoolean.Value;

                SqlDateTime sqlDateTime = (SqlDateTime)parmEndDate.SqlValue;
                if (sqlDateTime.IsNull)
                {
                    endDate = DateTime.Now.Date + TimeSpan.FromHours(17);
                    baselineSet = false;
                }
                else
                    endDate = sqlDateTime.Value;

                sqlDateTime = (SqlDateTime)parmStartDate.SqlValue;
                if (sqlDateTime.IsNull)
                {
                    startDate = (endDate - TimeSpan.FromDays(7)).Date + TimeSpan.FromHours(8);
                    baselineSet = false;
                }
                else
                    startDate = sqlDateTime.Value;

                SqlByte sqlByte = (SqlByte)parmDays.SqlValue;
                selectedDays = (sqlByte.IsNull) ? (short)0x7C : sqlByte.Value;

                sqlDateTime = (SqlDateTime)parmEarliestData.SqlValue;
                if (sqlDateTime.IsNull)
                    earliestDataAvailable = null;
                else
                    earliestDataAvailable = sqlDateTime.Value;
            }
            return baselineSet;
        }

        public static void SetBaselineParameters(SqlConnection connection, int instanceId, bool useDefaults, DateTime startDate, DateTime endDate, short days)
        {
            SqlHelper.ExecuteNonQuery(connection, "p_SetBaselineParameters", instanceId, useDefaults, startDate, endDate, days);
        }

        public static List<BaselineItemData> GetBaseline(SqlConnection connection, int SQLServerID, bool alertableOnly)
        {
            Dictionary<int, BaselineMetaDataItem> metaData = new Dictionary<int, BaselineMetaDataItem>();
            foreach (BaselineMetaDataItem metaDataItem in GetBaselineMetaData(connection))
            {
                if (alertableOnly && metaDataItem.MetricId == null)
                    continue;

                metaData.Add(metaDataItem.ItemId, metaDataItem);
            }

            return ExecuteBaselineQuery(connection, SQLServerID, alertableOnly, DateTime.Now, metaData);
        }

        public static List<BaselineItemData> ExecuteBaselineQuery(SqlConnection connection, int SQLServerID, bool alertableOnly, DateTime collectionDateTime, Dictionary<int, BaselineMetaDataItem> metaData)
        {
            return ExecuteBaselineQuery(connection, SQLServerID, alertableOnly, collectionDateTime, metaData, null);
        }

        public static List<BaselineItemData> ExecuteBaselineQuery(SqlConnection connection, int SQLServerID, bool alertableOnly, DateTime collectionDateTime, Dictionary<int, BaselineMetaDataItem> metaData, BaselineTemplate template)
        {
            List<BaselineItemData> result = new List<BaselineItemData>();
            string selectQuery = CreateBaselineQuery(connection, metaData.Values, template);

            LOG.Debug("BaselineQuery: " + string.Format("declare @SQLServerID int  set @SQLServerID = {0}  {1}", SQLServerID, selectQuery));

            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@SQLServerID", SQLServerID);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    do
                    {
                        while (dataReader.Read())
                        {
                            string colName = dataReader.GetName(0);
                            if (colName.Equals("CustomCounterID"))
                            {
                                int itemId = dataReader.GetInt32(0);
                                BaselineMetaDataItem item;
                                if (metaData.TryGetValue(itemId, out item))
                                {
                                    if (alertableOnly && item.MetricId == null)
                                        continue;
                                    BaselineItemData data = new BaselineItemData(item);
                                    data.CollectionDateTime = collectionDateTime;
                                    if ("DeltaValue".Equals(item.MetricValue))
                                    {
                                        data.SetAverage(dataReader.GetValue(dataReader.GetOrdinal("AverageDelta")));
                                        data.SetDeviation(dataReader.GetValue(dataReader.GetOrdinal("DeviationDelta")));
                                        data.SetCount(dataReader.GetValue(dataReader.GetOrdinal("CountDelta")));
                                        data.SetMaximum(dataReader.GetValue(dataReader.GetOrdinal("MaxDelta")));
                                        data.SetMinimum(dataReader.GetValue(dataReader.GetOrdinal("MinDelta")));
                                    }
                                    else
                                    {
                                        data.SetAverage(dataReader.GetValue(dataReader.GetOrdinal("AverageRaw")));
                                        data.SetDeviation(dataReader.GetValue(dataReader.GetOrdinal("DeviationRaw")));
                                        data.SetCount(dataReader.GetValue(dataReader.GetOrdinal("CountRaw")));
                                        data.SetMaximum(dataReader.GetValue(dataReader.GetOrdinal("MaxRaw")));
                                        data.SetMinimum(dataReader.GetValue(dataReader.GetOrdinal("MinRaw")));
                                    }

                                    result.Add(data);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < dataReader.FieldCount; i++)
                                {
                                    colName = dataReader.GetName(i);
                                    if (!colName.StartsWith(ITEMID_START_TAG))
                                        continue;
                                    int itemId;
                                    if (!Int32.TryParse(colName.Remove(0, ITEMID_START_TAG.Length), out itemId))
                                        continue;
                                    BaselineMetaDataItem item;
                                    if (metaData.TryGetValue(itemId, out item))
                                    {
                                        if (alertableOnly && item.MetricId == null)
                                            continue;

                                        BaselineItemData data = new BaselineItemData(item);
                                        data.CollectionDateTime = collectionDateTime;
                                        data.SetMaximum(dataReader.GetValue(i));
                                        data.SetAverage(dataReader.GetValue(i + 1));
                                        data.SetDeviation(dataReader.GetValue(i + 2));
                                        data.SetCount(dataReader.GetValue(i + 3));
                                        data.SetMinimum(dataReader.GetValue(i + 4));

                                        result.Add(data);
                                    }
                                }
                            }
                        }
                    } while (dataReader.NextResult());
                }
            }

            return result;
        }

        public static string CreateBaselineQuery(SqlConnection connection, IEnumerable<BaselineMetaDataItem> metaData, BaselineTemplate template)
        {
            using (LOG.InfoCall("CreateBaselineQuery"))
            {
                StringBuilder result = new StringBuilder();
                StringBuilder column = new StringBuilder();
                string groupby = null;

                bool customCountersAdded = false;
                int marker = result.Length;
                string lastStatisticTable = null;
                foreach (BaselineMetaDataItem item in metaData)
                {
                    if (String.IsNullOrEmpty(item.StatisticTable))
                        continue;

                    if (lastStatisticTable == null)
                    {
                        lastStatisticTable = item.StatisticTable;
                        result.AppendLine("declare @BeginUTC datetime, @EndUTC datetime");
                        result.AppendLine("declare @StartDate datetime, @EndDate datetime");
                        result.AppendLine("declare @StartTime datetime, @EndTime datetime");
                        result.AppendLine("declare @EarliestAvailableData datetime");
                        result.AppendLine("declare @Sun bit, @Mon bit, @Tue bit, @Wed bit, @Thu bit, @Fri bit, @Sat bit");
                        result.AppendLine("declare @UseDefaults bit, @Days tinyint");

                        // PR14404 - disable arithmetic overflow exceptions
                        result.AppendLine("set arithabort off");
                        result.AppendLine("set ansi_warnings off");
                        result.AppendLine("exec p_GetBaselineParameters @SQLServerID, @UseDefaults output, @BeginUTC output, @EndUTC output, @Days output, @EarliestAvailableData output");

                        // override the values if the template is available
                        
                        if (template != null)
                        {
                            result.AppendLine(string.Format("set @UseDefaults = {0}", template.UseDefault ? 1 : 0));
                            result.AppendLine(string.Format("set @BeginUTC = '{0}'", template.CalculationStartDate.ToUniversalTime()));
                            result.AppendLine(string.Format("set @EndUTC = '{0}'", template.CalculationEndDate.ToUniversalTime()));
                            //SQLDM 10.1(Pulkit Puri)--shift calulation days according to utc
                            result.AppendLine(string.Format("set @Days = {0}", template.GetShiftedUTCCalculationDays()));
                        }

                        result.AppendLine("if (@UseDefaults = 1)");
                        result.AppendLine("begin");
                        if (template != null)
                        {
                            result.AppendLine(string.Format("   set @StartTime = '{0}'",template.CalculationStartDate.ToUniversalTime()));
                            result.AppendLine(string.Format("   set @EndTime = '{0}'", template.CalculationEndDate.ToUniversalTime()));
                        }
                        else
                        {
                            result.AppendLine("   set @StartTime = @BeginUTC");
                            result.AppendLine("   set @EndTime = @EndUTC");
                        }
                        result.AppendLine("   select @EndUTC = GetUtcDate()");
                        result.AppendLine("   select @BeginUTC = dateadd(day,-7,@EndUTC)");
                        result.AppendLine("   set @StartDate = @BeginUTC");
                        result.AppendLine("   set @EndDate = @EndUTC");
                        result.AppendLine("   set @Sun = case when (@Days & 1) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Mon = case when (@Days & 4) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Tue = case when (@Days & 8) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Wed = case when (@Days & 16) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Thu = case when (@Days & 32) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Fri = case when (@Days & 64) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Sat = case when (@Days & 128) > 0 then 1 else 0 end");
                        result.AppendLine("end");
                        result.AppendLine("else");
                        result.AppendLine("begin");
                        result.AppendLine("   set @StartDate = @BeginUTC");
                        result.AppendLine("   set @EndDate = @EndUTC");
                        result.AppendLine("   set @StartTime = @BeginUTC");
                        result.AppendLine("   set @EndTime = @EndUTC");
                        result.AppendLine("   set @Sun = case when (@Days & 1) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Mon = case when (@Days & 4) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Tue = case when (@Days & 8) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Wed = case when (@Days & 16) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Thu = case when (@Days & 32) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Fri = case when (@Days & 64) > 0 then 1 else 0 end");
                        result.AppendLine("   set @Sat = case when (@Days & 128) > 0 then 1 else 0 end");
                        result.AppendLine("end");

                        // build temp table to hold list of custom counters assigned to this server 
                        result.AppendLine("declare @CustomCounters table (SQLServerID int, Metric int)");

                        result.AppendLine("insert into @CustomCounters");
                        result.AppendLine("select SQLServerID, Metric");
                        result.AppendLine("    from [CustomCounterMap] CCM");
                        result.AppendLine("    where CCM.[SQLServerID] = @SQLServerID ");

                        result.AppendLine("insert into @CustomCounters");
                        result.AppendLine("    select distinct @SQLServerID, CCT.[Metric]");
                        result.AppendLine("        from [CustomCounterTags] CCT");
                        result.AppendLine("        join [ServerTags] ST on ST.TagId = CCT.TagId");
                        result.AppendLine("    where ST.[SQLServerId] = @SQLServerID");
                        result.AppendLine("        and CCT.[Metric] not in");
                        result.AppendLine("           (select Metric from [CustomCounterMap] where [SQLServerID] = @SQLServerID)");

                        result.AppendLine("Select ");
                        marker = result.Length;
                    }
                    else
                        if (!lastStatisticTable.Equals(item.StatisticTable))
                    {
                        // append end of select statement
                        result.AppendLine();
                        result.AppendFormat(" from {0}", lastStatisticTable);
                        result.AppendLine(" ");
                        result.AppendLine("where S.SQLServerID = @SQLServerID ");
                        // next 2 lines should help the query subset the number of rows using an index
                        result.AppendLine("and UTCCollectionDateTime >= @BeginUTC ");
                        result.AppendLine("and UTCCollectionDateTime <= @EndUTC ");
                        // next line implements date,time,day of week selection
                        result.AppendLine("and 1 = dbo.fn_CompareDateTimeRange(UTCCollectionDateTime,@StartDate,@EndDate,@StartTime,@EndTime,@Sun,@Mon,@Tue,@Wed,@Thu,@Fri,@Sat)");

                        if (groupby != null)
                        {
                            result.AppendLine(groupby);
                            groupby = null;
                        }
                        // start the next select statement
                        result.AppendLine();
                        result.AppendLine("Select ");
                        marker = result.Length;
                        lastStatisticTable = item.StatisticTable;
                    }

                    //bool customCounter = false;
                    //if (item.MetricId.HasValue) 
                    //{

                    //    //Metric metric = MetricDefinition.GetMetric(item.MetricId.Value);
                    //    //customCounter = metric == Metric.Custom; // since custom counters seem not to be supported anyway

                    //    customCounter = item.MetricId.Value >= 1000; // This is stupid but has to be done this way because we have pigeon holed ourselves
                    //                                                 //   because metricIDs are used for different things and sometimes have different values... 
                    //                                                 //   the whole alert/metric ID BS needs to be completely redesigned and rewritten.
                    //}

                    var customCounter = (item.MetricId.HasValue && item.MetricId.Value >= 1000) ? true : false;

                    if (!customCountersAdded && customCounter)
                    {
                        result.AppendLine(" MetricID as [CustomCounterID],");
                        result.AppendLine("avg(RawValue) as [AverageRaw],");
                        result.AppendLine("stdev(RawValue) as [DeviationRaw],");
                        result.AppendLine("count(RawValue) as [CountRaw],");
                        result.AppendLine("max(RawValue) as [MaxRaw],");
                        result.AppendLine("avg(DeltaValue) as [AverageDelta],");
                        result.AppendLine("stdev(DeltaValue) as [DeviationDelta],");
                        result.AppendLine("count(DeltaValue) as [CountDelta],");
                        result.AppendLine("max(DeltaValue) as [MaxDelta],");
                        result.AppendLine("min(RawValue) as [MinRaw],");
                        result.AppendLine("min(DeltaValue) as [MinDelta]");
                        groupby = " group by [MetricID]";
                        customCountersAdded = true;
                    }

                    if (customCounter)
                        continue;

                    // ensure we are starting off with a blank string
                    column.Length = 0;
                    column.AppendFormat("max({0}) as [{2}{1}], avg({0}) as [Average_{1}], stdev({0}) as [Stddev_{1}], count({0}) as [Count_{1}], min({0}) as [Min_{1}]", item.MetricValue, item.ItemId, ITEMID_START_TAG);

                    if (result.Length > marker)
                        result.AppendLine(",");
                    result.Append(column.ToString());
                }

                if (result.Length > marker)
                {
                    if (lastStatisticTable != null)
                    {
                        // append end of select statement
                        result.AppendLine();
                        result.AppendFormat("from {0}", lastStatisticTable);
                        result.AppendLine(" ");
                        result.AppendLine("where S.SQLServerID = @SQLServerID ");
                        // next 2 lines should help the query subset the number of rows using an index
                        result.AppendLine("and UTCCollectionDateTime >= @BeginUTC ");
                        result.AppendLine("and UTCCollectionDateTime <= @EndUTC ");
                        // next line implements date,time,day of week selection
                        result.AppendLine("and 1 = dbo.fn_CompareDateTimeRange(UTCCollectionDateTime,@StartDate,@EndDate,@StartTime,@EndTime,@Sun,@Mon,@Tue,@Wed,@Thu,@Fri,@Sat)");
                    }
                }

                return result.ToString();
            }
        }

        public static List<BaselineMetaDataItem> GetBaselineMetaData(SqlConnection connection)
        {
            using (LOG.InfoCall("GetBaselineMetaData"))
            {
                List<BaselineMetaDataItem> items = new List<BaselineMetaDataItem>();

                try
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, "p_GetBaselineMetaData", new object[0]))
                    {
                        while (dataReader.Read())
                        {
                            items.Add(new BaselineMetaDataItem(dataReader));
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Exception loading baseline metadata items: ", e);
                    throw;
                }

                return items;
            }
        }

        public static Dictionary<int, BaselineConfiguration> GetCustomBaselines(int instanceId, string connectionString)
        {
            Dictionary<int, BaselineConfiguration> baselineConfigurationList = new Dictionary<int, BaselineConfiguration>();
            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(connectionString, GetBaselineTemplatesByIdStoredProcedure, instanceId))
            {
                while (reader.Read())
                {
                    BaselineConfiguration config = new BaselineConfiguration();
                    config = new BaselineConfiguration((string)reader["Template"]);
                    config.TemplateID = (int)reader["TemplateID"];
                    config.Active = (bool)reader["Active"];
                    config.BaselineName = reader["BaselineName"].ToString();
                    config.IsChanged = false;
                    baselineConfigurationList.Add(config.TemplateID, config);
                }
            }
            return baselineConfigurationList;
        }


        /// <summary>
        /// Retrieves baseline configuration settings.
        /// Checking time range if Time is crossing mid night
        /// SQLdm(10.1) Srishti Purohit
        /// </summary>
        /// <param name="timeToCompare"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public static bool CheckTimeInRange(DateTime timeToCompare, DateTime startTime, DateTime endTime, bool isRangeCrossingMidnight)
        {
            bool isTimeInRange = false;
            try
            {
                isTimeInRange = startTime.TimeOfDay <= timeToCompare.TimeOfDay && endTime.TimeOfDay >= timeToCompare.TimeOfDay;
                if (!isTimeInRange)
                {
                    if (isRangeCrossingMidnight)
                        isTimeInRange = startTime.TimeOfDay <= timeToCompare.TimeOfDay || timeToCompare.TimeOfDay <= endTime.TimeOfDay;
                    else
                        isTimeInRange = false;
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error in CheckTimeInRange function : " + ex);
            }
            return isTimeInRange;
        }

        public static Dictionary<int, BaselineItemData> ToDictionary(IEnumerable<BaselineItemData> list)
        {
            if (list == null)
                return null;

            Dictionary<int, BaselineItemData> result = new Dictionary<int, BaselineItemData>();
            foreach (BaselineItemData item in list)
            {
                result.Add(item.GetMetaData().ItemId, item);
            }
            return result;
        }

        #region SQL Helper Methods
        public static String GetString(SqlDataReader reader, int ordinal, string nullValue)
        {
            if (reader.IsDBNull(ordinal))
                return nullValue;

            return reader.GetString(ordinal);
        }

        public static String GetString(SqlDataReader reader, string columnName, string nullValue)
        {
            return GetString(reader, reader.GetOrdinal(columnName), nullValue);
        }

        public static int? GetInt(SqlDataReader reader, int ordinal, int? nullValue)
        {
            if (reader.IsDBNull(ordinal))
                return nullValue;

            return reader.GetInt32(ordinal);
        }

        public static int? GetInt(SqlDataReader reader, string columnName, int? nullValue)
        {
            return GetInt(reader, reader.GetOrdinal(columnName), nullValue);
        }

        public static long? GetLong(SqlDataReader reader, int ordinal, long? nullValue)
        {
            if (reader.IsDBNull(ordinal))
                return nullValue;

            return reader.GetInt64(ordinal);
        }

        public static long? GetLong(SqlDataReader reader, string columnName, long? nullValue)
        {
            return GetLong(reader, reader.GetOrdinal(columnName), nullValue);
        }

        public static decimal? GetDecimal(SqlDataReader reader, int ordinal, decimal? nullValue)
        {
            if (reader.IsDBNull(ordinal))
                return nullValue;

            return reader.GetDecimal(ordinal);
        }

        public static decimal? GetDecimal(SqlDataReader reader, string columnName, decimal? nullValue)
        {
            return GetDecimal(reader, reader.GetOrdinal(columnName), nullValue);
        }

        public static DateTime? GetDateTime(SqlDataReader reader, int ordinal, DateTime? nullValue)
        {
            if (reader.IsDBNull(ordinal))
                return nullValue;

            return reader.GetDateTime(ordinal);
        }

        public static DateTime? GetDateTime(SqlDataReader reader, string columnName, DateTime? nullValue)
        {
            return GetDateTime(reader, reader.GetOrdinal(columnName), nullValue);
        }

        /// <summary>
        /// To get Custom Baseline Templates
        /// SQLdm 10.1 (srishti purohit)
        /// Revised Baseline requirement
        /// </summary>
        public static Dictionary<int, BaselineConfiguration> GetBaselineDictionaryFromArray(string[] arrayOfCustomBaselines)
        {
            Dictionary<int, BaselineConfiguration> customBaselineItems = new Dictionary<int, BaselineConfiguration>();
            try
            {
                for (int indexSeq = 0; indexSeq < arrayOfCustomBaselines.Length; indexSeq = indexSeq + 3)
                {
                    if (indexSeq % 3 == 0)
                    {
                        if (!String.IsNullOrEmpty(arrayOfCustomBaselines[indexSeq + 1]))
                        {
                            BaselineConfiguration baseline = new BaselineConfiguration(((string)arrayOfCustomBaselines[indexSeq + 1]).Trim());
                            baseline.TemplateID = Convert.ToInt32(arrayOfCustomBaselines[indexSeq]);
                            baseline.BaselineName = ((string)arrayOfCustomBaselines[indexSeq + 2]).Trim();
                            baseline.Active = true;
                            customBaselineItems.Add(baseline.TemplateID, baseline);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message + " Error while getting custom baselines.");
                throw new Exception("Error while getting custom baselines. " + ex.Message);
            }
            return customBaselineItems;
        }

        #endregion

        public static List<BaselineItemData> FilterRecommendations(AlertConfiguration configuration, IEnumerable<BaselineItemData> baselineData)
        {
            List<BaselineItemData> recommendations = new List<BaselineItemData>();

            foreach (BaselineItemData item in baselineData)
            {
                // get the alert configuration item for the metric
                int metricId = item.GetMetaData().MetricId.Value;
                AlertConfigurationItem aci = configuration[metricId, String.Empty];
                if (aci == null)
                {
                    LOG.WarnFormat("Skipping alert - configuration not found: {0} - {1} ", metricId, item.Name);
                    continue;
                }

                // skip disabled alerts

                if (!aci.Enabled)
                {
                    LOG.VerboseFormat("Skipping disabled alert config item: {0}", aci.Name);
                    continue;
                }

                Threshold threshold = aci.GetThreshold(ThresholdItemType.Warning, false);

                // skip alerts that are disabled because both warning and critical thresholds are disabled
                if (!threshold.Enabled)
                {
                    threshold = aci.GetThreshold(ThresholdItemType.Critical, false);
                    if (!threshold.Enabled)
                    {
                        LOG.VerboseFormat("Skipping disabled alert config item: {0}", aci.Name);
                        continue;
                    }
                }

                // skip baseline items that had fewer than the mim number of required data points
                if (item.Count < BaselineItemData.MINIMUM_ITEM_COUNT_TO_BE_USEFUL)
                {
                    LOG.VerboseFormat("Skipping sparse baseline item {0} count={1}", aci.Name, item.Count);
                    continue;
                }

                // this shouldn't happen but...
                if (!(threshold.Value is IComparable))
                {
                    LOG.VerboseFormat(
                        "Skipping alert config item (threshold not IComparable): {0}: {1}", aci.Name,
                        threshold.Value);
                    continue;
                }

                decimal? testValue =
                    aci.GetMetaData().ComparisonType == ComparisonType.GE
                        ? item.ReferenceRangeEnd
                        : item.ReferenceRangeStart;

                // add the item if the high end of the ref range will generate an alert
                if (testValue != null &&
                    threshold.IsInViolation((IComparable)testValue.Value))
                {
                    recommendations.Add(item);
                }
                else
                {
                    LOG.VerboseFormat(
                        "Skipping alert config item because ref range values will not create an alert range={0}",
                        item.GetDisplayString());
                }
            }
            return recommendations;
        }
    }


    public class BaselineItemData
    {
        // minimum number of rows of collected data before a baseline item can be considered useful
        public const int MINIMUM_ITEM_COUNT_TO_BE_USEFUL = 5;

        private readonly BaselineMetaDataItem metaData;
        private DateTime collectionDateTime;
        private decimal? average;
        private decimal? deviation;
        private decimal? maximum;
        private decimal? minimum;
        private long? count;

        public BaselineItemData(BaselineMetaDataItem metaData)
        {
            this.metaData = metaData;
        }

        public BaselineMetaDataItem GetMetaData()
        {
            return metaData;
        }

        public string Name
        {
            get { return metaData.Name; }
        }

        public string Description
        {
            get { return metaData.Name; }
        }

        public string Category
        {
            get { return metaData.Name; }
        }

        public string Unit
        {
            get { return metaData.Unit; }
        }

        public DateTime CollectionDateTime
        {
            get { return collectionDateTime; }
            set { collectionDateTime = value; }
        }

        public decimal? Average
        {
            get
            {
                decimal? result = average;
                // apply scaling
                if (result.HasValue && metaData.Scale.HasValue)
                    result = result.Value * metaData.Scale.Value;
                // round off to the number of specified decimal places
                if (result.HasValue && metaData.Decimals.HasValue)
                    result = Math.Round(result.Value, metaData.Decimals.Value);

                return result;
            }
        }

        public decimal? GetAverageUnscaled()
        {
            return average;
        }

        public decimal? Deviation
        {
            get
            {
                decimal? result = deviation;
                // apply scaling
                if (result.HasValue && metaData.Scale.HasValue)
                    result = result.Value * metaData.Scale.Value;
                // round off to the number of specified decimal places
                if (result.HasValue && metaData.Decimals.HasValue)
                    result = Math.Round(result.Value, metaData.Decimals.Value);
                return result;
            }
        }

        public decimal? GetDeviationUnscaled()
        {
            return deviation;
        }

        public long? Count
        {
            get { return count; }
            set { count = value; }
        }

        public decimal? Maximum
        {
            get { return maximum; }
        }

        public decimal? Minimum
        {
            get { return minimum; }
        }

        public decimal? ReferenceRangeStart
        {
            get
            {
                decimal? result = average - deviation;
                // scale the value
                if (result.HasValue && metaData.Scale.HasValue)
                    result = result.Value * metaData.Scale.Value;
                // round off to the number of specified decimal places
                if (result.HasValue && metaData.Decimals.HasValue)
                    result = Math.Round(result.Value, metaData.Decimals.Value);
                // apply the lower limit
                if (metaData.LowerLimit.HasValue && metaData.LowerLimit.Value > result)
                    result = metaData.LowerLimit.Value;
                return result;
            }
        }

        public decimal? ReferenceRangeEnd
        {
            get
            {
                decimal? result = average + deviation;
                // scale the value
                if (result.HasValue && metaData.Scale.HasValue)
                    result = result.Value * metaData.Scale.Value;
                // round off to the number of specified decimal places
                if (result.HasValue && metaData.Decimals.HasValue)
                    result = Math.Round(result.Value, metaData.Decimals.Value);
                // apply the upper limit
                if (metaData.UpperLimit.HasValue && metaData.UpperLimit.Value < result)
                    result = metaData.UpperLimit.Value;
                return result;
            }
        }

        internal void SetAverage(object p)
        {
            if (p == DBNull.Value)
                average = null;
            else
                if (p is decimal)
                average = (decimal)p;
            else
                average = (decimal)Convert.ChangeType(p, typeof(decimal));
        }

        internal void SetMaximum(object p)
        {
            if (p == DBNull.Value)
                maximum = null;
            else
                if (p is decimal)
                maximum = (decimal)p;
            else
                maximum = (decimal)Convert.ChangeType(p, typeof(decimal));
        }

        internal void SetMinimum(object p)
        {
            if (p == DBNull.Value)
                minimum = null;
            else
                if (p is decimal)
                minimum = (decimal)p;
            else
                minimum = (decimal)Convert.ChangeType(p, typeof(decimal));
        }

        internal void SetDeviation(object p)
        {
            if (p == DBNull.Value)
                deviation = null;
            else
                if (p is decimal)
                deviation = (decimal)p;
            else
                deviation = (decimal)Convert.ChangeType(p, typeof(decimal));
        }

        internal void SetCount(object p)
        {
            if (p == DBNull.Value)
                count = null;
            else
                if (p is long)
                count = (long)p;
            else
                count = (long)Convert.ChangeType(p, typeof(long));
        }

        public string GetDisplayString()
        {
            decimal? rrStart = ReferenceRangeStart;
            decimal? rrEnd = ReferenceRangeEnd;
            string format = metaData.Format;
            if (rrStart == null && metaData.NullFormat != null)
                format = metaData.NullFormat;

            return String.Format(format, rrStart, rrEnd, metaData.Unit);
        }
    }


    public class BaselineMetaDataItem
    {
        private static readonly Logger LOG = Logger.GetLogger(typeof(BaselineMetaDataItem));

        public readonly int ItemId;
        public readonly string Name;
        public readonly string Description;
        public readonly string Category;
        public readonly string Unit;
        public readonly string Format;
        public readonly string NullFormat;
        public readonly string StatisticTable;
        public readonly string MetricValue;
        public readonly int? Decimals;
        public readonly int? MetricId;
        public readonly int? LowerLimit;
        public readonly long? UpperLimit;
        public readonly decimal? Scale;

        public BaselineMetaDataItem(SqlDataReader dataReader)
        {
            try
            {
                ItemId = dataReader.GetInt32(0);
                Name = BaselineHelpers.GetString(dataReader, "Name", String.Empty);
                Description = BaselineHelpers.GetString(dataReader, "Description", String.Empty);
                Category = BaselineHelpers.GetString(dataReader, "Category", String.Empty);

                Unit = BaselineHelpers.GetString(dataReader, "Unit", String.Empty);
                Format = BaselineHelpers.GetString(dataReader, "Format", String.Empty);
                NullFormat = BaselineHelpers.GetString(dataReader, "NullFormat", String.Empty);
                StatisticTable = BaselineHelpers.GetString(dataReader, "StatisticTable", String.Empty);
                MetricValue = BaselineHelpers.GetString(dataReader, "MetricValue", String.Empty);
                Decimals = BaselineHelpers.GetInt(dataReader, "Decimals", 1);

                MetricId = BaselineHelpers.GetInt(dataReader, "MetricID", null);
                LowerLimit = BaselineHelpers.GetInt(dataReader, "LLimit", null);
                UpperLimit = BaselineHelpers.GetLong(dataReader, "ULimit", null);
                Scale = BaselineHelpers.GetDecimal(dataReader, "Scale", null);
            }
            catch (Exception e)
            {
                LOG.Error("Error loading baseline metadata row: ", e);
                throw;
            }
        }


    }
}
