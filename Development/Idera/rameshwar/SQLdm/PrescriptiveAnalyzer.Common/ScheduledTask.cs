using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common
{
    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
    /// </summary>
    [Serializable]
    public class ScheduledTask : IComparable<ScheduledTask>
    {
        private int serverID;

        public int ServerID
        {
            get { return serverID; }
            set { serverID = value; }
        }
        private short selectedDays;

        public short SelectedDays
        {
            get { return selectedDays; }
            set { selectedDays = value; }
        }
        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public int CompareTo(ScheduledTask other)
        {
            if (this.startTime == other.startTime)
            { return this.serverID.CompareTo(other.serverID); }

            return this.startTime.CompareTo(other.startTime);
        }
    }
}
