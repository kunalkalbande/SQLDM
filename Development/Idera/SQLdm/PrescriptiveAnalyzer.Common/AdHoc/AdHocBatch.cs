using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.AdHoc
{
    [Serializable]
    public class AdHocBatch
    {
        public string Batch { get; set; }
        public int ExecutionCount { get; set; }
        public UInt32 DBID { get; set; }

        public AdHocBatch(string batch, int executionCount, UInt32 dbid)
        {
            Batch = batch;
            ExecutionCount = executionCount;
            DBID = dbid;
        }
    }
}
