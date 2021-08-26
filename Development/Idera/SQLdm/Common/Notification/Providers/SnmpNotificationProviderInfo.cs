//------------------------------------------------------------------------------
// <copyright file="SnmpNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using System;

    [Serializable]
    [Guid("398E48B4-3BBD-4bee-A2AB-E597F477DDEA")]
    [ProviderInfo(typeof(SnmpDestination), DestinationTypeLabel = "Send a Network Management (SNMP) Trap Message ")]
    [DisplayName("Snmp Action Provider"), Description("Action provider that sends SNMP trap messages")]
    public class SnmpNotificationProviderInfo : NotificationProviderInfo
    {
        private const string SnmpCommunityName = "SNMP Community Name";
        private const string NmsListeningPortForSnmpTraps = "NMS Listening port for SNMP traps";
        private const string HostNameIpAddressOfTheNms = "Host Name / IP Address of the NMS";

        public SnmpNotificationProviderInfo() : this(true)
        {
        }

        public SnmpNotificationProviderInfo(bool enabled) : base("Idera.SQLdm.ManagementService.Notification.Providers.SnmpNotificationProvider", enabled)
        {
        }
        
        public SnmpNotificationProviderInfo(NotificationProviderInfo info) : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            this.Name = info.Name;
            this.Properties = info.Properties;
        }

        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Snmp Notification Provider";
            }
        }

        [XmlAttribute]
        [Description(HostNameIpAddressOfTheNms), DefaultValue("")]
        [Auditable(HostNameIpAddressOfTheNms)]
        public string Address
        {
            get
            {
                NotificationProviderProperty property = GetProperty("Address");
                return (property == null) ? "" : property.Value.ToString();
            }
            set
            {
                this.SetProperty("Address", value);
            }
        }

        [XmlAttribute]
        [Description(NmsListeningPortForSnmpTraps), DefaultValue(162)]
        [Auditable(NmsListeningPortForSnmpTraps)]
        public int Port
        {
            get
            {
                NotificationProviderProperty property = GetProperty("Port");
                return (property == null) ? 162 : (int)property.Value;
            }
            set
            {
                this.SetProperty("Port", value);
            }
        }

        [XmlAttribute]
        [Description(SnmpCommunityName), DefaultValue("public")]
        [Auditable(SnmpCommunityName)]
        public string Community
        {
            get
            {
                NotificationProviderProperty property = GetProperty("Community");
                return (property == null) ? "public" : property.Value.ToString();
            }
            set
            {
                this.SetProperty("Community", value);
            }
        }

    }
}
