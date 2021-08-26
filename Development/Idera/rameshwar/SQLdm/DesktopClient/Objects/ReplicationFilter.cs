using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class ReplicationFilter : IUserFilter
    {
        #region constants

        //private const int DEFAULT_NUMBEROFRECORDS = 200;
        private const ReplicationTimeSpan DEFAULT_TIMESPAN = ReplicationTimeSpan.Any;

        public enum ReplicationTimeSpan
        {
            [Description("Any Time")]
            Any,
            [Description("Older than 30 minutes")]
            HalfHour,
            [Description("Older than 1 hour")]
            Hour,
            [Description("Older than 12 hours")]
            HalfDay,
            [Description("Older than 1 day")]
            Day,
            [Description("Older than 1 week")]
            Week,
            [Description("Older than 1 month")]
            Month
        }

        #endregion

        #region fields

        //private int numberOfRecords = DEFAULT_NUMBEROFRECORDS;
        private ReplicationTimeSpan filterTimeSpan = DEFAULT_TIMESPAN;

        #endregion

        #region constructors

        public ReplicationFilter()
        {
        }

        #endregion

        #region properties

        ///// <summary>
        ///// The number of records to return
        ///// </summary>
        //[DisplayName("Number of Records"), Category("Publisher")]
        //[Description("Enter the maximum number of unpublished transactions to return")]
        //[DefaultValue(DEFAULT_NUMBEROFRECORDS)]
        //public int NumberOfRecords
        //{
        //    get { return numberOfRecords; }
        //    set { numberOfRecords = value; }
        //}

        /// <summary>
        /// Time span of the jobs to return. Leave null to return all
        /// </summary>
        [DisplayName("Transaction Entry Time"), Category("Distributor")]
        [Description("Show only unsubscribed transactions older than the specified interval")]
        [DefaultValue(DEFAULT_TIMESPAN)]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ReplicationTimeSpan FilterTimeSpanProperty
        {
            get { return filterTimeSpan; }
            set { filterTimeSpan = value; }
        }

        [Browsable(false)]
        public TimeSpan? FilterTimeSpan
        {
            get
            {
                TimeSpan? ts = null;
                switch (filterTimeSpan)
                {
                    case ReplicationTimeSpan.Any:
                        ts = null;
                        break;
                    case ReplicationTimeSpan.HalfHour:
                        ts = new TimeSpan(0, 30, 0);
                        break;
                    case ReplicationTimeSpan.Hour:
                        ts = new TimeSpan(1, 0, 0);
                        break;
                    case ReplicationTimeSpan.HalfDay:
                        ts = new TimeSpan(12, 0, 0);
                        break;
                    case ReplicationTimeSpan.Day:
                        ts = new TimeSpan(24, 0, 0);
                        break;
                    case ReplicationTimeSpan.Week:
                        ts = new TimeSpan(7, 0, 0, 0);
                        break;
                    case ReplicationTimeSpan.Month:
                        ts = new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year,
                                                                DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1),
                                            0, 0, 0);
                        break;
                }

                return ts;
            }
        }

        #endregion

        #region interface implementations

        #region IUserFilter Members

        public void ClearValues()
        {
            //numberOfRecords = int.MaxValue;
            filterTimeSpan = ReplicationTimeSpan.Any;
        }

        public bool HasDefaultValues()
        {
            if (/*numberOfRecords != DEFAULT_NUMBEROFRECORDS
                || */filterTimeSpan != DEFAULT_TIMESPAN)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsFiltered()
        {
            if (filterTimeSpan != ReplicationTimeSpan.Any)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetValues()
        {
            //numberOfRecords = DEFAULT_NUMBEROFRECORDS;
            filterTimeSpan = DEFAULT_TIMESPAN;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is ReplicationFilter)
            {
                ReplicationFilter filter = (ReplicationFilter)selectionFilter;
                //numberOfRecords = filter.NumberOfRecords;
                filterTimeSpan = filter.FilterTimeSpanProperty;
            }
        }

        public bool Validate(out string Message)
        {
            //if (numberOfRecords < 1)
            //{
            //    Message = "Number of Records must be greater than zero.";
            //    return false;
            //}
            //else
            //{
                Message = String.Empty;
                return true;
            //}
        }

        #endregion

        #endregion
    }
}
