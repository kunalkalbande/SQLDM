using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects
{
    public  class WaitingBatches
    {
        private Dictionary<string, WaitingBatch> _batches = new Dictionary<string, WaitingBatch>();
        public void Add(string batch, string program)
        {
            WaitingBatch wb = null;
            if (!_batches.TryGetValue(batch, out wb)) 
            {
                if (_batches.Count >= 100) return;
                wb = new WaitingBatch() { Batch = batch, Program = program };
                _batches[batch] = wb;
            }
            ++wb.Count;
        }

        public void Add(WaitingBatches batches)
        {
            if (null == batches) return;
            foreach (WaitingBatch wb in batches._batches.Values) Add(wb.Batch, wb.Program);
        }

        public AffectedBatches GetAffectedBatches() 
        {
            AffectedBatches abs = new AffectedBatches();
            foreach (WaitingBatch b in _batches.Values)
            {
                abs.Add(new AffectedBatch(b.Program, b.Batch));
            }
            return (abs); 
        }
    }
}
