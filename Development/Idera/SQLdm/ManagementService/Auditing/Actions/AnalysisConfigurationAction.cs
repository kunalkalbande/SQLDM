//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;

    /// <summary>
    /// Prepares the environment to log changes for Analysis actions
    /// </summary>
    class AnalysisConfigurationAction : IAuditAction
    {
        private AnalysisConfiguration newConfiguration;
        private AnalysisConfiguration oldConfiguration;

        private MonitoredSqlServerConfiguration newMonitoredSQLConfig;

        public AuditableActionType Type { get; set; }

        /// <summary>
        /// Recieves the new Analysis configuration and the old configuration
        /// </summary>
        /// <param name="newConfiguration"></param>
        /// <param name="oldConfiguration"></param>
        public AnalysisConfigurationAction(MonitoredSqlServerConfiguration newConfiguration, MonitoredSqlServerConfiguration oldConfiguration)
        {
            this.newMonitoredSQLConfig = newConfiguration;

            this.newConfiguration = newConfiguration.AnalysisConfiguration;
            this.oldConfiguration = oldConfiguration.AnalysisConfiguration;

            Type = AuditableActionType.AnalysisConfigurationChanged;
        }

        /// <summary>
        /// Recieves an MAuditableEntity which already has the default values like SQLUser, and then adds more flavors to it according to the case.
        /// </summary>
        /// <param name="entity"></param>
        public void FillEntity(ref MAuditableEntity entity)
        {
            List<PropertiesComparer.PropertiesData> propertiesChanged = null;

            bool logAction = HasChangedProperties(oldConfiguration, out propertiesChanged);
            if (!logAction)
            {
                entity = null;
                return;
            }

            entity.Name = this.newMonitoredSQLConfig.InstanceName;

            if (oldConfiguration != null)
            {
                foreach (PropertiesComparer.PropertiesData propertiesData in propertiesChanged)
                {
                    entity.AddMetadataProperty(propertiesData.Name, propertiesData.Value);
                }
            }
        }

        /// <summary>
        /// Try to get the changed properties in to a list. Returns true if has succeeded and false in other case.
        /// </summary>
        /// <param name="oldConfiguration">Old object</param>
        /// <param name="propertiesChanged">The changed properties.</param>
        /// <returns>Returns true if has succeeded and false in other case.</returns>
        private bool HasChangedProperties(AnalysisConfiguration oldConfiguration,
            out List<PropertiesComparer.PropertiesData> propertiesChanged)
        {
            if (oldConfiguration == null)
            {
                propertiesChanged = null;
                return false;
            }

            var comparer = new PropertiesComparer();
            // Get changed properties.
            propertiesChanged = comparer.GetNewProperties(oldConfiguration, this.newConfiguration);

            return propertiesChanged.Count > 0;
        }
    }
}
