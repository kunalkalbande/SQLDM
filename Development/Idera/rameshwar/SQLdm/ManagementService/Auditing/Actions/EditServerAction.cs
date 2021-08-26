using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class EditServerAction : IAuditAction
    {
        private readonly MonitoredSqlServerConfiguration _newConfiguration;
        private readonly MonitoredSqlServerConfiguration _oldConfiguration;
        private VirtualizationConfiguration _vmOldConfiguration;
        private bool _notLog;
        private bool _isServerProperties = false;
        private int _oldBlockedProcessThreshold;
        private int _newBlockedProcessThreshold;
        private const string OleText = "OleAutomationDisabled";
        private const string WmiText = "DirectWmiEnabled";

        private const string StartTimeRelativeText = "Enabled collection of query wait statistics at a specified time and duration";
        private const string EnabledXeText = "Enabled use of extended events";
        private const string RunTimeText = "Collect query wait statistics duration";

        public const string BlockedThresholdvalue = "Changed Blocked Process Threshold value to";

        public EditServerAction(MonitoredSqlServerConfiguration newConfiguration, MonitoredSqlServerConfiguration oldconfiguration)
        {
            _newConfiguration = newConfiguration;
            _oldConfiguration = oldconfiguration;
            _vmOldConfiguration = MAuditingEngine.Instance.PopAuxiliarData("VmConfigurationOldValue") as VirtualizationConfiguration;
            this.Type = AuditableActionType.ServerPropertiesChanged;
            var flag = MAuditingEngine.Instance.PopAuxiliarData("VmConfigurationFlag") as AuditAuxiliar<bool>;
            var serverGroupProperties = MAuditingEngine.Instance.PopAuxiliarData("ServerGroupPropertiesView") as AuditAuxiliar<bool>;

            var oldBlocked = MAuditingEngine.Instance.PopAuxiliarData("OldBlockedProcessThreshold") as AuditAuxiliar<int>;
            var newBlocked = MAuditingEngine.Instance.PopAuxiliarData("NewBlockedProcessThreshold") as AuditAuxiliar<int>;

            if (oldBlocked != null)
            {
                _oldBlockedProcessThreshold = oldBlocked.Data;
            }

            if (newBlocked != null)
            {
                _newBlockedProcessThreshold = newBlocked.Data;
            }

            if (flag == null)
            {
                _notLog = false;
            }
            else
            {
                _notLog = flag.Data;
            }

            _isServerProperties = serverGroupProperties != null;
        }

        public AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            if (!_notLog)
            {
                entity = null;
                return;
            }

            AuditableEntity serverEntity = GetPropertiesChange();
            entity.Name = serverEntity.Name;
            entity.MetadataProperties.Clear();
            entity.SetHeaderParam(serverEntity.Name);

            if (serverEntity.MetadataProperties.Count <= 0)
            {
                entity = null;
                return;
            }

            var osStatisticsList = new List<Pair<string, string>>();

            foreach (var property in serverEntity.MetadataProperties)
            {
                if (property.First == OleText || property.First == WmiText)
                {
                    osStatisticsList.Add(property);
                }
                else
                {
                    entity.AddMetadataProperty(property.First, property.Second);
                }
            }

            FillOSStatisticsConfiguration(osStatisticsList, ref entity);
        }

        private void FillOSStatisticsConfiguration(List<Pair<string, string>> osStatisticsList, ref MAuditableEntity entity)
        {
            bool disabledWmiOrOleCollection = false;
            foreach (Pair<string, string> pair in osStatisticsList)
            {
                string wmiStatusText = string.Format("{0} collection of Operating System metrics with direct WMI", pair.Second == "False" ? "Disabled" : "Enabled");
                string oleStatusText = string.Format("{0} collection of Operating System metrics with OLE automation", pair.Second == "True" ? "Disabled" : "Enabled");
                const string DisabledCollectionText = "Disabled collection of Operating System metrics";

                disabledWmiOrOleCollection = pair.First == WmiText && pair.Second == "False" ||
                                             pair.First == OleText && pair.Second == "True";

                if (osStatisticsList.Count == 1 && disabledWmiOrOleCollection)
                {
                    entity.AddMetadataProperty(DisabledCollectionText, string.Empty);
                }
                else
                {
                    if (pair.First == WmiText)
                    {
                        entity.AddMetadataProperty(wmiStatusText, string.Empty);
                    }
                    if (pair.First == OleText)
                    {
                        entity.AddMetadataProperty(oleStatusText, string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Audit log for changes any configuration in server property
        /// </summary>
        /// <param name="oldConfiguration">Old MonitoredSqlServerConfiguration</param>
        /// <param name="newConfiguration">New MonitoredSqlServerConfiguration</param>
        private AuditableEntity GetPropertiesChange()
        {
            AuditableEntity entityResult = _newConfiguration.GetAuditableEntity();

            //Compare the properties of MonitoredSqlServerConfiguration objects.
            _newConfiguration.CompareTo(_oldConfiguration, ref entityResult);

            var newvmConfiguration = _newConfiguration.VirtualizationConfiguration;
            var oldVmConfiguration = _vmOldConfiguration;

            if (newvmConfiguration == null && oldVmConfiguration != null)
            {
                //Log delete linked vcenter 
                
                AuditableEntity entity = oldVmConfiguration.GetAuditableEntity();
                string description = null;
                if (oldVmConfiguration.VCServerType == "HyperV")
                {
                    description = AuditTools.GetEnumDescription(AuditableActionType.ServerUnlinkedFromHyperV);
                }
                else if (oldVmConfiguration.VCServerType == "vCenter")
                {
                    description = AuditTools.GetEnumDescription(AuditableActionType.ServerUnlinkedFromvCenter);
                }
                entityResult.AddMetadataProperty(description, string.Empty);
                foreach (var vmPropertie in entity.MetadataProperties)
                {
                    entityResult.AddMetadataProperty(vmPropertie.First, vmPropertie.Second);
                }
            }
            else if (newvmConfiguration != null && oldVmConfiguration != null)
            {
                //Log linked vcenter change
                AuditableEntity entity = newvmConfiguration.GetAuditableEntity(oldVmConfiguration);
                if (entity.HasMetadataProperties())
                {
                    string description = null;
                    if (newvmConfiguration.VCServerType == "HyperV")
                    {
                        description =
                            AuditTools.GetEnumDescription(AuditableActionType.ServerLinkConfigurationToHyperVChanged);
                    }
                    else if (newvmConfiguration.VCServerType == "vCenter")
                    {
                        description =
                            AuditTools.GetEnumDescription(AuditableActionType.ServerLinkConfigurationTovCenterChanged);
                    }
                    entityResult.AddMetadataProperty(description, string.Empty);
                    foreach (var vmPropertie in entity.MetadataProperties)
                    {
                        entityResult.AddMetadataProperty(vmPropertie.First, vmPropertie.Second);
                    }
                }
            }

            if (_newConfiguration.GrowthStatisticsDays != _oldConfiguration.GrowthStatisticsDays)
            {
                var days = MonitoredSqlServer.GetDays(_newConfiguration.GrowthStatisticsDays);

                if (days.Count > 0)
                {
                    string valueDays = string.Empty;

                    foreach (var day in days)
                    {
                        valueDays += " " + day.ToString();
                    }

                    entityResult.AddMetadataProperty("Days to collect table statistics", valueDays);
                }
            }

            if (_newConfiguration.GrowthStatisticsStartTime != _oldConfiguration.GrowthStatisticsStartTime)
            {
                var date = _newConfiguration.GrowthStatisticsStartTime.Value;
                TimeSpan time = new TimeSpan(date.Hour, date.Minute, date.Second);
                entityResult.AddMetadataProperty("Reorganization Statistics Start Time", time.ToString());
            }

            List<int> newCounters = _newConfiguration.CustomCounters ?? new List<int>();
            List<int> oldCounters = _oldConfiguration.CustomCounters ?? new List<int>();

            var countersAdded = AuditTools.Except(newCounters, oldCounters);
            var countersRemoved = AuditTools.Except(oldCounters, newCounters);

            string countersEvent = "Custom Counter linked to Server";

            AddCounterNames(countersAdded, ref entityResult, countersEvent);

            countersEvent = "Custom Counter unlinked from Server";

            AddCounterNames(countersRemoved, ref entityResult, countersEvent);

            List<string> newExcludeTables = _newConfiguration.TableStatisticsExcludedDatabases ?? new List<string>();
            List<string> oldExcludeTables = _oldConfiguration.TableStatisticsExcludedDatabases ?? new List<string>();

            var excludeAdded = AuditTools.Except(newExcludeTables, oldExcludeTables);
            var excludeRemoved = AuditTools.Except(oldExcludeTables, newExcludeTables);

            AddExcludeTables(excludeAdded, ref entityResult, "Selected additional Databases to Exclude from Table Statistics");
            AddExcludeTables(excludeRemoved, ref entityResult, "Unselected Databases to collect Table Statistics");

            if (_oldBlockedProcessThreshold != _newBlockedProcessThreshold)
            {
                if (_newBlockedProcessThreshold >= 0)
                {
                    entityResult.AddMetadataProperty(BlockedThresholdvalue, _newBlockedProcessThreshold.ToString());
                }
            }

            if (_newConfiguration.DiskCollectionSettings.AutoDiscover == false && DiskDrivesChanged())
            {
                entityResult.AddMetadataProperty("Selected disk drivers", GetArrayAsString());
            }

            if (_newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration != null)
            {
                LogFiltersQueries(ref entityResult);
            }

            if (_oldConfiguration.PreferredClusterNode == null || string.IsNullOrEmpty(_newConfiguration.PreferredClusterNode))
            {
                string clusterNewValueName = _newConfiguration.PreferredClusterNode ?? String.Empty;
                string clusterOldValueName = _oldConfiguration.PreferredClusterNode ?? String.Empty;

                if (!clusterNewValueName.Equals(clusterOldValueName))
                {
                    if (string.IsNullOrEmpty(clusterNewValueName))
                    {
                        entityResult.RemovePropertyValueByKey("Set Preferred Cluster Node to");
                        clusterNewValueName = "None";
                    }

                    entityResult.AddMetadataProperty("Set Preferred Cluster Node to", clusterNewValueName);
                }
            }

            int newRowCount = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.Rowcount;
            if (newRowCount != _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.Rowcount)
            {
                if (newRowCount == 0)
                {
                    entityResult.AddMetadataProperty("Changed row count to", "Unlimited");
                }
                else
                {
                    entityResult.AddMetadataProperty("Changed row count to", newRowCount.ToString());
                }
            }

            if (_newConfiguration.QueryMonitorConfiguration.CpuUsageFilter != _oldConfiguration.QueryMonitorConfiguration.CpuUsageFilter)
            {
                string cpuUsageText = "Changed CPU Usage threshold to (milliseconds)";

                entityResult.RemovePropertyValueByKey(cpuUsageText);
                entityResult.AddMetadataProperty(cpuUsageText,
                                                 _newConfiguration.QueryMonitorConfiguration.
                                                     CpuUsageFilter.TotalMilliseconds.ToString());
            }

            if (_newConfiguration.QueryMonitorConfiguration.DurationFilter != _oldConfiguration.QueryMonitorConfiguration.DurationFilter)
            {
                string durationFilterText = "Changed Duration threshold to (milliseconds)";

                entityResult.RemovePropertyValueByKey(durationFilterText);
                entityResult.AddMetadataProperty(durationFilterText,
                                                 _newConfiguration.QueryMonitorConfiguration.
                                                     DurationFilter.TotalMilliseconds.ToString());
            }

            if (_newConfiguration.QueryMonitorConfiguration.TopPlanCountFilter != _oldConfiguration.QueryMonitorConfiguration.TopPlanCountFilter)
            {
                string topPlanCountFilterText = "Top Query Plan Count";

                entityResult.RemovePropertyValueByKey(topPlanCountFilterText);
                entityResult.AddMetadataProperty(
                    topPlanCountFilterText,
                    _newConfiguration.QueryMonitorConfiguration.TopPlanCountFilter.ToString());
            }

            if (_newConfiguration.QueryMonitorConfiguration.TopPlanCategoryFilter != _oldConfiguration.QueryMonitorConfiguration.TopPlanCategoryFilter)
            {
                string topPlanCategoryFilterText = "Top Query Plan Category";

                entityResult.RemovePropertyValueByKey(topPlanCategoryFilterText);
                entityResult.AddMetadataProperty(
                    topPlanCategoryFilterText,
                    _newConfiguration.QueryMonitorConfiguration.TopPlanCategoryFilter.ToString());
            }

            SetMaintenanceMode(_newConfiguration, _oldConfiguration, ref entityResult);
            CheckForActivityWaitsDefaults(_newConfiguration, _oldConfiguration, ref entityResult);

            return entityResult;
        }

        /// <summary>
        /// This method added into entityResult the audit information corresponding to maintenance mode
        /// </summary>
        /// <param name="newConfiguration"></param>
        /// <param name="oldConfiguration"></param>
        /// <param name="entityResult"></param>
        public void SetMaintenanceMode(MonitoredSqlServerConfiguration newConfiguration, MonitoredSqlServerConfiguration oldConfiguration,
            ref AuditableEntity entityResult)
        {
            AuditableEntity entity =
                newConfiguration.MaintenanceMode.GetAuditableEntity(oldConfiguration.MaintenanceMode);

            if (entity == null)
            {
                return;
            }

            foreach (var data in entity.MetadataProperties)
            {
                entityResult.AddMetadataProperty(data.First, data.Second);
            }

            if (entity.MetadataProperties.Count > 0)
            {
                entityResult.AddMetadataProperty("Affected Server", newConfiguration.InstanceName);
            }
        }

        private void CheckForActivityWaitsDefaults(MonitoredSqlServerConfiguration _newConfiguration, MonitoredSqlServerConfiguration _oldConfiguration, ref AuditableEntity entityResult)
        {
            bool defaultStartTimeRelative = _oldConfiguration.ActiveWaitsConfiguration.StartTimeRelative ==
                                            new DateTime();

            bool enabledLogTimeChanges = _newConfiguration.ActiveWaitsConfiguration.Enabled && !_newConfiguration.ActiveWaitsConfiguration.InCollectionWindow;

            entityResult.RemovePropertyValueByKey(StartTimeRelativeText);
            entityResult.RemovePropertyValueByKey(EnabledXeText);
            entityResult.RemovePropertyValueByKey(RunTimeText);

            if (!defaultStartTimeRelative)
            {
                if (_newConfiguration.ActiveWaitsConfiguration.EnabledXe != _oldConfiguration.ActiveWaitsConfiguration.EnabledXe)
                {
                    entityResult.AddMetadataProperty(
                          EnabledXeText,
                          _newConfiguration.ActiveWaitsConfiguration.EnabledXe.ToString());
                }

                if (enabledLogTimeChanges)
                {
                    if (_newConfiguration.ActiveWaitsConfiguration.StartTimeRelative.ToString() != _oldConfiguration.ActiveWaitsConfiguration.StartTimeRelative.ToString())
                    {
                        entityResult.AddMetadataProperty(
                           StartTimeRelativeText,
                           _newConfiguration.ActiveWaitsConfiguration.StartTimeRelative.ToString());
                    }

                    if (_newConfiguration.ActiveWaitsConfiguration.RunTime.ToString() != _oldConfiguration.ActiveWaitsConfiguration.RunTime.ToString())
                    {
                        entityResult.AddMetadataProperty(
                           RunTimeText,
                           _newConfiguration.ActiveWaitsConfiguration.RunTime.ToString());
                    }
                }
            }
        }

        private void LogFiltersQueries(ref AuditableEntity entityResult)
        {
            string newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.ApplicationExcludeToString() ?? String.Empty;
            string oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.ApplicationExcludeToString() ?? String.Empty;
            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("Application excluded from activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("Application excluded from activity monitoring", "None");
                }

            }

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
            newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.ApplicationIncludeToString() ?? String.Empty;
            oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.ApplicationIncludeToString() ?? String.Empty;
            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("Application included for activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("Application included for activity monitoring", "None");
                }

            }
            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters


            newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.DatabaseExcludeToString() ?? String.Empty;
            oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.DatabaseExcludeToString() ?? String.Empty;

            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("Database excluded from activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("Database excluded from activity monitoring", "None");
                }

            }

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
            newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.DatabaseIncludeToString() ?? String.Empty;
            oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.DatabaseIncludeToString() ?? String.Empty;

            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("Database included for activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("Database included for activity monitoring", "None");
                }

            }
            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters


            newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.SqlTextExcludeToString() ?? String.Empty;
            oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.SqlTextExcludeToString() ?? String.Empty;

            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("SQL Text excluded from activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("SQL Text excluded from activity monitoring", "None");
                }

            }

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
            newConfiguration = _newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.SqlTextIncludeToString() ?? String.Empty;
            oldConfiguration = _oldConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration.SqlTextIncludeToString() ?? String.Empty;

            if (newConfiguration != oldConfiguration)
            {
                if (!newConfiguration.Equals(String.Empty))
                {
                    entityResult.AddMetadataProperty("SQL Text included for activity monitoring", newConfiguration);
                }
                else
                {
                    entityResult.AddMetadataProperty("SQL Text included for activity monitoring", "None");
                }

            }
            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

        }

        private string GetArrayAsString()
        {
            if (_newConfiguration.DiskCollectionSettings.Drives.Length == 0)
                return "None";

            var builder = new StringBuilder();
            foreach (string drive in _newConfiguration.DiskCollectionSettings.Drives)
            {
                builder.AppendFormat("{0}{1} ", drive, ",");
            }

            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }

        private bool DiskDrivesChanged()
        {
            int newDrives = _newConfiguration.DiskCollectionSettings.Drives.Length;
            int oldDrives = _oldConfiguration.DiskCollectionSettings.Drives.Length;

            if (newDrives != oldDrives)
            {
                return true;
            }

            if (newDrives == 0)
            {
                return false;
            }

            bool found = false;

            foreach (string newDrive in _newConfiguration.DiskCollectionSettings.Drives)
            {
                found = false;
                foreach (string oldDrive in _oldConfiguration.DiskCollectionSettings.Drives)
                {
                    if (oldDrive == newDrive)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    break;
                }
            }

            return !found;
        }

        private void AddCounterNames(List<int> countersAdded, ref AuditableEntity entityResult, string countersEvent)
        {
            var metrics = new MetricDefinitions(true, false, true);
            metrics.Load(ManagementServiceConfiguration.ConnectionString, true);

            if (countersAdded.Count <= 0)
            {
                return;
            }
            entityResult.AddMetadataProperty(countersEvent, string.Empty);

            foreach (var tagId in countersAdded)
            {
                MetricDescription? definition = metrics.GetMetricDescription(tagId);

                if (definition != null)
                {
                    entityResult.AddMetadataProperty("Custom Counter Name", definition.Value.Name);
                }
            }
        }

        private void AddExcludeTables(List<string> excludedTables, ref AuditableEntity entityResult, string actionName)
        {
            if (excludedTables.Count <= 0)
            {
                return;
            }

            entityResult.AddMetadataProperty(actionName, string.Empty);

            foreach (var name in excludedTables)
            {
                entityResult.AddMetadataProperty("Database Name", name);
            }
        }

        private string FilterConfigurations(IEnumerable<string> filterArray)
        {
            var builder = new StringBuilder();
            foreach (string item in filterArray)
            {
                builder.AppendFormat("{0}{1}", item, ",");
            }

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
    }
}
