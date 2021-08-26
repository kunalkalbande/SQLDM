using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class DataBucket
    {
        private UInt64 _totalCount = 0;

        /// <summary>
        /// Total buckets for all executions.
        /// </summary>
        private UInt64 _totalDuration = 0;
        private UInt64 _totalReads = 0;
        private UInt64 _totalWrites = 0;
        private UInt64 _totalCPU = 0;

        private TEBase _highDuration = null;
        private TEBase _lowDuration = null;
        private TEBase _highReads = null;
        private TEBase _lowReads = null;
        private TEBase _highWrites = null;
        private TEBase _lowWrites = null;
        private TEBase _highCPU = null;
        private TEBase _lowCPU = null;

        private StringCache _tsqlCache = null;

        public readonly long TextDataKey;
        public string TextData
        {
            get
            {
                return (_tsqlCache.GetString(TextDataKey));
            }
        }
        public readonly long TextNormalizedKey;
        public string TextNormalized
        {
            get
            {
                return (_tsqlCache.GetString(TextNormalizedKey));
            }
        }

        public DataBucket(StringCache tsqlCache, string textData, string textNormalized) 
        {
            _tsqlCache = tsqlCache;
            TextDataKey = _tsqlCache.AddString(textData);
            TextNormalizedKey = _tsqlCache.AddString(textNormalized);
        }

        public UInt64 TotalCount { get { return (_totalCount); } }
        public UInt64 TotalDuration { get { return (_totalDuration); } }
        public UInt64 TotalReads { get { return (_totalReads); } }
        public UInt64 TotalWrites { get { return (_totalWrites); } }
        public UInt64 TotalCPU { get { return (_totalCPU); } }

        public TEBase HighDuration { get { return (_highDuration); } }
        public TEBase LowDuration { get { return (_lowDuration); } }
        public TEBase HighReads { get { return (_highReads); } }
        public TEBase LowReads { get { return (_lowReads); } }
        public TEBase HighWrites { get { return (_highWrites); } }
        public TEBase LowWrites { get { return (_lowWrites); } }
        public TEBase HighCPU { get { return (_highCPU); } }
        public TEBase LowCPU { get { return (_lowCPU); } }

        public bool TraceCollectedEvent { get; private set; }

        internal bool Add(TEBase te)
        {
            te.SetParentDataBucket(this);
            TEWorstTSQL w = te as TEWorstTSQL;
            if (null != w)
            {
                if (TraceCollectedEvent) return (false);
                AddWorstTSQL(w);
                return (true);
            }
            TraceCollectedEvent = true;
            ++_totalCount;
            _totalDuration += te.Duration;
            _totalReads += te.Reads;
            _totalWrites += te.Writes;
            _totalCPU += te.CPU;

            if (null == _highDuration || _highDuration < te) _highDuration = te;
            if (null == _lowDuration ||  _lowDuration > te) _lowDuration = te;
            if (null == _highReads || _highReads.Reads < te.Reads) _highReads = te;
            if (null == _lowReads || _lowReads.Reads > te.Reads) _lowReads = te;
            if (null == _highWrites || _highWrites.Writes < te.Writes) _highWrites = te;
            if (null == _lowWrites || _lowWrites.Writes> te.Writes) _lowWrites = te;
            if (null == _highCPU || _highCPU.CPU < te.CPU) _highCPU = te;
            if (null == _lowCPU || _lowCPU.CPU > te.CPU) _lowCPU = te;
            return (true);
        }

        private void AddWorstTSQL(TEWorstTSQL w)
        {
            _totalCount = w.ExecutionCount;
            _totalDuration = w.TotalTime;
            _totalReads = w.TotalReads;
            _totalWrites = w.TotalWrites;
            _totalCPU = w.TotalCPU;

            _highDuration = w;
            _highReads = w;
            _highWrites = w;
            _highCPU = w;
            TEWorstTSQL wMin = w.CloneMin();
            _lowDuration = wMin;
            _lowReads = wMin;
            _lowWrites = wMin;
            _lowCPU = wMin;
        }

        public IEnumerable<DataBucketProp> GetProps()
        {
            yield return (new DataBucketProp("TotalCount", _totalCount));
            yield return (new DataBucketProp("TotalDuration", _totalDuration));
            yield return (new DataBucketProp("TotalReads", _totalReads));
            yield return (new DataBucketProp("TotalWrites", _totalWrites));
            yield return (new DataBucketProp("TotalCPU", _totalCPU));
        }


        internal void Merge(DataBucket dBucket)
        {
            _totalCount += dBucket._totalCount;
            _totalDuration += dBucket._totalDuration;
            _totalReads += dBucket._totalReads;
            _totalWrites += dBucket._totalWrites;
            _totalCPU += dBucket._totalCPU;

            if (null != dBucket._highDuration) if (null == _highDuration || _highDuration < dBucket._highDuration) _highDuration = dBucket._highDuration;
            if (null != dBucket._lowDuration) if (null == _lowDuration || _lowDuration > dBucket._lowDuration) _lowDuration = dBucket._lowDuration;
            if (null != dBucket._highReads) if (null == _highReads || _highReads.Reads < dBucket._highReads.Reads) _highReads = dBucket._highReads;
            if (null != dBucket._lowReads) if (null == _lowReads || _lowReads.Reads > dBucket._lowReads.Reads) _lowReads = dBucket._lowReads;
            if (null != dBucket._highWrites) if (null == _highWrites || _highWrites.Writes < dBucket._highWrites.Writes) _highWrites = dBucket._highWrites;
            if (null != dBucket._lowWrites) if (null == _lowWrites || _lowWrites.Writes > dBucket._lowWrites.Writes) _lowWrites = dBucket._lowWrites;
            if (null != dBucket._highCPU) if (null == _highCPU || _highCPU.CPU < dBucket._highCPU.CPU) _highCPU = dBucket._highCPU;
            if (null != dBucket._lowCPU) if (null == _lowCPU || _lowCPU.CPU > dBucket._lowCPU.CPU) _lowCPU = dBucket._lowCPU;
        }

        internal void DumpData(BBS.TracerX.Logger _logX)
        {
            using (_logX.InfoCall("DataBucket.DumpData()"))
            {
                _logX.InfoFormat("TotalCount = {0}", TotalCount);
                _logX.InfoFormat("TotalDuration = {0}", TotalDuration);
                _logX.InfoFormat("TotalReads = {0}", TotalReads);
                _logX.InfoFormat("TotalWrites = {0}", TotalWrites);
                _logX.InfoFormat("TotalCPU = {0}", TotalCPU);
                if (null != _highDuration)
                {
                    _highDuration.DumpData(_logX);
                }
            }
        }
    }

    public class DataBucketProp
    {
        public string Name;
        public UInt64 Count;
        private DataBucketProp() { }
        public DataBucketProp(string name, UInt64 count) { Name = name; Count = count; }
    }

    public class DataBucketRanking : IComparable
    {
        public UInt64 Count;
        public DataBucket Bucket;
        private DataBucketRanking() { }
        public DataBucketRanking(UInt64 count, DataBucket bucket) { Count = count; Bucket = bucket; }

        public override string ToString()
        {
            return (string.Format("Ranking Count: " + Count));
        }

        public int CompareTo(object obj)
        {
            DataBucketRanking bucket = obj as DataBucketRanking;
            if (null != bucket) return (this.Count.CompareTo(bucket.Count) * -1);
            return (-1);
        }
    }
    public class BucketWithPlan
    {
        public DataBucket Bucket;
        public TraceEventPlan Plan;
        private BucketWithPlan() { }
        public BucketWithPlan(DataBucket bucket, TraceEventPlan plan) { Bucket = bucket; Plan = plan; }
    }

}
