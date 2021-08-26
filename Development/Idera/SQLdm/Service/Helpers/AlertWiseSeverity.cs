using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers
{
    public class AlertWiseSeverity
    {
        private int[] CPU = { 0, 26, 27, 28, 29 };
        private int[] MEMORY = { 13, 24 };
        private int[] IO = { 25, 30, 31, 62, 63, 64, 76, 81, 82, 83, 84, 85, 86, 87, 111 };
        private int[] DATABASES = { 7, 8, 9, 11, 14, 20, 68, 69, 70, 71, 72, 73, 74, 75, 89, 90, 91, 92, 93, 94, 95, 109, 110, 116, 117, 118, 119, 120, 121, 122, 123, 124, 126, 127 };
        private int[] LOGS = { 66, 67 };
        private int[] QUERIES = { 51 };
        private int[] SERVICES = { 4, 5, 10, 12, 17, 18, 19, 21, 34, 35, 65, 77, 78, 79, 88, 128, 129 };
        private int[] SESSIONS = { 1, 6, 22, 32, 33, 57, 58, 80 };
        private int[] VIRTUALIZATION = { 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108 };
        private int[] OPERATIONAL = { 23, 48, 49, 50, 52, 53, 54, 56, 59, 60, 125 };

        public int ServerID { get; set; }
        public Dictionary<string, string> AlertSeverity;

        public AlertWiseSeverity()
        {
            AlertSeverity = new Dictionary<string, string>();
            AlertSeverity.Add("CPU", null);
            AlertSeverity.Add("MEMORY", null);
            AlertSeverity.Add("IO", null);
            AlertSeverity.Add("DATABASES", null);
            AlertSeverity.Add("LOGS", null);
            AlertSeverity.Add("QUERIES", null);
            AlertSeverity.Add("SERVICES", null);
            AlertSeverity.Add("SESSIONS", null);
            AlertSeverity.Add("VIRTUALIZATION", null);
            AlertSeverity.Add("OPERATIONAL", null);
        }


        /// <summary>
        /// Sets/Updates the max severity for a group of Alert Metric
        /// </summary>
        /// <param name="metricId">new metricID </param>
        /// <param name="newSeverity">new Severity for metricID </param>
        public void SetSeverity(int metricId, int newSeverity)
        {
            string Metric = null;
            if (CPU.Contains(metricId))
                Metric = "CPU";
            else if (MEMORY.Contains(metricId))
                Metric = "MEMORY";
            else if (IO.Contains(metricId))
                Metric = "IO";
            else if (DATABASES.Contains(metricId))
                Metric = "DATABASES";
            else if (LOGS.Contains(metricId))
                Metric = "LOGS";
            else if (QUERIES.Contains(metricId))
                Metric = "QUERIES";
            else if (SERVICES.Contains(metricId))
                Metric = "SERVICES";
            else if (SESSIONS.Contains(metricId))
                Metric = "SESSIONS";
            else if (VIRTUALIZATION.Contains(metricId))
                Metric = "VIRTUALIZATION";
            else if (OPERATIONAL.Contains(metricId))
                Metric = "OPERATIONAL";
            else return;

            string Severity = null;
            AlertSeverity.TryGetValue(Metric, out Severity);
            if (Severity == null || Convert.ToInt32(Severity) < newSeverity)
                AlertSeverity[Metric] = newSeverity.ToString();
        }
    }
}
