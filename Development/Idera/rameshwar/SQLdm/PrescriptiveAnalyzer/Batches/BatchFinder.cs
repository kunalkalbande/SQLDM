using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.IO;
using System.Threading;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Batches
{
    static class BatchFinder
    {
        public static readonly string WMI_Namespace_CIMV2 = @"\\.\root\CIMV2";
        public static readonly string WMI_Namespace_MicrosoftVolumeEncryption = @"\\.\root\CIMV2\Security\MicrosoftVolumeEncryption";
        private static int _uniqueIntance = 0;

        internal class ReplaceNameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        internal enum WmiObjectType
        {
            Int,
            BigInt,
            String
        }
        internal class WmiObjectNameType
        {
            public string Name { get; set; }
            public WmiObjectType Type { get; set; }
        }

        internal static string GetBatch(string name, ServerVersion ver)
        {
            return (GetBatch(name, ver, false));
        }

        internal static string GetBatch(string name, ServerVersion ver, bool suppressCopyright)
        {
            string temp;
            //Start: SQLDm 10.0 - Srishti Purohit - New Recommendations
            if (ver.Major >= 13)
            {
                temp = name + "2016";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            if (ver.Major >= 12)
            {
                temp = name + "2014";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            //End: SQLDm 10.0 - Srishti Purohit - New Recommendations

            if (ver.Major >= 11) // Test for a 2011 batch (Denali)
            {
                temp = name + "2012";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            if (ver.Major >= 10) // Test for a 2008 batch
            {
                temp = name + "2008";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            if (ver.Major >= 9) // Test for a 2005 batch
            {
                temp = name + "2005";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            //----------------------------------------------------------------------------
            // We are not currently supporting a 2000 batch.

            if (ver.Major >= 8) // Test for a 2000 batch
            {
                temp = name + "2000";
                if (BatchResourceReader.BatchExists(temp)) return (BatchResourceReader.GetBatch(temp, suppressCopyright));
            }
            return (BatchResourceReader.GetBatch(name, suppressCopyright));
        }

        internal static string GetIndexColumnsForTable(ServerVersion ver, string database, string schema, string table)
        {
            return (ReplaceVariables(GetBatch("GetIndexColumnsForTable", ver), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "Database", Value = SQLHelper.Bracket(database)},
                    new ReplaceNameValue() {Name = "SchemaTable", Value = SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table))}
            }));
        }

        internal static string GetEstimateNonclusteredIndexSize(ServerVersion ver, string database, string schema, string table, bool isUnique, int fillFactor, int[] keyColumns, int[] includeColumns)
        {
            string sql = GetBatch("EstimateNonclusteredIndexSize", ver);
            StringBuilder sbKeyColumns = new StringBuilder();
            StringBuilder sbIncludeColumns = new StringBuilder();
            ReplaceNameValue[] replaceColumn = new ReplaceNameValue[] { new ReplaceNameValue() { Name = "Column", Value = "" } };
            if (null != keyColumns)
            {
                string line = BatchResources.EstimateNonclusteredIndexSizeKeyColumns;
                foreach (int col in keyColumns)
                {
                    replaceColumn[0].Value = col.ToString();
                    sbKeyColumns.AppendLine(ReplaceVariables(line, replaceColumn));
                }
            }
            if (null != includeColumns)
            {
                string line = BatchResources.EstimateNonclusteredIndexSizeIncludeColumns;
                foreach (int col in includeColumns)
                {
                    replaceColumn[0].Value = col.ToString();
                    sbIncludeColumns.AppendLine(ReplaceVariables(line, replaceColumn));
                }
            }
            return (ReplaceVariables(sql, new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "Database", Value = SQLHelper.Bracket(database)},
                    new ReplaceNameValue() {Name = "SchemaTable", Value = SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table))},
                    new ReplaceNameValue() {Name = "NonUniqueIndex", Value = isUnique ? "0" : "1"},
                    new ReplaceNameValue() {Name = "FillFactor", Value = fillFactor.ToString()},
                    new ReplaceNameValue() {Name = "KeyColumns", Value = sbKeyColumns.ToString()},
                    new ReplaceNameValue() {Name = "IncludeColumns", Value = sbIncludeColumns.ToString()},
                    }));
        }

        internal static string GetAllowIndexRecommendation(ServerVersion ver, string database, string schema, string table, ICollection<string> allcolumns)
        {
            StringBuilder columns = new StringBuilder();
            int count = 0;
            foreach (string column in allcolumns)
            {
                columns.Append(SQLHelper.CreateSafeString(column));
                if (count < allcolumns.Count - 1)
                    columns.Append(",");
                count++;
            }

            string sql = GetBatch("AllowIndexRecommendation", ver);
            return (ReplaceVariables(sql, new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "Database", Value = SQLHelper.Bracket(database)},
                    new ReplaceNameValue() {Name = "SchemaTable", Value = SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table))},
                    new ReplaceNameValue() {Name = "SafeSchemaName", Value = SQLHelper.CreateSafeString(schema)},
                    new ReplaceNameValue() {Name = "SafeTableName", Value = SQLHelper.CreateSafeString(table)},
                    new ReplaceNameValue() {Name = "SafeAllColumnsList", Value = columns.ToString()}
                    }));
        }

        internal static string GetEnumWMI(ServerVersion ver, string wmiClassName, string[] wmiProperties)
        {
            return (GetEnumWMI(ver, WMI_Namespace_CIMV2, wmiClassName, wmiProperties));
        }
        internal static string GetEnumWMI(ServerVersion ver, string wmiNamespace, string wmiClassName, string[] wmiProperties)
        {
            string sql = GetBatch("EnumWMI", ver);
            string name = Environment.MachineName + string.Format("_{0}_", Interlocked.Increment(ref _uniqueIntance)) + wmiClassName;
            StringBuilder sbProps = new StringBuilder();
            foreach (char c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            name = string.Format("SQLdoctor_{0}", name);
            if (null != wmiProperties)
            {
                string propLine =  BatchResources.EnumWMIProperties;
                ReplaceNameValue[] prop = new ReplaceNameValue[] { new ReplaceNameValue() { Name = "WMIPropertyName", Value = "" } };
                foreach (string wmiProp in wmiProperties)
                {
                    prop[0].Value = wmiProp;
                    sbProps.AppendLine(ReplaceVariables(propLine, prop));
                }
            }
            return (ReplaceVariables(sql, new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "ScriptFilename", Value = SQLHelper.CreateSafeString(name + ".vbs")},
                    new ReplaceNameValue() {Name = "OutputFilename", Value = SQLHelper.CreateSafeString(name + ".txt")},
                    new ReplaceNameValue() {Name = "WMIClassName", Value = SQLHelper.CreateSafeString(wmiClassName)},
                    new ReplaceNameValue() {Name = "WMINamespace", Value = SQLHelper.CreateSafeString(wmiNamespace)},
                    new ReplaceNameValue() {Name = "OLEContext", Value = Idera.SQLdm.PrescriptiveAnalyzer.Properties.Settings.Default.OLEContext.ToString()},
                    new ReplaceNameValue() {Name = "WMIClassProperties", Value = sbProps.ToString()}
                    }));
        }

        internal static string GetDropIndex(ServerVersion ver, string index, string dbSchemaTable, bool suppressCopyright)
        {
            return (ReplaceVariables(GetBatch("DropIndex", ver, suppressCopyright), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "IndexName", Value = index},
                    new ReplaceNameValue() {Name = "TableName", Value = dbSchemaTable}
            }));
        }

        internal static string GetDisableIndex(ServerVersion ver, string index, string dbSchemaTable)
        {
            return (ReplaceVariables(GetBatch("DisableIndex", ver), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "IndexName", Value = index},
                    new ReplaceNameValue() {Name = "TableName", Value = dbSchemaTable}
            }));
        }

        internal static string GetCreateNonClusteredIndex(ServerVersion ver, string indexName, string dbSchemaTable, string columns)
        {
            return (ReplaceVariables(GetBatch("CreateNonClusteredIndex", ver), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "IndexName", Value = indexName},
                    new ReplaceNameValue() {Name = "TableName", Value = dbSchemaTable},
                    new ReplaceNameValue() {Name = "Columns", Value = columns},
                    new ReplaceNameValue() {Name = "TableNameSafe", Value = SQLHelper.CreateSafeString(dbSchemaTable)},
                    new ReplaceNameValue() {Name = "IndexNameSafe", Value = SQLHelper.CreateSafeString(indexName)}
            }));
        }

        internal static string GetWMIObjProps(ServerVersion ver, string wmiClassName, WmiObjectNameType[] wmiProperties)
        {
            return (GetWMIObjProps(ver, wmiClassName, wmiProperties, false));
        }
        internal static string GetWMIObjProps(ServerVersion ver, string wmiClassName, WmiObjectNameType[] wmiProperties, bool appendMachineName)
        {
            string sql = GetBatch("GetWMIObjProps", ver);
            StringBuilder sbProps = new StringBuilder();
            if (null != wmiProperties)
            {
                string propLineInt =  BatchResources.GetWMIObjPropsInt;
                string propLineBigInt =  BatchResources.GetWMIObjPropsBigInt;
                string propLineString = BatchResources.GetWMIObjPropsString;
                string propLine = string.Empty;
                ReplaceNameValue[] prop = new ReplaceNameValue[] { new ReplaceNameValue() { Name = "WMIPropertyName", Value = "" } };
                foreach (WmiObjectNameType wmiProp in wmiProperties)
                {
                    prop[0].Value = wmiProp.Name;
                    switch (wmiProp.Type)
                    {
                        case WmiObjectType.Int: propLine = propLineInt; break;
                        case WmiObjectType.BigInt: propLine = propLineBigInt; break;
                        case WmiObjectType.String: propLine = propLineString; break;
                        default: propLine = propLineInt; break;
                    }
                    sbProps.AppendLine(ReplaceVariables(propLine, prop));
                }
            }
            return (ReplaceVariables(sql, new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "WMIClassName", Value = SQLHelper.CreateSafeString(wmiClassName)},
                    new ReplaceNameValue() {Name = "OLEContext", Value = Idera.SQLdm.PrescriptiveAnalyzer.Properties.Settings.Default.OLEContext.ToString()},
                    new ReplaceNameValue() {Name = "WMIClassProperties", Value = sbProps.ToString()},
                    new ReplaceNameValue() {Name = "AppendMachineName", Value = appendMachineName ? "1" : "0"}
                    }));
        }

        internal static string GetWMIObjProps(ServerVersion ver, string wmiClassName, string[] wmiProperties)
        {
            return (GetWMIObjProps(ver, wmiClassName, wmiProperties, false));
        }
        internal static string GetWMIObjProps(ServerVersion ver, string wmiClassName, string[] wmiProperties, bool appendMachineName)
        {
            string sql = GetBatch("GetWMIObjProps", ver);
            StringBuilder sbProps = new StringBuilder();
            if (null != wmiProperties)
            {
                string propLine = "";// BatchResources.GetWMIObjPropsInt;
                ReplaceNameValue[] prop = new ReplaceNameValue[] { new ReplaceNameValue() { Name = "WMIPropertyName", Value = "" } };
                foreach (string wmiProp in wmiProperties)
                {
                    prop[0].Value = wmiProp;
                    sbProps.AppendLine(ReplaceVariables(propLine, prop));
                }
            }
            return (ReplaceVariables(sql, new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "WMIClassName", Value = SQLHelper.CreateSafeString(wmiClassName)},
                    new ReplaceNameValue() {Name = "OLEContext", Value = Idera.SQLdm.PrescriptiveAnalyzer.Properties.Settings.Default.OLEContext.ToString()},
                    new ReplaceNameValue() {Name = "WMIClassProperties", Value = sbProps.ToString()},
                    new ReplaceNameValue() {Name = "AppendMachineName", Value = appendMachineName ? "1" : "0"}
                    }));
        }

        private static string ReplaceVariables(string sql, ReplaceNameValue[] replaceNameValue)
        {
            if (sql.Length <= 0) return sql;
            StringBuilder sb = new StringBuilder(sql.Length * 2);
            int bracket = -1;
            for (int n = 0; n < sql.Length; ++n)
            {
                if ('{' == sql[n])
                {
                    if (-1 != bracket) sb.Append(sql.Substring(bracket, n - bracket));
                    bracket = n;
                }
                else if ('}' == sql[n])
                {
                    if (-1 == bracket) sb.Append(sql[n]);
                    else sb.Append(GetReplacement(sql.Substring(bracket, n - bracket + 1), replaceNameValue));
                    bracket = -1;
                }
                else if (-1 == bracket)
                {
                    sb.Append(sql[n]);
                }
            }
            return (sb.ToString());
        }

        private static string GetReplacement(string p, ReplaceNameValue[] replaceNameValue)
        {
            if (null == replaceNameValue) return (p);
            string name = p.TrimStart('{').TrimEnd('}');
            foreach (ReplaceNameValue r in replaceNameValue)
            {
                if (0 == string.Compare(r.Name, name, true)) return (r.Value);
            }
            return (p);
        }


        internal static string GetTableUpdatesPerSec(ServerVersion ver, string database, string schema, string table)
        {
            return (ReplaceVariables(GetBatch("GetTableUpdatesPerSec", ver), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "Database", Value = SQLHelper.CreateSafeString(database)},
                    new ReplaceNameValue() {Name = "SchemaTable", Value = SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table))}
            }));
        }

        internal static string GetTableUpdatesPerMin(ServerVersion ver, string database, string schema, string table)
        {
            return (ReplaceVariables(GetBatch("GetTableUpdatesPerMin", ver), new ReplaceNameValue[] {
                    new ReplaceNameValue() {Name = "Database", Value = SQLHelper.CreateSafeString(database)},
                    new ReplaceNameValue() {Name = "SchemaTable", Value = SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table))}
            }));
        }
    }
}
