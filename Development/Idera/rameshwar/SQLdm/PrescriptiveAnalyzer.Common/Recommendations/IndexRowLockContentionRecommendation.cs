using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexRowLockContentionRecommendation : IndexLockContentionRecommendation
    {
        public IndexRowLockContentionRecommendation(string db, 
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
            : base(IndexLockType.Row, db, schema, table, name, partition, lockCount, waitCount, lockPercent, waitMs, avgWaitMs, serverUpSeconds)
        {
        }

        public IndexRowLockContentionRecommendation(RecommendationProperties recProp)
            : base(IndexLockType.Row, recProp)
        { }

    }
}
