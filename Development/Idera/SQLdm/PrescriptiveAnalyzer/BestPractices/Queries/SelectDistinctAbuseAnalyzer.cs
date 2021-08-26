using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class SelectDistinctAbuseAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 112;
        internal SelectDistinctAbuseAnalyzer(string database) : base(database)
        {
            _id = id;
        }
    }
}
