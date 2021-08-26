using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class OffendingSql
    {
        private string script;
        private string _stmt;
        private SelectionRectangle stmtSelection;
        private List<SelectionRectangle> focusSelections;
        public OffendingSql() { }
        public OffendingSql(string batch, IEnumerable<string> stmts) : this(batch, GetFirstStmt(stmts)) {}

        public OffendingSql(string batch, string stmt)
        {
            try
            {
                script = batch;
                _stmt = stmt;
                if (script.Length < _stmt.Length)
                {
                    string s = script;
                    script = _stmt;
                    _stmt = s;
                }
                int offset = script.IndexOf(_stmt);
                if (offset < 0) offset = 0;
                stmtSelection = new SelectionRectangle(new BufferLocation(offset, CountOccurrences(script.Substring(0, offset), '\r'), 0), _stmt.Length);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("OffendingSql({0}, {1})", batch, stmt), ex);
            }
        }

        private int CountOccurrences(string p, char c)
        {
            int count = 1;
            int i = 0;
            while ((i = p.IndexOf(c, i)) >= 0)
            {
                ++i;
                count++;
            }
            return (count);
        }

        public string Script
        {
            get { return script; }
            set { script = value; }
        }

        public SelectionRectangle StatementSelection
        {
            get { return stmtSelection; }
            set { stmtSelection = value; }
        }

        public bool HasFocusSelections
        {
            get { return focusSelections != null && focusSelections.Count > 0; }
        }

        public List<SelectionRectangle> FocusSelections
        {
            get
            {
                if (focusSelections == null)
                    focusSelections = new List<SelectionRectangle>();

                return focusSelections;
            }
            set
            {
                if (value != null && value.Count == 0)
                    value = null;
                focusSelections = value;
            }
        }

        private static string GetFirstStmt(IEnumerable<string> stmts)
        {
            if (null == stmts) return (string.Empty);
            System.Collections.IEnumerator e = stmts.GetEnumerator();
            while (e.MoveNext())
            {
                return (string)e.Current;
            }
            //if ( stmts. stmts.Count() > 0) return (stmts.First());
            return (string.Empty);
        }

        public void GenerateFocusSelectionsWithinPredicate(string text)
        {
            try
            {
                List<SelectionRectangle> l = new List<SelectionRectangle>();
                int starting = _stmt.IndexOf("where", 0, StringComparison.InvariantCultureIgnoreCase);
                if (starting < 0) starting = 0;
                int n = 0;
                int offset = 0;
                while (0 < (n = _stmt.IndexOf(text, starting, StringComparison.InvariantCultureIgnoreCase)))
                {
                    offset = n + stmtSelection.Start.Offset;
                    l.Add(new SelectionRectangle(new BufferLocation(offset, CountOccurrences(script.Substring(0, offset), '\r'), 0), text.Length));
                    starting = n + 1;
                }
                FocusSelections = l;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("OffendingSql.GenerateFocusSelectionsWithinPredicate({0})", text), ex);
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(script)) return (string.Empty);
            if (null == stmtSelection) return (script);
            if (null == stmtSelection.Start) return (script);
            if (0 > stmtSelection.Start.Offset) return (script);
            return script.Substring(stmtSelection.Start.Offset, stmtSelection.Length);
        }
    }
}
