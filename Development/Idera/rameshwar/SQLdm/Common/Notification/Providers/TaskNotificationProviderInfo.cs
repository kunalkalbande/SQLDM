//------------------------------------------------------------------------------
// <copyright file="ODBCNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Idera.SQLdm.Common.Notification.Providers
{
    [Serializable]
    [Guid("24263D39-D8D7-475b-ABF6-97841A5E4772")]
    [ProviderInfo(typeof(TaskDestination), DestinationTypeLabel = "Create a new To Do for <a href=\"internal:TaskDestination{0}\">someone</a>")]
    [DisplayName("To Do Action Provider"), Description("Action provider that creates To Do items")]
    public class TaskNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Task Notification Provider";
            }
        }

        public TaskNotificationProviderInfo() 
            : this(true)
        {
        }

        public TaskNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.TaskNotificationProvider", enabled)
        {
        }

        public TaskNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
            this.Properties = info.Properties;
        }

    }
}
