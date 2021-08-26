using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wintellect.PowerCollections;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class ServerConfigurationMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("ServerConfigurationMetrics");
        private DataTable configurationSettings;
        private DataTable securitySettings;
        private List<Triple<string, bool, bool>> vulnerableLogins = new List<Triple<string, bool, bool>>();
        private List<Pair<string, string>> deprecatedAgentTokenJobs = new List<Pair<string, string>>();
        private bool traceFlag4199 = false;  // SQLdm10.0 -Srishti Purohit -  New Recommendations
        private Dictionary<string, int> compatibility = new Dictionary<string, int>(); // SQLdm10.0 -Srishti Purohit -  New Recommendations

        private bool traceFlag2312 = false;  // SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR_Q45)
        private bool traceFlag9481 = false;  // SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR_Q45)
        private long maxServerMemorySizeMB; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private long bufferPoolExtSizeKB;  // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private string bufferPoolExtFilePath = ""; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private string edition = ""; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private bool isResourceGovernerEnable = false; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private List<string> resourcePoolNameList = new List<string>(); // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private Dictionary<string, bool> availabilityGroups = new Dictionary<string, bool>(); // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-R8)

        public DataTable ConfigurationSettings
        {
            get { return configurationSettings; }
        }

        public DataTable SecuritySettings
        {
            get { return securitySettings; }
        }

        public List<Triple<string, bool, bool>> VulnerableLogins
        {
            get { return vulnerableLogins; }
        }

        public List<Pair<string, string>> DeprecatedAgentTokenJobs
        {
            get { return deprecatedAgentTokenJobs; }
        }

        // SQLdm10.0 -Srishti Purohit -  New Recommendations
        public bool TraceFlag4199
        {
            get { return traceFlag4199; }
        }

        // SQLdm10.0 -Srishti Purohit -  New Recommendations
        public Dictionary<string, int> Compatibility
        {
            get { return compatibility; }
        }

        // SQLdm10.0 -Srishti Purohit -  New Recommendations SDR-Q45
        public bool TraceFlag2312
        {
            get { return traceFlag2312; }
        }
        // SQLdm10.0 -Srishti Purohit -  New Recommendations SDR-Q45
        public bool TraceFlag9481
        {
            get { return traceFlag9481; }
        }
        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        public string BufferPoolExtFilePath
        {
            get { return bufferPoolExtFilePath; }
            internal set { bufferPoolExtFilePath = value; }
        }

        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        public long MaxServerMemorySizeMB
        {
            get { return maxServerMemorySizeMB; }
            internal set { maxServerMemorySizeMB = value; }
        }

        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        public long BufferPoolExtSizeKB
        {
            get { return bufferPoolExtSizeKB; }
            internal set { bufferPoolExtSizeKB = value; }
        }
        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        public string Edition
        {
            get { return edition; }
            internal set { edition = value; }
        }

        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        public bool IsResourceGovernerEnable
        {
            get { return isResourceGovernerEnable; }
            internal set { isResourceGovernerEnable = value; }
        }

        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        public List<string> ResourcePoolNameList
        {
            get { return resourcePoolNameList; }
            internal set { resourcePoolNameList = value; }
        }
        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-R8)
        public Dictionary<string, bool> AvailabilityGroups
        {
            get { return availabilityGroups; }
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("ServerConfigurationMetrics not added : " + snapshot.Error); return; }

            if (snapshot.ServerConfigurationSnapshotValueStartup == null) { return; }
            if (snapshot.ServerConfigurationSnapshotValueStartup.ServerConfiguration != null)
            {
                configurationSettings = snapshot.ServerConfigurationSnapshotValueStartup.ServerConfiguration;
            }
            if (snapshot.ServerConfigurationSnapshotValueStartup.SecuritySettings != null)
            {
                securitySettings = snapshot.ServerConfigurationSnapshotValueStartup.SecuritySettings;
            }
            if (snapshot.ServerConfigurationSnapshotValueStartup.VulnerableLogins != null && snapshot.ServerConfigurationSnapshotValueStartup.VulnerableLogins.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.ServerConfigurationSnapshotValueStartup.VulnerableLogins.Rows.Count; index++)
                {
                    DataRow r = snapshot.ServerConfigurationSnapshotValueStartup.VulnerableLogins.Rows[index];
                    string name = DataHelper.ToString(r, "username");
                    bool policy = DataHelper.ToBoolean(r, "policy");
                    bool expire = DataHelper.ToBoolean(r, "expire");
                    vulnerableLogins.Add(new Triple<string, bool, bool>(name, policy, expire));
                }
            }
            if (snapshot.ServerConfigurationSnapshotValueStartup.DeprecatedAgentTokenJobs != null && snapshot.ServerConfigurationSnapshotValueStartup.DeprecatedAgentTokenJobs.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.ServerConfigurationSnapshotValueStartup.DeprecatedAgentTokenJobs.Rows.Count; index++)
                {
                    DataRow r = snapshot.ServerConfigurationSnapshotValueStartup.DeprecatedAgentTokenJobs.Rows[index];
                    try
                    {
                        deprecatedAgentTokenJobs.Add(new Pair<string, string>(DataHelper.ToString(r, "name"), DataHelper.ToString(r, "step_name")));
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                }
            }
            if (snapshot.ServerConfigurationSnapshotValueStartup.TraceFlag4199 != null)
            {
                traceFlag4199 = snapshot.ServerConfigurationSnapshotValueStartup.TraceFlag4199;
            }
            if (snapshot.ServerConfigurationSnapshotValueStartup.Compatibility != null && snapshot.ServerConfigurationSnapshotValueStartup.Compatibility.Count > 0)
            {
                compatibility = snapshot.ServerConfigurationSnapshotValueStartup.Compatibility;
            }
            //SQLdm10.0 Srishti Purohit SDR-Q45
            traceFlag2312 = snapshot.ServerConfigurationSnapshotValueStartup.TraceFlag2312;
            traceFlag9481 = snapshot.ServerConfigurationSnapshotValueStartup.TraceFlag9481;
            //SQLdm10.0 Srishti Purohit SDR-M31, SDR-M32
            maxServerMemorySizeMB = snapshot.ServerConfigurationSnapshotValueStartup.MaxServerMemorySizeMB;
            bufferPoolExtSizeKB = snapshot.ServerConfigurationSnapshotValueStartup.BufferPoolExtSizeKB;
            bufferPoolExtFilePath = snapshot.ServerConfigurationSnapshotValueStartup.BufferPoolExtFilePath;
            // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
            edition = snapshot.ServerConfigurationSnapshotValueStartup.Edition;
            isResourceGovernerEnable = snapshot.ServerConfigurationSnapshotValueStartup.IsResourceGovernerEnable;
            resourcePoolNameList = snapshot.ServerConfigurationSnapshotValueStartup.ResourcePoolNameList;
            // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-R8)
            availabilityGroups = snapshot.ServerConfigurationSnapshotValueStartup.AvailabilityGroups;
        }
    }
}
