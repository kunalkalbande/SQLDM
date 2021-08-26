using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Wintellect.PowerCollections;

using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Snapshots;
using QueryMonitorViewMode = Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries.QueryMonitorView.QueryMonitorViewMode;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class QueryMonitorData
    {
        // columns returned from the stored procedure
        public const string COL_DB_StartTime = @"UTCStartTime";
        public const string COL_DB_CompletionTime = @"UTCCompletionTime";
        public const string COL_DB_Duration = @"DurationMilliseconds";
        public const string COL_DB_CPU = @"TotalCPUMilliseconds";
        public const string COL_DB_Reads = @"TotalReads";
        public const string COL_DB_Writes = @"TotalWrites";
        public const string COL_DB_Waits = @"WaitMilliseconds";
        public const string COL_DB_Deadlocks = @"Deadlocks";
        public const string COL_DB_Blocking = @"BlockingTimeMilliseconds";
        public const string COL_DB_Application = @"ApplicationName";
        public const string COL_DB_StatementType = @"StatementType";
        public const string COL_DB_SignatureID = @"SQLSignatureID";
        public const string COL_DB_SqlTextID = @"SQLTextID";
        public const string COL_DB_StatementText = @"SQLStatementText";
        public const string COL_DB_Database = @"DatabaseName";
        public const string COL_DB_User = @"LoginName";
        public const string COL_DB_Host = @"HostName";
        public const string COL_DB_CPUPerSecond = @"CPUPerSecond";
        public const string COL_DB_IOPerSecond = @"IOPerSecond";
        public const string COL_DB_Occurrences = @"Occurrences";
        public const string COL_DB_AggregationFlag = @"AggregationFlag";
        public const string COL_DB_DoNotAggregate = @"DoNotAggregate";
        public const string COL_DB_Spid = @"Spid";

        // columns added to the detail table after it is returned
        public const string COL_NAME = @"QueryName";
        public const string COL_NAME_SORT = @"QueryNameSort";
        public const string COL_CPU_Pct = @"CPUPctCalc";
        public const string COL_Reads_Pct = @"ReadsPctCalc";
        public const string COL_Writes_Pct = @"WritesPctCalc";

        public const string COL_SUM_ColumnName = @"ColumnName";
        public const string COL_SUM_ColumnValue = @"ColumnValue";
        public const string COL_SUM_AVG_Duration = @"AvgDurationMilliseconds";
        public const string COL_SUM_AVG_CPU = @"AvgCPUMilliseconds";
        public const string COL_SUM_AVG_Reads = @"AvgReads";
        public const string COL_SUM_AVG_Writes = @"AvgWrites";
        public const string COL_SUM_AVG_Waits = @"AvgWaitMilliseconds";
        public const string COL_SUM_AVG_Deadlocks = @"TotalDeadlocks";
        public const string COL_SUM_AVG_Blocking = @"AvgBlockMilliseconds";
        public const string COL_SUM_AVG_CPUPerSecond = @"CPUPerSecond";
        public const string COL_SUM_AVG_IOPerSecond = @"IOPerSecond";
        public const string COL_SUM_MAX_CPU = @"MaxCPUMilliseconds";
        public const string COL_SUM_MAX_Reads = @"MaxReads";
        public const string COL_SUM_MAX_Writes = @"MaxWrites";
        public const string COL_SUM_Occurrences = "Occurrences";
        public const string COL_SUM_OccurrencesPerDay = "OccurrencesPerDay";
        public const string COL_SUM_SQLSignatureId = "SQLSignatureID";
        public const string COL_SUM_SQLText = "SQLSignature";
        public const string COL_SUM_DoNotAggregate = "DoNotAggregate";

        public const string COL_RESULTS_Code = "EnumValue";
        public const string COL_RESULTS_Text = "Description";

        private const string COL_HISTORY_ColumnNameValue = "Total";

        public const string FORMAT_SUMMARY_ROWFILTER = "ColumnName = '{0}'";

		//START: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
        public const string COL_WEBUI_QueryLink = @"Details";
        public const string COL_QueryStatisticsId = @"QueryStatisticsID";
        //END: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab

        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options -
        #region New Constants

        public const string RESOURCE_ONLY_FILTER_FIRST = " {0} is NOT NULL OR {0} <> '' ";  
        public const string RESOURCE_ONLY_FILTER = " OR {0} is NOT NULL OR {0} <> '' ";  

        #endregion

        private const string FORMAT_SORT_DESC = "{0} Desc";
        private const string FORMAT_PCT_CALC = "IIF({0}=0, 0, {0}*100/Sum({0}))";
        private const string FORMAT_SIGNATURE_NAME = "Query {0}{1}{2}";
        private const string FORMAT_STATEMENT_NAME = "Query {0}{1}{2}";
        private const string FORMAT_NAME_SEP = @"_";
        private const string FORMAT_HISTORY_NAME = "#{0}";

        private QueryMonitorViewMode viewMode;
        private QueryMonitorFilter filter;
        private DataTable detailDataTable;
        private DataTable summaryDataTable;
        private DataTable resultsDataTable;
        //This contains a list of unique signature ids each Triple contains
        //  First: The named Signature Id
        //  Second: Signature Mode names - the key is a unique hash of the unique values and the value is the named occurrence
        //  Third: Statement Mode names - the key is the statement id and the value is the named id for that execution
        private Dictionary<long, Triple<int, Dictionary<int, int>, Dictionary<int, int>>> queryNames = new Dictionary<long, Triple<int, Dictionary<int, int>, Dictionary<int, int>>>();
        private int nextQueryId = 0;

        private int summaryItemsToReturn = 10;
        private int detailItemsToReturn = 200;
        private long? signatureId = null;
        private string signatureHash = null; 

        private bool resultsNoData = false;
        private bool resultsQueryMonitorOff = false;
        private bool resultsQueryMonitorUpgrading = false;
        private string resultsNoDataText = string.Empty;
        private string resultsQueryMonitorOffText = string.Empty;
        private string resultsQueryMonitorUpgradingText = string.Empty;
        private bool getFullQueryText = false; //SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added new field to get the Full QueryText
        private List<QueryMonitorPlanParameters> queriesIcons = new List<QueryMonitorPlanParameters>();

        public QueryMonitorData(QueryMonitorViewMode viewMode, QueryMonitorFilter filter, DataTable detailTable, DataTable summaryTable, DataTable resultsTable)
        {
            this.filter = filter;
            if (detailTable != null)
            {
                detailDataTable = detailTable;
                InitializeDetailTable();
                CreateQueryNames();
            }

            summaryDataTable = summaryTable;
            resultsDataTable = resultsTable;
            ProcessResultsTable();
        }

   

        public QueryMonitorData(QueryMonitorData oldData, DataTable detailTable, DataTable summaryTable, DataTable resultsTable, List<QueryMonitorPlanParameters> queriesIcons)
        {
            this.queriesIcons = queriesIcons;
            this.viewMode = oldData.viewMode;
            this.filter = oldData.filter;
            this.detailItemsToReturn = oldData.DetailItemsToReturn;
            this.summaryItemsToReturn = oldData.SummaryItemsToReturn;

            // SQLDM-27596 - History Range control_DC: Navigating to Queries ->Signature mode and Statement mode shows 'Refresh error' at the bottom
            this.queryNames = new Dictionary<long, Triple<int, Dictionary<int, int>, Dictionary<int, int>>>(oldData.queryNames);
            if (detailTable != null)
            {
                detailDataTable = detailTable;
                InitializeDetailTable();
                CreateQueryNames();
            }

            summaryDataTable = summaryTable;
            resultsDataTable = resultsTable;
            ProcessResultsTable();

            this.signatureId = oldData.SignatureId;
            this.signatureHash = oldData.SignatureHash;

            // if we came in with a signature hash then get the id for future reference
            if (signatureHash != null && !signatureId.HasValue && viewMode == QueryMonitorViewMode.History)
            {
                DataView historyData = GetHistoryTotalView();

                if (historyData.Count > 0)
                {
                    signatureId = (long)historyData[0][COL_SUM_SQLSignatureId];
                }
            }
        }

        #region public properties

        public int DetailItemsToReturn
        {
            get { return detailItemsToReturn; }
            set { detailItemsToReturn = value; }
        }

        public int SummaryItemsToReturn
        {
            get { return summaryItemsToReturn; }
        }

        public DataTable DetailTable
        {
            get
            {
                return ApplyResourceFilter(detailDataTable); //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Optionsreturn detailDataTable; }
            }
        }

        public DataTable SummaryTable
        {
            get { return summaryDataTable; }
        }

        public DataTable ResultsTable
        {
            get { return resultsDataTable; }
        }

        public QueryMonitorFilter Filter
        {
            get { return filter; }
        }

        public QueryMonitorViewMode ViewMode
        {
            get { return viewMode; }
            set { viewMode = value; }
        }

        public long? SignatureId
        {
            get { return signatureId; }
            set { signatureId = value; }
        }

        public List<QueryMonitorPlanParameters> QueriesIcons
        {
            get
            {
                if (queriesIcons == null)
                    queriesIcons = new List<QueryMonitorPlanParameters>();
                return queriesIcons;
            }
        }

        public long? HistorySignatureId
        {
            get
            {
                // note we don't set the signatureId so it won't use it for all future queries
                long? id = signatureId;
                if (!id.HasValue && viewMode == QueryMonitorViewMode.History)
                {
                    if (summaryDataTable != null && summaryDataTable.Rows.Count > 0 && summaryDataTable.Rows[0][COL_DB_SignatureID] is long)
                    {
                        id = (long)summaryDataTable.Rows[0][COL_DB_SignatureID];
                    }
                }

                return id;
            }
        }

        public string SignatureHash
        {
            get { return signatureHash; }
            set { signatureHash = value; }
        }

        public string SignatureName
        {
            get
            {
                if (SignatureId.HasValue)
                {
                    return GetQueryName(SignatureId.Value);
                }
                return string.Empty;
            }
        }

        public bool ResultsNoData
        {
            get { return resultsNoData; }
        }

        public bool ResultsQueryMonitorOff
        {
            get { return resultsQueryMonitorOff; }
        }

        public bool ResultsQueryMonitorUpgrading
        {
            get { return resultsQueryMonitorUpgrading; }
        }

        public string ResultsNoDataText
        {
            get { return resultsNoDataText; }
        }

        public string ResultsQueryMonitorOffText
        {
            get { return resultsQueryMonitorOffText; }
        }

        public string ResultsQueryMonitorUpgradingText
        {
            get { return resultsQueryMonitorUpgradingText; }
        }

        //Start -SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added new field to get the Full QueryText 
        public bool GetFullQueryText
        {
            get { return getFullQueryText; }
            set { getFullQueryText = value; }
        }
        //End -SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added new field to get the Full QueryText 

        #endregion

        #region Public Functions

        public static string GetSummaryDataColumn(ChartType chartType, ChartView chartView)
        {
            bool useSummary = (chartView != ChartView.QuerySQL);

            return GetSummaryDataColumn(useSummary, chartType, chartView);
        }

        public DataView GetHistoryTotalView()
        {
            return new DataView(summaryDataTable, string.Format(FORMAT_SUMMARY_ROWFILTER, COL_HISTORY_ColumnNameValue), string.Empty, DataViewRowState.CurrentRows);
        }

        public DataView GetSummaryView(ChartType chartType, ChartView chartView)
        {
            bool useSummary = (viewMode != QueryMonitorViewMode.History && chartView != ChartView.QuerySQL);
            DataTable dataTable = useSummary ? summaryDataTable : detailDataTable;

            return new DataView(dataTable, string.Empty, string.Format(FORMAT_SORT_DESC, GetSummaryDataColumn(useSummary, chartType, chartView)), DataViewRowState.CurrentRows);
        }

        public string GetQueryName(long id)
        {
            return getSignatureName(id);
        }

        #endregion

        #region helpers


        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options --Added New method
        private DataTable ApplyResourceFilter(DataTable detailDataTable)
        {
            if (detailDataTable != null)
            {
                var resourceRowFilter = String.Empty;
                try
                {

                    resourceRowFilter = GenerateResourceFilter(detailDataTable.Columns.Contains(QueryMonitorData.COL_SUM_AVG_Duration));
                }
                catch (Exception e)
                {
                    resourceRowFilter = String.Empty; //when Resource Row Filters Not working 
                }
                if (detailDataTable.Rows.Count > 0 && !String.IsNullOrWhiteSpace(resourceRowFilter))
                {
                    var foundRows = detailDataTable.Select(resourceRowFilter);
                    // SQLDM-19787 10.1.3 Performance - Query Monitor View Move local declarations closer to usage
                    var resultDataTable = detailDataTable.Clone();
                    if (foundRows.Length > 0)
                    {

                        foreach (var item in foundRows)
                        {
                            resultDataTable.ImportRow(item);
                        }
                    }
                    return resultDataTable;
                }

            }
            return detailDataTable;
            
        }

        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options --Added New Method
        private string GenerateResourceFilter(bool isSignature)
        {
            var resourceRowFilter = String.Empty;
            if (this.filter.IncludeOnlyResourceRows)
            {
                if (isSignature)
                {
                    resourceRowFilter += string.Format(QueryMonitorData.RESOURCE_ONLY_FILTER_FIRST, QueryMonitorData.COL_SUM_AVG_Duration);
                }
                else
                {
                    resourceRowFilter += string.Format(QueryMonitorData.RESOURCE_ONLY_FILTER_FIRST, QueryMonitorData.COL_DB_CompletionTime);
                }

            }
            return resourceRowFilter;
        }

        private void InitializeDetailTable()
        {
            detailDataTable.Columns.Add(COL_NAME, typeof(string));
            detailDataTable.Columns.Add(COL_NAME_SORT, typeof(long));

            detailDataTable.Columns.Add(COL_CPU_Pct, typeof(decimal));
            if (detailDataTable.Columns.Contains(COL_SUM_AVG_CPU))
            {
                detailDataTable.Columns[COL_CPU_Pct].Expression = string.Format(FORMAT_PCT_CALC, COL_SUM_AVG_CPU);
            }

            detailDataTable.Columns.Add(COL_Reads_Pct, typeof(decimal));
            if (detailDataTable.Columns.Contains(COL_SUM_AVG_Reads))
            {
                detailDataTable.Columns[COL_Reads_Pct].Expression = string.Format(FORMAT_PCT_CALC, COL_SUM_AVG_Reads);
            }

            detailDataTable.Columns.Add(COL_Writes_Pct, typeof(decimal));
            if (detailDataTable.Columns.Contains(COL_SUM_AVG_Writes))
            {
                detailDataTable.Columns[COL_Writes_Pct].Expression = string.Format(FORMAT_PCT_CALC, COL_SUM_AVG_Writes);
            }

            //SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            if (!detailDataTable.Columns.Contains(COL_QueryStatisticsId))
            {
                detailDataTable.Columns.Add(COL_QueryStatisticsId, typeof(int));
            }
        }

        private void ProcessResultsTable()
        {
            resultsNoData = false;
            resultsQueryMonitorOff = false;
            resultsQueryMonitorUpgrading = false;
            resultsNoDataText = string.Empty;
            resultsQueryMonitorOffText = string.Empty;
            resultsQueryMonitorUpgradingText = string.Empty;

            if (resultsDataTable != null && resultsDataTable.Rows.Count > 0)
            {
                foreach (DataRow row in resultsDataTable.Rows)
                {
                    if ((int)row[COL_RESULTS_Code] == (int)IncompleteQueryMonitorDataReason.NoData)
                    {
                        resultsNoData = true;
                        resultsNoDataText = (string)row[COL_RESULTS_Text];
                    }
                    if ((int)row[COL_RESULTS_Code] == (int)IncompleteQueryMonitorDataReason.QueryMonitorOff)
                    {
                        resultsQueryMonitorOff = true;
                        resultsQueryMonitorOffText = (string)row[COL_RESULTS_Text];
                    }
                    if ((int)row[COL_RESULTS_Code] == (int)IncompleteQueryMonitorDataReason.QueryMonitorUpgrading)
                    {
                        resultsQueryMonitorUpgrading = true;
                        resultsQueryMonitorUpgradingText = (string)row[COL_RESULTS_Text];
                    }
                }
            }
        }

        private static string GetSummaryDataColumn(bool useSummary, ChartType chartType, ChartView chartView)
        {
            string col = COL_SUM_ColumnValue;
            switch (chartType)
            {
                case ChartType.QueryDuration:
                    col = useSummary ? COL_SUM_AVG_Duration : COL_DB_Duration;
                    break;
                case ChartType.QueryCPU:
                    col = useSummary ? COL_SUM_AVG_CPU : COL_DB_CPU;
                    break;
                case ChartType.QueryReads:
                    col = useSummary ? COL_SUM_AVG_Reads : COL_DB_Reads;
                    break;
                case ChartType.QueryWrites:
                    col = useSummary ? COL_SUM_AVG_Writes : COL_DB_Writes;
                    break;
                case ChartType.QueryWaits:
                    col = useSummary ? COL_SUM_AVG_Waits : COL_DB_Waits;
                    break;
                case ChartType.QueryDeadlocks:
                    col = useSummary ? COL_SUM_AVG_Deadlocks : COL_DB_Deadlocks;
                    break;
                case ChartType.QueryBlocking:
                    col = useSummary ? COL_SUM_AVG_Blocking : COL_DB_Blocking;
                    break;
                case ChartType.QueryCPUPerSecond:
                    col = useSummary ? COL_SUM_AVG_CPUPerSecond : COL_DB_CPUPerSecond;
                    break;
                case ChartType.QueryIOPerSecond:
                    col = useSummary ? COL_SUM_AVG_IOPerSecond : COL_DB_IOPerSecond;
                    break;
                case ChartType.QueryHistoryDuration:
                    col = COL_DB_Duration;
                    break;
                case ChartType.QueryHistoryCPU:
                    col = COL_DB_CPU;
                    break;
                case ChartType.QueryHistoryReads:
                    col = COL_DB_Reads;
                    break;
                case ChartType.QueryHistoryWrites:
                    col = COL_DB_Writes;
                    break;
                case ChartType.QueryHistoryWaits:
                    col = COL_DB_Waits;
                    break;
                case ChartType.QueryHistoryDeadlocks:
                    col = COL_DB_Deadlocks;
                    break;
                case ChartType.QueryHistoryBlocking:
                    col = COL_DB_Blocking;
                    break;
                case ChartType.QueryHistoryCPUPerSecond:
                    col = COL_DB_CPUPerSecond;
                    break;
                case ChartType.QueryHistoryIOPerSecond:
                    col = COL_DB_IOPerSecond;
                    break;
                default:
                    break;
            }

            return col;
        }

        private void CreateQueryNames()
        {
            string name;
            detailDataTable.DefaultView.Sort = QueryMonitorData.COL_DB_CompletionTime;
            if (detailDataTable.Columns.Contains(COL_DB_SignatureID) &&
                detailDataTable.Columns.Contains(COL_NAME))
            {
                if (viewMode == QueryMonitorViewMode.History)
                {
                    // for history mode, just generate new ones each time for the query
                    int nextId = 1;
                    foreach (DataRowView row in detailDataTable.DefaultView)
                    {
                        long id = (long)row[COL_DB_SignatureID];
                        name = string.Format(FORMAT_HISTORY_NAME, nextId);
                        row[COL_NAME] = name;
                        row[COL_NAME_SORT] = nextId;
                        nextId++;
                    }
                }
                else
                {
                    nextQueryId = queryNames.Count + 1;
                    int signatureNameId;
                    int uniqueId = 0;
                    Dictionary<int, int> signatureNames;
                    Dictionary<int, int> statementNames;
                    foreach (DataRowView row in detailDataTable.DefaultView)
                    {
                        long id = (long)row[COL_DB_SignatureID];
                        int signatureHash = getRowHash(row.Row, true);
                        int statementHash = getRowHash(row.Row, false);

                        Triple<int, Dictionary<int, int>, Dictionary<int, int>> nameLists;
                        if (queryNames.TryGetValue(id, out nameLists))
                        {
                            signatureNameId = nameLists.First;
                            signatureNames = nameLists.Second;
                            statementNames = nameLists.Third;

                            // SQLDM-19787 10.1.3 Performance - Query Monitor
                            name = viewMode == QueryMonitorViewMode.Statement
                                ? getUniqueStatementName(signatureNameId, statementHash, statementNames, out uniqueId)
                                : getUniqueSignatureName(signatureNameId, signatureHash, signatureNames, out uniqueId);
                        }
                        else
                        {
                            signatureNameId = nextQueryId++;
                            signatureNames = new Dictionary<int, int>();
                            statementNames = new Dictionary<int, int>();

                            // SQLDM-19787 10.1.3 Performance - Query Monitor
                            name = viewMode == QueryMonitorViewMode.Statement
                                ? getUniqueStatementName(signatureNameId, statementHash, statementNames, out uniqueId)
                                : getUniqueSignatureName(signatureNameId, signatureHash, signatureNames, out uniqueId);

                            queryNames.Add(id, new Triple<int,Dictionary<int,int>,Dictionary<int,int>>(signatureNameId, signatureNames, statementNames));
                        }

                        row[COL_NAME] = name;
                        row[COL_NAME_SORT] = (signatureNameId << 40) + uniqueId;
                    }
                }
            }
        }

        /// <summary>
        /// Create a unique hash code for the data associated with this query
        /// </summary>
        /// <param name="row">the datarow containing all database values for the query</param>
        /// <param name="signatureOnly">true to use only the columns that make a signature unique, otherwise use all columns that make a statement unique</param>
        /// <returns></returns>
        private int getRowHash(DataRow row, bool signatureOnly)
        {
            // note including an invalid db name character to frame it and prevent runon matches
            StringBuilder hashText = new StringBuilder(row[COL_DB_StatementType].ToString());
            hashText.Append(':');
            hashText.Append(row[COL_DB_Database]);
            hashText.Append(':');
            hashText.Append(row[COL_DB_Application]);
            if (!signatureOnly)
            {
                hashText.Append(':');
                hashText.Append(row[COL_DB_StartTime].ToString());
                hashText.Append(':');
                hashText.Append(row[COL_DB_User]);
                hashText.Append(':');
                hashText.Append(row[COL_DB_Host]);
                hashText.Append(':');
                hashText.Append(row[COL_DB_Spid]);
            }

            return hashText.ToString().GetHashCode();
        }

        /// <summary>
        /// Find the general signature name for the passed id in the list of queries.
        /// </summary>
        /// <param name="id">the generated signature id name</param>
        /// <param name="hash">the signature only hash of the unique componenents of the data</param>
        /// <param name="signatures">the list of signatures to search. The value will be added to the list if not found.</param>
        /// <param name="signatureId">returns the unique id for this signature occurrence</param>
        /// <returns>The unique name of this occurrence of the signature</returns>
        private string getSignatureName(long id)
        {
            string name = string.Empty;
            Triple<int, Dictionary<int, int>, Dictionary<int, int>> nameLists;
            if (queryNames.TryGetValue(id, out nameLists))
            {
                name = string.Format(FORMAT_SIGNATURE_NAME,
                                        nameLists.First,
                                        string.Empty,
                                        string.Empty);
            }

            return name;
        }

        /// <summary>
        /// Find the general signature name for the passed id.  If not found, create a new one and add it to the list.
        /// </summary>
        /// <param name="id">the generated signature id name</param>
        /// <param name="hash">the signature only hash of the unique componenents of the data</param>
        /// <param name="signatures">the list of signatures to search. The value will be added to the list if not found.</param>
        /// <param name="signatureId">returns the unique id for this signature occurrence</param>
        /// <returns>The unique name of this occurrence of the signature</returns>
        private string getUniqueSignatureName(int id, int hash, Dictionary<int, int> signatures, out int signatureId)
        {
            if (!signatures.TryGetValue(hash, out signatureId))
            {
                signatureId = signatures.Count + 1;
                signatures.Add(hash, signatureId);
            }

            return string.Format(FORMAT_SIGNATURE_NAME, 
                                        id,
                                        signatureId > 1 ? FORMAT_NAME_SEP : string.Empty,
                                        signatureId > 1 ? getAlphaCodeFromId(signatureId) : string.Empty);
        }

        /// <summary>
        /// Find the unique statement name for the passed id and hash in the list of statement signatures. If not found, create a new one and it to the list.
        /// </summary>
        /// <param name="id">the generated signature id name</param>
        /// <param name="hash">the statement hash of the unique componenents of the data</param>
        /// <param name="statements">the list of statements to search. The value will be added to the list if not found.</param>
        /// <param name="statementId">returns the unique id for this statement occurrence</param>
        /// <returns>The unique display name of this occurrence of the statement</returns>
        private string getUniqueStatementName(int id, int hash, Dictionary<int, int> statements, out int statementId)
        {
            if (!statements.TryGetValue(hash, out statementId))
            {
                statementId = statements.Count + 1;
                statements.Add(hash, statementId);
            }

            return string.Format(FORMAT_STATEMENT_NAME,
                                        id,
                                        statementId > 1 ? FORMAT_NAME_SEP : string.Empty,
                                        statementId > 1 ? statementId.ToString() : string.Empty);
        }

        /// <summary>
        /// Convert the passed id value into a unique displayable alpha identifier
        /// </summary>
        /// <param name="signatureId">The generated id for this unique occurrence of a signature or statement</param>
        /// <returns></returns>
        private string getAlphaCodeFromId(int signatureId)
        {
            StringBuilder code = new StringBuilder();
            int beginCode = 'a';
            int endCode = 'z';
            int range = endCode - beginCode + 1;

            int id = signatureId;
            if (id == 1)
            {
                code.Append(beginCode);
            }
            else
            {
                code.Append(Convert.ToChar(beginCode + (id % range) - 1));
                while (id > range)
                {
                    id = (id - (id % range)) / range;
                    code.Insert(0, Convert.ToChar(beginCode + (id % range) - 1));
                }
            }

            return code.ToString();
        }

        #endregion
    }
}
