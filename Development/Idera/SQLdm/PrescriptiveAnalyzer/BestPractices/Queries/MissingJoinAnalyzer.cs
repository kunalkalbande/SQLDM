using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class MissingJoinAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 109;
        internal MissingJoinAnalyzer(string database) : base(database)
        {
            _id = id;
        }

    }
}
