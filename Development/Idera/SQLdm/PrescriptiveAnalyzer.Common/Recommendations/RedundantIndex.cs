using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class RedundantIndex
    {
        public string Name { get; private set; }
        public long Size { get; private set; }
        private RedundantIndex() { }
        public RedundantIndex(string name, long size) { Name = name; Size = size; }
    }
}
