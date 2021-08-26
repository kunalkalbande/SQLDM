using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Snapshots;
using Microsoft.ApplicationBlocks.Data;
using Wintellect.PowerCollections;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.WebClient
{
    public class Sessions
    {
        public DataTable GetSessionMetrics(int monitoredServerId, TimeSpan? backfill)
        {
            int? historyMinutes = null;
            if (backfill.HasValue && backfill.Value.TotalMinutes > 0)
                historyMinutes = (int)backfill.Value.TotalMinutes;

            return GetSessionStatistics(monitoredServerId, historyMinutes);
        }

        private const string GetSessionStatisticsStoredProcedure = "p_GetSessionStatistics";

        private static DataTable GetSessionStatistics(int monitoredServerId, int? historyInMinutes)
        {
            DataTable result = new DataTable("SessionMetrics");
            result.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            result.Columns.Add("ResponseTimeMs", typeof(int));
            result.Columns.Add("ClientComputers", typeof(int));
            result.Columns.Add("ActiveProcesses", typeof(int));
            result.Columns.Add("IdleProcesses", typeof(int));
            result.Columns.Add("SystemProcesses", typeof(int));
            result.Columns.Add("Blocked", typeof(int));
            result.Columns.Add("LeadBlockers", typeof(int));
            result.Columns.Add("Deadlocks", typeof(long));

            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection,
                                                                          GetSessionStatisticsStoredProcedure, 
                                                                          monitoredServerId,
                                                                          null, 
                                                                          historyInMinutes))
                {
                    while (dataReader.Read())
                    {
                        DataRow row = result.NewRow();
                        row[0] = dataReader[0];
                        row[1] = dataReader[1];
                        row[2] = dataReader[2];
                        row[3] = dataReader[3];
                        row[4] = dataReader[4];
                        row[5] = dataReader[5];
                        row[6] = dataReader[6];
                        row[7] = dataReader[7];

                        LockStatistics lockStatistics = null;
                        object lockblob = dataReader[8];
                        if (lockblob is byte[])
                        {
                            lockStatistics =  Serialized<object>.DeserializeCompressed<LockStatistics>((byte[])lockblob);
                            row[8] = lockStatistics.TotalCounters.Deadlocks.HasValue
                                         ? (object)lockStatistics.TotalCounters.Deadlocks.Value
                                         : DBNull.Value;
                        }
                        result.Rows.Add(row);
                    }
                }
            }
            return result;
        }

        public DataTable GetSessionSummary(SessionSummaryConfiguration configuration)
        {
            ManagementService service = new ManagementService();
            SessionSummary summary = service.GetSessionSummary(configuration);

            if (summary.Error != null)
                throw summary.Error;

            DataTable result = new DataTable("SessionSummary");
            result.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            result.Columns.Add("ActiveProcesses", typeof(int));
            result.Columns.Add("BlockedProcesses", typeof(int));
            result.Columns.Add("LeadBlockers", typeof(int));
            result.Columns.Add("IdleProcesses", typeof (int));
            result.Columns.Add("SystemProcesses", typeof (int));
            result.Columns.Add("Clients", typeof(int));

            DataRow row = result.NewRow();
            row[0] = GetNullableValue(summary.TimeStamp);
            row[1] = GetNullableValue(summary.SystemProcesses.ActiveProcesses);
            row[2] = GetNullableValue(summary.SystemProcesses.BlockedProcesses);
            row[3] = GetNullableValue(summary.SystemProcesses.LeadBlockers);
            row[4] = GetNullableValue(summary.SystemProcesses.CurrentProcesses - summary.SystemProcesses.ActiveProcesses);
            row[5] = GetNullableValue(summary.SystemProcesses.CurrentSystemProcesses);
            row[6] = GetNullableValue(summary.SystemProcesses.ComputersHoldingProcesses);
            
            result.Rows.Add(row);

            return result;
        }

        public DataTable GetSessions(SessionsConfiguration configuration, bool shortList)
        {
            ManagementService service = new ManagementService();
            SessionSnapshot snapshot = service.GetSessions(configuration);

            if (snapshot.Error != null)
                throw snapshot.Error;

            DataTable result = new DataTable("Sessions");
            result.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            result.Columns.Add("Spid", typeof(int));
            result.Columns.Add("Status", typeof(string));
            result.Columns.Add("User", typeof(string));
            result.Columns.Add("Database", typeof(string));
            result.Columns.Add("Application", typeof(string));
            result.Columns.Add("Cpu", typeof(TimeSpan));
            result.Columns.Add("IO", typeof(long));
            result.Columns.Add("Memory", typeof(decimal));

            if (!shortList)
            {
                result.Columns.Add("Host", typeof (string));
                result.Columns.Add("NetLibrary", typeof (string));
                result.Columns.Add("NetworkAddress", typeof (string));
                result.Columns.Add("OpenTransactions", typeof (long));
                result.Columns.Add("LoginTime", typeof (DateTime));
                result.Columns.Add("LastActivity", typeof(DateTime));
                result.Columns.Add("WaitTime", typeof(TimeSpan));
                result.Columns.Add("WaitType", typeof(string));
                result.Columns.Add("WaitResource", typeof(string));
                result.Columns.Add("Blocking", typeof(bool));
                result.Columns.Add("BlockedBy", typeof(int));
                result.Columns.Add("LastCommand", typeof(string));
                result.Columns.Add("Type", typeof (string));
                result.Columns.Add("CpuDelta", typeof (TimeSpan));
                result.Columns.Add("IODelta", typeof (long));
                result.Columns.Add("SessionUserAllocatedTotal", typeof(decimal));
                result.Columns.Add("SessionUserDeallocatedTotal", typeof(decimal));
                result.Columns.Add("SessionInternalAllocatedTotal", typeof(decimal));
                result.Columns.Add("SessionInternalDeallocatedTotal", typeof(decimal));
                result.Columns.Add("TaskUserAllocatedTotal", typeof(decimal));
                result.Columns.Add("TaskUserDeallocatedTotal", typeof(decimal));
                result.Columns.Add("TaskInternalAllocatedTotal", typeof(decimal));
                result.Columns.Add("TaskInternalDeallocatedTotal", typeof(decimal));
                result.Columns.Add("VersionStoreElapsedTime", typeof(TimeSpan));
            }


            foreach (Session session in snapshot.SessionList.Values)
            {
                DataRow row = result.NewRow();

                row[0] = GetNullableValue(snapshot.TimeStamp);
                row[1] = GetNullableValue(session.Spid);
                row[2] = session.Status.ToString(); 
                row[3] = GetNullableString(session.UserName);
                row[4] = GetNullableString(session.Database);
                row[5] = GetNullableString(session.Application);
                row[6] = session.Cpu;
                row[7] = GetNullableValue(session.PhysicalIo);
                row[8] = GetNullableValue(session.Memory.Bytes);

                if (!shortList)
                {
                    row[9] = GetNullableString(session.Workstation);
                    row[10] = GetNullableString(session.NetLibrary);
                    row[11] = GetNullableString(session.WorkstationNetAddress);
                    row[12] = GetNullableValue(session.OpenTransactions);
                    row[13] = GetNullableValue(session.LoggedInSince);
                    row[14] = GetNullableValue(session.LastActivity);
                    row[15] = session.WaitTime;
                    row[16] = session.WaitType ?? String.Empty;
                    row[17] = session.WaitResource ?? String.Empty;
                    row[18] = session.Blocking;
                    row[19] = GetNullableValue(session.BlockedBy);
                    row[20] = GetNullableString(session.LastCommand);
                    row[21] = GetNullableString(session.IsUserProcess ? "User" : "System");
                    row[22] = session.CpuDelta;
                    row[23] = GetNullableValue(session.PhysicalIoDelta);
                    row[24] = GetNullableValue(session.SessionUserAllocatedTotal);
                    row[25] = GetNullableValue(session.SessionUserDeallocatedTotal);
                    row[26] = GetNullableValue(session.SessionInternalAllocatedTotal);
                    row[27] = GetNullableValue(session.SessionInternalDeallocatedTotal);
                    row[28] = GetNullableValue(session.TaskUserAllocatedTotal);
                    row[29] = GetNullableValue(session.TaskUserDeallocatedTotal);
                    row[30] = GetNullableValue(session.TaskInternalAllocatedTotal);
                    row[31] = GetNullableValue(session.TaskInternalDeallocatedTotal);
                    row[32] = session.VersionStoreElapsedTime ?? TimeSpan.Zero;
                }

                result.Rows.Add(row);
            }

            return result;
        }

        public static object GetNullableString(string value)
        {
            return value ?? (object) DBNull.Value;
        }

        public static object GetNullableValue(FileSize size)
        {
            if (size == null)
                return DBNull.Value;

            return GetNullableValue(size.Bytes);
        }

        public static object GetNullableValue<T>(T? value) where T : struct
        { 
            if (value.HasValue)
                return value.Value;
            return DBNull.Value;
        }
    }
}
