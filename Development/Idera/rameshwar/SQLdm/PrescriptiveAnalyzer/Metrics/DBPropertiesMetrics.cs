﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class DBPropertiesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("DBPropertiesMetrics");
        public List<DBPropertiesSnapshot> DBPropertiesSnapshots { get; set; }
        public string Edition { get; set; }

        public DBPropertiesMetrics()
        {
            DBPropertiesSnapshots = new List<DBPropertiesSnapshot>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("DBPropertiesMetrics not added : " + snapshot.Error); return; }

            if (snapshot.DatabaseConfigurationSnapshotValue == null) { return; }

            Edition = snapshot.DatabaseConfigurationSnapshotValue.ProductEdition;

            if (snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings != null && snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows.Count; index++)
                {
                    DBPropertiesSnapshot obj = new DBPropertiesSnapshot();
                    try
                    {
                        obj.dbname = (string)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["DatabaseName"];
                        if (snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Collation"] != DBNull.Value)
                        { obj.collation_name = (string)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Collation"]; }
                        else
                        { obj.collation_name = string.Empty; }
                        obj.state_desc = (string)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Status"];
                        //multipling compatibility level value with 10 as they are devided by 10 in databaseConfigurationProbe but SQlDoctor use the value as it is we are also getting the actaul value for use
                        //handling invalid cast exception
                        if (snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Compatibility"] != DBNull.Value)
                        {
                            obj.compatibility_level = Convert.ToInt32(snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Compatibility"]) * 10;
                        }
                        if (snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Containment"] != DBNull.Value)
                        {
                            obj.containment = Convert.ToInt32(snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Containment"]);
                        }
                        obj.is_auto_create_stats_on = (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAutoCreateStatistics"];
                        obj.auto_update_stats = (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAutoUpdateStatistics"];
                        obj.page_verify_option_desc = (string)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["PageVerifyOption"];
                        //handling invalid cast exception
                        obj.is_read_only = Convert.ToBoolean(snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsReadOnly"]);
                        obj.recovery_model = (string)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["Recovery"];
                        obj.properties = new Dictionary<string, bool>();
                        obj.properties.Add("AUTO_CLOSE", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAutoClose"]);
                        obj.properties.Add("CURSOR_CLOSE_ON_COMMIT", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsCloseCursorsOnCommitEnabled"]);
                        obj.properties.Add("CURSOR_DEFAULT", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsLocalCursorsDefault"]);
                        obj.properties.Add("DATE_CORRELATION_OPTIMIZATION", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsDateCorrelationOn"]);
                        obj.properties.Add("PARAMETERIZATION", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsParameterizationForced"]);
                        obj.properties.Add("ANSI_NULL_DEFAULT", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAnsiNullDefault"]);
                        obj.properties.Add("ANSI_NULLS", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAnsiNullsEnabled"]);
                        obj.properties.Add("ANSI_PADDING", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAnsiPaddingEnabled"]);
                        obj.properties.Add("ANSI_WARNINGS", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsAnsiWarningsEnabled"]);
                        obj.properties.Add("ARITHABORT", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsArithmeticAbortEnabled"]);
                        obj.properties.Add("QUOTED_IDENTIFIER", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsQuotedIdentifiersEnabled"]);
                        obj.properties.Add("NUMERIC_ROUNDABORT", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsNumericRoundAbortEnabled"]);
                        obj.properties.Add("RECURSIVE_TRIGGERS", (bool)snapshot.DatabaseConfigurationSnapshotValue.ConfigurationSettings.Rows[index]["IsRecursiveTriggersEnabled"]);
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    DBPropertiesSnapshots.Add(obj);
                }
            }
        }
    }

    public class DBPropertiesSnapshot
    {
        public string dbname { get; set; }
        public string collation_name { get; set; }
        //public int state { get; set; }
        public string state_desc { get; set; }
        public int compatibility_level { get; set; }
        public int containment { get; set; }
        public Boolean is_auto_create_stats_on { get; set; }
        public Boolean auto_update_stats { get; set; }
        public string page_verify_option_desc { get; set; }
        public Boolean is_read_only { get; set; }
        public string recovery_model { get; set; }
        public Dictionary<string, bool> properties { get; set; }
    }


}
