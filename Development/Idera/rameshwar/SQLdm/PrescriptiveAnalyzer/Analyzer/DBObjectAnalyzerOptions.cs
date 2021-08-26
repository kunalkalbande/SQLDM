using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class DBObjectAnalyzerOptions : BaseOptions
    {
        public DBObjectAnalyzerOptions(AnalysisState state) :base(state){ }
    }
}
