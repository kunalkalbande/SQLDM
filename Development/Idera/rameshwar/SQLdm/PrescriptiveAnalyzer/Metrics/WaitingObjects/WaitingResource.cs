using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects
{
    public class WaitingResource
    {
        public UInt32 DB { get; private set; }
        public UInt32 File { get; private set; }
        public UInt64 Page { get; private set; }
        public int Count { get; private set; }
        public WaitingResource(UInt32 db, UInt32 file, UInt64 page)
        {
            DB = db;
            File = file;
            Page = page;
            Count = 1;
        }
        public WaitingResource(WaitingResource wr) : this(wr.DB, wr.File, wr.Page)
        {
            Count = wr.Count;
        }
        public void Add(WaitingResource wr)
        {
            System.Diagnostics.Debug.Assert(DB== wr.DB);
            System.Diagnostics.Debug.Assert(File == wr.File);
            System.Diagnostics.Debug.Assert(Page == wr.Page);
            Count += wr.Count;
        }
    }
}
