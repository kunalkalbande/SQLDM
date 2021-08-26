//------------------------------------------------------------------------------
// <copyright file="ServerSystemProcesses.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an overview of the sampled SQL Server system processes.
    /// </summary>
    [Serializable]
    public sealed class ServerSystemProcesses : ISerializable
    {
        #region fields

        private int? computersHoldingProcesses;
        private int? currentSystemProcesses;
        private int? currentUserProcesses;
        private int? userProcessesConsumingCpu;
        private int? systemProcessesConsumingCpu;
        private int? blockedProcesses;
        private int? openTransactions = null;
        private int? leadBlockers = null;
        private int? activeProcesses = null;

        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baseline
        private double? userConnectionPctBaselineMean = null;
        private double? userConnectionPctAsBaselinePerc = null;

        private double? computersHoldingProcessesBaselineMean = null;
        private double? computersHoldingProcessesAsBaselinePerc = null;

        private double? blockedProcessesBaselineMean = null;
        private double? blockedProcessesAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baseline

        #endregion

        #region constructors

        public ServerSystemProcesses()
        {
            computersHoldingProcesses = null;
            currentSystemProcesses = null;
            currentUserProcesses = null;
            userProcessesConsumingCpu = null;
            systemProcessesConsumingCpu = null;
            blockedProcesses = null;
            openTransactions = null;
            leadBlockers = null;
            activeProcesses = null;
        }

        public ServerSystemProcesses(SerializationInfo info, StreamingContext context)
        {
            computersHoldingProcesses = (int?)info.GetValue("computersHoldingProcesses", typeof(int?));
            currentSystemProcesses = (int?)info.GetValue("currentSystemProcesses", typeof(int?));
            currentUserProcesses = (int?)info.GetValue("currentUserProcesses", typeof(int?));
            userProcessesConsumingCpu = (int?)info.GetValue("userProcessesConsumingCpu", typeof(int?));
            systemProcessesConsumingCpu = (int?)info.GetValue("systemProcessesConsumingCpu", typeof(int?));
            blockedProcesses = (int?)info.GetValue("blockedProcesses", typeof(int?));
            openTransactions = (int?)info.GetValue("openTransactions", typeof(int?));
            leadBlockers = (int?)info.GetValue("leadBlockers", typeof(int?));
            activeProcesses = (int?)info.GetValue("activeProcesses", typeof(int?));
        }

        #endregion

        #region properties

        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baseline
        public double? UserConnectionPctBaselineMean
        {
            get { return userConnectionPctBaselineMean; }
            internal set { userConnectionPctBaselineMean = value; }
        }
        public double? UserConnectionPctAsBaselinePerc
        {
            get { return userConnectionPctAsBaselinePerc; }
            internal set { userConnectionPctAsBaselinePerc = value; }
        }

        public double? ComputersHoldingProcessesBaselineMean
        {
            get { return computersHoldingProcessesBaselineMean; }
            internal set { computersHoldingProcessesBaselineMean = value; }
        }
        public double? ComputersHoldingProcessesAsBaselinePerc
        {
            get { return computersHoldingProcessesAsBaselinePerc; }
            internal set { computersHoldingProcessesAsBaselinePerc = value; }
        }

        public double? BlockedProcessesBaselineMean
        {
            get { return blockedProcessesBaselineMean; }
            internal set { blockedProcessesBaselineMean = value; }
        }
        public double? BlockedProcessesAsBaselinePerc
        {
            get { return blockedProcessesAsBaselinePerc; }
            internal set { blockedProcessesAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baseline

        /// <summary>
        /// Gets the count of distinct computers holding processes on the SQL Server.
        /// </summary>
        public int? ComputersHoldingProcesses
        {
            get { return computersHoldingProcesses; }
            internal set { computersHoldingProcesses = value; }
        }

        /// <summary>
        /// Gets the count of system processes running on the SQL Server.
        /// </summary>
        public int? CurrentSystemProcesses
        {
            get { return currentSystemProcesses; }
            internal set { currentSystemProcesses = value; }
        }

        /// <summary>
        /// Gets the count of user processes running on the SQL Server.
        /// </summary>
        public int? CurrentUserProcesses
        {
            get { return currentUserProcesses; }
            internal set { currentUserProcesses = value; }
        }

        /// <summary>
        /// The number of processes consuming CPU at the time of refresh
        /// </summary>
        public int? ProcessesConsumingCpu
        {
            get { return UserProcessesConsumingCpu + SystemProcessesConsumingCpu; }
        }

        /// <summary>
        /// Gets the count of processes consuming CPU on the SQL Server.
        /// </summary>
        public int? UserProcessesConsumingCpu
        {
            get { return userProcessesConsumingCpu; }
            internal set { userProcessesConsumingCpu = value; }
        }
        
        /// <summary>
        /// Gets the count of processes consuming CPU on the SQL Server.
        /// </summary>
        public int? SystemProcessesConsumingCpu
        {
            get { return systemProcessesConsumingCpu; }
            internal set { systemProcessesConsumingCpu = value; }
        }

        /// <summary>
        /// Gets the total count of processes on the CPU, idle or active, system or user
        /// </summary>
        public int? CurrentProcesses
        {
            get
            {
                return CurrentSystemProcesses +
                       CurrentUserProcesses;
            }
        }


        /// <summary>
        /// The number of blocked processes at the time of refresh
        /// </summary>
        public int? BlockedProcesses
        {
            get { return blockedProcesses; }
            internal set {blockedProcesses = value;}
        }

        /// <summary>
        /// The number of open transactions at the time of refresh
        /// </summary>
        public int? OpenTransactions
        {
            get { return openTransactions; }
            set{openTransactions = value;}
        }

        /// <summary>
        /// Number of lead blocking processes
        /// </summary>
        public int? LeadBlockers
        {
            get { return leadBlockers; }
            internal set { leadBlockers = value; }
        }

        /// <summary>
        /// Returns the total number of sessions on the system
        /// </summary>
        public int? TotalSessions
        {
            get
            {
                if (CurrentSystemProcesses.HasValue && CurrentUserProcesses.HasValue)
                {
                    return CurrentSystemProcesses + CurrentUserProcesses;
                }
                else
                {
                    if (CurrentSystemProcesses.HasValue) return currentSystemProcesses;
                    if (CurrentUserProcesses.HasValue) return CurrentUserProcesses;
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the number of sessions that are not sleeping
        /// </summary>
        public int? ActiveProcesses
        {
            get { return activeProcesses; }
            internal set { activeProcesses = value; }
        }

        #endregion

        #region methods

        ///// <summary>
        ///// Dumps the sample data to a string.
        ///// </summary>
        ///// <returns>A string representation of the sample data.</returns>
        //public string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();

        //    dump.Append("ComputersHoldingProcesses: " + ComputersHoldingProcesses); dump.Append("\n");
        //    dump.Append("CurrentSystemProcesses: " + CurrentSystemProcesses); dump.Append("\n");
        //    dump.Append("CurrentUserProcesses: " + CurrentUserProcesses); dump.Append("\n");
        //    dump.Append("ProcessesConsumingCpu: " + ProcessesConsumingCpu); dump.Append("\n");

        //    return dump.ToString();
        //}

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("computersHoldingProcesses", computersHoldingProcesses);
            info.AddValue("currentSystemProcesses", currentSystemProcesses);
            info.AddValue("currentUserProcesses", currentUserProcesses);
            info.AddValue("userProcessesConsumingCpu", userProcessesConsumingCpu);
            info.AddValue("systemProcessesConsumingCpu", systemProcessesConsumingCpu);
            info.AddValue("blockedProcesses", blockedProcesses);
            info.AddValue("openTransactions", openTransactions);
            info.AddValue("leadBlockers", leadBlockers);
            info.AddValue("activeProcesses", activeProcesses);
        }

        #endregion
    }
}
