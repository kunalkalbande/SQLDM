//------------------------------------------------------------------------------
// <copyright file="JobNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("30DD3BCB-F491-4fab-AB25-BB7C733B50F6")]
    [ProviderInfo(typeof(JobDestination), DestinationTypeLabel = "Run the following SQL Agent Job: <a href=\"internal:JobDestination{0}\">SQL Agent Job</a>")]
    [DisplayName("SQL Agent Job Action Provider"), Description("Action provider that runs an SQL Agent Job")]

    public class JobNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Job Notification Provider";
            }
        }

        public JobNotificationProviderInfo() 
            : this(true)
        {
        }

        public JobNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.JobNotificationProvider", enabled)
        {
        }

        public JobNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
            this.Properties = info.Properties;
        }
    }
}
