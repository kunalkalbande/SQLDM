using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Idera.SQLdm.Common.Objects
{
    /// <summary>
    /// Sql 10.2 (Anshul Aggarwal) - New History Browser
    /// Represents state of server - RealTime, Historical (Snapshot selection) and Custom Range
    /// </summary>
    public enum ServerViewMode
    {
        RealTime,
        Historical,
        Custom  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
    }

    /// <summary>
    /// SqlDm 10.2 -- Mitul Kapoor - Get User Saved Settings
    /// Used for saving/retrieving user preferences like StartTime, EndTime and Scale(Minutes).
    /// </summary>
    public class HistoryTimeValue
    {
        #region Private variables

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("HistoryTimeValue");

        #endregion

        #region constants

        private const int DEFAULT_VISIBILE_MINUTES = 60;

        #endregion

        #region Properties

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }


        /// <summary>
        /// Gives custom range minutes by taking ceiling of the time difference b/w start and end dates.
        /// </summary>
        [XmlIgnore]
        public int CustomTimeMinutes
        {
            get
            {
                return (StartDateTime.HasValue && EndDateTime.HasValue)
                    ? (int) Math.Ceiling((EndDateTime - StartDateTime).Value.TotalMinutes)
                    : 0;
            }
        }

        public int RealTimeMinutes { get; set; }

        public ServerViewMode ViewMode { get; set; }
        
        [XmlIgnore]
        public DateTime? HistoricalSnapshotDateTime  { get; set; }

        #endregion

        public HistoryTimeValue()
        {
           Initialize();
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Refreshes all values.
        /// </summary>
        public void Refresh(HistoryTimeValue timeValue)
        {
            if (timeValue != null)
            {
                this.EndDateTime = timeValue.EndDateTime;
                this.StartDateTime = timeValue.StartDateTime;
                this.RealTimeMinutes = timeValue.RealTimeMinutes;
                this.HistoricalSnapshotDateTime = timeValue.HistoricalSnapshotDateTime;
                this.ViewMode = timeValue.ViewMode == ServerViewMode.Historical ? ServerViewMode.RealTime : timeValue.ViewMode;
            }
            else
            {
                // [SQLDM-27451] (Anshul Aggarwal) - History range settings are not persisted when switched between multiple users without closing DM console
                Initialize();
            }
            LogSettings(); // Log new history settings.
        }

        public bool SetServerViewMode(ServerViewMode newViewMode)
        {
            if (this.ViewMode == newViewMode)
                return false;

            this.ViewMode = newViewMode;
            LogSettings(); // Log new history settings.
            return true;
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Refreshes real time visible minutes.
        /// </summary>
        public bool SetVisibleMinutes(int realTimeMinutes)
        {
            if (this.ViewMode == ServerViewMode.RealTime && RealTimeMinutes == realTimeMinutes)
                return false;
            this.ViewMode = ServerViewMode.RealTime;
            this.RealTimeMinutes = realTimeMinutes;
            LogSettings(); // Log new history settings.
            return true;
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Refreshes historical snaphshot datetime.
        /// </summary>
        public bool SetHistoricalSnapshotDateTime(DateTime? historicalSnapshotDateTime)
        {
            if (this.ViewMode == ServerViewMode.Historical && HistoricalSnapshotDateTime == historicalSnapshotDateTime)
                return false;

            this.ViewMode = ServerViewMode.Historical;
            this.HistoricalSnapshotDateTime = historicalSnapshotDateTime;
            LogSettings(); // Log new history settings.
            return true;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Refreshes start and end dates.
        /// </summary>
        public bool SetCustomRange(DateTime? startDateTime, DateTime? endDateTime)
        {
            if (this.ViewMode == ServerViewMode.Custom && StartDateTime == startDateTime && EndDateTime == endDateTime)
                return false;

            this.ViewMode = ServerViewMode.Custom;
            this.EndDateTime = endDateTime;
            this.StartDateTime = startDateTime;
            LogSettings(); // Log new history settings.
            return true;
        }

        /// <summary>
        /// Initializes HistoryTimeValue to default values.
        /// </summary>
        private void Initialize()
        {
            this.RealTimeMinutes = DEFAULT_VISIBILE_MINUTES;
            this.ViewMode = ServerViewMode.RealTime;
            this.EndDateTime = this.StartDateTime = this.HistoricalSnapshotDateTime = null;
        }

        /// <summary>
        /// Logs new history settings
        /// </summary>
        private void LogSettings()
        {
            LOG.Info(string.Format("New HistoryTimeValue settings - RealTimeMinutes : {0} HistoricalSnapshotDateTime : {1} " +
                "CustomStart : {2} CustomEnd : {3} ViewMode : {4}", RealTimeMinutes, HistoricalSnapshotDateTime, StartDateTime, EndDateTime, ViewMode));
        }
    }
}
