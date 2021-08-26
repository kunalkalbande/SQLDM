using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.State
{
    [Serializable]
    public class AnalysisStateInfoHistory
    {
        private string _status;
        private DateTime _tstamp;
        public DateTime Time { get { return (_tstamp); } }
        public string Status { get { return (_status); } }
        public AnalysisStateInfoHistory(string status)
        {
            _status = status;
            _tstamp = DateTime.Now;
        }
        public override string ToString()
        {
            return (string.Format("{0} - {1}", _tstamp, _status));
        }
    }
}
