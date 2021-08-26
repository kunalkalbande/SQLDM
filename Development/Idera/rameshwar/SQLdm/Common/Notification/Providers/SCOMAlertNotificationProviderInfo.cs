//------------------------------------------------------------------------------
// <copyright file="SCOMAlertNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("BBF105EE-D00B-4D18-8F74-20C519BE39F5")]
    [ProviderInfo(typeof(SCOMAlertDestination), DestinationTypeLabel = "Send to SCOM as Alert")]
    [DisplayName("Send Alert to SCOM Action Provider"), Description("Action provider that decide which alert to send as Alert and which to send as Event to SCOM")]
    public class SCOMAlertNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override String Caption
        {
            get { return "SCOM Alert Notification Provider"; }
        }

        public SCOMAlertNotificationProviderInfo() : this(true)
        {
        }

        public SCOMAlertNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.SCOMAlertNotificationProvider", enabled)
        {
        }

        public SCOMAlertNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            Id = info.Id;
            SetName(info.Name);
            Properties = info.Properties;
        }
    }
}
