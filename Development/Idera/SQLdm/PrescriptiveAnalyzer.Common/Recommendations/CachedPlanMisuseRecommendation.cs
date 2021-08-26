using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class CachedPlanMisuseRecommendation : TSqlRecommendation
    {
        private ulong _executionCount;
        private ulong _minDuration;
        private ulong _minReads;
        private ulong _minCPU;
        private ulong _maxDuration;
        private ulong _maxReads;
        private ulong _maxCPU;
        private string _objectName;

        public CachedPlanMisuseRecommendation()
        {

        }

        public CachedPlanMisuseRecommendation(String database, String application, String user, String host)
            : base(RecommendationType.CachedPlanMisuse, database, application, user, host)
        {
        }
        public CachedPlanMisuseRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CachedPlanMisuse, recProp)
        {
            _executionCount = recProp.GetULong("_executionCount");
            _minDuration = recProp.GetULong("_minDuration");
            _minReads = recProp.GetULong("_minReads");
            _minCPU = recProp.GetULong("_minCPU");
            _maxDuration = recProp.GetULong("_maxDuration");
            _maxReads = recProp.GetULong("_maxReads");
            _maxCPU = recProp.GetULong("_maxCPU");
            _objectName = recProp.GetString("_objectName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("_executionCount", _executionCount.ToString());
            prop.Add("_minDuration", _minDuration.ToString());
            prop.Add("_minReads", _minReads.ToString());
            prop.Add("_minCPU", _minCPU.ToString());
            prop.Add("_maxDuration", _maxDuration.ToString());
            prop.Add("_maxReads", _maxReads.ToString());
            prop.Add("_maxCPU", _maxCPU.ToString());
            prop.Add("_objectName", _objectName.ToString());
            return prop;
        }

        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; }
        }

        public ulong ExecutionCount
        {
            get { return _executionCount; }
            set { _executionCount = value; }
        }

        public ulong MinimumDuration
        {
            get { return _minDuration / 1000; }
            set { _minDuration = value; }
        }

        public ulong MinimumReads
        {
            get { return _minReads; }
            set { _minReads = value; }
        }

        public ulong MinimumCPU
        {
            get { return _minCPU; }
            set { _minCPU = value; }
        }

        public ulong MaximumDuration
        {
            get { return _maxDuration / 1000; }
            set { _maxDuration = value; }
        }

        public ulong MaximumReads
        {
            get { return _maxReads; }
            set { _maxReads = value; }
        }

        public ulong MaximumCPU
        {
            get { return _maxCPU; }
            set { _maxCPU = value; }
        }

    }
}
