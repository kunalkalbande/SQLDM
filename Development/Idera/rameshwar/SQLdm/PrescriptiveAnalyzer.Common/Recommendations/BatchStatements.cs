using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class BatchStatements
    {
        private readonly string _batch;
        private readonly List<string> _statements = new List<string>();
        public string Batch { get { return (_batch); } }
        public IEnumerable<string> Statements { get { return (_statements); } }
        public BatchStatements() { }
        public BatchStatements(string batch, IEnumerable<string> statements) 
        { 
            _batch = batch;
            if (null != statements) { _statements.AddRange(statements); }
        }
    }
}
