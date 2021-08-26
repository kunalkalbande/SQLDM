using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Presenters
{
    class QuietTimeValidate:IValidate
    {

        private bool MModeTimeEqualsQuietTime(MonitoredSqlServerConfiguration configuration)
        {
            bool mmodeTimeQuietTimeEqual = false;
            DateTime? mModeStart = null;
            DateTime? mModeStop = configuration.MaintenanceMode.MaintenanceModeStop;
            DateTime? quietTimeStart = configuration.GrowthStatisticsStartTime;
            DateTime? quietTimeStop = quietTimeStart + TimeSpan.FromHours(2);

            switch (configuration.MaintenanceMode.MaintenanceModeType)
            {
                case MaintenanceModeType.Always:
                    {
                        mmodeTimeQuietTimeEqual = true;
                        break;
                    }
                case MaintenanceModeType.Once:
                    {
                        mModeStart = configuration.MaintenanceMode.MaintenanceModeStart;

                        if ((mModeStop - mModeStart).Value.TotalDays <= 1)
                        {
                            if (((mModeStart.Value.TimeOfDay >= quietTimeStart.Value.TimeOfDay) && (mModeStart.Value.TimeOfDay < quietTimeStop.Value.TimeOfDay)) ||
                                ((mModeStop.Value.TimeOfDay <= quietTimeStop.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay > quietTimeStart.Value.TimeOfDay)) ||
                                ((mModeStart.Value.TimeOfDay <= quietTimeStart.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay >= quietTimeStop.Value.TimeOfDay)))
                            {
                                mmodeTimeQuietTimeEqual = true;
                            }
                        }
                        else
                        {
                            // the maintenance mode length is multiple days.
                            mmodeTimeQuietTimeEqual = true;
                        }
                        break;
                    }
                case MaintenanceModeType.Recurring:
                    {
                        mModeStart = configuration.MaintenanceMode.MaintenanceModeRecurringStart;
                        mModeStop = mModeStart + configuration.MaintenanceMode.MaintenanceModeDuration;

                        if ((configuration.MaintenanceMode.MaintenanceModeDays & configuration.GrowthStatisticsDays) != 0)
                        {
                            if (((mModeStart.Value.TimeOfDay >= quietTimeStart.Value.TimeOfDay) && (mModeStart.Value.TimeOfDay < quietTimeStop.Value.TimeOfDay)) ||
                                ((mModeStop.Value.TimeOfDay <= quietTimeStop.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay > quietTimeStart.Value.TimeOfDay)) ||
                                ((mModeStart.Value.TimeOfDay <= quietTimeStart.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay >= quietTimeStop.Value.TimeOfDay)))
                            {
                                mmodeTimeQuietTimeEqual = true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        mmodeTimeQuietTimeEqual = false;
                        break;
                    }
            }
            return mmodeTimeQuietTimeEqual;
        }

        public bool Validate(object data, out string description)
        {                  
            if (MModeTimeEqualsQuietTime((MonitoredSqlServerConfiguration) data))
            {
                description =
                    "Maintenance Mode and Quiet time data collection are scheduled at the same time.  Quiet time data collection will not occur when the server is in maintenance mode.";
                return false;
            }

            description = string.Empty;
            return true;
        }
    }
}
