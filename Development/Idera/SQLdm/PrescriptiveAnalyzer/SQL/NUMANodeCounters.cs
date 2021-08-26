using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
//using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class NUMANodeCounters
    {
        public string NodeName { get; private set; }
        public UInt64 PageLifeExpectancy { get; private set; }
        public UInt64 TargetPages { get; private set; }

        public NUMANodeCounters(DataRow dr) 
        {
            NodeName = DataHelper.ToString(dr, 0);
            PageLifeExpectancy = DataHelper.ToUInt64(dr, 1);
            TargetPages = DataHelper.ToUInt64(dr, 2);
        }
    }
}