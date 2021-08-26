//------------------------------------------------------------------------------
// <copyright file="SCOMAlertDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Analysis;
    using System.Threading;
    using Events;
    [Serializable]
    public class SCOMAlertDestination : NotificationDestinationInfo
    {
     
        public SCOMAlertDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(SCOMAlertNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public SCOMAlertDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

       
        public override string ToString()
        {
            return string.Empty;
        }

    }
}
