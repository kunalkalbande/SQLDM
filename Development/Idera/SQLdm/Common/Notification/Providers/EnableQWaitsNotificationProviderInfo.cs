//------------------------------------------------------------------------------
// <copyright file="EnableQWaitsDestinationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("2E1E62CB-30F8-413E-90CC-CADC1C514D66")]
    [ProviderInfo(typeof(EnableQWaitsDestination), DestinationTypeLabel = "Enable the Query Waits <a href=\"internal:QWaitsDestination{0}\">for this time period</a>")]
    [DisplayName("Enable Query Waits Action Provider"), Description("Action provider that enables the Query Waits on the monitored instance")]
    public class EnableQWaitsNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override String Caption
        {
            get { return "QWaits Notification Provider"; }
        }

        public EnableQWaitsNotificationProviderInfo() : this(true)
        {
        }

        public EnableQWaitsNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.EnableQWaitsNotificationProvider", enabled)
        {
        }

        public EnableQWaitsNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            Id = info.Id;
            SetName(info.Name);
            Properties = info.Properties;
        }
    }
}
