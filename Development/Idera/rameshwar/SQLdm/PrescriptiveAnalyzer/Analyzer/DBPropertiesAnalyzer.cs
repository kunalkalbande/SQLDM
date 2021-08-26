using System;
using System.Collections.Generic;
using System.Data.SqlClient;
//using System.Linq;
//using Idera.SQLdm.PrescriptiveAnalyzer.AnalysisEngine.Batches;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using System.Collections;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class DBPropertiesAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 2;
        private static Logger _logX = Logger.GetLogger("DBPropertiesAnalyzer");        
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("DBProperties analysis"); }
        private List<string> databases;
        private bool isFiltered = false;
        private ServerVersion ver = null;
        internal DBObjectAnalyzerOptions Options { get; set; }

        public DBPropertiesAnalyzer()
        {
            _id = id;
//Changes done to support SDR-R4
            if (databases == null)
            {
                databases = new List<string>();
            }
        }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            string dbName;
//Changes done to support SDR-R4
            //Setting version details Srishti PUrohit
            using (conn)
            {
                conn.Open();
                if (conn.ServerVersion != null)
                    ver = new ServerVersion(conn.ServerVersion);
            }

            string modelDatabaseCollation = "";

            // Set up default properties based on edition

            List<DatabasePropertySettings> defaultProperties = new List<DatabasePropertySettings>();
            IEnumerator e = GetDefaultProperties(sm.DBPropertiesMetrics.Edition).GetEnumerator();
            while (e.MoveNext())
            {
                defaultProperties.Add((DatabasePropertySettings)e.Current);
            }
            foreach (DBPropertiesSnapshot snap in sm.DBPropertiesMetrics.DBPropertiesSnapshots)
            {

                bool isAutoCreateStatistics = true;
                bool isAutoUpdateStatistics = true;
                int temp = 0;
                bool tempBool = false;
                string tempString = "";

                if (string.IsNullOrEmpty(snap.dbname)) continue;
                dbName = snap.dbname;

                if (dbName.ToLower().Trim() == "model")
                {   // save this tidbit just in case model gets filtered out
                    modelDatabaseCollation = snap.collation_name;
                }

                if (dbName.Length == 0)// || (isFiltered && !databases.Contains(dbName)))
                    continue;

                if (Options != null && Options.IsDatabaseBlocked(dbName))
                {
                    _logX.InfoFormat("Database {0} is being blocked.  Skipping to the next database.", dbName);
                    continue;
                }

                // Skip databases which are not online
                 string status = snap.state_desc;
                 if (status.Trim().ToLower() != "online")
                {
                    _logX.Info(string.Format("Database '{0}' is being skipped due to state {1} ({2})",
                                             dbName,
                                             temp,
                                             snap.state_desc));
                    continue;
                }

                // Database Compatibility Check
                temp = snap.compatibility_level;
//Changes done to support SDR-R4
                //[START] SQLdm 10.0 (Gaurav Karwal): clubbing the code into one line
                string targetCompat = ver.Major.ToString() + "0"; //if major version is 13, compat is 130

                if (ver != null && ver is ServerVersion && temp != int.Parse(targetCompat)) 
                {
                    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), targetCompat));
                }
                //[END] SQLdm 10.0 (Gaurav Karwal): clubbing the code into one line

             
                //if (ver != null && ver.Major == 9 && temp != 90)
                //{
                //    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "90"));
                //}

                //if (ver != null && ver.Major == 10 && temp != 100)
                //{
                //    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "100"));
                //}

                //if (ver != null && ver.Major == 11 && temp != 110)
                //{
                //    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "110"));
                //}

                //if (ver != null && ver.Major == 12 && temp != 120)
                //{
                //    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "120"));
                //}

                //if (ver != null && ver.Major == 13 && temp != 130)
                //{
                //    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "130"));
                //}

                if (ver != null && ver.Major == 11 && temp != 110)
                {
                    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "110"));
                }

                if (ver != null && ver.Major == 12 && temp != 120)
                {
                    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "120"));
                }

                if (ver != null && ver.Major == 13 && temp != 130)
                {
                    AddRecommendation(new DatabaseCompatibilityRecommendation(dbName, temp.ToString(), "130"));
                }

                if (temp < 90)
                {
                    _logX.Info(string.Format("Database '{0}' is being skipped due to compatibility level {1}",
                                             dbName,
                                             temp));
                    continue;
                }

                databases.Add(dbName);

                // Collation check
                tempString = snap.collation_name;
                if (dbName.ToLower().Trim() == "model")
                {
                    modelDatabaseCollation = tempString;
                }
                else
                {
                    if (!String.IsNullOrEmpty(modelDatabaseCollation) && !String.IsNullOrEmpty(tempString) &&
                        modelDatabaseCollation.ToLower().Trim() != tempString.ToLower().Trim())
                    {
                        //---------------------------------------------------------------------
                        // Add the recommendation about mismatched collations.
                        // 
                        // If SQL Server 2012, make sure that the databases is not contained before
                        // adding the recommendation.  Contained databases will have special logic to
                        // try and support the mismatched collation issues.
                        //
                        if ((ver != null && ver.Major < 11) || (0 == snap.containment)) AddRecommendation(new DatabaseCollationRecommendation(dbName, tempString, modelDatabaseCollation));
                        else _logX.Info(string.Format("Skip SDR-DC5 for SQL Server 2012 Database '{0}' with compatibility level {1} that does not match model {2}", dbName, tempString, modelDatabaseCollation));
                    }
                }

                // Auto Stats Recommendation Check
                isAutoCreateStatistics = snap.is_auto_create_stats_on;
                isAutoUpdateStatistics = snap.auto_update_stats;

                // Page Verify cannot be changed for tempdb in SQL 2005
                if (ver != null && ver.Major > 9 || dbName != "tempdb")
                {
                    tempString = snap.page_verify_option_desc;
                }
                else
                {
                    tempString = null;
                }

                if (!snap.is_read_only)
                {
                    // Auto Stats Recommendation
                    if (!isAutoCreateStatistics || !isAutoUpdateStatistics)
                    {
                        AddRecommendation(new AutoStatsRecommendation(dbName,
                                                                      isAutoCreateStatistics,
                                                                      isAutoUpdateStatistics));
                    }
                    // Page Verify Recommendation 
                    if (!String.IsNullOrEmpty(tempString) && tempString.ToLower().Trim() == "none")
                    {
                        AddRecommendation(new PageVerifyRecommendation(dbName, false, tempString.Trim()));
                    }
                    // simple recovery model Recommendation 
                    string recoveryModel = snap.recovery_model;
                    if (("simple" == recoveryModel.ToLower()) && (dbName != "tempdb"))
                    {
                        AddRecommendation(new DatabaseRecoveryModelRecommendation(dbName, recoveryModel));
                    }
                }
                else
                {
                    // Page Verify Recommendation 
                    if (!String.IsNullOrEmpty(tempString) && tempString.ToLower().Trim() != "none")
                    {
                        //AddRecommendation(new PageVerifyRecommendation(dbName, true, tempString.Trim()));
                    }
                }

                // Database Properties Recommendation Check

                foreach (DatabasePropertySettings defaultProperty in defaultProperties)
                {
                    if (snap.properties.ContainsKey(defaultProperty.ColumnName))
                    {
                        tempBool = snap.properties[defaultProperty.ColumnName];
                        if (tempBool != defaultProperty.DefaultValue)
                        {
                            AddRecommendation(new DatabaseConfigurationRecommendation(dbName,
                                                                                      defaultProperty.DisplayName,
                                                                                      tempBool
                                                                                          ? defaultProperty.TrueText
                                                                                          : defaultProperty.FalseText,
                                                                                      tempBool
                                                                                          ? defaultProperty.FalseText
                                                                                          : defaultProperty.TrueText));
                        }
                    }
                }
            }
        }
        

        internal List<string> Databases
        {
            get { return databases; }
            set
            {
                if (value != null) databases = value;
                else databases = new List<string>();
            }
        }

        private static IEnumerable<DatabasePropertySettings> GetDefaultProperties(string Edition)
        {
            bool IsExpress = !String.IsNullOrEmpty(Edition) && Edition.ToLower().Contains("express");

            yield return (new DatabasePropertySettings("AUTO_CLOSE", "auto_close", IsExpress, "ON", "OFF"));  // True for SQL Express, False for other editions
            yield return (new DatabasePropertySettings("CURSOR_CLOSE_ON_COMMIT", "is_cursor_close_on_commit_on", false,"ON","OFF"));
            yield return (new DatabasePropertySettings("CURSOR_DEFAULT", "is_local_cursor_default", false,"LOCAL","GLOBAL"));
            yield return (new DatabasePropertySettings("DATE_CORRELATION_OPTIMIZATION", "is_date_correlation_on", false, "ON","OFF"));
            yield return (new DatabasePropertySettings("PARAMETERIZATION", "is_parameterization_forced", false, "FORCED", "SIMPLE"));
            yield return (new DatabasePropertySettings("ANSI_NULL_DEFAULT", "is_ansi_null_default_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("ANSI_NULLS", "is_ansi_nulls_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("ANSI_PADDING", "is_ansi_padding_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("ANSI_WARNINGS", "is_ansi_warnings_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("ARITHABORT", "is_arithabort_on", false, "ON", "OFF"));
            //yield return (new DatabasePropertySettings("CONCAT_NULL_YIELDS_NULL", "is_concat_null_yields_null_on", false, "ON", "OFF"));  // removed per PR# DR1206
            yield return (new DatabasePropertySettings("QUOTED_IDENTIFIER", "is_quoted_identifier_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("NUMERIC_ROUNDABORT", "is_numeric_roundabort_on", false, "ON", "OFF"));
            yield return (new DatabasePropertySettings("RECURSIVE_TRIGGERS", "is_recursive_triggers_on", false, "ON", "OFF"));
        }

        public class DatabasePropertySettings
        {
            internal string ColumnName { get; set; }
            internal string DisplayName { get; set; }
            internal bool DefaultValue { get; set; }
            internal string TrueText { get; set; }
            internal string FalseText { get; set; }


            public DatabasePropertySettings(string displayName, string columnName, bool defaultValue)
                : this(displayName, columnName, defaultValue, "TRUE", "FALSE")
            {
            }

            public DatabasePropertySettings(string displayName, string columnName, bool defaultValue, string trueText, string falseTExt)
            {
                ColumnName = columnName;
                DisplayName = displayName;
                DefaultValue = defaultValue;
                TrueText = trueText;
                FalseText = falseTExt;
            }

        }
         
    }
}
