//------------------------------------------------------------------------------
// <copyright file="SCOMEventNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("EA0BF79A-C370-4D24-A26B-AC5D1BE5EC49")]
    [ProviderInfo(typeof(SCOMEventDestination), DestinationTypeLabel = "Send to SCOM as Event")]
    [DisplayName("Send Event to SCOM Action Provider"), Description("Action provider that decide which alert to send as Event to SCOM")]
    public class SCOMEventNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override String Caption
        {
            get { return "SCOM Event Notification Provider"; }
        }

        public SCOMEventNotificationProviderInfo() : this(true)
        {
        }

        public SCOMEventNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.SCOMEventNotificationProvider", enabled)
        {
        }

        public SCOMEventNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            Id = info.Id;
            SetName(info.Name);
            Properties = info.Properties;
        }
    }
}
