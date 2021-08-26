using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Wintellect.PowerCollections;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class CloseCursorAnalyzer : AbstractQueryAnalyzer
    {
        private enum CursorState
        {
            Declared,
            Open,
            Closed,
            Deallocated
        }

        private const Int32 id = 101;
        private Set<string> openCursorMap;
        private Set<string> declaredCursorMap;
        private Dictionary<string, string> cursorVariableMap;
        private Dictionary<string, TSqlStatement> cursorMap;

        public CloseCursorAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlBatch batchParseTree)
        {
            if (cursorMap == null)
                cursorMap = new Dictionary<string, TSqlStatement>();
            else
                cursorMap.Clear();
            if (declaredCursorMap == null)
                declaredCursorMap = new Set<string>();
            else
                declaredCursorMap.Clear();
            if (openCursorMap == null)
                openCursorMap = new Set<string>();
            else
                openCursorMap.Clear();
            if (cursorVariableMap == null)
                cursorVariableMap = new Dictionary<string, string>();
            else
                cursorVariableMap.Clear();

            base.Analyze(script, batchParseTree);

            if (openCursorMap.Count > 0 || declaredCursorMap.Count > 0)
            {
                Set<string> cursors = new Set<string>(openCursorMap);
                cursors.AddMany(declaredCursorMap);
                foreach (string name in cursors)
                {
                    TSqlStatement declare = null;
                    if (!cursorMap.TryGetValue(name, out declare))
                        continue;
                    
                    OffendingSql sql = new OffendingSql();
                    sql.Script = script;
                    sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(declare);

                    TSqlCursorRecommendation recommendation = new TSqlCursorRecommendation(RecommendationType.DeallocateCursor, Database, Application, User, Host);
                    recommendation.Sql = sql;
                    recommendation.CursorName = name;
                    AddRecommendation(recommendation);
                }
            }
            cursorMap.Clear();
            openCursorMap.Clear();
            declaredCursorMap.Clear();
            cursorVariableMap.Clear();
        }

 
        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);
            if (fragment is UseStatement)
                return;

            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);

            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is SetVariableStatement)
                {
                    CheckSetCursor((SetVariableStatement)frag);
                    continue;
                } 
                if (frag is DeclareCursorStatement)
                {
                    CheckCursor((DeclareCursorStatement)frag);
                    continue;
                }
                if (frag is OpenCursorStatement)
                {
                    CheckOpen((OpenCursorStatement)frag);
                    continue;
                }
                if (frag is CloseCursorStatement)
                {
                    CheckClose((CloseCursorStatement)frag);
                    continue;
                }
                if (frag is DeallocateCursorStatement)
                {
                    CheckDeallocate((DeallocateCursorStatement)frag);
                    continue;
                }
            }
        }

        private void CheckOpen(OpenCursorStatement openCursorStatement)
        {
            string name = TSqlParsingHelpers.GetCursorName(openCursorStatement.Cursor);
            if (!String.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                if (!openCursorMap.Contains(name))
                {
                    openCursorMap.Add(name);
                }
            }
        }

        private void CheckDeallocate(DeallocateCursorStatement deallocateCursorStatement)
        {
            string name = TSqlParsingHelpers.GetCursorName(deallocateCursorStatement.Cursor);
            if (!String.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                if (declaredCursorMap.Contains(name))
                {
                    declaredCursorMap.Remove(name);
                } else
                if (name[0] == '@')
                {
                    string cname = null;
                    if (cursorVariableMap.TryGetValue(name, out cname))
                    {
                        if (declaredCursorMap.Contains(cname))
                        {
                            declaredCursorMap.Remove(cname);
                            cursorVariableMap.Remove(name);
                        }
                    }
                }
            }
        }

        private void CheckClose(CloseCursorStatement closeCursorStatement)
        {
            string name = TSqlParsingHelpers.GetCursorName(closeCursorStatement.Cursor);
            if (!String.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                if (openCursorMap.Contains(name))
                {
                    openCursorMap.Remove(name);
                } else
                if (name[0] == '@')
                {
                    string cname = null;
                    if (cursorVariableMap.TryGetValue(name, out cname))
                    {
                        if (openCursorMap.Contains(cname))
                        {
                            openCursorMap.Remove(cname);
                        }
                    }
                }
            }
        }

        private void CheckSetCursor(SetVariableStatement set)
        {
            if (set.CursorDefinition != null)
            {
                // cursor declared as part of set - create a decl cursor stmt, add to maps
                string var = set.VariableName.Value.ToLower();
                if (cursorMap.ContainsKey(var))
                    cursorMap.Remove(var);
                cursorMap.Add(var, set);
                declaredCursorMap.Add(var);
                cursorVariableMap.Add(var, var);
            }
            else
            if (set.AssignmentKind == AssignmentKind.Equals && set.Expression is Column)
            {   // assigning a cursor to a cursor variable - just add to cursor var map
                string name = ((Column)set.Expression).Identifiers[0].Value;
                if (cursorMap.ContainsKey(name.ToLower()))
                {
                    string var = set.VariableName.Value.ToLower();
                    if (cursorVariableMap.ContainsKey(var))
                        cursorVariableMap.Remove(var);
                    cursorVariableMap.Add(var, name.ToLower());
                }
            }
        }

        private void CheckCursor(DeclareCursorStatement declare)
        {
            string name = declare.Name.Value.ToLower();

            // add/replace cursor definition
            if (cursorMap.ContainsKey(name))
            {
                cursorMap.Remove(name);
            }
            cursorMap.Add(name, declare);

            // add to declared map
            if (!declaredCursorMap.Contains(name))
                declaredCursorMap.Add(name);
        }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is FunctionCall)
                return false;
            else if (fragment is ExistsPredicate)
                return false;
            else if (fragment is Subquery)
                return false;
            else if (fragment is SelectStatement)
                return false;
            else if (fragment is DeclareCursorStatement)
                return false;
            else if (fragment is UpdateStatement)
                return false;
            else if (fragment is DeleteStatement)
                return false;
            else if (fragment is InsertStatement)
                return false;
            else if (fragment is FetchCursorStatement)
                return false;
            else if (fragment is CursorDefinition)
                return false;
            else if (fragment is SetVariableStatement)
                return false;

                return true;
        }       
        

    }
}


