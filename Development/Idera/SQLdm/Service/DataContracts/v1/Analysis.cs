using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class Analysis
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public Exception Error { get; set; }

        [DataMember]
        public int SQLServerID { get; set; }

        [DataMember]
        public int AnalysisID { get; set; }

        [DataMember]
        public DateTime AnalysisStartTime { get; set; }

        [DataMember]
        public DateTime AnalysisCompleteTime { get; set; }

        [DataMember]
        public int TotalRecommendationCount { get; set; }

        [DataMember]
        public string AnalysisDuration { get; set; }

        [DataMember]
        public float ComputedRankFactor { get; set; }
    }
}
