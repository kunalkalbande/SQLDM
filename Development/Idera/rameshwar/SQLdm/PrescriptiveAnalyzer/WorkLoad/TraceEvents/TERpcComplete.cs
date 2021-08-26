using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TERpcComplete : TEBase
    {
        public TERpcComplete(DataRow dr)
            : base(dr) 
        { 
        }
        public TERpcComplete(SqlDataReader r)
            : base(r)
        {
        }
    }
}
