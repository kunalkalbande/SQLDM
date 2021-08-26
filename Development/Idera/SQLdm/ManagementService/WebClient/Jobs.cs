using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.ManagementService.WebClient
{
    internal class Jobs
    {
        public DataTable GetJobs(int monitoredServerId, AgentJobSummaryConfiguration.JobSummaryFilterType type, TimeSpan? timePeriod)
        {
            AgentJobSummaryConfiguration configuration = new AgentJobSummaryConfiguration(monitoredServerId, type, timePeriod);
            ManagementService service = new ManagementService();
            AgentJobSummary summary = service.GetAgentJobSummary(configuration);
            if (summary.Error != null)
                throw summary.Error;

            DataTable jobTable = new DataTable("Jobs");
            jobTable.Columns.Add("JobId", typeof (Guid));
            jobTable.Columns.Add("JobName", typeof(string));
            jobTable.Columns.Add("Category", typeof(string));
            jobTable.Columns.Add("Enabled", typeof(bool));
            jobTable.Columns.Add("Scheduled", typeof (bool));
            jobTable.Columns.Add("Status", typeof(string));
            jobTable.Columns.Add("LastRunStatus", typeof(string));
            jobTable.Columns.Add("LastRunStartTime", typeof(DateTime));
            jobTable.Columns.Add("LastRunDuration", typeof (TimeSpan));
            jobTable.Columns.Add("NextRunStartTime", typeof(DateTime));
            jobTable.Columns.Add("Owner", typeof(string));
            jobTable.Columns.Add("Description", typeof(string));
            jobTable.Columns.Add("IsRunning", typeof (bool));

            jobTable.ExtendedProperties.Add("AgentStatus", (string)summary.AgentServiceState.ToString());

            foreach (AgentJob job in summary.Jobs.Values)
            {
                DataRow row = jobTable.NewRow();

                row[0] = job.JobId;
                row[1] = job.JobName;
                row[2] = job.Category ?? String.Empty;
                row[3] = Sessions.GetNullableValue(job.Enabled);
                row[4] = Sessions.GetNullableValue(job.Scheduled);
                if (job.Status.HasValue)
                    row[5] = job.Status.Value.ToString();
                else
                    row[5] = DBNull.Value;
                if (job.LastRunStatus.HasValue)
                    row[6] = job.LastRunStatus.ToString();
                else
                    row[6] = DBNull.Value;
                row[7] = Sessions.GetNullableValue(job.LastRunStartTime);
                row[8] = job.RunDuration;
                row[9] = Sessions.GetNullableValue(job.NextRunDate);
                row[10] = job.Owner ?? String.Empty;
                row[11] = job.JobDescription ?? String.Empty;

                row[12] = job.IsRunning;

                jobTable.Rows.Add(row);
            }

            return jobTable;
        }

        public DataTable GetJobHistory(int monitoredServerId, Guid jobId, bool failedOnly, int maxRows)
        {
            AgentJobHistoryConfiguration configuration = new AgentJobHistoryConfiguration(monitoredServerId, failedOnly);
            configuration.JobIdList.Add(jobId);

            ManagementService service = new ManagementService();
            AgentJobHistorySnapshot history = service.GetAgentJobHistory(configuration);

            DataTable jobTable = new DataTable("JobHistory");
            jobTable.Columns.Add("JobId", typeof(Guid));
            jobTable.Columns.Add("JobName", typeof(string));
            jobTable.Columns.Add("Status", typeof(string));
            jobTable.Columns.Add("StartTime", typeof (DateTime));
            jobTable.Columns.Add("Duration", typeof (TimeSpan));
            jobTable.Columns.Add("Retries", typeof(int));
            jobTable.Columns.Add("Message", typeof(string));

            AgentJobHistory job;
            if (history.JobHistories.TryGetValue(jobId, out job))
            {
                List<AgentJobExecution> historyList = job.Executions;
                int end = maxRows == 0 ? 0 : historyList.Count - maxRows;
                if (end < 0) end = 0;

                // return last x rows in reverse order
                for (int i = historyList.Count - 1; i >= end; i--)
                {
                    AgentJobExecution execution = historyList[i];
                    DataRow row = jobTable.NewRow();
                    
                    row[0] = job.JobId;
                    row[1] = job.Name;
                    if (execution.Outcome.RunStatus.HasValue)
                        row[2] = execution.Outcome.RunStatus.ToString();
                    else
                        row[2] = DBNull.Value;

                    row[3] = Sessions.GetNullableValue(execution.Outcome.StartTime);
                    row[4] = execution.Outcome.RunDuration;
                    row[5] = Sessions.GetNullableValue(execution.Outcome.Retries);
                    row[6] = execution.Outcome.Message ?? String.Empty;

                    jobTable.Rows.Add(row);
                }
            }

            return jobTable;
        }
    }
}
