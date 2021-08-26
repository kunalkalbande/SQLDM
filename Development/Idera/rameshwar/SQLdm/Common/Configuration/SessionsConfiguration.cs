//------------------------------------------------------------------------------
// <copyright file="SessionsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Snapshots;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Configuration
{
    /// <summary>
    /// Configuration object for sessions details on-demand probe
    /// </summary>
    [Serializable]
    public class SessionsConfiguration : OnDemandConfiguration, IUserFilter
    {
        #region constants

        private const bool DEFAULT_EXCLUDE_DM = true;
        private const bool DEFAULT_EXCLUDE_SYSTEM = true;
        private const bool DEFAULT_ACTIVE = false;
        private const bool DEFAULT_BLOCKED = false;
        private const bool DEFAULT_BLOCKINGBLOCKED = false;
        private const bool DEFAULT_BLOCKINGORBLOCKED = false;
        private const bool DEFAULT_LEADBLOCKERS = false;
        private const bool DEFAULT_CONSUMINGCPU = false;
        private const bool DEFAULT_LOCKING = false;
        private const bool DEFAULT_NONSHAREDLOCKING = false;
        private const bool DEFAULT_OPENTRANSACTIONS = false;
        // This is changed for the tempdb configuration
        protected const bool DEFAULT_TEMPDBAFFECTING = false;
        protected const string DEFAULT_FILTER = "";

        //These are the names that need to be used in messages
        private const string DISPLAYNAME_LEADBLOCKERS = "Lead Blockers only";
        private const string DISPLAYNAME_BLOCKED = "Blocked only";
        private const string DISPLAYNAME_BLOCKINGBLOCKED = "Blocking and Blocked only";
        private const string DISPLAYNAME_BLOCKINGORBLOCKED = "Blocking or Blocked only";

        private const string MESSAGE_MUTUALLYEXCLUSIVE = "The selections for '{0}' and '{1}' are mutually exclusive and will return no data. Please make different selections and try again.";

        #endregion

        #region fields

        private bool excludeDiagnosticManagerProcesses = DEFAULT_EXCLUDE_DM;
        private bool excludeSystemProcesses = DEFAULT_EXCLUDE_SYSTEM;
        private bool active = DEFAULT_ACTIVE;
        private bool blocked = DEFAULT_BLOCKED;
        private bool blockingBlocked = DEFAULT_BLOCKINGBLOCKED;
        private bool blockingOrBlocked = DEFAULT_BLOCKINGORBLOCKED;
        private bool leadBlockers = DEFAULT_LEADBLOCKERS;
        private bool consumingCpu = DEFAULT_CONSUMINGCPU;
        private bool locking = DEFAULT_LOCKING;
        private bool nonSharedLocking = DEFAULT_NONSHAREDLOCKING;
        private bool openTransactions = DEFAULT_OPENTRANSACTIONS;
        protected bool tempdbAffecting = DEFAULT_TEMPDBAFFECTING;
        private Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, Int64?>> previousValues = new Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, Int64?>>();
        private int inputBufferLimiter = 0;
        private string applicationExcludeFilter = DEFAULT_FILTER;
        private string applicationIncludeFilter = DEFAULT_FILTER;
        private string hostExcludeFilter = DEFAULT_FILTER;
        private string hostIncludeFilter = DEFAULT_FILTER;

        private string searchTerm = null;
        private bool topCpu = false;
        private bool topIo = false;
        private bool topMemory = false;
        private bool topWait = false;
        private int topLimit = 0;
        private int? spid = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initialize a new instance of the SessionsConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="excludeDiagnosticManagerProcesses">Exclude SQLdm processes from collection.</param>
        /// <param name="excludeSystemProcesses">Exclude system processes from collection.</param>
        /// <param name="active">Filter for active processes</param>
        /// <param name="blocked">Filter for blocked processes</param>
        /// <param name="blockingBlocked">Filter for blocking blocked processes</param>
        /// <param name="leadBlockers">Filter for lead blockers</param>
        /// <param name="consumingCpu">Filter for consuming cpu</param>
        /// <param name="locking">Filter for locking</param>
        /// <param name="nonSharedLocking">Filter for exclusive locking</param>
        /// <param name="openTransactions">Filter for open transactions</param>
        /// <param name="previousSnapshot">Previous snapshot for calculation purposes</param>
        public SessionsConfiguration(int monitoredServerId,
            bool excludeDiagnosticManagerProcesses,
            bool excludeSystemProcesses,
            bool active,
            bool blocked,
            bool blockingBlocked,
            bool leadBlockers,
            bool consumingCpu,
            bool locking,
            bool nonSharedLocking,
            bool openTransactions,
            SessionSnapshot previousSnapshot,
            string applicationExcludeFilter,
            string applicationIncludeFilter,
            string hostExcludeFilter,
            string hostIncludeFilter)
            : base(monitoredServerId)
        {
            this.excludeDiagnosticManagerProcesses = excludeDiagnosticManagerProcesses;
            this.excludeSystemProcesses = excludeSystemProcesses;
            this.active = active;
            this.blocked = blocked;
            this.blockingBlocked = blockingBlocked;
            this.leadBlockers = leadBlockers;
            this.consumingCpu = consumingCpu;
            this.locking = locking;
            this.nonSharedLocking = nonSharedLocking;
            this.openTransactions = openTransactions;
            this.applicationExcludeFilter = applicationExcludeFilter;
            this.applicationIncludeFilter = applicationIncludeFilter;
            this.hostExcludeFilter = hostExcludeFilter;
            this.hostIncludeFilter = hostIncludeFilter;

            if (previousSnapshot != null)
            {
                SetPreviousValues(previousSnapshot);
            }
        }

        /// <summary>
        /// Initialize a new instance of the SessionsConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="previousSnapshot">Previous snapshot for calculation purposes</param>
        public SessionsConfiguration(
            int monitoredServerId,
            SessionSnapshot previousSnapshot)
            : base(monitoredServerId)
        {
            if (previousSnapshot != null)
            {
                SetPreviousValues(previousSnapshot);
            }
        }

        #endregion

        #region properties

        [Auditable(false)]
        new public bool ReadyForCollection
        {
            get
            {
                string temp;
                return (Validate(out temp));
            }
        }

        /// <summary>
		/// Gets the string representing the filter by application exclusive
		/// </summary>
        [DisplayName("Exclude Application"), Category("Exclude Filters")]
        [Description("Specify the application name to exclude in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        [DefaultValue(DEFAULT_FILTER)]
        public string ApplicationExcludeFilter
        {
            get { return applicationExcludeFilter; }
            set { applicationExcludeFilter = value; }
        }

        /// <summary>
		/// Gets the string representing the filter by application inclusive
		/// </summary>
        [DisplayName("Include Application"), Category("Include Filters")]
        [Description("Specify the application name to include in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        [DefaultValue(DEFAULT_FILTER)]
        public string ApplicationIncludeFilter
        {
            get { return applicationIncludeFilter; }
            set { applicationIncludeFilter = value; }
        }

        /// <summary>
		/// Gets the string representing the filter by Host exclusive
		/// </summary>
        [DisplayName("Exclude Host"), Category("Exclude Filters")]
        [Description("Specify the host name to exclude in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        [DefaultValue(DEFAULT_FILTER)]
        public string HostExcludeFilter
        {
            get { return hostExcludeFilter; }
            set { hostExcludeFilter = value; }
        }

        /// <summary>
		/// Gets the string representing the filter by Host inclusive
		/// </summary>
        [DisplayName("Include Host"), Category("Include Filters")]
        [Description("Specify the host name to include in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        [DefaultValue(DEFAULT_FILTER)]
        public string HostIncludeFilter
        {
            get { return hostIncludeFilter; }
            set { hostIncludeFilter = value; }
        }

        /// <summary>
		/// Gets whether processes associated with SQLdm are excluded.
		/// </summary>
        [DisplayName("Exclude SQLdm"), Category("System")]
        [Description("Exclude all Sessions associated with SQL Diagnostic Manager")]
        [DefaultValue(DEFAULT_EXCLUDE_DM)]
        public bool ExcludeDiagnosticManagerProcesses
        {
            get { return excludeDiagnosticManagerProcesses; }
            set { excludeDiagnosticManagerProcesses = value; }
        }

        /// <summary>
		/// Gets whether system processes are excluded.
		/// </summary>
        [DisplayName("User Only"), Category("System")]
        [Description("Show only User Sessions and exclude all System Sessions")]
        [DefaultValue(DEFAULT_EXCLUDE_SYSTEM)]
        public bool ExcludeSystemProcesses
        {
            get { return excludeSystemProcesses; }
            set { excludeSystemProcesses = value; }
        }

        /// <summary>
        /// Collect active processes.
        /// </summary>
        [DisplayName("Active only"), Category("Resources")]
        [Description("Show only Sessions that are running or runnable")]
        [DefaultValue(DEFAULT_ACTIVE)]
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Collect only the blocked processes, which does not include 
        /// blocking processes unless they are themselves blocked.
        /// </summary>
        [DisplayName(DISPLAYNAME_BLOCKED), Category("Resources")]
        [Description("Show only Sessions that are blocked by another process")]
        [DefaultValue(DEFAULT_BLOCKED)]
        public bool Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        /// <summary>
        /// Collect only processes blocking other processes that are also blocked by a particular process. 
        /// </summary>
        [DisplayName(DISPLAYNAME_BLOCKINGBLOCKED), Category("Resources")]
        [Description("Show only Sessions that are both blocking other sessions and blocked by another session")]
        [DefaultValue(DEFAULT_BLOCKINGBLOCKED)]
        public bool BlockingBlocked
        {
            get { return blockingBlocked; }
            set { blockingBlocked = value; }
        }

        /// <summary>
        /// Collect only sessions that either blocking other sessions or blocked by another session. 
        /// </summary>
        [DisplayName(DISPLAYNAME_BLOCKINGORBLOCKED), Category("Resources")]
        [Description("Show only Sessions that are either blocking other sessions or blocked by another session")]
        [DefaultValue(DEFAULT_BLOCKINGORBLOCKED)]
        public bool BlockingOrBlocked
        {
            get { return blockingOrBlocked; }
            set { blockingOrBlocked = value; }
        }

        /// <summary>
        /// Collect only processes blocking other processes that are not themselves blocked.
        /// </summary>
        [DisplayName(DISPLAYNAME_LEADBLOCKERS), Category("Resources")]
        [Description("Show only Sessions that are at the head of a blocking chain")]
        [DefaultValue(DEFAULT_LEADBLOCKERS)]
        public bool LeadBlockers
        {
            get { return leadBlockers; }
            set { leadBlockers = value; }
        }

        /// <summary>
        /// Collect processes consuming CPU cycles.
        /// </summary>
        [DisplayName("Consuming CPU only"), Category("Resources")]
        [Description("Show only Sessions that are consuming CPU cycles")]
        [DefaultValue(DEFAULT_CONSUMINGCPU)]
        public bool ConsumingCpu
        {
            get { return consumingCpu; }
            set { consumingCpu = value; }
        }


        /// <summary>
        /// Collect processes holding locks.
        /// </summary>
        [DisplayName("Holding Locks only"), Category("Resources")]
        [Description("Show only Sessions that are holding locks")]
        [DefaultValue(DEFAULT_LOCKING)]
        public bool Locking
        {
            get { return locking; }
            set { locking = value; }
        }

        /// <summary>
        /// Collect processes holding exclusive locks.
        /// </summary>
        [DisplayName("Exclusive Locks only"), Category("Resources")]
        [Description("Show only Sessions that are holding exclusive locks")]
        [DefaultValue(DEFAULT_NONSHAREDLOCKING)]
        public bool NonSharedLocking
        {
            get { return nonSharedLocking; }
            set { nonSharedLocking = value; }
        }

        /// <summary>
        /// Collect processes with open transactions.
        /// </summary>
        [DisplayName("Open Transactions only"), Category("Resources")]
        [Description("Show only Sessions that have open transactions")]
        [DefaultValue(DEFAULT_OPENTRANSACTIONS)]
        public bool OpenTransactions
        {
            get { return openTransactions; }
            set { openTransactions = value; }
        }

        /// <summary>
        /// Collect processes affecting tempdb
        /// </summary>
        [DisplayName("Affecting tempdb only"), Category("Resources")]
        [Description("Show only Sessions that are affecting tempdb (2005+ only)")]
        [DefaultValue(DEFAULT_TEMPDBAFFECTING)]
        public bool TempdbAffecting
        {
            get { return tempdbAffecting; }
            set { tempdbAffecting = value; }
        }

        /// <summary>
        /// Collect processes with open transactions.
        /// </summary>
        [Browsable(false)]
        public string SearchTerm
        {
            get { return searchTerm; }
            set { searchTerm = value; }
        }

        [Browsable(false)]
        public bool TopCpu
        {
            get { return topCpu; }
            set { topCpu = value; }
        }

        [Browsable(false)]
        public bool TopIo
        {
            get { return topIo; }
            set { topIo = value; }
        }

        [Browsable(false)]
        public bool TopMemory
        {
            get { return topMemory; }
            set { topMemory = value; }
        }

        [Browsable(false)]
        public bool TopWait
        {
            get { return topWait; }
            set { topWait = value; }
        }

        [Browsable(false)]
        public int TopLimit
        {
            get { return topLimit; }
            set { topLimit = value; }
        }

        [Browsable(false)]
        public int? Spid
        {
            get { return spid; }
            set { spid = value; }
        }

        /// <summary>
        /// Represents the CPU time and Physical IO of the previous refresh
        /// </summary>
        [Browsable(false)]
        public Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, Int64?>> PreviousValues
        {
            get { return previousValues; }
            set { previousValues = value; }
        }

        [Browsable(false)]
        [Auditable("Changed limit of DBCC Input Buffer executions to")]
        public int InputBufferLimiter
        {
            get { return inputBufferLimiter; }
            set { inputBufferLimiter = value; }
        }

        #endregion

        #region methods

        /// <summary>
        /// To prevent having to copy the entire session list back to the collection service, just 
        /// extract the CPU and Physical IO which we are calculating deltas upon
        /// </summary>
        public void SetPreviousValues(SessionSnapshot previousSnapshot)
        {
            if (previousSnapshot == null || previousSnapshot.SessionList == null || previousSnapshot.SessionList.Count == 0)
                return;

            //Clear old values
            PreviousValues = new Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, long?>>();

            //Load only those metrics we care about from old list
            foreach (Session session in previousSnapshot.SessionList.Values)
            {
                if (session != null)
                    PreviousValues.Add(
                        session.InternalSessionIdentifier,
                        new Pair<TimeSpan, long?>(session.Cpu, session.PhysicalIo));
            }

        }

        #endregion

        #region IUserFilter Members

        public void ClearValues()
        {
            excludeDiagnosticManagerProcesses = true;
            excludeSystemProcesses = false;
            active = false;
            blocked = false;
            blockingBlocked = false;
            blockingOrBlocked = false;
            leadBlockers = false;
            consumingCpu = false;
            locking = false;
            nonSharedLocking = false;
            openTransactions = false;
            tempdbAffecting = false;
            applicationExcludeFilter = string.Empty;
            applicationIncludeFilter = string.Empty;
            hostExcludeFilter = string.Empty;
            hostIncludeFilter = string.Empty;
        }

        public bool HasDefaultValues()
        {
            if (excludeDiagnosticManagerProcesses != DEFAULT_EXCLUDE_DM
                    || excludeSystemProcesses != DEFAULT_EXCLUDE_SYSTEM
                    || active != DEFAULT_ACTIVE
                    || blocked != DEFAULT_BLOCKED
                    || blockingBlocked != DEFAULT_BLOCKINGBLOCKED
                    || blockingOrBlocked != DEFAULT_BLOCKINGORBLOCKED
                    || leadBlockers != DEFAULT_LEADBLOCKERS
                    || consumingCpu != DEFAULT_CONSUMINGCPU
                    || locking != DEFAULT_LOCKING
                    || nonSharedLocking != DEFAULT_NONSHAREDLOCKING
                    || openTransactions != DEFAULT_OPENTRANSACTIONS
                    || tempdbAffecting != DEFAULT_TEMPDBAFFECTING
                    || applicationExcludeFilter != DEFAULT_FILTER
                    || applicationIncludeFilter != DEFAULT_FILTER
                    || hostExcludeFilter != DEFAULT_FILTER
                    || hostIncludeFilter != DEFAULT_FILTER
                )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsFiltered()
        {
            return !HasDefaultValues();
        }

        public void ResetValues()
        {
            excludeDiagnosticManagerProcesses = DEFAULT_EXCLUDE_DM;
            excludeSystemProcesses = DEFAULT_EXCLUDE_SYSTEM;
            active = DEFAULT_ACTIVE;
            blocked = DEFAULT_BLOCKED;
            blockingBlocked = DEFAULT_BLOCKINGBLOCKED;
            blockingOrBlocked = DEFAULT_BLOCKINGORBLOCKED;
            leadBlockers = DEFAULT_LEADBLOCKERS;
            consumingCpu = DEFAULT_CONSUMINGCPU;
            locking = DEFAULT_LOCKING;
            nonSharedLocking = DEFAULT_NONSHAREDLOCKING;
            openTransactions = DEFAULT_OPENTRANSACTIONS;
            tempdbAffecting = DEFAULT_TEMPDBAFFECTING;
            applicationExcludeFilter = DEFAULT_FILTER;
            applicationIncludeFilter = DEFAULT_FILTER;
            hostExcludeFilter = DEFAULT_FILTER;
            hostIncludeFilter = DEFAULT_FILTER;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is SessionsConfiguration)
            {
                SessionsConfiguration filter = (SessionsConfiguration)selectionFilter;
                excludeDiagnosticManagerProcesses = filter.ExcludeDiagnosticManagerProcesses;
                excludeSystemProcesses = filter.ExcludeSystemProcesses;
                active = filter.Active;
                blocked = filter.Blocked;
                blockingBlocked = filter.BlockingBlocked;
                blockingOrBlocked = filter.BlockingOrBlocked;
                leadBlockers = filter.LeadBlockers;
                consumingCpu = filter.ConsumingCpu;
                locking = filter.Locking;
                nonSharedLocking = filter.NonSharedLocking;
                openTransactions = filter.OpenTransactions;
                tempdbAffecting = filter.tempdbAffecting;
                applicationExcludeFilter = filter.ApplicationExcludeFilter;
                applicationIncludeFilter = filter.ApplicationIncludeFilter;
                hostExcludeFilter = filter.HostExcludeFilter;
                hostIncludeFilter = filter.HostIncludeFilter;
            }
        }

        public bool Validate(out string Message)
        {
            if (LeadBlockers && Blocked)
            {
                Message = String.Format(MESSAGE_MUTUALLYEXCLUSIVE, DISPLAYNAME_LEADBLOCKERS, DISPLAYNAME_BLOCKED);
                return false;
            }
            else if (LeadBlockers && BlockingBlocked)
            {
                Message = String.Format(MESSAGE_MUTUALLYEXCLUSIVE, DISPLAYNAME_LEADBLOCKERS, DISPLAYNAME_BLOCKINGBLOCKED);
                return false;
            }
            else if (applicationExcludeFilter != string.Empty && applicationIncludeFilter != string.Empty)
            {
                Message = String.Format(MESSAGE_MUTUALLYEXCLUSIVE, "Application Exclude Filter", "Application Include Filter");
                return false;
            }
            else if (hostExcludeFilter != string.Empty && hostIncludeFilter != string.Empty)
            {
                Message = String.Format(MESSAGE_MUTUALLYEXCLUSIVE, "Host Exclude Filter", "Host Include Filter");
                return false;
            }
            else
            {
                Message = String.Empty;
                return true;
            }
            
        }

        #endregion
    }

    /// <summary>
    /// Configuration object for tempdb sessions details on-demand probe
    /// </summary>
    [Serializable]
    public class TempdbConfiguration : SessionsConfiguration
    {
        private new const bool DEFAULT_TEMPDBAFFECTING = true;

		#region constructors

        /// <summary>
        /// Initialize a new instance of the tempdbConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="excludeDiagnosticManagerProcesses">Exclude SQLdm processes from collection.</param>
        /// <param name="excludeSystemProcesses">Exclude system processes from collection.</param>
        /// <param name="active">Filter for active processes</param>
        /// <param name="blocked">Filter for blocked processes</param>
        /// <param name="blockingBlocked">Filter for blocking blocked processes</param>
        /// <param name="leadBlockers">Filter for lead blockers</param>
        /// <param name="consumingCpu">Filter for consuming cpu</param>
        /// <param name="locking">Filter for locking</param>
        /// <param name="nonSharedLocking">Filter for exclusive locking</param>
        /// <param name="openTransactions">Filter for open transactions</param>
        /// <param name="previousSnapshot">Previous snapshot for calculation purposes</param>
        public TempdbConfiguration(int monitoredServerId, 
            bool excludeDiagnosticManagerProcesses, 
            bool excludeSystemProcesses,
            bool active, 
            bool blocked, 
            bool blockingBlocked, 
            bool leadBlockers, 
            bool consumingCpu, 
            bool locking, 
            bool nonSharedLocking, 
            bool openTransactions,
            SessionSnapshot previousSnapshot)
            : base(monitoredServerId, 
                    excludeDiagnosticManagerProcesses, 
                    excludeSystemProcesses,
                    active, 
                    blocked, 
                    blockingBlocked, 
                    leadBlockers, 
                    consumingCpu, 
                    locking, 
                    nonSharedLocking, 
                    openTransactions,
                    previousSnapshot, string.Empty, string.Empty, string.Empty, string.Empty)
        {
            init();
        }

        /// <summary>
        /// Initialize a new instance of the tempdbConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="previousSnapshot">Previous snapshot for calculation purposes</param>
        public TempdbConfiguration(
            int monitoredServerId, 
            SessionSnapshot previousSnapshot)
            : base(monitoredServerId,
                    previousSnapshot)
        {
            init();
        }

        private void init()
        {
            tempdbAffecting = true;
            //DefaultValueAttribute[] attrs = (DefaultValueAttribute[])typeof(
        }

		#endregion

        /// <summary>
        /// Collect processes affecting tempdb
        /// </summary>
        [DefaultValue(DEFAULT_TEMPDBAFFECTING)]
        public new bool TempdbAffecting
        {
            get { return true; }
        }
    }

    /// <summary>
    /// Configuration object for SessionsConfiguration2000 details on-demand probe
    /// to avoid TempdbAffecting wich is 2005+ only
    /// </summary>
    [Serializable]
    public class SessionsConfiguration2000 : SessionsConfiguration
    {
        
        #region constructors

        /// <summary>
        /// Initialize a new instance of the tempdbConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="previousSnapshot">Previous snapshot for calculation purposes</param>
        public SessionsConfiguration2000(
            int monitoredServerId,
            SessionSnapshot previousSnapshot)
            : base(monitoredServerId,
                    previousSnapshot)
        {
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Collect processes affecting tempdb, mark as hidden for this class
        /// </summary>
        [DisplayName("Affecting tempdb only"), Category("Resources")]
        [Description("Show only Sessions that are affecting tempdb (2005+ only)")]
        [DefaultValue(DEFAULT_TEMPDBAFFECTING)]
        [Bindable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TempdbAffecting
        {
            get { return false; }
        }
        #endregion
    }

}


