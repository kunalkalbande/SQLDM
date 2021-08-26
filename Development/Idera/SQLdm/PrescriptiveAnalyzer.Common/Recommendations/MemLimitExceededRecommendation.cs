using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    //----------------------------------------------------------------------------
    // This recommendation is based on link: http://msdn.microsoft.com/en-us/library/ms189298.aspx
    // 
    //  If MemoryLimitExceeded is returned for the StatementOptmEarlyAbortReason attribute, 
    //  the XML Showplan produced will still be correct, but it may not be optimal. Try one of 
    //  the following methods to increase available memory: 
    //    1) Reduce the load on the server. 
    //    2) Increase memory available to SQL Server. For more information, see Managing Memory for Large Databases. 
    //    3) Check the max server memory option that is set with sp_configure, 
    //       and increase the value if it is too low. For more information, see Server Memory Options.
    //
    [Serializable]
    public class MemLimitExceededRecommendation : Recommendation
    {
        private readonly List<BatchStatements> _batches = new List<BatchStatements>();

        public List<BatchStatements> Batches { get { return (_batches); } }
        public UInt64 AvgCountPerMinute { get; private set; }

        public MemLimitExceededRecommendation() : base(RecommendationType.MemLimitExceeded) {}

        public MemLimitExceededRecommendation(RecommendationProperties recProp) : base(RecommendationType.MemLimitExceeded, recProp) 
        {
            AvgCountPerMinute = recProp.GetUInt64("AvgCountPerMinute");
            _batches = recProp.GetBatchStatementsList("_batches");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgCountPerMinute", AvgCountPerMinute.ToString());
            prop.Add("_batches", RecommendationProperties.GetXml<List<BatchStatements>>(_batches));
            return prop;
        }

        public MemLimitExceededRecommendation(IEnumerable<BatchStatements> batches, UInt64 count, int traceDurationMinutes) : this()
        {
            try
            {
                if (null != batches) _batches.AddRange(batches);
                AvgCountPerMinute = (traceDurationMinutes > 0) ? (count / Convert.ToUInt64(traceDurationMinutes)) : count;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("MemLimitExceededRecommendation() Exception:", ex);
            }
        }

        public override int AdjustImpactFactor(int i)
        {
            //---------------------------------------------------------------------------
            // Per Brett:
            //   Low - occurred less than 5 times per minute
            //   Med - occurred between 5 and 50 times per minute
            //   Hi  - occurred over 50 times per minute
            if (AvgCountPerMinute < 5) return (LOW_IMPACT);
            else if (AvgCountPerMinute <= 50) return (LOW_IMPACT + 1);
            else return (HIGH_IMPACT);
        }
    }
}
