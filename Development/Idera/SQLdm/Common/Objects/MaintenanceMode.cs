using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;


namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public enum MaintenanceModeType
    {
        Never,
        Always,
        Recurring,
        Once,
        Monthly
    }

    [Serializable]
    public sealed class MaintenanceMode : IAuditable
    {
        private MaintenanceModeType maintenanceModeType = MaintenanceModeType.Never;
        private DateTime? maintenanceModeStart = null;
        private DateTime? maintenanceModeStop = null;
        private DateTime? maintenanceModeRecurringStart = null;
        private TimeSpan? maintenanceModeDuration = null;
        private Int16? maintenanceModeDays = null;
        private DateTime? maintenanceModeMonthRecurringStart = null;
        private int maintenanceModeMonth = 0;
        private int maintenanceModeSpecificDay = 0;
        private int maintenanceModeWeekOrdinal = 0;
        private int maintenanceModeWeekDay = 0;
        private TimeSpan? maintenanceModeMonthDuration = null;

        public MaintenanceMode()
        {
        }

        [Auditable(false)]
        public MaintenanceModeType MaintenanceModeType
        {
            get { return maintenanceModeType; }
            set { maintenanceModeType = value; }
        }

        [Auditable(false)]
        public DateTime? MaintenanceModeStart
        {
            get { return maintenanceModeStart; }
            set { maintenanceModeStart = value; }
        }

        [Auditable(false)]
        public DateTime? MaintenanceModeRecurringStart
        {
            get { return maintenanceModeRecurringStart; }
            set { maintenanceModeRecurringStart = value; }
        }

        [Auditable(false)]
        public DateTime? MaintenanceModeStop
        {
            get { return maintenanceModeStop; }
            set { maintenanceModeStop = value; }
        }

        /// <summary>
        /// Sets or gets the MaintenanceModeOnDemand flag. When it is true it means a maintenance mode is running on demand mode, no scheduled.
        /// </summary>
        [Auditable(false)]
        public bool? MaintenanceModeOnDemand
        {
            get; set;
        }

        [Auditable(false)]
        public TimeSpan? MaintenanceModeDuration
        {
            get { return maintenanceModeDuration; }
            set { maintenanceModeDuration = value; }
        }

        [Auditable(false)]
        public Int16? MaintenanceModeDays
        {
            get { return maintenanceModeDays; }
            set { maintenanceModeDays = value; }
        }

        [Auditable(false)]
        public int MaintenanceModeMonth
        {
            get { return maintenanceModeMonth; }
            set { maintenanceModeMonth = value; }
        }

        [Auditable(false)]
        public int MaintenanceModeSpecificDay
        {
            get { return maintenanceModeSpecificDay; }
            set { maintenanceModeSpecificDay = value; }
        }

        [Auditable(false)]
        public int MaintenanceModeWeekOrdinal
        {
            get { return maintenanceModeWeekOrdinal; }
            set { maintenanceModeWeekOrdinal = value; }
        }

        [Auditable(false)]
        public int MaintenanceModeWeekDay
        {
            get { return maintenanceModeWeekDay; }
            set { maintenanceModeWeekDay = value; }
        }

        [Auditable(false)]
        public TimeSpan? MaintenanceModeMonthDuration
        {
            get { return maintenanceModeMonthDuration; }
            set { maintenanceModeMonthDuration = value; }
        }

        [Auditable(false)]
        public DateTime? MaintenanceModeMonthRecurringStart
        {
            get { return maintenanceModeMonthRecurringStart; }
            set { maintenanceModeMonthRecurringStart = value; }
        }

        [Auditable(false)]
        public string MaintenanceModeDaysText
        {
            get
            {
                string maintenanceModeDaysText = String.Empty;
                List<DayOfWeek> daysOfWeek = MonitoredSqlServer.GetDays(MaintenanceModeDays);

                if(daysOfWeek.Count > 0)
                {
                    foreach (DayOfWeek day in daysOfWeek)
                    {
                        maintenanceModeDaysText = String.Format("{0}{1}, ", maintenanceModeDaysText, day);
                    }

                    maintenanceModeDaysText = maintenanceModeDaysText.Substring(0, maintenanceModeDaysText.Length - 2);
                }

                return maintenanceModeDaysText;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            switch (maintenanceModeType)
            {
                case MaintenanceModeType.Never:
                    sb.Append("Not set");
                    break;
                case MaintenanceModeType.Always:
                    sb.Append(Constants.Always);
                    break;
                case MaintenanceModeType.Recurring:
                    sb.Append("Recurring every ");
                     foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        if (MonitoredSqlServer.MatchDayOfWeek(d,maintenanceModeDays))
                        {
                            sb.Append(d.ToString().Substring(0, 3));
                            sb.Append(",");
                        }
                    }
                    if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);
                    if (MaintenanceModeRecurringStart.HasValue)
                    {
                        sb.Append(" at ");
                        sb.Append(MaintenanceModeRecurringStart.Value.TimeOfDay);
                    }
                    if (MaintenanceModeDuration.HasValue)
                    {
                        sb.Append(" for ");
                        sb.Append(MaintenanceModeDuration.Value.TotalMinutes);
                        sb.Append(" minutes");
                    }
                    break;
                case MaintenanceModeType.Monthly:
                    sb.Append("Recurring ");
                    
                    if (maintenanceModeSpecificDay > 0)
                    {
                        sb.Append("on ");
                        sb.Append(maintenanceModeSpecificDay);
                        sb.Append(" day of every ");
                        sb.Append(MaintenanceModeMonth);
                        sb.Append(" month(s) ");
                    }
                    else 
                    {
                        sb.Append("on ");

                        if (maintenanceModeWeekOrdinal == 1)
                            sb.Append("First ");
                        if (maintenanceModeWeekOrdinal == 2)
                           sb.Append("Second ");
                        if (maintenanceModeWeekOrdinal == 3)
                            sb.Append("Third ");
                        if (maintenanceModeWeekOrdinal == 4)
                            sb.Append("Fourth ");

                        if (maintenanceModeWeekDay == 1)
                            sb.Append("Sunday ");
                        if (maintenanceModeWeekDay == 2)
                            sb.Append("Monday ");
                        if (maintenanceModeWeekDay == 3)
                            sb.Append("Tuesday");
                        if (maintenanceModeWeekDay == 4)
                            sb.Append("Wednesday ");
                        if (maintenanceModeWeekDay == 5)
                            sb.Append("Thursday ");
                        if (maintenanceModeWeekDay == 6)
                            sb.Append("Friday ");
                        if (maintenanceModeWeekDay == 7)
                            sb.Append("Saturday ");

                        sb.Append(" of every ");
                        sb.Append(MaintenanceModeMonth);
                        sb.Append(" month(s) ");
                    }

                    if (maintenanceModeMonthRecurringStart.HasValue)
                    {
                        sb.Append(" at ");
                        sb.Append(maintenanceModeMonthRecurringStart.Value.TimeOfDay);
                    }
                    if (maintenanceModeMonthDuration.HasValue)
                    {
                        sb.Append(" for ");
                        sb.Append(maintenanceModeMonthDuration.Value.TotalMinutes);
                        sb.Append(" minutes");
                    }
                    break;
                case MaintenanceModeType.Once:

                    sb.Append("Occurring once ");
                    if (maintenanceModeStart.HasValue && maintenanceModeStop.HasValue)
                    {
                        sb.Append("from ");
                        sb.Append(maintenanceModeStart.Value.ToString());
                        sb.Append(" to ");
                        sb.Append(maintenanceModeStop.Value.ToString());
                    }
                    break;
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Get an Auditable Entity from Tag
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {            
            return GetAuditableEntity(null);
        }

        /// <summary>
        /// Return a MaintenanceMode object diferent to the actual
        /// </summary>
        /// <returns></returns>
        private MaintenanceMode GetDifferent()
        {
            MaintenanceMode configuration = new MaintenanceMode();


            configuration.MaintenanceModeStart = MaintenanceModeStart == null
                                                     ? MaintenanceModeStart
                                                     : MaintenanceModeStart.Value.AddSeconds(1);

            configuration.MaintenanceModeStop = MaintenanceModeStop == null
                                                    ? MaintenanceModeStop
                                                    : MaintenanceModeStop.Value.AddSeconds(1);

            configuration.MaintenanceModeRecurringStart = MaintenanceModeRecurringStart == null
                                                              ? MaintenanceModeRecurringStart
                                                              : MaintenanceModeRecurringStart.Value.AddSeconds(1);

            configuration.MaintenanceModeDays = MaintenanceModeDays == null
                                                    ? MaintenanceModeDays
                                                    : AuditTools.Oposite(MaintenanceModeDays);

            configuration.MaintenanceModeDuration = MaintenanceModeDuration == null
                                                        ? MaintenanceModeDuration
                                                        : MaintenanceModeDuration.Value.Add(
                                                            new TimeSpan(MaintenanceModeDuration.Value.Ticks + 1));

            return configuration;
        }

        /// <summary>
        /// Return the entity for this object with the name only
        /// </summary>
        /// <returns></returns>
        private AuditableEntity CreateAuditable()
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = this.MaintenanceModeType.ToString();

            return auditable;
        }


        /// <summary>
        /// Get an Auditable Entity from maintenance mode. If the oldAuditValue is null return a entity with properties corresponding 
        /// to Maintenance Mode Type
        /// </summary>
        /// <param name="oldAuditValue">IAuditable</param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldAuditValue)
        {
            AuditableEntity entity = CreateAuditable();

            MaintenanceMode oldValue = oldAuditValue as MaintenanceMode ?? GetDifferent();

            string typeText = "None";

            bool ignore = oldValue.MaintenanceModeType != this.MaintenanceModeType;

            switch (MaintenanceModeType)
            {
                case MaintenanceModeType.Never:
                    typeText = Constants.Never;
                    break;
                case MaintenanceModeType.Always:
                    typeText = Constants.Always;
                    break;
                case MaintenanceModeType.Once:
                    typeText = Constants.Once;

                    if (!AuditTools.Equals(oldValue.MaintenanceModeStart, MaintenanceModeStart, ignore))
                    {
                        entity.AddMetadataProperty("Maintenance Mode's Start Date and Time",
                                                   MaintenanceModeStart.ToString());
                    }
                    if (!AuditTools.Equals(oldValue.MaintenanceModeStop, MaintenanceModeStop, ignore))
                    {
                        entity.AddMetadataProperty("Maintenance Mode's Stop Date and Time",
                                                   MaintenanceModeStop.ToString());
                    }
                    break;
                case MaintenanceModeType.Recurring:
                    typeText = Constants.Recurring;

                    if (
                        !AuditTools.Equals(oldValue.MaintenanceModeRecurringStart, MaintenanceModeRecurringStart, ignore))
                    {
                        entity.AddMetadataProperty("Maintenance Mode's recurring Start Time",
                                                   MaintenanceModeRecurringStart.Value.ToLongTimeString());
                    }
                    if (!AuditTools.Equals(oldValue.MaintenanceModeDays, MaintenanceModeDays, ignore))
                    {
                        entity.AddMetadataProperty("Days selected for Maintenance Mode", MaintenanceModeDaysText);
                    }

                    if (!AuditTools.Equals(oldValue.MaintenanceModeDuration, MaintenanceModeDuration, ignore))
                    {
                        entity.AddMetadataProperty("Maintenance Mode Duration", MaintenanceModeDuration.ToString());
                    }
                    break;
                default:
                    typeText = "None";
                    break;
            }
            
            if (MaintenanceModeType != oldValue.MaintenanceModeType)
            {
                entity.AddMetadataProperty("Changed Maintenance Mode to", typeText);
            }
            else if(!entity.HasMetadataProperties())
            {
                return null;
            }
            

            return entity;
        }
    }
}
