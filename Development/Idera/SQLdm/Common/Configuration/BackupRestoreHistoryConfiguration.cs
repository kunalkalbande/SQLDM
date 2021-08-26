//------------------------------------------------------------------------------
// <copyright file="BackupRestoreHistoryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.ComponentModel;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for database backup and restore history on-demand probe
    /// </summary>
    [Serializable]
    public sealed class BackupRestoreHistoryConfiguration : OnDemandConfiguration, IUserFilter
    {
        #region constants

        private const bool DEFAULT_SHOWLOGICALFILENAMES = false;
        private const int DEFAULT_DAYSTOSHOW = 31;
        private const bool DEFAULT_SHOWBACKUPS = true;
        private const bool DEFAULT_SHOWRESTORES = true;

        #endregion

        #region fields

        private List<string> databaseNames = new List<string>();
        private bool showLogicalFileNames = DEFAULT_SHOWLOGICALFILENAMES;
        private int daysToShow = DEFAULT_DAYSTOSHOW;
        private bool showBackups = DEFAULT_SHOWBACKUPS;
        private bool showRestores = DEFAULT_SHOWRESTORES;

        #endregion

        #region constructors

        public BackupRestoreHistoryConfiguration(int monitoredServerId, List<string> databaseNames) : base(monitoredServerId)
        {
            this.databaseNames = databaseNames;
        }


        public BackupRestoreHistoryConfiguration(int monitoredServerId, List<string> databaseNames, int daysToShow) : base(monitoredServerId)
        {
            this.databaseNames = databaseNames;
            this.daysToShow = daysToShow;
        }

        public BackupRestoreHistoryConfiguration(int monitoredServerId, List<string> databaseNames, bool showLogicalFileNames) : base(monitoredServerId)
        {
            this.databaseNames = databaseNames;
            this.showLogicalFileNames = showLogicalFileNames;
        }


        public BackupRestoreHistoryConfiguration(int monitoredServerId, List<string> databaseNames, bool showLogicalFileNames, int daysToShow) : base(monitoredServerId)
        {
            this.databaseNames = databaseNames;
            this.showLogicalFileNames = showLogicalFileNames;
            this.daysToShow = daysToShow;
        }

        #endregion

        #region properties

        /// <summary>
        /// Return without collecting if the filters are set to prevent any data
        /// </summary>
        [Auditable(false)]
        new public bool ReadyForCollection
        {
            get
            {
                if (DatabaseNames.Count == 0 || (!ShowBackups && !ShowRestores))
                {
                 return false;
                }
                else
                {
                 return true;
                }
            }
        }

        /// <summary>
        /// List of databases to filter for
        /// This IS case sensitive
        /// </summary>
        [Browsable(false)]
        public List<string> DatabaseNames
        {
            get { return databaseNames; }
            set { databaseNames = value; }
        }

        /// <summary>
        /// Show Logical File Names for all backups
        /// </summary>
        [DisplayName("Show Logical File Names"), Category("Backups")]
        [Description("Show Logical File Names for all backups")]
        [DefaultValue(DEFAULT_SHOWLOGICALFILENAMES)]
        public bool ShowLogicalFileNames
        {
            get { return showLogicalFileNames; }
            set { showLogicalFileNames = value; }
        }

        /// <summary>
        /// Collect backups and restores for this many days
        /// </summary>
        [DisplayName("Days To Show"), Category("History")]
        [Description("Show Backup and Restore events for the number of days specified back from today")]
        [DefaultValue(DEFAULT_DAYSTOSHOW)]
        public int DaysToShow
        {
            get { return daysToShow; }
            set { daysToShow = value; }
        }

        /// <summary>
        /// Collect backups
        /// </summary>
        [DisplayName("Show Backups"), Category("Backups")]
        [Description("Show Backups for the selected dates")]
        [DefaultValue(DEFAULT_SHOWBACKUPS)]
        public bool ShowBackups
        {
            get { return showBackups; }
            set { showBackups = value; }
        }

        /// <summary>
        /// Collect restores
        /// </summary>
        [DisplayName("Show Restores"), Category("Restores")]
        [Description("Show Restores for the selected dates")]
        [DefaultValue(DEFAULT_SHOWRESTORES)]
        public bool ShowRestores
        {
            get { return showRestores; }
            set { showRestores = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #region IUserFilter Members

        public void ClearValues()
        {
            //showLogicalFileNames = DEFAULT_SHOWLOGICALFILENAMES;
            daysToShow = int.MaxValue;
            showBackups = true;
            showRestores = true;
        }

        public bool HasDefaultValues()
        {
            if (
                showLogicalFileNames != DEFAULT_SHOWLOGICALFILENAMES
                    || daysToShow != DEFAULT_DAYSTOSHOW
                    || showBackups != DEFAULT_SHOWBACKUPS
                    || showRestores != DEFAULT_SHOWRESTORES)
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
            if (daysToShow > 0
                    || !showBackups
                    || !showRestores
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ResetValues()
        {
            showLogicalFileNames = DEFAULT_SHOWLOGICALFILENAMES;
            daysToShow = DEFAULT_DAYSTOSHOW;
            showBackups = DEFAULT_SHOWBACKUPS;
            showRestores = DEFAULT_SHOWRESTORES;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is BackupRestoreHistoryConfiguration)
            {
                BackupRestoreHistoryConfiguration filter = (BackupRestoreHistoryConfiguration)selectionFilter;
                showLogicalFileNames = filter.ShowLogicalFileNames;
                daysToShow = filter.DaysToShow;
                showBackups = filter.ShowBackups;
                showRestores = filter.ShowRestores;
            }
        }

        public bool Validate(out string Message)
        {
            Message = String.Empty;
            return true;
        }

        #endregion

        #endregion

        #region nested types

        #endregion

    }
}
