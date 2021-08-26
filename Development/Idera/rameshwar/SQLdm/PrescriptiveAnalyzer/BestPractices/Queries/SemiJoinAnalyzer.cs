using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Wintellect.PowerCollections;
using Idera.SQLdm.PrescriptiveAnalyzer.MetaData;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class SemiJoinAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 113;

        internal class MultilevelStore<T>
        {
            private readonly Deque<LevelStore<T>> scope;

            internal MultilevelStore()
            {
                scope = new Deque<LevelStore<T>>();
                // add the global scope
                Push();
            }

            internal void Push()
            {
                LevelStore<T> level = new LevelStore<T>();
                scope.AddToBack(level);
            }

            internal void Pop()
            {
                if (scope.Count > 1)
                {
                    LevelStore<T> store = scope.RemoveFromBack();
                    if (store != null)
                        store.Clear();
                }
                else
                    scope[0].Clear();
            }

            internal void Clear()
            {
                // pop until all thats left is the global scope
                while (scope.Count > 1)
                    Pop();
                // pop the global scope to clear it
                Pop();
            }

            internal T this[string key]
            {
                get
                {
                    T result = default(T);

                    for (int i = scope.Count - 1; i >= 0; i--)
                    {
                        LevelStore<T> store = scope[i];
                        if (store.TryGetValue(key, out result))
                            break;
                    }
                    return result;
                }
                set
                {
                    if (scope.Count > 0)
                    {
                        LevelStore<T> store = scope[scope.Count - 1];
                        store.SetValue(key, value);
                    }
                }
            }

            internal class LevelStore<T2>
            {
                private readonly Dictionary<string, T2> store;

                internal LevelStore()
                {
                    store = new Dictionary<string, T2>();
                }

                internal bool TryGetValue(string key, out T2 value)
                {
                    return store.TryGetValue(key, out value);
                }

                internal void SetValue(string key, T2 value)
                {
                    if (store.ContainsKey(key))
                        store.Remove(key);
                    store.Add(key, value);
                }
                internal void Clear()
                {
                    store.Clear();
                }
            }
        }

        private TSqlFragmentWalker walker;
        private MultilevelStore<TableSource> tableSources;
        private MultilevelStore<TableSourceMetaData> metaData;
       
        public SemiJoinAnalyzer(string database) : base(database) 
        {
            _id = id;
            walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            walker.PushEndFilter = ReturnEnd;

            tableSources = new MultilevelStore<TableSource>();
            metaData = new MultilevelStore<TableSourceMetaData>();
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            tableSources.Clear();
            metaData.Clear();

            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is SelectStatement)
                    BeginSelect(script, (SelectStatement)frag);
                else
                if (frag is TSqlEndFragment)
                    Analyze((TSqlEndFragment)frag);
            }

            tableSources.Clear();
            metaData.Clear();
        }

        private void Analyze(TSqlEndFragment endFragment)
        {
            TSqlFragment wrapped = endFragment.StartFragment;
            if (wrapped is SelectStatement)
            {
                AnalyzeSelect((SelectStatement)wrapped);
                tableSources.Pop();
            }

            if (wrapped is Subquery)
            {
                AnalyzeSubquery((Subquery)wrapped);
                tableSources.Pop();
            }
        }

        private void AnalyzeSelect(SelectStatement select)
        {

        }

        private void AnalyzeSubquery(Subquery query)
        {

        }

        private void BeginSelect(string script, SelectStatement select)
        {
            QuerySpecification spec = select.QueryExpression as QuerySpecification;
            if (spec == null)
                return;

            tableSources.Push();

            // get a dictionary of tables used in the query
            Dictionary<string, TableSource> tables = TSqlParsingHelpers.GetTableSources(select);

            // add each table source to the store
            foreach (string key in tables.Keys)
            {
                tableSources[key] = tables[key];
            }
        }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is FunctionCall)
            {
                FunctionCall func = (FunctionCall)fragment;
                switch (func.FunctionName.Value.ToLower())
                {
                    case "count":
                        if (func.Parameters.Count == 1 && func.Parameters[0] is Column)
                            return ((Column)func.Parameters[0]).ColumnType != ColumnType.Wildcard;
                        return true;
                }
            }
            else if (fragment is SelectColumn)
                return false;
            else if (fragment is ExistsPredicate)
                return false;

            return true;
        }

        private bool ReturnEnd(TSqlFragment fragment)
        {
            if (fragment is Subquery)
                return true;
            else if (fragment is SelectStatement)
                return true;

            return false;
        }
    }
}
