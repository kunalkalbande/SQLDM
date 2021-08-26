//------------------------------------------------------------------------------
// <copyright file="EnableQMDestinationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("4F0F8D8D-54D6-4e6f-9CB6-1820C5574B6F")]
    [ProviderInfo(typeof(EnableQMDestination), DestinationTypeLabel = "Enable the Query Monitor <a href=\"internal:QMDestination{0}\">for this time period</a>")]
    [DisplayName("Enable Query Monitor Action Provider"), Description("Action provider that enables the Query Monitor on the monitored instance")]
    public class EnableQMNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override String Caption
        {
            get { return "QM Notification Provider"; }
        }

        public EnableQMNotificationProviderInfo() : this(true)
        {
        }

        public EnableQMNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.EnableQMNotificationProvider", enabled)
        {
        }

        public EnableQMNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            Id = info.Id;
            SetName(info.Name);
            Properties = info.Properties;
        }
    }
}
