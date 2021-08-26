using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexPageLockContentionRecommendation : IndexLockContentionRecommendation
    {
        public IndexPageLockContentionRecommendation(string db, 
                                                        string schema, 
                                                        string table, 
                                                        string name, 
                                                        int partition,
                                                        long lockCount,
                                                        long waitCount,
                                                        decimal lockPercent,
                                                        long waitMs,
                                                        decimal avgWaitMs,
                                                        UInt64 serverUpSeconds
                                                        )
            : base(IndexLockType.Page, db, schema, table, name, partition, lockCount, waitCount, lockPercent, waitMs, avgWaitMs, serverUpSeconds)
        {
        }

        public IndexPageLockContentionRecommendation(RecommendationProperties recProp)
            : base(IndexLockType.Page, recProp)
        {
        }
    }
}
