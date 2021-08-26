//------------------------------------------------------------------------------
// <copyright file="PowerShellNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
//File added to support Alert Power Shell Provider
//SQLdm 10.1 Srishti Purohit
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("A215535A-8C67-44D5-BD0E-EB0BF238A8E6")]
    [ProviderInfo(typeof(PowerShellDestination), DestinationTypeLabel = "Run the following Power Shell Script: <a href=\"internal:PowerShellDestination{0}\">Power Shell Script</a>")]
    [DisplayName("Power Shell Action Provider"), Description("Action provider that runs a user written Power Shell script")]
    public class PowerShellNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Power Shell Notification Provider";
            }
        }

        public PowerShellNotificationProviderInfo() 
            : this(true)
        {
        }

        public PowerShellNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.PowerShellNotificationProvider", enabled)
        {
        }

        public PowerShellNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
            this.Properties = info.Properties;
        }
    }
}
