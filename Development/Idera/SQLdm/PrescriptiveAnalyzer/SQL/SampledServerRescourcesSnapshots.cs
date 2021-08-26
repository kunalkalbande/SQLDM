using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    internal class SampledServerResourcesSnapshots : List<SampledServerResources>
    {
        public UInt64 LastPageLifeExpectancy
        {
            get
            {
                if (this.Count <= 0) return (0);
                return (this[this.Count - 1].PageLifeExpectancy);
            }
        }
        public double LastBufferCacheHitRatio
        {
            get
            {
                if (this.Count <= 0) return (0);
                return (this[this.Count - 1].BufferCacheHitRatio);
            }
        }
        public UInt64 AvgMemoryGrantsPending
        {
            get
            {
                if (this.Count <= 0) return (0);
                UInt64 t = 0;
                UInt64 count = 0;
                foreach (SampledServerResources s in this)
                {
                    if (null != s)
                    {
                        t += s.MemoryGrantsPending;
                        ++count;
                    }
                }
                return (count > 0 ? Convert.ToUInt64(t / count) : 0);
            }
        }
        public UInt64 AvgPagesAllocatedSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                return Convert.ToUInt64((this[this.Count - 1].PagesAllocatedSec - this[0].PagesAllocatedSec) / SecondsBetweenSnapshots);
            }
        }
        public UInt64 AvgPageSplitsSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                return Convert.ToUInt64((this[this.Count - 1].PageSplitsSec - this[0].PageSplitsSec) / SecondsBetweenSnapshots);
            }
        }
        public UInt64 AvgBatchReqSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                return Convert.ToUInt64((this[this.Count - 1].BatchReqSec - this[0].BatchReqSec) / SecondsBetweenSnapshots);
            }
        }
        public UInt64 AvgSqlCompilationsSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                return Convert.ToUInt64((this[this.Count - 1].SqlCompilationsSec - this[0].SqlCompilationsSec) / SecondsBetweenSnapshots);
            }
        }
        public UInt64 AvgSqlRecompilationsSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                return Convert.ToUInt64((this[this.Count - 1].SqlRecompilationsSec - this[0].SqlRecompilationsSec) / SecondsBetweenSnapshots);
            }
        }
        public double SecondsBetweenSnapshots
        {
            get
            {
                if (this.Count <= 1) return (1);
                double seconds = Math.Abs((this[this.Count - 1].AsOf - this[0].AsOf).TotalSeconds);
                return (0 == seconds ? 1 : seconds);
            }
        }
        public UInt64 Connections
        {
            get
            {
                if (this.Count <= 1) return (0);
                return (this[this.Count - 1].Connections - this[0].Connections);
            }
        }
        public UInt64 SumPackSentAndReceived
        {
            get
            {
                if (this.Count <= 1) return (0);
                var a = this[0];
                var b = this[this.Count - 1];
                UInt64 r = b.Pack_Sent - a.Pack_Sent;
                r += b.Pack_Received - a.Pack_Received;
                return (r);
            }
        }
        public UInt64 SumNetWaits
        {
            get
            {
                if (this.Count <= 1) return (0);
                var a = this[0];
                var b = this[this.Count - 1];
                UInt64 r = b.Async_Network_IO - a.Async_Network_IO;
                r += b.Net_Waitfor_Packet - a.Net_Waitfor_Packet;
                return (r);
            }
        }

        public UInt64 SumNonNetWaits
        {
            get
            {
                if (this.Count <= 1) return (0);
                var a = this[0];
                var b = this[this.Count - 1];
                UInt64 r = b.WriteLog - a.WriteLog;
                r += b.LogBuffer - a.LogBuffer;
                r += b.Pageiolatch_NL - a.Pageiolatch_NL;
                r += b.Pageiolatch_KP - a.Pageiolatch_KP;
                r += b.Pageiolatch_SH - a.Pageiolatch_SH;
                r += b.Pageiolatch_UP - a.Pageiolatch_UP;
                r += b.Pageiolatch_EX - a.Pageiolatch_EX;
                r += b.Pageiolatch_DT - a.Pageiolatch_DT;
                r += b.Async_IO_Completion - a.Async_IO_Completion;
                r += b.IO_Completion - a.IO_Completion;
                r += b.Logmgr - a.Logmgr;
                r += b.CXPacket - a.CXPacket;
                r += b.Sos_Scheduler_Yield - a.Sos_Scheduler_Yield;
                return (r);
            }
        }

        public UInt64 SumIoWaits
        {
            get
            {
                if (this.Count <= 1) return (0);
                var a = this[0];
                var b = this[this.Count - 1];
                UInt64 r = b.WriteLog - a.WriteLog;
                r += b.LogBuffer - a.LogBuffer;
                r += b.Pageiolatch_NL - a.Pageiolatch_NL;
                r += b.Pageiolatch_KP - a.Pageiolatch_KP;
                r += b.Pageiolatch_SH - a.Pageiolatch_SH;
                r += b.Pageiolatch_UP - a.Pageiolatch_UP;
                r += b.Pageiolatch_EX - a.Pageiolatch_EX;
                r += b.Pageiolatch_DT - a.Pageiolatch_DT;
                r += b.Async_IO_Completion - a.Async_IO_Completion;
                r += b.IO_Completion - a.IO_Completion;
                r += b.Logmgr - a.Logmgr;
                return (r);
            }
        }

        public UInt64 SumTotalWaits
        {
            get
            {
                if (this.Count > 1)
                    return this[this.Count - 1].TotalWaits - this[0].TotalWaits;
                return 0;
            }
        }

        public Int64 SumTempDbMetadataWaits
        {
            get
            {
                Int64 sum = 0;
                foreach (SampledServerResources ssr in this)
                {
                    sum += ssr.TempDbMetadataWaits;
                }
                return sum;
            }
        }
        public Int32 TempDbMetadataWaitingSampleCounts
        {
            get
            {

                Int32 count = 0;
                foreach (SampledServerResources ssr in this)
                {
                    if (ssr.TempDbMetadataWaits > 0)
                        count++;
                }
                return count;
            }
        }
    }
}
