//------------------------------------------------------------------------------
// <copyright file="EnableQMDestinationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System;

    [Serializable]
    [Guid("7B60DCE5-EEB0-4B9B-A38D-16348D50948B")]
    [ProviderInfo(typeof(EnablePADestination), DestinationTypeLabel = "Enable the Prescriptive Analyzer to analyze server <a href=\"internal:PADestination{0}\">for selected categories</a>.")]
    [DisplayName("Enable Prescriptive Analysis Action Provider"), Description("Action provider that enables the Prescriptive Analysis on the monitored instance")]
    public class EnablePANotificationProviderInfo : NotificationProviderInfo
    {
        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override String Caption
        {
            get { return "PA Notification Provider"; }
        }

        public EnablePANotificationProviderInfo() : this(true)
        {
        }

        public EnablePANotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.EnablePANotificationProvider", enabled)
        {
        }

        public EnablePANotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            Id = info.Id;
            SetName(info.Name);
            Properties = info.Properties;
        }
    }
}
