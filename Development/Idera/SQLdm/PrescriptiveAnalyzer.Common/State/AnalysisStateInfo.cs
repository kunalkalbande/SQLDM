using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.State
{
    [Serializable]
    public class AnalysisStateInfo 
    {
        private AnalysisStateType _ast;
        private string _status;
        private int _current;
        private int _max;
        private bool _cancelled = false;

        public AnalysisStateType AnalysisStateType { get { return (_ast); } }
        public int Current { get { return (_current); } }
        public int Max { get { return (_max); } }
        public string Status { get { return (_status); } }
        public bool Cancelled { get { return (_cancelled); } }
        internal List<AnalysisStateInfoHistory> History { get { return (_history); } }

        private List<AnalysisStateInfoHistory> _history = new List<AnalysisStateInfoHistory>();

        internal void Update(AnalysisStateType ast, string status, int current, int max)
        {
            if (_cancelled) return;
            _ast = ast;
            _status = status;
            _current = current;
            _max = max;
            _history.Add(new AnalysisStateInfoHistory(status));
        }

        internal void Update(AnalysisStateType ast, int current, int max)
        {
            _current = current;
            _max = max;
        }

        internal void Update(AnalysisStateType ast, string status)
        {
            Update(ast, status, _current, _max);
        }

        internal void Cancel(AnalysisStateType ast, string status)
        {
            if (_cancelled) return;
            _cancelled = true;
            _current = 100;
            _max = 100;
            _history.Add(new AnalysisStateInfoHistory(status));
        }

        internal AnalysisStateInfo Clone()
        {
            AnalysisStateInfo asi = new AnalysisStateInfo();
            asi._ast = this._ast;
            asi._cancelled = this._cancelled;
            asi._current = this._current;
            asi._history = new List<AnalysisStateInfoHistory>(this._history);
            asi._max = this._max;
            asi._status = this._status;
            return (asi);
        }
    }
}
