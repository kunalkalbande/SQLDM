//------------------------------------------------------------------------------
// <copyright file="ProgramNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("966DEC7D-7FDB-4bb6-82F7-73821D990C67")]
    [ProviderInfo(typeof(ProgramDestination), DestinationTypeLabel = "Run the following program: <a href=\"internal:ProgramDestination{0}\">Program</a>")]
    [DisplayName("Program Action Provider"), Description("Action provider that runs a program on the SQLdm Management Service machine")]

    public class ProgramNotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Program Notification Provider";
            }
        }

        public ProgramNotificationProviderInfo() 
            : this(true)
        {
        }

        public ProgramNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.ProgramNotificationProvider", enabled)
        {
        }

        public ProgramNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            SetName(info.Name);
            this.Properties = info.Properties;
        }
    }
}
