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
    public class WaitingDatabases : Dictionary<UInt32, WaitingDatabase>
    {
        public void Add(WaitingResource wr)
        {
            WaitingDatabase wd;
            if (!this.TryGetValue(wr.DB, out wd)) { this.Add(wr.DB, wd = new WaitingDatabase(wr.DB)); }
            wd.Add(wr);
        }
        public void Add(WaitingDatabase addWD)
        {
            WaitingDatabase wd;
            if (!this.TryGetValue(addWD.ID, out wd)) 
            {
                this.Add(addWD.ID, new WaitingDatabase(addWD));
                return;
            }
            wd.Add(addWD);
        }
        public void Add(WaitingDatabases databases)
        {
            foreach (WaitingDatabase wd in databases.Values) Add(wd);
        }
    }

}
