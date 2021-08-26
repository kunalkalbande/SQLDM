using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TEWorstTSQL : TEBase
    {
        public int ObjectID { get; private set; }
        public string ObjectType { get; private set; }
        public string XMLPlan { get; private set; }
        public UInt64 ExecutionCount { get; private set; }
        public UInt64 TotalReads { get; private set; }
        public UInt64 MinReads { get; private set; }
        public UInt64 TotalWrites { get; private set; }
        public UInt64 MinWrites { get; private set; }
        public UInt64 TotalCPU { get; private set; }
        public UInt64 MinCPU { get; private set; }
        public UInt64 TotalTime { get; private set; }
        public UInt64 MinTime { get; private set; }

        public TEWorstTSQL(DataRow dr, SQLSchemaNameHelper ssnh, BaseOptions options)
            : base(dr)
        {
            ObjectID = DataHelper.ToInt32(dr, "objectid");
            ObjectType = DataHelper.ToString(dr, "objtype");
            ExecutionCount = DataHelper.ToUInt64(dr, "execution_count");
            TotalReads = DataHelper.ToUInt64(dr, "total_logical_reads");
            MinReads = DataHelper.ToUInt64(dr, "min_logical_reads");
            TotalWrites = DataHelper.ToUInt64(dr, "total_logical_writes");
            MinWrites = DataHelper.ToUInt64(dr, "min_logical_writes");
            TotalCPU = DataHelper.ToUInt64(dr, "total_worker_time");
            MinCPU = DataHelper.ToUInt64(dr, "min_worker_time");
            TotalTime = DataHelper.ToUInt64(dr, "total_elapsed_time");
            MinTime = DataHelper.ToUInt64(dr, "min_elapsed_time");
            XMLPlan = DataHelper.ToString(dr, "Plan");
            if ((0 != ObjectID) && (0 != DBID) && (string.IsNullOrEmpty(ObjectName)))
            {
                try
                {
                    ObjectName = ssnh.GetObjectName((UInt32)ObjectID, options.GetDatabaseName(DBID));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("TEWorstTSQL() Exception: ", ex);
                }
            }

        }
        public TEWorstTSQL(TEWorstTSQL w, bool cloneMin) : base(w, cloneMin) { }
        public TEWorstTSQL CloneMin()
        {
            TEWorstTSQL w = new TEWorstTSQL(this, true);
            w.ObjectID = ObjectID;
            w.ObjectType = ObjectType;
            w.XMLPlan = XMLPlan;
            w.ExecutionCount = ExecutionCount;
            w.TotalReads = TotalReads;
            w.MinReads = MinReads;
            w.TotalWrites = TotalWrites;
            w.MinWrites = MinWrites;
            w.TotalCPU = TotalCPU;
            w.MinCPU = MinCPU;
            w.TotalTime = TotalTime;
            w.MinTime = MinTime;
            return (w);
        }
        internal override void DumpData(BBS.TracerX.Logger _logX)
        {
            base.DumpData(_logX);
            _logX.InfoFormat("ObjectID = {0}", ObjectID);
            _logX.InfoFormat("ObjectType = {0}", ObjectType);
            _logX.InfoFormat("ExecutionCount = {0}", ExecutionCount);
            _logX.InfoFormat("TotalReads = {0}", TotalReads);
            _logX.InfoFormat("MinReads = {0}", MinReads);
            _logX.InfoFormat("TotalWrites = {0}", TotalWrites);
            _logX.InfoFormat("MinWrites = {0}", MinWrites);
            _logX.InfoFormat("TotalCPU = {0}", TotalCPU);
            _logX.InfoFormat("MinCPU = {0}", MinCPU);
            _logX.InfoFormat("TotalTime = {0}", TotalTime);
            _logX.InfoFormat("MinTime = {0}", MinTime);
            _logX.InfoFormat("XMLPlan = {0}", XMLPlan);
        }
    }
}
