using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Idera.SQLdm.DesktopClient.Objects;
using Wintellect.PowerCollections;
using BlockingType = Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions.SessionsBlockingView.BlockingType;
using System.ComponentModel;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using System.Xml.Serialization;
using System.IO;

namespace Idera.SQLdm.DesktopClient.Properties 
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings 
    {
        public EventHandler ActiveRepositoryConnectionChanging;
        public EventHandler ActiveRepositoryConnectionChanged;

        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        /// <summary>
        /// Dummy property to satisfy the ReportDataset controls need for a connection string setting.
        /// </summary>
        internal String MALVM2003_SQLdmRepository_ConnectionString
        {
            get { return "Data Source=.;Initial Catalog=SQLdmDatabase;Integrated Security=True"; }
        }

        /// <summary>
        /// The GetDeviceCaps function retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hDC">[in] Handle to the device context.</param>
        /// <param name="nIndex">[in] Specifies the item to return.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        /// <summary>
        /// Returns the OS current DPI value
        /// </summary>
        /// <returns></returns>
        private static short GetOSCurrentDPI()
        {
            // IntPtr.Zero is a handle to the current user session's desktop
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);

            // Gets the handle to the device context associated with this Graphics.
            IntPtr desktop = g.GetHdc();

            // 88 refers to the DPI settings
            return (short)GetDeviceCaps(desktop, 88);
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public List<RepositoryConnection> RepositoryConnections
        {
            get
            {
                if (this["RepositoryConnections"] == null)
                {
                    this["RepositoryConnections"] = new List<RepositoryConnection>();
                }

                return ((List<RepositoryConnection>)(this["RepositoryConnections"]));
            }
        }

        public RepositoryConnection ActiveRepositoryConnection
        {
            get
            {
                if (RepositoryConnections == null || RepositoryConnections.Count == 0)
                {
                    return null;
                }
                else
                {
                    // Since the RepositoryConnectionTimeoutInSeconds is an application level setting,
                    // it needs to be applied to previously saved connections
                    RepositoryConnection activeConnection = RepositoryConnections[RepositoryConnections.Count - 1];
                    activeConnection.ConnectionInfo.ConnectionTimeout = RepositoryConnectionTimeoutInSeconds;
                    activeConnection.ConnectionInfo.AllowAsynchronousCommands = false;
                    return activeConnection;
                }
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public List<int> RecentServers
        {
            get
            {
                if (this["RecentServers"] == null)
                {
                    this["RecentServers"] = new List<int>();
                }

                return ((List<int>)(this["RecentServers"]));
            }
        }

        public void AddRecentServer(int serverId)
        {
            if (RecentServers.Contains(serverId))
            {
                RecentServers.Remove(serverId);
            }

            RecentServers.Insert(0, serverId);

            if (RecentServers.Count > 5)
            {
                RecentServers.RemoveRange(5, RecentServers.Count - 5);
            }
        }

        public void SyncRecentServers()
        {
            List<int> missingInstances = new List<int>();

            foreach (int instanceId in RecentServers)
            {
                if (!ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                {
                    missingInstances.Add(instanceId);
                }
            }

            foreach (int instanceId in missingInstances)
            {
                RecentServers.Remove(instanceId);
            }
        }

        #region Custom View Object Types

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings AlertsViewMainGrid
        {
            get
            {
                if (this["AlertsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["AlertsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["AlertsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesBackupsViewMainGrid
        {
            get
            {
                if (this["DatabasesBackupsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesBackupsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesBackupsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesBackupsViewHistoryGrid
        {
            get
            {
                if (this["DatabasesBackupsViewHistoryGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesBackupsViewHistoryGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesBackupsViewHistoryGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesConfigurationViewMainGrid
        {
            get
            {
                if (this["DatabasesConfigurationViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesConfigurationViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesConfigurationViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesFilesViewMainGrid
        {
            get
            {
                if (this["DatabasesFilesViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesFilesViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesFilesViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesFilesViewFilesGrid
        {
            get
            {
                if (this["DatabasesFilesViewFilesGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesFilesViewFilesGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesFilesViewFilesGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesFilesViewGroupsGrid
        {
            get
            {
                if (this["DatabasesFilesViewGroupsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesFilesViewGroupsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesFilesViewGroupsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesFilesViewLogsGrid
        {
            get
            {
                if (this["DatabasesFilesViewLogsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesFilesViewLogsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesFilesViewLogsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesSummaryViewMainGrid
        {
            get
            {
                if (this["DatabasesSummaryViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesSummaryViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesSummaryViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesTablesViewMainGrid
        {
            get
            {
                if (this["DatabasesTablesViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesTablesViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesTablesViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesTablesViewDependenciesGrid
        {
            get
            {
                if (this["DatabasesTablesViewDependenciesGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesTablesViewDependenciesGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesTablesViewDependenciesGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesTablesViewIndexesGrid
        {
            get
            {
                if (this["DatabasesTablesViewIndexesGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesTablesViewIndexesGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesTablesViewIndexesGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesTablesViewColumnStatisticsGrid
        {
            get
            {
                if (this["DatabasesTablesViewColumnStatisticsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesTablesViewColumnStatisticsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesTablesViewColumnStatisticsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabasesTablesViewDataDistributionGrid
        {
            get
            {
                if (this["DatabasesTablesViewDataDistributionGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabasesTablesViewDataDistributionGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabasesTablesViewDataDistributionGrid"] = value; }
        }
        
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings MirroredDatabasesViewMirroredDatabasesGrid
        {
            get
            {
                if (this["MirroredDatabasesViewMirroredDatabasesGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["MirroredDatabasesViewMirroredDatabasesGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["MirroredDatabasesViewMirroredDatabasesGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings MirroredDatabasesViewHistoryGrid
        {
            get
            {
                if (this["MirroredDatabasesViewHistoryGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["MirroredDatabasesViewHistoryGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["MirroredDatabasesViewHistoryGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabaseAlwaysOnAvailabilityGroupsGrid
        {
            get
            {
                if (this["DatabaseAlwaysOnAvailabilityGroupsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabaseAlwaysOnAvailabilityGroupsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabaseAlwaysOnAvailabilityGroupsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings DatabaseAlwaysOnStatisticsGrid
        {
            get
            {
                if (this["DatabaseAlwaysOnStatisticsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["DatabaseAlwaysOnStatisticsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["DatabaseAlwaysOnStatisticsGrid"] = value; }
        }
        
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings LogsViewMainGrid
        {
            get
            {
                if (this["LogsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["LogsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["LogsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewHistoryModeChart1
        {
            get
            {
                if (this["QueryMonitorViewHistoryModeChart1"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewHistoryModeChart1"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewHistoryModeChart1"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewHistoryModeChart2
        {
            get
            {
                if (this["QueryMonitorViewHistoryModeChart2"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewHistoryModeChart2"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewHistoryModeChart2"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings QueryMonitorViewHistoryModeGrid
        {
            get
            {
                if (this["QueryMonitorViewHistoryModeGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["QueryMonitorViewHistoryModeGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewHistoryModeGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewSignatureModeChart1
        {
            get
            {
                if (this["QueryMonitorViewSignatureModeChart1"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewSignatureModeChart1"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewSignatureModeChart1"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewSignatureModeChart2
        {
            get
            {
                if (this["QueryMonitorViewSignatureModeChart2"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewSignatureModeChart2"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewSignatureModeChart2"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings QueryMonitorViewSignatureModeGrid
        {
            get
            {
                if (this["QueryMonitorViewSignatureModeGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["QueryMonitorViewSignatureModeGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewSignatureModeGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewStatementModeChart1
        {
            get
            {
                if (this["QueryMonitorViewStatementModeChart1"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewStatementModeChart1"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewStatementModeChart1"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public ChartSettings QueryMonitorViewStatementModeChart2
        {
            get
            {
                if (this["QueryMonitorViewStatementModeChart2"] is ChartSettings)
                {
                    return ((ChartSettings)(this["QueryMonitorViewStatementModeChart2"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewStatementModeChart2"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings QueryMonitorViewStatementModeGrid
        {
            get
            {
                if (this["QueryMonitorViewStatementModeGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["QueryMonitorViewStatementModeGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["QueryMonitorViewStatementModeGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ResourcesProcedureCacheViewMainGrid
        {
            get
            {
                if (this["ResourcesProcedureCacheViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ResourcesProcedureCacheViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ResourcesProcedureCacheViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServerConfigurationViewMainGrid
        {
            get
            {
                if (this["ServerConfigurationViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServerConfigurationViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServerConfigurationViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServerDetailsViewMainGrid
        {
            get
            {
                if (this["ServerDetailsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServerDetailsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServerDetailsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServerGroupDetailsViewMainGrid
        {
            get
            {
                if (this["ServerGroupDetailsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServerGroupDetailsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServerGroupDetailsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServerGroupPropertiesViewMainGrid
        {
            get
            {
                if (this["ServerGroupPropertiesViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServerGroupPropertiesViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServerGroupPropertiesViewMainGrid"] = value; }
        }


        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesAgentJobsViewMainGrid
        {
            get
            {
                if (this["ServicesAgentJobsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesAgentJobsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesAgentJobsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesAgentJobsViewHistoryGrid
        {
            get
            {
                if (this["ServicesAgentJobsViewHistoryGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesAgentJobsViewHistoryGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesAgentJobsViewHistoryGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesFullTextViewMainGrid
        {
            get
            {
                if (this["ServicesFullTextViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesFullTextViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesFullTextViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesFullTextViewTablesGrid
        {
            get
            {
                if (this["ServicesFullTextViewTablesGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesFullTextViewTablesGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesFullTextViewTablesGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesFullTextViewColumnsGrid
        {
            get
            {
                if (this["ServicesFullTextViewColumnsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesFullTextViewColumnsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesFullTextViewColumnsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesReplicationViewTopologyGrid
        {
            get
            {
                if (this["ServicesReplicationViewTopologyGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesReplicationViewTopologyGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesReplicationViewTopologyGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesReplicationViewDistributorGrid
        {
            get
            {
                if (this["ServicesReplicationViewDistributorGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesReplicationViewDistributorGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesReplicationViewDistributorGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ServicesSummaryViewMainGrid
        {
            get
            {
                if (this["ServicesSummaryViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ServicesSummaryViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ServicesSummaryViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings SessionTraceDialogMainGrid
        {
            get
            {
                if (this["SessionTraceDialogMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["SessionTraceDialogMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["SessionTraceDialogMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings SessionsDetailsViewMainGrid
        {
            get
            {
                if (this["SessionsDetailsViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["SessionsDetailsViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["SessionsDetailsViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings SessionsLocksViewMainGrid
        {
            get
            {
                if (this["SessionsLocksViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["SessionsLocksViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["SessionsLocksViewMainGrid"] = value; }
        }

		//START SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - field to store grid setting
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings ResourcesDiskSizeViewMainGrid
        {
            get
            {
                if (this["ResourcesDiskSizeViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["ResourcesDiskSizeViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["ResourcesDiskSizeViewMainGrid"] = value; }
        }
		//END  SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - field to store grid setting
		
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public BlockingType SessionsBlockingViewBlockingType
        {
            get
            {
                if (this["SessionsBlockingViewBlockingType"] is BlockingType)
                {
                    return ((BlockingType)(this["SessionsBlockingViewBlockingType"]));
                }
                else
                {
                    return BlockingType.Sessions;
                }
            }
            set { this["SessionsBlockingViewBlockingType"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings TasksViewMainGrid
        {
            get
            {
                if (this["TasksViewMainGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["TasksViewMainGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["TasksViewMainGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings TodayViewAlertsGrid
        {
            get
            {
                if (this["TodayViewAlertsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["TodayViewAlertsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["TodayViewAlertsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings NewTodayViewAlertsGrid
        {
            get
            {
                if (this["NewTodayViewAlertsGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["NewTodayViewAlertsGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["NewTodayViewAlertsGrid"] = value; }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public GridSettings TodayViewTasksGrid
        {
            get
            {
                if (this["TodayViewTasksGrid"] is GridSettings)
                {
                    return ((GridSettings)(this["TodayViewTasksGrid"]));
                }
                else
                {
                    return null;
                }
            }
            set { this["TodayViewTasksGrid"] = value; }
        }

        //public bool EnableBaseline { get; internal set; }

        #endregion

        private void OnActiveRepositoryConnectionChanging(EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (ActiveRepositoryConnectionChanging != null)
            {
                ActiveRepositoryConnectionChanging(this, e);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by OnActiveRepositoryConnectionChanging : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void OnActiveRepositoryConnectionChanged(EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (ActiveRepositoryConnectionChanged != null)
            {
                ActiveRepositoryConnectionChanged(this, e);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by OnActiveRepositoryConnectionChanged : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void SetActiveRepositoryConnection(RepositoryConnection connection)
        {
            SetActiveRepositoryConnection(connection, false);
        }

        public void SetActiveRepositoryConnection(RepositoryConnection connection, bool bypassInitialization)
        {
            SetActiveRepositoryConnection(connection, bypassInitialization, true);
        }

        public void SetActiveRepositoryConnection(RepositoryConnection connection, bool bypassInitialization, bool fireEvents)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (fireEvents)
                OnActiveRepositoryConnectionChanging(EventArgs.Empty);
            
            Default.RepositoryConnections.Remove(connection);
            Default.RepositoryConnections.Add(connection);

            if (!bypassInitialization)
            {
                //SqlDm 10.2 (Tushar)--Adding background worker for initializing application model after active repository connection has been successfully changed.
                BackgroundWorker applicationModelInitializer = new BackgroundWorker();
                applicationModelInitializer.DoWork += ApplicationModelInitializer_DoWork;
                applicationModelInitializer.RunWorkerCompleted += ApplicationModelInitializer_RunWorkerCompleted;
                applicationModelInitializer.RunWorkerAsync();
                //End 10.2 (Tushar)--Adding background worker for initializing application model after active repository connection has been successfully changed.
            }

            Save();
            
            if (fireEvents)
                OnActiveRepositoryConnectionChanged(EventArgs.Empty);
        }

        //Start: SqlDm 10.2 (Tushar)--Adding background worker for initializing application model after active repository connection has been successfully changed.
        private void ApplicationModelInitializer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void ApplicationModelInitializer_DoWork(object sender, DoWorkEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            ApplicationModel.Default.Initialize();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken  by ApplicationModelInitializer_DoWork : {0}", stopWatch.ElapsedMilliseconds);
        }
        //End: SqlDm 10.2 (Tushar)--Adding background worker for initializing application model after active repository connection has been successfully changed.

        public void DeleteRepositoryConnection(string serverName, string databaseName)
        {
            RepositoryConnection connection = FindRepositoryConnection(serverName, databaseName);
            Default.RepositoryConnections.Remove(connection);
        }

        public void RefreshRepositoryConnection()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var activeConnection = ActiveRepositoryConnection;
            if (activeConnection == null) return;
            OnActiveRepositoryConnectionChanging(EventArgs.Empty);
            ApplicationModel.Default.Initialize();
            OnActiveRepositoryConnectionChanged(EventArgs.Empty);
            StartUpTimeLog.DebugFormat("Time taken by Settings.RefreshRepositoryConnection : {0}", stopWatch.ElapsedMilliseconds);
        }

        public ReadOnlyCollection<RepositoryConnection> FindRepositoryConnections(string serverName)
        {
            List<RepositoryConnection> matchingConnections = new List<RepositoryConnection>();

            foreach (RepositoryConnection connection in Settings.Default.RepositoryConnections)
            {
                if (String.Compare(connection.ServerName, serverName, true) == 0)
                {
                    matchingConnections.Add(connection);
                }
            }

            return matchingConnections.AsReadOnly();
        }

        public RepositoryConnection FindRepositoryConnection(string serverName, string databaseName)
        {
            if (Default.RepositoryConnections != null)
            {
                foreach (RepositoryConnection connection in Default.RepositoryConnections)
                {
                    // SQL Server names are always case-insensitive
                    if (string.Compare(connection.ServerName, serverName, true) == 0)
                    {
                        // Database names are compared case-sensitive since the SQL Server host may be
                        // case-sensitive
                        if (string.CompareOrdinal(connection.DatabaseName, databaseName) == 0)
                        {
                            return connection;
                        }
                    }
                }
            }

            return null;
        }

        public bool RepositoryConnectionExists(string serverName, string databaseName)
        {
            return FindRepositoryConnection(serverName, databaseName) != null;
        }

        public override void Save()
        {
            List<Pair<RepositoryConnection, string>> protectedPasswords = new List<Pair<RepositoryConnection, string>>();

            // Clear protected passwords
            if (RepositoryConnections != null)
            {
                foreach (RepositoryConnection connection in RepositoryConnections)
                {
                    if (!connection.SavePassword)
                    {
                        protectedPasswords.Add(
                            new Pair<RepositoryConnection, string>(connection, connection.ConnectionInfo.Password));
                        connection.ConnectionInfo.Password = null;
                    }
                }
            }

            // Save the current settings
            base.Save();

            // Reset the protected passwords in case they need to be used again in the current client session
            foreach (Pair<RepositoryConnection, string> protectedPassword in protectedPasswords)
            {
                protectedPassword.First.ConnectionInfo.Password = protectedPassword.Second;
            }
        }
    }
}
