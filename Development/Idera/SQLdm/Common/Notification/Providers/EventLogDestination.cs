//------------------------------------------------------------------------------
// <copyright file="EventLogDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable]
    public class EventLogDestination : NotificationDestinationInfo
    {        
        public EventLogDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(EventLogNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public EventLogDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
