using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using BBS.TracerX;


namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class BaseDataCollector
    {
        private static Logger _logX = Logger.GetLogger("BaseDataCollector");
        private static readonly int MAX_RANKED_RESULTS = 1000;
        private static readonly int MAX_CAPTURED_DATA = Settings.Default.Max_UniqueTraceBuckets;

        private object _dataLock = new object();
        private Dictionary<string, DataBucket> _data = new Dictionary<string, DataBucket>();
        private StringCache _tsqlCache = null;

        private DateTime _lastPurge = DateTime.MinValue;
        private Stopwatch _timeLookup = new Stopwatch();
        private Stopwatch _timeParser = new Stopwatch();
        private ulong _charsParsed = 0;
        static BaseDataCollector() { if (MAX_CAPTURED_DATA <= 0) MAX_CAPTURED_DATA = 5000; }

        private BaseDataCollector() { }
        internal BaseDataCollector(StringCache tsqlCache) { _tsqlCache = tsqlCache; }

        public IEnumerable<DataBucket> GetDataBuckets()
        {
            lock (_dataLock)
            {
                LogStats();
                foreach (DataBucket d in _data.Values)
                {
                    yield return d;
                }
            }
        }

        public int GetDataBucketCount()
        {
            lock (_dataLock)
            {
                return (_data.Count);
            }
        }

        public DataBucket AddData(TEBase te, TSql90Parser parser, SHA1 sha1)
        {
            if (null == te) return (null);
            //--------------------------------------------------------------
            // Ignore all events for the resource db.
            //
            if (SQLHelper.RESOURCEDB_ID == te.DBID) return (null);

            lock (_dataLock)
            {
                _timeParser.Start();
                DataBucket bData = null;
                bool parsed = string.IsNullOrEmpty(te.TextNormalized) && !string.IsNullOrEmpty(te.TextData);
                string textNormalized = te.GetTextNormalized(parser, sha1);

                _timeParser.Stop();
                if (parsed) _charsParsed += (ulong)te.TextData.Length;

                _timeLookup.Start();
                bool found = _data.TryGetValue(textNormalized, out bData);
                _timeLookup.Stop();

                if (!found)
                {
                    bData = null;
                    if (MAX_CAPTURED_DATA <= _data.Count) { PurgeData(); }
                    if (MAX_CAPTURED_DATA > _data.Count) { bData = new DataBucket(_tsqlCache, te.TextData, textNormalized); _data.Add(textNormalized, bData); }
                }
                if (null != bData) { bData.Add(te); }
                return (bData);
            }
        }

        private void PurgeData()
        {
            using (_logX.DebugCall("PurgeData()"))
            {
                try
                {
                    int count = 0;
                    _logX.DebugFormat("Unique data bucket count: {0}", _data.Count);
                    if (DateTime.MinValue != _lastPurge)
                    {
                        TimeSpan ts = DateTime.Now - _lastPurge;
                        _logX.DebugFormat("LastPurge:{0}  TimeSpan:{1}", _lastPurge, ts);
                        if (ts.TotalSeconds < 5)
                        {
                            _logX.DebugFormat("Last purge was {0} seconds ago.  Skipping this purge", ts.TotalSeconds);
                            return;
                        }
                    }
                    _lastPurge = DateTime.Now;
                    List<DataBucket> l = new List<DataBucket>(_data.Values);
                    foreach (DataBucket d in l)
                    {
                        if ((d.TotalCount <= 1) && (null != d.HighDuration))
                        {
                            if (_data.Remove(d.HighDuration.TextNormalized)) { ++count; }
                        }
                    }
                    _logX.DebugFormat("Data buckets removed count: {0}", count);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "PurgeData()", ex);
                }
            }
        }

        public Dictionary<string, List<DataBucketRanking>> GetResults()
        {
            lock (_dataLock)
            {
                LogStats();
                Dictionary<string, List<DataBucketRanking>> results = new Dictionary<string, List<DataBucketRanking>>();
                foreach (DataBucket bucket in _data.Values)
                {
                    ProcessBucket(results, bucket);
                }
                return (results);
            }
        }

        public void LogStats()
        {
            using (_logX.InfoCall("BaseDataCollector.LogStats()"))
            {
                _logX.Info(string.Format("Unique Data Buckets : {0}", _data.Count));
                _logX.Info(string.Format("Total time in BaseDataCollector Lookup {0} Parsing {1} Characters Parsed Total: {2} PerSec:{3}kb",
                                            _timeLookup.Elapsed,
                                            _timeParser.Elapsed,
                                            _charsParsed,
                                            ((_timeParser.Elapsed.TotalSeconds > 0) ?
                                                ((_charsParsed / 1024) / _timeParser.Elapsed.TotalSeconds) :
                                                0))
                                            );
                ProcessInfoHelper.Log(_logX);
            }
        }

        private void ProcessBucket(Dictionary<string, List<DataBucketRanking>> results, DataBucket bucket)
        {
            List<DataBucketRanking> rank;
            foreach (DataBucketProp prop in bucket.GetProps())
            {
                if (!results.TryGetValue(prop.Name, out rank))
                {
                    rank = new List<DataBucketRanking>(200);
                    results.Add(prop.Name, rank);
                }

                RankBucket(new DataBucketRanking(prop.Count, bucket), rank);
            }
        }

        private void RankBucket(DataBucketRanking bucket, List<DataBucketRanking> ranking)
        {
            if (ranking.Count <= 0) { ranking.Add(bucket); return; }
            if (ranking[0].Count < bucket.Count)
            {
                ranking.Insert(0, bucket);
                return;
            }
            else if (1 == ranking.Count)
            {
                ranking.Add(bucket);
                return;
            }

            int index = ranking.BinarySearch(bucket);
            ranking.Insert((index < 0) ? ~index : index, bucket);
            if (ranking.Count > MAX_RANKED_RESULTS) ranking.RemoveRange(MAX_RANKED_RESULTS, ranking.Count - MAX_RANKED_RESULTS);
        }

        internal void Merge(BaseDataCollector bdc)
        {
            using (_logX.InfoCall("BaseDataCollector.Merge()"))
            {
                DataBucket dBucket;
                int added = 0;
                int merged = 0;
                lock (_dataLock)
                {
                    lock (bdc._dataLock)
                    {
                        foreach (var kv in bdc._data)
                        {
                            dBucket = null;
                            if (!_data.TryGetValue(kv.Key, out dBucket))
                            {
                                _data.Add(kv.Key, kv.Value);
                                ++added;
                            }
                            else if (null != dBucket)
                            {
                                dBucket.Merge(kv.Value);
                                ++merged;
                            }
                        }
                    }
                }
                _logX.InfoFormat("DataBuckets Added:{0} Merged:{1}", added, merged);
            }
        }

        internal void DumpData()
        {
            using (_logX.InfoCall("BaseDataCollector.DumpData()"))
            {
                lock (_dataLock)
                {
                    foreach (DataBucket d in _data.Values)
                    {
                        try
                        {
                            d.DumpData(_logX);
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.Log(_logX, "DumpData Exception: ", ex);
                        }
                    }
                }
            }
        }
    }
}
