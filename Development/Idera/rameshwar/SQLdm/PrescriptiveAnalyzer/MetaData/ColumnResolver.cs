using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using System.Collections.Specialized;
using SMO=Microsoft.SqlServer.Management.Smo;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MetaData
{
    internal class ColumnResolver
    {
        private Dictionary<string, Dictionary<string, TableSourceMetaData>> cache;
        private readonly SqlServerSchema schema;

        public ColumnResolver(SqlServerSchema schema)
        {
            cache = new Dictionary<string, Dictionary<string, TableSourceMetaData>>();
            this.schema = schema;
        }

        internal void Clear()
        {
            cache.Clear();
        }

        public SqlServerSchema SqlServerSchema
        {
            get;
            set;
        }

        private TableSourceMetaData GetCachedTableSourceMetaData(string database, string objectName)
        {
            TableSourceMetaData cached = null;

            if (cache.ContainsKey(database))
                cache[database].TryGetValue(objectName, out cached);

            return cached;
        }

        public TableSourceMetaData GetTableSourceMetaData(string database, TableSource tableSource)
        {
            TableSourceMetaData metaData = null;
            if (tableSource is QueryDerivedTable)
            {
                QueryDerivedTable qdt = (QueryDerivedTable)tableSource;
                metaData = new TableSourceMetaData(qdt.Alias.Value, TableSourceKind.Query);
                if (qdt.Columns.Count > 0)
                    AddColumns(metaData, qdt.Columns);
                else
                {
                    SubquerySpecification spec = qdt.Subquery.QueryExpression as SubquerySpecification;
                    if (spec == null)
                        return null;

                    int colid = 0;
                    foreach (TSqlFragment fragment in spec.SelectElements)
                    {
                        colid++;
                        if (fragment is SelectColumn)
                        {
                            string name = null;
                            SelectColumn col = (SelectColumn)fragment;

                            Identifier id = col.ColumnName as Identifier;
                            if (id != null)
                                name = id.Value;
                            else
                            {
                                Literal lit = col.ColumnName as Literal;
                                if (lit != null)
                                    name = lit.Value;
                                else
                                {
                                    Expression e = ((SelectColumn)fragment).Expression;
                                    if (e is Column)
                                    {
                                        name = GetLastIdentifier(((Column)e).Identifiers);
                                    }
                                    else
                                    {
                                        name = String.Format("{0}_{1}", metaData.Name, colid);
                                    }
                                }
                            }
                            if (name != null)
                                metaData.AddColumn(name);
                        }
                    }
                }

                return metaData;
            }

            if (tableSource is SchemaObjectTableSource)
            {
                SchemaObjectTableSource sots = (SchemaObjectTableSource)tableSource;
                
                string name = null;
                string schemaName = null;

                if (sots.SchemaObject != null)
                {
                    SchemaObjectName son = sots.SchemaObject;
                    name = son.BaseIdentifier.Value;
                    if (son.SchemaIdentifier != null)
                        schemaName = son.SchemaIdentifier.Value;
                    if (son.DatabaseIdentifier != null)
                    {
                        if (!String.IsNullOrEmpty(son.DatabaseIdentifier.Value))
                            database = son.DatabaseIdentifier.Value;
                    }
                } 
                else if (sots.Alias != null)
                    name = sots.Alias.Value;
                else
                    return null;

                string objectName = GetObjectName(schemaName, name);
                metaData = GetCachedTableSourceMetaData(database, objectName);
                if (metaData == null)
                {
                    if (sots.Columns != null && sots.Columns.Count > 0)
                    {
                        metaData = new TableSourceMetaData(objectName, TableSourceKind.Query);
                        AddColumns(metaData, sots.Columns);
                        CacheTableSourceMetaData(database, objectName, metaData);
                    }
                    else
                    {
                        metaData = schema.GetTableSourceMetaData(database, name, schemaName);
                    }
                }
            }

            return metaData;
        }

        private void CacheTableSourceMetaData(string database, string objectName, TableSourceMetaData metaData)
        {
            Dictionary<string, TableSourceMetaData> dbcache = null;
            if (!cache.TryGetValue(database, out dbcache))
            {
                dbcache = new Dictionary<string, TableSourceMetaData>();
                cache.Add(database, dbcache);
            }
            dbcache.Add(objectName, metaData);
        }

        internal static string GetLastIdentifier(IList<Identifier> identifiers)
        {
            if (identifiers == null || identifiers.Count == 0)
                return String.Empty;
            return identifiers[identifiers.Count - 1].Value;
        }

        private static void AddColumns(TableSourceMetaData metaData, IEnumerable<Identifier> columns)
        {
            foreach (Identifier id in columns)
            {
                metaData.AddColumn(id.Value.ToLower());
            }
        }

        internal  static string GetObjectName(string schema, string name)
        {
            if (!String.IsNullOrEmpty(schema))
                return String.Format("{0}.{1}", schema, name);
            return name;
        }


    }
}
