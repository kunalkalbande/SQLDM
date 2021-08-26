using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Service.Helpers;

namespace Idera.SQLdm.Service.DataContracts.v1.Category.Sessions
{
    [DataContract]
    public class ServerSessionStatistics
    {
       
        //[DataMember]
        //public string InstanceName { get; set; }

        [DataMember]
        public Int32 ResponseTime { get; set; }

        [DataMember]
        public int ActiveSessionCount { get; set; }

        [DataMember]
        public int IdleSessionCount { get; set; }

        [DataMember]
        public int SystemSessionCount { get; set; }

        [DataMember]
        public int BlockedCount { get; set; }

        [DataMember]
        public int LeadBlockers { get; set; }

        [DataMember]
        public long TotalDeadLock { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        public ServerSessionStatistics(Common.Snapshots.SessionSnapshot ss, DateTime utcCollectionTime, string timeZoneOffset)
        {
            IList<SessionsForInstance> sessions = ConvertToDataContract.ToDC(ss, timeZoneOffset);
            foreach(SessionsForInstance sfi in sessions)
            {
                if (sfi.connection.IsSystemSession)
                {
                    this.SystemSessionCount++;
                }
                if ("Running".Equals(sfi.connection.Status))
                {
                    this.ActiveSessionCount++;
                }
            }

            if (ss.SystemProcesses != null)
            {
                this.BlockedCount = ss.SystemProcesses.BlockedProcesses != null ? ss.SystemProcesses.BlockedProcesses.Value : 0;
                this.LeadBlockers = ss.SystemProcesses.LeadBlockers != null ? ss.SystemProcesses.LeadBlockers.Value : 0;
                //this.TotalDeadLock = ss.SystemProcesses.TotalDeadLoc != null ? ss.SystemProcesses.BlockedProcesses.Value : 0; ///TODO: Add TotalDeadlock field
            }

            this.UTCCollectionDateTime = utcCollectionTime;
        }

        public ServerSessionStatistics()
        {

        }


    }
}
