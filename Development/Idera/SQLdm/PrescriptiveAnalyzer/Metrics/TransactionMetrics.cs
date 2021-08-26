using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class TransactionMetrics : BaseMetrics
    {
        public int OpenTransactionCount { get; set; }
        public IEnumerable<OpenTransaction> OpenTransactions { get; set; }
    }
}
