//------------------------------------------------------------------------------
// <copyright file="SqlNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("D29E6F2D-C95A-4782-8454-C8314C836051")]
    [ProviderInfo(typeof(SqlDestination), DestinationTypeLabel = "Run the following SQL Script: <a href=\"internal:SqlDestination{0}\">SQL Script</a>")]
    [DisplayName("SQL Script Action Provider"), Description("Action provider that runs a user written SQL script")]
    public class SqlNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Sql Notification Provider";
            }
        }

        public SqlNotificationProviderInfo() 
            : this(true)
        {
        }

        public SqlNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.SqlNotificationProvider", enabled)
        {
        }

        public SqlNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
            this.Properties = info.Properties;
        }
    }
}
