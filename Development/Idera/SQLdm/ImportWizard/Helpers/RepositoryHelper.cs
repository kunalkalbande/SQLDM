/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Xml;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.ImportWizard.Objects;
using BBS.TracerX;
using Microsoft.ApplicationBlocks.Data;
using System.IO;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.ImportWizard.Helpers
{
    internal static class RepositoryHelper
    {
        #region types, constants & members

        private const string GetRepositoryInfoStoredProcedure = "p_RepositoryInfo";
        private const string GetMonitoredSqlServersStoredProcedure = "p_GetMonitoredSqlServers";

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(Program));

        #endregion

        #region methods

        //public static RepositoryInfo GetRepositoryInfo(
        //        SqlConnectionInfo connectionInfo
        //    )
        //{
        //    Debug.Assert(connectionInfo != null);

        //    if (connectionInfo == null)
        //    {
        //        throw new ArgumentNullException("connectionInfo");
        //    }

        //    using (SqlConnection connection =
        //                connectionInfo.GetConnection(Constants.ImportWizardConnectionStringApplicationName))
        //    {
        //        try
        //        {
        //            using (
        //                SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
        //                                                                        GetRepositoryInfoStoredProcedure))
        //            {
        //                string versionString = string.Empty;
        //                int monitoredServerCount = 0;

        //                while (dataReader.Read())
        //                {
        //                    switch (dataReader.GetString(0))
        //                    {
        //                        case "Repository Version":
        //                            versionString = dataReader.GetString(2);
        //                            break;
        //                        case "Active Servers":
        //                            monitoredServerCount = dataReader.GetInt32(1);
        //                            break;
        //                    }
        //                }

        //                return new RepositoryInfo(versionString, monitoredServerCount);
        //            }
        //        }
        //        catch (SqlException e)
        //        {
        //            // Assuming that a valid connection can be established to the SQL Server, 
        //            // an invalid call to the version procedure would indicate an invalid database;
        //            // all other exceptions will be passed on.
        //            //
        //            // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
        //            //
        //            Log.Error(e);
        //            if (e.Number == 2812)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}

        //public static List<SqlServer> GetMonitoredSqlServers(
        //        SqlConnectionInfo connectionInfo,
        //        bool activeOnly
        //    )
        //{
        //    Debug.Assert(connectionInfo != null);

        //    if (connectionInfo == null)
        //    {
        //        throw new ArgumentNullException("connectionInfo");
        //    }

        //    using ( SqlConnection connection = connectionInfo.GetConnection())
        //    {
        //        try
        //        {
        //            using (SqlDataReader dataReader =
        //                    SqlHelper.ExecuteReader(connection, GetMonitoredSqlServersStoredProcedure, null, activeOnly))
        //            {
        //                List<SqlServer> instances = new List<SqlServer>();

        //                while (dataReader.Read())
        //                {
        //                    SqlString instance = dataReader.GetSqlString(1);
        //                    instances.Add(new SqlServer((string)instance));
        //                }

        //                return instances;
        //            }
        //        }
        //        catch (SqlException e)
        //        {
        //            // Assuming that a valid connection can be established to the SQL Server, 
        //            // an invalid call to the version procedure would indicate an invalid database;
        //            // all other exceptions will be passed on.
        //            //
        //            // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
        //            //
        //            Log.Error(e);
        //            if (e.Number == 2812)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}

        #endregion

        private const string GetRepositoryVersionSqlCommand = "select dbo.fn_GetDatabaseVersion()";
        private const string GetDefaulManagementServiceStoredProcedure = "p_GetDefaultManagementService";
        private const string GetMonitoredSqlServersByIdStoredProcedure = "p_GetMonitoredSqlServerById";
        private const string GetMonitoredSqlServersStoredProcedure = "p_GetMonitoredSqlServers";
        private const string GetServerWideStatisticsStoredProcedure = "p_GetServerWideStatistics";
        private const string GetServerOverviewStoredProcedure = "p_GetServerOverview";
        private const string GetTasksStoredProcedure = "p_GetTasks";
        private const string GetMonitoredSqlServerStatusStoredProcedure = "p_GetMonitoredSqlServerStatus";
        private const string GetQueryMonitorStatementsStoredProcedure = "p_GetQueryMonitorStatements";

        private static readonly Logger Log = Logger.GetLogger("RepositoryHelper");

        public static bool IsValidRepository(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                    GetRepositoryVersionSqlCommand))
                    {
                        dataReader.Read();
                        return dataReader.GetString(0) == Constants.ValidRepositorySchemaVersion;
                    }
                }
                catch (SqlException e)
                {
                    // Assuming that a valid connection can be established to the SQL Server, 
                    // an invalid call to the version function would indicate an invalid database;
                    // all other exceptions will be passed on.
                    //
                    // Error 208 = is invalid object in SQL Server 2000
                    // Error 4121 - invalid object in SQL Server 2005
                    //
                    if (e.Number == 208 || e.Number == 4121)
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static ManagementServiceConfiguration GetDefaultManagementService(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                GetDefaulManagementServiceStoredProcedure))
                {
                    if (!dataReader.HasRows)
                    {
                        return null;
                    }

                    dataReader.Read();

                    string identifier = dataReader["ManagementServiceID"] as string;
                    string machineName = dataReader["MachineName"] as string;
                    string instanceName = dataReader["InstanceName"] as string;
                    string address = dataReader["Address"] as string;
                    int port = (int) dataReader["Port"];

                    return new ManagementServiceConfiguration(identifier, machineName, instanceName, address, port);
                }
            }
        }

        private static MonitoredSqlServer ConstructMonitoredSqlServer(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            int returnId = (int)dataReader["SQLServerID"];
            SqlConnectionInfo instanceConnectionInfo = new SqlConnectionInfo();
            instanceConnectionInfo.ApplicationName = Constants.CollectionServceConnectionStringApplicationName;
            instanceConnectionInfo.InstanceName = dataReader["InstanceName"] as string;
            bool isActive = (bool)dataReader["Active"];
            DateTime registeredDate = (DateTime)dataReader["RegisteredDate"];
            int collectionServiceIdColumn = dataReader.GetOrdinal("CollectionServiceID");
            SqlGuid sqlGuid = dataReader.GetSqlGuid(collectionServiceIdColumn);
            Guid collectionServiceId = sqlGuid.IsNull ? Guid.Empty : sqlGuid.Value;
            instanceConnectionInfo.UseIntegratedSecurity = (bool)dataReader["UseIntegratedSecurity"];

            if (!instanceConnectionInfo.UseIntegratedSecurity)
            {
                instanceConnectionInfo.UserName = dataReader["Username"] as string;
                instanceConnectionInfo.EncryptedPassword = dataReader["Password"] as string;
            }

            int scheduledCollectionInterval = (int)dataReader["ScheduledCollectionIntervalInSeconds"];
            bool maintenanceModeEnabled = (bool)dataReader["MaintenanceModeEnabled"];
            QueryMonitorConfiguration queryMonitorConfiguration = new QueryMonitorConfiguration(
                (bool)dataReader["QueryMonitorEnabled"],
                (bool)dataReader["QueryMonitorSqlBatchEventsEnabled"],
                (bool)dataReader["QueryMonitorSqlStatementEventsEnabled"],
                (bool)dataReader["QueryMonitorStoredProcedureEventsEnabled"],
                TimeSpan.FromMilliseconds((int)dataReader["QueryMonitorDurationFilterInMilliseconds"]),
                TimeSpan.FromMilliseconds((int)dataReader["QueryMonitorCpuUsageFilterInMilliseconds"]),
                (int)dataReader["QueryMonitorLogicalDiskReadsFilter"],
                (int)dataReader["QueryMonitorPhysicalDiskWritesFilter"],
                (bool)dataReader["QueryMonitorExcludeProfiler"],
                (bool)dataReader["QueryMonitorExcludeDMO"],
                (bool)dataReader["QueryMonitorExcludeAgent"],
                new FileSize((int)dataReader["QueryMonitorTraceFileSizeKB"]),
                (int)dataReader["QueryMonitorTraceFileRollovers"],
                (int)dataReader["QueryMonitorTraceRecordsPerRefresh"]);


            MonitoredSqlServer instance =
                new MonitoredSqlServer(returnId, registeredDate, isActive, collectionServiceId,
                                       instanceConnectionInfo,
                                       TimeSpan.FromSeconds(scheduledCollectionInterval),
                                       maintenanceModeEnabled, queryMonitorConfiguration);

            return instance;
        }

        public static MonitoredSqlServer GetMonitoredSqlServer(SqlConnectionInfo connectionInfo, int id)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetMonitoredSqlServersByIdStoredProcedure, id))
                {
                    if (!dataReader.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        dataReader.Read();
                        return ConstructMonitoredSqlServer(dataReader);
                    }
                }
            }
        }

        public static IList<MonitoredSqlServer> GetMonitoredSqlServers(SqlConnectionInfo connectionInfo,
                                                                                     bool activeOnly)
        {
            if (connectionInfo == null)
            {
                // Use Settings.Default.ActiveRepositoryConnection.ConnectionInfo.
                if (Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null) {
                    throw new ArgumentNullException("connectionInfo");
                } else {
                    connectionInfo = Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                }
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetMonitoredSqlServersStoredProcedure, null, activeOnly))
                {
                    List<MonitoredSqlServer> instances = new List<MonitoredSqlServer>();

                    while (dataReader.Read())
                    {
                        instances.Add(ConstructMonitoredSqlServer(dataReader));
                    }

                    return instances;
                }
            }
        }

        public static DataTable GetReportData(string storedProcName, SqlConnectionInfo connectionInfo, IList<int> servers, IList<DateRangeOffset> dateRanges, int sample) {
            using (Log.DebugCall()) {
                Log.Debug("storedProcName = ", storedProcName);
                if (connectionInfo == null) {
                    // Use Settings.Default.ActiveRepositoryConnection.ConnectionInfo.
                    if (Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null) {
                        throw new ArgumentNullException("connectionInfo");
                    } else {
                        connectionInfo = Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                    }
                }

                if (servers == null) {
                    throw new ArgumentNullException("servers");
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("ReportParameters");
                xmlDoc.AppendChild(rootElement);
                XmlAddServers(xmlDoc, servers);
                XmlAddDates(xmlDoc, dateRanges);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName)) {

                    if (Log.IsDebugEnabled) {
                        Log.Debug("Calling ", storedProcName, " with these parameters:");
                        Log.Debug("xmlDoc = ", FormatXml(xmlDoc));
                        Log.Debug("sample = ", sample);
                    }

                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, storedProcName,
                            xmlDoc.InnerXml, sample)) {

                        DataTable table = GetTable(dataReader);
                        Log.Debug(table.Rows.Count, " records were returned.");
                        return table;
                    }
                }
            }
        }

        public static string FormatXml(XmlDocument doc) {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);

            return sw.ToString();
        }

        // For each server, insert an XML element under the document's root node.
        private static void XmlAddServers(XmlDocument xmlDoc, IList<int> servers) {
            foreach (int id in servers) {
                XmlElement instanceElement = xmlDoc.CreateElement("SQLServer");
                instanceElement.SetAttribute("SQLServerID", id.ToString());
                xmlDoc.FirstChild.AppendChild(instanceElement);
            }
        }

        // For each DateRangeOffset, insert an XML element under the document's root node.
        private static void XmlAddDates(XmlDocument xmlDoc, IList<DateRangeOffset> dateRanges) {
            foreach (DateRangeOffset dro in dateRanges) {
                XmlElement elem = xmlDoc.CreateElement("AllowedDates");
                elem.SetAttribute("UtcStart", dro.UtcStart.ToString());
                elem.SetAttribute("UtcEnd", dro.UtcEnd.ToString());
                elem.SetAttribute("UtcOffset", dro.UtcOffset.ToString());
                xmlDoc.FirstChild.AppendChild(elem);
            }
        }

        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo, UserView view)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            XmlDocument serversParameter = new XmlDocument();
            XmlElement serversElement = serversParameter.CreateElement("SQLServers");
            serversParameter.AppendChild(serversElement);

            foreach (int id in view.Instances)
            {
                XmlElement instanceElement = serversParameter.CreateElement("SQLServer");
                instanceElement.SetAttribute("SQLServerID", id.ToString());
                serversElement.AppendChild(instanceElement);
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                // TODO: Get an hour of data for now, but this should be bound to a user setting
                DateTime historyMarker = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));

                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerWideStatisticsStoredProcedure,
                        serversParameter.InnerXml, 1, null, historyMarker.ToUniversalTime(), null))
                {
                    return GetTable(dataReader);
                }
            }
        }

        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                // TODO: Get an hour of data for now, but this should be bound to a user setting
                DateTime historyMarker = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));

                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerWideStatisticsStoredProcedure,
                        null, 1, null, historyMarker.ToUniversalTime(), null))
                {
                    return GetTable(dataReader);
                }
            }
        }

        public static DataTable GetServerOverview(SqlConnectionInfo connectionInfo, int id) {
            if (connectionInfo == null) {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName)) {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerOverviewStoredProcedure, id, null)) {
                    return GetTable(dataReader);
                }
            }
        }

        public static DataTable GetTasks(SqlConnectionInfo connectionInfo, DateTime fromDate, TaskStatus status, MonitoredStateFlags severity, string instance, string owner) {
            if (connectionInfo == null) {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName)) {
                Log.DebugFormat("GetTasks called with fromDate = {0}, status = {1}, severity = {2}, instance = {3}, owner = {4}.", fromDate, status, severity, instance, owner); 
                using (

                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetTasksStoredProcedure, fromDate, status, severity, instance, owner)) {
                    DataTable table = GetTable(dataReader);
                    Log.DebugFormat("Stored proc {0} returned {1} records.", GetTasksStoredProcedure, table.Rows.Count);
                    return table;
                }
            }
        }

        public static Pair<DataTable, DataTable> GetQueryMonitorStatements(
            SqlConnectionInfo connectionInfo, int id, DateTime startTime, DateTime endTime, 
            QueryMonitorSummaryInterval interval, bool includeSqlStatements, bool includeStoredProcedures,
            bool includeSqlBatches, string applicationNameFilter, string clientComputerNameFilter,
            string databaseNameFilter, string windowsUserNameFilter, string sqlUserNameFilter,
            string sqlTextFilter, Int64 durationFilter)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetQueryMonitorStatementsStoredProcedure,
                        id, startTime, endTime, (int)interval, includeSqlStatements, includeStoredProcedures,
                        includeSqlBatches, applicationNameFilter, clientComputerNameFilter, databaseNameFilter,
                        windowsUserNameFilter, sqlUserNameFilter, sqlTextFilter, durationFilter, 0))
                {
                    DataTable statementsTable = GetTable(dataReader);
                    dataReader.NextResult();
                    DataTable summaryTable = GetTable(dataReader);

                    return new Pair<DataTable, DataTable>(summaryTable, statementsTable);
                }
            }
        }

        public static DataTable GetTable(SqlConnectionInfo connectionInfo, string spName, params object[] parameterValues)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    return GetTable(dataReader);
                }
            }
        }

        private static DataTable GetTable(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                return null;
            }

            List<int> dateColumns = new List<int>();
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                if (!dataTable.Columns.Contains(schemaTable.Rows[i]["ColumnName"].ToString()))
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = schemaTable.Rows[i]["ColumnName"].ToString();
                    dataColumn.Unique = Convert.ToBoolean(schemaTable.Rows[i]["IsUnique"]);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTable.Rows[i]["AllowDBNull"]);
                    dataColumn.ReadOnly = Convert.ToBoolean(schemaTable.Rows[i]["IsReadOnly"]);
                    dataColumn.DataType = schemaTable.Rows[i]["DataType"] as Type;
                    dataTable.Columns.Add(dataColumn);

                    if (dataColumn.DataType == typeof(DateTime))
                        dateColumns.Add(i);
                }
            }

            object[] itemArray = new object[dataReader.FieldCount];

            dataTable.BeginLoadData();
            while (dataReader.Read())
            {
                dataReader.GetValues(itemArray);
                if (dateColumns.Count > 0)
                {
                    foreach (int columnIndex in dateColumns)
                    {
                        if (itemArray[columnIndex] != DBNull.Value)
                        {
                            itemArray[columnIndex] = ((DateTime)itemArray[columnIndex]).ToLocalTime();
                        }
                    }
                }

                dataTable.LoadDataRow(itemArray, true);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        public static XmlDocument GetMonitoredSqlServerStatus(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (XmlReader xmlReader =
                    SqlHelper.ExecuteXmlReader(connection, GetMonitoredSqlServerStatusStoredProcedure))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(xmlReader);
                    return document;
                }
            }
        }


        public static DataSource GetDataSource(SqlConnectionInfo connectionInfo, string spName, params object[] parameterValues)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    return new DataSource(dataReader);
                }
            }
        }

        public static void LoadDataSource<KeyColumnType>(SqlConnectionInfo connectionInfo, DataSourceWithID<KeyColumnType> dataSource, string spName, params object[] parameterValues)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    dataSource.Update(dataReader);
                }
            }
        }
    }

    internal enum QueryMonitorSummaryInterval
    {
        Minutes,
        Hours,
        Days,
        Months,
        Years
    }
}
*/