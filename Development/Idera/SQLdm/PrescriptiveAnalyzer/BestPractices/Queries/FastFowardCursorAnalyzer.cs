using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class FastForwardCursorAnalyzer : AbstractQueryAnalyzer
    {
        Dictionary<string, TSqlStatement> cursorMap;
        Dictionary<string, string> cursorVariableMap;
        private const Int32 id = 103;

        public FastForwardCursorAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlBatch batchParseTree)
        {
            cursorVariableMap = null;
            if (cursorMap == null)
                cursorMap = new Dictionary<string, TSqlStatement>();
            else
                cursorMap.Clear();


            base.Analyze(script, batchParseTree);

            foreach (String cursorName in cursorMap.Keys)
            {
                TSqlStatement declare = cursorMap[cursorName];
                OffendingSql sql = new OffendingSql();
                sql.Script = script;
                sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(declare);

                TSqlCursorRecommendation recommendation = new TSqlCursorRecommendation(RecommendationType.FastForwardCursor, Database, Application, User, Host);
                recommendation.Sql = sql;
                recommendation.CursorName = cursorName;
                AddRecommendation(recommendation);
            }

            cursorMap.Clear();
            if (cursorVariableMap != null)
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
                if (frag is FetchCursorStatement)
                {
                    CheckFetch((FetchCursorStatement)frag);
                    continue;
                }
                if (frag is UpdateStatement)
                {
                    CheckUpdate((UpdateStatement)frag);
                    continue;
                }
                if (frag is DeleteStatement)
                {
                    CheckDelete((DeleteStatement)frag);
                    continue;
                }
            }
        }

        private void CheckSetCursor(SetVariableStatement set)
        {
            if (set.CursorDefinition != null)
            {
                if ((set.CursorDefinition.Options & CursorOptions.FastForward) == CursorOptions.FastForward)
                    return;

                // cursor declared as part of set - create a decl cursor stmt, add to maps
                cursorMap.Add(set.VariableName.Value, set);
                if (cursorVariableMap == null)
                    cursorVariableMap = new Dictionary<string, string>();
                string var = set.VariableName.Value.ToLower();
                cursorVariableMap.Add(var, var);
            }
            else
            if (set.AssignmentKind == AssignmentKind.Equals && set.Expression is Column)
            {
                string name = ((Column)set.Expression).Identifiers[0].Value;
                if (cursorMap.ContainsKey(name.ToLower()))
                {
                    string var = set.VariableName.Value.ToLower();
                    if (cursorVariableMap == null)
                        cursorVariableMap = new Dictionary<string, string>();
                    if (cursorVariableMap.ContainsKey(var))
                        cursorVariableMap.Remove(var);
                    cursorVariableMap.Add(var, name.ToLower());
                }
            }
        }

        private void CheckDelete(DeleteStatement deleteStatement)
        {
            if (deleteStatement.WhereClause != null && deleteStatement.WhereClause.Cursor != null)
            {
                string name = TSqlParsingHelpers.GetCursorName(deleteStatement.WhereClause.Cursor);
                if (String.IsNullOrEmpty(name))
                    return;
                // remove from map since it is used in a data modification operation
                UnMap(name);
            }
        }

        private void CheckUpdate(UpdateStatement updateStatement)
        {
            if (updateStatement.WhereClause != null && updateStatement.WhereClause.Cursor != null)
            {
                string name = TSqlParsingHelpers.GetCursorName(updateStatement.WhereClause.Cursor);
                if (String.IsNullOrEmpty(name))
                    return;
                // remove from map since it is used in a data modification operation
                UnMap(name);
            }
        }

        private void CheckFetch(FetchCursorStatement fetchCursorStatement)
        {
            FetchOrientation orientation = FetchOrientation.None;
            if (fetchCursorStatement.FetchType != null)
                orientation = fetchCursorStatement.FetchType.Orientation;
            if (orientation == FetchOrientation.None || orientation == FetchOrientation.Next)
                return;

            string name = TSqlParsingHelpers.GetCursorName(fetchCursorStatement.Cursor);
            if (String.IsNullOrEmpty(name))
                return;

            // remove from map since it is used in a data modification operation
            UnMap(name);
        }

        private void CheckCursor(DeclareCursorStatement declare)
        {
            CursorDefinition cd = declare.CursorDefinition;
            CursorOptions options = cd.Options;

            if ((options & CursorOptions.FastForward) == CursorOptions.FastForward)
                return;

            string name = declare.Name.Value.ToLower();
            if (cursorMap.ContainsKey(name))
            {
                cursorMap.Remove(name);
            }
            cursorMap.Add(name, declare);
        }

        private void UnMap(string name)
        {
            string varname = name.ToLower();

            if (name[0] == '@')
            {
               if (cursorVariableMap.TryGetValue(varname, out varname))
               {
                   cursorVariableMap.Remove(name.ToLower());
               }
            }

            if (cursorMap.ContainsKey(varname))
                cursorMap.Remove(varname);
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

                return true;
        }       
        

    }
}


