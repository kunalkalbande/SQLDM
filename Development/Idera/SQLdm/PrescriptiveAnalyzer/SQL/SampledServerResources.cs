using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
//using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class SampledServerResources
    {
        public DateTime AsOf { get; private set; }
        public UInt64 BatchReqSec { get; private set; }
        public UInt64 PageLifeExpectancy { get; private set; }
        public UInt64 SqlCompilationsSec { get; private set; }
        public UInt64 SqlRecompilationsSec { get; private set; }
        public UInt64 Pack_Sent { get; private set; }
        public UInt64 Pack_Received { get; private set; }
        public UInt64 Connections { get; private set; }
        public UInt64 Async_Network_IO { get; private set; }
        public UInt64 Net_Waitfor_Packet { get; private set; }
        public UInt64 WriteLog { get; private set; }
        public UInt64 LogBuffer { get; private set; }
        public UInt64 Pageiolatch_NL { get; private set; }
        public UInt64 Pageiolatch_KP { get; private set; }
        public UInt64 Pageiolatch_SH { get; private set; }
        public UInt64 Pageiolatch_UP { get; private set; }
        public UInt64 Pageiolatch_EX { get; private set; }
        public UInt64 Pageiolatch_DT { get; private set; }
        public UInt64 Async_IO_Completion { get; private set; }
        public UInt64 IO_Completion { get; private set; }
        public UInt64 Logmgr { get; private set; }
        public UInt64 CXPacket { get; private set; }
        public UInt64 Sos_Scheduler_Yield { get; private set; }
        public UInt64 TotalWaits { get; private set; }
        public Int64  TempDbMetadataWaits { get; private set; }
        public UInt64 BufferCacheHitRatioRaw { get; private set; }
        public UInt64 BufferCacheHitRatioBaseRaw { get; private set; }
        public UInt64 PageSplitsSec { get; private set; }
        public UInt64 PagesAllocatedSec { get; private set; }
        public UInt64 MemoryGrantsPending { get; private set; }
        public double BufferCacheHitRatio { get { return (BufferCacheHitRatioBaseRaw > 0 ? 100 * (BufferCacheHitRatioRaw / (double)BufferCacheHitRatioBaseRaw) : 0); } }

        public UInt64 IoWaits
        {
            get
            {
                return WriteLog + LogBuffer + 
                       Pageiolatch_NL + Pageiolatch_KP + 
                       Pageiolatch_SH + Pageiolatch_UP + 
                       Pageiolatch_EX + Pageiolatch_DT +
                       Async_IO_Completion + IO_Completion + Logmgr;
            }
        }

        private SampledServerResources() { }
        public SampledServerResources(DataTable dt) 
        {
            if (null == dt) return;
            if (null == dt.Rows) return;
            if (null == dt.Columns) return;
            if (dt.Columns.Count < 2) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (null == dr[0]) continue;
                switch (dr[0].ToString().ToLower())
                {
                    case ("asof"): { AsOf = DataHelper.ToDateTime(dr, 1, CultureInfo.CreateSpecificCulture("en-US")); break; }
                    case ("page life expectancy"): { PageLifeExpectancy = DataHelper.ToUInt64(dr, 1); break; }
                    case ("batch requests/sec"): { BatchReqSec = DataHelper.ToUInt64(dr, 1); break; }
                    case ("sql compilations/sec"): { SqlCompilationsSec = DataHelper.ToUInt64(dr, 1); break; }
                    case ("sql re-compilations/sec"): { SqlRecompilationsSec = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pack_sent"): { Pack_Sent = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pack_received"): { Pack_Received = DataHelper.ToUInt64(dr, 1); break; }
                    case ("connections"): { Connections = DataHelper.ToUInt64(dr, 1); break; }
                    case ("async_network_io"): { Async_Network_IO = DataHelper.ToUInt64(dr, 1); break; }
                    case ("net_waitfor_packet"): { Net_Waitfor_Packet = DataHelper.ToUInt64(dr, 1); break; }
                    case ("writelog"): { WriteLog = DataHelper.ToUInt64(dr, 1); break; }
                    case ("logbuffer"): { LogBuffer = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_nl"): { Pageiolatch_NL = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_kp"): { Pageiolatch_KP = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_sh"): { Pageiolatch_SH = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_up"): { Pageiolatch_UP = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_ex"): { Pageiolatch_EX = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pageiolatch_dt"): { Pageiolatch_DT = DataHelper.ToUInt64(dr, 1); break; }
                    case ("async_io_completion"): { Async_IO_Completion = DataHelper.ToUInt64(dr, 1); break; }
                    case ("io_completion"): { IO_Completion = DataHelper.ToUInt64(dr, 1); break; }
                    case ("logmgr"): { Logmgr = DataHelper.ToUInt64(dr, 1); break; }
                    case ("cxpacket"): { CXPacket = DataHelper.ToUInt64(dr, 1); break; }
                    case ("sos_scheduler_yield"): { Sos_Scheduler_Yield = DataHelper.ToUInt64(dr, 1); break; }
                    case ("_total_waits"): { TotalWaits = DataHelper.ToUInt64(dr, 1); break; }
                    case ("tempdbmetadatawaits"): { TempDbMetadataWaits = (Int64)DataHelper.ToUInt64(dr, 1); break; }
                    case ("buffer cache hit ratio"): { BufferCacheHitRatioRaw = DataHelper.ToUInt64(dr, 1); break; }
                    case ("buffer cache hit ratio base"): { BufferCacheHitRatioBaseRaw = DataHelper.ToUInt64(dr, 1); break; }
                    case ("page splits/sec"): { PageSplitsSec = DataHelper.ToUInt64(dr, 1); break; }
                    case ("pages allocated/sec"): { PagesAllocatedSec = DataHelper.ToUInt64(dr, 1); break; }
                    case ("memory grants pending"): { MemoryGrantsPending = DataHelper.ToUInt64(dr, 1); break; }
                    default: { System.Diagnostics.Debug.WriteLine("Unknown Server Resource: " + dr[0].ToString()); break; }

                }
            }
        }    
    }
}
