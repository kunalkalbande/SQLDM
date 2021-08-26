using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class NUMANodeCountersMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("NUMANodeCountersMetrics");
        public List<NUMANodeCounters> NumaNodeCountersList { get; set; }

        public NUMANodeCountersMetrics()
        {
            NumaNodeCountersList = new List<NUMANodeCounters>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("NUMANodeCountersMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.NUMANodeCountersSnapshotValueShutdown == null) { return; }
            if (snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters != null && snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters.Rows.Count; index++)
                {
                    NUMANodeCounters obj = new NUMANodeCounters();
                    try
                    {
                        obj.NodeName = (string)snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters.Rows[index]["NodeName"];
                        obj.PageLifeExpectancy = Convert.ToUInt64((long)snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters.Rows[index]["PLE"]);
                        obj.TargetPages = Convert.ToUInt64((long)snapshot.NUMANodeCountersSnapshotValueShutdown.NUMANodeCounters.Rows[index]["TargetPages"]);
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    NumaNodeCountersList.Add(obj);
                }
            }
        }
    }

    public class NUMANodeCounters
    {
        public string NodeName { get;  set; }
        public UInt64 PageLifeExpectancy { get;  set; }
        public UInt64 TargetPages { get;  set; }
    }
}
