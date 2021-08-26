using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class SampledServerResourcesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("SampledServerResourcesMetrics");
        public SampledServerResources FirstSnapshot { get; set; }
        SampledServerResourcesSnapshots _snapshots = new SampledServerResourcesSnapshots();

        public UInt64 Connections { get { return (_snapshots.Connections); } }
        public UInt64 BatchReqSec { get { return (_snapshots.AvgBatchReqSec); } }
        public UInt64 PageSplitsSec { get { return (_snapshots.AvgPageSplitsSec); } }
        public UInt64 PagesAllocatedSec { get { return (_snapshots.AvgPagesAllocatedSec); } }
        public UInt64 MemoryGrantsPending { get { return (_snapshots.AvgMemoryGrantsPending); } }
        public UInt64 LastPageLifeExpectancy { get { return (_snapshots.LastPageLifeExpectancy); } }
        public double LastBufferCacheHitRatio { get { return (_snapshots.LastBufferCacheHitRatio); } }
        public UInt64 SqlCompilationsSec { get { return (_snapshots.AvgSqlCompilationsSec); } }
        public UInt64 SqlRecompilationsSec { get { return (_snapshots.AvgSqlRecompilationsSec); } }
        public UInt64 SumNetWaits { get { return (_snapshots.SumNetWaits); } }
        public UInt64 SumNonNetWaits { get { return (_snapshots.SumNonNetWaits); } }
        public UInt64 SumPackSentAndReceived { get { return (_snapshots.SumPackSentAndReceived); } }
        public UInt64 SumIoWaits { get { return (_snapshots.SumIoWaits); } }
        public UInt64 SumTotalWaits { get { return (_snapshots.SumTotalWaits); } }
        public Int64 SumTempDbMetadataWaits { get { return (_snapshots.SumTempDbMetadataWaits); } }
        public Int32 TempDbMetadataWaitingSampleCounts { get { return (_snapshots.TempDbMetadataWaitingSampleCounts); } }

        public int DefaultFillFactor { get; set; }

        public override void AddSnapshot(SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) return;
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("SampledServerResourcesMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot != null && snapshot.SampleServerResourcesSnapshotValueStartup != null && snapshot.SampleServerResourcesSnapshotValueStartup.SampleServerResources != null)
                _snapshots.Add(new SampledServerResources(snapshot.SampleServerResourcesSnapshotValueStartup.SampleServerResources));
            if (snapshot.SampleServerResourcesSnapshotValueInterval != null && snapshot.SampleServerResourcesSnapshotValueInterval.LstSampleServerResources!=null)
            {
                foreach (var dt in snapshot.SampleServerResourcesSnapshotValueInterval.LstSampleServerResources)
                    _snapshots.Add(new SampledServerResources(dt));
            }
            //This is old code as per previous approach of having multiple snapshots
            //foreach (Idera.SQLdm.Common.Snapshots.SampleServerResourcesSnapshot snp in snapshot.SampleServerResourcesSnapshotValueInterval)
            //{
            //    _snapshots.Add(new SampledServerResources(snp.SampleServerResources));
            //}
            if (snapshot != null && snapshot.SampleServerResourcesSnapshotValueShutdown != null && snapshot.SampleServerResourcesSnapshotValueShutdown.SampleServerResources != null)
                _snapshots.Add(new SampledServerResources(snapshot.SampleServerResourcesSnapshotValueShutdown.SampleServerResources));
            
            base.AddSnapshot(snapshot);
        }
    }
}
