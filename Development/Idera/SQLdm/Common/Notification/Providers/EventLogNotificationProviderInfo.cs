//------------------------------------------------------------------------------
// <copyright file="EventLogNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("75B6B4A9-CB0F-4b81-A72C-219C78C520BB")]
    [ProviderInfo(typeof(EventLogDestination), DestinationTypeLabel = "Write an event to the Windows Event Log")]
    [DisplayName("EventLog Action Provider"), Description("Action provider that sends alerts to the Application Event Log")]
    public class EventLogNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Log Notification Provider";
            }
        }

        public EventLogNotificationProviderInfo() : this(true)
        {
        }

        public EventLogNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.EventLogNotificationProvider", enabled)
        {
        }

        public EventLogNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
//            this.Description = info.Description;
            this.Properties = info.Properties;
        }
    }
}
