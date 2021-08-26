using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Specialized;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MetaData
{
    internal class SqlServerSchema
    {
        private Server server;
        private Dictionary<string, MultiDictionary<string, TableSourceMetaData>> cache;

        internal SqlServerSchema(Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration.SqlConnectionInfo connectionInfo)
        {
            SqlConnection cnn = connectionInfo.GetConnection();
            ServerConnection connection = new ServerConnection(cnn);
            server = new Server(connection);
            server.UserOptions.QuotedIdentifier = false;

            cache = new Dictionary<string, MultiDictionary<string, TableSourceMetaData>>();
        }

        internal TableSourceMetaData GetTableSourceMetaData(string database, string table, string schema)
        {
            TableSourceMetaData result = GetCachedTableSource(database, table, schema);
            if (result == null)
            {
                TableViewBase tvb = GetTableOrView(database, table, schema);
                if (tvb != null)
                {
                    
                    result = new TableSourceMetaData(schema, table, TableSourceKind.TableOrView);
                    foreach (Column col in tvb.Columns)
                    {
                        result.AddColumn(col.Name);
                    }
                    AddCachedTableSource(database, result);
                }
            }
            return result;
        }

        private void AddCachedTableSource(string database, TableSourceMetaData metaData)
        {
            TableSourceMetaData tsmd = GetCachedTableSource(database, metaData.Name, metaData.Schema);
            if (tsmd != null)
                RemoveCachedTableSource(database, tsmd);
            MultiDictionary<string, TableSourceMetaData> dic = null;

            if (!cache.TryGetValue(database, out dic))
            {
                dic = new MultiDictionary<string, TableSourceMetaData>(true);
                cache.Add(database, dic);
            }

            dic.Add(metaData.Name, metaData);
        }

        private void RemoveCachedTableSource(string database, TableSourceMetaData metaData)
        {
            MultiDictionary<string, TableSourceMetaData> dic = null;
            if (!cache.TryGetValue(database, out dic))
                return;
            dic.Remove(metaData.Name, metaData);
        }

        private TableSourceMetaData GetCachedTableSource(string database, string name, string schema)
        {
            TableSourceMetaData[] cached = new TableSourceMetaData[1];

            if (cache.ContainsKey(database))
            {
                MultiDictionary<string, TableSourceMetaData> dic = cache[database];
                ICollection<TableSourceMetaData> values = dic[name.ToLower()];
                if (values != null)
                {
                    switch (values.Count)
                    {
                        case 0:
                            break;
                        case 1:
                            values.CopyTo(cached, 0);
                            break;
                        default:
                            cached[0] = FindTableSource(schema.ToLower(), values);
                            break;

                    }
                }
            }
            return cached[0];
        }

        private TableSourceMetaData FindTableSource(string schema, ICollection<TableSourceMetaData> tableSources)
        {
            string schemaName = String.IsNullOrEmpty(schema) ? "dbo" : schema;

            foreach (TableSourceMetaData tsmd in tableSources)
            {
                if (tsmd.Schema.Equals(schemaName))
                    return tsmd;
            }

            return null;
        }

        private Database GetDatabase(string name)
        {
            try
            {
                if (server.Databases.Contains(name))
                    return server.Databases[name];
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Table GetTable(string databaseName, string tableName, string schema)
        {
            try
            {
                Database db = GetDatabase(databaseName);
                if (db != null)
                {
                   return db.Tables[tableName, schema];
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private View GetView(string databaseName, string viewName, string schema)
        {
            try
            {
                Database db = GetDatabase(databaseName);
                if (db != null)
                {
                    if (db.Views.Contains(viewName))
                        return db.Views[viewName, schema];
                } 
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private TableViewBase GetTableOrView(string databaseName, string tableOrViewName, string schema)
        {
            TableViewBase tvb = null;
            try
            {
                tvb = GetTable(databaseName, tableOrViewName, schema);
                if (tvb == null)
                    tvb = GetView(databaseName, tableOrViewName, schema);
            }
            catch (Exception e)
            {
                throw e;
            }

            return tvb;
        } 

        private ColumnCollection GetColumns(string databaseName, string tableOrViewName, string schema)
        {
            try
            {
                TableViewBase tvb = GetTableOrView(databaseName, tableOrViewName, schema);
                if (tvb != null)
                    return tvb.Columns;
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }
    }

    internal class DatabaseMetadata
    {
        public readonly string Name;
        private readonly Dictionary<string, TableSourceMetaData> tableSources;
        private readonly object sync;

        internal DatabaseMetadata(string name)
        {
            Name = name;
            tableSources = new Dictionary<string, TableSourceMetaData>();
            sync = new object();
        }

        internal TableSourceMetaData AddTableSource(string name, TableSourceKind kind)
        {
            return AddTableSource(new TableSourceMetaData(name, kind));
        }

        internal TableSourceMetaData AddTableSource(TableSourceMetaData data)
        {
            lock (sync)
            {
                if (tableSources.ContainsKey(data.Name))
                    tableSources.Remove(data.Name);
                tableSources.Add(data.Name, data);
            }
            return data;
        }

        internal TableSourceMetaData GetTableSource(string name)
        {
            TableSourceMetaData source = null;
            lock (sync)
            {
                tableSources.TryGetValue(name.ToLower(), out source);
            }

            return source;
        }
    }

    internal enum TableSourceKind
    {
        TableOrView,
        Query,
        Function,
        ExtendedProc,
        Unknown
    }

    internal class TableSourceMetaData
    {
        public readonly string Name;
        public readonly string Schema;
        public readonly TableSourceKind Kind;
        private readonly HybridDictionary columns;

        internal TableSourceMetaData(string name, TableSourceKind kind)
        {
            Name = name.ToLower();
            Kind = kind;

            columns = new HybridDictionary(true);
        }

        internal TableSourceMetaData(string schema, string name, TableSourceKind kind)
        {
            Schema = schema.ToLower();
            Name = name.ToLower();
            Kind = kind;

            columns = new HybridDictionary(true);
        }

        internal ColumnMetaData AddColumn(string name)
        {
            ColumnMetaData cmd = new ColumnMetaData(name.ToLower());
            if (columns.Contains(cmd.Name))
                columns.Remove(cmd.Name);
            columns.Add(cmd.Name, cmd);
            return cmd;
        }

    }

    internal class ColumnMetaData
    {
        public readonly string Name;

        public ColumnMetaData(string name)
        {
            Name = name.ToLower();
        }
    }
}
