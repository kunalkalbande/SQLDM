//------------------------------------------------------------------------------
// <copyright file="SnapshotTimeRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class SnapshotTimeRule
    {
        public static TimeSpan MIDNIGHT = new TimeSpan(0, 0, 0);

        [XmlAttribute]
        public bool Sunday;
        [XmlAttribute]
        public bool Monday;
        [XmlAttribute]
        public bool Tuesday;
        [XmlAttribute]
        public bool Wednesday;
        [XmlAttribute]
        public bool Thursday;
        [XmlAttribute]
        public bool Friday;
        [XmlAttribute]
        public bool Saturday;

        [XmlIgnore]
        public TimeSpan StartTime;
        [XmlIgnore]
        public TimeSpan EndTime;

        private bool enabled;

        public SnapshotTimeRule()
        {
            Enabled = false;
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    Initialize();
                }
            }
        }

        private void Initialize()
        {
            Sunday = Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = false;
            StartTime = MIDNIGHT;
            EndTime = MIDNIGHT;
        }

        [XmlIgnore]
        public bool IsStartTimeSet
        {
            get { return StartTime != MIDNIGHT; }
        }

        [XmlIgnore]
        public bool IsEndTimeSet
        {
            get { return EndTime != MIDNIGHT; }
        }

        [XmlIgnore]
        public bool AllDays
        {
            get { return Sunday && Monday && Tuesday && Wednesday && Thursday && Friday && Saturday; }
        }

        [XmlIgnore]
        public bool AnyDays
        {
            get { return Sunday || Monday || Tuesday || Wednesday || Thursday || Friday || Saturday; }
        }

        [XmlAttribute]
        public long StartTimeTicks
        {
            get { return StartTime.Ticks; }
            set { StartTime = TimeSpan.FromTicks(value); }
        }

        [XmlAttribute]
        public long EndtTimeTicks
        {
            get { return EndTime.Ticks; }
            set { EndTime = TimeSpan.FromTicks(value); }
        }

        public bool IsValid()
        {
/*
            bool isOK = false;
            bool dodays = AnyDays && !AllDays;

            TimeSpan st = StartTime;
            TimeSpan et = EndTime;
            if (st.Hours != et.Hours || st.Minutes != et.Minutes)
                isOK = true;

            return isOK || dodays;
*/
            return true;
        }
    }
}
