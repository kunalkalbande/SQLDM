//------------------------------------------------------------------------------
// <copyright file="MaintenanceModeConfigurationInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using Idera.SQLdm.Common.Objects;

    public class MaintenanceModeConfigurationInfo : ICloneable 
    {
        private MaintenanceMode configuration;

        public MaintenanceModeConfigurationInfo()
        {
            configuration = new MaintenanceMode();
        }

        public MaintenanceModeConfigurationInfo(MaintenanceMode configuration)
        {
            this.configuration = configuration;
        }

        public MaintenanceModeConfigurationInfo(MaintenanceModeConfigurationInfo copy)
        {
            MaintenanceMode copymm = copy.GetInternalConfiguration();
            configuration = new MaintenanceMode();
            configuration.MaintenanceModeDays = copymm.MaintenanceModeDays;
            configuration.MaintenanceModeDuration = copymm.MaintenanceModeDuration;
            configuration.MaintenanceModeRecurringStart = copymm.MaintenanceModeRecurringStart;
            configuration.MaintenanceModeStart = copymm.MaintenanceModeStart;
            configuration.MaintenanceModeStop = copymm.MaintenanceModeStop;
            configuration.MaintenanceModeType = copymm.MaintenanceModeType;
        }

        internal MaintenanceMode GetInternalConfiguration()
        {
            return configuration;
        }

        public MaintenanceModeType MaintenanceModeType
        {
            get { return configuration.MaintenanceModeType; }
            set { configuration.MaintenanceModeType = value; }
        }

        public DateTime? MaintenanceModeStart
        {
            get { return configuration.MaintenanceModeStart; }
            set { configuration.MaintenanceModeStart = value; }
        }

        public TimeSpan MaintenanceModeRecurringStart
        {
            get { return configuration.MaintenanceModeRecurringStart == null ? TimeSpan.Zero : configuration.MaintenanceModeRecurringStart.Value.TimeOfDay; }
            set
            {
                configuration.MaintenanceModeRecurringStart = SQLdmProvider.Jan_1_1900 + value;
            }
        }

        public DateTime? MaintenanceModeStop
        {
            get { return configuration.MaintenanceModeStop; }
            set { configuration.MaintenanceModeStop = value; }
        }

        public TimeSpan? MaintenanceModeDuration
        {
            get { return configuration.MaintenanceModeDuration; }
            set { configuration.MaintenanceModeDuration = value; }
        }

        public Days MaintenanceModeDays
        {
            get { return (Days)(configuration.MaintenanceModeDays ?? 0); }
            set { configuration.MaintenanceModeDays = (short)value; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return new MaintenanceModeConfigurationInfo(this);
        }

        #endregion
    }
}
