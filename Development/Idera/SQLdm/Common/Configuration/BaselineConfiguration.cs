using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;
using System.Linq;
using Idera.SQLdm.Common.Baseline;
using Idera.SQLdm.Common.Thresholds;

namespace Idera.SQLdm.Common.Configuration
{
    /// <summary>
    /// NOTE: We have a config wrapper around the template because in the future we may have more than one template per server, 
    /// for instance, we had planned on having one template per metric per server.
    /// </summary>

    [Serializable]
    public class BaselineConfiguration : ISerializable, IAuditable
    {
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger(typeof(BaselineConfiguration));
        private BaselineTemplate template;
        private int templateId;
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
        private bool active;
        private string baselineName;
        private bool isChanged;
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration
        //SQLdm 10.1 (pulkit puri)--to get xml having actual timezone saved in db
        private string originalTimeZoneTemplateXML;
        //SQLdm 10.1 (Pulkit Puri)-- to get the timezone of desktop client 
        private string desktopUTCtimezoneOffset;

        public BaselineTemplate Template
        {
            get { return template; }
            set { template = value; }
        }

        public int TemplateID
        {
            get { return templateId; }
            set { templateId = value; }
        }
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }
        [XmlElement("Name")]
        public string BaselineName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(baselineName))
                    return Common.Constants.DEFAULT_BASELINE_NAME;
                return baselineName;
            }
            set { baselineName = value; }
        }
        //public string TemplateXML
        //{
        //    get { return Serialize(); }
        //    set { }
        //}
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
        //SQLdm 10.1 (pulkit Puri) -- adding new member to baseline config for having timezone when it was saved
        public string OriginalTimeZoneTemplateXML
        {
            get { return originalTimeZoneTemplateXML; }
            set { originalTimeZoneTemplateXML = value; }
        }
        public string DesktopUTCtimezoneOffset
        {
            get { return desktopUTCtimezoneOffset; }
            set { desktopUTCtimezoneOffset = value; }
        }
        public BaselineConfiguration()
        {
            template = new BaselineTemplate();
        }
        //SQLdm 10.1 (Pulkit Puri)--check if the xml is of 10.0 format without schedule tag in xml
        [XmlIgnore]
        //SQLdm 10.1.1 (srishti purohit)schedule is Not mandatory field as per 10.1.1
        public bool IsScheduledNotFound
        {
            get { return template.ScheduledStartDate == null; }
        }
        public BaselineConfiguration(string templateName, bool customBaseline)
        {
            template = new BaselineTemplate();
            this.baselineName = templateName;
        }
        public BaselineConfiguration(bool? usedefaults, DateTime? calculationStart, DateTime? calculationEnd, short? calculationDays, DateTime? scheduledStart, DateTime? scheduledEnd, short? scheduledDays)
        {
            template = new BaselineTemplate();

            if (usedefaults != null && usedefaults.HasValue)
                template.UseDefault = usedefaults.Value;

            if (calculationStart != null && calculationStart.HasValue)
                template.CalculationStartDate = calculationStart.Value;

            if (calculationEnd != null && calculationEnd.HasValue)
                template.CalculationEndDate = calculationEnd.Value;

            if (calculationDays != null && calculationDays.HasValue)
                template.CalculationSelectedDays = calculationDays.Value;

            if (scheduledStart != null && scheduledStart.HasValue)
                template.ScheduledStartDate = scheduledStart.Value;

            if (scheduledEnd != null && scheduledEnd.HasValue)
                template.ScheduledEndDate = scheduledEnd.Value;

            if (scheduledDays != null && scheduledDays.HasValue)
                template.ScheduledSelectedDays = scheduledDays.Value;
        }

        public BaselineConfiguration(string xml)
        {
            originalTimeZoneTemplateXML = xml;
            template = Deserialize(xml);
        }

        public BaselineConfiguration(SerializationInfo info, StreamingContext context)
        {
            template = new BaselineTemplate();

            template.UseDefault = info.GetBoolean("UseDefault");
            template.CalculationStartDate = info.GetDateTime("StartDate").ToLocalTime();
            template.CalculationEndDate = info.GetDateTime("EndDate").ToLocalTime();
            template.CalculationSelectedDays = (short)info.GetValue("SelectedDays", typeof(short));

            try
            {
                var startschedule = info.GetValue("ScheduledStartDate", typeof(DateTime?));
                if (startschedule != null)
                {
                    template.ScheduledStartDate = Convert.ToDateTime(startschedule).ToLocalTime();
                    template.ScheduledEndDate = Convert.ToDateTime(info.GetValue("ScheduledEndDate", typeof(DateTime?))).ToLocalTime();
                    template.ScheduledSelectedDays = (short)info.GetValue("ScheduledSelectedDays", typeof(short));
                }
            }
            catch (Exception ex) 
            { 
                //Only logging this in Verbose because this exception will happen for all instances that do not have a custom baseline.
                LOG.Verbose(String.Format("Baseline schedule for custom baseline : {0} not found. {1}", info.GetString("BaselineName"), ex.ToString())); 
            }

            templateId = info.GetInt32("TemplateID");
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
            active = info.GetBoolean("Active");
            baselineName = info.GetString("BaselineName");
            isChanged = info.GetBoolean("IsChanged");
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
            originalTimeZoneTemplateXML = info.GetString("BaselineXmlSetTimezone");
            desktopUTCtimezoneOffset = info.GetString("DesktopUTCtimezoneOffset");
        }

        public string Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaselineTemplate));
            StringBuilder buffer = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, template);
                writer.Flush();
            }

            return buffer.ToString();
        }

        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Serialize  BaselineConfiguration list
        public static StringWriter XmlSerialize(Dictionary<int, BaselineConfiguration> baselineConfigList)
        {
            List<BaselineConfiguration> configList = new List<BaselineConfiguration>();
            TimeSpan desktopOffset = TimeSpan.Zero;
            foreach (BaselineConfiguration conf in baselineConfigList.Values)
            {
                //SQLDM 10.1 --save days according to the services at end
                try
                {
                    if (conf.desktopUTCtimezoneOffset != null)
                    {
                        desktopOffset = TimeSpan.Parse(conf.DesktopUTCtimezoneOffset);
                        conf.template.CalculationSelectedDays = conf.GetDaysOfWeekAfterShiftCaclulationBaseline(desktopOffset, true);
                        conf.template.ScheduledSelectedDays = conf.GetDaysOfWeekAfterShiftScheduledBaseline(desktopOffset, true);
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Error parsing the desktop time offset for baseline {0}.", conf.baselineName, ex);
                }
                configList.Add(conf);
            }
            var xs = new XmlSerializer(configList.GetType());
            var xml = new StringWriter();
            xs.Serialize(xml, configList);
            return xml;
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Serialize  BaselineConfiguration list
        private BaselineTemplate Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaselineTemplate));
            BaselineTemplate result = null;
            StringReader stream = new StringReader(xml);

            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                result = (BaselineTemplate)serializer.Deserialize(xmlReader);
            }

            return result;
        }

        /// <summary>
        /// Validates whether a datetime is valid for use in SQL Server.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private bool IsSQLDateValid(DateTime dateTime)
        {
            return !(dateTime.Year < 1900);
        }

        /// <summary>
        /// Gets a valid date for SQL Server usage. Example of a Not SQL Server valid date: 0001-01-01T00:00:00
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void GetValidSQLDateRange(ref DateTime start, ref DateTime end)
        {
            while (!this.IsSQLDateValid(start) || !this.IsSQLDateValid(end))
            {
                if (!this.IsSQLDateValid(start) && !this.IsSQLDateValid(end))
                {
                    end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
                    start = (new DateTime(end.Year, end.Month, end.Day, 8, 0, 0)).AddDays(-7);
                    break;
                }

                if (!this.IsSQLDateValid(end))
                {
                    end = (new DateTime(start.Year, start.Month, start.Day, 17, 0, 0)).AddDays(7);
                    break;
                }

                if (!this.IsSQLDateValid(start))
                {
                    start = (new DateTime(end.Year, end.Month, end.Day, 8, 0, 0)).AddDays(-7);
                    break;
                }
            }
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            DateTime start = template.CalculationStartDate;
            DateTime end = template.CalculationEndDate;

            this.GetValidSQLDateRange(ref start, ref end);

            template.CalculationStartDate = start;
            template.CalculationEndDate = end;
            #region Scheduling Date Range
            if (template.ScheduledStartDate == null)
            {
                LOG.Verbose("Baseline custom saved with no schedule data.");
            }
            else
            {
                DateTime startScheduling = template.ScheduledStartDate.Value;
                DateTime endScheduling = template.ScheduledEndDate.Value;
                this.GetValidSQLDateRange(ref startScheduling, ref endScheduling);
                template.ScheduledStartDate = startScheduling;
                template.ScheduledEndDate = endScheduling;

                //4.1.12 Revise Multiple Baseline for Independent Scheduling
                //SQLdm 10.1 (Srishti Purohit)
                info.AddValue("ScheduledStartDate", startScheduling.ToUniversalTime());
                info.AddValue("ScheduledEndDate", endScheduling.ToUniversalTime());
                info.AddValue("ScheduledSelectedDays", template.ScheduledSelectedDays);
            }
            //SQldm 10.1 (pulkit puri)
            info.AddValue("BaselineXmlSetTimezone", originalTimeZoneTemplateXML);
            info.AddValue("DesktopUTCtimezoneOffset", desktopUTCtimezoneOffset);
            #endregion

            info.AddValue("UseDefault", template.UseDefault);
            info.AddValue("StartDate", start.ToUniversalTime());
            info.AddValue("EndDate", end.ToUniversalTime());
            info.AddValue("SelectedDays", template.CalculationSelectedDays);
            info.AddValue("TemplateID", templateId);
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 
            info.AddValue("BaselineName", baselineName);
            info.AddValue("Active", active);
            info.AddValue("IsChanged", isChanged);
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added new fields in BaselineConfiguration 

        }

        #endregion

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entity = new AuditableEntity();

            PropertyInfo[] properties = template.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(AuditableAttribute), true);
                String propertyName = property.Name;

                if (attributes.Length > 0)
                {
                    AuditableAttribute attribute = (AuditableAttribute)attributes[0];

                    if (!attribute.IsAuditable)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(attribute.NewPropertyName))
                    {
                        propertyName = attribute.NewPropertyName;
                    }

                    entity.AddMetadataProperty(propertyName, property.GetValue(template, null).ToString());
                }
            }


            return entity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity entity = new AuditableEntity();
            BaselineConfiguration oldConfiguration = oldValue as BaselineConfiguration;

            List<PropertiesComparer.PropertiesData> propertiesChanged = null;

            if (oldConfiguration != null && HasChangedProperties(oldConfiguration, out propertiesChanged))
            {
                // Build metadata.
                propertiesChanged.ForEach(property => entity.AddMetadataProperty(property.Name, property.Value));
            }

            // If the configuration does not changed, returns an empty entity.
            return entity;
        }

        /// <summary>
        /// Try to get the changed properties in to a list. Returns true if has succeeded and false in other case.
        /// </summary>
        /// <param name="oldConfiguration">Old object</param>
        /// <param name="propertiesChanged">The changed properties.</param>
        /// <returns>Returns true if has succeeded and false in other case.</returns>
        private bool HasChangedProperties(BaselineConfiguration oldConfiguration,
            out List<PropertiesComparer.PropertiesData> propertiesChanged)
        {
            PropertiesComparer comparer = new PropertiesComparer();
            // Get changed properties.
            propertiesChanged = comparer.GetNewProperties(oldConfiguration.template, this.template);

            return propertiesChanged.Count > 0;
        }

        /// <summary>
        ///  gets the day shift in baseline
        /// SQLdm 10.1 Pulkit Puri-360 baseline implemenation
        /// gets the timezone of required time and finds whether there is shift in days
        /// </summary>
        /// <param name="timeZoneOffset"></param>
        /// <param name="checkGapForScheduledBaseline"></param>
        /// <param name="isDBUpdateCall"></param>
        private BaselineDayShift DayShiftInBaseline(TimeSpan timeZoneOffset, bool checkGapForScheduledBaseline, bool isDBUpdateCall)
        {
            try
            {
                DateTimeOffset dateBeforeShift, dateAfterShift;

                if (checkGapForScheduledBaseline)
                {
                    dateBeforeShift = (DateTimeOffset)template.ScheduledStartDate; 
                }
                else
                {
                    dateBeforeShift = (DateTimeOffset)template.CalculationStartDate;
                }
                dateAfterShift = dateBeforeShift;
                dateAfterShift = dateAfterShift.Add(timeZoneOffset - dateBeforeShift.Offset);
                if (dateAfterShift.DayOfWeek == DayOfWeek.Saturday && dateBeforeShift.DayOfWeek == DayOfWeek.Sunday)
                    return BaselineDayShift.Preshift;
                else if (dateAfterShift.DayOfWeek == DayOfWeek.Sunday && dateBeforeShift.DayOfWeek == DayOfWeek.Saturday)
                    return BaselineDayShift.PostShift;
                return (BaselineDayShift)(dateAfterShift.DayOfWeek - dateBeforeShift.DayOfWeek);
            }
            catch (Exception ex)
            {
                LOG.Error("Error shifting baseline days.", ex);
                return BaselineDayShift.NoShift;
            }
        }
        /// <summary>
        /// SQLdm 10.1 Pulkit Puri-360 baseline implemenation
        /// gets the day shift if there is shift in time zone for scheduled baseline
        /// </summary>
        /// <param name="timeZoneOffset"></param>
        /// <param name="isDBUpdateCall"></param>
        public short GetDaysOfWeekAfterShiftScheduledBaseline(TimeSpan timeZoneOffset, bool isDBUpdateCall = false)
        {
            try
            {
                BaselineDayShift dayDifference = DayShiftInBaseline(timeZoneOffset, true, isDBUpdateCall);
                //if due to desktop there is day shift , so in services there must be opposite shift from desktop
                if (isDBUpdateCall) dayDifference = (BaselineDayShift)(BaselineDayShift.NoShift - dayDifference);
                return template.GetDaysOfWeekToSelect(template.ScheduledSelectedDays, (int)dayDifference);
            }
            catch (Exception ex)
            {
                LOG.Error("error shifting Scheduled baseline days.", ex);
                return template.ScheduledSelectedDays;
            }
        }
        /// <summary>
        /// SQLdm 10.1 Pulkit Puri-360 baseline implemenation
        /// gets the day shift if there is shift in time zone for calculated baseline
        /// </summary>
        /// <param name="timeZoneOffset"></param>
        /// <param name="isDBUpdateCall"></param>
        public short GetDaysOfWeekAfterShiftCaclulationBaseline(TimeSpan timeZoneOffset, bool isDBUpdateCall = false)
        {
            try
            {
                BaselineDayShift dayDifference = DayShiftInBaseline(timeZoneOffset, false, isDBUpdateCall);
                //if due to desktop there is day shift , so in services there must be opposite shift from desktop
                if (isDBUpdateCall) dayDifference = (BaselineDayShift)(BaselineDayShift.NoShift - dayDifference);
                return template.GetDaysOfWeekToSelect(template.CalculationSelectedDays, (int)dayDifference);
            }
            catch (Exception ex)
            {
                LOG.Error("Error shifting Caclulation baseline days.", ex);
                return template.CalculationSelectedDays;
            }
        }
        /// <summary>
        /// shift days according to utc shift
        /// </summary>


        //SQLDM 10.1 (Pulkit Puri)--baseline 360 implemnatation(end)

        /// <summary>
        ///START: SQLdm 10.0 (Tarun Sapra)- make a unified xml for all the configs
        /// </summary>
        /// <param name="sqlServerID"></param>
        /// <param name="defaultConfig"></param>
        /// <param name="customConfigs"></param>
        /// <returns></returns>        
        public static string SerializeAllBaselineConfigurations(int sqlServerID, BaselineConfiguration defaultConfig, Dictionary<int, BaselineConfiguration> customConfigs)
        {
            Dictionary<int, BaselineConfiguration> clonedDic = ObjectHelper.Clone(customConfigs);
            IList<BaselineConfiguration> customTemplates = new List<BaselineConfiguration>();
            if (clonedDic != null)
                customTemplates = clonedDic.Values.ToList();

            customTemplates.Add(defaultConfig);
            customTemplates = customTemplates.Select(x => { x.BaselineName = ConvertToSQLSupportedXml(x.BaselineName); return x; }).ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(List<BaselineConfiguration>));
            StringBuilder buffer = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, customTemplates);
                writer.Flush();
            }

            return buffer.ToString();

            //StringBuilder builder = new StringBuilder();
            //builder.Append("<BaselineConfigs>");
            //foreach (BaselineConfiguration config in customTemplates)
            //{
            //    builder.Append("<BaselineConfig>");
            //    builder.Append("<Name>");
            //    if (config.BaselineName == null) builder.Append("Default");
            //    else builder.Append(config.BaselineName);
            //    builder.Append("</Name>");
            //    builder.Append(config.TemplateXML);
            //    builder.Append("</BaselineConfig>");
            //}
            //builder.Append("</BaselineConfigs>");
            //return builder.ToString();
        }
        /// <summary>
        /// SQLdm 10.1 (Srishti Purohit)
        /// Replacing chars which are not supported by xml in sql
        /// </summary>
        /// <param name="baselineName">baselineConfgi xml after serialization</param>
        private static string ConvertToSQLSupportedXml(string baselineName)
        {
            string escapedXml;
            escapedXml = baselineName.Replace("&", "&amp;");
            escapedXml = escapedXml.Replace("\"", "&quot;");
            escapedXml = escapedXml.Replace("<", "&lt;");
            escapedXml = escapedXml.Replace(">", "&gt;");
            return escapedXml;
        }
    }

    [Serializable()]
    public class BaselineTemplate
    {
        private bool useDefault;
        private DateTime calculationStartDate;
        private DateTime calculationEndDate;
        private short caculationSelectedDays;
        //4.1.12 Revise Multiple Baseline for Independent Scheduling
        //SQLdm 10.1 (Srishti Purohit)
        private DateTime? scheduledStartDate;
        private DateTime? scheduledEndDate;
        private short scheduledSelectedDays = 0;
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger(typeof(BaselineConfiguration));
        public BaselineTemplate()
        {
            useDefault = true;
            caculationSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            caculationSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            caculationSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            caculationSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            caculationSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);

            DateTime now = DateTime.Now;
            calculationStartDate = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
            calculationEndDate = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
            calculationStartDate = calculationStartDate.AddDays(-30);

            //4.1.12 Revise Multiple Baseline for Independent Scheduling
            //SQLdm 10.1 (Srishti Purohit)
            //scheduledSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            //scheduledSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            //scheduledSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            //scheduledSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            //scheduledSelectedDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            //scheduledStartDate = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
            //scheduledEndDate = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
            //scheduledStartDate = scheduledStartDate.AddDays(-30);
        }

        [AuditableAttribute(false)]
        public bool UseDefault
        {
            get { return useDefault; }
            set { useDefault = value; }
        }

        [AuditableAttribute(false)]
        public DateTime CalculationStartDate
        {
            get { return calculationStartDate; }
            set { calculationStartDate = value; }
        }

        [AuditableAttribute(false)]
        public DateTime CalculationEndDate
        {
            get { return calculationEndDate; }
            set { calculationEndDate = value; }
        }

        [AuditableAttribute(false)]
        public short CalculationSelectedDays
        {
            get { return caculationSelectedDays; }
            set { caculationSelectedDays = value; }
        }

        [AuditableAttribute("Time Period")]
        public string TimePeriodString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek(d, caculationSelectedDays))
                    {
                        sb.Append(d.ToString().Substring(0, 3));
                        sb.Append(",");
                    }
                }
                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                sb.Append(" between ");
                sb.Append(CalculationStartDate.ToString("t"));
                sb.Append(" and ");
                sb.Append(CalculationEndDate.ToString("t"));
                return sb.ToString();
            }
        }

        [Auditable("Date Range")]
        public string DateRangeString
        {
            get
            {
                if (UseDefault)
                    return "Automatic (Last 7 days)";
                else
                {
                    return "Custom (" + CalculationStartDate.ToString("d") + " - " + CalculationEndDate.ToString("d") + ")";
                }
            }
        }

        [AuditableAttribute(false)]
        public DateTime? ScheduledStartDate
        {
            get { return scheduledStartDate; }
            set { scheduledStartDate = value; }
        }
        public bool ShouldSerializeScheduledStartDate()
        {
            return ScheduledStartDate != null;
        }
        [AuditableAttribute(false)]
        public DateTime? ScheduledEndDate
        {
            get { return scheduledEndDate; }
            set { scheduledEndDate = value; }
        }

        public bool ShouldSerializeScheduledEndDate()
        {
            return scheduledEndDate != null;
        }
        [AuditableAttribute(false)]
        public short ScheduledSelectedDays
        {
            get { return scheduledSelectedDays; }
            set { scheduledSelectedDays = value; }
        }
        public bool ShouldSerializeScheduledSelectedDays()
        {
            return scheduledSelectedDays != 0;
        }
        //returns shiftedCalculationDays according to UTC
        public short GetShiftedUTCCalculationDays()
        {
            try
            {
                DateTime calculationUTCStartDate = calculationStartDate.ToUniversalTime();
                BaselineDayShift daydifference;
                if (calculationUTCStartDate.DayOfWeek == DayOfWeek.Saturday && calculationStartDate.DayOfWeek == DayOfWeek.Sunday)
                    daydifference = BaselineDayShift.Preshift;
                else if (calculationUTCStartDate.DayOfWeek == DayOfWeek.Sunday && calculationStartDate.DayOfWeek == DayOfWeek.Saturday)
                    daydifference = BaselineDayShift.PostShift;
                else
                    daydifference = (BaselineDayShift)(calculationUTCStartDate.DayOfWeek - calculationStartDate.DayOfWeek);
                return GetDaysOfWeekToSelect(CalculationSelectedDays, (int)daydifference);
            }
            catch (Exception ex)
            {
                LOG.Error("Error shifting caclucation days according to UTC.", ex);
                return CalculationSelectedDays;
            }

        }
        //SQLdm10.1 --shift days according to utc 
        public short GetDaysOfWeekToSelect(short daysScheduled, int daydifference)
        {
            short days = 0;
            try
            {
                if (daydifference == (int)BaselineDayShift.Preshift)
                {
                    if ((daysScheduled & 1) == 1) days |= 128;
                    if ((daysScheduled & 4) == 4) days |= 1;
                    for (int bitpointer = 3; bitpointer <= 7; bitpointer++)
                    {
                        if ((daysScheduled & (short)Math.Pow(2, bitpointer)) == (short)Math.Pow(2, bitpointer))
                            days |= (short)Math.Pow(2, (bitpointer + 7) % 8);
                    }
                }
                else if (daydifference == (int)BaselineDayShift.PostShift)
                {
                    if ((daysScheduled & 1) == 1) days |= 4;
                    for (int bitpointer = 2; bitpointer <= 7; bitpointer++)
                    {
                        if ((daysScheduled & (short)Math.Pow(2, bitpointer)) == (short)Math.Pow(2, bitpointer))
                            days |= (short)Math.Pow(2, (bitpointer + 1) % 8);
                    }
                }
                else
                    days = daysScheduled;
            }
            catch (Exception ex)
            {
                LOG.Error("Error shifting baseline days.", ex);
                return daysScheduled;
            }
            return days;

        }

    }
}
