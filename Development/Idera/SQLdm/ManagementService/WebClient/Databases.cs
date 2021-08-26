using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.ManagementService.WebClient
{
    internal class Databases
    {
        private Dictionary<string,int> databaseStatus = new Dictionary<string, int>();

        public DataTable GetDatbases(int monitoredServerId, bool includeSystemDatabases)
        {
            DatabaseSummaryConfiguration configuration = new DatabaseSummaryConfiguration(monitoredServerId, includeSystemDatabases);
            configuration.IncludeSummaryData = true;
            ManagementService service = new ManagementService();
            DatabaseSummary summary = service.GetDatabaseSummary(configuration);
            if (summary.Error != null)
                throw summary.Error;

            DataTable result = new DataTable("Databases");
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Status", typeof(string));
            result.Columns.Add("Sessions", typeof(int));
            result.Columns.Add("State", typeof(int));
            result.Columns.Add("Type", typeof (string));
            result.Columns.Add("DataFileSize", typeof(decimal));
            result.Columns.Add("DataFilePctUsed", typeof(float));
            result.Columns.Add("LogFileSize", typeof(decimal));
            result.Columns.Add("LogFilePctUsed", typeof(float));
            result.Columns.Add("UserTables", typeof(int));
            result.Columns.Add("OldestOpenStart", typeof(DateTime));
            result.Columns.Add("LastBackup", typeof(DateTime));
            result.Columns.Add("Transactions", typeof(long));

            MapDatabaseStatus(monitoredServerId);

            foreach (DatabaseDetail detail in  summary.Databases.Values)
            {
                DataRow row = result.NewRow();
                row[0] = detail.Name;
                row[1] = detail.Status.ToString();
                row[2] = Sessions.GetNullableValue(detail.ProcessCount);
                row[3] = GetDatabaseState(detail.Name);
                row[4] = detail.IsSystemDatabase ? "System" : "User";
                row[5] = Sessions.GetNullableValue(detail.DataFileSize.Bytes);
                row[6] = Sessions.GetNullableValue(detail.PercentDataSize);
                row[7] = Sessions.GetNullableValue(detail.LogFileSize.Bytes);
                row[8] = Sessions.GetNullableValue(detail.PercentLogSpace);
                row[9] = Sessions.GetNullableValue(detail.UserTables);
                row[10] = Sessions.GetNullableValue(detail.OldestOpenTransactionStartTime);
                row[11] = Sessions.GetNullableValue(detail.LastBackup);
                row[12] = Sessions.GetNullableValue(detail.Transactions);                

                result.Rows.Add(row);
            }

            return result;
        }

        public DataTable GetDatbaseFileInfo(int monitoredServerId, string databaseName)
        {
            List<string> databaseNames = new List<string>();
            databaseNames.Add(databaseName);
            DatabaseFilesConfiguration configuration = new DatabaseFilesConfiguration(monitoredServerId, databaseNames);
            ManagementService service = new ManagementService();
            DatabaseFilesSnapshot summary = service.GetDatabaseFiles(configuration);
            if (summary.Error != null)
                throw summary.Error;

            DataTable result = new DataTable("DatabaseFileInfo");
            result.Columns.Add("DatabaseName", typeof(string));
            result.Columns.Add("FilegroupName", typeof(string));
            result.Columns.Add("LogicalFileName", typeof(string));
            result.Columns.Add("IsDataFile", typeof(bool));
            result.Columns.Add("DriveName", typeof(string));
            result.Columns.Add("FilePath", typeof(string));
            result.Columns.Add("GrowthLabel", typeof(string));
            result.Columns.Add("FreeSpaceOnDisk", typeof(decimal));
            result.Columns.Add("CurrentSize", typeof(decimal));
            result.Columns.Add("CurrentUsedSize", typeof(decimal));
            result.Columns.Add("CurrentFreeSize", typeof(decimal));
            result.Columns.Add("CurrentPotentialFreeSize", typeof(decimal));
            result.Columns.Add("MaximumPotentialFileSize", typeof(decimal));
            result.Columns.Add("PercentUsed", typeof(float));
            result.Columns.Add("PercentFull", typeof(float));
            result.Columns.Add("PercentFreePotential", typeof(float));
            result.Columns.Add("PercentFreeCurrent", typeof(float));
            result.Columns.Add("ExpansionSpace", typeof(decimal));
            result.Columns.Add("InternalObjectsSpace", typeof(decimal));
            result.Columns.Add("MixedExtentsSpace", typeof(decimal));
            result.Columns.Add("UnallocatedSpace", typeof(decimal));
            result.Columns.Add("UserObjectsSpace", typeof(decimal));
            result.Columns.Add("VersionStoreSpace", typeof(decimal));
            result.Columns.Add("TempdbFileSize", typeof(decimal));
            result.Columns.Add("VersionStoreCleanupKilobytes", typeof(decimal));
            result.Columns.Add("VersionStoreGenerationKilobytes", typeof(decimal));
            result.Columns.Add("TimeDelta", typeof(TimeSpan));

            foreach (DatabaseFile detail in  summary.DatabaseFiles.Values)
            {
                DataRow row = result.NewRow();
                row[0] = detail.DatabaseName;
                row[1] = Sessions.GetNullableString(detail.FilegroupName);
                row[2] = Sessions.GetNullableString(detail.LogicalFilename);
                row[3] = detail.IsDataFile;
                row[4] = Sessions.GetNullableString(detail.DriveName);
                row[5] = Sessions.GetNullableString(detail.FilePath);
                row[6] = Sessions.GetNullableString(detail.GrowthLabel(CultureInfo.CurrentCulture));
                row[7] = Sessions.GetNullableValue(detail.FreeSpaceOnDisk.Bytes);
                row[8] = Sessions.GetNullableValue(detail.CurrentSize.Bytes);
                row[9] = Sessions.GetNullableValue(detail.CurrentUsedSize.Bytes);
                row[10] = Sessions.GetNullableValue(detail.CurrentFreeSize.Bytes);
                row[11] = Sessions.GetNullableValue(detail.CurrentPotentialFreeSize.Bytes);
                row[12] = Sessions.GetNullableValue(detail.MaximumPotentialFileSize.Bytes);
                row[13] = Sessions.GetNullableValue(detail.PercentUsed);
                row[14] = Sessions.GetNullableValue(detail.PercentFull);
                row[15] = Sessions.GetNullableValue(detail.PercentFreePotential);
                row[16] = Sessions.GetNullableValue(detail.PercentFreeCurrent);
                row[17] = Sessions.GetNullableValue(detail.ExpansionSpace.Bytes);

                if (detail.TempdbFileActivity != null)
                {
                    //row[0] = detail.TempdbFileActivity.Filename;
                    row[0] = detail.TempdbFileActivity.FileSize;
                    row[0]  = Sessions.GetNullableString(detail.TempdbFileActivity.DatabaseName);
                    row[3]  = detail.TempdbFileActivity.FileType == FileActivityFileType.Data;
                    row[4]  = Sessions.GetNullableString(detail.TempdbFileActivity.DriveName);
                    row[5]  = Sessions.GetNullableString(detail.TempdbFileActivity.Filepath);                                     
                    row[18] = Sessions.GetNullableValue(detail.TempdbFileActivity.InternalObjects);
                    row[19] = Sessions.GetNullableValue(detail.TempdbFileActivity.MixedExtents);
                    row[20] = Sessions.GetNullableValue(detail.TempdbFileActivity.UnallocatedSpace);
                    row[21] = Sessions.GetNullableValue(detail.TempdbFileActivity.UserObjects);
                    row[22] = Sessions.GetNullableValue(detail.TempdbFileActivity.VersionStore);
                    row[23] = Sessions.GetNullableValue(detail.TempdbFileActivity.FileSize);
                }

                if (summary.TempdbStatistics != null)
                {
                    row[24] = Sessions.GetNullableValue(summary.TempdbStatistics.VersionStoreCleanupKilobytes);
                    row[25] = Sessions.GetNullableValue(summary.TempdbStatistics.VersionStoreGenerationKilobytes);
                    row[26] = summary.TempdbStatistics.TimeDelta ?? TimeSpan.Zero;
                }

                result.Rows.Add(row);
            }

            return result;
        }

        private void MapDatabaseStatus(int monitoredServerId)
        {
            string statusXml = Management.ScheduledCollection.MonitoredSQLServerStatusDocument;
            if (!String.IsNullOrEmpty(statusXml))
            {
                databaseStatus.Clear();
                XmlDocument statusDocument = new XmlDocument();
                statusDocument.LoadXml(statusXml);
                string nodequery = String.Format("/Servers/Server[@SQLServerID={0}]/Category[@Name='Databases']/Database", monitoredServerId);
                XmlNodeList databaseNodes = statusDocument.SelectNodes(nodequery);
                if (databaseNodes == null) return;

                foreach (XmlElement databaseNode in databaseNodes)
                {
                    string state = "0";
                    string dbname = databaseNode.GetAttribute("Name");
                    foreach (XmlElement stateNode in databaseNode.ChildNodes)
                    {
                        if (stateNode.Name.Equals("State"))
                        {
                            string severity = stateNode.GetAttribute("Severity");
                            if (string.IsNullOrEmpty(severity)) continue;
                            if (state.CompareTo(severity) < 0)
                                state = severity;
                        }
                    }

                    int s = 0;
                    int.TryParse(state, out s);

                    if (!databaseStatus.ContainsKey(dbname))
                        databaseStatus.Add(dbname, s);
                    else
                    {
                        if (databaseStatus[dbname] < s)
                            databaseStatus[dbname] = s;
                    }
                }
            }
        }

        private object GetDatabaseState(string dbname)
        {
            if (databaseStatus == null)
                return DBNull.Value;

            int state = 0;
            databaseStatus.TryGetValue(dbname, out state);

            return state;
        }
    }
}
