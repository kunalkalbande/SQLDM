using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MissingIndexCost
    {
        private readonly UInt64 _executedCount;
        private readonly string _batch;
        private readonly double _before;
        private readonly double _after;

        public UInt64 ExecutedCount { get { return (_executedCount); } }
        public string Batch { get { return (_batch); } }
        public double TotalCostBefore { get { return (_before); } }
        public double TotalCostAfter { get { return (_after); } }

        public MissingIndexCost() { }

        public MissingIndexCost(UInt64 count, string batch, double before, double after) 
        {
            _executedCount = count;
            _batch = batch;
            _before = before;
            _after = after;
        }
        public override string ToString()
        {
            if (TotalCostBefore > TotalCostAfter)
            {
                return (string.Format("Batch{0} with {1} improvement",
                                    (0 == _executedCount) ? "" :
                                        (" executed " + _executedCount.ToString() + (1 == _executedCount ? " time" : " times")),
                                    GetPercentString(TotalCostBefore, TotalCostAfter)
                                    ));
            }
            return (string.Format("Batch{0} with a cost increase of {1}",
                                    " executed " + _executedCount.ToString() + (1 == _executedCount ? " time" : " times"),
                                    GetPercentString(TotalCostAfter, TotalCostBefore)));
        }

        private string GetPercentString(double before, double after)
        {
            if (0 == before) return ("0%");
            return (string.Format("{0:0.#}%", ((1 - (after / before)) * 100.0)));
        }
    }
}
