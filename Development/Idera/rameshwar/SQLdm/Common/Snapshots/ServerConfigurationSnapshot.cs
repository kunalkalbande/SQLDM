//------------------------------------------------------------------------------
// <copyright file="ServerConfigurationSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the ServerConfiguration information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class ServerConfigurationSnapshot : Snapshot
    {
        #region fields

        private DataTable serverConfiguration = new DataTable("ServerConfiguration");
        private DataTable vulnerableLogins = new DataTable("VulnerableLogins");
        private DataTable securitySettings = new DataTable("SecuritySettings");
        private DataTable deprecatedAgentTokenJobs = new DataTable("DeprecatedAgentTokenJobs");
        private bool traceFlag4199 = false;  // SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR-Q37, SDR-Q38)
        // SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR-Q45)
        private bool traceFlag2312 = false;
        private bool traceFlag9481 = false;
        private Dictionary<string, int> compatibility = new Dictionary<string, int>(); // SQLdm10.0 -Srishti Purohit -  New Recommendations(SDR-Q37, SDR-Q38)
        private long maxServerMemorySizeMB; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private long bufferPoolExtSizeKB;  // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private string bufferPoolExtFilePath = ""; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-M31, SDR-M32)
        private string edition = ""; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private bool isResourceGovernerEnable = false; // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private List<string> resourcePoolNameList = new List<string>(); // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-D23)
        private Dictionary<string, bool> availabilityGroups = new Dictionary<string, bool>(); // SQLDm 10.0 srishti purohit -- new recommendations (SDR-R8)


        #endregion

        #region constructors

        internal ServerConfigurationSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            serverConfiguration.RemotingFormat = SerializationFormat.Binary;
            vulnerableLogins.RemotingFormat = SerializationFormat.Binary;
            securitySettings.RemotingFormat = SerializationFormat.Binary;
            deprecatedAgentTokenJobs.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable ServerConfiguration
        {
            get { return serverConfiguration; }
            internal set { serverConfiguration = value; }
        }

        public DataTable VulnerableLogins
        {
            get { return vulnerableLogins; }
            internal set { vulnerableLogins = value; }
        }

        public DataTable SecuritySettings
        {
            get { return securitySettings; }
            internal set { securitySettings = value; }
        }

        public DataTable DeprecatedAgentTokenJobs
        {
            get { return deprecatedAgentTokenJobs; }
            internal set { deprecatedAgentTokenJobs = value; }
        }

        public bool TraceFlag4199
        {
            get { return traceFlag4199; }
            internal set { traceFlag4199 = value; }
        }

        public Dictionary<string, int> Compatibility
        {
            get { return compatibility; }
            internal set { compatibility = value; }
        }

        // SQLdm10.0 -Srishti Purohit -- new recommendations (SDR-Q45)
        public bool TraceFlag2312 
        {
            get { return traceFlag2312; }
            internal set { traceFlag2312 = value; }
        }
        public bool TraceFlag9481
        {
            get { return traceFlag9481; }
            internal set { traceFlag9481 = value; }
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
        #endregion
        
        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
