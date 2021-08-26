using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.AdHoc;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TEBatchComplete : TEBase
    {
        public TEBatchComplete(DataRow dr)
            : base(dr) 
        { 
        }
        public TEBatchComplete(SqlDataReader r)
            : base(r)
        {
        }
        public TEBatchComplete(AdHocBatch b)
            : base(b)
        {
        }
    }
}
